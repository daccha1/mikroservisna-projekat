using Common.Saga_Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using PosetilacSagaOrkestrator.Data;
using PosetilacSagaOrkestrator.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PosetilacSagaOrkestrator.Services.MQ_Container
{
	public static class MessageDispatcher
	{
		public async static void DispatchGiftPosetilacOutboxMessage() // DISPATCHER ZA GIFT SERVICE | I 
		{
			while (true)
			{
				// uzmi outbox poruku
				// proveri da li postoji saga za taj correlation ID 
				// ako ne postoji saga -> preskoci

				// uzmemo sagu na osnovu correlation-a
				// publish poruke na odgovarajuci queue
				// azuriramo state sage, azuriramo state outbox poruke, save changes

				var sagaDb = new PosetilacOrkestratorDbContext();
				var outboxMsg = sagaDb.GiftsOutboxMessages.Where(msg => msg.Status == GiftOutboxStatus.ForProcessing || msg.Status == (GiftOutboxStatus)0).FirstOrDefault();

				if (outboxMsg == null)
				{
					await Task.Delay(2000);
					continue;
				};
				
				var sagaState = sagaDb.PosetilacSagaStates.Where(saga => saga.CorrelationId == outboxMsg.CorrelationId).FirstOrDefault();

				if (sagaState == null)
				{
					await Task.Delay(2000);
					continue;
				};

				CreateGift createGiftRequest = new()
				{
					CorrelationId = sagaState.CorrelationId,
					Interesovanje = sagaState.Interesovanje,
				};

				sagaState.State = SagaStates.NotGifted; // u sustini waiting for gift (los naziv)
				outboxMsg.Status = GiftOutboxStatus.Processed;

				using var mqClient = new MQClient();

				await mqClient.Publish("create-gift", JsonSerializer.Serialize<CreateGift>(createGiftRequest));

				sagaDb.PosetilacSagaStates.Update(sagaState);
				sagaDb.GiftsOutboxMessages.Update(outboxMsg);

				await sagaDb.SaveChangesAsync();

				await Task.Delay(3000);
			}
		}

		public async static void DispatchNotificationOutboxMessage() // DISPATCHER ZA NOTIFICATION SERVICE | II
		{
			while (true)
			{
				var dbSaga = new PosetilacOrkestratorDbContext();
				var outboxMsg = await dbSaga.NotificationsOutboxMessages.Where(msg => msg.Status == NotificationOutboxStatus.ForProcessing).FirstOrDefaultAsync();

				if(outboxMsg == null)
				{
					await Task.Delay(3000);
					continue;
				}

				NotifyPosetilac notificationObject = new()
				{
					Id = Guid.NewGuid(),
					CorrelationId = outboxMsg.CorrelationId,
					Email = "nijedavid@gmail.com"
				};

				var currentSaga = await dbSaga.PosetilacSagaStates.Where(saga => saga.CorrelationId == outboxMsg.CorrelationId).FirstOrDefaultAsync();

				if (currentSaga == null)
				{
					await Task.Delay(2000);
					continue;
				};

				currentSaga.State = SagaStates.WaitingForNotification;

				dbSaga.PosetilacSagaStates.Update(currentSaga);
				
				using var mqClient = new MQClient();
				await mqClient.Publish("notify-posetilac", JsonSerializer.Serialize<NotifyPosetilac>(notificationObject));

				outboxMsg.Status = NotificationOutboxStatus.Processed;
				dbSaga.NotificationsOutboxMessages.Update(outboxMsg);
				await dbSaga.SaveChangesAsync();

				await Task.Delay(3000);
			}
		}
		
		internal static async void DispatchTransactionOutboxMessage() // DISPATCHER ZA FINALNI FEEDBACK U SAGI
		{	
			while (true)
			{
				var dbSaga = new PosetilacOrkestratorDbContext();
				var transactionMsg = await dbSaga.TransactionConfirmationOutboxMessages.Where(msg => msg.MessageStatus == TransactionMessageStatus.ForProcessing).FirstOrDefaultAsync();

				if(transactionMsg == null)
				{
					await Task.Delay(3000);
					continue;
				}

				// mapiranje u contract klasu
				TransactionFinalState transactionState = new()
				{
					CorrelationId = transactionMsg.CorrelationId,
					TranscationStatus = (transactionMsg.TransactionStatus == TransactionStatus.Successful ? FinalTransactionState.Successful : FinalTransactionState.Failed)
				};
				
				MQClient client = new();
				await client.Publish("transaction-final-feedback", JsonSerializer.Serialize<TransactionFinalState>(transactionState));

				transactionMsg.MessageStatus = TransactionMessageStatus.Processed;
				dbSaga.TransactionConfirmationOutboxMessages.Update(transactionMsg);
				await dbSaga.SaveChangesAsync();
				await Task.Delay(3000);
			}
		}
	}
}
