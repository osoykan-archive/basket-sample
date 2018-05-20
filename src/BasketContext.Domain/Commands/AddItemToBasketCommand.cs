using System;

using MediatR;

namespace BasketContext.Domain.Commands
{
	public class AddItemToBasketCommand : IRequest
	{
		public readonly Guid BasketId;
		public readonly Guid ItemId;
		public readonly int Quantity;

		public AddItemToBasketCommand(Guid basketId, Guid itemId, int quantity)
		{
			BasketId = basketId;
			ItemId = itemId;
			Quantity = quantity;
		}
	}
}
