using App.Routing;
using Microsoft.AspNetCore.Builder;
using Registrai.Modules.JAR.Methods;
using Registrai.Modules.JAR.Models;

namespace Registrai.Modules;

/// <summary>Juridinių asmenų registras</summary>
public static class JuridiniuAsmenuRegistras {
	/// <summary>Juridinių asmenų registro inicijavimas</summary>
	/// <returns></returns>
	public static AppRouteEndpoint Init() {

		var tg1 = "JAR - Vieši duomenys";

		return new() {
			Name = "Juridinių Asmenų Registras",
			Description = "Registrų centro Juridinių Asmenų Registras",
			Tag = "jar",
			Version = "v1",
			Routes = [
				new ("/jar/details", JARDetails.Detales) {
					Description = "Gauti juridinio asmens detales", Group = tg1, Response=typeof(JAR_Item),
				},
				new ("/jar/search", JARSearch.FullSearch) {
					Description = "Gauti filtruotą juridinių asmenų paieškos resultatą", Group = tg1, Method = Method.Post, Response=typeof(JAR_Search),
					Params = [new("code") { Description = "Ieškoti pagal kodą" }]
				},
				new ("/jar/search", JARSearch.GetSrh) {
					Description = "Gauti supaprastintą juridinių asmenų paieškos resultatą", Group = tg1,  Response=typeof(JAR_Search),
					Params = [
						new("details") { Description = "Rodyti daugiau informacijos" },
						new("active") { Description = "Tik aktyvūs juridiniai asmenys" }
					]
				}
			],
		};
	}
}
