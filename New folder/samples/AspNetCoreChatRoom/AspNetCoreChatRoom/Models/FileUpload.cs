using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreChatRoom.Models
{
    public class FileUpload
    {
        public IFormFile Photo { get; set; }
    }
}
