using App;
using App.Routing;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace Registrai.Modules.UPD;

/// <summary>UPD duomenų gavimo metodai</summary>
public static partial class UpdMedicines {

	static readonly List<string> ListFld = ["med_id", "med_date", "list_lt", "list_en", "search"];
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
			Search = ctx.ParamString("q")?.MkSerach(),
			WhereAdd = "med_idf is not null",
			Fields = ListFld,
			Select = en ? ListEn : ListLt
		}.Execute();
		await ctx.Response.WriteAsJsonAsync(m);
	}


	/// <summary>Vaistų sąrašas (detali paieška)</summary>
	/// <param name="ctx"></param>
	/// <param name="qry">Filtro užklausa</param>
	/// <returns></returns>
	public static async Task ListFilter(HttpContext ctx, MedQuery qry) {
		var en = ctx.ParamString("lang")?.ToLower() == "en";
#if DEBUG
		var tbl = ctx.ParamTrue("uat") ? "upd_uat.v_med" : "upd.v_med";
#else
		var tbl = "upd.v_med";
#endif
		qry.Search = qry.Search?.MkSerach();
		var (qr, whr, prm) = GetQuery(tbl, qry);
		var cnt = await GetCount(tbl, qr, whr, prm);

		if (cnt > 0) {
			var m = await new DBPagingRequest<MedListItem>(tbl, DB.VVR) {
				Limit = (qry.Limit ?? 25).Limit(100),
				Page = qry.Page ?? 1,
				Sort = qry.Order ?? "med_date",
				Total = false,
				Desc = qry.Order is null || qry.Desc,
				JsonField = en ? "list_en" : "list_lt",
				Search = qry.Search,
				Params = prm,
				WhereAdd = whr,
				Fields = ListFld,
				Select = en ? ListEn : ListLt
			}.Execute();
			m.Total = cnt;
			await ctx.Response.WriteAsJsonAsync(m); 
		} else {
			await ctx.Response.WriteAsJsonAsync(new DBPagingResponse<MedListItem>());
		}
	}

	private static readonly char[] MkSrhExclude = ['-', '/'];
	private static string? MkSerach(this string? q) => q?.RemoveAccents().RemoveNonAlphanumeric(MkSrhExclude).ToLower();
	private static readonly ConcurrentDictionary<string, (int num, DateTime tmo)> Counts = [];
	private static DateTime CountClean { get; set; }
	private static async Task<int> GetCount(string table, string qry, string whr, Dictionary<string, object?> prm) {
		if (Counts.TryGetValue(qry, out var cnt)) {
			var now = DateTime.UtcNow;
			if (cnt.tmo > now) return cnt.num;
			else if (CountClean < now) {
				CountClean.AddMinutes(10);
				var clr = new List<string>();
				foreach (var i in Counts) if (i.Value.tmo < now) clr.Add(i.Key);
				foreach (var i in clr) Counts.TryRemove(i, out _);
			}
		}
		using var db = new DBRead($"SELECT count(*) FROM {table} WHERE {whr};", prm, DB.VVR);
		var ret = await db.GetScalar<long>();
		return (Counts[qry] = ((int)ret, DateTime.UtcNow.AddMinutes(5))).num;
	}

	private static (string qry, string whr, Dictionary<string, object?> prm) GetQuery(string table, MedQuery qry) {
		var prm = new Dictionary<string, object?>() { { "@spc", qry.Species ?? (object)DBNull.Value }, { "@leg", qry.LegalCode ?? (object)DBNull.Value }, { "@frm", qry.DoseForm ?? (object)DBNull.Value } };
		var qryadd = new List<string>() { "med_idf is not null" };
		var totcnt = new HashSet<string>();
		if (qry.Species?.Count > 0) { qryadd.Add("flt_species && @spc"); totcnt.Add("spc:" + string.Join(',', qry.Species)); }
		if (qry.LegalCode?.Count > 0) { qryadd.Add("flt_legal = ANY(@leg)"); totcnt.Add("leg:" + string.Join(',', qry.LegalCode)); }
		if (qry.DoseForm?.Count > 0) { qryadd.Add("flt_form = ANY(@frm)"); totcnt.Add("frm:" + string.Join(',', qry.DoseForm)); }

		if (!string.IsNullOrEmpty(qry.Search)) {
			var qs = qry.Search.Split(" ");
			for (var i = 0; i < qs.Length; i++) {
				var j = qs[i];
				qryadd.Add($"search like '%'||@s{i}||'%'");
				prm[$"@s{i}"] = j;
			}
		}

		return ($"{qry.Search}{string.Join(',', totcnt)}", string.Join(" and ", qryadd), prm);
	}



	/// <summary>Filtrų sąrašas</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task Filters(HttpContext ctx) {
		var en = ctx.ParamString("lang")?.ToLower() == "en";
#if DEBUG
		var tbl = ctx.ParamTrue("uat") ? "upd_uat.v_med_filter_list" : "upd.v_med_filter_list";
#else
		var tbl = "upd.v_med_filter_list";
#endif
		using var db = new DBRead($"SELECT {(en ? "en" : "lt")} FROM {tbl};", DB.VVR);
		var dt = await db.GetBytes();
		if (dt is not null) {
			ctx.Response.ContentType = "application/json; charset=utf-8";
			ctx.Response.ContentLength = dt.Length;
			await ctx.Response.Body.WriteAsync(dt);
		}
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
			//var m = await db.GetObject<Medicine>(0);
			//if (m is null) ctx.Response.E404(true);
			//else await ctx.Response.WriteAsJsonAsync(m);

			var dt = await db.GetBytes();
			if (dt is not null) {
				//TODO: Remove this
				_ = Task.Run(() => { _ = JsonSerializer.Deserialize<Medicine>(Encoding.UTF8.GetString(dt)); });

				ctx.Response.ContentType = "application/json; charset=utf-8";
				ctx.Response.ContentLength = dt.Length;
				await ctx.Response.Body.WriteAsync(dt);
			}

		}
		else ctx.Response.E404(true); //TODO: Normalią klaidą grąžinti
	}

	/// <summary>Gauti trūkstamus vertimus</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task Refs(HttpContext ctx) {
		if (DateTime.TryParse(ctx.ParamString("dt"), out var dt)) {
			using var db = new DBRead($"WITH grp as (SELECT log_code, sum(log_count) cnt FROM upd.log_translate WHERE log_date>@dt GROUP BY log_code),\r\n\tprs as (SELECT jsonb_build_array(list_id, log_code, cnt, status_code, name_en) dt FROM grp LEFT JOIN spor.ref_terms trm ON (trm.term_id = grp.log_code) ORDER BY cnt desc)\r\nSELECT jsonb_agg(dt) FROM prs;", DB.VVR, ("@dt", dt));
			var m = await db.GetObject<List<List<object?>>>(0);
			if (m is null) ctx.Response.E404(true);
			else await ctx.Response.WriteAsJsonAsync(m);
		}
		else ctx.Response.E404(true); //TODO: Normalią klaidą grąžinti
	}


	private static readonly XmlReaderSettings XmlProps = new() { Async = true, IgnoreWhitespace = true, DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };

	private static UPDRequest UPD { get; set; } = new("upd._cfg");
#if DEBUG
	private static UPDRequest UPDUAT { get; set; } = new("upd_uat._cfg");
#endif

	/// <summary>Atsisiųsti pakuotės lapelį</summary>
	/// <param name="ctx"></param>
	/// <param name="id">Vaisto ID</param>
	/// <param name="file">Failo ID</param>
	/// <returns></returns>
	public static async Task DownloadFile(HttpContext ctx, long id, string file) {
		if (Guid.TryParse(file, out var gid)) {
#if DEBUG
			var tbl = ctx.ParamTrue("uat") ? "upd_uat.docs" : "upd.docs";
			var cl = ctx.ParamTrue("uat") ? UPDUAT.GetClient() : UPD.GetClient();
#else
			var tbl = "upd.docs";
			var cl = UPD.GetClient();
#endif
			using var db = new DBRead($"SELECT med_id \"MedId\", doc_id \"Id\", doc_status \"Status\", doc_date \"Date\", doc_file_name \"File\", doc_file_type \"Type\" FROM {tbl} WHERE doc_id=@file and med_id=@id;", DB.VVR, ("@file", gid), ("@id", id));
			var dt = await db.GetObject<DocumentInfo>();
			if (dt is not null) {
				var req = new HttpRequestMessage(HttpMethod.Get, file);
				req.Headers.Accept.Add(new("application/fhir+xml"));

				using var rsp = await cl.SendAsync(req);
				if (rsp.IsSuccessStatusCode) {
					await using var strm = await rsp.Content.ReadAsStreamAsync();
					using var rdr = XmlReader.Create(strm, XmlProps);
					while (await rdr.ReadAsync())
						if (rdr.IsStartElement("attachment"))
							while (await rdr.ReadAsync())
								if (rdr.IsStartElement("data"))
									if (rdr.MoveToAttribute("value")) {
										if (ctx.ParamTrue("preview")) {
											ctx.Response.ContentType = dt.Type;
											ctx.Response.Headers.ContentDisposition = $"inline; filename=\"{dt.File}\"";											
										}
										else {
											ctx.Response.ContentType = "application/octet-stream";
											ctx.Response.Headers.ContentDisposition = $"attachment; filename=\"{dt.File}\"";
										}
										while (rdr.ReadAttributeValue())
											if (rdr.NodeType == XmlNodeType.Text) {
												string chunk = rdr.Value;
												if (!string.IsNullOrEmpty(chunk))
													await ctx.Response.Body.WriteAsync(Convert.FromBase64String(chunk));
											}

									}
					if (!ctx.Response.HasStarted) ctx.Response.E500();
				}
				else ctx.Response.E404();
			}
			else ctx.Response.E404();
		}
		else ctx.Response.E404();
	}
}

/// <summary>EMA UPD integracija</summary>
/// <param name="table">Konfiguracijos lentelė</param>
public class UPDRequest(string table) {
	/// <summary>Konfiguracijos lentelė</summary>
	public string Table { get; set; } = table;
	private HttpClient Client { get; set; } = new();
	private DateTime ClientTimeout { get; set; }
	private AuthConfig Cfg { get; set; } = new();
	/// <summary>Užklausos klientas</summary>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public HttpClient GetClient() {
		lock (Client) {
			if (ClientTimeout < DateTime.UtcNow) {
				LoadConfig().Wait();
				var urlToken = new Uri(Cfg.AuthEndpoint ?? "");
				var formContent = new FormUrlEncodedContent([
					new ("client_id", Cfg.AuthClientId),
					new ("client_secret", Cfg.AuthClientSecret),
					new ("grant_type", "client_credentials"),
					new ("scope", Cfg.AuthScope)
				]);
				Client.BaseAddress = new((Cfg.UrlDoc ?? "") + "/");

				using var rsp = Client.PostAsync(urlToken, formContent).Result;
				if (rsp.IsSuccessStatusCode) {
					var at = rsp.Content.ReadFromJsonAsync<AuthToken>().Result;
					if (!string.IsNullOrEmpty(at?.AccessToken) && !string.IsNullOrEmpty(at?.TokenType)) {
						Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(at.TokenType, at.AccessToken);
						ClientTimeout = DateTime.UtcNow.AddSeconds(at.Expire * 0.8);
					}
					else throw new Exception("Auth Error");
				}
				else throw new Exception($"Auth Error {rsp.StatusCode}");

			}
		}
		return Client;
	}

	private async Task LoadConfig() {
		using var db = new DBRead($"SELECT cfg_key, cfg_value FROM {Table}", DB.VVR);
		using var rdr = await db.GetReader();
		Cfg = new();
		var tp = typeof(AuthConfig);
		while (await rdr.ReadAsync()) tp.GetProperty(rdr.GetString(0))?.SetValue(Cfg, rdr.GetStringN(1));
	}


	private class AuthConfig {
		public string? UrlMain { get; set; }
		public string? UrlDoc { get; set; }
		public string? AuthEndpoint { get; set; }
		public string? AuthClientId { get; set; }
		public string? AuthClientSecret { get; set; }
		public string? AuthScope { get; set; }
	}

	private class AuthToken {
		[JsonPropertyName("token_type")] public string? TokenType { get; set; }
		[JsonPropertyName("expires_in")] public int Expire { get; set; }
		[JsonPropertyName("ext_expires_in")] public int ExtExpire { get; set; }
		[JsonPropertyName("access_token")] public string? AccessToken { get; set; }
	}
}


