using App.Routing;
using Registrai.Modules.JAR.Methods;
using Registrai.Modules.JAR.Models;

namespace Registrai.Modules;

/// <summary>Registro inicijavimas</summary>
public class JuridiniuAsmenuRegistras {
	/// <summary>JAR Maršrutų priskyrimas</summary>
	/// <returns></returns>
	public static RouteDefinition Route() => new("Juridiniai Asmenys") {
		Description = "Registrų centro juridinių asmenų registras",
		Tag = "jar", Version = "v1",
		Routes = [
			new RouteGroup("JAR - Vieši duomenys")
				.Map(new ("/jar/details", JARDetails.Detales) {
					Name = "Gauti juridinio asmens detales", Response=typeof(JAR_Item),
				})
				.Map(new ("/jar/search", JARSearch.FullSearch) {
					Name = "Gauti filtruotą juridinių asmenų paieškos resultatą", Method = Method.Post, Response=typeof(JAR_Search),
					Params = [new("code") { Description = "Ieškoti pagal kodą" }]
				})
				.Map(new ("/jar/search", JARSearch.GetSrh) {
					Name = "Gauti supaprastintą juridinių asmenų paieškos resultatą", Response=typeof(JAR_Search),
					Params = [
						new("details") { Description = "Rodyti daugiau informacijos" },
						new("active") { Description = "Tik aktyvūs juridiniai asmenys" }
					]
				})
			],
	};
}
