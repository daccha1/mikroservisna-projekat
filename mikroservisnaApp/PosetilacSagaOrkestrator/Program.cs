using Common.Saga_Contracts;
using PosetilacSagaOrkestrator.Data;
using PosetilacSagaOrkestrator.Models;
using PosetilacSagaOrkestrator.Services.MQ_Container;

namespace PosetilacSagaOrkestrator
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			var _ = Task.Run(() => MessageDispatcher.DispatchGiftPosetilacOutboxMessage());

			// uhvati poruku da je kreiran posetilac
			// kreiramo inicijalni state, outbox state, i bg workera
			// publishujemo na GiftService, generise se gift vraca na queue
			// uhvatimo ponovo poruku, izmenimo state preko CorrelationId-a
			// tada publishujemo poruku na EmailService
			// u emailService u mejlu dovucemo taj pdf, i saljemo i njega
			Console.WriteLine("Pokrenuta SAGA konzola.");
			using var mqClient = new MQClient();
			
			await mqClient.StartClient();
			
			await mqClient.Subscribe<PosetilacCreated>("events.orch.pos-creation", async (e) => {
				Console.WriteLine(" -- kreiran je novi posetilac -- ");

				// saga: da li postoji vec

				// ako ne:
				//		kreiraj state
				//		pusti outbox da ide to dalje
				//		sacuvaj promene

				var dbSaga = new PosetilacOrkestratorDbContext();

				var saga = dbSaga.PosetilacSagaStates.Where(s => s.CorrelationId == e.CorrelationId).FirstOrDefault();

				if(saga != null)
				{
					return;
				}

				PosetilacSagaState sagaState = new()
				{
					CorrelationId = e.CorrelationId,
					CreatedAt = DateTime.UtcNow,
					State = SagaStates.Created,
					Interesovanje = e.Interesovanje
				};

				GiftOutboxMessage outboxMsg = new()
				{
					CorrelationId = e.CorrelationId,
					CreatedAt = DateTime.UtcNow,
					Status = GiftOutboxStatus.ForProcessing,
				};

				await dbSaga.PosetilacSagaStates.AddAsync(sagaState);
				await dbSaga.GiftsOutboxMessages.AddAsync(outboxMsg);

				await dbSaga.SaveChangesAsync();

			});

			await mqClient.Subscribe<CreatedGift>("events.orch.consume-queue", async (e) => {
				Console.WriteLine(" STIGAO ODGOVOR SA GiftService ");

				var dbSaga = new PosetilacOrkestratorDbContext();

				var saga = dbSaga.PosetilacSagaStates.Where(s => s.CorrelationId == e.CorrelationId).FirstOrDefault();

				if (saga == null)
				{
					return;
				}

				if(e.GiftStatus == GiftStatus.Created)
				{
					saga.State = SagaStates.Gifted;
				}
				else
				{
					saga.FailedReason = "Gift nije uspesno kreiran.";
					Console.WriteLine("Gift nije uspesno kreiran. Zaustavljanje Sage... ");
					dbSaga.PosetilacSagaStates.Update(saga);
					await dbSaga.SaveChangesAsync();

					return;
				}
				

				dbSaga.PosetilacSagaStates.Update(saga);
				await dbSaga.SaveChangesAsync();

			});

			Thread.Sleep(Timeout.Infinite);
		}
	}
}
