using App;
using Registrai.App;
using Registrai.Modules;


var app = Startup.Build(args);

var cfg = new Configuration();

DB.ConnStr = cfg.Data.ConnString;
DB.Debug = cfg.Data.Debug;


app.UseAdresuRegistras();
app.UseJuridiniuAsmenuRegistras();
app.UseSaliuRegistras();
app.UseEvrkRegistras();

app.Run();

