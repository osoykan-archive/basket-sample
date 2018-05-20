using System;

using BasketContext.Domain.Events;
using BasketContext.Framework;

namespace BasketContext.Domain.Aggregates
{
	public class BasketItem : Entity<Guid>
	{
		public BasketItem(Guid basketId, Guid itemId, int quantity) : this()
		{
			Id = itemId;
			BasketId = basketId;
			Quantity = quantity;
		}

		protected BasketItem()
		{
			Register<ItemQuantityChangedEvent>(When);
		}

		public virtual Basket Basket { get; protected set; }

		public virtual Guid BasketId { get; protected set; }

		public virtual int Quantity { get; protected set; }

		private void When(ItemQuantityChangedEvent @event)
		{
			Quantity = @event.ToQuantity;
		}
	}
}
