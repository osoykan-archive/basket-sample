using System;

using MediatR;

namespace BasketContext.Domain.Events
{
	public class ItemQuantityChangedEvent : INotification
	{
		public readonly int ToQuantity;
		public readonly Guid BasketId;
		public readonly int FromQuantity;
		public readonly Guid ItemId;

		public ItemQuantityChangedEvent(Guid basketId, Guid itemId, int fromQuantity, int toQuantity)
		{
			BasketId = basketId;
			ItemId = itemId;
			FromQuantity = fromQuantity;
			ToQuantity = toQuantity;
		}
	}
}