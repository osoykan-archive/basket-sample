using BasketContext.Domain.Aggregates;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasketContext.Domain.Database
{
	public class BasketMapping : IEntityTypeConfiguration<Aggregates.Basket>
	{
		public void Configure(EntityTypeBuilder<Aggregates.Basket> builder)
		{
			builder.ToTable("Baskets");
			builder.HasKey(x => x.Id);
			builder.HasMany(x => x.Items).WithOne(x => x.Basket).HasForeignKey(x => x.BasketId);
		}
	}

	public class BasketItemMapping : IEntityTypeConfiguration<BasketItem>
	{
		public void Configure(EntityTypeBuilder<BasketItem> builder)
		{
			builder.ToTable("BasketItems");
			builder.HasKey(x => x.Id);
			builder.HasOne(x => x.Basket).WithMany(x => x.Items).HasForeignKey(x => x.BasketId);
			builder.Property(x => x.Quantity);
		}
	}
}
