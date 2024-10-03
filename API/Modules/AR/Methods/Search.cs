using App;
using App.Routing;
using AR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace AR.Methods;

/// <summary>Adresų paieškos modelis</summary>
public static class Search {
	private static readonly int pagelimit = 50;

	private static List<string>? CachedRemTypes { get; set; }
	/// <summary>Paieškos frazių pašalinimo sąrašas</summary>
	public static List<string> RemTypes { get {
			//TODO: Reload CACHE!!!
			if (CachedRemTypes is null) {
				using var db = new DBRead("SELECT tipas FROM ar.v_app_types;");
				using var rdr = db.GetReader().Result;
				var ret = new List<string>(); while (rdr.Read()) ret.AddN(rdr.GetStringN(0)?.RemoveAccents().RemoveNonAlphanumeric(true)); CachedRemTypes = ret;
			}
			return CachedRemTypes;
	} }



	private static string? MkSerach(this string? q) => q?.RemoveAccents().RemoveNonAlphanumeric(true).ToLower().RemWords(AR.Methods.Search.RemTypes);


	private static readonly List<string> SrhFields = ["ID", "Pavad", "Vietove", "Tipas", "Src", "Adm", "Sav", "Sen", "Gyv", "Gat", "Aob", "search", "sort"];
	private static readonly List<string> SrhSelect = ["ID","Pavad","Vietove","Tipas"];

	/// <summary>Detali adresų paieška</summary>
	/// <param name="ctx"></param>
	/// <param name="q">Paieškos užklausa</param>
	/// <returns></returns>
	public static async Task FullSearch(HttpContext ctx, AR_SearchQuery q) {
		var m = new DBPagingRequest<AR_SearchItem>("ar.v_app_search" + (q.Full ? "_full" : "")) {
			Limit = q.Top?.Limit(pagelimit) ?? 10, Page = 1, Sort = "sort", Desc = true,
			Fields = SrhFields, Select = SrhSelect, Search = MkSerach(q.Search), Total = false
		};
		if (q.Filter is not null) {
			var f = q.Filter;
			m.Where = new() { Adm = f.Adm, Sav = f.Sav, Sen = f.Sen, Gyv = f.Gyv, Gat = f.Gat, Aob = f.Aob };
		}
		if (q.Type is not null) {
			if (q.Type == AR_SearchTypes.Adr) m.Table = "ar.v_app_search_adr";
			else (m.Where ??= new()).Src = q.Type.ToString()?.ToLower();
		}
		var ret = await m.Execute();
		await ctx.Response.WriteAsJsonAsync(ret.Data);
	}



	/// <summary>2 - Savivaldybių paieška (SAV)</summary>
	/// <param name="ctx"></param>
	/// <param name="q">Paieškos frazė</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <returns></returns>
	public static async Task Sav(HttpContext ctx, string q, int top = 10) => await ctx.Response.WriteAsJsonAsync(await GetSrh(q, top, "sav"));
	/// <summary>3 - Seniunijų paieška (SEN)</summary>
	/// <param name="ctx"></param>
	/// <param name="q">Paieškos frazė</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <returns></returns>
	public static async Task Sen(HttpContext ctx, string q, int top = 10) => await ctx.Response.WriteAsJsonAsync(await GetSrh(q, top, "sen"));
	/// <summary>4 - Gyvenviečių paieška (GYV)</summary>
	/// <param name="ctx"></param>
	/// <param name="q">Paieškos frazė</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <returns></returns>
	public static async Task Gyv(HttpContext ctx, string q, int top = 10) => await ctx.Response.WriteAsJsonAsync(await GetSrh(q, top, "gyv"));
	/// <summary>5 - Gatvių paieška (GAT)</summary>
	/// <param name="ctx"></param>
	/// <param name="gyv">Gyvenvietės kodas</param>
	/// <param name="q">Paieškos frazė</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <returns></returns>
	public static async Task Gat(HttpContext ctx, int gyv, string q, int top = 10) => await ctx.Response.WriteAsJsonAsync(await GetSrh(q, top, "gat", gyv));
	/// <summary>6 - Adresų paieška (AOB)</summary>
	/// <param name="ctx"></param>
	/// <param name="gyv">Gyvenvietės kodas</param>
	/// <param name="q">Paieškos frazė</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <returns></returns>
	public static async Task Aob(HttpContext ctx, int gyv, string q, int top = 10) => await ctx.Response.WriteAsJsonAsync(await GetSrh(q, top, "aob",gyv));
	/// <summary>Adresų paieška (GAT+AOB+PAT)</summary>
	/// <param name="ctx"></param>
	/// <param name="gyv">Gyvenvietės kodas</param>
	/// <param name="q">Paieškos frazė</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <returns></returns>
	public static async Task Adr(HttpContext ctx, int gyv, string q, int top = 10) {
		var m = await new DBPagingRequest<AR_SearchItem>("ar.v_app_search_adr") {
			Limit = top.Limit(pagelimit), Page = 1, Sort = "sort", Desc = true, Where = new() { Gyv = gyv },
			Fields = SrhFields, Select = SrhSelect, Total = false, Search = MkSerach(q)
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(new AR_Search() { Data = m.Data });
	}

	private static async Task<AR_Search> GetSrh(string q, int top, string src, int? gyv = null) {
		var ret = await new DBPagingRequest<AR_SearchItem>("ar.v_app_search") {
			Limit = top.Limit(pagelimit), Page = 1, Sort = "sort", Desc = true, Where = new() { Src = src, Gyv = gyv },
			Fields = SrhFields, Select = SrhSelect, Total = false, Search = MkSerach(q)
		}.Execute();
		return new AR_Search() { Data = ret.Data };
	}
}
