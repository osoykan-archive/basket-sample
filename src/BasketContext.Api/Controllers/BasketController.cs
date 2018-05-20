using System;
using System.Threading.Tasks;

using BasketContext.Api.Models.Requests;
using BasketContext.Domain.Commands;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketContext.Api.Controllers
{
	[Authorize]
	[Route("/api/basket")]
	public class BasketController : Controller
	{
		private readonly IMediator _mediator;

		public BasketController(IMediator mediator) => _mediator = mediator;

		[HttpPut]
		[Route("/{basketId}")]
		public Task Put(Guid basketId, [FromBody] AddItemToBasketRequest request) =>
			_mediator.Send(new AddItemToBasketCommand(basketId, request.ItemId, request.Quantity));

		[HttpPut]
		[Route("/{basketId}/items/")]
		public Task Put(Guid basketId, [FromBody] ChangeQuantityRequest request) =>
			_mediator.Send(new ChangeQuantityCommand(basketId, request.ItemId, request.Quantity));

		[HttpDelete]
		[Route("/{basketId}/clear")]
		public Task Delete(Guid basketId) =>
			_mediator.Send(new ClearBasketCommand(basketId));
	}
}
