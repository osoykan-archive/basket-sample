using System.Threading;
using System.Threading.Tasks;

using BasketContext.Domain.Events;

using MediatR;

namespace BasketContext.Domain.DomainEventHandlers
{
	public class BasketEventHandlers :
		INotificationHandler<BasketCreatedEvent>,
		INotificationHandler<BasketCleaned>,
		INotificationHandler<ItemAddedEvent>,
		INotificationHandler<ItemQuantityChangedEvent>,
		INotificationHandler<ItemRemovedEvent>
	{
		public Task Handle(BasketCleaned notification, CancellationToken cancellationToken)
		{
			// Handle Projections or inform the other Bounded Context...

			return Task.CompletedTask;
		}

		public Task Handle(BasketCreatedEvent notification, CancellationToken cancellationToken)
		{
			// Handle Projections or inform the other Bounded Context...

			return Task.CompletedTask;
		}

		public Task Handle(ItemAddedEvent notification, CancellationToken cancellationToken)
		{
			// Handle Projections or inform the other Bounded Context...

			return Task.CompletedTask;
		}

		public Task Handle(ItemQuantityChangedEvent notification, CancellationToken cancellationToken)
		{
			// Handle Projections or inform the other Bounded Context...

			return Task.CompletedTask;
		}

		public Task Handle(ItemRemovedEvent notification, CancellationToken cancellationToken)
		{
			// Handle Projections or inform the other Bounded Context...

			return Task.CompletedTask;
		}
	}
}
