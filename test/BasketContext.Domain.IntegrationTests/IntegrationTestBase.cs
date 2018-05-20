using System;
using System.Threading.Tasks;

using BasketContext.Domain.CommandHandlers;
using BasketContext.Domain.Database;

using Bogus;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BasketContext.Domain.IntegrationTests
{
	public class IntegrationTestBase : IDisposable
	{
		protected Faker Faker = new Faker();
		protected IServiceProvider ServiceProvider;
		protected IMediator Sut;

		public IntegrationTestBase()
		{
			var services = new ServiceCollection();

			services.AddMediatR(typeof(BasketCommandHandler).Assembly);
			services.AddDbContextPool<BasketDbContext>(builder => builder.UseInMemoryDatabase("BasketContext"));

			ServiceProvider = services.BuildServiceProvider();
			Sut = The<IMediator>();

			The<BasketDbContext>().Database.EnsureCreated();
		}

		public void Dispose()
		{
			The<BasketDbContext>().Database.EnsureDeleted();
		}

		protected T The<T>() => ServiceProvider.CreateScope().ServiceProvider.GetService<T>();

		protected async Task<TResponse> Read<TResponse>(Func<BasketDbContext, Task<TResponse>> when)
		{
			TResponse response;
			using (var dbContext = The<BasketDbContext>())
			{
				response = await when(dbContext);

				await dbContext.SaveChangesAsync();
			}

			return response;
		}

		protected async Task Build(Func<BasketDbContext, Task> when)
		{
			using (var dbContext = The<BasketDbContext>())
			{
				await when(dbContext);

				await dbContext.SaveChangesAsync();
			}
		}
	}
}
