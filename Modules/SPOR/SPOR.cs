using App.Routing;
using Registrai.Modules.SPOR;
using System.Diagnostics.Metrics;

namespace Registrai.Modules;

/// <summary>Registro inicijavimas</summary>
public class SporRegistras {
	/// <summary>SPOR Maršrutų priskyrimas</summary>
	/// <returns></returns>
	public static RouteDefinition Route() => new("EMA SPOR") {
		Description = "<B>European Medicines Agency</B><BR>Substances, products, organisations and referentials (SPOR)",
		Tag = "spor", Version = "v1",
		Routes = [
			new RouteGroup("Substances")
				.Map(new("/spor/list", Spor.List){
					Name = "Kodų sąrašas", Response=typeof(Evrk_List),
					Params = [new("l1") { Description = "Rodyti tik pirmą lygį (parent=\"\")" }, new("desc") { Description = "Rikiuoti mažėjančia tvarka" }],
				})
				.Map(new("/spor/details", Spor.Details){
					Name= "Įrašo informacija", Response = typeof(Evrk_Item),
					Params = [new("code") { Description = "Ieškoti pagal kodą" }]
				})
				.Map(new("/spor/details", Spor.DetailsMulti){
					Name= "Įrašo informacija", Response = typeof(List<Evrk_Item>), Method=Method.Post,
					Params = [new("code") { Description = "Ieškoti pagal kodą" }]
				})
				.Map(new("/spor/search", Spor.Search){
					Name= "Kodų paieška", Response = typeof(List<Evrk_Item>),
					Params = [new("code") { Description = "Ieškoti pagal kodą" }]
				}),
			new RouteGroup("Organisations")
				.Map(new("/spor/organisations", SporLocations.List){
					Name = "Organizacijų sąrašas", Response=typeof(SporLoc_List),
					Params = [
						new("page") { Description = "Puslapio numeris", Type=RouteParamType.Integer },
						new("limit") { Description = "Įrašų skaičius puslapyje", Type=RouteParamType.Integer },
						new("org") { Description = "Filtruoti pagal organizaciją", Type=RouteParamType.String },
						new("country") { Description = "Filtruoti pagal šalį", Type=RouteParamType.String },
						new("inactive") { Description = "Rodyti neaktyvias lokacijas" },
						new("order") { Description = "Rikiuoti pagal", Type=RouteParamType.String },
						new("desc") { Description = "Rikiuoti mažėjančia tvarka" }],
				})
				.Map(new ("/spor/organisation/{id}", SporLocations.Item) {
					Name = "Organizacijos informacija", Response=typeof(SporLoc_Item)
				}),
			new RouteGroup("Locations")
				.Map(new("/spor/locations", SporLocations.List){
					Name = "Lokacijų sąrašas", Response=typeof(SporLoc_List),
					Params = [
						new("page") { Description = "Puslapio numeris", Type=RouteParamType.Integer },
						new("limit") { Description = "Įrašų skaičius puslapyje", Type=RouteParamType.Integer },
						new("org") { Description = "Filtruoti pagal organizaciją", Type=RouteParamType.String },
						new("country") { Description = "Filtruoti pagal šalį", Type=RouteParamType.String },
						new("inactive") { Description = "Rodyti neaktyvias lokacijas" },
						new("order") { Description = "Rikiuoti pagal", Type=RouteParamType.String },
						new("desc") { Description = "Rikiuoti mažėjančia tvarka" }],
				})
				.Map(new ("/spor/location/{id}", SporLocations.Item) {
					Name = "Lokacijos informacija", Response=typeof(SporLoc_Item)
				})
		],
	};
}
