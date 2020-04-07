using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBotApp.Models.data
{
    public class Customer
    {
        public string Id { get; set; }                              // Primary Key, NOT identiy
        public string Name { get; set; }                            // Name
        public string FullName { get; set; }                            // Name
        public string NickName { get; set; }                            // Name
        public string MobilePhone { get; set; }                     // Mobile phone
        public string Email { get; set; }
        public Byte[] Avatar { get; set; }
        public DateTime FirstActivity { get; set; }                 // Date and time of first activity - creation
        public DateTime LastActivity { get; set; }                  // Date and time of last activity

        public ICollection<ChattingLog> ChatMessages { get; }           // list of all Messages sent or receaved by this user

        public CustomerStatus Status { get; set; }                  // indicate who is talking with customer: Bot or Agent
    }
}
