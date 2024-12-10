using App.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
#if DEBUG
	using Microsoft.OpenApi.Models;
#endif

namespace App.Routing;


public enum Method { Get, Post, Put, Patch, Delete }

public class AppRouteEndpoint {
	public string? Name { get; set; } = "EVRK Kodai";
	public string? Description { get; set; }
	public string? Version { get; set; } = "v1";
	public string? Tag { get; set; } = "evrk";
    public List<Route2>? Routes { get; set; }
}


public class Route<T>(Delegate hnd, string path="") {
	public string? Summary { get; set; }
	public Method Method { get; set; } = Method.Get;
	public string Path { get; set; } = path;
	public string? Group { get; set; }
	public string? Description { get; set; }
	public int Status { get; set; } = 200;
	public string Tag { get; set; } = "v1";
	public Type Type { get; set; }
	public List<int>? Errors { get; set; }
	public List<RouteParam>? Params { get; set; }
	public Delegate Handler { get; set; } = hnd;
}



public class Route2(string path, Delegate hnd) {
	public string? Summary { get; set; }
	public Method Method { get; set; } = Method.Get;
	public string Path { get; set; } = path;
	public string? Group { get; set; }
	public string? Description { get; set; }
	public int Status { get; set; } = 200;
	public string Tag { get; set; } = "v1";
	public Type Response { get; set; }
	public List<int>? Errors { get; set; }
	public List<RouteParam>? Params { get; set; }
	public Delegate Handler { get; set; } = hnd;
}

public enum RouteParamType { String, Boolean, Integer, Number }
public class RouteParam(string name) {
	public string Name { get; set; } = name;
	public string? Description { get; set; }
	public RouteParamType Type { get; set; } = RouteParamType.Boolean;
	public bool Required { get; set; }
#if DEBUG
	public OpenApiParameter GetParam() => new() { Name = Name, In = ParameterLocation.Query, Description = Description, Required = Required, Schema = new() { Type = Type.ToString().ToLower() } };
#endif
}

public static class Routing {
	/// <summary>Extension for route handler to add swagger info for dev environment</summary>
	/// <param name="rtx">Route handler</param>
	/// <param name="route">Route details</param>
	/// <param name="group">Route group name</param>
	/// <returns>Route handler</returns>
	public static RouteHandlerBuilder Swagger<T>(this RouteHandlerBuilder rtx, Route<T> route) {
#if DEBUG //Disable swagger
		var op = new Microsoft.OpenApi.Models.OpenApiOperation();
		rtx.WithOpenApi(o => {
			o.Summary = route.Summary; o.Description = route.Description; o.Tags = route.Group is null ? null : [new() { Name = route.Group }];
			if (route.Params is not null) foreach (var i in route.Params) o.Parameters.Add(i.GetParam()); return o;
		});
		rtx.WithMetadata(new EndpointGroupNameAttribute(route.Tag));

#endif
		return rtx;
	}

	public static RouteHandlerBuilder Swagger(this RouteHandlerBuilder rtx, Route2 route) {
#if DEBUG //Disable swagger
		var op = new Microsoft.OpenApi.Models.OpenApiOperation();
		rtx.WithOpenApi(o => {
			o.Summary = route.Summary; o.Description = route.Description; o.Tags = route.Group is null ? null : [new() { Name = route.Group }];
			if (route.Params is not null) foreach (var i in route.Params) o.Parameters.Add(i.GetParam()); return o;
		});
		rtx.WithMetadata(new EndpointGroupNameAttribute(route.Tag));

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