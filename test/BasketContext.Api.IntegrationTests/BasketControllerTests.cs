using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using BasketContext.Api.Models.Requests;
using BasketContext.Api.Models.Responses;

using FluentAssertions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

using Newtonsoft.Json;

using Xunit;

namespace BasketContext.Api.IntegrationTests
{
	public class BasketControllerTests
	{
		private readonly HttpClient _client;

		public BasketControllerTests()
		{
			var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
			_client = server.CreateClient();
		}

		[Fact]
		public async Task Add_Item_To_Basket_Should_Return_Success()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Guid.NewGuid();
			Guid itemId = Guid.NewGuid();
			const int quantity = 2;
			var request = new AddItemToBasketRequest { Quantity = quantity, ItemId = itemId };

			await EnsureTokenIsCreated();

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			HttpResponseMessage response = await _client.PutAsync($"/{basketId}", request.ToJsonStringContent());

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			response.EnsureSuccessStatusCode();
		}

		[Fact]
		public async Task Add_Item_To_Basket_Should_Return_BadRequest_When_Provided_Request_Does_Not_Proper()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Guid.NewGuid();
			Guid itemId = Guid.Empty;
			const int quantity = 2;
			var request = new AddItemToBasketRequest { Quantity = quantity, ItemId = itemId };

			await EnsureTokenIsCreated();

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			HttpResponseMessage response = await _client.PutAsync($"/{basketId}", request.ToJsonStringContent());

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Add_Item_To_Basket_With_Zero_Quantity_Should_Return_BadRequest()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Guid.NewGuid();
			Guid itemId = Guid.NewGuid();
			const int quantity = 0;
			var request = new AddItemToBasketRequest { Quantity = quantity, ItemId = itemId };

			await EnsureTokenIsCreated();

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			HttpResponseMessage response = await _client.PutAsync($"/{basketId}", request.ToJsonStringContent());

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Change_Quantity_Of_Item_Should_Return_Success()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Guid.NewGuid();
			Guid itemId = Guid.NewGuid();
			const int quantity = 2;
			var addItemToBasketRequest = new AddItemToBasketRequest { Quantity = quantity, ItemId = itemId };
			var changeQuantityRequest = new ChangeQuantityRequest { Quantity = quantity, ItemId = itemId };

			await EnsureTokenIsCreated();

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------

			await _client.PutAsync($"/{basketId}", addItemToBasketRequest.ToJsonStringContent());
			HttpResponseMessage response = await _client.PutAsync($"/{basketId}/items", changeQuantityRequest.ToJsonStringContent());

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			response.EnsureSuccessStatusCode();
		}

		[Fact]
		public async Task Change_Quantity_Of_Item_Should_Return_NotFound_When_Basket_Does_Not_Exist()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Guid.NewGuid();
			Guid itemId = Guid.NewGuid();
			const int quantity = 2;
			var changeQuantityRequest = new ChangeQuantityRequest { Quantity = quantity, ItemId = itemId };

			await EnsureTokenIsCreated();

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			HttpResponseMessage response = await _client.PutAsync($"/{basketId}/items", changeQuantityRequest.ToJsonStringContent());

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task Clear_Basket_Should_Return_Success()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Guid.NewGuid();
			Guid itemId = Guid.NewGuid();
			const int quantity = 2;

			var addItemToBasketRequest = new AddItemToBasketRequest { Quantity = quantity, ItemId = itemId };

			await EnsureTokenIsCreated();

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			await _client.PutAsync($"/{basketId}", addItemToBasketRequest.ToJsonStringContent());
			HttpResponseMessage response = await _client.DeleteAsync($"/{basketId}/clear");

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			response.EnsureSuccessStatusCode();
		}

		[Fact]
		public async Task Clear_Basket_Should_Return_Not_Found_When_Basket_Does_Not_Exist()
		{
			//-----------------------------------------------------------------------------------------------------------
			// Arrange
			//-----------------------------------------------------------------------------------------------------------
			Guid basketId = Guid.NewGuid();

			await EnsureTokenIsCreated();

			//-----------------------------------------------------------------------------------------------------------
			// Act
			//-----------------------------------------------------------------------------------------------------------
			HttpResponseMessage response = await _client.DeleteAsync($"/{basketId}/clear");

			//-----------------------------------------------------------------------------------------------------------
			// Assert
			//-----------------------------------------------------------------------------------------------------------
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		private async Task<string> EnsureTokenIsCreated()
		{
			HttpResponseMessage tokenResponse = await _client.PostAsync("/generate", new GetTokenRequest { Password = "test", Username = "test" }.ToJsonStringContent());
			string token = JsonConvert.DeserializeObject<GetTokenResponse>(await tokenResponse.Content.ReadAsStringAsync()).Token;
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			return token;
		}
	}
}
