using App.Routing;
using Microsoft.AspNetCore.Builder;
using Registrai.Modules.EVRK.Methods;
using Registrai.Modules.EVRK.Models;

namespace Registrai.Modules;

/// <summary>Šalių registras</summary>
public static class EvrkRegistras {
	/// <summary>Šalių registro inicijavimas</summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public static WebApplication UseEvrkRegistras(this WebApplication app) {

		var tg1 = "EVRK - Kodų Registras";
		app.Attach(new Route<Evrk_List>(Evrk.List) {
			Path = "/evrk/list", Description = "Kodų sąrašas", Tag = tg1, Method = Method.Get,
			Params = [
				new("l1") { Description = "Rodyti tik pirmą lygį (parent=\"\")" },
				new("desc") { Description = "Rikiuoti mažėjančia tvarka" }]
		});
		app.Attach(new Route<Evrk_Item>(Evrk.Details) {
			Path = "/evrk/details", Description = "Įrašo informacija", Tag = tg1, Method = Method.Get,
			Params = [new("code") { Description = "Ieškoti pagal kodą" }]
		});
		app.Attach(new Route<List<Evrk_Item>>(Evrk.DetailsMulti) { Path = "/evrk/details", Description = "Įrašo informacija", Tag = tg1, Method = Method.Post, 
			Params  = [new("code") { Description = "Ieškoti pagal kodą" }]
		});
		app.Attach(new Route<List<Evrk_Item>>(Evrk.Search) {
			Path = "/evrk/search", Description = "Kodų paieška", Tag = tg1, Method = Method.Get
		});
		return app;
	}

}
