using App;
using System.Text.Json.Serialization;

namespace Modules.AR.Models;


/// <summary>Adresų paieškos rezultatas</summary>
public class AR_Search : List<AR_SearchItem> { }

/// <summary>Adresų paieškos įrašo informacijos modelis</summary>
public class AR_SearchItem {
	/// <summary>Adresų registro kodas</summary>
	public int? ID { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Vietovės pavadinimas</summary>
	public string? Vietove { get; set; }
	/// <summary>Įrašo tipas</summary>
	public string? Tipas { get; set; }

	/// <summary>Paieškos tipo filtras</summary>
	[JsonIgnore] public string? Src { get; set; }
	/// <summary>Apskrities kodas</summary>
	[JsonIgnore] public int? Adm { get; set; }
	/// <summary>Savivaldybės kodas</summary>
	[JsonIgnore] public int? Sav { get; set; }
	/// <summary>Seniunijos kodas</summary>
	[JsonIgnore] public int? Sen { get; set; }
	/// <summary>Gyvenvietės kodas</summary>
	[JsonIgnore] public int? Gyv { get; set; }
	/// <summary>Gatvės kodas</summary>
	[JsonIgnore] public int? Gat { get; set; }
	/// <summary>Adreso kodas</summary>
	[JsonIgnore] public int? Aob { get; set; }
}

/// <summary>Adresų paieškos filtro užklausa</summary>
public class AR_SearchFilter {
	/// <summary>Apskrities kodas</summary>
	/// <example>null</example>
	public int? Adm { get; set; }
	/// <summary>Savivaldybės kodas</summary>
	/// <example>null</example>
	public int? Sav { get; set; }
	/// <summary>Seniunijos kodas</summary>
	/// <example>null</example>
	public int? Sen { get; set; }
	/// <summary>Gyvenvietės kodas</summary>
	/// <example>null</example>
	public int? Gyv { get; set; }
	/// <summary>Gatvės kodas</summary>
	/// <example>null</example>
	public int? Gat { get; set; }
	/// <summary>Adreso kodas</summary>
	/// <example>null</example>
	public int? Aob { get; set; }
}

/// <summary>Adresų paieškos filtro tipas</summary>
public enum AR_SearchTypes {
	/// <summary>Apskritys</summary>
	Adm,
	/// <summary>Savivaldybės</summary>
	Sav,
	/// <summary>Seniunijos</summary>
	Sen,
	/// <summary>Gyvenvietės</summary>
	Gyv,
	/// <summary>Gatvės</summary>
	Gat,
	/// <summary>Adresai (AOB)</summary>
	Aob,
	/// <summary>Patalpos</summary>
	Pat,
	/// <summary>Adresai (GAT+AOB+PAT)</summary>
	Adr
}

/// <summary>Adresų paieškos užklausa</summary>
public class AR_SearchQuery {
	/// <summary>Paieškos frazė</summary>
	/// <example></example>
	public string? Search { get; set; }
	/// <summary>Gaunamų įrašų skaičius (max 50)</summary>
	/// <example>10</example>
	public int? Top { get; set; } = 10;
	/// <summary>Ieškoti pilname adreso pavadinime</summary>
	/// <example>false</example>
	public bool Full { get; set; }
	/// <summary>Paieškos tipas</summary>
	/// <example>Gyv</example>
	public AR_SearchTypes? Type { get; set; }
	/// <summary>Paieškos filtras</summary>
	public AR_SearchFilter? Filter { get; set; }
}

