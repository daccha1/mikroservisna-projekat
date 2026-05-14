using System.Diagnostics;

namespace mikroservisnaApp.Patterns
{
	public enum CircuitBreakerState
	{
		Closed,
		Open,
		HalfOpen
	}

	public class CircuitBreaker
	{
		private object _lock = new object();
		private readonly int _failuireThreshold;
		private readonly TimeSpan _openDuration; // koliko dugo da stoji otvoren pre nego sto pusti zahtev
		private DateTime _lastFailiureTime = DateTime.MinValue;
		private int _failuireCount;
		private CircuitBreakerState _state = CircuitBreakerState.Closed;

		public CircuitBreaker(int failuireThreshold, TimeSpan openDuration)
		{
			_failuireThreshold = failuireThreshold;
			_openDuration = openDuration;
		}

		public CircuitBreakerState State
		{
			get
			{
				lock (_lock)
				{					//						   ako je proslo dovoljno da pusti zahtev
					if (_state == CircuitBreakerState.Open && (DateTime.UtcNow - _lastFailiureTime) > _openDuration)
					{
						_state = CircuitBreakerState.HalfOpen;
					}
				}

				return _state;
			}
		}

		public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
		{
			if (State == CircuitBreakerState.Open)
			{
				throw new CircuitBreakerOpenException("CircuitBreaker Open!");
			}

			try
			{
				var result = await action();

				lock (_lock)
				{
					_failuireCount = 0;
					_state = CircuitBreakerState.Closed;
				}

				return result;
			}
			catch (Exception)
			{
				lock (_lock)
				{
					// podestnik: u program.cs je _failuireThreshold = 1 zbog testiranja
					_failuireCount += 2;
					_lastFailiureTime = DateTime.UtcNow;

					if (_state == CircuitBreakerState.HalfOpen)
					{
						_state = CircuitBreakerState.Open;
						Debug.Write("CircuitBreaker OPEN");
					}
					if (_failuireCount > _failuireThreshold)
					{
						_state = CircuitBreakerState.Open;
						Debug.Write("CircuitBreaker OPEN");
					}
				}

				throw;
			}
		}
	}

	public class CircuitBreakerOpenException : Exception
	{
		public CircuitBreakerOpenException(string? message) : base(message)
		{
		}
	}
}
