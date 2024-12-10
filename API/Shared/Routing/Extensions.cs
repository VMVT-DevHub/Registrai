using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace App.Routing;

/// <summary>Plėtiniai</summary>
public static partial class Extensions {
	/// <summary>Pridėti API maršrutą</summary>
	/// <typeparam name="T">Atsakymo tipas</typeparam>
	/// <param name="app"></param>
	/// <param name="route">Maršrutas</param>
	/// <returns></returns>
	public static WebApplication Attach<T>(this WebApplication app, Route<T> route) {
		var bld = route.Method switch {
			Method.Get => app.MapGet(route.Path, route.Handler),
			Method.Post => app.MapPost(route.Path, route.Handler),
			Method.Put => app.MapPut(route.Path, route.Handler),
			Method.Patch => app.MapPatch(route.Path, route.Handler),
			Method.Delete => app.MapDelete(route.Path, route.Handler),
			_ => app.Map(route.Path, route.Handler),
		};
#if DEBUG //Disable Swagger
		bld.Swagger(route).Produces<T>(route.Status);
		if (route.Errors?.Count > 0) { bld.Errors([.. route.Errors]); }
#endif
		return app;
	}

	public static WebApplication Attach(this WebApplication app, Route2 route) {
		var bld = route.Method switch {
			Method.Get => app.MapGet(route.Path, route.Handler),
			Method.Post => app.MapPost(route.Path, route.Handler),
			Method.Put => app.MapPut(route.Path, route.Handler),
			Method.Patch => app.MapPatch(route.Path, route.Handler),
			Method.Delete => app.MapDelete(route.Path, route.Handler),
			_ => app.Map(route.Path, route.Handler),
		};
#if DEBUG //Disable Swagger
		
		bld.Swagger(route).Produces(route.Status, route.Response);
		if (route.Errors?.Count > 0) { bld.Errors([.. route.Errors]); }
#endif
		return app;
	}

	public static bool ParamTrue(this HttpContext ctx, string prm) => ctx.Request.Query.TryGetValue(prm, out var flg) && (string.IsNullOrEmpty(flg) || flg == "1" || (bool.TryParse(flg, out var b3) && b3));
	public static bool ParamNull(this HttpContext ctx, string prm) => !ctx.Request.Query.TryGetValue(prm, out var flg) || string.IsNullOrEmpty(flg);


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
