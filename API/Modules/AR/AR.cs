using App.Routing;
using AR.Methods;
using AR.Models;
using Microsoft.AspNetCore.Builder;

namespace AR {

	/// <summary>Adresų registras</summary>
	public static class AdresuRegistras {
		/// <summary>Adresų registro inicijavimas</summary>
		/// <param name="app"></param>
		/// <returns></returns>
		public static WebApplication UseAdresuRegistras(this WebApplication app) {

			var tg1 = "Adresų Registras - Visi duomenys";
			app.Attach(new Route<AR_List>(Lists.Adm) { Path = "/ar/list/adm", Description = "Apskričių sąrašas", Tag = tg1 });
			app.Attach(new Route<AR_List>(Lists.Sav) { Path = "/ar/list/sav", Description = "Savivaldybių sąrašas", Tag = tg1 });
			app.Attach(new Route<AR_List>(Lists.Sen) { Path = "/ar/list/sen", Description = "Seniunijų sąrašas", Tag = tg1 });
			app.Attach(new Route<AR_List>(Lists.Gyv) { Path = "/ar/list/gyv", Description = "Gyvenviečių sąrašas", Tag = tg1 });
			app.Attach(new Route<AR_List>(Lists.Gat) { Path = "/ar/list/gat", Description = "Gatvių sąrašas", Tag = tg1 });
			app.Attach(new Route<AR_List>(Lists.Aob) { Path = "/ar/list/aob", Description = "Adresų sąrašas", Tag = tg1 });
			app.Attach(new Route<AR_List>(Lists.Pat) { Path = "/ar/list/pat", Description = "Patalpų sąrašas", Tag = tg1 });
			app.Attach(new Route<AR_List>(Lists.Filter) { Path = "/ar/list", Description = "Gauti filtruotą sąrašų rezultatą", Tag = tg1, Method = Method.Post });

			var tg2 = "Adresų Registras - Paieška";
			app.Attach(new Route<AR_Search>(Search.Sav) { Path = "/ar/search/sav", Description = "Savivaldybių paieška", Tag = tg2, Method = Method.Get });
			app.Attach(new Route<AR_Search>(Search.Sen) { Path = "/ar/search/sen", Description = "Seniunijų paieška", Tag = tg2, Method = Method.Get });
			app.Attach(new Route<AR_Search>(Search.Gyv) { Path = "/ar/search/gyv", Description = "Gyvenviečių paieška", Tag = tg2, Method = Method.Get });
			app.Attach(new Route<AR_Search>(Search.Gat) { Path = "/ar/search/gat", Description = "Gatvių paieška", Tag = tg2, Method = Method.Get });
			app.Attach(new Route<AR_Search>(Search.Aob) { Path = "/ar/search/aob", Description = "Adresų paieška", Tag = tg2, Method = Method.Get });
			app.Attach(new Route<AR_Search>(Search.Adr) { Path = "/ar/search/adr", Description = "išplėstinė adresų paieška pagal gatvę, adresą ir patalpą", Tag = tg2, Method = Method.Get });
			app.Attach(new Route<AR_Search>(Search.FullSearch) { Path = "/ar/search", Description = "Gauti filtruotą adresų paieškos resultatą", Tag = tg2, Method = Method.Post });

			var tg3 = "Adresų Registras - Įrašo detalės";
			app.Attach(new Route<AR_Item>(Details.Detales) { Path = "/ar/details", Description = "Gauti adreso detales", Tag = tg3 });




			return app;
		}

	}

}
