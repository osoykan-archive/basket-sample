using System;

using MediatR;

namespace BasketContext.Domain.Events
{
	public class ItemRemovedEvent : INotification
	{
		public readonly Guid BasketId;
		public readonly Guid ItemId;

		public ItemRemovedEvent(Guid basketId, Guid itemId)
		{
			BasketId = basketId;
			ItemId = itemId;
		}
	}
}
