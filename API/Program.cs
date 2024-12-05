using Registrai.App;
using App.Routing;
using API.App;
using Modules;
using API;


var app = Startup.Build(args);

var cfg = new Configuration();

DB.ConnStr = cfg.Data.ConnString;
DB.Debug = cfg.Data.Debug;


app.UseAdresuRegistras();
app.UseJuridiniuAsmenuRegistras();

app.Run();

