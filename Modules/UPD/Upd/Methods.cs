using App.Routing;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace Registrai.Modules.UPD;

/// <summary>UPD duomenų gavimo metodai</summary>
public static class UpdMedicines {
	/// <summary>VVR registro adresas</summary>
	public static string? Endpoint { get; set; }
	private static HttpClient? Client { get; set; }
	private static HttpClient GetClient() => Client ??= new() { BaseAddress = new(Endpoint ?? "http://localhost:5507/pub/v1") };

	private static async Task PassResponse(HttpContext ctx, HttpResponseMessage rsp) {
		ctx.Response.StatusCode = (int)rsp.StatusCode;
		foreach (var i in rsp.Headers) if (!IsBlacklistedHeader(i.Key)) ctx.Response.Headers[i.Key] = i.Value.ToArray();
		if (rsp.Content != null) {
			foreach (var i in rsp.Content.Headers) if (!IsBlacklistedHeader(i.Key)) ctx.Response.Headers[i.Key] = i.Value.ToArray();
			await rsp.Content.CopyToAsync(ctx.Response.Body);
		}
	}

	private static readonly string[] _blacklist = ["Content-Length", "Transfer-Encoding", "Keep-Alive", "Connection", "Host"];
	private static bool IsBlacklistedHeader(string headerName) { 
		foreach (var i in _blacklist) if (headerName.Equals(i, System.StringComparison.OrdinalIgnoreCase)) return true; return false; 
	}

	/// <summary>Vaistų sąrašas</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task List(HttpContext ctx) {
		var cli = GetClient();
		using var rsp = await cli.GetAsync($"med{ctx.Request.QueryString}");
		await PassResponse(ctx, rsp);
	}

	/// <summary>Vaistų sąrašas (detali paieška)</summary>
	/// <param name="ctx"></param>
	/// <param name="qry">Filtro užklausa</param>
	/// <returns></returns>
	public static async Task ListFilter(HttpContext ctx, MedQuery qry) {
		var cli = GetClient();
		using var rsp = await cli.PostAsJsonAsync($"med{ctx.Request.QueryString}", qry);
		await PassResponse(ctx, rsp);
	}

	/// <summary>Filtrų sąrašas</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task Filters(HttpContext ctx) {
		var q = ctx.ParamString("lang")?.ToLower() == "en" ? "?lang=en" : "";
		var cli = GetClient();
		using var rsp = await cli.GetAsync($"med/filters{q}");
		await PassResponse(ctx, rsp);
	}

	/// <summary>Gauti vaistą pagal ID</summary>
	/// <param name="ctx"></param>
	/// <param name="id"></param>
	/// <returns></returns>
	public static async Task Item(HttpContext ctx, string id) {
		if (long.TryParse(id, out _)) {
			var q = ctx.ParamString("lang")?.ToLower() == "en" ? "?lang=en" : "";
			var cli = GetClient();
			using var rsp = await cli.GetAsync($"med/{id}{q}");
			await PassResponse(ctx, rsp);
		}
		else ctx.Response.E404();
	}

	/// <summary>Atsisiųsti pakuotės lapelį</summary>
	/// <param name="ctx"></param>
	/// <param name="med">Vaisto ID</param>
	/// <param name="file">Failo ID</param>
	/// <returns></returns>
	public static async Task DownloadFile(HttpContext ctx, long med, string file) {
		if (Guid.TryParse(file, out _)) {
			var cli = GetClient();
			using var rsp = await cli.GetAsync($"doc/{med}/{file}");
			await PassResponse(ctx, rsp);
		}
		else ctx.Response.E404();
	}
}
