using Npgsql;
using System.Reflection;
using System.Text.Json;

namespace App {
	/// <summary></summary>
	public class DBRead : IDisposable {
		private NpgsqlConnection Conn { get; }
		private NpgsqlCommand Cmd { get; }
		/// <summary>Duomenų skaitymas</summary>
		/// <returns>Npgsql duomenų skaitytuvas</returns>
		public async Task<NpgsqlDataReader> GetReader() {
			await Conn.OpenAsync();
			return await Cmd.ExecuteReaderAsync();
		}

		/// <summary>Gauti objektą iš duomenų bazės pirmo įrašo</summary>
		/// <typeparam name="T">Objekto klasė</typeparam>
		/// <returns>Suformuotas objektas</returns>
		public async Task<T?> GetObject<T>() where T: new() {
			using var rdr = await GetReader();
			return await rdr.GetObject<T>();
		}

		/// <summary></summary>
		/// <param name="db"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		public DBRead(string sql, Dictionary<string, object?>? param = null, DB? db=null) {
			db ??= App.DB.Master;
			Conn = new NpgsqlConnection(db.ConnStr);
			Cmd = new NpgsqlCommand(sql, Conn);
			if (param?.Count > 0) {
				foreach (var p in param) Cmd.Parameters.Add(new(p.Key, p.Value));
				if (db.Debug) {
					var inc = db.DebugIncr++;
					Console.WriteLine($"[SQL{inc}]: {sql}");
					Console.WriteLine($"[SQL{inc}]: {JsonSerializer.Serialize(param)}");
				}
			}
			else if (db.Debug) {
				Console.WriteLine($"[SQL{db.DebugIncr++}]: {sql}");
			}
		}

		//TODO: nauja funkcija parametrams sukrauti;

		/// <summary></summary>
		/// <param name="db"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		public DBRead(string sql, DB? db, params (string key, object? val)[] param) {
			db ??= App.DB.Master;
			Conn = new NpgsqlConnection(db.ConnStr);
			Cmd = new NpgsqlCommand(sql, Conn);
			if (param?.Length > 0) {
				foreach (var (key, val) in param) Cmd.Parameters.Add(new(key, val));
				if (db.Debug) {
					var inc = db.DebugIncr++;
					Console.WriteLine($"[SQL{inc}]: {sql}");
					Console.WriteLine($"[SQL{inc}]: {JsonSerializer.Serialize(param.ToDictionary(x => x.key, x => x.val))}");
				}
			}
			else if (db.Debug) {
				Console.WriteLine($"[SQL{db.DebugIncr++}]: {sql}");
			}
		}

		// To detect redundant calls
		private bool IsDisposed;
		/// <summary>Duomenų bazės uždarymo metodas</summary>
		public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
		/// <summary>Duomenų bazės uždarymo metodas</summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing) {
			if (!IsDisposed) {
				if (disposing) {
					try {
						Cmd.Dispose();
						Conn.Dispose();
					} catch (Exception ex) {
						Console.WriteLine($"[SQLTranError] Dispose  {ex.Message}");
						Console.WriteLine(ex.StackTrace);
					}
				}
				IsDisposed = true;
			}
		}

	}

	/// <summary>Duomenų bazės pagalbininkas</summary>
	/// <remarks>Naujas duomenų bazės prisijungimas</remarks>
	/// <param name="conn"></param>
	public class DB(string conn) {
		/// <summary>Pagrindinis DB prisijungimas</summary>
		public static DB Master { get; set; } = new ("User ID=postgres; Password=postgres; Server=localhost:5432; Database=master;");
		/// <summary>VVR DB prisijungimas</summary>
		public static DB VVR { get; set; } = new ("User ID=postgres; Password=postgres; Server=localhost:5432; Database=VVR;");


		//TODO: addminutes from config
		/// <summary>Įrašų skaičiaus atnaujinimas (sekundės)</summary>
		public int CountReset { get; set; } = 5000;

		/// <summary>Print query to console</summary>
		public bool Debug { get; set; }
		/// <summary>Print query numbering</summary>
		public long DebugIncr { get; set; }

		/// <summary>Prisijungimo tekstas</summary>
		public string ConnStr { get; set; } = conn;
		/// <summary>Vykdyti užklausą</summary>
		/// <param name="sql">Užklausa</param>
		/// <returns>Paveiktų įrašų skaičius</returns>
		public async Task<int> Execute(string sql) {
			var conn = new NpgsqlConnection(ConnStr); await conn.OpenAsync();
			return await new NpgsqlCommand(sql, conn).ExecuteNonQueryAsync();
		}

		//TODO: Clear old COUNTS
		private readonly Dictionary<string, (int num, DateTime tmo)> Counts = [];

		/// <summary>Užklausos įrašų skaičiaus gavimas</summary>
		/// <param name="table">Lentelė</param>
		/// <param name="where">WHERE sąlyga</param>
		/// <param name="param">Užklausos parametrai</param>
		/// <returns>Įrašų skaičius</returns>
		public async Task<int> GetCount(string table, string? where, Dictionary<string, object?>? param = null) {
			var qry = $"{table}{where}";
			if (param?.Count > 0) foreach (var i in param) qry += i.Value?.ToString();
			if (Counts.TryGetValue(qry, out var cnt) && cnt.tmo > DateTime.UtcNow) return cnt.num;
			using var db = new DBRead($"SELECT count(*) FROM {table}{where};", param, this);
			using var rdr = await db.GetReader();
			if (await rdr.ReadAsync()) return (Counts[qry] = (rdr.GetInt32(0), DateTime.UtcNow.AddSeconds(CountReset))).num;
			return 0;
		}
	}

	/// <summary>Duomenų puslapiavimo filtras</summary>
	/// <typeparam name="T">Duomenų tipas</typeparam>
	public class DBPagingFilter<T> {
		/// <summary>Įrašų kiekis</summary>
		/// <example>20</example>
		public int Top { get; set; } = 50;
		/// <summary>Puslapio numeris</summary>
		/// <example>1</example>
		public int Page { get; set; } = 1;
		/// <summary>Įrašų rikiavimas</summary>
		/// <example>ID</example>
		public string Order { get; set; } = "ID";
		/// <summary>Paeiškos frazė</summary>
		public string? Search { get; set; }
		/// <summary>Rikiavimo tvarka</summary>
		/// <example>false</example>
		public bool Desc { get; set; }
		/// <summary>Filtras</summary>
		public T? Filter { get; set; }
	}
	/// <summary>Duomenų puslapiavimo užklausa</summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks>Puslapiavimo užklausos konstruktorius</remarks>
	/// <param name="db">Duomenų bazės prisijungimas</param>
	/// <param name="table">Lentelės pavadinimas</param>
	public class DBPagingRequest<T>(string table, DB? db = null) where T : new() {
		private DB Db { get; set; } = db ?? DB.Master;
		/// <summary>Lentelės pavadinimas</summary>
		public string Table { get; set; } = table;
		/// <summary>Užklausos ribojimas</summary>
		public T? Where { get; set; }
		/// <summary>Užklausos ribojimas</summary>
		public string? WhereAdd { get; set; }
		/// <summary>Paieškos laukas</summary>
		public string? Search { get; set; }
		/// <summary>Paieška prasideda fraze</summary>
		public string? SearchSort { get; set; }
		/// <summary>Paieška prasideda fraze</summary>
		public bool StartsWith { get; set; }
		/// <summary>Puslapio dydis</summary>
		public int Limit { get; set; } = 50;
		/// <summary>Puslapis</summary>
		public int Page { get; set; } = 1;
		/// <summary>Rikiavimas</summary>
		public string? Sort { get; set; } = "ID";
		/// <summary>Rodomi duomenų laukai</summary>
		public List<string>? Select { get; set; }
		/// <summary>Duomenų lentelės laukai</summary>
		public List<string>? Fields { get; set; }
		/// <summary>Didėjančia tvarka</summary>
		public bool Desc { get; set; }
		/// <summary>Get total number of rows</summary>
		public bool Total { get; set; } = true;
		/// <summary>Vykdyti užklausą</summary>
		/// <returns></returns>
		public async Task<DBPagingResponse<T>> Execute() {
			if (Fields is null) { throw new Exception("Missing data fields"); }
			if (Select is null) { throw new Exception("Missing select fields"); }
			if (Sort is not null && !Fields.Contains(Sort)) { throw new Exception("Sort not valid"); }
			var srt = $"\"{Sort}\""; var slt = $"\"{string.Join("\",\"", Select)}\"";
			var advs = false;

			string where = ""; var param = new Dictionary<string, object?>();
			var whr = new List<string>(); if (!string.IsNullOrEmpty(WhereAdd)) whr.Add(WhereAdd);
			if (Where is not null) {
				foreach (var i in typeof(T).GetProperties()) {
					var n = i.Name;
					var pv = i.GetValue(Where);
					if (pv is not null) {
						if (!Fields.Contains(n)) { throw new Exception($"Invalid search field '{n}'"); }
						if (i.PropertyType == typeof(int?) && (int)pv == -1) whr.Add($"\"{n}\" is null");
						else { param[$"@{n}"] = pv; whr.Add($"\"{n}\"=@{n}"); }
					}
				}
			}
			if (!string.IsNullOrWhiteSpace(Search)) {
				if (!Fields.Contains("search")) { throw new Exception("Search not available"); }
				if (StartsWith) { whr.Add($"search like @qs||'%'"); param[$"@qs"] = Search; }
				else {
					var qs = Search.ToLower().Replace("  ", " ").Split(" ");
					for (var i = 0; i < qs.Length; i++) {
						var j = qs[i];
						whr.Add($"search like '%'||@s{i}||'%'");
						param[$"@s{i}"] = j;
					}
					slt = $"similarity(search,@srhq){(string.IsNullOrEmpty(SearchSort) ? "" : "*" + SearchSort)} srsiml, {slt}";
					srt = $"srsiml desc" + (srt is null ? "" : "," + srt);
					param["@srhq"] = Search; advs = true;
				}
			}
			if (whr.Count > 0) where = $" WHERE {string.Join(" and ", whr)} ";

			var ret = new DBPagingResponse<T>() { Total = Total ? await Db.GetCount(Table, where, param) : 0, Page = Page };
			var qry = $"SELECT {slt} FROM {Table} {where} {(srt is null ? "" : $"ORDER By {srt} {(Desc ? "Desc" : "Asc")}")} LIMIT {Limit} OFFSET {(Page - 1) * Limit}";
			if (advs) qry = $"SELECT * FROM ({qry}) WHERE srsiml>0";
			using var db = new DBRead(qry, param, Db);
			using var rdr = await db.GetReader();

			var props = rdr.GetProps<T>();

			while (await rdr.ReadAsync()) {
				var itm = await rdr.GetObject<T>(props);
				//TODO: Sleep;
				if (itm is not null) ret.Data.Add(itm);
			}
			if (!Total) { ret.Total = ret.Data.Count; }
			return ret;
		}
	}


	/// <summary>Plėtiniai</summary>
	public static class Extensions {
		/// <summary>Gauti tekstinę reikšmę</summary>
		/// <param name="rdr"></param><param name="id"></param><returns></returns>
		public static string? GetStringN(this NpgsqlDataReader rdr, int id) => !rdr.IsDBNull(id) ? rdr.GetString(id) : null;
		/// <summary>Gauti skaitinę reikšmę</summary>
		/// <param name="rdr"></param><param name="id"></param><returns></returns>
		public static int? GetIntN(this NpgsqlDataReader rdr, int id) => !rdr.IsDBNull(id) ? rdr.GetInt32(id) : null;
		/// <summary>Gauti skaitinę reikšmę</summary>
		/// <param name="rdr"></param><param name="id"></param><returns></returns>
		public static long? GetLongN(this NpgsqlDataReader rdr, int id) => !rdr.IsDBNull(id) ? rdr.GetInt64(id) : null;
		/// <summary>Gauti datos reikšmę</summary>
		/// <param name="rdr"></param><param name="id"></param><returns></returns>
		public static DateOnly? GetDateOnlyN(this NpgsqlDataReader rdr, int id) => !rdr.IsDBNull(id) ? DateOnly.FromDateTime(rdr.GetDateTime(id)) : null;
		/// <summary>Gauti visas įrašo reikšmes kaip objektų masyvą</summary>
		/// <param name="rdr"></param><returns></returns>
		public static object[] GetRow(this NpgsqlDataReader rdr) {
			var cnt = rdr.FieldCount;
			var row = new object[cnt];
			for (int i = 0; i < cnt; i++) row[i] = rdr.GetValue(i);
			return row;
		}
		/// <summary>Gauti objekto klasės parametrų informaciją</summary>
		/// <typeparam name="T">Objekto klasė</typeparam>
		/// <param name="rdr">SQL duomenų skaitytuvas</param>
		/// <returns>Parametrų sąrašas</returns>
		public static PropertyInfo?[] GetProps<T>(this NpgsqlDataReader rdr) {
			var cnt = rdr.FieldCount;
			var props = new PropertyInfo?[cnt];
			var ptp = typeof(T);
			for (var i = 0; i < cnt; i++) props[i] = ptp.GetProperty(rdr.GetName(i));
			return props;
		}
		/// <summary>Gauti suformuotą objektą iš duomenų įrašo</summary>
		/// <typeparam name="T">Klasė</typeparam>
		/// <param name="rdr">Duomenų bazės skaitytuvas</param>
		/// <param name="props">Duomenų parametrai</param>
		/// <returns>Objektas</returns>
		public static async Task<T?> GetObject<T>(this NpgsqlDataReader rdr, PropertyInfo?[]? props=null) where T : new() {
			var t = new T();
			if (!rdr.IsOnRow) if (!await rdr.ReadAsync()) return default;

			props ??= rdr.GetProps<T>();
			for (int i = 0; i < props.Length; i++) {
				var pi = props[i];
				if (pi != null && !await rdr.IsDBNullAsync(i)) {
					var pt = pi.PropertyType;
					switch (pt.Name) {
						case "Int32": pi.SetValue(t, rdr.GetInt32(i)); break;
						case "Int64": pi.SetValue(t, rdr.GetInt64(i)); break;
						case "DateTime": pi.SetValue(t, rdr.GetDateTime(i)); break;
						case "DateOnly": pi.SetValue(t, DateOnly.FromDateTime(rdr.GetDateTime(i))); break;
						case "String": pi.SetValue(t, rdr.GetString(i)); break;
						default:
							if (pt.IsClass && rdr.GetDataTypeName(i).Equals("jsonb", StringComparison.OrdinalIgnoreCase))
								pi.SetValue(t, JsonSerializer.Deserialize(rdr.GetString(i), pt));
							else pi.SetValue(t, rdr.GetValue(i));
							break;
					}
				}
			}
			return t;
		}
	}

	/// <summary>Duomenų puslapiavimo užklausa</summary>
	/// <typeparam name="T"></typeparam>
	public class DBPagingResponse<T> {
		/// <summary>Grąžinamų duomenų kiekis</summary>
		/// <example>10</example>
		public int Items => Data.Count;
		/// <summary>Bendras duomenų kiekis</summary>
		/// <example>100</example>
		public int Total { get; set; }
		/// <summary>Puslapis</summary>
		/// <example>1</example>
		public int Page { get; set; }
		/// <summary>Duomenys</summary>
		public List<T> Data { get; set; } = [];
	}

}
