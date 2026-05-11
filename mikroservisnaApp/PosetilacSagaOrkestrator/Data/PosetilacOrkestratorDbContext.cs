using Microsoft.EntityFrameworkCore;
using PosetilacSagaOrkestrator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PosetilacSagaOrkestrator.Data
{
	public class PosetilacOrkestratorDbContext : DbContext
	{
		public PosetilacOrkestratorDbContext(DbContextOptions options) : base(options) {}

		public PosetilacOrkestratorDbContext() {}

		public DbSet<PosetilacSagaState> PosetilacSagaStates { get; set; }
		public DbSet<GiftOutboxMessage> GiftsOutboxMessages { get; set; }
		public DbSet<NotificationOutboxMessage> NotificationsOutboxMessages { get; set; }


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PosetilacSagaOrkestrator_Db;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
			}
		}

	}
}
