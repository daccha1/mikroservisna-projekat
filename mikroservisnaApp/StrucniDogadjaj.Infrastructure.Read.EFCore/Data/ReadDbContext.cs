using Common.EventService;
using Microsoft.EntityFrameworkCore;
using StrucniDogadjaj.Domain.Read;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace StrucniDogadjaj.Infrastructure.Read.EFCore.Data
{
	public class ReadDbContext : DbContext
	{
		public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
		{
		}

		protected ReadDbContext()
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<StrucniDogadjaj.Domain.Read.StrucniDogadjaj>()
						.ToTable("Dogadjaj", t => t.ExcludeFromMigrations());
		}
		
		public DbSet<StrucniDogadjaj.Domain.Read.StrucniDogadjaj> Dogadjaji { get; set; }

		public override int SaveChanges()
		{
			throw new InvalidOperationException("Nije dozvoljeno menjati stanje sistema kroz ReadDbContext (Query operacije).");
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			throw new InvalidOperationException("Nije dozvoljeno menjati stanje sistema kroz ReadDbContext (Query operacije).");
		}

	}
}
