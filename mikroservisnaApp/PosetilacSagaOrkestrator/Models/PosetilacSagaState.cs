using System;
using System.Collections.Generic;
using System.Text;

namespace PosetilacSagaOrkestrator.Models
{
	public enum SagaStates
	{
		Failed,
		Created,					// kreiran
		NotGifted,				    // ceka za generisanje svojih "giftova"
		Gifted,						// generisani giftovi
		WaitingForNotification,     // slanje mejla sa giftovima
		Notified
	}

	public class PosetilacSagaState
	{
		public int Id { get; set; } // za saga tabelu
		public int PosetilacId { get; set; } // posetilac id
		public Guid CorrelationId { get; set; } // za identifikaciju poruke i recorda
		public SagaStates State { get; set; } = SagaStates.Created;
		public DateTime CreatedAt { get; set; }
		
		public string? FailedReason { get; set; }
	}
}
