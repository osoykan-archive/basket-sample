using System;

namespace BasketContext.Api.Models.Responses
{
	public class GetTokenResponse
	{
		public string Token { get; set; }

		public DateTimeOffset ExpiredDate { get; set; }
	}
}
