namespace Registrai.Modules.AR.Models;

/// <summary>Adresų registro įrašas</summary>
public class AR_Item {
	/// <summary>Adresų registro kodas</summary>
	public int? ID { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Vietovės pavadinimas</summary>
	public string? Vietove { get; set; }
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
	/// <summary>Adreso registracijos data</summary>
	public DateOnly? RegData { get; set; }
	/// <summary>Adreso LKS koordinatės</summary>
	/// <example>[0,0]</example>
	public int[]? LKS { get; set; }
	/// <summary>Adreso WGS koordinatės</summary>
	/// <example>[0,0]</example>
	public double[]? WGS { get; set; }

	/// <summary>Apskrities detalės</summary>
	public AR_ItemDetails? Adm { get; set; }
	/// <summary>Savivaldybės detalės</summary>
	public AR_ItemDetails? Sav { get; set; }
	/// <summary>Seniunijos detalės</summary>
	public AR_ItemDetails? Sen { get; set; }
	/// <summary>Gyvenvietės detalės</summary>
	public AR_ItemDetails? Gyv { get; set; }
	/// <summary>Gatvės detalės</summary>
	public AR_ItemDetails? Gat { get; set; }
	/// <summary>Adreso (AOB) detalės</summary>
	public AR_ItemDetails? Aob { get; set; }

	/// <summary>Adreso informaciniai kodai</summary>
	public AR_ItemCodes? Kodai { get; set; }
}

/// <summary>Adreso informaciniai kodai</summary>
public class AR_ItemCodes {
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
	/// <summary>Patalpos kodas</summary>
	public int? Pat { get; set; }
}

/// <summary>Adreso įrašo trumpoji informacija</summary>
public class AR_ItemDetails {
	/// <summary>Adresų registro kodas</summary>
	public int? ID { get; set; }
	/// <summary>Įrašo vardas (kilmininko laipsniu)</summary>
	public string? Vardas { get; set; }
	/// <summary>Įrašo pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Įrašo tipas</summary>
	public string? Tipas { get; set; }
	/// <summary>Įrašo trumpinys</summary>
	public string? Trump { get; set; }
	/// <summary>Žemesnio lygio įrašų kiekis (Chlid count)</summary>
	/// <example>0</example>
	public int? Chc { get; set; }
	/// <summary>Dviem lygiais žemesnio įrašų kiekis be žemesnio lygio reikšmės</summary>
	/// <example>null</example>
	public int? Chm { get; set; }
}


