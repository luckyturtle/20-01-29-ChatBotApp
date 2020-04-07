using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MrBot.Models
{
    // Customer status says who the customer is talking to: bot / wating transfer / agent
    public enum CustomerStatus
    {
        TalkingToBot,
        WatingForAgent,
        TalkingToAgent,
    }
    // User who connects to send and receave messages
    public class Customer
    {
        [Key]                                                   
        public string Id { get; set; }                              // Primary Key, NOT identiy
        public string Name { get; set; }                            // Name
        public string MobilePhone { get; set; }                     // Mobile phone
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime FirstActivity { get; set; }                 // Date and time of first activity - creation

        [DataType(DataType.Date)]
        public DateTime LastActivity { get; set; }                  // Date and time of last activity

        public ICollection<Message> ChatMessages { get; }           // list of all Messages sent or receaved by this user

        public CustomerStatus Status { get; set; }                  // indicate who is talking with customer: Bot or Agent
    }
}
