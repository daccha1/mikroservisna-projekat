using CertificationService.Data;
using CertificationService.Models;
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


			db.DisposeAsync();

		}
	}
}
