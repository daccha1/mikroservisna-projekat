using Common.StrucniDogadjajDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailService.Models
{
	public class ProcessedMessage
	{
		public int Id { get; set; }
		public string EventId { get; set; }
		public DateTime ProcessedTime { get; set; } = DateTime.UtcNow;
	}
}
