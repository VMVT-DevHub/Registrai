using App;
using Registrai.App;


using Registrai.Modules;


using Registrai.Modules.JAR;
using Registrai.Modules.Salys;
using Registrai.Modules.EVRK;


Startup.Routes(
	JuridiniuAsmenuRegistras.Route(),
	AdresuRegistras.Route(),
	SaliuRegistras.Route(),
	EvrkRegistras.Route()
);



var app = Startup.Build(args);

var cfg = new Configuration();

DB.ConnStr = cfg.Data.ConnString;
DB.Debug = cfg.Data.Debug;


app.Run();

