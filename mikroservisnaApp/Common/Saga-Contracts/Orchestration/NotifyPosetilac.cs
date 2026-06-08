using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Common.Saga_Contracts
{
	public class NotifyPosetilac
	{
		public Guid Id { get; set; }
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public string Email { get; set; } = "nijedavid@gmail.com";
	}
}
