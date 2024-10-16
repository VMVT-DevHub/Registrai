using Registrai.App.Config;
using System.Text.Json;

namespace Registrai.App { 

	/// <summary>Sistemos konfigūracija</summary>
	public class Configuration : Config<ConfigData> { }
	namespace Config {
		/// <summary>Konfigūracijos duomenų modelis</summary>
		public class ConfigData {
			/// <summary>Duomenų bazės prisijungimo tekstas</summary>
			public string ConnString { get; set; } = "User ID=postgres; Password=postgres; Server=localhost:5432; Database=master;";
			/// <summary>Spausdinti klaidas konsolėje</summary>
			public bool PrintOutput { get; set; } = true;
			/// <summary>Saugoti klaidų įrašus faile</summary>
			public bool LogFile { get; set; } = false;
		}

		/// <summary>Konfigūracijos modelis</summary>
		/// <typeparam name="T"></typeparam>
		public class Config<T>() where T : new() {
			/// <summary>Reikalauti konfigūracinio failo</summary>
			public bool RequireFile { get; set; } = true;
			/// <summary>Configuration file</summary>
			public string JsonFile { get; set; } = "appsettings.json";
			/// <summary>Configuration secrets file (optional, not in GIT)</summary>
			public string JsonSecrets { get; set; } = "secrets.json";
			/// <summary>Configuration change check interval in seconds (0 - every time)</summary>
			public int Cache { get; set; } = 10;
			private DateTime LastModif { get; set; }
			private DateTime LastCheck { get; set; }
			private T? Cfg { get; set; }
			private DateTime Latest() { var f = File.GetLastWriteTime(JsonFile); var s = File.GetLastWriteTime(JsonSecrets); return f > s ? f : s; }
			/// <summary>Configuration data object</summary>
			public T Data {
				get {
					var rld = true;
					if (Cache > 0) { var dt = DateTime.UtcNow; if (LastCheck < dt) LastCheck = dt.AddSeconds(Cache); else rld = false; }
					if (rld && Latest() > LastModif) Reload(); return Cfg ??= Reload();
				}
			}

			/// <summary>Reload configuration file</summary>
			/// <returns>Configuration item</returns>
			/// <exception cref="Exception"></exception>
			public T Reload() {
				if (File.Exists(JsonFile)) {
					LastModif = Latest();
					var data = JsonSerializer.Deserialize<T>(File.ReadAllText(JsonFile));
					if (data is not null) {
						return Cfg = LoadSecrets(data);
					}
					if (RequireFile) throw new Exception("Unable to read appsettings.json file");
				}
				if (RequireFile) throw new Exception("Missing appsettings.json file");
				return Cfg = LoadSecrets(new());
			}
			private T LoadSecrets(T data) {
				if (File.Exists(JsonSecrets)) {
					var dflt = new T();
					var secr = JsonSerializer.Deserialize<T>(File.ReadAllText(JsonSecrets));
					if (secr is not null) foreach (var i in secr.GetType().GetProperties()) {
							var val = i.GetValue(secr);
							var df = i.GetValue(dflt);
							if (val is string v) { if (!string.IsNullOrEmpty(v) && val != df) i.SetValue(data, val); }
							else if (val is not null) {
								if (!i.PropertyType.IsPrimitive || val.ToString() != df?.ToString())
									i.SetValue(data, val);
							}
						}
				}
				return data;
			}
		}
	}
}