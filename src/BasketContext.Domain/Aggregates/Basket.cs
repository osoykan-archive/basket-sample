using System;
using System.Collections.Generic;
using System.Linq;

using BasketContext.Domain.Events;
using BasketContext.Framework;

namespace BasketContext.Domain.Aggregates
{
	public class Basket : AggregateRoot<Guid>
	{
		protected Basket()
		{
			Register<ItemAddedEvent>(When);
			Register<ItemQuantityChangedEvent>(When);
			Register<ItemRemovedEvent>(When);
			Register<BasketCleaned>(When);
			Register<BasketCreatedEvent>(When);

			Items = new List<BasketItem>();
		}

		public virtual ICollection<BasketItem> Items { get; protected set; }

		public static Basket Create(Guid basketId)
		{
			var aggregate = new Basket();
			aggregate.ApplyChange(
				new BasketCreatedEvent(basketId)
			);
			return aggregate;
		}

		public void AddItem(Guid itemId, int quantity)
		{
			if (quantity <= 0)
			{
				throw new Exceptions.InvalidItemQuantityException(itemId);
			}

			if (AlreadyInBasketWithSameQuantity(itemId, quantity))
			{
				return;
			}

			if (AlreadyInBasketButDifferentQuantity(itemId, quantity))
			{
				ChangeItemQuantity(itemId, quantity);
				return;
			}

			ApplyChange(
				new ItemAddedEvent(Id, itemId, quantity)
			);
		}

		public void ChangeItemQuantity(Guid itemId, int toQuantity)
		{
			BasketItem item = Items.FirstOrDefault(x => x.Id == itemId);
			if (item == null)
			{
				throw new Exceptions.ItemNotFoundException(itemId);
			}

			if (toQuantity <= 0)
			{
				RemoveItem(itemId);
				return;
			}

			ApplyChange(
				new ItemQuantityChangedEvent(Id, itemId, item.Quantity, toQuantity)
			);
		}

		public void RemoveItem(Guid itemId)
		{
			ApplyChange(
				new ItemRemovedEvent(Id, itemId)
			);
		}

		public void Clear()
		{
			if (Items.Count == 0)
			{
				return;
			}

			ApplyChange(
				new BasketCleaned(Id)
			);
		}

		public bool AlreadyInBasketWithSameQuantity(Guid itemId, int quantity)
		{
			return Items.Any(x => x.Id == itemId && x.Quantity == quantity);
		}

		public bool AlreadyInBasketButDifferentQuantity(Guid itemId, int quantity)
		{
			return Items.Any(x => x.Id == itemId && x.Quantity != quantity);
		}

		private void When(ItemQuantityChangedEvent @event)
		{
			Items.First(x => x.Id == @event.ItemId).Route(@event);
		}

		private void When(ItemAddedEvent @event)
		{
			Items.Add(new BasketItem(Id, @event.ItemId, @event.Quantity));
		}

		private void When(ItemRemovedEvent @event)
		{
			BasketItem item = Items.First(x => x.Id == @event.ItemId);
			Items.Remove(item);
		}

		private void When(BasketCreatedEvent @event)
		{
			Id = @event.BasketId;
		}

		private void When(BasketCleaned @event)
		{
			Items.Clear();
		}
	}
}
