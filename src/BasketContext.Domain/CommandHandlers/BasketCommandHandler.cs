using System.Threading;
using System.Threading.Tasks;

using BasketContext.Domain.Aggregates;
using BasketContext.Domain.Commands;
using BasketContext.Domain.Database;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Optional;

namespace BasketContext.Domain.CommandHandlers
{
	public class BasketCommandHandler : HandlerBase,
		IRequestHandler<AddItemToBasketCommand>,
		IRequestHandler<ClearBasketCommand>,
		IRequestHandler<ChangeQuantityCommand>
	{
		public BasketCommandHandler(BasketDbContext basketDbContext, IMediator mediator) : base(basketDbContext, mediator)
		{
		}

		public async Task Handle(AddItemToBasketCommand request, CancellationToken cancellationToken) =>
			await When(request, async (command, context, cToken) =>
				{
					Basket basket = await context.Baskets.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == command.BasketId, cToken);
					if (basket == null)
					{
						basket = Basket.Create(command.BasketId);
						await context.AddAsync(basket, cToken);
					}

					basket.AddItem(command.ItemId, command.Quantity);
				},
				cancellationToken: cancellationToken);

		public async Task Handle(ChangeQuantityCommand request, CancellationToken cancellationToken) =>
			await When(request, async (command, context, cToken) =>
				{
					(await context.Baskets.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == command.BasketId, cToken))
						.SomeNotNull()
						.Match(
							basket => basket.ChangeItemQuantity(command.ItemId, command.Quantity),
							() => throw new Exceptions.AggregateNotFoundException(command.BasketId)
						);
				},
				cancellationToken: cancellationToken);

		public async Task Handle(ClearBasketCommand request, CancellationToken cancellationToken) =>
			await When(request, async (command, context, cToken) =>
				{
					(await context.Baskets.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == command.BasketId, cToken))
						.SomeNotNull()
						.Match(
							basket => basket.Clear(),
							() => throw new Exceptions.AggregateNotFoundException(command.BasketId)
						);
				},
				cancellationToken: cancellationToken);
	}
}
