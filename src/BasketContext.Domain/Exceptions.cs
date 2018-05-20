using System;

namespace BasketContext.Domain
{
	public static class Exceptions
	{
		public class InvalidItemQuantityException : Exception
		{
			public InvalidItemQuantityException(Guid itemId) : base(string.Format(Messages.InvalidItemQuantityExceptionMessage, itemId))
			{
			}
		}

		public class AggregateNotFoundException : Exception
		{
			public AggregateNotFoundException(Guid aggregateId) : base(string.Format(Messages.AggregateNotFoundExceptionMessage, aggregateId))
			{
			}
		}

		public class ItemNotFoundException : Exception
		{
			public ItemNotFoundException(Guid itemId) : base(string.Format(Messages.ItemNotFoundExceptionMessage, itemId))
			{
			}
		}
	}
}
