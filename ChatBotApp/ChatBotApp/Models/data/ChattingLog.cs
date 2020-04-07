using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBotApp.Models.data
{
    public class ChattingLog
    {
        public long Id { get; set; }
        public ChatMsgType Type { get; set; }                   // Type of Message
        public string Text { get; set; }                        // Text - in case type is text
        public string Filename { get; set; }                    // Filename - in case type is voice, image or other file format
        public MessageSource Source { get; set; }               // Indicates which role sent message: user / bot / agent
        public bool Read { get; set; }                          // Indicates if the message was read; Only used for messages destinated to Agents.
        public DateTime Time { get; set; }                      // Date and time message was sent
        public string CustomerId { get; set; }                  // User who sent or receved the message
        public string AgentId { get; set; }                     // In case message was sent by Agent, Agent ID is saved here
        /*public long Id { get; set; }
        public string QueryTxt { get; set; }
        public string AnswerTxt { get; set; }
        public string QueryAth { get; set; }
        public string AnswerAth { get; set; }
        public string QueryBy { get; set; }
        public DateTime QueryDate { get; set; }
        public string AnswerBy { get; set; }
        public DateTime AnswerDate { get; set; }*/
    }
}
