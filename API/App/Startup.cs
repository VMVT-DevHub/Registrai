using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;

namespace Registrai.App;

/// <summary>Application initial startup class</summary>
public static class Startup {
	/// <summary></summary>
	public static string ConnStr { get; set; } = "";

	/// <summary>Build minimal API app</summary>
	/// <param name="args">primary execution arguments</param>
	/// <returns>WebApplication</returns>
	public static WebApplication Build(string[] args){
		var builder = WebApplication.CreateBuilder(args);
		builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

		ConnStr = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

		#if DEBUG //Disable Swagger
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen( c => {
				c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Registrai API", Version = "v1" });
				foreach (var i in Directory.GetFiles(AppContext.BaseDirectory, "*.xml")) 
					c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, i));
			});
			builder.Services.AddMvc().AddJsonOptions(opt => { 
				//opt.JsonSerializerOptions.PropertyNamingPolicy = null;
				opt.JsonSerializerOptions.WriteIndented=false; 
				opt.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
				});
		#endif
		
		builder.Services.ConfigureHttpJsonOptions( a => {
			a.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			//a.SerializerOptions.PropertyNamingPolicy=null; 
			a.SerializerOptions.WriteIndented=false;
			a.SerializerOptions.Converters.Add(new CustomDateTimeConverter());
			a.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
		});



		var app = builder.Build();

		app.UseExceptionHandler(exh=>exh.Run(HandleError));

		#if DEBUG //Disable Swagger
			app.UseSwagger();
			app.UseSwaggerUI();
		#endif
		return app;
	}


	private static async Task HandleError(HttpContext ctx){
		var rsp = ctx.Response;

		var ex = ctx.Features.Get<IExceptionHandlerFeature>();

		if(ex is not null && ex.Error is not null){
			await rsp.WriteAsync("Error...");
		}
	}

}
