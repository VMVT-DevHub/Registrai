using App;
using App.Routing;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Registrai.Modules.EVRK.Models;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Registrai.Modules.EVRK.Methods;

/// <summary>Adreso detalių gavimo metodas</summary>
public static partial class Evrk {
	//[GeneratedRegex(@"[^\w\-_\.]")] private static partial Regex FileNameRgx();
	private static string FlagDir(string name) => Path.Combine(AppContext.BaseDirectory, "Content", "Flags", $"{name}.svg");


	static readonly List<string> ListFld = ["ID", "Sekcija", "Kodas", "Pavad", "Parent", "Layer", "Last", "search"];
	static readonly List<string> ListSel = ["ID", "Sekcija", "Kodas", "Pavad", "Parent", "Layer", "Last"];

	/// <summary>Gauti pilną sąrašą</summary>
	/// <param name="ctx"></param>
	/// <param name="page">Puslapis</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <param name="parent">Aukštesnio lygmens elementas</param>
	/// <param name="order">Rikiavimas</param>
	/// <returns></returns>
	public static async Task List(HttpContext ctx, int page = 1, int top = 25, string? parent=null, string? order = null) {
		var m = await new DBPagingRequest<Evrk_Item>("public.v_app_evrk") {
			Limit = top.Limit(1000), Page = page, Sort = order ?? "ID", Desc = ctx.ParamTrue("desc"),
			Where = string.IsNullOrEmpty(parent) ? (ctx.ParamTrue("l1") || parent == "" ? new() { Layer = 1 } : null) : new() { Parent = parent }, Fields = ListFld, Select = ListSel
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m);
	}

	/// <summary>Įrašo informacija</summary>
	/// <param name="ctx" ></param>
	/// <param name="id">EVRK ID</param>
	/// <returns></returns>
	public static async Task Details(HttpContext ctx, string id) {
		if(string.IsNullOrWhiteSpace(id)) { ctx.Response.E400(true, "Missing id or code"); return; }
		var qry = $"SELECT id,l1,code,parent,layer,pavad,last FROM public.evrk WHERE {(ctx.ParamTrue("code")?"code":"id")}=@id LIMIT 1;";
		//  IDs:           0  1  2    3      4     5     6
		using var db = new DBRead(qry, new() { { "@id", id.ToUpper() } });
		using var rdr = await db.GetReader();
		if (await rdr.ReadAsync()) {
			await ctx.Response.WriteAsJsonAsync(new Evrk_Item() {
				ID=rdr.GetString(0), Sekcija=rdr.GetString(1), Kodas=rdr.GetStringN(2), Parent=rdr.GetStringN(3),
				Layer=rdr.GetInt32(4), Pavad=rdr.GetString(5), Last=rdr.GetBoolean(6)
			});
		}
		else { ctx.Response.E404(true); }
	}

	/// <summary>Įrašų informacija</summary>
	/// <param name="ctx" ></param>
	/// <param name="id">EVRK ID's</param>
	/// <returns></returns>
	public static async Task DetailsMulti(HttpContext ctx, [FromBody] HashSet<string> id) {
		var ret = new List<Evrk_Item>();
		if (id.Count > 0) {
			var qrid = new List<string>();
			var prm = new Dictionary<string, object?>();
			var cn = 1;
			foreach (var i in id) { qrid.Add("@id" + cn); prm["@id" + cn] = i.ToUpper(); cn++; }
			var qry = $"SELECT id,l1,code,parent,layer,pavad,last FROM public.evrk WHERE {(ctx.ParamTrue("code") ? "code" : "id")} in ({string.Join(',', qrid)});";
			//  IDs:           0  1  2    3      4     5     6
			using var db = new DBRead(qry, prm);
			using var rdr = await db.GetReader();
			while (await rdr.ReadAsync()) {
				ret.Add(new() {
					ID = rdr.GetString(0), Sekcija = rdr.GetString(1), Kodas = rdr.GetStringN(2), Parent = rdr.GetStringN(3),
					Layer = rdr.GetInt32(4), Pavad = rdr.GetString(5), Last = rdr.GetBoolean(6)
				});
			}
		}
		await ctx.Response.WriteAsJsonAsync(ret);
	}

	/// <summary>Kodų paieška</summary>
	/// <param name="ctx"></param>
	/// <param name="q">Paieškos užklausa</param>
	/// <param name="top">Įrašų skaičius</param>
	/// <returns></returns>
	public static async Task Search(HttpContext ctx, string q, int top = 10) {
		var m = await new DBPagingRequest<Evrk_Item>("public.v_app_evrk") {
			Limit = top.Limit(50),
			Page = 1,
			Fields = ListFld,
			Select = ListSel,
			Total = false,
			Where = new() { Last = true },
			Search = q.RemoveAccents().RemoveNonAlphanumeric().ToLower()
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m.Data);
	}


}