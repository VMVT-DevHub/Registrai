﻿using App;
using App.Routing;
using Microsoft.AspNetCore.Http;
using Registrai.Modules.AR.Models;

namespace Registrai.Modules.AR.Methods;

/// <summary>Adreso detalių gavimo metodas</summary>
public static class ARDetails {
    enum AdrType { adm, sav, sen, gyv, gat, aob, pat }

    /// <summary>Detali adreso informacija</summary>
    /// <param name="ctx"></param>
    /// <param name="id">AR kodas</param>
    /// <param name="details">Išsami informacija</param>
    /// <returns></returns>
    public static async Task Detales(HttpContext ctx, int id, bool? details = false) {
        var dtl = details ?? true;
        using var db = new DBRead(
            "SELECT id, src, pavad, vietove, tipas, reg_data, aob_post, aob_lks, aob_wgs, " +
                //  0   1    2      3        4      5         6         7        8
                "adm_kodas, sav_kodas, sen_kodas, gyv_kodas, gat_kodas, aob_kodas, aob_nr, aob_korpusas, aob_patalpa " +
            //9          10         11         12         13         14         15      16            17
            (dtl ?
                ",adm_vardas, adm_tipas, adm_trump, adm_cnt, " +
                //18          19         20         21
                "sav_vardas, sav_tipas, sav_trump, sav_cnt, sav_mis, " +
                //22          23         24         25       26
                "sen_vardas, sen_tipas, sen_trump, sen_cnt, " +
                //27          28         29         30
                "gyv_vardas, gyv_pavad, gyv_tipas, gyv_trump, gyv_cnt, gyv_mis, " +
                //31          32         33        34		   35       36
                "gat_vardas, gat_tipas, gat_trump, gat_cnt, aob_cnt, dydis "
            //	  37          38         39         40       41       42
            : "") +
            $"FROM ar.v_app_detales WHERE id={id}");
        using var rdr = await db.GetReader();
        var ret = new AR_Item() { ID = id };
        if (await rdr.ReadAsync()) {

            if (Enum.TryParse<AdrType>(rdr.GetString(1), out var src)) {
                ret.Pavad = rdr.GetStringN(2); ret.Vietove = rdr.GetStringN(3); ret.Tipas = rdr.GetStringN(4);
                ret.RegData = DateOnly.FromDateTime(rdr.GetDateTime(5));
                ret.Post = rdr.GetStringN(6);
                ret.LKS = rdr.IsDBNull(7) ? null : (int[])rdr.GetValue(7);
                ret.WGS = rdr.IsDBNull(8) ? null : (double[])rdr.GetValue(8);
                ret.Nr = rdr.GetStringN(15); ret.Korp = rdr.GetStringN(16); ret.Pat = rdr.GetStringN(17);

                var kd = new AR_ItemCodes() {
                    Adm = rdr.GetIntN(9),
                    Sav = rdr.GetIntN(10),
                    Sen = rdr.GetIntN(11),
                    Gyv = rdr.GetIntN(12),
                    Gat = rdr.GetIntN(13),
                    Aob = rdr.GetIntN(14),
                    Pat = src == AdrType.pat ? id : null
                };

                if (dtl) {
                    if (kd.Adm is not null) ret.Adm = new() { ID = kd.Adm, Vardas = rdr.GetStringN(18), Tipas = rdr.GetStringN(19), Trump = rdr.GetStringN(20), Chc = rdr.GetIntN(21) };
                    if (kd.Sav is not null) ret.Sav = new() { ID = kd.Sav, Vardas = rdr.GetStringN(22), Tipas = rdr.GetStringN(23), Trump = rdr.GetStringN(24), Chc = rdr.GetIntN(25), Chm = rdr.GetIntN(26) };
                    if (kd.Sen is not null) ret.Sen = new() { ID = kd.Sen, Vardas = rdr.GetStringN(27), Tipas = rdr.GetStringN(28), Trump = rdr.GetStringN(29), Chc = rdr.GetIntN(30) };
                    if (kd.Gyv is not null) ret.Gyv = new() { ID = kd.Gyv, Vardas = rdr.GetStringN(31), Pavad = rdr.GetStringN(32), Tipas = rdr.GetStringN(33), Trump = rdr.GetStringN(34), Chc = rdr.GetIntN(35), Chm = rdr.GetIntN(36) };
                    if (kd.Gat is not null) ret.Gat = new() { ID = kd.Gat, Vardas = rdr.GetStringN(37), Tipas = rdr.GetStringN(38), Trump = rdr.GetStringN(39), Chc = rdr.GetIntN(40) };
                    if (kd.Aob is not null) ret.Aob = new() { ID = kd.Aob, Vardas = ret.Nr + (ret.Korp is null ? "" : " K" + ret.Korp) + (ret.Pat is null ? "" : "-" + ret.Pat), Chc = rdr.GetIntN(41) };
                    if (!rdr.IsDBNull(42)) ret.Dydis = rdr.GetLongN(42);
                }
                else {
                    ret.Kodai = kd;
                }
                await ctx.Response.WriteAsJsonAsync(ret);
            }
            else { ctx.Response.E400(true, "Invalid source (src)"); }
        }
        else { ctx.Response.E404(true); }
    }

}