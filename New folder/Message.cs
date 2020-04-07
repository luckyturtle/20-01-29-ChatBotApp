using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MrBot.Models
{
	public enum MessageSource           // Indicates which role sent message: customer, bot, agent
	{
		Bot,								
		Customer,
		Agent
	}
	public enum ChatMsgType				// Type of Message
	{
		Text,							// Text
		Voice,							// Voce - audio Ogg/Wav
		Image,							// Images - Gif, Jpg, Png
		File							// Other files
	}
	public class Message				// Chat Message
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string Id { get; set; }							// Primary Key, Indentity ( auto generated )
		public ChatMsgType Type { get; set; }					// Type of Message
		public string Text { get; set; }						// Text - in case type is text
		public string Filename { get; set; }					// Filename - in case type is voice, image or other file format
		public MessageSource Source { get; set; }               // Indicates which role sent message: user / bot / agent
		public bool Read { get; set; }							// Indicates if the message was read; Only used for messages destinated to Agents.

		[DataType(DataType.Date)]
		public DateTime Time { get; set; }                      // Date and time message was sent

		[ForeignKey("Id")]
		public string CustomerId { get; set; }                  // User who sent or receved the message

		[ForeignKey("Id")]
		public string AgentId { get; set; }                     // In case message was sent by Agent, Agent ID is saved here

	}
}
