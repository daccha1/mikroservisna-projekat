using EventActivityService.Models.Domain_models;
using EventActivityService.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace EventActivityService.Data
{
	public class EventsDbContext : DbContext
	{
		public EventsDbContext(DbContextOptions options) : base(options)
		{
		}

		protected EventsDbContext()
		{
		}

		public DbSet<EventEntity> Events { get; set; }
		public DbSet<EventActivitySnapshot> ActivitySnapshots { get; set; }

	}
}
