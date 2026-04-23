using System;
using System.Collections.Generic;
using System.Text;
using Resend;
using Microsoft.Extensions.Configuration;

namespace EmailService.Services
{
	public class EmailSenderClient
	{
		static IResend resend;
		public static void StartClient()
		{
			var config = new ConfigurationBuilder()
							.AddJsonFile("secrets.json", optional: false, reloadOnChange: true)
							.Build();

			string? apiKey = config["resendKey"];
			Console.WriteLine(apiKey);
			resend = ResendClient.Create(apiKey);
			
		}
	}
}
