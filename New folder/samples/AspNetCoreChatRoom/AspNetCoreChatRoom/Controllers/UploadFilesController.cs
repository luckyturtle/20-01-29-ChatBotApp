using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting; //*get RootPath


namespace AspNetCoreChatRoom.Controllers
{
    public class UploadFilesController : Controller
    {
       

            private readonly IHostingEnvironment _appEnvironment;

            public UploadFilesController(IHostingEnvironment appEnvironment)

            {

                //----< Init: Controller >----

                _appEnvironment = appEnvironment;

                //----</ Init: Controller >----

            }







            [HttpGet] //1.Load

            public IActionResult Index()

            {

                //--< Upload Form >--

                return View();

                //--</ Upload Form >--

            }





            [HttpPost] //Postback

            public async Task<IActionResult> Index(IFormFile file)

            {

                //--------< Upload_ImageFile() >--------

                //< check >

                if (file == null || file.Length == 0) return Content("file not selected");

                //</ check >



                //< get Path >

                string path_Root = _appEnvironment.WebRootPath;

                string path_to_Images = path_Root + "\\Home\\" + file.FileName;

                //</ get Path >



                //< Copy File to Target >

                using (var stream = new FileStream(path_to_Images, FileMode.Create))

                {

                    await file.CopyToAsync(stream);

                }

                //</ Copy File to Target >



                //< output >

                ViewData["FilePath"] = path_to_Images;

                return View();

                //</ output >

                //--------</ Upload_ImageFile() >--------

            }

        }

    }
