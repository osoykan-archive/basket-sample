using System;

using MediatR;

namespace BasketContext.Domain.Events
{
	public class BasketCleaned : INotification
	{
		public readonly Guid BasketId;

		public BasketCleaned(Guid basketId)
		{
			BasketId = basketId;
		}
	}
}
