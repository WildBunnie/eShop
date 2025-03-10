using Asp.Versioning.Builder;
using System.Reflection;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Basket"))
        .AddAspNetCoreInstrumentation()
        .AddGrpcClientInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("basket.api")
        .AddOtlpExporter(o =>{ o.Endpoint = new Uri("http://localhost:4317"); })
    )
    .WithMetrics(metrics => metrics
        .AddMeter("basket.api")
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter(o =>{ o.Endpoint = new Uri("http://localhost:4316"); })
    );;

builder.AddBasicServiceDefaults();
builder.AddApplicationServices();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGrpcService<BasketService>();

app.Run();
