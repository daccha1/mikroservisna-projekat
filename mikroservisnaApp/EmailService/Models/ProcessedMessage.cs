using Common.StrucniDogadjajDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailService.Models
{
	public enum OperationEvent
	{
		Created,
		Read,
		Updated,
		Deleted,
	}

	public class ProcessedMessage
	{
		public int Id { get; set; }
		public string JsonObject { get; set; }
		public OperationEvent EventType { get; set; }
		public DateTime ProcessedTime { get; set; } = DateTime.UtcNow;
	}
}
