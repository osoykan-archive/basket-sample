using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BasketContext.Api.Plumbing
{
	public class ModelStateFilter : IAsyncActionFilter
	{
		public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (!context.ModelState.IsValid)
			{
				context.Result = new BadRequestResult();
				return Task.CompletedTask;
			}

			return next();
		}
	}
}
