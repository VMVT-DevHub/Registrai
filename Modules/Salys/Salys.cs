using App.Routing;
using Registrai.Modules.Salys;

namespace Registrai.Modules;

/// <summary>Registro inicijavimas</summary>
public class SaliuRegistras {
	/// <summary>Šalių registro maršrutų priskyrimas</summary>
	/// <returns></returns>
	public static RouteDefinition Route() => new("Šalys") {
		Description = "Pilnas šalių sąrašas su vėliavomis",
		Tag = "salys", Version = "v1",
		Routes = [
			new RouteGroup("Šalių API")
				.Map(new ("/salys/search", Sal.Search) {
					Name = "Šalies paieška", Response=typeof(List<Sal_Item>),
					Params = [new("eu") { Description = "Rodyti tik EU šalis" }]
				})
				.Map(new ("/salys/list", Sal.List) {
					Name = "Visų šalių informacija", Response=typeof(Sal_List),
					Params = [new("eu") { Description = "Rodyti tik EU šalis" }, new("desc") { Description = "Rikiuoti mažėjančia tvarka" }]
				})
				.Map(new ("/salys/{iso3}", Sal.Info) {
					Name = "Šalies informacija", Response=typeof(Sal_Item),
					Params = [new("flag") { Description = "Rodyti šalies vėliavą" }]
				})
			],
	};
}
