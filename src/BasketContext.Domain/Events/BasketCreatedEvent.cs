using System;

using MediatR;

namespace BasketContext.Domain.Events
{
	public class BasketCreatedEvent : INotification
	{
		public readonly Guid BasketId;

		public BasketCreatedEvent(Guid basketId)
		{
			BasketId = basketId;
		}
	}
}
