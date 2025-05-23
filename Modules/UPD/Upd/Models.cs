using App;
using App.Routing;
using System.Collections.Concurrent;

namespace Registrai.Modules.UPD;

/// <summary>Trumpas vaistų sąrašo įrašas</summary>
public class MedListItem {
	/// <summary>Vaisto Id</summary>
	public long Id { get; set; }
	/// <summary>Vaisto kodas</summary>
	public string? Code { get; set; }
	/// <summary>Autorizacijos data</summary>
	public DateOnly? Date { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Name { get; set; }
	/// <summary>Teisinis statusas</summary>
	public string? Legal { get; set; }
	/// <summary>Teisinio statuso kodas</summary>
	public long? LegalCode { get; set; }
	/// <summary>Vaisto laikytojas</summary>
	public string? Holder { get; set; }
	/// <summary>Vaisto statusas</summary>
	public string? Status { get; set; }
	/// <summary>Sudedamosios medžiagos</summary>
	public List<string>? Ingredients { get; set; }
	/// <summary>Gyvūnų rūšys</summary>
	public List<string>? Species { get; set; }
}


/// <summary>Medicininio vaisto informacija</summary>
public class Medicine {
	/// <summary>Identifikacinis numeris</summary>
	public long Id { get; set; }
	/// <summary>Vaisto kodas</summary>
	public string? Code { get; set; }
	/// <summary>Registracijos data</summary>
	public DateOnly? Date { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Name { get; set; }
	/// <summary>Pavadinimai</summary>
	public List<object>? Names { get; set; }
	/// <summary>Vaisto statusas</summary>
	public object? Status { get; set; }
	/// <summary>Tipas</summary>
	public object? Type { get; set; }
	/// <summary></summary>
	public object? ReglCase { get; set; }
	/// <summary>Teisinis statusas</summary>
	public object? Legal { get; set; }
	/// <summary>Teisinis pagrindas</summary>
	public object? Basis { get; set; }
	/// <summary>Registruotojas</summary>
	public object? Holder { get; set; }
	/// <summary>Pakuotės</summary>
	public List<object>? Packs { get; set; }
	/// <summary>Gamintojai</summary>
	public List<object>? MfctOps { get; set; }
	/// <summary>Nuorodos</summary>
	public List<object>? Reference { get; set; }
	/// <summary>Klasifikavimas</summary>
	public List<object>? Classif { get; set; }
	/// <summary>Plėtinys</summary>
	public object? Extension { get; set; }
	/// <summary>Vartojamas produktas</summary>
	public List<object>? AdmProd { get; set; }
	/// <summary>Gaminio dalys</summary>
	public List<object>? MfctItem { get; set; }
	/// <summary>Sudėtis</summary>
	public List<object>? Ingredients { get; set; }
	/// <summary>Indikacijos</summary>
	public object? Directions { get; set; }
	/// <summary>Prisegti dokumentai</summary>
	public List<object>? Documents { get; set; }
}

/// <summary>Vaistų sąrašo filtro užklausa</summary>
public class MedQuery {
	/// <summary>Puslapio numeris</summary>
	/// <example>1</example>
	public int? Page { get; set; }
	/// <summary>Įrašų skaičius puslapyje</summary>
	/// <example>25</example>
	public int? Limit { get; set; }
	/// <summary>Rikiuoti pagal</summary>
	/// <example></example>
	public string? Order { get; set; }
	/// <summary>Rikiuoti mažėjančia tvarka</summary>
	/// <example>true</example>
	public bool Desc { get; set; }
	/// <summary>Paieškos tekstas</summary>
	/// <example></example>
	public string? Search { get; set; }
	
	/// <summary>Gyvūno rūšis</summary>
	public List<long>? Species { get; set; }
	/// <summary>Vaisto grupė</summary>
	public List<long>? LegalCode { get; set; }
	/// <summary>Farmacinė forma</summary>
	public List<long>? DoseForm { get; set; }
}

/// <summary>Galimi filtrai</summary>
public class MedFilters {
	/// <summary>Gyvūno rūšis</summary>
	public Dictionary<long, string>? Species { get; set; }
	/// <summary>Vaisto grupė</summary>
	public Dictionary<long, string>? LegalCode { get; set; }
	/// <summary>Farmacinė forma</summary>
	public Dictionary<long, string>? DoseForm { get; set; }
}
