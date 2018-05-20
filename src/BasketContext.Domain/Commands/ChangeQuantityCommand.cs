using System;

using MediatR;

namespace BasketContext.Domain.Commands
{
	public class ChangeQuantityCommand : IRequest
	{
		public readonly Guid BasketId;
		public readonly Guid ItemId;
		public readonly int Quantity;

		public ChangeQuantityCommand(Guid basketId, Guid itemId, int quantity)
		{
			BasketId = basketId;
			ItemId = itemId;
			Quantity = quantity;
		}
	}
}
