using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChatBotApp.Models.data;
using DBSetup;
using DBSetup.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.data;
using Newtonsoft.Json;
using WebPush;
using RichardSzalay.MockHttp;
using System.Net.Http;

namespace ChatBotApp.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;//Entity Instance for ChattingLog and Customer table
        private readonly UserManager<ApplicationUser> _userManager;//Admin and Agent Table Managenment
        [Obsolete]
        private readonly IWebHostEnvironment _environment;
        private readonly IAccountManager _accountManager;//Login Manager
        public ChatController(IAccountManager accountManager, IWebHostEnvironment iWebHostEnvironment, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _accountManager = accountManager;
            _environment = iWebHostEnvironment;
            _userManager = userManager;
            _context = context;
        }

        //[SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
        public IActionResult Index()
        {
            ViewData["current_userid"] = _userManager.GetUserId(User);
            ViewData["current_username"] = _userManager.GetUserName(User);
            bool isadmin = isAdministrator(ViewData["current_username"].ToString());
            ViewData["administrator"] = isadmin?"1":"0";
            
            var agents = getAgentList();
            ViewBag.agents = Json(agents);
            ViewBag.current_agent = null;
            if (isadmin)
            {
                foreach (AgentModel agent in agents)
                {
                    if (agent.Id.Equals(ViewData["current_userid"]))
                    {
                        ViewBag.current_agent = agent;
                        break;
                    }
                }
            }
            else {
                var avapath = "/assets-chatroom/images/avatars/default.png";
                IQueryable<ApplicationUser> query = _userManager.Users.Where(u => u.UserName.Equals(ViewData["current_username"].ToString()));
                var users = query.ToList();
                if (query.Count() > 0) { 
                    AgentModel agent = new AgentModel();
                    agent.Id = users[0].Id;
                    agent.Avatar = "NULL";// avapath;
                    //
                    if (users[0].JobTitle != null && (!users[0].JobTitle.Equals("")))
                    {
                        agent.Avatar = users[0].JobTitle;
                    }
                    agent.Name = users[0].UserName;
                    if (users[0].UserName == null || users[0].UserName.Equals("") || users[0].UserName.Equals("NULL"))
                    {
                        agent.Name = "&nbsp;";
                        if (agent.Avatar.Equals("NULL")) agent.Avatar = avapath;
                    }
                    else agent.Name = users[0].UserName;
                    agent.FullName = users[0].FullName;
                    agent.NickName = users[0].NickName;
                    agent.Email = users[0].Email;
                    agent.Phone = users[0].PhoneNumber;
                    agent.Roles = "agent";
                    agent.LastMessage = "";
                    agent.LastTime = "";
                    IQueryable<ChattingLog> agentQuery = _context.ChattingLog.Where(u => u.AgentId == agent.Id).OrderByDescending(u => u.Time);
                    if (agentQuery.Count() > 0)
                    {
                        var agentlog = agentQuery.First();
                        agent.LastMessage = (agentlog.Text.Equals("")) ? agentlog.Filename : agentlog.Text;
                        agent.LastTime = agentlog.Time.ToString();
                    }
                    if (agent.LastMessage.Equals("")) agent.LastMessage = "&nbsp;";
                    ViewBag.current_agent = agent;
                } 
            }
            ViewData["current_avatar"] = ViewBag.current_agent.Avatar;

            /*
            string TestPublicKey =
            @"BCvKwB2lbVUYMFAaBUygooKheqcEU-GDrVRnu8k33yJCZkNBNqjZj0VdxQ2QIZa4kV5kpX9aAqyBKZHURm6eG1A";

            string TestPrivateKey = @"on6X5KmLEFIVvPP3cNX9kE0OF6PV9TJQXVbnKU2xEHI";

            string TestGcmEndpoint = @"https://android.googleapis.com/gcm/send/";

            string TestFcmEndpoint =
                @"https://fcm.googleapis.com/fcm/send/efz_TLX_rLU:APA91bE6U0iybLYvv0F3mf6";
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(TestFcmEndpoint).Respond(HttpStatusCode.Created);

            var client = new WebPushClient(mockHttp.ToHttpClient());
            client.SetVapidDetails("mailto:example@example.com", TestPublicKey, TestPrivateKey);

            var subscription = new PushSubscription(TestFcmEndpoint, TestPublicKey, TestPrivateKey);

            client.SendNotification(subscription, "123");
            */
            return View();
        }
        /*
        *Getting Left Sidebar Body  
        * Now this is not used
        */
        [HttpGet]
        public ActionResult GetSideBarHtml() {
            var avapath = "/assets-chatroom/images/avatars/default.png";
            IQueryable<ApplicationUser> query = _userManager.Users.OrderBy(u => u.CreatedBy);
            var users = query.ToList();
            //string path = "";
            ViewBag.agents = new List<AgentModel>();
            foreach (ApplicationUser user in users)
            {
                AgentModel agent = new AgentModel();
                agent.Id = user.Id;
                //path = Directory.GetCurrentDirectory() + "\\wwwroot\\assets-chatroom\\images/avatars/default.png";
                //string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "default" + ".*", SearchOption.AllDirectories).ToArray();
                if (user.AvatarImage == null)
                {
                    agent.Avatar = avapath;
                }
                else
                    agent.Avatar = avapath;//user.AvatarImage;
                agent.Name = user.UserName;
                agent.NickName = user.FullName;
                agent.LastMessage = "";
                agent.LastTime = "";
                IQueryable<ChattingLog> agentQuery = _context.ChattingLog.Where(u => u.AgentId == agent.Id).OrderByDescending(u => u.Time);
                if (agentQuery.Count() > 0)
                {
                    var agentlog = agentQuery.First();
                    agent.LastMessage = agentlog.Text.Equals("")?agentlog.Filename:agentlog.Text;
                    agent.LastTime = agentlog.Time.ToString();
                }
                if (agent.LastMessage.Equals("")) agent.LastMessage = "New agent";
                ViewBag.agents.Add(agent);
            }
            return View();
        }
        /*
        *Getting Agent and Customer List Json
        */
        [HttpPost]
        public async Task<JsonResult> GetSideBarJsonAsync(string agent,int flag)
        {
            List<ChattingLogModel> chattingLog=new List<ChattingLogModel>();
            try
            {
                string session_agent = HttpContext.Session.GetString("last_user");
                if (agent.Equals(session_agent))
                {
                    IQueryable<ChattingLogModel> Query;
                    if (flag == 1)
                    {//chatting customer <-> bot & agent
                        Query = (from a in _context.Set<ChattingLog>().Where(u => u.CustomerId.Equals(agent)).OrderByDescending(u => u.Time)
                                 join b in _context.Set<ApplicationUser>()
                                 on a.AgentId equals b.Id into g
                                 from b in g.DefaultIfEmpty()
                                 select new ChattingLogModel { chattingLog = a, CustomerName = "", AgentName = b.UserName });
                    }
                    else
                    { //agent -> customer
                        Query = (from a in _context.Set<ChattingLog>().Where(u => u.AgentId.Equals(agent) && (u.Source == MessageSource.Agent || u.Source == MessageSource.Customer)).OrderByDescending(u => u.Time)
                                 join b in _context.Set<Customer>() on a.CustomerId equals b.Id
                                 select new ChattingLogModel { chattingLog = a, CustomerName = b.Name, AgentName = "" });
                    }
                    //if (Query.Count() > 0) if (!HttpContext.Session.GetString("lastLogId").Equals(Query.Last().chattingLog.Id))
                    
                    int i=0;
                    chattingLog = await Query.ToListAsync();
                    var lastlogid = HttpContext.Session.GetString("last_logid");
                    if (!lastlogid.Equals("null"))
                    {
                        for (; i < chattingLog.Count; i++)
                        {
                            if (lastlogid.Equals(chattingLog[i].chattingLog.Id.ToString())) break;
                        }
                        Query = Query.Take(i);
                        Query = Query.OrderBy(u => u.chattingLog.Time);
                        chattingLog = await Query.ToListAsync();
                        if(Query.Count()>0)HttpContext.Session.SetString("last_logid", Query.Last().chattingLog.Id.ToString());
                    }
                }
                else{
                    //HttpContext.Session.SetString("last_user", agent);
                    //HttpContext.Session.SetString("lastLogId", Query.Last().chattingLog.Id.ToString());
                }
            }
            catch (Exception) { }

            return Json(new { 
                agent=getAgentList(),
                customer=getCustomerList(),
                data = JsonConvert.SerializeObject(chattingLog)
            });
        }
        /*
        *Getting right chattingroom content
        * <agent>       current selected agent
        * <flag>        agent/customer flag
        * <page>        page number
        * <oageSize>    page size
        * <lastLogId>   last chatting history record id
        */
        [HttpPost]
        public async Task<ActionResult> GetChatRoomLogData(string agent, int flag, int page, int pageSize, string lastLogId)
        {
            if (lastLogId == null) lastLogId = "";
            IQueryable<ChattingLogModel> Query;

            if (flag == 1)
            {//chatting customer <-> bot & agent
                Query = (from a in _context.Set<ChattingLog>().Where(u => u.CustomerId.Equals(agent)).OrderByDescending(u => u.Time)
                           join b in _context.Set<ApplicationUser>() 
                           on a.AgentId equals b.Id into g
                           from b in g.DefaultIfEmpty()
                         select new ChattingLogModel { chattingLog = a, CustomerName = "", AgentName = b.UserName });
            }
            else { //agent -> customer
                Query = (from a in _context.Set<ChattingLog>().Where(u => u.AgentId.Equals(agent) && (u.Source == MessageSource.Agent|| u.Source == MessageSource.Customer)).OrderByDescending(u => u.Time)
                           join b in _context.Set<Customer>() on a.CustomerId equals b.Id
                           select new ChattingLogModel { chattingLog = a, CustomerName = b.Name, AgentName = "" });
            }
            var chattingLog = await Query.ToListAsync();
            int i = 0;
            if (chattingLog.Count > 0) { 
                if (lastLogId.Equals("")) lastLogId = chattingLog[0].chattingLog.Id.ToString();
                for (; i < chattingLog.Count; i++) {
                    if (lastLogId.Equals(chattingLog[i].chattingLog.Id.ToString())) break;
                }
            }

            HttpContext.Session.SetString("last_user", agent);
            HttpContext.Session.SetString("last_logid", lastLogId);

            if (page > 0) Query = Query.Skip((page - 1) * pageSize+i);
            if (pageSize != -1) Query = Query.Take(pageSize);
            Query = Query.OrderBy(u => u.chattingLog.Time);
            chattingLog = await Query.ToListAsync();

            return Json(new {
                data = JsonConvert.SerializeObject(chattingLog),
                count = chattingLog.Count,
                lastLogId = lastLogId
            });
        }
        /*
        *adding a chatting log record
        * <attachedFile> attached file object
        * <queryTo>      to target id
        * <queryDate>    adding record's datetime
        * <queryText>    adding record's chatting text
        */
        [HttpPost("UploadFiles")]
        public async Task<ActionResult> ChattingEventAsync(List<IFormFile> attachedFile, string queryTo, string queryDate, string queryText)
        {
            ChatMsgType chatMsgType = ChatMsgType.Text;
            string filename = "";
            if (attachedFile.Count > 0)
            {
                chatMsgType = ChatMsgType.File;
                long size = attachedFile.Sum(f => f.Length);
                //tring filePath = "/attach/";// "D:/BotAppAttachedFiles/";
                string filePath = Path.Combine(_environment.WebRootPath, "attach");// + $@"\{newFileName}";
                if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }
                filename = attachedFile[0].FileName;
                filePath += $@"\{filename}";
                if (filename.ToLower().EndsWith("ogg") || filename.ToLower().EndsWith("wma") || filename.ToLower().EndsWith("mp3")) chatMsgType = ChatMsgType.Voice;
                if (filename.ToLower().EndsWith("png") || filename.ToLower().EndsWith("jpg") || filename.ToLower().EndsWith("bmp") || filename.ToLower().EndsWith("gif")) chatMsgType = ChatMsgType.Image;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachedFile[0].CopyToAsync(stream);
                }
            }
            
            try
            {
                var queryDateTime= DateTime.ParseExact(queryDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                var chatlog = new ChattingLog
                {
                    //Id = ,
                    AgentId = _userManager.GetUserId(User),
                    Time =queryDateTime,
                    Text = queryText,
                    Filename = filename,
                    CustomerId = queryTo,
                    Read=false,
                    Type=chatMsgType,
                    Source=MessageSource.Agent
                };
                await _context.ChattingLog.AddAsync(chatlog);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("last_user", queryTo);
                HttpContext.Session.SetString("last_logid", chatlog.Id.ToString());
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            return Json(new
            {
                //answerText = answerText,
                //answerAth = answerFile,
                //answerDate = answerDate,
                QueryAth = filename
            }); 
        }
        /*
        *deleting a selected agent
        * <id> selected agent's id
        */
        [HttpPost]
        public async Task<ActionResult> DeleteAgentData(string id)
        {
            try
            {
                ApplicationUser cuser = await _userManager.FindByIdAsync(id);
                
                _context.Remove(cuser);
                _context.SaveChanges();
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            return Json(new
            {
                error = ""
            });
        }
        /*
        *updateing a selected agent profle information
        * <avatar>    vatar image file object
        * <id>        selected agent id
        * <fullname>  fullname
        * <nickname>  nickname is name displayed into UI
        * <username>  login user name
        */
        [HttpPost]
        public async Task<ActionResult> EditAgentDataAAA(List<IFormFile> avatar, string id, string fullname, string nickname, string username, string reset_password)//, byte[] avatar_blob)//, string password)
        {
            string filePath = "";
            string filename = "";
            //byte[] avatar_data = null;
            try
            {
                if (avatar.Count > 0)
                {
                    long size = avatar.Sum(f => f.Length);
                    filename = $@"assets-chatroom\images\avatars\";
                    filePath = Path.Combine(_environment.WebRootPath, filename);
                    if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }
                    filename += $"{id}" + Path.GetExtension(avatar[0].FileName);
                    filePath += $"{id}" + Path.GetExtension(avatar[0].FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatar[0].CopyToAsync(stream);
                    }
                    //Image image = new Image();

                    /*
                    System.IO.Stream fs = avatar[0].OpenReadStream();
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                    avatar_data = System.Text.Encoding.UTF8.GetBytes(Convert.ToBase64String(bytes));
                    //avatar_data = System.Text.Encoding.UTF8.GetBytes(Convert.ToBase64String(ReadToEnd(avatar[0].OpenReadStream())));
                    */
                }
                //ApplicationUser cuser = await _userManager.GetUserAsync(User);
                ApplicationUser cuser = await _userManager.FindByIdAsync(id);
                ApplicationUser user = new ApplicationUser
                {
                    Id = id,
                    UserName = username,
                    PasswordHash = cuser.PasswordHash,
                    NormalizedUserName = username.ToUpper(),
                    FullName = fullname,
                    NickName = nickname,
                    IsEnabled = true,
                    JobTitle = filename
                    //AvatarImage = avatar_data//avatar_blob
                };
                _context.Entry(cuser).CurrentValues.SetValues(user);
                _context.Entry(cuser).State = EntityState.Modified;
                _context.SaveChanges();

                if (reset_password.Equals("on"))
                {
                    cuser = await _userManager.FindByIdAsync(id);
                    await _userManager.RemovePasswordAsync(cuser);
                    await _userManager.AddPasswordAsync(cuser, "tempP@ss123");
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            return Json(new
            {
                error = "",
                name = username,
                avatar = filename
            });
        }
        /*
        *updateing a selected customer profle information
        * <avatar>    vatar image file object
        * <id>        selected agent id
        * <fullname>  fullname
        * <nickname>  nickname is name displayed into UI
        * <username>  login user name
        */
        [HttpPost]
        public async Task<ActionResult> EditCustomerDataAAA(List<IFormFile> avatar, string id, string fullname, string nickname, string username, string reset_password)//, byte[] avatar_blob)//, string password)
        {
            string filePath = "";
            string filename = "";
            //byte[] avatar_data = null;
            try
            {
                if (avatar.Count > 0)
                {
                    long size = avatar.Sum(f => f.Length);
                    filename = $@"assets-chatroom\images\avatars\";
                    filePath = Path.Combine(_environment.WebRootPath, filename);
                    if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }
                    filename += $"{id}" + Path.GetExtension(avatar[0].FileName);
                    filePath += $"{id}" + Path.GetExtension(avatar[0].FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatar[0].CopyToAsync(stream);
                    }
                }

                IQueryable<Customer> Query = _context.Customer.Where(u => u.Id.Equals(id));
                if (Query.Count() > 0)
                {
                    Customer cuser = await _context.Customer.FirstAsync();
                    cuser.Name = username;
                    cuser.NickName = nickname;
                    cuser.FullName = fullname;
                    //cuser.Avatar = filename;
                    _context.Entry(cuser).CurrentValues.SetValues(cuser);
                    _context.Entry(cuser).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            return Json(new
            {
                error = "",
                name = username,
                avatar = filename
            });
        }
        /*
        *adding a new agent with profle information
        * <avatar>    vatar image file object
        * <id>        agent id
        * <fullname>  fullname
        * <nickname>  nickname is name displayed into UI
        * <username>  login user name
        */
        public async Task<string> AddAgentDataBBB(List<IFormFile> avatar, string fullname, string nickname, string username, string password)//, byte[] avatar_blob)
        {
            String filePath = "";
            string filename = "";
            if (avatar.Count > 0)
            {
                long size = avatar.Sum(f => f.Length);
                filename = $@"assets-chatroom\images\avatars\";
                filePath = Path.Combine(_environment.WebRootPath, filename);
                if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }
                filename+= $"{DateTime.Now.Year}1{DateTime.Now.Month}1{DateTime.Now.Day}1{DateTime.Now.Hour}1{DateTime.Now.Minute}1{DateTime.Now.Second}" + Path.GetExtension(avatar[0].FileName); 
                filePath += $"{DateTime.Now.Year}1{DateTime.Now.Month}1{DateTime.Now.Day}1{DateTime.Now.Hour}1{DateTime.Now.Minute}1{DateTime.Now.Second}" + Path.GetExtension(avatar[0].FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar[0].CopyToAsync(stream);
                }
            }
            try
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = username,
                    NormalizedUserName = username.ToUpper(),
                    FullName = fullname,
                    NickName = nickname,
                    IsEnabled = true,
                    JobTitle= filename
                    //AvatarImage = avatar_data//avatar_blob
                };
                //await _userManager.CreateAsync(user, password);
                //await _userManager.AddToRoleAsync(user, "user");
                
                var res=await _accountManager.CreateUserAsync(user, new string[] { "agent" }, password);
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            return "ok";
        }
        /*
        *converting image stream to byte array
        * <stream> stream object
        */
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
        /*
        *converting image stream to byte array
        * <input> stream object
        */
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        /*
        *converting image byte array to base64
        * <input> stream object
        */
        public string getImageFromBytes(byte[] input)
        {
            string res=Convert.ToBase64String(input);

            var filename = $@"assets-chatroom\images\avatars\";
            string filePath = Path.Combine(_environment.WebRootPath, filename);
            if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }
            filePath += "anca.jpg";
            System.IO.File.WriteAllBytes(filePath, input);
            return $"data:image/png;base64, {res}";
        }
        /*
        *getting agent list from user table
        */
        public List<AgentModel> getAgentList()
        {
            var avapath = "/assets-chatroom/images/avatars/default.png";
            List<AgentModel> res = new List<AgentModel>();
            IQueryable<ApplicationUser> query = _userManager.Users.OrderBy(u => u.CreatedBy);
            if (!isAdministrator(_userManager.GetUserName(User))) return res;
                //query = _userManager.Users.Where(u=>u.UserName.Equals(_userManager.GetUserName(User)));
            var users = query.ToList();
            foreach (ApplicationUser user in users)
            {
                AgentModel agent = new AgentModel();
                agent.Id = user.Id;

                //path = Directory.GetCurrentDirectory() + "\\wwwroot\\assets-chatroom\\images/avatars/default.png";
                //string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "default" + ".*", SearchOption.AllDirectories).ToArray();
                agent.Avatar = "NULL";// avapath;
                if (user.AvatarImage != null)
                {
                //    agent.Avatar = user.AvatarImage;
                }
                else
                {
                    //agent.Avatar = "data:image/png;base64," + Convert.ToBase64String(user.AvatarImage, 0, user.AvatarImage.Length);
                    //agent.Avatar = getImageFromBytes(user.AvatarImage);//user.AvatarImage;
                }
                if (user.JobTitle != null&& (!user.JobTitle.Equals("")))
                {
                    agent.Avatar = user.JobTitle;
                }
                if (user.UserName == null || user.UserName.Equals("") || user.UserName.Equals("NULL"))
                {
                    agent.Name = "&nbsp;";
                    if (agent.Avatar.Equals("NULL")) agent.Avatar = avapath;
                }
                else agent.Name = user.UserName;
                agent.FullName = user.FullName;
                agent.NickName = user.NickName;
                agent.Email = user.Email;
                agent.Phone = user.PhoneNumber;
                agent.Roles = isAdministrator(user.UserName)? "administrator" : "agent";
                agent.LastMessage = "&nbsp;";
                agent.LastTime = "";
                IQueryable<ChattingLog> agentQuery = _context.ChattingLog.Where(u => u.AgentId == agent.Id).OrderByDescending(u => u.Time);
                if (agentQuery.Count() > 0)
                {
                    var agentlog = agentQuery.First();
                    agent.LastMessage = getLastMessageText(agentlog.Text,agentlog.Filename);
                    agent.LastTime = agentlog.Time.ToString("MM/dd/yyyy HH:mm:ss tt");
                }
                res.Add(agent);
            }
            return res;
        }
        /*
         *check administrator 
         */
        private bool isAdministrator(string username) {
            IList<ApplicationUser> adminusers = _userManager.GetUsersInRoleAsync("administrator").Result;
            foreach (ApplicationUser adminuser in adminusers)
            {
                if (adminuser.UserName.Equals(username))
                {
                    return true;
                }
            }
            return false;
        }
        /*
        *getting customer list from customer table
        */
        public List<AgentModel> getCustomerList()
        {
            var avapath = "/assets-chatroom/images/avatars/default.png";
            IQueryable<Customer> query = _context.Customer.OrderByDescending(u => u.Status).ThenByDescending(u => u.LastActivity);
            var users = query.ToList();
            List<AgentModel> res = new List<AgentModel>();
            foreach (Customer user in users)
            {
                AgentModel agent = new AgentModel();
                agent.Id = user.Id;
                if (user.Avatar == null) agent.Avatar = "NULL";//avapath;
                if (user.Name == null || user.Name.Equals("") || user.Name.Equals("NULL"))
                {
                    agent.Name = "&nbsp;";
                    if (agent.Avatar.Equals("NULL")) agent.Avatar = avapath;
                }
                else agent.Name = user.Name;
                agent.FullName = user.FullName;
                agent.NickName = user.NickName;
                agent.Email = user.Email;
                agent.Phone = user.MobilePhone;
                agent.Roles = "customer";
                agent.LastMessage = "&nbsp;";
                agent.LastTime = "";
                agent.Status = user.Status;
                IQueryable<ChattingLog> agentQuery = _context.ChattingLog.Where(u=>u.CustomerId.Equals(user.Id)).OrderByDescending(u => u.Time);
                if (agentQuery.Count() > 0)
                {
                    var agentlog = agentQuery.First();
                    agent.LastMessage = getLastMessageText(agentlog.Text,agentlog.Filename);
                    agent.LastTime = agentlog.Time.ToString("MM/dd/yyyy HH:mm:ss tt");
                }
                agent.UnReadCount = agentQuery.Where(u => u.Read == false).Count();
                res.Add(agent);
            }
            return res;
        }

        /**
         * saving webpush subscribe  to user table
         */
        [HttpPost]
        public async Task<ActionResult> SaveSubscribeIdFromWebpush(string sid) {
            try {
                ApplicationUser cuser = await _userManager.GetUserAsync(User);
                cuser.SubscribeId = sid;
                _context.Entry(cuser).CurrentValues.SetValues(cuser);
                _context.Entry(cuser).State = EntityState.Modified;
                _context.SaveChanges();
            } catch (Exception) { }
            return Json(new
            {
                error = ""
            });
        }
        /**
         * convert long text to kindly view text in left panel's last message
        */
        public string getLastMessageText(string s,string f) {
            if (s == null || s.Equals("") || s.Equals("NULL")) return (f!=null&&!f.Equals(""))?f:"&nbsp;";
            var len = 35;
            var arr = s.Split("\n");
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                if (arr[i].Equals("")) continue;
                s = arr[i];
                if (s.Length > len) s = s.Substring(0, len - 3) + "...";
                return s;
            }
            return s;
        }
        /*
        *error page's action
        */
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}