using System;
using System.Collections.Generic;
using System.Text;

namespace EmailService.Models
{
	public class EmailMessageContent
	{
		public string From { get; set; } = "onboarding@resend.dev";
		public string To { get; set; }
		public string Subject { get; set; }
		public string HtmlBody { get; set; }
	}
}
