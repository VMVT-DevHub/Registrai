using App;
using App.Routing;
using Microsoft.AspNetCore.Http;
using Registrai.Modules.Salys.Models;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Registrai.Modules.Salys.Methods;

/// <summary>Adreso detalių gavimo metodas</summary>
public static partial class Sal {
	[GeneratedRegex(@"[^\w\-_\.]")] private static partial Regex FileNameRgx();
	private static string FlagDir(string name) => Path.Combine(AppContext.BaseDirectory, "Content", "Flags", $"{name}.svg");



	static readonly List<string> ListFld = ["Iso3", "Iso2", "Pavad", "Pilnas", "Sostine", "Eng", "Eu", "search"];
	static readonly List<string> ListSel = ["Iso3", "Iso2", "Pavad", "Pilnas", "Sostine", "Eng", "Eu"];

	/// <summary>Šalių sąrašas</summary>
	/// <param name="ctx"></param>
	/// <param name="page">Puslapis</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <param name="eu">Autopos sąjungo šalis</param>
	/// <param name="order">Rikiavimas</param>
	/// <param name="desc">Rikiavimas mažėjančiai</param>
	/// <returns></returns>
	public static async Task List(HttpContext ctx, int page = 1, int top = 250, bool? eu=false, string? order = null, bool desc = false) {
		var m = await new DBPagingRequest<Sal_Item>("public.v_app_salys") {
			Limit = top, Page = page, Sort = order ?? "ISO3", Desc = desc,
			Where = eu ?? true ? new() { Eu = true } : null, Fields = ListFld, Select = ListSel
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m);
	}

	/// <summary>Šalies informacija</summary>
	/// <param name="ctx"></param>
	/// <param name="iso3">Šalies kodas</param>
	/// <returns></returns>
	public static async Task Info(HttpContext ctx, string iso3) {
		string fn = FileNameRgx().Replace(iso3, "_").ToUpper();
		if (fn.Length > 3 || fn.Length < 3) { ctx.Response.E400(true, "Invalid ISO 3166-1 alpha-3 country code"); return; }
		if (ctx.ParamTrue("flag")) {
			var path = FlagDir(fn);
			if (!File.Exists(path)) path = FlagDir("_blank");
			ctx.Response.ContentType = "image/svg+xml";
			ctx.Response.ContentLength = new FileInfo(path).Length;
			await ctx.Response.SendFileAsync(path);
			return;
		}

		using var db = new DBRead("SELECT iso3, iso2, name, title, capital, eng, eu FROM public.salys WHERE iso3=@iso3;", new() { { "@iso3", fn } });
		using var rdr = await db.GetReader();

		if (await rdr.ReadAsync()) {
			await ctx.Response.WriteAsJsonAsync(new Sal_Item() {
				Iso3 = rdr.GetStringN(0), Iso2 = rdr.GetStringN(1), Pavad = rdr.GetStringN(2), Pilnas = rdr.GetStringN(3), 
				Sostine = rdr.GetStringN(4), Eng = rdr.GetStringN(5), Eu = rdr.IsDBNull(6) ? null : rdr.GetBoolean(6)
			});
		} 

		else { ctx.Response.E404(true); }
	}

	private static string? MkSerach(this string? q) => q?.RemoveAccents().RemoveNonAlphanumeric(true).ToLower();
	/// <summary>Šalių paieška</summary>
	/// <param name="ctx"></param>
	/// <param name="q">Paieškos užklausa</param>
	/// <param name="top">Įrašų skaičius</param>
	/// <returns></returns>
	public static async Task Search(HttpContext ctx, string q, int top = 10) {
		var m = await new DBPagingRequest<Sal_Item>("public.v_app_salys") {
			Limit = top.Limit(50),
			Page = 1,
			Sort = "Iso3",
			Fields = ListFld,
			Select = ListSel,
			Total = false,
			Search = q.MkSerach()
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m.Data);
	}


}