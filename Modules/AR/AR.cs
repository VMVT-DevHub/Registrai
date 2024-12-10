using App.Routing;
using Registrai.Modules.AR.Methods;
using Registrai.Modules.AR.Models;

namespace Registrai.Modules;

/// <summary>Registro inicijavimas</summary>
public class AdresuRegistras {
	private static readonly List<RouteParam> Prdsc = [new("desc") { Description = "Rikiavimas mažėjančiai" }];
	/// <summary>Adresų registro maršrutų priskyrimas</summary>
	/// <returns></returns>
	public static RouteDefinition Route() => new("Adresai") {
		Description = "Registrų centro adresų registras su GEO duomenimis",
		Tag = "ar", Version = "v1",
		Routes = [
			new RouteGroup("Įrašo detalės")
				.Map(new("/ar/details", ARDetails.Detales){ Name = "Gauti adreso detales", Response=typeof(AR_Item) }),

			new RouteGroup("Visi duomenys")
				.Map(new("/ar/list/adm", ARLists.Adm){ Name = "Apskričių sąrašas", Response=typeof(AR_List), Params=Prdsc })
				.Map(new("/ar/list/sav", ARLists.Sav){ Name = "Savivaldybių sąrašas", Response=typeof(AR_List), Params=Prdsc })
				.Map(new("/ar/list/sen", ARLists.Sen){ Name = "Seniunijų sąrašas", Response=typeof(AR_List), Params=Prdsc })
				.Map(new("/ar/list/gyv", ARLists.Gyv){ Name = "Gyvenviečių sąrašas", Response=typeof(AR_List), Params=Prdsc })
				.Map(new("/ar/list/gat", ARLists.Gat){ Name = "Gatvių sąrašas", Response=typeof(AR_List), Params=Prdsc })
				.Map(new("/ar/list/aob", ARLists.Aob){ Name = "Adresų sąrašas", Response=typeof(AR_List), Params=Prdsc })
				.Map(new("/ar/list/pat", ARLists.Pat){ Name = "Patalpų sąrašas", Response=typeof(AR_List), Params=Prdsc })
				.Map(new("/ar/list",     ARLists.Filter, Method.Post){ Name = "Gauti filtruotą sąrašų rezultatą", Response=typeof(AR_List) }),

			new RouteGroup("Detali paieška")
				.Map(new("/ar/search/sav", ARSearch.Sav){ Name = "Savivaldybių paieška", Response=typeof(AR_Search) })
				.Map(new("/ar/search/sen", ARSearch.Sen){ Name = "Seniunijų paieška", Response=typeof(AR_Search) })
				.Map(new("/ar/search/gyv", ARSearch.Gyv){ Name = "Gyvenviečių paieška", Response=typeof(AR_Search) })
				.Map(new("/ar/search/gat", ARSearch.Gat){ Name = "Gatvių paieška", Response=typeof(AR_Search) })
				.Map(new("/ar/search/aob", ARSearch.Aob){ Name = "Adresų paieška", Response=typeof(AR_Search) })
				.Map(new("/ar/search/adr", ARSearch.Adr) { Deprecated=true, Name = "Naudoti <b>/ar/find/adr</b>", Response=typeof(AR_Search) })
				.Map(new("/ar/search",     ARSearch.FullSearch, Method.Post){ Name = "Gauti filtruotą adresų paieškos resultatą", Response=typeof(AR_Search) }),

			new RouteGroup("Geografinė paieška")
				.Map(new("/ar/geo/aob", ARGEO.AobSearch){ Name = "Artimiausio adreso paieška pagal koordinates", Response=typeof(AR_GEO) })
				.Map(new("/ar/geo/gyv", ARGEO.Gyv){ Name = "Gyvenvietė pagal koordinates", Response=typeof(AR_GEOItem) })
				.Map(new("/ar/geo/sav", ARGEO.Sav){ Name = "Savivaldybė pagal koordinates", Response=typeof(AR_GEOItem) }),

			new RouteGroup("Greita paieška")
				.Map(new("/ar/find/gyv",   ARSearch.FGyv){ Name = "Greita gyvenviečių paieška", Response=typeof(AR_Search) })
				.Map(new("/ar/find/adr",   ARSearch.FAdr){ Name = "Adresų paieška pagal gatvę, adresą ir/ar patalpą", Response=typeof(AR_Search) }),
			],
	};
}
