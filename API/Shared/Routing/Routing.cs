using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace App.Routing;

public enum Method { Get, Post, Put, Patch, Delete }

public class Route<T>(Delegate hnd) {
	public Method Method { get; set; } = Method.Get;
	public string Path { get; set; } = "";
	public string? Tag { get; set; }
	public string? Description { get; set; }
	public int Status { get; set; } = 200;
	public List<int>? Errors { get; set; }
    public List<RouteParam>? Params { get; set; }
    public Delegate Handler { get; set; } = hnd;
}
#if DEBUG
public enum RouteParamType { String, Boolean, Integer, Number }
public class RouteParam(string name) {
	public string Name { get; set; } = name;
	public string? Description { get; set; }
	public RouteParamType Type { get; set; } = RouteParamType.String;
	public bool Required { get; set; }
	public OpenApiParameter GetParam() => new() { Name = Name, In = ParameterLocation.Query, Description = Description, Required = Required, Schema = new() { Type = Type.ToString().ToLower() } };
}
#endif

public static class Routing {
	/// <summary>Extension for route handler to add swagger info for dev environment</summary>
	/// <param name="rtx">Route handler</param>
	/// <param name="summary">API Summary</param>
	/// <param name="desc">API Description</param>
	/// <param name="tag">API Tag name</param>
	/// <returns>Route handler</returns>
	public static RouteHandlerBuilder Swagger(this RouteHandlerBuilder rtx, string summary, string? desc = null, string? tag = null, List<RouteParam>? prm = null) {
#if DEBUG //Disable swagger
		var op = new OpenApiOperation();
		rtx.WithOpenApi(o => {
			o.Summary = summary; o.Description = desc; o.Tags = tag is null ? null : [new() { Name = tag }];
			if (prm is not null) foreach (var i in prm) o.Parameters.Add(i.GetParam()); return o;
		});
#endif
		return rtx;
	}


	/// <summary>Registruoti API atsakymo klaidas</summary>
	/// <param name="builder"></param><param name="err"></param><returns></returns>
	public static RouteHandlerBuilder Errors(this RouteHandlerBuilder builder, params int[] err) {
		foreach (var i in err) {
			switch (i) {
				case 401: builder.Produces<E401>(401); break;
				case 403: builder.Produces<E403>(403); break;
				case 404: builder.Produces<E404>(404); break;
				case 422: builder.Produces<E422>(422); break;
			}
		}
		return builder;
	}

	/// <summary>Registruoti API atsakymo formatą</summary>
	/// <typeparam name="T">Formatas</typeparam><param name="builder"></param><returns></returns>
	public static RouteHandlerBuilder Response<T>(this RouteHandlerBuilder builder) => builder.Produces<T>(200);

	/// <summary>Registruoti API atsakymo formatą</summary>
	/// <typeparam name="T">Formatas</typeparam><param name="builder"></param>
	/// <param name="main">Pagrindinis atsakymo statusas</param>
	/// <param name="err">Klaidos kodai</param><returns></returns>
	public static RouteHandlerBuilder Response<T>(this RouteHandlerBuilder builder, int main = 200, params int[] err) => builder.Produces<T>(main).Errors(err);
}