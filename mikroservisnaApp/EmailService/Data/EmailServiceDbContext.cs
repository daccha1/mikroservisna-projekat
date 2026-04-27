using EmailService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EmailService.Data
{
	public class EmailServiceDbContext : DbContext
	{
		public EmailServiceDbContext(DbContextOptions options) : base(options) {}

		protected EmailServiceDbContext() {}

		public DbSet<ProcessedMessage> ProcessedMessages { get; set; }

	}
}
