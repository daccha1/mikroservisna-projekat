using Common.Saga_Contracts;
using GiftService.Contracts;
using GiftService.Models;

namespace GiftService.Services
{
	public interface IGiftEventsService
	{
		Task ExecuteCompensation(Guid correlationId);
		public Task HandleGiftCreation(PosetilacCreated posetilac);
	}

	public class GiftEventsService : IGiftEventsService
	{
		private IGift _repository;
		public GiftEventsService(IGift repo)
		{
			_repository = repo;
		}

		public Task ExecuteCompensation(Guid correlationId)
		{
			var result = _repository.RemoveGift(correlationId);
			return result;
		}

		public async Task HandleGiftCreation(PosetilacCreated posetilac)
		{

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
					prirucnikTip = GiftType.None;
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
				CreatedAt = DateTime.UtcNow,
			};

			if (prirucnikTip == GiftType.None)
			{
				outboxMsg.SuccessfulCreation = false;
			}
			else
			{
				outboxMsg.SuccessfulCreation = true;
			}

			await _repository.CreateGift(g);
			await _repository.CreateGiftOutboxMessage(outboxMsg);
		}

	}
}
