using System.Threading.Tasks;

using BasketContext.Domain;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BasketContext.Api.Plumbing
{
	public class GlobalExceptionHandler : IAsyncExceptionFilter
	{
		public async Task OnExceptionAsync(ExceptionContext context)
		{
			switch (context.Exception)
			{
				case Exceptions.AggregateNotFoundException _:
					context.Result = new NotFoundObjectResult(new
					{
						((Exceptions.AggregateNotFoundException)context.Exception).Message
					});
					break;
				case Exceptions.InvalidItemQuantityException _:
					context.Result = new BadRequestObjectResult(
						new
						{
							((Exceptions.InvalidItemQuantityException)context.Exception).Message
						});
					break;
				default:
					context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
					break;
			}

			context.ExceptionHandled = true;
			await context.Result.ExecuteResultAsync(context);
		}
	}
}
