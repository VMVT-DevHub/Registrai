using App;
using App.Routing;
using Microsoft.AspNetCore.Http;
using Registrai.Modules.AR.Models;

namespace Registrai.Modules.AR.Methods;

/// <summary>Adresų registro sąrašai</summary>
public static class ARLists {
	private static readonly int pagelimit = 1000;

	/// <summary>Filtruoti duomenys</summary>
	/// <param name="ctx"></param>
	/// <param name="qry">Filtro užklausa</param>
	/// <returns></returns>
	public static async Task Filter(HttpContext ctx, AR_ListQuery qry) {
		if (qry.Filter is null && string.IsNullOrWhiteSpace(qry.Search)) { throw new("No filter provided"); }
		//TODO: Limit from settings
		var top = qry.Top.Limit(pagelimit);
		qry.Filter ??= new();
		var m = qry.Type switch {
			AR_ListTypes.Adm => Filter(ctx, "ar.v_app_1_apskritys",    AdmFld, AdmSel, qry.Page, top, qry.Order, qry.Desc),
			AR_ListTypes.Sav => Filter(ctx, "ar.v_app_2_savivaldybes", SavFld, SavSel, qry.Page, top, qry.Order, qry.Desc, qry.Search, qry.Filter.Adm),
			AR_ListTypes.Sen => Filter(ctx, "ar.v_app_3_seniunijos",   SenFld, SenSel, qry.Page, top, qry.Order, qry.Desc, qry.Search, qry.Filter.Adm, qry.Filter.Sav),
			AR_ListTypes.Gyv => Filter(ctx, "ar.v_app_4_gyvenvietes",  GyvFld, GyvSel, qry.Page, top, qry.Order, qry.Desc, qry.Search, qry.Filter.Adm, qry.Filter.Sav, qry.Filter.Sen),
			AR_ListTypes.Gat => Filter(ctx, "ar.v_app_5_gatves",       GatFld, GatSel, qry.Page, top, qry.Order, qry.Desc, qry.Search, qry.Filter.Adm, qry.Filter.Sav, qry.Filter.Sen, qry.Filter.Gyv),
			AR_ListTypes.Aob => Filter(ctx, "ar.v_app_6_adresai",      AobFld, AobSel, qry.Page, top, qry.Order, qry.Desc, qry.Search, qry.Filter.Adm, qry.Filter.Sav, qry.Filter.Sen, qry.Filter.Gyv, qry.Filter.Gat),
			AR_ListTypes.Pat => Filter(ctx, "ar.v_app_7_patalpos",     PatFld, PatSel, qry.Page, top, qry.Order, qry.Desc, qry.Search, qry.Filter.Adm, qry.Filter.Sav, qry.Filter.Sen, qry.Filter.Gyv, qry.Filter.Gat, qry.Filter.Aob),
			_ => throw new NotImplementedException(),
		};
		await m;
	}

	private static async Task Filter(HttpContext ctx, string table, List<string> fld, List<string> sel, int page = 1, int top = 100, string? order = null, bool desc = false, string? q = null,
			int? adm = null, int? sav = null, int? sen = null, int? gyv = null, int? gat = null, int? aob = null) {
		var m = await new DBPagingRequest<AR_ListItem>(table) {
			Limit = top.Limit(pagelimit), Page = page, Sort = order ?? "ID", Desc = desc,
			Where = new() { Adm = adm, Sav = sav, Sen = sen, Gyv = gyv, Gat = gat, Aob = aob },
			Fields = fld, Select = sel, Search = q?.MkSerach()
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m);
	}

	/// <summary>1 - Apskričių duomenys (ADM)</summary>
	/// <param name="ctx"></param>
	/// <param name="page">Puslapis</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <param name="order">Rikiavimas</param>
	/// <returns></returns>
	public static async Task Adm(HttpContext ctx, int page = 1, int top = 100, string? order = null) =>
		await Filter(ctx, "ar.v_app_1_apskritys", AdmFld, AdmSel, page, top, order, ctx.ParamTrue("desc"));
	
	/// <summary>2 - Savivaldybių duomenys (SAV)</summary>
	/// <param name="ctx"></param>
	/// <param name="page">Puslapis</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <param name="adm">Apskrities kodas</param>
	/// <param name="q">Paieškos frazė</param>
	/// <param name="order">Rikiavimas</param>
	/// <returns></returns>
	public static async Task Sav(HttpContext ctx, int page = 1, int top = 100, int? adm = null, string? q = null, string? order = null) =>
		await Filter(ctx, "ar.v_app_2_savivaldybes", SavFld, SavSel, page, top, order, ctx.ParamTrue("desc"), q, adm);

	/// <summary>3 - Seniunijų duomenys (SEN)</summary>
	/// <param name="ctx"></param>
	/// <param name="page">Puslapis</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <param name="adm">Apskrities kodas</param>
	/// <param name="sav">Savivaldybes kodas</param>
	/// <param name="q">Paieškos frazė</param>
	/// <param name="order">Rikiavimas</param>
	/// <returns></returns>
	public static async Task Sen(HttpContext ctx, int page = 1, int top = 100, int? adm = null, int? sav = null, string? q = null, string? order = null) =>
		await Filter(ctx, "ar.v_app_3_seniunijos", SenFld, SenSel, page, top, order, ctx.ParamTrue("desc"), q, adm, sav);

	/// <summary>4 - Gyvenviečių duomenys(GYV)</summary>
	/// <param name="ctx"></param>
	/// <param name="page">Puslapis</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <param name="adm">Apskrities kodas</param>
	/// <param name="sav">Savivaldybes kodas</param>
	/// <param name="sen">Seniunijos kodas</param>
	/// <param name="q">Paieškos frazė</param>
	/// <param name="order">Rikiavimas</param>
	/// <returns></returns>
	public static async Task Gyv(HttpContext ctx, int page = 1, int top = 100, int? adm = null, int? sav = null, int? sen = null, string? q = null, string? order = null) =>
		await Filter(ctx, "ar.v_app_4_gyvenvietes", GyvFld, GyvSel, page, top, order, ctx.ParamTrue("desc"), q, adm, sav, sen);

	/// <summary>5 - Gatvių duomenys (GAT)</summary>
	/// <param name="ctx"></param>
	/// <param name="page">Puslapis</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <param name="adm">Apskrities kodas</param>
	/// <param name="sav">Savivaldybes kodas</param>
	/// <param name="sen">Seniunijos kodas</param>
	/// <param name="gyv">Gyvenvietės kodas</param>
	/// <param name="q">Paieškos frazė</param>
	/// <param name="order">Rikiavimas</param>
	/// <returns></returns>
	public static async Task Gat(HttpContext ctx, int page = 1, int top = 100, int? adm = null, int? sav = null, int? sen = null, int? gyv = null, string? q = null, string? order = null) =>
		await Filter(ctx, "ar.v_app_5_gatves", GatFld, GatSel, page, top, order, ctx.ParamTrue("desc"), q, adm, sav, sen, gyv);

	/// <summary>6 - Adresų duomenys (AOB)</summary>
	/// <param name="ctx"></param>
	/// <param name="page">Puslapis</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <param name="adm">Apskrities kodas</param>
	/// <param name="sav">Savivaldybes kodas</param>
	/// <param name="sen">Seniunijos kodas</param>
	/// <param name="gyv">Gyvenvietės kodas</param>
	/// <param name="gat">Gatvės kodas</param>
	/// <param name="order">Rikiavimas</param>
	/// <returns></returns>
	public static async Task Aob(HttpContext ctx, int page = 1, int top = 100, int? adm = null, int? sav = null, int? sen = null, int? gyv = null, int? gat = null, string? order = null) =>
		await Filter(ctx, "ar.v_app_6_adresai", AobFld, AobSel, page, top, order, ctx.ParamTrue("desc"), null, adm, sav, sen, gyv, gat);

	/// <summary>7 - Patalpų duomenys (PAT)</summary>
	/// <param name="ctx"></param>
	/// <param name="page">Puslapis</param>
	/// <param name="top">Duomenų ribojimas</param>
	/// <param name="adm">Apskrities kodas</param>
	/// <param name="sav">Savivaldybes kodas</param>
	/// <param name="sen">Seniunijos kodas</param>
	/// <param name="gyv">Gyvenvietės kodas</param>
	/// <param name="gat">Gatvės kodas</param>
	/// <param name="aob">Adredo AOB kodas</param>
	/// <param name="order">Rikiavimas</param>
	/// <returns></returns>
	public static async Task Pat(HttpContext ctx, int page = 1, int top = 100, int? adm = null, int? sav = null, int? sen = null, int? gyv = null, int? gat = null, int? aob = null, string? order = null) =>
		await Filter(ctx, "ar.v_app_7_patalpos", PatFld, PatSel, page, top, order, ctx.ParamTrue("desc"), null, adm, sav, sen, gyv, gat, aob);


	private static string? MkSerach(this string? q) => q?.RemoveAccents().RemoveNonAlphanumeric(true).ToLower().RemWords(Methods.ARSearch.RemTypes);


	static readonly List<string> AdmFld = ["ID", "Vardas", "Chc", "RegData"];
	static readonly List<string> AdmSel = ["ID", "Vardas", "Tipas", "Trump", "Chc"];

	static readonly List<string> SavFld = ["ID", "Vardas", "Adm", "Chc", "Chm", "RegData", "search"];
	static readonly List<string> SavSel = ["ID", "Vardas", "Tipas", "Trump", "Adm", "Chc", "Chm"];

	static readonly List<string> SenFld = ["ID", "Vardas", "Adm", "Sav", "Chc", "RegData", "search"];
	static readonly List<string> SenSel = ["ID", "Vardas", "Tipas", "Trump", "Adm", "Sav", "Chc"];

	static readonly List<string> GyvFld = ["ID", "Vardas", "Adm", "Sav", "Sen", "Chc", "Chm", "RegData", "search"];
	static readonly List<string> GyvSel = ["ID", "Vardas", "Tipas", "Trump", "Adm", "Sav", "Sen", "Chc", "Chm"];

	static readonly List<string> GatFld = ["ID", "Vardas", "Adm", "Sav", "Sen", "Gyv", "Chc", "RegData", "search"];
	static readonly List<string> GatSel = ["ID", "Vardas", "Tipas", "Trump", "Adm", "Sav", "Sen", "Gyv", "Chc"];

	static readonly List<string> AobFld = ["ID", "Nr", "Korp", "Post", "Adm", "Sav", "Sen", "Gyv", "Gat", "Chc", "RegData"];
	static readonly List<string> AobSel = ["ID", "Nr", "Korp", "Post", "Adm", "Sav", "Sen", "Gyv", "Gat", "Chc"];

	static readonly List<string> PatFld = ["ID", "Nr", "Adm", "Sav", "Sen", "Gyv", "Gat", "Aob", "RegData"];
	static readonly List<string> PatSel = ["ID", "Pat", "Adm", "Sav", "Sen", "Gyv", "Gat", "Aob"];

}