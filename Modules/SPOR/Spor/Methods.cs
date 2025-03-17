using App;
using App.Routing;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Registrai.Modules.SPOR;

/// <summary>SPOR lokacijų metodai</summary>
public static partial class SporLocations {

	static readonly List<string> ListFld = ["ID", "CountryCode", "OrgID", "OrgNameEn", "OrgNameLt", "En", "Lt", "Inactive"];
	static readonly List<string> ListSel = ["ID", "CountryCode", "OrgID", "OrgNameEn", "OrgNameLt", "En", "Lt", "Inactive"];
	static readonly string ListSelTxt = $"\"{string.Join("\",\"", ListSel)}\"";

	/// <summary>Lokacijų sąrašas</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task List(HttpContext ctx) {
		var m = await new DBPagingRequest<Location_Item>("spor.v_locations", DB.VVR) {
			Limit = (ctx.ParamIntN("limit") ?? 25).Limit(1000),
			Page = ctx.ParamIntN("page") ?? 1,
			Sort = ctx.ParamString("order") ?? "ID",
			Desc = ctx.ParamTrue("desc"),
			Where = (!ctx.ParamNull("org") || !ctx.ParamNull("country")) ? new() { OrgID = ctx.ParamString("org"), CountryCode = ctx.ParamString("country")?.ToUpper() } : null,
			WhereAdd = ctx.ParamTrue("inactive") ? null : "\"Inactive\" is null",
			Fields = ListFld,
			Select = ListSel
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m);
	}

	/// <summary>Gauti lokaciją pagal ID</summary>
	/// <param name="ctx"></param>
	/// <param name="id"></param>
	/// <returns></returns>
	public static async Task Item(HttpContext ctx, string id) {
		using var db = new DBRead($"SELECT {ListSelTxt} FROM spor.v_locations WHERE \"ID\"=@id", DB.VVR, ("@id", id));
		var m = await db.GetObject<Location_Item>();
		if (m is null) ctx.Response.E404(true);
		else await ctx.Response.WriteAsJsonAsync(m);
	}
}


/// <summary>SPOR organizacijų metodai</summary>
public static partial class SporOrganisations {

	static readonly List<string> ListFld = ["ID", "NameEn", "NameLt", "Country", "Locations", "Inactive", "search"];
	static readonly List<string> ListSel = ["ID", "NameEn", "NameLt", "Country", "Locations", "Inactive"];
	static readonly string ListSelTxt = $"\"{string.Join("\",\"", ListSel)}\"";

	/// <summary>Organizacijų sąrašas</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task List(HttpContext ctx) {
		var m = await new DBPagingRequest<Organisation_Item>("spor.v_organisations", DB.VVR) {
			Limit = (ctx.ParamIntN("limit") ?? 25).Limit(1000),
			Page = ctx.ParamIntN("page") ?? 1,
			Sort = ctx.ParamString("order") ?? "ID",
			Desc = ctx.ParamTrue("desc"),
			Where = (!ctx.ParamNull("country")) ? new() { Country = ctx.ParamString("country")?.ToUpper() } : null,
			WhereAdd = ctx.ParamTrue("inactive") ? null : "\"Inactive\" is null",
			Fields = ListFld,
			Select = ListSel
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m);
	}

	/// <summary>Gauti organizaciją pagal ID</summary>
	/// <param name="ctx"></param>
	/// <param name="id"></param>
	/// <returns></returns>
	public static async Task Item(HttpContext ctx, string id) {
		using var db = new DBRead($"SELECT {ListSelTxt} FROM spor.v_organisations WHERE \"ID\"=@id", DB.VVR, ("@id", id));
		var m = await db.GetObject<Organisation_Item>();
		if (m is null) ctx.Response.E404(true);
		else await ctx.Response.WriteAsJsonAsync(m);
	}

	/// <summary>Organizacijų paieška</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task Find(HttpContext ctx) {
		var m = await new DBPagingRequest<Organisation_Item>("spor.v_organisations", DB.VVR) {
			Limit = (ctx.ParamIntN("limit") ?? 10).Limit(100), //TODO: limit from config
			Page = 1,
			Sort = ctx.ParamString("order") ?? "ID",
			Desc = ctx.ParamTrue("desc"),
			Where = (!ctx.ParamNull("country")) ? new() { Country = ctx.ParamString("country")?.ToUpper() } : null,
			WhereAdd = ctx.ParamTrue("inactive") ? null : "\"Inactive\" is null",
			Fields = ListFld,
			Select = ListSel,
			Total = false,
			Search = ctx.ParamString("q")?.MkSerach()
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m.Data);
	}

	private static string? MkSerach(this string? q) => q?.RemoveAccents().RemoveNonAlphanumeric(true).ToLower();//.RemWords(RemTypes);
}




/// <summary>SPOR klasifikatorių sąrašai</summary>
public static partial class SporReferences {

	static readonly List<string> ListFld = ["ID", "Name", "Short", "Description", "Domain", "TermCount", "search"];
	static readonly List<string> ListSel = ["ID", "Name", "Short", "Description", "Domain", "TermCount"];
	static readonly string ListSelTxt = $"\"{string.Join("\",\"", ListSel)}\"";

	static readonly List<string> TermFld = ["ListID", "ID", "Status", "En", "Lt", "Symbol", "ParentID", "Children", "search"];
	static readonly List<string> TermSel = ["ListID", "ID", "Status", "En", "Lt", "Symbol", "ParentID", "Children"];
	static readonly List<string> TermListSel = ["ID", "En", "Lt", "Symbol", "ParentID", "Children"];
	static readonly string TermSelTxt = $"\"{string.Join("\",\"", TermSel)}\"";

	/// <summary>Klasifikatorių sąrašai</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task List(HttpContext ctx) {
		var m = await new DBPagingRequest<References_ListItem>("spor.v_references_list", DB.VVR) {
			Limit = (ctx.ParamIntN("limit") ?? 25).Limit(1000),
			Page = ctx.ParamIntN("page") ?? 1,
			Sort = ctx.ParamString("order") ?? "ID",
			Desc = ctx.ParamTrue("desc"),
			Fields = ListFld,
			Select = ListSel
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m);
	}

	/// <summary>Gauti lokaciją pagal ID</summary>
	/// <param name="ctx"></param>
	/// <param name="id"></param>
	/// <returns></returns>
	public static async Task ListItem(HttpContext ctx, long id) {
		using var db = new DBRead($"SELECT {ListSelTxt} FROM spor.v_references_list WHERE \"ID\"=@id", DB.VVR, ("@id", id));
		var m = await db.GetObject<References_ListItem>();
		if (m is null) ctx.Response.E404(true);
		else {
			if (m.TermCount < 300) { //TODO: 300 iš konfigo
				var ls = await new DBPagingRequest<References_TermItem>("spor.v_references_terms", DB.VVR) {
					Limit = 300, Page = 1,
					Where = new() { ListID = id }, WhereAdd = "\"Status\" is null",
					Fields = TermSel, Select = TermListSel
				}.Execute();
				m.Terms = ls.Data;
			}
			await ctx.Response.WriteAsJsonAsync(m);
		}
	}

	/// <summary>Gauti terminų sąrašą</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task TermList(HttpContext ctx) {
		var m = await new DBPagingRequest<References_TermItem>("spor.v_references_terms", DB.VVR) {
			Limit = (ctx.ParamIntN("limit") ?? 25).Limit(1000),
			Page = ctx.ParamIntN("page") ?? 1,
			Sort = ctx.ParamString("order") ?? "ID",
			Desc = ctx.ParamTrue("desc"),
			Where = (!ctx.ParamNull("list") || !ctx.ParamNull("parent")) ? new() { ListID = ctx.ParamLongN("list"), ParentID = ctx.ParamLongN("parent") } : null,
			WhereAdd = ctx.ParamTrue("inactive") ? null : "\"Status\" is null",
			Fields = TermFld, Select = TermSel
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m);
	}

	/// <summary>Gauti terminą pagal ID</summary>
	/// <param name="ctx"></param>
	/// <param name="id">Termino ID</param>
	/// <returns></returns>
	public static async Task TermItem(HttpContext ctx, long id) {
		using var db = new DBRead($"SELECT {TermSelTxt} FROM spor.v_references_terms WHERE \"ID\"=@id", DB.VVR, ("@id", id));
		var m = await db.GetObject<References_TermItem>();
		if (m is null) ctx.Response.E404(true);
		else await ctx.Response.WriteAsJsonAsync(m);
	}

	/// <summary>Terminų paieška</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task TermFind(HttpContext ctx) {
		var m = await new DBPagingRequest<References_TermItem>("spor.v_references_terms", DB.VVR) {
			Limit = (ctx.ParamIntN("limit") ?? 10).Limit(100), //TODO: limit from config
			Page = 1,
			Sort = ctx.ParamString("order") ?? "ID",
			Desc = ctx.ParamTrue("desc"),
			Where = (!ctx.ParamNull("list")) ? new() { ListID = ctx.ParamLong("list") } : null,
			WhereAdd = ctx.ParamTrue("inactive") ? null : "\"Status\" is null",
			Fields = TermFld, Select = TermSel,
			Total = false,
			Search = ctx.ParamString("q")?.MkSerach()
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m.Data);
	}
	private static string? MkSerach(this string? q) => q?.RemoveAccents().RemoveNonAlphanumeric(true).ToLower();

}



/// <summary>SPOR medžiagų sąrašai</summary>
public static partial class SporSubstances {
	static readonly List<string> ListFld = ["ID", "Name", "Source", "Domain", "Type", "Formula", "Weight", "ParentID", "ChildCount", "Names", "search"];
	static readonly List<string> ListSel = ["ID", "Name", "Source", "Domain", "Type", "Formula", "Weight", "ParentID", "ChildCount", "Names"];
	static readonly string ListSelTxt = $"\"{string.Join("\",\"", ListSel)}\"";

	/// <summary>Medžiagos</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task List(HttpContext ctx) {
		var m = await new DBPagingRequest<Substances_Item>("spor.v_substances", DB.VVR) {
			Limit = (ctx.ParamIntN("limit") ?? 25).Limit(1000),
			Page = ctx.ParamIntN("page") ?? 1,
			Sort = ctx.ParamString("order") ?? "ID",
			Desc = ctx.ParamTrue("desc"),
			Where = (!ctx.ParamNull("domain") || !ctx.ParamNull("type")) ? new() { Domain = ctx.ParamString("org"), Type = ctx.ParamString("type") } : null,
			Fields = ListFld,
			Select = ListSel
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m);
	}

	/// <summary>Gauti medžiagą pagal ID</summary>
	/// <param name="ctx"></param>
	/// <param name="id">Medžiagos ID</param>
	/// <returns></returns>
	public static async Task Item(HttpContext ctx, long id) {
		using var db = new DBRead($"SELECT {ListSelTxt} FROM spor.v_substances WHERE \"ID\"=@id", DB.VVR, ("@id", id));
		var m = await db.GetObject<Substances_Item>();
		if (m is null) ctx.Response.E404(true);
		else await ctx.Response.WriteAsJsonAsync(m);
	}


	/// <summary>Medžiagų paieška</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task Find(HttpContext ctx) {
		var m = await new DBPagingRequest<Substances_Item>("spor.v_substances", DB.VVR) {
			Limit = (ctx.ParamIntN("limit") ?? 10).Limit(100), //TODO: limit from config
			Page = 1,
			Sort = ctx.ParamString("order") ?? "ID",
			Desc = ctx.ParamTrue("desc"),
			Where = (!ctx.ParamNull("domain") || !ctx.ParamNull("type")) ? new() { Domain = ctx.ParamString("org"), Type = ctx.ParamString("type") } : null,
			Fields = ListFld, Select = ListSel,
			Total = false,
			Search = ctx.ParamString("q")?.MkSerach()
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m.Data);
	}
	private static string? MkSerach(this string? q) => q?.RemoveAccents().RemoveNonAlphanumeric(true).ToLower();
}