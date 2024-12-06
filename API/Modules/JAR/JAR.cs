using App.Routing;
using Microsoft.AspNetCore.Builder;
using Registrai.Modules.JAR.Methods;
using Registrai.Modules.JAR.Models;

namespace Registrai.Modules;

/// <summary>Juridinių asmenų registras</summary>
public static class JuridiniuAsmenuRegistras {
	/// <summary>Juridinių asmenų registro inicijavimas</summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public static WebApplication UseJuridiniuAsmenuRegistras(this WebApplication app) {

		var tg3 = "Juridinių Asmenų Registras";
		app.Attach(new Route<JAR_Item>(JARDetails.Detales) { Path = "/jar/details", Description = "Gauti juridinio asmens detales", Tag = tg3 });

		app.Attach(new Route<JAR_Search>(JARSearch.FullSearch) { Path = "/jar/search", Description = "Gauti filtruotą juridinių asmenų paieškos resultatą", Tag = tg3, Method = Method.Post });
		app.Attach(new Route<JAR_Search>(JARSearch.GetSrh) { Path = "/jar/search", Description = "Gauti supaprastintą juridinių asmenų paieškos resultatą", Tag = tg3 });

		return app;
	}
}
