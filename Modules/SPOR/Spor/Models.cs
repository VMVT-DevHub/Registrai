using App;

namespace Registrai.Modules.SPOR;

/// <summary>SPOR lokacijų sąrašo rezultatas</summary>
public class Locations_List : DBPagingResponse<Location_Item> { }

/// <summary>SPOR lokacijos įrašo informacijos modelis</summary>
public class Location_Item {
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
	public Location_ItemLang? En { get; set; }
	/// <summary>Adresas lietuvių kalba</summary>
	public Location_ItemLang? Lt { get; set; }
	/// <summary>Neaktyvi lokacija</summary>
	public bool? Inactive { get; set; }
}

/// <summary>SPOR Lokacijos adreso informacija</summary>
public class Location_ItemLang {
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
public class Organisation_List : DBPagingResponse<Organisation_Item> { }

/// <summary>SPOR organizacijos įrašo informacijos modelis</summary>
public class Organisation_Item {
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


/// <summary>SPOR šaltinių sąrašas</summary>
public class References_ListsList : DBPagingResponse<References_ListItem> { }
/// <summary>SPOR sąrašo įrašas</summary>
public class References_ListItem {
	/// <summary>Identifikacinis numeris</summary>
	public long? ID { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Name { get; set; }
	/// <summary>Trumpas pavadinimas</summary>
	public string? Short { get; set; }
	/// <summary>Aprašymas</summary>
	public string? Description { get; set; }
	/// <summary>Domenas</summary>
	public string? Domain { get; set; }
	/// <summary>Terminų skaičius</summary>
	public int? TermCount { get; set; }
	/// <summary>Terminai</summary>
	public List<References_TermItem>? Terms { get; set; }
}

/// <summary>Terminų sąrašas</summary>
public class References_TermList : DBPagingResponse<References_TermItem> { }
/// <summary>Termino informacija</summary>
public class References_TermItem {
	/// <summary>Identifikacinis numeris</summary>
	public long? ID { get; set; }
	/// <summary>Termino sąrašo identifikatorius</summary>
	public long? ListID { get; set; }
	/// <summary>Statusas</summary>
	public string? Status { get; set; }
	/// <summary>Termino informacija angliškai</summary>
	public References_TermLang? En { get; set; }
	/// <summary>Termino informacija lietuviškai</summary>
	public References_TermLang? Lt { get; set; }
	/// <summary>Simbolis</summary>
	public string? Symbol { get; set; }
	/// <summary>Aukštesnio įrašo identifikatorius</summary>
	public long? ParentID { get; set; }
	/// <summary>Žemesnio įrašo skaičius</summary>
	public int? Children { get; set; }
}

/// <summary>Termino informacjos vertimas</summary>
public class References_TermLang {
	/// <summary>Pavadinimas</summary>
	public string? Name { get; set; }
	/// <summary>Aprašymas</summary>
	public string? Descr { get; set; }
	/// <summary>Trumpas aprašymas</summary>
	public string? Short { get; set; }
	/// <summary>Kita informacija</summary>
	public List<string>? Other { get; set; }
}