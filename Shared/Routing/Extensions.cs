using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace App.Routing;

/// <summary>Plėtiniai</summary>
public static partial class Extensions {

	public static void UseRouteEndpoints(this WebApplication app, List<RouteDefinition> routes) {
#if DEBUG //Disable Swagger
		app.UseSwagger();
		app.UseSwaggerUI(c => {
			foreach (var i in routes) {
				c.SwaggerEndpoint($"/swagger/{i.Path}/swagger.json", i.Name + " " + i.Version);
				c.InjectStylesheet("/swagger-custom.css");
			}
		});
#endif

		foreach (var i in routes) {
			var eps = 0;
			foreach (var j in i.Routes) {
				foreach (var k in j.Routes) {
					var m = app.Attach(k); eps++;
#if DEBUG //Disable Swagger
					m.Produces(k.Status, k.Response)
					.WithOpenApi(o => {
						o.Summary = k.Summary; o.Description = k.Name; o.Tags = [new() { Name = j.Name }]; o.Deprecated = k.Deprecated ?? false;
						if (k.Params is not null) foreach (var i in k.Params) o.Parameters.Add(i.GetParam()); return o;
					}).WithMetadata(new EndpointGroupNameAttribute($"{i.Path}"));
					if (k.Errors?.Count > 0) { m.Errors([.. k.Errors]); }
#endif
				}
			}
			Console.WriteLine($"Endpoint: {i.Name} {i.Path} / {eps}");
		}

#if DEBUG //Disable Swagger
		app.MapGet("swagger-custom.css", ()=> ".model-container { margin: 5px 10px !important; }\r\n.model-box { padding: 5px 10px !important; }\r\n.swagger-ui .info { margin:20px 10px !important; }\r\n").ExcludeFromDescription();
#endif
	}

	public static IServiceCollection AddSwagger(this IServiceCollection services, List<RouteDefinition> routes) {

#if DEBUG //Disable Swagger
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(c => {
			foreach (var i in routes)
				c.SwaggerDoc($"{i.Path}", new() { Title = i.Name, Version = i.Version, Description = i.Description });
			foreach (var i in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
				c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, i));
		});
#endif
		return services;
	}

	public static RouteHandlerBuilder Attach(this WebApplication app, Route route) => route.Method switch {
		Method.Get => app.MapGet(route.Path, route.Handler),
		Method.Post => app.MapPost(route.Path, route.Handler),
		Method.Put => app.MapPut(route.Path, route.Handler),
		Method.Patch => app.MapPatch(route.Path, route.Handler),
		Method.Delete => app.MapDelete(route.Path, route.Handler),
		_ => app.Map(route.Path, route.Handler),
	};

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


	public static bool ParamTrue(this HttpContext ctx, string prm) => ctx.Request.Query.TryGetValue(prm, out var flg) && (string.IsNullOrEmpty(flg) || flg == "1" || (bool.TryParse(flg, out var b3) && b3));
	public static bool ParamNull(this HttpContext ctx, string prm) => !ctx.Request.Query.TryGetValue(prm, out var flg) || string.IsNullOrEmpty(flg);
	public static int ParamInt(this HttpContext ctx, string prm) => ctx.Request.Query.TryGetValue(prm, out var flg) && !string.IsNullOrEmpty(flg) && int.TryParse(flg, out var num) ? num : 0;
	public static int? ParamIntN(this HttpContext ctx, string prm) => ctx.Request.Query.TryGetValue(prm, out var flg) && !string.IsNullOrEmpty(flg) && int.TryParse(flg, out var num) ? num : null;
	public static long ParamLong(this HttpContext ctx, string prm) => ctx.Request.Query.TryGetValue(prm, out var flg) && !string.IsNullOrEmpty(flg) && long.TryParse(flg, out var lng) ? lng : 0;
	public static long? ParamLongN(this HttpContext ctx, string prm) => ctx.Request.Query.TryGetValue(prm, out var flg) && !string.IsNullOrEmpty(flg) && long.TryParse(flg, out var lng) ? lng : null;
	public static string? ParamString(this HttpContext ctx, string prm) => ctx.Request.Query.TryGetValue(prm, out var flg) ? flg.FirstOrDefault() : null;


	public static int Limit(this int num, int max) => num > max ? max : num;


	public static string RemoveAccents(this string text) {
		var str = text.Normalize(NormalizationForm.FormD);
		var sb = new StringBuilder(capacity: str.Length);
		for (int i = 0; i < str.Length; i++) {
			char c = str[i];
			if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) sb.Append(c);
		}
		return sb.ToString().Normalize(NormalizationForm.FormC);
	}

	public static string RemoveNonAlphanumeric(this string text, bool dash = false) {
		if (string.IsNullOrEmpty(text)) return text;
		var sb = new StringBuilder();
		foreach (char c in text) {
			//if (exc is not null && exc.Contains(c)) sb.Append(c); else 
			sb.Append(char.IsLetterOrDigit(c) || (dash && c=='-') ? c : ' ');
		}
		return RgxMultiSpace().Replace(sb.ToString(), " ").Trim();
	}

	public static string RemWords(this string text, List<string> words) {
		var sp = text.Split(" "); var ret = new List<string>();
		foreach(var i in sp) if (!words.Contains(i)) ret.Add(i);
		return string.Join(" ", ret);
	}

	[GeneratedRegex(@"\s+")] private static partial Regex RgxMultiSpace();


	public static string AddN(this List<string> lst, params string?[] val) {
		var ls = val.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray(); var ret = "";
		if (ls.Length > 0) { ret = string.Join(" ", ls); lst.Add(ret); }
		return ret;
	}
}
