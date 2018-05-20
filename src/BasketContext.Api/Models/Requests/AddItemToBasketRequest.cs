using System;

using FluentValidation;

namespace BasketContext.Api.Models.Requests
{
	[Serializable]
	public class AddItemToBasketRequest
	{
		public Guid ItemId { get; set; }

		public int Quantity { get; set; }
	}

	public class AddItemToBasketRequestValidator : AbstractValidator<AddItemToBasketRequest>
	{
		public AddItemToBasketRequestValidator()
		{
			RuleFor(x => x.ItemId).NotNull().NotEmpty().GreaterThan(Guid.Empty).NotEqual(Guid.Empty);
			RuleFor(x => x.Quantity).GreaterThan(0).NotEmpty().NotNull();
		}
	}
}
