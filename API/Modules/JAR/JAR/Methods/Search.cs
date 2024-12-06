using App;
using App.Routing;
using Microsoft.AspNetCore.Http;
using Registrai.Modules.JAR.Models;

namespace Registrai.Modules.JAR.Methods;

/// <summary>Juridinių asmenų paieškos modelis</summary>
public static class JARSearch {
    private static readonly int pagelimit = 50;

    private static List<string> CachedRemTypes { get; set; } = [];
    /// <summary>Paieškos frazių pašalinimo sąrašas</summary>
    public static List<string> RemTypes => CachedRemTypes;

	private static string? MkSerach(this string? q) => q?.RemoveAccents().RemoveNonAlphanumeric(true).ToLower().RemWords(RemTypes);


    private static readonly List<string> SrhFields = ["ID", "Pavad", "Adresas", "Statusas", "Forma", "AobKodas", "FormKodas", "StatusKodas", "Active", "search", "sort"];
	private static readonly List<string> SrhSelect1 = ["ID", "Pavad", "Adresas", "Statusas", "Forma"];
	private static readonly List<string> SrhSelect2 = ["ID", "Pavad"];


	/// <summary>Supaprastinta juridinių asmenų paieška</summary>
	/// <param name="ctx"></param>
	/// <param name="q">Paieškos užklausa</param>
	/// <param name="top">Įrašų skaičius</param>
	/// <param name="status">Statuso kodo filtravimas</param>
	/// <param name="details">Rodyti daugiau informacijos</param>
	/// <param name="active">Tik aktyvūs juridiniai asmenys</param>
	/// <returns></returns>
	public static async Task GetSrh(HttpContext ctx, string q, int top=10, bool? details=false, bool? active=false, int? status=null) {
		var srt = false;
		if (q.Length > 6 && long.TryParse(q, out var num) && num > 1e5) { srt = true; q = num.ToString(); }
		else { q = q.MkSerach() ?? ""; }

		var ret = await new DBPagingRequest<JAR_SearchItem>("jar.v_app_search_" + (srt ? "id" : "name")) {
			Limit = top.Limit(pagelimit),
			Page = 1,
			StartsWith = srt,
			Sort = "sort",
			Where = new() { StatusKodas = status, Active = active ?? true ? true : null },
			Fields = SrhFields,
			Select = details ?? true ? SrhSelect1 : SrhSelect2,
			Total = false,
			Search = q
		}.Execute();

		await ctx.Response.WriteAsJsonAsync(ret.Data);
	}

	/// <summary>Detali juridinių asmenų paieška</summary>
	/// <param name="ctx"></param>
	/// <param name="q">Paieškos užklausa</param>
	/// <returns></returns>
	public static async Task FullSearch(HttpContext ctx, JAR_SearchQuery q) {
		var srt = false; var qs = q.Search;
		if (qs?.Length > 6 && long.TryParse(q.Search, out var num) && num > 1e5) { srt = true; qs = num.ToString(); }
		else { qs = qs.MkSerach() ?? ""; }

		var m = new DBPagingRequest<JAR_SearchItem>("jar.v_app_search_" + (srt ? "id" : "name")) {
			Limit = q.Top?.Limit(pagelimit) ?? 10,
			Page = 1,
			Sort = "sort",
			Fields = SrhFields,
			Select = q.Detales ?? true ? SrhSelect1 : SrhSelect2,
			Search = qs,
			Total = false
		};


		if (q.Filter is not null) {
			var f = q.Filter;
			m.Where = new() { AobKodas = f.AobKodas, FormKodas = f.FormKodas, StatusKodas = f.StatusKodas };
		}
		if (q.Active == true) (m.Where ??= new()).Active = true;

		var ret = await m.Execute();
		await ctx.Response.WriteAsJsonAsync(ret.Data);
	}
}
