using App;
using App.Routing;
using Microsoft.AspNetCore.Http;
using Registrai.Modules.AR.Models;

namespace Registrai.Modules.AR.Methods;

/// <summary>Adresų registro sąrašai</summary>
public static class ARLists
{
    private static readonly int pagelimit = 1000;

    /// <summary>Filtruoti duomenys</summary>
    /// <param name="ctx"></param>
    /// <param name="qry">Filtro užklausa</param>
    /// <returns></returns>
    public static async Task Filter(HttpContext ctx, AR_ListQuery qry)
    {
        if (qry.Filter is null && string.IsNullOrWhiteSpace(qry.Search)) { throw new("No filter provided"); }
        //TODO: Limit from settings
        var top = qry.Top.Limit(pagelimit);
        qry.Filter ??= new();
        var m = qry.Type switch
        {
            AR_ListTypes.Adm => Adm(ctx, qry.Page, top, qry.Order, qry.Desc),
            AR_ListTypes.Sav => Sav(ctx, qry.Page, top, qry.Filter.Adm, qry.Search, qry.Order, qry.Desc),
            AR_ListTypes.Sen => Sen(ctx, qry.Page, top, qry.Filter.Adm, qry.Filter.Sav, qry.Search, qry.Order, qry.Desc),
            AR_ListTypes.Gyv => Gyv(ctx, qry.Page, top, qry.Filter.Adm, qry.Filter.Sav, qry.Filter.Sen, qry.Search, qry.Order, qry.Desc),
            AR_ListTypes.Gat => Gat(ctx, qry.Page, top, qry.Filter.Adm, qry.Filter.Sav, qry.Filter.Sen, qry.Filter.Gyv, qry.Search, qry.Order, qry.Desc),
            AR_ListTypes.Aob => Aob(ctx, qry.Page, top, qry.Filter.Adm, qry.Filter.Sav, qry.Filter.Sen, qry.Filter.Gyv, qry.Filter.Gat, qry.Order, qry.Desc),
            AR_ListTypes.Pat => Pat(ctx, qry.Page, top, qry.Filter.Adm, qry.Filter.Sav, qry.Filter.Sen, qry.Filter.Gyv, qry.Filter.Gat, qry.Filter.Aob, qry.Order, qry.Desc),
            _ => throw new NotImplementedException(),
        };
        await m;
    }


    /// <summary>1 - Apskričių duomenys (ADM)</summary>
    /// <param name="ctx"></param>
    /// <param name="page">Puslapis</param>
    /// <param name="top">Duomenų ribojimas</param>
    /// <param name="order">Rikiavimas</param>
    /// <param name="desc">Rikiavimas mažėjančiai</param>
    /// <returns></returns>
    public static async Task Adm(HttpContext ctx, int page = 1, int top = 100, string? order = null, bool desc = false)
    {
        var m = await new DBPagingRequest<AR_ListItem>("ar.v_app_1_apskritys")
        {
            Limit = top.Limit(pagelimit),
            Page = page,
            Sort = order ?? "ID",
            Desc = desc,
            Fields = AdmFld,
            Select = AdmSel
        }.Execute();
        await ctx.Response.WriteAsJsonAsync(m);

    }

    /// <summary>2 - Savivaldybių duomenys (SAV)</summary>
    /// <param name="ctx"></param>
    /// <param name="page">Puslapis</param>
    /// <param name="top">Duomenų ribojimas</param>
    /// <param name="adm">Apskrities kodas</param>
    /// <param name="q">Paieškos frazė</param>
    /// <param name="order">Rikiavimas</param>
    /// <param name="desc">Rikiavimas mažėjančiai</param>
    /// <returns></returns>
    public static async Task Sav(HttpContext ctx, int page = 1, int top = 100, int? adm = null, string? q = null, string? order = null, bool desc = false)
    {
        var m = await new DBPagingRequest<AR_ListItem>("ar.v_app_2_savivaldybes")
        {
            Limit = top.Limit(pagelimit),
            Page = page,
            Sort = order ?? "ID",
            Desc = desc,
            Where = new() { Adm = adm },
            Fields = SavFld,
            Select = SavSel,
            Search = q.MkSerach()
        }.Execute();
        await ctx.Response.WriteAsJsonAsync(m);
    }


    /// <summary>3 - Seniunijų duomenys (SEN)</summary>
    /// <param name="ctx"></param>
    /// <param name="page">Puslapis</param>
    /// <param name="top">Duomenų ribojimas</param>
    /// <param name="adm">Apskrities kodas</param>
    /// <param name="sav">Savivaldybes kodas</param>
    /// <param name="q">Paieškos frazė</param>
    /// <param name="order">Rikiavimas</param>
    /// <param name="desc">Rikiavimas mažėjančiai</param>
    /// <returns></returns>
    public static async Task Sen(HttpContext ctx, int page = 1, int top = 100, int? adm = null, int? sav = null, string? q = null, string? order = null, bool desc = false)
    {
        var m = await new DBPagingRequest<AR_ListItem>("ar.v_app_3_seniunijos")
        {
            Limit = top.Limit(pagelimit),
            Page = page,
            Sort = order ?? "ID",
            Desc = desc,
            Where = new() { Adm = adm, Sav = sav },
            Fields = SenFld,
            Select = SenSel,
            Search = q.MkSerach()
        }.Execute();
        await ctx.Response.WriteAsJsonAsync(m);
    }

    /// <summary>4 - Gyvenviečių duomenys(GYV)</summary>
    /// <param name="ctx"></param>
    /// <param name="page">Puslapis</param>
    /// <param name="top">Duomenų ribojimas</param>
    /// <param name="adm">Apskrities kodas</param>
    /// <param name="sav">Savivaldybes kodas</param>
    /// <param name="sen">Seniunijos kodas</param>
    /// <param name="q">Paieškos frazė</param>
    /// <param name="order">Rikiavimas</param>
    /// <param name="desc">Rikiavimas mažėjančiai</param>
    /// <returns></returns>
    public static async Task Gyv(HttpContext ctx, int page = 1, int top = 100, int? adm = null, int? sav = null, int? sen = null, string? q = null, string? order = null, bool desc = false)
    {
        var m = await new DBPagingRequest<AR_ListItem>("ar.v_app_4_gyvenvietes")
        {
            Limit = top.Limit(pagelimit),
            Page = page,
            Sort = order ?? "ID",
            Desc = desc,
            Where = new() { Adm = adm, Sav = sav, Sen = sen },
            Fields = GyvFls,
            Select = GyvSel,
            Search = q.MkSerach()
            //TODO: Null Gatvės
        }.Execute();
        await ctx.Response.WriteAsJsonAsync(m);
    }

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
    /// <param name="desc">Rikiavimas mažėjančiai</param>
    /// <returns></returns>
    public static async Task Gat(HttpContext ctx, int page = 1, int top = 100, int? adm = null, int? sav = null, int? sen = null, int? gyv = null, string? q = null, string? order = null, bool desc = false)
    {
        var m = await new DBPagingRequest<AR_ListItem>("ar.v_app_5_gatves")
        {
            Limit = top.Limit(pagelimit),
            Page = page,
            Sort = order ?? "ID",
            Desc = desc,
            Where = new() { Adm = adm, Sav = sav, Sen = sen, Gyv = gyv },
            Fields = GatFld,
            Select = GatSel,
            Search = q.MkSerach()
        }.Execute();
        await ctx.Response.WriteAsJsonAsync(m);
    }


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
    /// <param name="desc">Rikiavimas mažėjančiai</param>
    /// <returns></returns>
    public static async Task Aob(HttpContext ctx, int page = 1, int top = 100, int? adm = null, int? sav = null, int? sen = null, int? gyv = null, int? gat = null, string? order = null, bool desc = false)
    {
        var m = await new DBPagingRequest<AR_ListItem>("ar.v_app_6_adresai")
        {
            Limit = top.Limit(pagelimit),
            Page = page,
            Sort = order ?? "ID",
            Desc = desc,
            Where = new() { Adm = adm, Sav = sav, Sen = sen, Gyv = gyv, Gat = gat },
            Fields = AobFld,
            Select = FldSel
        }.Execute();
        await ctx.Response.WriteAsJsonAsync(m);
    }


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
    /// <param name="desc">Rikiavimas mažėjančiai</param>
    /// <returns></returns>
    public static async Task Pat(HttpContext ctx, int page = 1, int top = 100, int? adm = null, int? sav = null, int? sen = null, int? gyv = null, int? gat = null, int? aob = null, string? order = null, bool desc = false)
    {
        var m = await new DBPagingRequest<AR_ListItem>("ar.v_app_7_patalpos")
        {
            Limit = top.Limit(pagelimit),
            Page = page,
            Sort = order ?? "ID",
            Desc = desc,
            Where = new() { Adm = adm, Sav = sav, Sen = sen, Gyv = gyv, Gat = gat, Aob = aob },
            Fields = PatFld,
            Select = PatSel
        }.Execute();
        await ctx.Response.WriteAsJsonAsync(m);
    }

    private static string? MkSerach(this string? q) => q?.RemoveAccents().RemoveNonAlphanumeric(true).ToLower().RemWords(Methods.ARSearch.RemTypes);


    static readonly List<string> AdmFld = ["ID", "Vardas", "Chc", "RegData"];
    static readonly List<string> AdmSel = ["ID", "Vardas", "Tipas", "Trump", "Chc"];

    static readonly List<string> SavFld = ["ID", "Vardas", "Adm", "Chc", "Chm", "RegData", "search"];
    static readonly List<string> SavSel = ["ID", "Vardas", "Tipas", "Trump", "Adm", "Chc", "Chm"];

    static readonly List<string> SenFld = ["ID", "Vardas", "Adm", "Sav", "Chc", "RegData", "search"];
    static readonly List<string> SenSel = ["ID", "Vardas", "Tipas", "Trump", "Adm", "Sav", "Chc"];

    static readonly List<string> GyvFls = ["ID", "Vardas", "Adm", "Sav", "Sen", "Chc", "Chm", "RegData", "search"];
    static readonly List<string> GyvSel = ["ID", "Vardas", "Tipas", "Trump", "Adm", "Sav", "Sen", "Chc", "Chm"];

    static readonly List<string> GatFld = ["ID", "Vardas", "Adm", "Sav", "Sen", "Gyv", "Chc", "RegData", "search"];
    static readonly List<string> GatSel = ["ID", "Vardas", "Tipas", "Trump", "Adm", "Sav", "Sen", "Gyv", "Chc"];

    static readonly List<string> AobFld = ["ID", "Nr", "Korp", "Post", "Adm", "Sav", "Sen", "Gyv", "Gat", "Chc", "RegData"];
    static readonly List<string> FldSel = ["ID", "Nr", "Korp", "Post", "Adm", "Sav", "Sen", "Gyv", "Gat", "Chc"];

    static readonly List<string> PatFld = ["ID", "Nr", "Adm", "Sav", "Sen", "Gyv", "Gat", "Aob", "RegData"];
    static readonly List<string> PatSel = ["ID", "Pat", "Adm", "Sav", "Sen", "Gyv", "Gat", "Aob"];

}