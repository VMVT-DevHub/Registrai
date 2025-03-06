using App;

namespace Registrai.Modules.SPOR;

/// <summary>SPOR lokacijų sąrašo rezultatas</summary>
public class SporLoc_List : DBPagingResponse<SporLoc_Item> { }

/// <summary>SPOR lokacijos įrašo informacijos modelis</summary>
public class SporLoc_Item {
	/// <summary>Identifikacinis numeris</summary>
	public string? ID { get; set; }
	/// <summary>Šalies kodas (ISO2)</summary>
	public string? CountryCode { get; set; }
	/// <summary>Organizacijos ID</summary>
	public string? OrgID { get; set; }
	/// <summary>Organizacijos pavadinimas</summary>
	public string? OrgName { get; set; }
	/// <summary>Adresas anglų kalba</summary>
	public SporLoc_ItemLang? En { get; set; }
	/// <summary>Adresas lietuvių kalba</summary>
	public SporLoc_ItemLang? Lt { get; set; }
	/// <summary>Neaktyvi lokacija</summary>
	public bool? Inactive { get; set; }
}

public class SporLoc_ItemLang {
	public string? country { get; set; }
	public string? city { get; set; }
	public string? state { get; set; }
	public string? county { get; set; }
	public string? address { get; set; }
}

/// <summary>Evrk sąrašo rezultatas</summary>
public class Evrk_List : DBPagingResponse<Evrk_Item> { }

/// <summary>EVRK įrašo informacijos modelis</summary>
public class Evrk_Item
{
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
