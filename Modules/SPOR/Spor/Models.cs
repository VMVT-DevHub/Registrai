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
	public string? OrgNameEn { get; set; }
	/// <summary>Organizacijos lietuviškas pavadinimas</summary>
	public string? OrgNameLt { get; set; }
	/// <summary>Adresas anglų kalba</summary>
	public SporLoc_ItemLang? En { get; set; }
	/// <summary>Adresas lietuvių kalba</summary>
	public SporLoc_ItemLang? Lt { get; set; }
	/// <summary>Neaktyvi lokacija</summary>
	public bool? Inactive { get; set; }
}

/// <summary>Adreso informacija</summary>
public class SporLoc_ItemLang {
	/// <summary>Šalis</summary>
	public string? Country { get; set; }
	/// <summary>Miestas</summary>
	public string? City { get; set; }
	/// <summary>Valstija</summary>
	public string? State { get; set; }
	/// <summary>Apskritis</summary>
	public string? County { get; set; }
	/// <summary>Adresas</summary>
	public string? Address { get; set; }
}


/// <summary>SPOR organizacijų sąrašo rezultatas</summary>
public class SporOrg_List : DBPagingResponse<SporOrg_Item> { }

/// <summary>SPOR organizacijos įrašo informacijos modelis</summary>
public class SporOrg_Item {
	/// <summary>Identifikacinis numeris</summary>
	public string? ID { get; set; }
	/// <summary>Organizacijos pavadinimas</summary>
	public string? NameEn { get; set; }
	/// <summary>Organizacijos lietuviškas pavadinimas</summary>
	public string? NameLt { get; set; }
	/// <summary>Šalies kodas (ISO2)</summary>
	public string? Country { get; set; }
	/// <summary>Lokacijų skaičius</summary>
	public int? Locations { get; set; }
	/// <summary>Neaktyvi organizacija</summary>
	public bool? Inactive { get; set; }
}




public class SporRef_ListsList : DBPagingResponse<SporRef_ListItem> { }
public class SporRef_ListItem {
	public long? ID { get; set; }
	public string? Name { get; set; }
	public string? Short { get; set; }
	public string? Description { get; set; }
	public string? Domain { get; set; }
	public int? Terms { get; set; }
}