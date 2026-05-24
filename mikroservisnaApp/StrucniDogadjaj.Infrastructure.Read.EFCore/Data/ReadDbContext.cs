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

	}
}
