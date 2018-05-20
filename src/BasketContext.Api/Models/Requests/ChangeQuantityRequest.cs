using System;

using FluentValidation;

namespace BasketContext.Api.Models.Requests
{
	public class ChangeQuantityRequest
	{
		public Guid ItemId { get; set; }

		public int Quantity { get; set; }
	}

	public class ChangeQuantityRequestValidator : AbstractValidator<ChangeQuantityRequest>
	{
		public ChangeQuantityRequestValidator()
		{
			RuleFor(x => x.ItemId).NotNull().NotEmpty().GreaterThan(Guid.Empty).NotEqual(Guid.Empty);
			RuleFor(x => x.Quantity).GreaterThan(0).NotEmpty().NotNull();
		}
	}
}
