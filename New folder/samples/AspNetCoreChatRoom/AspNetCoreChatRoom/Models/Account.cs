using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreChatRoom.Models
{
    public class Account
    {       
        public string Name { get; set; }     
        public string Password { get; set; }
    }
}
