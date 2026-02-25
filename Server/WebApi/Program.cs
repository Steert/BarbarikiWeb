using BusinessLogic;
using DataAccess;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load(options: new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 4));

builder.Services.AddDataAccess(builder.Configuration, Environment.GetEnvironmentVariable("DATABASE_STRING_KEY"));
builder.Services.AddBusinessLogic();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
var app = builder.Build();


app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();