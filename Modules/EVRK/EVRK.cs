using App.Routing;
using Registrai.Modules.EVRK;

namespace Registrai.Modules;

/// <summary>Registro inicijavimas</summary>
public class EvrkRegistras {
	/// <summary>EVRK Maršrutų priskyrimas</summary>
	/// <returns></returns>
	public static RouteDefinition Route() => new("EVRK") {
		Description = "Ekonominės veiklos rūšių klasifikatorius (v2.1)",
		Tag = "evrk", Version = "v1",
		Routes = [
			new RouteGroup("EVRK Kodai")
				.Map(new("/evrk/list", Evrk.List){
					Name = "Kodų sąrašas", Response=typeof(Evrk_List),
					Params = [new("l1") { Description = "Rodyti tik pirmą lygį (parent=\"\")" }, new("desc") { Description = "Rikiuoti mažėjančia tvarka" }],
				})
				.Map(new("/evrk/details", Evrk.Details){
					Name= "Įrašo informacija", Response = typeof(Evrk_Item),
					Params = [new("code") { Description = "Ieškoti pagal kodą" }]
				})
				.Map(new("/evrk/details", Evrk.DetailsMulti){
					Name= "Įrašo informacija", Response = typeof(List<Evrk_Item>), Method=Method.Post,
					Params = [new("code") { Description = "Ieškoti pagal kodą" }]
				})
				.Map(new("/evrk/search", Evrk.Search){
					Name= "Kodų paieška", Response = typeof(List<Evrk_Item>),
					Params = [new("code") { Description = "Ieškoti pagal kodą" }]
				})
			],
	};
}
