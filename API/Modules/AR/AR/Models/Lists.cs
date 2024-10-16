using API;

namespace Modules.AR.Models;

/// <summary>Adresų sąrašo rezultatas</summary>
public class AR_List : DBPagingResponse<AR_ListItem> { }

/// <summary>Adresų sąrašo įrašo informacijos modelis</summary>
public class AR_ListItem {
	/// <summary>Adresų registro kodas</summary>
	public int? ID { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Vardas (kilmininko reikšme)</summary>
	public string? Vardas { get; set; }
	/// <summary>Įrašo tipas</summary>
	public string? Tipas { get; set; }
	/// <summary>Tipo santrumpa</summary>
	public string? Trump { get; set; }
	/// <summary>Adreso numeris (AOB)</summary>
	public string? Nr { get; set; }
	/// <summary>Adreso korpusas (AOB)</summary>
	public string? Korp { get; set; }
	/// <summary>Patalpos numeris (PAT)</summary>
	public string? Pat { get; set; }
	/// <summary>Pašto adresas (AOB)</summary>
	public string? Post { get; set; }
	/// <summary>Apskrities kodas</summary>
	public int? Adm { get; set; }
	/// <summary>Savivaldybės kodas</summary>
	public int? Sav { get; set; }
	/// <summary>Seniunijos kodas</summary>
	public int? Sen { get; set; }
	/// <summary>Gyvenvietės kodas</summary>
	public int? Gyv { get; set; }
	/// <summary>Gatvės kodas</summary>
	public int? Gat { get; set; }
	/// <summary>Adreso (AOB) kodas</summary>
	public int? Aob { get; set; }
	/// <summary>Žemesnio lygio įrašų kiekis</summary>
	public int? Chc { get; set; }
	/// <summary>Dviem lygiais žemesnio įrašų kiekis be žemesnio lygio reikšmės</summary>
	public int? Chm { get; set; }
}

/// <summary>Adresų registro filtro užklausa</summary>
public class AR_ListFilter {
	/// <summary>Apskrities kodas</summary>
	public int? Adm { get; set; }
	/// <summary>Savivaldybės kodas</summary>
	public int? Sav { get; set; }
	/// <summary>Seniunijos kodas</summary>
	public int? Sen { get; set; }
	/// <summary>Gyvenvietės kodas</summary>
	public int? Gyv { get; set; }
	/// <summary>Gatvės kodas</summary>
	public int? Gat { get; set; }
	/// <summary>Adreso kodas</summary>
	public int? Aob { get; set; }
}

/// <summary>Adresų registro filtro tipas</summary>
public enum AR_ListTypes {
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
	Pat
}

/// <summary>Adresų registro filtro užklausa</summary>
public class AR_ListQuery : DBPagingFilter<AR_ListFilter> {
	/// <summary>Filtro tipas</summary>
	public AR_ListTypes? Type { get; set; }
}

