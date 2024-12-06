using App;
using App.Routing;
using Microsoft.AspNetCore.Http;
using Registrai.Modules.AR.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Registrai.Modules.AR.Methods;

/// <summary>Adreso detalių gavimo metodas</summary>
public static partial class ARGEO {
	enum AdrType { adm, sav, sen, gyv, gat, aob, pat }

	private static (double x, double y, bool valid) MkKord(double x, double y) {
		if (x > y) (y, x) = (x, y);
		if (x < 200) (x, y) = GeoConverter.GeoToGrid(y, x);
		return (x,y,(x < 700000 && x > 300000 && y < 6300000 && y > 5950000));
	}
	[GeneratedRegex(@"[\d.]+")] private static partial Regex GetKoordRgx();
	private static readonly CultureInfo GetKoordCIn = new ("en-US");
	private static List<double> GetKoord(string input) {
		var ret = new List<double>();
		foreach (Match match in GetKoordRgx().Matches(input))
			if (double.TryParse(match.Value, GetKoordCIn, out double number)) ret.Add(number);
		return ret;
	}

	/// <summary>Artimiausio adreso paieška</summary>
	/// <param name="ctx"></param>
	/// <param name="x">LKS vakarų kryptis (x) / WGS latuma (lat)</param>
	/// <param name="y">LKS šiaurės kryptis (y) / WGS ilguma (lon)</param>
	/// <param name="q">Koordinatės iš teksto</param>
	/// <param name="dist">Maksimalus atstumas nuo taško</param>
	/// <param name="limit">Rezultatų kiekis</param>
	/// <returns></returns>
	public static async Task AobSearch(HttpContext ctx, double? x = 0, double? y = 0, string? q = null, int dist = 50, int limit = 5) {
		if (q is not null) { var k = GetKoord(q); if (k.Count > 1) { x = k[0]; y = k[1]; } }
		if (x is null || y is null) { ctx.Response.E400(true, "Invalid search coordinates"); return; }
		var z = MkKord(x.Value, y.Value);
		if (!z.valid) { ctx.Response.E400(true, "Invalid coordinates"); return; }
		if (limit > 50) limit = 50;

		using var db = new DBRead(
			"SELECT d.id, z.dist, d.pavad, d.vietove, d.tipas, d.reg_data, d.aob_post, d.aob_lks, d.aob_wgs, " +
			//        0     1      2        3          4        5           6           7          8
			"d.adm_kodas, d.sav_kodas, d.sen_kodas, d.gyv_kodas, d.gat_kodas, d.aob_kodas, d.aob_nr, d.aob_korpusas  " +
			//9          10         11         12         13         14         15      16           
			$"FROM ar.geo_adr_near(@x,@y,{dist},{limit}) z LEFT JOIN ar.v_app_detales as d on (z.aob = d.id);",
			new() { { "@x", z.x }, { "@y", z.y } }
		);
		using var rdr = await db.GetReader();
		var ret = new List<AR_GEOItem>();
		while (await rdr.ReadAsync()) {
			ret.Add(new() {
				ID = rdr.GetInt32(0), Atstumas = rdr.GetDouble(1), Pavad = rdr.GetStringN(2), Vietove = rdr.GetStringN(3), Tipas = rdr.GetStringN(4),
				RegData = DateOnly.FromDateTime(rdr.GetDateTime(5)), Post = rdr.GetStringN(6),
				LKS = rdr.IsDBNull(7) ? null : (int[])rdr.GetValue(7), WGS = rdr.IsDBNull(8) ? null : (double[])rdr.GetValue(8),
				Nr = rdr.GetStringN(15), Korp = rdr.GetStringN(16),
				Kodai = new() {
					Adm = rdr.GetIntN(9), Sav = rdr.GetIntN(10),
					Sen = rdr.GetIntN(11), Gyv = rdr.GetIntN(12),
					Gat = rdr.GetIntN(13), Aob = rdr.GetIntN(14)
				}
			});
		}
		await ctx.Response.WriteAsJsonAsync(ret);
	}

	/// <summary>Gyvenvietė pagal koordinates (GYV)</summary>
	/// <param name="ctx"></param>
	/// <param name="x">LKS vakarų kryptis (x) / WGS latuma (lat)</param>
	/// <param name="y">LKS šiaurės kryptis (y) / WGS ilguma (lon)</param>
	/// <param name="q">Koordinatės iš teksto</param>
	/// <returns></returns>
	public static async Task Gyv(HttpContext ctx, double? x = 0, double? y = 0, string? q = null) {
		if (q is not null) { var k = GetKoord(q); if (k.Count > 1) { x = k[0]; y = k[1]; } }
		if (x is null || y is null) { ctx.Response.E400(true, "Invalid search coordinates"); return; }
		var z = MkKord(x.Value, y.Value);
		if (!z.valid) { ctx.Response.E400(true, "Invalid coordinates"); return; }

		using var db = new DBRead(
			"SELECT id, pavad, vietove, gyv_pavad, gyv_tipas, gyv_trump, reg_data, adm_kodas, adm_vardas, adm_tipas, adm_trump, " +
			//      0   1      2        3          4          5          6         7          8           9          10
			"sav_kodas, sav_vardas, sav_tipas, sav_trump, sen_kodas, sen_vardas, sen_tipas, sen_trump " +
			//11        12          13         14         15         16          17         18
			"FROM ar.v_app_detales WHERE id=(SELECT gyv_kodas FROM ar.geo_4_gyvenvietes WHERE ar.ST_Contains(geom, ar.ST_Point(@x,@y,3346)) LIMIT 1);",
			new() { { "@x", z.x }, { "@y", z.y } }
		);
		using var rdr = await db.GetReader();
		if (await rdr.ReadAsync()) {
			var ret = new AR_GEOGyvItem() {
				ID = rdr.GetInt32(0), Vardas = rdr.GetStringN(1), Pavad = rdr.GetStringN(3), Vietove = rdr.GetStringN(2), Tipas = rdr.GetStringN(4),
				Trump = rdr.GetStringN(5), RegData = DateOnly.FromDateTime(rdr.GetDateTime(6)),
				Adm = new() { ID = rdr.GetIntN(7), Vardas = rdr.GetStringN(8), Tipas = rdr.GetStringN(9), Trump = rdr.GetStringN(10) },
				Sav = new() { ID = rdr.GetIntN(11), Vardas = rdr.GetStringN(12), Tipas = rdr.GetStringN(13), Trump = rdr.GetStringN(14) },
				Sen = rdr.IsDBNull(15) ? null : new() { ID = rdr.GetIntN(15), Vardas = rdr.GetStringN(16), Tipas = rdr.GetStringN(17), Trump = rdr.GetStringN(18) }
			};
			await ctx.Response.WriteAsJsonAsync(ret);
		}
		else { ctx.Response.E404(true); }
	}

	/// <summary>Savivaldybė pagal koordinates (SAV)</summary>
	/// <param name="ctx"></param>
	/// <param name="x">LKS vakarų kryptis (x) / WGS latuma (lat)</param>
	/// <param name="y">LKS šiaurės kryptis (y) / WGS ilguma (lon)</param>
	/// <param name="q">Koordinatės iš teksto</param>
	/// <returns></returns>
	public static async Task Sav(HttpContext ctx, double? x = 0, double? y = 0, string? q = null) {
		if (q is not null) { var k = GetKoord(q); if (k.Count > 1) { x = k[0]; y = k[1]; } }
		if (x is null || y is null) { ctx.Response.E400(true, "Invalid search coordinates"); return; }
		var z = MkKord(x.Value, y.Value);
		if (!z.valid) { ctx.Response.E400(true, "Invalid coordinates"); return; }

		using var db = new DBRead(
			"SELECT id, pavad, vietove, sav_vardas, sav_tipas, sav_trump, reg_data, adm_kodas, adm_vardas, adm_tipas, adm_trump " +
			//      0   1      2        3           4          5          6         7          8           9          10
			"FROM ar.v_app_detales WHERE id=(SELECT sav_kodas::int FROM ar.geo_2_savivaldybes WHERE ar.ST_Contains(geom, ar.ST_Point(@x,@y,3346)) LIMIT 1);",
			new() { { "@x", z.x }, { "@y", z.y } }
		);
		using var rdr = await db.GetReader();
		if (await rdr.ReadAsync()) {
			var ret = new AR_GEOGyvItem() {
				ID = rdr.GetInt32(0), Pavad=rdr.GetStringN(1), Vietove = rdr.GetStringN(2), Vardas = rdr.GetStringN(3), Tipas = rdr.GetStringN(4),
				Trump = rdr.GetStringN(5), RegData = DateOnly.FromDateTime(rdr.GetDateTime(6)),
				Adm = new() { ID = rdr.GetIntN(7), Vardas = rdr.GetStringN(8), Tipas = rdr.GetStringN(9), Trump = rdr.GetStringN(10) }
			};
			await ctx.Response.WriteAsJsonAsync(ret);
		}
		else { ctx.Response.E404(true); }
	}

	private static class GeoConverter {
		private const double Pi = Math.PI;
		private const double A = 6378137;
		private const double B = A * (1 - (1 / 298.257223563));
		private const double C = (A * A - B * B) / (A * A);
		private const double E = (1 - (C / 4) - (3 * C * C / 64) - (5 * C * C * C / 256));
		private const double F = ((3.0 / 8) * (C + (C * C / 4) + (15 * C * C * C / 128)));
		private const double G = ((15.0 / 256) * (C * C + (3 * C * C * C / 4)));
		private const double I = (35 * C * C * C / 3072);
		private const double J = A * (1 - C);
		private const double K = 0.9998;

		public static (double east, double north) GeoToGrid(double lat, double lon, int round = 2) {
			double latrad = lat * (Pi / 180);
			double lonrad = (lon - 24) * (Pi / 180);
			double coslat = Math.Cos(latrad);
			double sinlat = Math.Sin(latrad);
			double lr2c = 1 - (C * sinlat * sinlat);
			double t = Math.Tan(latrad); var t2 = t * t; var t4 = Math.Pow(t, 4); var t6 = Math.Pow(t, 6);
			double nu = A / Math.Sqrt(lr2c); var nusin = nu * sinlat;
			double psi = nu / J / Math.Pow(lr2c, 1.5); var psi2 = psi * psi;
			double m = A * (E * latrad - F * Math.Sin(2 * latrad) + G * Math.Sin(4 * latrad) - I * Math.Sin(6 * latrad));

			return (
				Math.Round(500000 + (K * nu * lonrad * coslat * (1 + ((Math.Pow(lonrad, 2) / 6) * coslat * coslat * (psi - t2)) +
					((Math.Pow(lonrad, 4) / 120) * Math.Pow(coslat, 4) * (4 * Math.Pow(psi, 3) * (1 - 6 * t2) + psi2 * (1 + 8 * t2) - psi * 2 * t2 + t4)) +
					((Math.Pow(lonrad, 6) / 5040) * Math.Pow(coslat, 6) * (61 - 479 * t2 + 179 * t4 - t6)))), round),
				Math.Round(K * (m + ((Math.Pow(lonrad, 2) / 2) * nusin * coslat) + ((Math.Pow(lonrad, 4) / 24) * nusin * Math.Pow(coslat, 3) * (4 * psi2 + psi - t2)) +
					((Math.Pow(lonrad, 6) / 720) * nusin * Math.Pow(coslat, 5) * (8 * Math.Pow(psi, 4) * (11 - 24 * t2) - 28 * Math.Pow(psi, 3) * (1 - 6 * t2) + psi2 * (1 - 32 * t2) - psi * 2 * t2 + t4)) +
					((Math.Pow(lonrad, 8) / 40320) * nusin * Math.Pow(coslat, 7) * (1385 - 3111 * t2 + 543 * t4 - t6))), round)
				);
		}
	}
}