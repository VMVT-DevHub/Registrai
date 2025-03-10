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
				.Map(new("/spor/substances", SporSubstances.List){
					Name = "Medžiagos", Response=typeof(Substances_List),
					Params = [
						new("page") { Description = "Puslapio numeris", Type=RouteParamType.Integer },
						new("limit") { Description = "Įrašų skaičius puslapyje", Type=RouteParamType.Integer },
						new("domain") { Description = "Filtruoti pagal domeną", Type=RouteParamType.String },
						new("type") { Description = "Filtruoti pagal tipą", Type=RouteParamType.String },
						new("order") { Description = "Rikiuoti pagal", Type=RouteParamType.String },
						new("desc") { Description = "Rikiuoti mažėjančia tvarka" }],
				})
				.Map(new ("/spor/substances/{id}", SporSubstances.Item) {
					Name = "Medžiagos informacija pagal ID", Response=typeof(Substances_Item)
				})
				.Map(new("/spor/substances/find", SporSubstances.Find){
					Name = "Madžiagų paieška", Response=typeof(Substances_List),
					Params = [
						new("q") { Description = "Paieškos frazė", Type=RouteParamType.String },
						new("limit") { Description = "Įrašų skaičius puslapyje", Type=RouteParamType.Integer },
						new("domain") { Description = "Filtruoti pagal domeną", Type=RouteParamType.String },
						new("type") { Description = "Filtruoti pagal tipą", Type=RouteParamType.String },
						new("order") { Description = "Rikiuoti pagal", Type=RouteParamType.String },
						new("desc") { Description = "Rikiuoti mažėjančia tvarka" }],
				}),
			new RouteGroup("References")
				.Map(new("/spor/references/lists", SporReferences.List){
					Name = "Sąrašai", Response=typeof(References_ListsList),
					Params = [
						new("page") { Description = "Puslapio numeris", Type=RouteParamType.Integer },
						new("limit") { Description = "Įrašų skaičius puslapyje", Type=RouteParamType.Integer },
						new("order") { Description = "Rikiuoti pagal", Type=RouteParamType.String },
						new("desc") { Description = "Rikiuoti mažėjančia tvarka" }],
				})
				.Map(new ("/spor/references/list/{id}", SporReferences.ListItem) {
					Name = "Sąrašo informacija pagal ID", Response=typeof(References_ListItem),
					Params = [
						new("terms") { Description = "Rodyti sąrašo terminus" }
					]
				})
				.Map(new("/spor/references/terms", SporReferences.TermList){
					Name = "Sąrašai", Response=typeof(References_TermList),
					Params = [
						new("page") { Description = "Puslapio numeris", Type=RouteParamType.Integer },
						new("limit") { Description = "Įrašų skaičius puslapyje", Type=RouteParamType.Integer },
						new("list") { Description = "Filtruoti pagal sąrašą", Type=RouteParamType.Integer },
						new("parent") { Description = "Filtruoti pagal aukštesnio lygio terminą", Type=RouteParamType.Integer },
						new("inactive") { Description = "Rodyti neaktyvius terminus" },
						new("order") { Description = "Rikiuoti pagal", Type=RouteParamType.String },
						new("desc") { Description = "Rikiuoti mažėjančia tvarka" }],
				})
				.Map(new ("/spor/references/term/{id}", SporReferences.TermItem) {
					Name = "Termino informacija pagal ID", Response=typeof(References_TermItem)
				})
				.Map(new("/spor/references/term/find", SporReferences.TermFind){
					Name = "Organizacijų paieška", Response=typeof(References_TermList),
					Params = [
						new("q") { Description = "Paieškos frazė", Type=RouteParamType.String },
						new("limit") { Description = "Įrašų skaičius puslapyje", Type=RouteParamType.Integer },
						new("list") { Description = "Filtruoti pagal sąrašą", Type=RouteParamType.Integer },
						new("inactive") { Description = "Rodyti neaktyvius terminus" },
						new("order") { Description = "Rikiuoti pagal", Type=RouteParamType.String },
						new("desc") { Description = "Rikiuoti mažėjančia tvarka" }],
				}),
			new RouteGroup("Organisations")
				.Map(new("/spor/organisations", SporOrganisations.List){
					Name = "Organizacijų sąrašas", Response=typeof(Organisation_List),
					Params = [
						new("page") { Description = "Puslapio numeris", Type=RouteParamType.Integer },
						new("limit") { Description = "Įrašų skaičius puslapyje", Type=RouteParamType.Integer },
						new("country") { Description = "Filtruoti pagal šalį (ISO2)", Type=RouteParamType.String },
						new("inactive") { Description = "Rodyti neaktyvias lokacijas" },
						new("order") { Description = "Rikiuoti pagal", Type=RouteParamType.String },
						new("desc") { Description = "Rikiuoti mažėjančia tvarka" }],
				})
				.Map(new("/spor/organisations/find", SporOrganisations.Find){
					Name = "Organizacijų paieška", Response=typeof(Organisation_List),
					Params = [
						new("q") { Description = "Paieškos frazė", Type=RouteParamType.String },
						new("limit") { Description = "Įrašų skaičius puslapyje", Type=RouteParamType.Integer },
						new("country") { Description = "Filtruoti pagal šalį (ISO2)", Type=RouteParamType.String },
						new("inactive") { Description = "Rodyti neaktyvias lokacijas" },
						new("order") { Description = "Rikiuoti pagal", Type=RouteParamType.String },
						new("desc") { Description = "Rikiuoti mažėjančia tvarka" }],
				})
				.Map(new ("/spor/organisation/{id}", SporOrganisations.Item) {
					Name = "Gauti organizaciją pagal ID, organizacijos informacija", Response=typeof(Organisation_Item)
				}),
			new RouteGroup("Locations")
				.Map(new("/spor/locations", SporLocations.List){
					Name = "Lokacijų sąrašas", Response=typeof(Locations_List),
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
					Name = "Gauti lokaciją pagal ID, lokacijos informacija", Response=typeof(Location_Item)
				})
		],
	};
}
