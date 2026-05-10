using Common.Saga_Contracts;
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
		public async static void DispatchGiftPosetilacOutboxMessage()
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

				if (outboxMsg == null) continue;
				
				var sagaState = sagaDb.PosetilacSagaStates.Where(saga => saga.CorrelationId == outboxMsg.CorrelationId).FirstOrDefault();

				if (sagaState == null) continue;

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

				Task.Delay(3000);
			}
		}
	}
}
