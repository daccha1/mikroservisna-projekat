using System.Diagnostics;
using System.Text.Json;

namespace EventActivityService.Models.Events
{
	public class EventEntity
	{
		public int Id { get; set; }
		public EventEntity()
		{
			EventId = Guid.NewGuid();
			EventType = this.GetType().Name;
		}

		public Guid EventId { get; set; }
		public Guid UserCorrelationId { get; set; } 
		public DateTime OccuredAt { get; set; } = DateTime.UtcNow;
		
		public string EventType { get; set; }
		public string Payload { get; set; }
		
		
		public static string Serialize<T>(T obj)
		{
			string payload = JsonSerializer.Serialize<T>(obj);
			return payload;
		}

		public static T Deserialize<T>(string jsonString)
		{
			T obj = JsonSerializer.Deserialize<T>(jsonString);
			return obj;
		}

	}
}
