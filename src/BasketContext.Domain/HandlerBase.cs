using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BasketContext.Domain.Database;
using BasketContext.Framework;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BasketContext.Domain
{
	public abstract class HandlerBase
	{
		private readonly BasketDbContext _basketDbContext;
		private readonly IMediator _mediator;

		protected HandlerBase(BasketDbContext basketDbContext, IMediator mediator)
		{
			_basketDbContext = basketDbContext;
			_mediator = mediator;
		}

		public async Task When<TMessage>(
			TMessage message,
			Func<TMessage, BasketDbContext, CancellationToken, Task> when,
			Action onCompleted = null,
			Action<Exception> onFailed = null,
			CancellationToken cancellationToken = default)

		{
			Func<Task> internalOnCompleted;
			try
			{
				await when(message, _basketDbContext, cancellationToken);

				internalOnCompleted = PrepareCompleteAction(_basketDbContext);

				await _basketDbContext.SaveChangesAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				onFailed?.Invoke(ex);
				
				throw;
			}

			await internalOnCompleted();

			onCompleted?.Invoke();
		}

		private Func<Task> PrepareCompleteAction(DbContext dbContext)
		{
			if (dbContext.ChangeTracker.HasChanges())
			{
				List<EntityEntry> changes = dbContext.ChangeTracker.Entries().ToList();
				IEnumerable<IAggregateChangeTracker> trackedChanges = changes.Where(x => x.Entity is IAggregateChangeTracker)
				                                                             .Select(x => (IAggregateChangeTracker)x.Entity);

				return async () =>
				{
					List<INotification> events = trackedChanges.SelectMany(x => x.GetChanges()).OfType<INotification>().ToList();
					foreach (INotification @event in events)
					{
						await _mediator.Publish(@event);
					}
				};
			}

			return () => Task.CompletedTask;
		}
	}
}
