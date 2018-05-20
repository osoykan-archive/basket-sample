using System;

namespace BasketContext.Api.Models.Requests
{
	[Serializable]
	public class GetTokenRequest
	{
		public string Password { get; set; }

		public string Username { get; set; }
	}
}
