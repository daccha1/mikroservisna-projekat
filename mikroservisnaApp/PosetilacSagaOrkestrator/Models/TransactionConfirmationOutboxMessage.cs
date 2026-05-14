using System;
using System.Collections.Generic;
using System.Text;

namespace PosetilacSagaOrkestrator.Models
{
	public enum TransactionStatus
	{
		Successful,
		Failed
	}

	public enum TransactionMessageStatus
	{
		ForProcessing,
		Processed
	}

	public enum FailedService
	{
		None,
		Gift,
		Notification
	}

	public class TransactionConfirmationOutboxMessage
	{
		public int Id { get; set; }
		public Guid CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public TransactionStatus TransactionStatus { get; set; }
		public TransactionMessageStatus MessageStatus { get; set; }
		public FailedService FailedService { get; set; } = FailedService.None;
	}
}
