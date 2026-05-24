using Common.EventService;
using Microsoft.EntityFrameworkCore;
using StrucniDogadjaj.Domain.Write;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrucniDogadjaj.Infrastructure.Write.EFCore.Data
{
	public class WriteDbContext : DbContext
	{
		public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
		{
		}

		protected WriteDbContext()
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<StrucniDogadjaj.Domain.Write.StrucniDogadjaj>()
						.ToTable("Dogadjaj", t => t.ExcludeFromMigrations());
		}

		public DbSet<StrucniDogadjaj.Domain.Write.StrucniDogadjaj> Dogadjaji { get; set; }
		//public DbSet<DogadjajPredavac> DogadjajPredavaci { get; set; }

	}
}
