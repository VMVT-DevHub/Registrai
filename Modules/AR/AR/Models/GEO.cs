namespace Registrai.Modules.AR.Models;


/// <summary>Adresų paieškos rezultatas</summary>
public class AR_GEO : List<AR_GEOItem> { }

/// <summary>Adresų paieškos įrašo informacijos modelis</summary>
public class AR_GEOItem {
	/// <summary>Adresų registro kodas</summary>
	public int? ID { get; set; }
	/// <summary>Atstumas nuo pateikto taško</summary>
	public double? Atstumas { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Vietovės pavadinimas</summary>
	public string? Vietove { get; set; }
	/// <summary>Įrašo tipas</summary>
	public string? Tipas { get; set; }
	/// <summary>Adreso numeris (AOB)</summary>
	public string? Nr { get; set; }
	/// <summary>Adreso korpusas (AOB)</summary>
	public string? Korp { get; set; }
	/// <summary>Pašto adresas (AOB)</summary>
	public string? Post { get; set; }
	/// <summary>Adreso registracijos data</summary>
	public DateOnly? RegData { get; set; }
	/// <summary>Adreso LKS koordinatės</summary>
	/// <example>[0,0]</example>
	public int[]? LKS { get; set; }
	/// <summary>Adreso WGS koordinatės</summary>
	/// <example>[0,0]</example>
	public double[]? WGS { get; set; }
	/// <summary>Adreso informaciniai kodai</summary>
	public AR_ItemCodes? Kodai { get; set; }
}

/// <summary>GEO gyvenvietės įrašas</summary>
public class AR_GEOGyvItem {
	/// <summary>Adresų registro kodas</summary>
	public int? ID { get; set; }
	/// <summary>Įrašo vardas (kilmininko laipsniu)</summary>
	public string? Vardas { get; set; }
	/// <summary>Įrašo pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Vietovės pavadinimas</summary>
	public string? Vietove { get; set; }
	/// <summary>Įrašo tipas</summary>
	public string? Tipas { get; set; }
	/// <summary>Tipo santrumpa</summary>
	public string? Trump { get; set; }

	/// <summary>Adreso registracijos data</summary>
	public DateOnly? RegData { get; set; }

	/// <summary>Apskrities detalės</summary>
	public AR_GEOItemDetails? Adm { get; set; }
	/// <summary>Savivaldybės detalės</summary>
	public AR_GEOItemDetails? Sav { get; set; }
	/// <summary>Seniunijos detalės</summary>
	public AR_GEOItemDetails? Sen { get; set; }
}

/// <summary>GEO Adreso įrašo trumpoji informacija</summary>
public class AR_GEOItemDetails {
	/// <summary>Adresų registro kodas</summary>
	public int? ID { get; set; }
	/// <summary>Įrašo vardas (kilmininko laipsniu)</summary>
	public string? Vardas { get; set; }
	/// <summary>Įrašo tipas</summary>
	public string? Tipas { get; set; }
	/// <summary>Įrašo trumpinys</summary>
	public string? Trump { get; set; }
}