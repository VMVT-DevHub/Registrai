using App.Routing;
using Microsoft.AspNetCore.Builder;
using Registrai.Modules.Salys.Methods;
using Registrai.Modules.Salys.Models;

namespace Registrai.Modules;

/// <summary>Šalių registras</summary>
public static class SaliuRegistras {
	/// <summary>Šalių registro inicijavimas</summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public static WebApplication UseSaliuRegistras(this WebApplication app) {

		var tg1 = "Šalių Registras";
		app.Attach(new Route<string>(Sal.Search) {
			Path = "/salys/search", Description = "Šalies paieška", Group = tg1, Method = Method.Get,
			Params = [new("eu") { Description = "Rodyti tik EU šalis" }]
		});
		app.Attach(new Route<string>(Sal.List) { Path = "/salys/list", Description = "Visų šalių informacija", Group = tg1, Method = Method.Get,
			Params = [new("eu") { Description = "Rodyti tik EU šalis" }, new("desc") { Description = "Rikiuoti mažėjančia tvarka" }]
		});
		app.Attach(new Route<string>(Sal.Info) {
			Path = "/salys/{iso3}", Description = "Šalies informacija", Group = tg1, Method = Method.Get,
			Params = [new("flag") { Description = "Rodyti šalies vėliavą" }]
		});

		return app;
	}

}
