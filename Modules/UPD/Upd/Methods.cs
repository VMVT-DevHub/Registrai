using App;
using App.Routing;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Registrai.Modules.UPD;

/// <summary>UPD duomenų gavimo metodai</summary>
public static partial class UpdMedicines {

	static readonly List<string> ListFld = ["med_id", "med_date", "list_lt", "list_en"];
	static readonly List<string> ListLt = ["med_id", "list_lt"];
	static readonly List<string> ListEn = ["med_id", "list_en"];

	/// <summary>Vaistų sąrašas</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task List(HttpContext ctx) {
		var en = ctx.ParamString("lang")?.ToLower() == "en";
		var desc = ctx.ParamString("order") is null;
#if DEBUG
		var tbl = ctx.ParamTrue("uat") ? "upd_uat.v_med" : "upd.v_med";
#else
		var tbl = "upd.v_med";
#endif
		var m = await new DBPagingRequest<MedListItem>(tbl, DB.VVR) {
			Limit = (ctx.ParamIntN("limit") ?? 25).Limit(100),
			Page = ctx.ParamIntN("page") ?? 1,
			Sort = ctx.ParamString("order") ?? "med_date",
			Desc = desc || ctx.ParamTrue("desc"),
			JsonField = en ? "list_en" : "list_lt",
			//TODO: Filtrai
			//Where = (!ctx.ParamNull("org") || !ctx.ParamNull("country")) ? new() { OrgID = ctx.ParamString("org"), CountryCode = ctx.ParamString("country")?.ToUpper() } : null,
			WhereAdd = "med_idf is not null",
			Fields = ListFld,
			Select = en ? ListEn : ListLt
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m);
	}

	/// <summary>Gauti vaistą pagal ID</summary>
	/// <param name="ctx"></param>
	/// <param name="id"></param>
	/// <returns></returns>
	public static async Task Item(HttpContext ctx, string id) {
		var en = ctx.ParamString("lang")?.ToLower() == "en";
#if DEBUG
		var tbl = ctx.ParamTrue("uat") ? "upd_uat.v_med" : "upd.v_med";
#else
		var tbl = "upd.v_med";
#endif
		if (long.TryParse(id, out var idi)) {
			using var db = new DBRead($"SELECT {(en ? "med_en" : "med_lt")} FROM {tbl} WHERE \"med_id\"=@id", DB.VVR, ("@id", idi));
			var m = await db.GetObject<Medicine>(0);
			if (m is null) ctx.Response.E404(true);
			else await ctx.Response.WriteAsJsonAsync(m);
		}
		else ctx.Response.E404(true); //TODO: Normalią klaidą grąžinti
	}
}

