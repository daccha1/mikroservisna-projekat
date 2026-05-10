using GiftService.Models;
using Microsoft.EntityFrameworkCore;

namespace GiftService.Data
{
	public class GiftDbContext : DbContext
	{
		public GiftDbContext(DbContextOptions options) : base(options) {}

		protected GiftDbContext() {}

		public DbSet<Gift> Gifts { get; set; }
		public DbSet<GiftCreatedOutboxMessage> GiftCreatedOutboxTable { get; set; }
	}
}
