using App;

namespace Registrai.Modules.Salys.Models;


/// <summary>Šalių paieškos rezultatas</summary>
public class Sal_List : DBPagingResponse<Sal_Item> { }

/// <summary>Šalių paieškos įrašo informacijos modelis</summary>
public class Sal_Item {
	/// <summary>Adresų registro kodas</summary>
	public string? Iso3 { get; set; }
	/// <summary>Atstumas nuo pateikto taško</summary>
	public string? Iso2 { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Pilnas pavadinimas</summary>
	public string? Pilnas { get; set; }
	/// <summary>Šalies sostinė</summary>
	public string? Sostine { get; set; }
	/// <summary>Angliškas pavadinimas</summary>
	public string? Eng { get; set; }
	/// <summary>Europos sąjungos šalis</summary>
	public bool? Eu { get; set; }
}
