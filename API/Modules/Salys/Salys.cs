using App.Routing;
using Microsoft.AspNetCore.Builder;
using Registrai.Modules.Salys.Methods;
using Registrai.Modules.Salys.Models;

namespace Registrai.Modules;

/// <summary>Šalių registras</summary>
public static class SaliuRegistras {
	
	/// <summary>Šalių registro inicijavimas</summary>
	/// <returns></returns>
	public static AppRouteEndpoint Init() {
		var tg1 = "Šalių API";
		return new() {
			Name = "Šalių registras",
			Description = "Pilnas šalių sąrašas su vėliavomis",
			Tag = "salys",
			Version = "v1",
			Routes = [
				new ("/salys/search", Sal.Search) {
					Description = "Šalies paieška", Group = tg1, Response=typeof(List<Sal_Item>),
					Params = [new("eu") { Description = "Rodyti tik EU šalis" }]
				},
				new ("/salys/list", Sal.List) {
					Description = "Visų šalių informacija", Group = tg1, Response=typeof(Sal_List),
					Params = [new("eu") { Description = "Rodyti tik EU šalis" }, new("desc") { Description = "Rikiuoti mažėjančia tvarka" }]
				},
				new ("/salys/{iso3}", Sal.Info) {
					Description = "Šalies informacija", Group = tg1,  Response=typeof(Sal_Item),
					Params = [new("flag") { Description = "Rodyti šalies vėliavą" }]
				}
			],
		};
	}
}
