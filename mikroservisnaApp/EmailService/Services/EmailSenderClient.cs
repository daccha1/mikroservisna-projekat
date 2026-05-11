using Azure.Core.GeoJson;
using Common.Saga_Contracts;
using Common.StrucniDogadjajDTO;
using EmailService.Models;
using Microsoft.Extensions.Configuration;
using Resend;
using System;
using System.Collections.Generic;
using System.Text;

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

		public async Task SendMessage(NotifyPosetilac notification)
		{
			if (resendClient == null)
			{
				Console.WriteLine("Resend client is not initialized.");
				return;
			}
			
			var response = await resendClient.EmailSendAsync(new EmailMessage
			{
				From = "david@dachadev.xyz",
				To = ["ilijazeljkovic1312@gmail.com", $"{notification.Email}"],
				Subject = "Dobrodošli na event!",
				HtmlBody = $"<p> Uspešno ste prijavljeni na event!  <strong> Vaš vaučer je: {notification.CorrelationId} </strong>!</p>",
			});

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
				From = "david@dachadev.xyz",
				To = ["nijedavid@gmail.com", "ilijazeljkovic1312@gmail.com" ],
				Subject = "Test MessageQueue aplikacije",
				HtmlBody = $"<p> Uspesno je kreiran objekat na servisu: DogadjajiService. <strong> {dtoObject} </strong>!</p>",
			});

		}

	}
}
