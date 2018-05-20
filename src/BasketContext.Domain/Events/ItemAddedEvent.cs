using System;

using MediatR;

namespace BasketContext.Domain.Events
{
	public class ItemAddedEvent : INotification
	{
		public readonly Guid BasketId;
		public readonly Guid ItemId;
		public readonly int Quantity;

		public ItemAddedEvent(Guid basketId, Guid itemId, int quantity)
		{
			BasketId = basketId;
			ItemId = itemId;
			Quantity = quantity;
		}
	}
}
