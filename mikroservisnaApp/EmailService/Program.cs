using EmailService.Services;

namespace EmailService
{
	internal class Program
	{
		static void Main(string[] args)
		{
			EmailSenderClient.StartClient();
		}
	}
}
