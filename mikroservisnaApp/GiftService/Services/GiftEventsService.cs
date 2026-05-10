using Common.Saga_Contracts;
using GiftService.Contracts;
using GiftService.Models;

namespace GiftService.Services
{
	public interface IGiftEventsService
	{
		public Task HandleGiftCreation(PosetilacCreated posetilac);
	}

	public class GiftEventsService : IGiftEventsService
	{
		private IGift _repository;
		public GiftEventsService(IGift repo)
		{
			_repository = repo;
		}

		public async Task HandleGiftCreation(PosetilacCreated posetilac)
		{
			// kreiramo gift
			// to se prosledjuje repository-ju

			GiftType prirucnikTip;

			switch (posetilac.Interesovanje)
			{
				case "Web development":
					prirucnikTip = GiftType.WebDevelopmentPDF;
					break;
				case "Machine Learning":
					prirucnikTip = GiftType.MachineLearningPDF;
					break;
				case "Mikroservisi":
					prirucnikTip = GiftType.DistributedSystemsPDF;
					break;
				case "Vestacka Inteligencija":
					prirucnikTip = GiftType.MachineLearningPDF;
					break;
				case "Cyber security":
					prirucnikTip = GiftType.CyberSecurityPDF;
					break;
				default:
					prirucnikTip = GiftType.WebDevelopmentPDF;
					break;
			}


			Gift g = new()
			{
				CorrelationId = posetilac.CorrelationId,
				Instrukcije = "Test: doći na lokaciju u 20:00h",
				Interesovanje = posetilac.Interesovanje,
				Prirucnik = prirucnikTip,
				Vaucer = posetilac.CorrelationId
			};

			GiftCreatedOutboxMessage outboxMsg = new()
			{
				CorrelationId = posetilac.CorrelationId,
				CreatedAt = DateTime.UtcNow
			};

			await _repository.CreateGift(g);
			await _repository.CreateGiftOutboxMessage(outboxMsg);
		}

	}
}
