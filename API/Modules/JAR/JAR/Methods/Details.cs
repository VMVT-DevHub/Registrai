using API;
using App.Routing;
using Microsoft.AspNetCore.Http;
using Modules.JAR.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.JAR.Methods;

/// <summary>Juridinio asmens detalių gavimo metodas</summary>
public static class JARDetails {
	enum AdrType { adm, sav, sen, gyv, gat, aob, pat }

	/// <summary>Detali juridinio asmens informacija</summary>
	/// <param name="ctx"></param>
	/// <param name="id">JAR kodas</param>
	/// <returns></returns>
	public static async Task Detales(HttpContext ctx, int id) {
		using var db = new DBRead(
			$"SELECT ja_kodas, ja_pavadinimas, adresas, aob_kodas, form_kodas, form_pavadinimas, status_kodas, stat_pavadinimas, stat_data, reg_data, aob_data, isreg_data, formavimo_data FROM jar.v_app_detales WHERE ja_kodas={id};");
		//          0         1               2        3          4           5                 6             7                 8          9         10        11          12    
		using var rdr = await db.GetReader();
		var ret = new JAR_Item() { ID = id };
		if (await rdr.ReadAsync()) {
			ret.Pavad = rdr.GetString(1);
			ret.Adresas = rdr.GetStringN(2);
			ret.AobKodas = rdr.GetIntN(3);
			ret.FormKodas = rdr.GetIntN(4);
			ret.Forma = rdr.GetStringN(5);
			ret.StatusKodas = rdr.GetIntN(6);
			ret.Statusas = rdr.GetStringN(7);
			ret.StatData = rdr.GetDateOnlyN(8);
			ret.RegData = rdr.GetDateOnlyN(9);
			ret.AobData = rdr.GetDateOnlyN(10);
			ret.IsregData = rdr.GetDateOnlyN(11);
			ret.ModData = rdr.GetDateOnlyN(12);

			await ctx.Response.WriteAsJsonAsync(ret);
		}
		else ctx.Response.E404(true);
	}
}