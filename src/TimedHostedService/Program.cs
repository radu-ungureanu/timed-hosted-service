using TimedHostedService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<SampleTimedService>();

var app = builder.Build();

app.Run();
