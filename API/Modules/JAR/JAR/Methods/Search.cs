﻿using API;
using App.Routing;
using Microsoft.AspNetCore.Http;
using Modules.JAR.Models;
using System.Text.Json.Serialization;

namespace Modules.JAR.Methods;

/// <summary>Adresų paieškos modelis</summary>
public static class JARSearch {
    private static readonly int pagelimit = 50;

    private static List<string> CachedRemTypes { get; set; } = [];
    /// <summary>Paieškos frazių pašalinimo sąrašas</summary>
    public static List<string> RemTypes => CachedRemTypes;

    private static string? MkSerach(this string? q) => q?.RemoveAccents().RemoveNonAlphanumeric(true).ToLower().RemWords(RemTypes);


    private static readonly List<string> SrhFields = ["ID", "Pavad", "Adresas", "Statusas", "Forma", "AobKodas", "FormKodas", "StatusKodas", "search", "sort"];
	private static readonly List<string> SrhSelect1 = ["ID", "Pavad", "Adresas", "Statusas", "Forma"];
	private static readonly List<string> SrhSelect2 = ["ID", "Pavad"];


	/// <summary>Supaprastinta adresų paieška</summary>
	/// <param name="ctx"></param>
	/// <param name="q">Paieškos užklausa</param>
	/// <param name="top">Įrašų skaičius</param>
	/// <param name="status">Statuso kodo filtravimas</param>
	/// <param name="details">Rodyti daugiau informacijos</param>
	/// <returns></returns>
	public static async Task GetSrh(HttpContext ctx, string q, int top=10, bool? details=false, int? status=null) {
		var ret = await new DBPagingRequest<JAR_SearchItem>("jar.v_app_search") {
			Limit = top.Limit(pagelimit),
			Page = 1,
			Sort = "sort",
			Where = status is null ? null : new() { StatusKodas = status },
			Fields = SrhFields,
			Select = details ?? true ? SrhSelect1 : SrhSelect2,
			Total = false,
			Search = q.MkSerach()
		}.Execute();

		await ctx.Response.WriteAsJsonAsync(ret.Data);
	}

	/// <summary>Detali adresų paieška</summary>
	/// <param name="ctx"></param>
	/// <param name="q">Paieškos užklausa</param>
	/// <returns></returns>
	public static async Task FullSearch(HttpContext ctx, JAR_SearchQuery q) {
		var m = new DBPagingRequest<JAR_SearchItem>("jar.v_app_search") {
			Limit = q.Top?.Limit(pagelimit) ?? 10,
			Page = 1,
			Sort = "sort",
			Fields = SrhFields,
			Select = q.Detales ?? true ? SrhSelect1 : SrhSelect2,
			Search = q.Search.MkSerach(),
			Total = false
		};

		if (q.Filter is not null) {
			var f = q.Filter;
			m.Where = new() { AobKodas = f.AobKodas, FormKodas = f.FormKodas, StatusKodas = f.StatusKodas };
		}

		var ret = await m.Execute();
		await ctx.Response.WriteAsJsonAsync(ret.Data);
	}
}