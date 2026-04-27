using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Resend;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailService.Data
{
	public class EmailServiceDbContextFactory : IDesignTimeDbContextFactory<EmailServiceDbContext>
	{
		public EmailServiceDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<EmailServiceDbContext>();
			optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EmailServiceDb;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");

			return new EmailServiceDbContext(optionsBuilder.Options);
		}
	}
}
