using System;
using System.Linq;
using System.Threading.Tasks;

using BasketContext.Domain.Aggregates;
using BasketContext.Domain.Commands;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace BasketContext.Domain.IntegrationTests
{
	public class BasketCommandHandlerTest : IntegrationTestBase
	{
		[Fact]
		public async Task Basket_Should_Be_Created_With_Items()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			Guid itemId = Faker.Random.Uuid();
			const int quantity = 2;
			var command = new AddItemToBasketCommand(basketId, itemId, quantity);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			await Sut.Send(command);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = await Read(async context => { return await context.Baskets.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == basketId); });
			basket.Should().NotBeNull();
			basket.Items.Count.Should().Be(1);
			basket.Items.Should().Contain(x => x.Id == itemId);
			basket.Items.First(x => x.Id == itemId).Quantity.Should().Be(quantity);
		}

		[Fact]
		public async Task Quantity_Should_Be_Changed_When_New_Quantity_Provided_For_Same_Item()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			Guid itemId = Faker.Random.Uuid();
			const int quantity = 2;
			const int toQuantity = 4;
			var addItemToBasketCommand = new AddItemToBasketCommand(basketId, itemId, quantity);
			var changeQuantityCommand = new ChangeQuantityCommand(basketId, itemId, toQuantity);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			await Sut.Send(addItemToBasketCommand);
			await Sut.Send(changeQuantityCommand);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = await Read(async context => { return await context.Baskets.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == basketId); });
			basket.Should().NotBeNull();
			basket.Items.Count.Should().Be(1);
			basket.Items.Should().Contain(x => x.Id == itemId);
			basket.Items.First(x => x.Id == itemId).Quantity.Should().Be(toQuantity);
		}

		[Fact]
		public async Task When_Change_Quantity_Is_Made_With_Zero_Then_Item_Should_Be_Removed_From_The_Basket()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			Guid itemId = Faker.Random.Uuid();
			const int quantity = 2;
			const int toQuantity = 0;
			var addItemToBasketCommand = new AddItemToBasketCommand(basketId, itemId, quantity);
			var changeQuantityCommand = new ChangeQuantityCommand(basketId, itemId, toQuantity);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			await Sut.Send(addItemToBasketCommand);
			await Sut.Send(changeQuantityCommand);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = await Read(async context => { return await context.Baskets.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == basketId); });
			basket.Should().NotBeNull();
			basket.Items.Count.Should().Be(0);
			basket.Items.Should().NotContain(x => x.Id == itemId);
		}

		[Fact]
		public void Change_Quantiy_Should_Throw_AggregateNotFoundException_When_Basket_Does_Not_Exist()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			Guid itemId = Faker.Random.Uuid();
			const int toQuantity = 4;

			var changeQuantityCommand = new ChangeQuantityCommand(basketId, itemId, toQuantity);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			Func<Task> act = () => Sut.Send(changeQuantityCommand);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			act.Should().Throw<Exceptions.AggregateNotFoundException>().WithMessage(string.Format(Messages.AggregateNotFoundExceptionMessage, basketId));
		}

		[Fact]
		public async Task Clear_Basket_Should_Clear_All_Items_In_The_Basket()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			Guid itemId = Faker.Random.Uuid();
			const int quantity = 2;

			var addItemToBasketCommand = new AddItemToBasketCommand(basketId, itemId, quantity);
			var clearBasketCommand = new ClearBasketCommand(basketId);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			await Sut.Send(addItemToBasketCommand);
			await Sut.Send(clearBasketCommand);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			Basket basket = await Read(async context => { return await context.Baskets.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == basketId); });
			basket.Should().NotBeNull();
			basket.Items.Count.Should().Be(0);
			basket.Items.Should().NotContain(x => x.Id == itemId);
		}

		[Fact]
		public void Clear_Basket_Should_Throw_AggregateNotFoundException_If_Basket_Does_Not_Exist()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Faker.Random.Uuid();
			var clearBasketCommand = new ClearBasketCommand(basketId);

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			Func<Task> act = () => Sut.Send(clearBasketCommand);

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			act.Should().Throw<Exceptions.AggregateNotFoundException>().WithMessage(string.Format(Messages.AggregateNotFoundExceptionMessage, basketId));
		}
	}
}
