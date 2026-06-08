using CertificationService.Data;
using CertificationService.Models;
using Common;
using Common.Saga_Contracts.Choreography;
using System;
using System.Collections.Generic;
using System.Text;

namespace CertificationService.Services
{
	public class CertificationService
	{
		public async Task<int> HandleCertificationRequest(CertificationRequested evt)
		{
			CertificationDbContext db = new();
			try
			{

				Certification certificate = new()
				{
					CertificationStatus = CertificationStatus.Certified,
					CertifiedAt = DateTime.UtcNow,
					CorrelationId = evt.CorrelationId
				};

				await db.Certifications.AddAsync(certificate);
				await db.SaveChangesAsync();

				return 1;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return -1;
			}
			finally
			{
				await db.DisposeAsync();
			}
		}

		public async Task HandleCreatedCertificate(Guid correlationId)
		{
			CertificationDbContext db = new();

			CertificationCreatedOutboxMessage msg = new()
			{
				CorrelationId = correlationId,
				CreatedAt = DateTime.UtcNow,
				Status = Status.NotProcessed
			};

			await db.CertificationsOutboxTable.AddAsync(msg);
			await db.SaveChangesAsync();

		}

		public async Task<CertificationFailed> HandleEmailFailed(CertificationCompleted evt)
		{
			CertificationFailed failMsg = new()
			{
				CorrelationId = evt.CorrelationId,
				CreatedAt = DateTime.UtcNow,
				FailType = FailType.EmailFail
			};
			return failMsg;
		}

		public async Task<CertificationFailed> HandleCertificationServiceFail(CertificationRequested evt)
		{
			CertificationFailed failMsg = new()
			{
				CorrelationId = evt.CorrelationId,
				CreatedAt = DateTime.UtcNow,
				FailType = FailType.CertificationServiceFail
			};
			return failMsg;
		}
	}
}
