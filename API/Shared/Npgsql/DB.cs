using Npgsql;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace API {
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

		/// <summary></summary>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		public DBRead(string sql, Dictionary<string, object?>? param = null) {
			Conn = new NpgsqlConnection(DB.ConnStr);
			Cmd = new NpgsqlCommand(sql, Conn);
			if (param?.Count > 0) foreach (var p in param) Cmd.Parameters.Add(new(p.Key, p.Value));
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

	//TODO: separate non-static connections for modules
	/// <summary>Duomenų bazės pagalbininkas</summary>
	public static class DB {
		/// <summary>Prisijungimo tekstas</summary>
		public static string ConnStr { get; set; } = "User ID=postgres; Password=postgres; Server=localhost:5432; Database=master;";
		/// <summary>Vykdyti užklausą</summary>
		/// <param name="sql">Užklausa</param>
		/// <returns>Paveiktų įrašų skaičius</returns>
		public static async Task<int> Execute(string sql) {
			var conn = new NpgsqlConnection(ConnStr); await conn.OpenAsync();
			return await new NpgsqlCommand(sql, conn).ExecuteNonQueryAsync();
		}


		//TODO: Clear COUNTS!!!
		private static readonly Dictionary<string, int> Counts = [];

		/// <summary>Užklausos įrašų skaičiaus gavimas</summary>
		/// <param name="table">Lentelė</param>
		/// <param name="where">WHERE sąlyga</param>
		/// <param name="param">Užklausos parametrai</param>
		/// <returns>Įrašų skaičius</returns>
		public static async Task<int> GetCount(string table, string? where, Dictionary<string, object?>? param = null) {
			var qry = $"{table}{where}";
			if (param?.Count > 0) foreach (var i in param) qry += i.Value?.ToString();
			if (Counts.TryGetValue(qry, out var cnt)) return cnt;
			using var db = new DBRead($"SELECT count(*) FROM {table}{where};", param);
			using var rdr = await db.GetReader();
			if (await rdr.ReadAsync()) return Counts[qry] = rdr.GetInt32(0);
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
	/// <param name="table">Lentelės pavadinimas</param>
	public class DBPagingRequest<T>(string table) where T : new() {
		/// <summary>Lentelės pavadinimas</summary>
		public string Table { get; set; } = table;
		/// <summary>Užklausos ribojimas</summary>
		public T? Where { get; set; }
		/// <summary>Paieškos laukas</summary>
		public string? Search { get; set; }
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

			string where = ""; var param = new Dictionary<string, object?>();
			var whr = new List<string>();
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
				var sr = Search.Split(" ");
				for (var i = 0; i < sr.Length; i++) {
					whr.Add($"search like '%'||@q{i}||'%'");
					param[$"@q{i}"] = sr[i];
				}
			}
			if (whr.Count > 0) where = $" WHERE {string.Join(" and ", whr)} ";

			var ret = new DBPagingResponse<T>() {
				Total = Total ? await DB.GetCount(Table, where, param) : 0, Page = Page
			};
			using var db = new DBRead($"SELECT \"{string.Join("\",\"", Select)}\" FROM {Table} {where} {(Sort is null ? "" : $"ORDER By \"{Sort}\" {(Desc ? "Desc" : "Asc")}")} LIMIT {Limit} OFFSET {(Page - 1) * Limit}", param);
			using var rdr = await db.GetReader();


			var cnt = rdr.FieldCount;
			var props = new PropertyInfo?[cnt];
			for (var i = 0; i < cnt; i++) props[i] = typeof(T).GetProperty(rdr.GetName(i));

			while (await rdr.ReadAsync()) {
				var t = new T();
				for (int i = 0; i < cnt; i++) {
					var pi = props[i];
					if (pi != null && !await rdr.IsDBNullAsync(i)) {
						switch (pi.PropertyType.Name) {
							case "Int32": pi.SetValue(t, rdr.GetInt32(i)); break;
							case "Int64": pi.SetValue(t, rdr.GetInt64(i)); break;
							case "DateTime": pi.SetValue(t, rdr.GetDateTime(i)); break;
							case "DateOnly": pi.SetValue(t, DateOnly.FromDateTime(rdr.GetDateTime(i))); break;
							case "String": pi.SetValue(t, rdr.GetString(i)); break;
							default: pi.SetValue(t, rdr.GetValue(i)); break;
						}
					}
				}
				//TODO: Sleep;
				ret.Data.Add(t);
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
