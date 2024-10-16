using App;
using System.Text.Json.Serialization;

namespace Modules.JAR.Models;


/// <summary>Juridinių asmenų paieškos rezultatas</summary>
public class JAR_Search : List<JAR_SearchItem> { }

/// <summary>Juridinių asmenų paieškos įrašo informacijos modelis</summary>
public class JAR_SearchItem {
	/// <summary>Juridinio asmens registro kodas</summary>
	public int? ID { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Adresas</summary>
	public string? Adresas { get; set; }
	/// <summary>Juridinio asmens statusas</summary>
	public string? Statusas { get; set; }
	/// <summary>Juridinio asmens forma</summary>
	public string? Forma { get; set; }
	/// <summary>Adresų registro kodas (AOB)</summary>
	[JsonIgnore] public int? AobKodas { get; set; }
	/// <summary>Juridinio asmens formos kodas</summary>
	[JsonIgnore] public int? FormKodas { get; set; }
	/// <summary>Juridinio asmens statuso kodas</summary>
	[JsonIgnore] public int? StatusKodas { get; set; }
}


/// <summary>Juridinių asmenų paieškos filtro užklausa</summary>
public class JAR_SearchFilter {
	/// <summary>Adresų registro kodas (AOB)</summary>
	/// <example>null</example>
	public int? AobKodas { get; set; }
	/// <summary>Juridinio asmens formos kodas</summary>
	/// <example>null</example>
	public int? FormKodas { get; set; }
	/// <summary>Juridinio asmens statuso kodas</summary>
	/// <example>null</example>
	public int? StatusKodas { get; set; }
}

/// <summary>Juridinių asmenų paieškos užklausa</summary>
public class JAR_SearchQuery {
    /// <summary>Paieškos frazė</summary>
    /// <example></example>
    public string? Search { get; set; }
	/// <summary>Rodyti daugiau informacijos</summary>
	/// <example>false</example>
	public bool? Detales { get; set; } = false;
    /// <summary>Gaunamų įrašų skaičius (max 50)</summary>
    /// <example>10</example>
    public int? Top { get; set; } = 10;

	/// <summary>Paieškos filtras</summary>
	public JAR_SearchFilter? Filter { get; set; }

}

