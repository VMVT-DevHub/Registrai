using App.Routing;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;

namespace Registrai.App;

/// <summary>Application initial startup class</summary>
public static class Startup {
	/// <summary></summary>
	public static string ConnStr { get; set; } = "";

	private static List<RouteDefinition> Endpoints { get; } = [];
	/// <summary></summary>
	/// <param name="route"></param>
	public static void Routes(params RouteDefinition[] route) { Endpoints.AddRange(route); }

	/// <summary>Build minimal API app</summary>
	/// <param name="args">primary execution arguments</param>
	/// <returns>WebApplication</returns>
	public static WebApplication Build(string[] args) {
		var builder = WebApplication.CreateBuilder(args);
		builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

		ConnStr = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

		Endpoints.Insert(0, new("Registrai") {
			Description = "Bendra VMVT registrų informacinė sistema priklausanti OKIS posistemei", Version = "v1",
			Routes = [new RouteGroup("Registrai").Map(new("/info", () => Endpoints) { Name = "Bendra informacija apie registrą" })]
		});

		builder.Services.AddSwagger(Endpoints);

		builder.Services.ConfigureHttpJsonOptions(a => {
			a.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			//a.SerializerOptions.PropertyNamingPolicy=null; 
			a.SerializerOptions.WriteIndented = false;
			a.SerializerOptions.Converters.Add(new CustomDateTimeConverter());
			a.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
		});

		var app = builder.Build();
		app.UseExceptionHandler(exh => exh.Run(HandleError));
		app.UseRouteEndpoints(Endpoints);

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
