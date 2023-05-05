using Innowise.Clinic.Gateway.Configuration;
using Innowise.Clinic.Gateway.Middleware;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(x => x.AddDefaultPolicy(p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
builder.Services.AddEndpointsApiExplorer();
builder.ConfigureGateway();
builder.ConfigureJwtAuth();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCors();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseSwaggerForOcelotUI(opt => { opt.PathToSwaggerGenerator = "/swagger/docs"; });
app.UseOcelot().Wait();


app.Run();