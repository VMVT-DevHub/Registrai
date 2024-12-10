namespace Registrai.Modules.JAR.Models;

/// <summary>Juridinio asmens registro įrašas</summary>
public class JAR_Item {
	/// <summary>Juridinio asmens registro kodas</summary>
	public int? ID { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Adresas</summary>
	public string? Adresas { get; set; }
	/// <summary>Adresų registro kodas (AOB)</summary>
	public int? AobKodas { get; set; }
	/// <summary>Juridinio asmens forma</summary>
	public string? Forma { get; set; }
	/// <summary>Juridinio asmens formos kodas</summary>
	public int? FormKodas { get; set; }
	/// <summary>Juridinio asmens statusas</summary>
	public string? Statusas { get; set; }
	/// <summary>Juridinio asmens statuso kodas</summary>
	public int? StatusKodas { get; set; }

	/// <summary>Statuso keitimo data</summary>
	public DateOnly? StatData { get; set; }
	/// <summary>Registracijos data</summary>
	public DateOnly? RegData { get; set; }
	/// <summary>Adreso keitimo data</summary>
	public DateOnly? AobData { get; set; }
	/// <summary>Išregistravimo data</summary>
	public DateOnly? IsregData { get; set; }
	/// <summary>Atnaujinimo data</summary>
	public DateOnly? ModData { get; set; }

}
