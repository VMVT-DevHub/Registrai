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
        var inact = ctx.ParamTrue("inactive");
		var m = await new DBPagingRequest<SporLoc_Item>("spor.v_locations", DB.VVR) {
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
        var m = await new DBRead($"SELECT {ListSelTxt} FROM spor.v_locations WHERE \"ID\"=@id", DB.VVR, ("@id", id)).GetObject<SporLoc_Item>();
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
		var inact = ctx.ParamTrue("inactive");
		var m = await new DBPagingRequest<SporOrg_Item>("spor.v_organisations", DB.VVR) {
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
		var m = await new DBRead($"SELECT {ListSelTxt} FROM spor.v_organisations WHERE \"ID\"=@id", DB.VVR, ("@id", id)).GetObject<SporOrg_Item>();
		if (m is null) ctx.Response.E404(true);
		else await ctx.Response.WriteAsJsonAsync(m);
	}

	/// <summary>Organizacijų paieška</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task Find(HttpContext ctx) {
		var m = await new DBPagingRequest<SporOrg_Item>("spor.v_organisations", DB.VVR) {
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

	static readonly List<string> ListFld = ["ID", "Name", "Short", "Description", "Domain", "Terms", "search"];
	static readonly List<string> ListSel = ["ID", "Name", "Short", "Description", "Domain", "Terms"];
	static readonly string ListSelTxt = $"\"{string.Join("\",\"", ListSel)}\"";

	/// <summary>Klasifikatorių sąrašai</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task List(HttpContext ctx) {
		var inact = ctx.ParamTrue("inactive");
		var m = await new DBPagingRequest<SporRef_ListItem>("spor.v_references_list", DB.VVR) {
			Limit = (ctx.ParamIntN("limit") ?? 25).Limit(1000),
			Page = ctx.ParamIntN("page") ?? 1,
			Sort = ctx.ParamString("order") ?? "ID",
			Desc = ctx.ParamTrue("desc"),
			//Where = (!ctx.ParamNull("org") || !ctx.ParamNull("country")) ? new() { OrgID = ctx.ParamString("org"), CountryCode = ctx.ParamString("country")?.ToUpper() } : null,
			//WhereAdd = ctx.ParamTrue("inactive") ? null : "\"Inactive\" is null",
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
		var m = await new DBRead($"SELECT {ListSelTxt} FROM spor.v_references_list WHERE \"ID\"=@id", DB.VVR, ("@id", id)).GetObject<SporRef_ListItem>();
		if (m is null) ctx.Response.E404(true);
		else await ctx.Response.WriteAsJsonAsync(m);
	}
}