using System.Linq;

using BasketContext.Framework;

using FluentAssertions;

using KellermanSoftware.CompareNetObjects;

namespace BasketContext.Domain.Tests
{
	public static class AggregateRootExtensions
	{
		public static void ShouldPublishDomainEvents<T>(this IAggregateRoot<T> aggregate, params object[] events)
		{
			new CompareLogic().Compare(aggregate.GetChanges().ToArray(), events).AreEqual.Should().Be(true);
		}
	}
}
