using App;
using App.Routing;
using Registrai.App;
using Registrai.Modules;


Startup.AddEndpoint(Registrai.Modules.EvrkRegistras.Init());
Startup.AddEndpoint(Registrai.Modules.JuridiniuAsmenuRegistras.Init());
Startup.AddEndpoint(Registrai.Modules.SaliuRegistras.Init());

var app = Startup.Build(args);

var cfg = new Configuration();

DB.ConnStr = cfg.Data.ConnString;
DB.Debug = cfg.Data.Debug;


app.UseAdresuRegistras();

app.Run();

