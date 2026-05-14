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

				TransactionConfirmationOutboxMessage transactionMsg = new()
				{
					CorrelationId = e.CorrelationId,  
					TransactionStatus = TransactionStatus.Failed,
					MessageStatus = TransactionMessageStatus.ForProcessing,
					FailedService = FailedService.Gift
				};

				await dbSaga.TransactionConfirmationOutboxMessages.AddAsync(transactionMsg);
				dbSaga.PosetilacSagaStates.Update(saga);
				await dbSaga.SaveChangesAsync();
				return;
			}
		}

		public static async void HandleNotificationEvent(NotifyOrchestratorEvent e)
		{
			Console.WriteLine($"Notification status: {e.EventType} -- {e.GiftStatus} -- {e.NotificationStatus} -- {e.CorrelationId}" );

			var dbSaga = new PosetilacOrkestratorDbContext();
			var saga = dbSaga.PosetilacSagaStates.Where(s => s.CorrelationId == e.CorrelationId).FirstOrDefault();

			if (saga == null)
			{
				return;
			}

			if (e.NotificationStatus == NotificationStatus.Sent)
			{
				saga.State = SagaStates.Notified;
				// spusti u outbox za notifikacije
				TransactionConfirmationOutboxMessage transactionMsg = new()
				{
					CorrelationId = e.CorrelationId,
					TransactionStatus = TransactionStatus.Successful,
					MessageStatus = TransactionMessageStatus.ForProcessing
				};

				await dbSaga.TransactionConfirmationOutboxMessages.AddAsync(transactionMsg);
				dbSaga.PosetilacSagaStates.Update(saga);
				await dbSaga.SaveChangesAsync();
			}
			else
			{
				saga.FailedReason = "Notifikacija nije poslata posetiocu.";
				Console.WriteLine("Notifikacija nije uspešna. Zaustavljanje Sage... ");

				TransactionConfirmationOutboxMessage transactionMsg = new()
				{
					CorrelationId = e.CorrelationId,
					TransactionStatus = TransactionStatus.Failed,
					MessageStatus = TransactionMessageStatus.ForProcessing,
					FailedService = FailedService.Notification
				};

				await dbSaga.TransactionConfirmationOutboxMessages.AddAsync(transactionMsg);
				dbSaga.PosetilacSagaStates.Update(saga);
				await dbSaga.SaveChangesAsync();
				return;
			}




		}
	}
}
