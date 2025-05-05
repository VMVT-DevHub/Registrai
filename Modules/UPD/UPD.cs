using App.Routing;
using Registrai.Modules.UPD;
using System.Diagnostics.Metrics;

namespace Registrai.Modules;

/// <summary>Registro inicijavimas</summary>
public class UpdRegistras {
	/// <summary>UPD Maršrutų priskyrimas</summary>
	/// <returns></returns>
	public static RouteDefinition Route() => new("EMA UPD") {
		Description = "<B>European Medicines Agency</B><BR>Union Product Database (UPD)",
		Tag = "upd", Version = "v1",
		Routes = [
			new RouteGroup("Medicine")
				.Map(new("/upd/med", UpdMedicines.List){
					Name = "Medžiagos", Response=typeof(MedListItem),
					Params = [
						new("page") { Description = "Puslapio numeris", Type=RouteParamType.Integer },
						new("limit") { Description = "Įrašų skaičius puslapyje", Type=RouteParamType.Integer },
						new("lang") { Description = "Kalba (lt/en)", Type=RouteParamType.String, Default="LT" },
						new("order") { Description = "Rikiuoti pagal", Type=RouteParamType.String },
						new("desc") { Description = "Rikiuoti mažėjančia tvarka" }],
				})
				.Map(new ("/upd/med/{id}", UpdMedicines.Item) {
					Name = "Medžiagos informacija pagal ID", Response=typeof(Medicine),
					Params = [
						new("lang") { Description = "Kalba (lt/en)", Type=RouteParamType.String, Default="LT" },
					]
				})
		],
	};
}
