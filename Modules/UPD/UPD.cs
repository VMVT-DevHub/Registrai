using App.Routing;
using Registrai.Modules.UPD;
using System.Diagnostics.Metrics;

namespace Registrai.Modules;

/// <summary>Registro inicijavimas</summary>
public class UpdRegistras {
	/// <summary>UPD Maršrutų priskyrimas</summary>
	/// <returns></returns>
	public static RouteDefinition Route() => new("EMA UPD") {
		Description = "<B>European Medicines Agency</B><BR>Union Product Database (UPD)<BR>Source: <a target=\"_blank\" href=\"https://vvr.test.vmvt.lt/swagger\">https://vvr.test.vmvt.lt/swagger</a>",
		Tag = "upd", Version = "v1",
		Routes = [
			new RouteGroup("Sąrašas")
				.Map(new("/upd/med", UpdMedicines.List){
					Name = "Pilnas vaistų sąrašas", Response=typeof(MedListItem),
					Params = [
						new("q") { Description = "Paieškos tekstas", Type=RouteParamType.String },
						new("page") { Description = "Puslapio numeris", Type=RouteParamType.Integer },
						new("limit") { Description = "Įrašų skaičius puslapyje", Type=RouteParamType.Integer },
						new("lang") { Description = "Kalba (lt/en)", Type=RouteParamType.String, Default="LT" },
					],
				})
				.Map(new("/upd/med",UpdMedicines.ListFilter, Method.Post){ 
					Name="Vaistų sąrašo filtravimas", Response=typeof(MedListItem),
					Params = [
						new("lang") { Description = "Kalba (lt/en)", Type=RouteParamType.String, Default="LT" },
					],
				})
				.Map(new("/upd/med/filters", UpdMedicines.Filters){
					Name = "Trūkstami vertimai", Response=typeof(MedFilters), Params = [
						new("lang") { Description = "Kalba (lt/en)", Type=RouteParamType.String, Default="LT" },
					]
				}),
			new RouteGroup("Vaisto informacija")
				.Map(new ("/upd/med/{id}", UpdMedicines.Item) {
					Name = "Detali vaisto informacija pagal ID", Response=typeof(Medicine),
					Params = [
						new("lang") { Description = "Kalba (lt/en)", Type=RouteParamType.String, Default="LT" },
					]
				})
				.Map(new("/upd/doc/{id}/{file}", UpdMedicines.DownloadFile){
					Name = "Pakuotės lapelio atsisiuntimas", Params = []
				})

		],
	};
}
