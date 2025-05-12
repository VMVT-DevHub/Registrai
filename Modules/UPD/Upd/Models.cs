using App;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Registrai.Modules.UPD;

/// <summary>Trumpas vaistų sąrašo įrašas</summary>
public class MedListItem {
	public long Id { get; set; }
	public string? Code { get; set; }
	public DateOnly? Date { get; set; }
	public string? Name { get; set; }
	public string? Legal { get; set; }
	public long? LegalCode { get; set; }
	public string? Holder { get; set; }
	public string? Status { get; set; }
	public List<string>? Ingredients { get; set; }
	public List<string>? Species { get; set; }
}


public static class RefCounter {
	public static ConcurrentDictionary<long, long> Counts { get; set; } = new();
	public static DateTime Load { get; set; } = DateTime.UtcNow.AddSeconds(30);
	public static void Add(long id) {
		var now = DateTime.UtcNow;
		lock (Counts) {
			if (Counts.TryGetValue(id, out var cnt)) Counts[id] = cnt + 1;
			else Counts[id] = 1;
			if (Load < now) {
				foreach (var i in Counts)
					DB.VVR.Execute($"INSERT INTO upd.log_translate (log_code, log_count) VALUES ({i.Key}, {i.Value});");
				Counts = new();
			}
		}
	}
}


/// <summary>SPOR Reference</summary>
public class Ref {
	/// <summary>Klasifikatoriaus kodas</summary>
	public long Code { get; set; }
	/// <summary>Klasifikatoriaus pavadinimas</summary>
	public string? Type { get; set; }
	private bool VarLang { get; set; }
	/// <summary>Ar reikalingas LT vertimas</summary>
	public bool? Lang { get => null; set { if (value == true) { Task.Run(() => { Task.Delay(100); RefCounter.Add(Code); }); } } }
}

/// <summary>Įrašo reikšmė</summary>
public class Value : Ref {
	/// <summary>Pavadinimas</summary>
	public string? Name { get; set; }
	/// <summary>Skaitinė reikšmė</summary>
	public float? Num { get; set; }
	/// <summary>Data</summary>
	public DateTime? Date { get; set; }
	/// <summary>Nuoroda į susijusį įrašą</summary>
	public long? Ref { get; set; }
}

/// <summary>Organizacijos detalės</summary>
public class Location {
	/// <summary>Identifikatorius</summary>
	public long? Id { get; set; }
	/// <summary>Lokacijos tipas</summary>
	Ref? Type { get; set; }
	/// <summary>Lokacijos identifikatorius</summary>
	public string? Loc { get; set; }
	/// <summary>Organizacijos identifikatorius</summary>
	public string? Org { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Name { get; set; }
	/// <summary>Šalis</summary>
	public string? Country { get; set; }
	/// <summary>Miestas</summary>
	public string? City { get; set; }
	/// <summary>Adresas</summary>
	public string? Address { get; set; }
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
	public List<Value>? Names { get; set; }
	/// <summary>Vaisto statusas</summary>
	public Ref? Status { get; set; }
	/// <summary>Tipas</summary>
	public Ref? Type { get; set; }
	public Value? Case { get; set; }
	/// <summary>Teisinis statusas</summary>
	public Ref? Legal { get; set; }
	/// <summary>Teisinis pagrindas</summary>
	public Ref? Basis { get; set; }
	/// <summary>Registruotojas</summary>
	public Location? Holder { get; set; }
	/// <summary>CMS</summary>
	public List<Ref>? ReglCase { get; set; }
	/// <summary>Pakuotės</summary>
	public List<Pack>? Packs { get; set; }
	/// <summary>Gamintojai</summary>
	public List<Location>? MfctOps { get; set; }
	/// <summary>Nuorodos</summary>
	public List<Ref>? Reference { get; set; }
	/// <summary>Klasifikavimas</summary>
	public List<Value>? Classif { get; set; }
	/// <summary>Plėtinys</summary>
	public Ref? Extension { get; set; }
	/// <summary>Vartojamas produktas</summary>
	public List<AdministrableProduct>? AdmProd { get; set; }
	/// <summary>Gaminio dalys</summary>
	public List<MfctItem>? MfctItem { get; set; }
	/// <summary>Sudėtis</summary>
	public List<Ingredient>? Ingredients { get; set; }
	/// <summary>Prisegti dokumentai</summary>
	public List<Document>? Documents { get; set; }
}


/// <summary>Vaisto pakuotės informacija</summary>
public class Pack {
	/// <summary>Pakuotės identifikacinis numeris</summary>
	public string? Id { get; set; }
	/// <summary>Pakuotės pavadinimas</summary>
	public string? Name { get; set; }
	/// <summary>Kalba</summary>
	public Ref? Language { get; set; }
	/// <summary>Teisinis statusas</summary>
	public Ref? Legal { get; set; }
	/// <summary>Rinkodara</summary>
	public Value? Marketing { get; set; }
	/// <summary>Kiekis</summary>
	public Value? Quantity { get; set; }
	/// <summary>Sudedamosios dalys</summary>
	public List<Value>? Items { get; set; }
	/// <summary>Pakavimas</summary>
	public Value? Packaging { get; set; }
	/// <summary>Pakuotės medžiagos</summary>
	public List<Value>? Material { get; set; }
}

/// <summary>Vartojamas produktas</summary>
public class AdministrableProduct {
	/// <summary>Sudedamosios dalys</summary>
	public List<long>? Ingredients { get; set; }
	/// <summary>Naudojimo būdas</summary>
	public List<AdmRoute>? Routes { get; set; }
}

/// <summary>Naudojimo būdai</summary>
public class AdmRoute : Ref {
	/// <summary>Gyvūnų rūšys</summary>
	public List<AdmSpecies>? Species { get; set; }
}


/// <summary>Gyvūnų rūšys</summary>
public class AdmSpecies : Ref {
	/// <summary>Išlaukos periodas</summary>
	public List<AdmWithdrawalPeriod>? WithdrawalPeriod { get; set; }
}


/// <summary>Išlaukos periodas</summary>
public class AdmWithdrawalPeriod : Ref {
	/// <summary>Aprašymas</summary>
	public string? Descr { get; set; }
	/// <summary>Skaitinė vertė</summary>
	public float? Num { get; set; }
	/// <summary>Audinys</summary>
	public Ref? Tissue { get; set; }
}


/// <summary>Gaminys</summary>
public class MfctItem {
	/// <summary>Identifikatorius</summary>
	public long? Id { get; set; }
	/// <summary>Sudedamosios dalys</summary>
	public List<long>? Ingredients { get; set; }
	/// <summary>Dozė</summary>
	public Ref? Dose { get; set; }
	/// <summary>Prezentacija</summary>
	public Ref? Presentation { get; set; }
}

/// <summary>Sudedamoji dalis</summary>
public class Ingredient : Ref {
	/// <summary>Gamintojas</summary>
	public List<long>? MfctOps { get; set; }
	/// <summary>Prezentacija</summary>
	public bool? Presentation { get; set; }
	/// <summary>Koncentracija</summary>
	public bool? Concentration { get; set; }
	/// <summary>ID</summary>
	public long? Id { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Name { get; set; }
	/// <summary>Sudėtinė medžiaga</summary>
	public Substance? Substance { get; set; }
	/// <summary>Nuoroda</summary>
	public Substance? Ref { get; set; }
}

/// <summary>Substancija</summary>
public class Substance : Ref {
	/// <summary>Pagrindinis pavadinimas</summary>
	public string? Name { get; set; }
	/// <summary>Pavadinimai</summary>
	public List<string>? Names { get; set; }
	/// <summary></summary>
	public string? Domain { get; set; }
	/// <summary></summary>
	public Value? Numerator { get; set; }
	/// <summary></summary>
	public Value? Denominator { get; set; }
}

/// <summary>Dokumentas</summary>
public class Document {
	/// <summary>Dokumento ID</summary>
	public Guid? Id { get; set; }
	/// <summary>Data</summary>
	public DateTime? Date { get; set; }
	/// <summary>Kalba</summary>
	public string? Lang { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Name { get; set; }
	/// <summary>Failo turinio tipas</summary>
	public string? Content { get; set; }
	/// <summary>Kategorija</summary>
	public Ref? Category { get; set; }
	/// <summary>Tipas</summary>
	public Ref? Type { get; set; }
}
