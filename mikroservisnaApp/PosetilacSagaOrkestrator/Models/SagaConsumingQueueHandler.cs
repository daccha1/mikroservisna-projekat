using Common.Saga_Contracts;
using PosetilacSagaOrkestrator.Data;
using System;
using System.Collections.Generic;
using System.Text;



namespace PosetilacSagaOrkestrator.Models
{
	public static class SagaConsumingQueueHandler
	{
		public static async void HandleGiftEvent(NotifyOrchestratorEvent e)
		{
			var dbSaga = new PosetilacOrkestratorDbContext();
			var saga = dbSaga.PosetilacSagaStates.Where(s => s.CorrelationId == e.CorrelationId).FirstOrDefault();

			if (saga == null)
			{
				return;
			}

			if (e.GiftStatus == GiftStatus.Created)
			{
				saga.State = SagaStates.Gifted;
				// spusti u outbox za notifikacije
				NotificationOutboxMessage notificationMsg = new()
				{
					CorrelationId = e.CorrelationId,
					Status = NotificationOutboxStatus.ForProcessing
				};

				await dbSaga.NotificationsOutboxMessages.AddAsync(notificationMsg);
				dbSaga.PosetilacSagaStates.Update(saga);
				await dbSaga.SaveChangesAsync();
			}
			else
			{
				saga.FailedReason = "Gift nije uspesno kreiran.";
				Console.WriteLine("Gift nije uspesno kreiran. Zaustavljanje Sage... ");
				dbSaga.PosetilacSagaStates.Update(saga);
				await dbSaga.SaveChangesAsync();
				return;
			}
		}

		internal static void HandleNotificationEvent(NotifyOrchestratorEvent e)
		{
			//
		}
	}
}
