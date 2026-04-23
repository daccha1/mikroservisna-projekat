using System;
using System.Collections.Generic;
using System.Text;
using Resend;
using Microsoft.Extensions.Configuration;
using EmailService.Models;
using Common.StrucniDogadjajDTO;

namespace EmailService.Services
{
	public class EmailSenderClient
	{

		private static EmailSenderClient instance;
		public static EmailSenderClient Instance
		{
			get
			{
				if (instance == null) instance = new EmailSenderClient();
				return instance;
			}
		}
		public EmailSenderClient() { }


		private IResend resendClient;
		public void StartClient()
		{
			var config = new ConfigurationBuilder()
							.AddJsonFile("secrets.json", optional: false, reloadOnChange: true)
							.Build();

			string? apiKey = config["resendKey"];

			resendClient = ResendClient.Create(apiKey);
			
		}

		public async Task SendMessage(StrucniDogadjajRequestDTO dtoObject)
		{
			if(resendClient == null)
			{
				Console.WriteLine("Resend client is not initialized.");
				return;
			}

			var response = await resendClient.EmailSendAsync(new EmailMessage
			{
				From = "onboarding@resend.dev",
				To = "nijedavid@gmail.com",
				Subject = "Test MessageQueue aplikacije",
				HtmlBody = $"<p> Uspesno je kreiran objekat na servisu: DogadjajiService. <strong> {dtoObject} </strong>!</p>",
			});

		}

	}
}
