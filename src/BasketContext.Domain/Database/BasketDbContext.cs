using BasketContext.Domain.Aggregates;

using Microsoft.EntityFrameworkCore;

namespace BasketContext.Domain.Database
{
	public class BasketDbContext : DbContext
	{
		public BasketDbContext(DbContextOptions<BasketDbContext> options) : base(options)
		{
		}

		public virtual DbSet<Basket> Baskets { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new BasketMapping());
			modelBuilder.ApplyConfiguration(new BasketItemMapping());
			base.OnModelCreating(modelBuilder);
		}
	}
}
