namespace PosetilacAPI.Models
{
	public enum State
	{
		Success,
		Fail
	}

	public enum OutboxState
	{
		ForProcessing,
		Processed
	}

	public class SagaResultOutboxMessage
	{
		public int Id { get; set; }
		public Guid CorrelationId { get; set; }
		public State FinalState { get; set; } = State.Success;
		public OutboxState OutboxState { get; set; }

	}
}
