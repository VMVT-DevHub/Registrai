using System.Text.Json.Serialization;
#if DEBUG
	using Microsoft.OpenApi.Models;
#endif

namespace App.Routing;


public class RouteDefinition(string name) {
	public string Name { get; set; } = name;
	public string? Description { get; set; }
	public string? Version { get; set; } = "v1";
	public string? Tag { get; set; }
	[JsonIgnore] public string? Path => (string.IsNullOrEmpty(Tag) ? "" : Tag + "_") + Version;
	public List<RouteGroup> Routes { get; set; } = [];
	public RouteDefinition Add(RouteGroup group) { Routes.Add(group); return this; }
}

public class RouteGroup(string name) {
	public string Name { get; set; } = name;
	public List<Route> Routes { get; } = [];
	public RouteGroup Map(Route route) { Routes.Add(route); return this; }
}

public enum Method { Get, Post, Put, Patch, Delete }
public class Route(string path, Delegate hnd, Method method = Method.Get) {
	public string? Name { get; set; }
	public string? Summary { get; set; }
	public Method Method { get; set; } = method;
	public string Path { get; set; } = path;
	public int Status { get; set; } = 200;
	[JsonIgnore] public Type? Response { get; set; }
	public List<int>? Errors { get; set; }
	public bool? Deprecated { get; set; }
	[JsonIgnore] public List<RouteParam>? Params { get; set; }
	[JsonIgnore] public Delegate Handler { get; set; } = hnd;
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
