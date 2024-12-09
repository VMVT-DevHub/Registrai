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
		//app.Attach(new Route<AR_GEO>(Sal.AobSearch) { Path = "/ar/geo/aob", Description = "Artimiausio adreso paieška pagal koordinates", Tag = tg1, Method = Method.Get });
		//app.Attach(new Route<AR_GEOItem>(ARGEO.Gyv) { Path = "/ar/geo/gyv", Description = "Gyvenvietė pagal koordinates", Tag = tg1, Method = Method.Get });
		//app.Attach(new Route<AR_GEOItem>(ARGEO.Sav) { Path = "/ar/geo/sav", Description = "Savivaldybė pagal koordinates", Tag = tg1, Method = Method.Get });
		app.Attach(new Route<string>(Sal.Search) {
			Path = "/salys/search", Description = "Šalies paieška", Tag = tg1, Method = Method.Get,
			Params = [new("flag") { Description = "Rodyti šalies vėliavą" }]
		});
		app.Attach(new Route<string>(Sal.List) { Path = "/salys/list", Description = "Visų šalių informacija", Tag = tg1, Method = Method.Get });
		app.Attach(new Route<string>(Sal.Info) {
			Path = "/salys/{iso3}", Description = "Šalies informacija", Tag = tg1, Method = Method.Get,
			Params = [new("flag") { Description = "Rodyti šalies vėliavą" }]
		});

		return app;
	}

}
