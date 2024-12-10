using App;
using App.Routing;
using Registrai.App;
using Registrai.Modules;


Startup.AddEndpoint(Registrai.Modules.EvrkRegistras.Init());

var app = Startup.Build(args);

var cfg = new Configuration();

DB.ConnStr = cfg.Data.ConnString;
DB.Debug = cfg.Data.Debug;


app.UseAdresuRegistras();
app.UseJuridiniuAsmenuRegistras();
app.UseSaliuRegistras();

app.Run();

