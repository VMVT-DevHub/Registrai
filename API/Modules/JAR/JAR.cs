using App.Routing;
using Microsoft.AspNetCore.Builder;
using Modules.JAR.Methods;
using Modules.JAR.Models;

namespace Modules
{

    /// <summary>Adresų registras</summary>
    public static class JuridiniuAsmenuRegistras {
		/// <summary>Adresų registro inicijavimas</summary>
		/// <param name="app"></param>
		/// <returns></returns>
		public static WebApplication UseJuridiniuAsmenuRegistras(this WebApplication app) {

			//var tg1 = "Juridinių Asmenų Registras - Visi duomenys";
			//app.Attach(new Route<JAR_List>(JARLists.Adm) { Path = "/jar/list/adm", Description = "Apskričių sąrašas", Tag = tg1 });
			//app.Attach(new Route<JAR_List>(JARLists.Filter) { Path = "/jar/list", Description = "Gauti filtruotą sąrašų rezultatą", Tag = tg1, Method = Method.Post });

			//var tg2 = "Juridinių Asmenų Registras - Paieška";
			//app.Attach(new Route<JAR_Search>(JARSearch.Sav) { Path = "/jar/search/sav", Description = "Savivaldybių paieška", Tag = tg2, Method = Method.Get });
			//app.Attach(new Route<JAR_Search>(JARSearch.FullSearch) { Path = "/jar/search", Description = "Gauti filtruotą adresų paieškos resultatą", Tag = tg2, Method = Method.Post });

			var tg3 = "Juridinių Asmenų Registras";
			app.Attach(new Route<JAR_Item>(JARDetails.Detales) { Path = "/jar/details", Description = "Gauti juridinio asmens detales", Tag = tg3 });

			app.Attach(new Route<JAR_Search>(JARSearch.FullSearch) { Path = "/jar/search", Description = "Gauti filtruotą juridinių asmenų paieškos resultatą", Tag = tg3, Method = Method.Post });
			app.Attach(new Route<JAR_Search>(JARSearch.GetSrh) { Path = "/jar/search", Description = "Gauti supaprastintą juridinių asmenų paieškos resultatą", Tag = tg3 });

			return app;
		}

	}

}
