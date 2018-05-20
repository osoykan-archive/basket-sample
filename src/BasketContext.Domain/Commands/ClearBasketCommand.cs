using System;

using MediatR;

namespace BasketContext.Domain.Commands
{
	public class ClearBasketCommand : IRequest
	{
		public readonly Guid BasketId;

		public ClearBasketCommand(Guid basketId) => BasketId = basketId;
	}
}
