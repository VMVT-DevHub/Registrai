using App;
using Registrai.App;


using Registrai.Modules;


Startup.Routes(
	JuridiniuAsmenuRegistras.Route(),
	AdresuRegistras.Route(),
	SaliuRegistras.Route(),
	EvrkRegistras.Route(),
	SporRegistras.Route()
);



var app = Startup.Build(args);

var cfg = new Configuration();

DB.Master.ConnStr = cfg.Data.ConnString;
DB.Master.Debug = cfg.Data.Debug;

DB.VVR.ConnStr = cfg.Data.ConnVVR;
DB.VVR.Debug = cfg.Data.Debug;


app.Run();

