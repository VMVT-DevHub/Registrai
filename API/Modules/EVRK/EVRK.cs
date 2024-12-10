using App.Routing;
using Microsoft.AspNetCore.Builder;
using Registrai.Modules.EVRK.Methods;
using Registrai.Modules.EVRK.Models;

namespace Registrai.Modules;

/// <summary>Šalių registras</summary>
public static class EvrkRegistras {
	/// <summary>Šalių registro inicijavimas</summary>
	/// <returns></returns>
	public static AppRouteEndpoint Init() {

		var tg1 = "EVRK Kodai";

		return new() {
			Name = "EVRK",
			Description = "Ekonominės veiklos rūšių klasifikatorius (v2.1)",
			Tag = "evrk",
			Version = "v1",
			Routes = [
				new ("/evrk/list", Evrk.List) {
					Description = "Kodų sąrašas", Group = tg1, Method = Method.Get, Response=typeof(Evrk_List),
					Params = [
						new("l1") { Description = "Rodyti tik pirmą lygį (parent=\"\")" },
						new("desc") { Description = "Rikiuoti mažėjančia tvarka" }],
				},
				new ("/evrk/details", Evrk.Details) {
					Description = "Įrašo informacija", Group = tg1, Method = Method.Get, Response=typeof(Evrk_Item),
					Params = [new("code") { Description = "Ieškoti pagal kodą" }]
				},
				new ("/evrk/details", Evrk.DetailsMulti) {
					Description = "Įrašo informacija", Group = tg1, Method = Method.Post, Response=typeof(List<Evrk_Item>),
					Params = [new("code") { Description = "Ieškoti pagal kodą" }]
				},
				new ("/evrk/search", Evrk.Search ) {
					Description = "Kodų paieška", Group = tg1, Method = Method.Get, Response=typeof(List<Evrk_Item>),
				}
			],
		};
	}
}
