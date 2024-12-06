namespace App.Routing;


/// <summary>Klaidos standartinis modelis</summary>
public class Error {
	/// <summary>Klaidos kodas</summary>
	public virtual int Code { get; set; }



}

public static class ErrorExrensions {
	public static T Respond<T>(this HttpResponse rsp, T obj, bool print = false) where T : Error {
		rsp.StatusCode = obj.Code;
		if (print) rsp.WriteAsJsonAsync(obj);
		return obj;
	}
	public static T Respond<T>(this HttpResponse rsp, bool print = false, params string[] str) where T : ErrorB, new() {
		var err = new T();
		foreach (var i in str) err.Details.Add(i);
		return Respond(rsp, err, print);
	}

	/// <summary>Užklausos klaida</summary>
	public static E400 E400(this HttpResponse rsp, bool print = false, params string[] str) => rsp.Respond<E400>(print, str);
	/// <summary>Autorizacijos klaida</summary>
	public static E401 E401(this HttpResponse rsp, bool print = false) => rsp.Respond(Er401, print);
	private static E401 Er401 { get; } = new();
	/// <summary>Prieigos klaida</summary>
	public static E403 E403(this HttpResponse rsp, bool print = false) => rsp.Respond(Er403, print);
	private static E403 Er403 { get; } = new();
	/// <summary>Nerasto resurso klaida</summary>
	public static E404 E404(this HttpResponse rsp, bool print = false) => rsp.Respond(Er404, print);
	private static E404 Er404 { get; } = new();

	/// <summary>Validacijos klaida</summary>
	public static E422 E422(this HttpResponse rsp, bool print = false, params string[] str) => rsp.Respond<E422>(print, str);
	/// <summary>kritinė klaida</summary>
	public static E500 E500(this HttpResponse rsp, bool print = false, params string[] str) => rsp.Respond<E500>(print, str);
}

/// <summary>Klaida su aprašymu</summary>
public class ErrorB : Error {

	/// <summary>Klaidos informacija</summary>
	/// <example>Validacijos informacija</example>
	public virtual List<string> Details { get; set; } = [];
}


/// <summary>Užklausos klaida</summary>
public class E400 : ErrorB {
	/// <summary>Klaidos kodas</summary>
	/// <example>400</example>
	public override int Code { get; set; } = 400;
	/// <summary>Klaidos statusas</summary>
	/// <example>Bad Request</example>
	public string Status { get; set; } = "Bad Request";
	/// <summary>Klaidos Žinutė</summary>
	/// <example>Užklausos klaida</example>
	public string Message { get; set; } = "Užklausos klaida";
	/// <summary>Klaidos informacija</summary>
	/// <example>Validacijos informacija</example>
	public override List<string> Details { get; set; } = [];
}

/// <summary>Vartotojo autorizacijos klaida</summary>
public class E401 : Error {
	/// <summary>Klaidos kodas</summary>
	/// <example>401</example>
	public override int Code { get; set; } = 401;
	/// <summary>Klaidos statusas</summary>
	/// <example>Unauthorized</example>
	public string Status { get; set; } = "Unauthorized";
	/// <summary>Klaidos Žinutė</summary>
	/// <example>Reikalinga vartotojo autorizacija</example>
	public string Message { get; set; } = "Reikalinga vartotojo autorizacija";
}


/// <summary>Vartotojo prieigos klaida</summary>
public class E403 : Error {
	/// <summary>Klaidos kodas</summary>
	/// <example>403</example>
	public override int Code { get; set; } = 403;
	/// <summary>Klaidos statusas</summary>
	/// <example>Forbidden</example>
	public string Status { get; set; } = "Forbidden";
	/// <summary>Klaidos Žinutė</summary>
	/// <example>Jūs neturite prieigos prie šio resourso</example>
	public string Message { get; set; } = "Jūs neturite prieigos prie šio resourso";

}

/// <summary>Vartotojo prieigos klaida</summary>
public class E404 : Error {
	/// <summary>Klaidos kodas</summary>
	/// <example>404</example>
	public override int Code { get; set; } = 404;
	/// <summary>Klaidos statusas</summary>
	/// <example>Not Found</example>
	public string Status { get; set; } = "Not Found";
	/// <summary>Klaidos Žinutė</summary>
	/// <example>Resursas kurio ieškote neegzistuoja</example>
	public string Message { get; set; } = "Resursas kurio ieškote neegzistuoja";

}

/// <summary>Vartotojo prieigos klaida</summary>
public class E422 : ErrorB {
	/// <summary>Klaidos kodas</summary>
	/// <example>422</example>
	public override int Code { get; set; } = 422;
	/// <summary>Klaidos statusas</summary>
	/// <example>Validation Error</example>
	public string Status { get; set; } = "Validation Error";
	/// <summary>Klaidos Žinutė</summary>
	/// <example>Duomenų validacijos klaida</example>
	public string Message { get; set; } = "Duomenų validacijos klaida";
	/// <summary>Klaidos informacija</summary>
	/// <example>Validacijos informacija</example>
	public override List<string> Details { get; set; } = [];

}

/// <summary>Vartotojo prieigos klaida</summary>
public class E500 : ErrorB {
	/// <summary>Klaidos kodas</summary>
	/// <example>500</example>
	public override int Code { get; set; } = 500;
	/// <summary>Klaidos statusas</summary>
	/// <example>Server Error</example>
	public string Status { get; set; } = "Server Error";
	/// <summary>Klaidos Žinutė</summary>
	/// <example>Sistemos klaida</example>
	public string Message { get; set; } = "Sistemos klaida";
	/// <summary>Klaidos informacija</summary>
	/// <example>Validacijos informacija</example>
	public override List<string> Details { get; set; } = [];

}
