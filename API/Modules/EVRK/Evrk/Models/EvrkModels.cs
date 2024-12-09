using App;

namespace Registrai.Modules.EVRK.Models;

/// <summary>Evrk sąrašo rezultatas</summary>
public class Evrk_List : DBPagingResponse<Evrk_Item> { }

/// <summary>EVRK įrašo informacijos modelis</summary>
public class Evrk_Item {
	/// <summary>Identifikacinis numeris</summary>
	public string? ID { get; set; }
	/// <summary>Pirminė kodo sekcija</summary>
	public string? Sekcija { get; set; }
	/// <summary>Kodas</summary>
	public string? Kodas { get; set; }
	/// <summary>Pilnas pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Aukštesnio lygio kodas</summary>
	public string? Parent { get; set; }
	/// <summary>Lygmens numeris</summary>
	public int? Layer { get; set; }
	/// <summary>Paskutinis kodas, gali</summary>
	public bool? Last { get; set; }
}
