using App;
using Registrai.App;


using Registrai.Modules;
using Registrai.Modules.UPD;


Startup.Routes(
	JuridiniuAsmenuRegistras.Route(),
	AdresuRegistras.Route(),
	SaliuRegistras.Route(),
	EvrkRegistras.Route(),
	SporRegistras.Route(),
	UpdRegistras.Route()
);



var app = Startup.Build(args);

var cfg = new Configuration().Data;

DB.Master.ConnStr = cfg.ConnString;
DB.Master.Debug = cfg.Debug;

DB.VVR.ConnStr = cfg.ConnVVR;
DB.VVR.Debug = cfg.Debug;

UpdMedicines.Endpoint = cfg.VVREndpoint;

app.Run();

