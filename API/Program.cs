using Registrai.App;
using AR;
using App.Routing;
using API.App;


var app = Startup.Build(args);

var cfg = new Configuration();

App.DB.ConnStr = cfg.Data.ConnString;



app.UseAdresuRegistras();


app.Run();

