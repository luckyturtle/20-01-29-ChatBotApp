
using Microsoft.AspNetCore.Mvc;
using AspNetCoreChatRoom.Models;
using System.Data.SqlClient;



namespace AspNetCoreChatRoom.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        

        //GET: Account
        [HttpGet]
        public IActionResult Index()
        {
            return View("InsertUserName");
        }
        
        void connectionString()
        {
            con.ConnectionString = @"Data Source=H-A-C\MSSQLSERVERR;Initial Catalog=master;Integrated Security=True";
        }

        [HttpPost]
        public IActionResult Login(Account acc)
        {
            connectionString();
            //con.Open();
            //com.Connection = con;
            //com.CommandText = "select * from [user] where name='" + acc.Name + "' and password='" + acc.Password + "'";

            //SqlDataReader dr = com.ExecuteReader();
            //if (dr.Read())
            {
                //con.Close();
                return View("Index",acc.Name);
            }
            //else
            //{
            //    con.Close();
            //    return View("Error");
           // }


        }
    
  //  [HttpGet]
   //    public IActionResult Index()
     //   {
      //      return View("InsertUserName");
      //  }

      //  [HttpPost]
      //  public IActionResult Index(string username)
       // {
       //     return View("Index", username);
       // }

     //   public IActionResult Error()
     //   {
      //      return View();
      //  }
    }
}
