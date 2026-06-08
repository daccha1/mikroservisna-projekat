using CertificationService.Data;
using CertificationService.Services.RabbitMQ;
using Common.Saga_Contracts.Choreography;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CertificationService.Services
{
	public static class DispatcherService
	{


		public async static Task DispatchCreatedOutboxMessage()
		{
			while (true)
			{
				CertificationDbContext db = new();

				var message = await db.CertificationsOutboxTable.Where(msg => msg.Status == Common.Status.NotProcessed).FirstOrDefaultAsync();

				if (message == null)
				{
					await Task.Delay(2000);
					continue;
				}

				CertificationCreated createdMsg = new()
				{
					CorrelationId = message.CorrelationId,
					CreatedAt = DateTime.UtcNow
				};


				await MQClient.Instance.SendMessage("certification-created", JsonSerializer.Serialize<CertificationCreated>(createdMsg));

				message.Status = Common.Status.Processed;
				db.CertificationsOutboxTable.Update(message);
				await db.SaveChangesAsync();

				await Task.Delay(2000);
			}

		}


		public static void DispatchFailedMessage()
		{

		}


	}
}
