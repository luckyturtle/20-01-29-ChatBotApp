using ChatBotApp.Models.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.data
{
    public class AgentModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string LastMessage { get; set; }
        public string LastTime { get; set; }
        public string Avatar { get; set; }
        public string Roles { get; set; }
        public int UnReadCount { get; set; }
        public CustomerStatus Status { get; set; }
    }
}
