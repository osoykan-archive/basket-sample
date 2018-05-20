using System;
using System.Collections.Generic;
using System.Linq;

using BasketContext.Api.Controllers;
using BasketContext.Api.Plumbing;
using BasketContext.Domain.CommandHandlers;
using BasketContext.Domain.Database;

using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json.Serialization;

using NSwag.AspNetCore;

using Swashbuckle.AspNetCore.Swagger;

namespace BasketContext.Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			AddMvc(services);
			AddSwagger(services);
			ConfigureAuthorization(services);
			services.AddLogging();
			services.AddMediatR(typeof(BasketCommandHandler).Assembly);

			services.AddDbContextPool<BasketDbContext>(builder => builder.UseInMemoryDatabase("BasketContext"));
		}

		private void AddMvc(IServiceCollection services)
		{
			services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });
			services
				.AddMvc(options =>
				{
					options.Filters.Add<ModelStateFilter>();
					options.Filters.Add<GlobalExceptionHandler>();
				})
				.AddFluentValidation(configuration =>
				{
					configuration.RegisterValidatorsFromAssembly(GetType().Assembly);
				})
				.AddJsonOptions(options =>
				{
					options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
				})
				.AddControllersAsServices();
		}

		private static void AddSwagger(IServiceCollection services)
		{
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "BasketApi", Version = "v1" });

				c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
				{
					{ "Bearer", Enumerable.Empty<string>() }
				});

				c.AddSecurityDefinition("Bearer",
					new ApiKeyScheme { In = "header", Name = "Authorization", Type = "apiKey" });

				c.DescribeAllEnumsAsStrings();
			});
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseAuthentication();
			app.UseCors(builder =>
			{
				builder.AllowAnyHeader()
				       .AllowAnyMethod()
				       .AllowAnyOrigin()
				       .AllowCredentials()
				       .Build();
			});

			app.UseSwagger()
			   .UseSwaggerUI(
				   c =>
				   {
					   c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket Api v1");
				   })
			   .UseMvc();

			lifetime.ApplicationStarted.Register(() =>
			{
				using (var basketContext = app.ApplicationServices.CreateScope().ServiceProvider.GetService<BasketDbContext>())
				{
					basketContext.Database.EnsureCreated();
				}
			});

			lifetime.ApplicationStopped.Register(() =>
			{
				using (var basketContext = app.ApplicationServices.CreateScope().ServiceProvider.GetService<BasketDbContext>())
				{
					basketContext.Database.EnsureDeleted();
				}
			});
		}

		private void ConfigureAuthorization(IServiceCollection services)
		{
			services.AddAuthorization();
			services
				.AddAuthentication(o =>
				{
					o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(x =>
				{
					x.SaveToken = true;
					x.RequireHttpsMetadata = false;
					x.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Shared.Key()),
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateLifetime = true,
						ClockSkew = TimeSpan.FromMinutes(5)
					};
				});
		}
	}
}
