using System;
using System.Linq;

using BasketContext.Domain.Aggregates;
using BasketContext.Domain.Events;

using FluentAssertions;

using Xunit;

namespace BasketContext.Domain.Tests.Specs
{
	public class BasketSpecs : SpecBase
	{
		[Fact]
		public void Basket_Should_Be_Created_Without_Any_Item()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			var basketCreatedEvent = new BasketCreatedEvent(basketId);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = Basket.Create(basketId);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------

			basket.ShouldPublishDomainEvents(basketCreatedEvent);
		}

		[Fact]
		public void BasketItem_Should_Be_Added_To_Basket_When_Item_Quantity_Greater_Than_Zero()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			Guid itemId = Faker.Random.Uuid();
			const int quantity = 2;

			var basketCreatedEvent = new BasketCreatedEvent(basketId);
			var itemAddedEvent = new ItemAddedEvent(basketId, itemId, quantity);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = Basket.Create(basketId);
			basket.AddItem(itemId, quantity);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			basket.Items.Count.Should().Be(1);
			basket.ShouldPublishDomainEvents(basketCreatedEvent, itemAddedEvent);
		}

		[Fact]
		public void Adding_SameItem_With_Same_Quantity_To_Basket_Should_Be_Idempotent()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			Guid itemId = Faker.Random.Uuid();
			const int quantity = 2;

			var basketCreatedEvent = new BasketCreatedEvent(basketId);
			var itemAddedEvent = new ItemAddedEvent(basketId, itemId, quantity);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = Basket.Create(basketId);
			basket.AddItem(itemId, quantity);
			basket.AddItem(itemId, quantity);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			basket.Items.Count.Should().Be(1);
			basket.ShouldPublishDomainEvents(basketCreatedEvent, itemAddedEvent);
		}

		[Fact]
		public void Adding_SameItem_With_Different_Quantity_To_Basket_Should_Change_Quantity()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			Guid itemId = Faker.Random.Uuid();
			const int quantity = 2;
			const int toQuantity = 4;

			var basketCreatedEvent = new BasketCreatedEvent(basketId);
			var itemAddedEvent = new ItemAddedEvent(basketId, itemId, quantity);
			var quantityChangedEvent = new ItemQuantityChangedEvent(basketId, itemId, quantity, toQuantity);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = Basket.Create(basketId);
			basket.AddItem(itemId, quantity);
			basket.AddItem(itemId, toQuantity);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			basket.Items.Count.Should().Be(1);
			basket.ShouldPublishDomainEvents(basketCreatedEvent, itemAddedEvent, quantityChangedEvent);
		}

		[Fact]
		public void Changing_Not_Existed_Item_Quantity_Should_Throw_ItemNotFoundException()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			Guid itemId = Faker.Random.Uuid();
			Guid notExisteditemId = Faker.Random.Uuid();
			const int quantity = 2;

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = Basket.Create(basketId);
			basket.AddItem(itemId, quantity);
			Action act = () => basket.ChangeItemQuantity(notExisteditemId, quantity);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			act.Should().Throw<Exceptions.ItemNotFoundException>().WithMessage(string.Format(Messages.ItemNotFoundExceptionMessage, notExisteditemId));
		}

		[Fact]
		public void BasketItem_Should_Not_Be_Added_When_Item_Quantity_Is_Less_or_Equal_To_Zero_And_Should_Throw_InvalidItemQuantityException()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			Guid itemId = Faker.Random.Uuid();
			const int quantity = 0;

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = Basket.Create(basketId);
			Action act = () => basket.AddItem(itemId, quantity);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			act.Should().Throw<Exceptions.InvalidItemQuantityException>().WithMessage(string.Format(Messages.InvalidItemQuantityExceptionMessage, itemId));
		}

		[Fact]
		public void BasketItem_Quantity_Should_Be_Changed()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			Guid itemId = Faker.Random.Uuid();
			const int fromQuantity = 2;
			const int toQuantity = 4;

			var basketCreatedEvent = new BasketCreatedEvent(basketId);
			var itemAddedEvent = new ItemAddedEvent(basketId, itemId, fromQuantity);
			var itemQuantityChangedEvent = new ItemQuantityChangedEvent(basketId, itemId, fromQuantity, toQuantity);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = Basket.Create(basketId);
			basket.AddItem(itemId, fromQuantity);
			basket.ChangeItemQuantity(itemId, toQuantity);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			basket.Items.Count.Should().Be(1);
			basket.ShouldPublishDomainEvents(basketCreatedEvent, itemAddedEvent, itemQuantityChangedEvent);
		}

		[Fact]
		public void BasketItem_Quantity_Should_Be_Changed_And_Product_Should_Be_Removed_When_Quantity_Is_Zero()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			Guid itemId = Faker.Random.Uuid();
			const int fromQuantity = 2;
			const int toQuantity = 0;

			var basketCreatedEvent = new BasketCreatedEvent(basketId);
			var itemAddedEvent = new ItemAddedEvent(basketId, itemId, fromQuantity);
			var itemRemovedEvent = new ItemRemovedEvent(basketId, itemId);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = Basket.Create(basketId);
			basket.AddItem(itemId, fromQuantity);
			basket.ChangeItemQuantity(itemId, toQuantity);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			basket.Items.Count.Should().Be(0);
			basket.ShouldPublishDomainEvents(basketCreatedEvent, itemAddedEvent, itemRemovedEvent);
		}

		[Fact]
		public void Clearing_Items_From_Basket_Should_Clear_All_Items_In_The_Basket()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			Guid itemId = Faker.Random.Uuid();
			const int fromQuantity = 2;

			var basketCreatedEvent = new BasketCreatedEvent(basketId);
			var itemAddedEvent = new ItemAddedEvent(basketId, itemId, fromQuantity);
			var basketCleanedEvent = new BasketCleaned(basketId);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = Basket.Create(basketId);
			basket.AddItem(itemId, fromQuantity);
			basket.Clear();

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			basket.Items.Count.Should().Be(0);
			basket.ShouldPublishDomainEvents(basketCreatedEvent, itemAddedEvent, basketCleanedEvent);
		}

		[Fact]
		public void Clearing_Items_From_Basket_Should_Be_Idempotent()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			var basketCreatedEvent = new BasketCreatedEvent(basketId);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = Basket.Create(basketId);
			basket.Clear();
			basket.Clear();

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			basket.Items.Count.Should().Be(0);
			basket.ShouldPublishDomainEvents(basketCreatedEvent);
		}
	}
}
