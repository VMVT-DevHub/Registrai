using System.Text.Json.Serialization;
using App.Routing;
using Microsoft.AspNetCore.Diagnostics;

namespace Registrai.App;

/// <summary>Application initial startup class</summary>
public static class Startup {
	/// <summary></summary>
	public static string ConnStr { get; set; } = "";

	private static List<AppRouteEndpoint> Endpoints { get; set; } = [];

	/// <summary>Add new endpoint mapping</summary>
	/// <param name="endpoint"></param>
	public static void AddEndpoint(AppRouteEndpoint endpoint) { Endpoints.Add(endpoint); }

	/// <summary>Build minimal API app</summary>
	/// <param name="args">primary execution arguments</param>
	/// <returns>WebApplication</returns>
	public static WebApplication Build(string[] args) {
		var builder = WebApplication.CreateBuilder(args);
		builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

		ConnStr = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

		#if DEBUG //Disable Swagger
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c => {
				c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Registrai API", Version = "v1" });
				foreach (var i in Endpoints)
					c.SwaggerDoc($"{i.Tag}_{i.Version}", new() { Title = i.Name, Version = i.Version, Description = i.Description });
				foreach (var i in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
					c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, i));
			});
			builder.Services.AddMvc().AddJsonOptions(opt => {
				//opt.JsonSerializerOptions.PropertyNamingPolicy = null;
				opt.JsonSerializerOptions.WriteIndented = false;
				opt.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
			});
		#endif

		builder.Services.ConfigureHttpJsonOptions(a => {
			a.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			//a.SerializerOptions.PropertyNamingPolicy=null; 
			a.SerializerOptions.WriteIndented = false;
			a.SerializerOptions.Converters.Add(new CustomDateTimeConverter());
			a.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
		});



		var app = builder.Build();

		app.UseExceptionHandler(exh => exh.Run(HandleError));

		#if DEBUG //Disable Swagger
			app.UseSwagger();
			app.UseSwaggerUI(c => {
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Registrai API v1");
				foreach (var i in Endpoints) {
					c.SwaggerEndpoint($"/swagger/{i.Tag}_{i.Version}/swagger.json", i.Name + " " + i.Version);
				}
			});
		#endif

		foreach (var i in Endpoints) 
			if (i.Routes?.Count > 0) 
				foreach (var j in i.Routes) {
					j.Tag = $"{i.Tag}_{i.Version}";
					app.Attach(j);
				}

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
