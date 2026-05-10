namespace PosetilacSagaOrkestrator
{
	internal class Program
	{
		static void Main(string[] args)
		{
			
			// uhvati poruku da je kreiran posetilac
			// kreiramo inicijalni state, outbox state, i bg workera
			// publishujemo na GiftService, generise se gift vraca na queue
			// uhvatimo ponovo poruku, izmenimo state preko CorrelationId-a
			// tada publishujemo poruku na EmailService
			// u emailService u mejlu dovucemo taj pdf, i saljemo i njega



		}
	}
}
