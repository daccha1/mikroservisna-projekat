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

		protected PosetilacOrkestratorDbContext() {}

		public DbSet<PosetilacSagaState> PosetilacSagaStates { get; set; }


	}
}
