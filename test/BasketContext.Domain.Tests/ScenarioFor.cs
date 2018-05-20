using System;
using System.Linq;

using BasketContext.Framework;

using FluentAssertions;

using KellermanSoftware.CompareNetObjects;

namespace BasketContext.Domain.Tests
{
	public class ScenarioFor<TAggregateRoot> where TAggregateRoot : IAggregateChangeTracker
	{
		private TAggregateRoot _aggregateRoot;

		public ScenarioFor(Func<TAggregateRoot> constructor) => _aggregateRoot = constructor();

		public ScenarioFor<TAggregateRoot> When(params Action<TAggregateRoot>[] whens)
		{
			foreach (Action<TAggregateRoot> action in whens)
			{
				action(_aggregateRoot);
			}

			return this;
		}

		public void ThenAssert(params object[] events)
		{
			new CompareLogic().Compare(_aggregateRoot.GetChanges().ToArray(), events).AreEqual.Should().Be(true);
		}
	}
}
