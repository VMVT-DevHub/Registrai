using System.Text.Json.Serialization;
using System.Text.Json;

namespace Registrai.App {
	/// <summary>Plėtiniai</summary>
	public static class Extensions {

	}


	/// <summary>Datos formatavimas</summary>
	public class CustomDateTimeConverter : JsonConverter<DateTime> {
		/// <summary></summary><param name="reader"></param><param name="typeToConvert"></param><param name="options"></param><returns></returns>
		public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => DateTime.TryParse(reader.GetString(), out var dt) ? dt : default;
		/// <summary></summary><param name="writer"></param><param name="value"></param><param name="options"></param>
		public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
	}

}
