using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using BasketContext.Api.Models.Requests;
using BasketContext.Api.Models.Responses;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BasketContext.Api.Controllers
{
	[Route("/api/token")]
	public class TokenController : Controller
	{
		[HttpPost]
		[Route("/generate")]
		public IActionResult Post([FromBody] GetTokenRequest request)
		{
			if (IsValid(request.Username, request.Password))
			{
				return Ok(Generate(request.Username));
			}

			return BadRequest();
		}

		private static bool IsValid(string username, string password) =>
			!string.IsNullOrEmpty(username) && username == password;

		private static GetTokenResponse Generate(string username)
		{
			var dtExpired = new DateTimeOffset(DateTime.Now.AddDays(1));

			var claims = new Claim[]
			{
				new Claim(ClaimTypes.Name, username),
				new Claim(ClaimTypes.Email, $"{username}@basketapi.com"),
				new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
				new Claim(JwtRegisteredClaimNames.Exp, dtExpired.ToUnixTimeSeconds().ToString())
			};

			var token = new JwtSecurityToken(
				new JwtHeader(new SigningCredentials(new SymmetricSecurityKey(Shared.Key()), SecurityAlgorithms.HmacSha256)),
				new JwtPayload(claims));

			return new GetTokenResponse
			{
				ExpiredDate = dtExpired,
				Token = new JwtSecurityTokenHandler().WriteToken(token)
			};
		}
	}
}
