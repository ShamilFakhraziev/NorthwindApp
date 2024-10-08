using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Packt.Shared;
using Northwind.WebApi.Repositories;
using Microsoft.AspNetCore.HttpLogging;
using static System.Console;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://localhost:5002/");
builder.Services.AddCors();
// Add services to the container.
builder.Services.AddNorthwindContext();
builder.Services.AddControllers(options =>
{
    WriteLine("Default output formatters:");
    foreach (IOutputFormatter formatter in options.OutputFormatters)
    {
        OutputFormatter? mediaFormatter = formatter as OutputFormatter;
        if (mediaFormatter == null)
        {
            WriteLine($" {formatter.GetType().Name}");
        }
        else // ����� ��������� ������ � ��������������� ��������������
        {
            WriteLine(" {0}, Media types: {1}",
            arg0: mediaFormatter.GetType().Name,
            arg1: string.Join(", ",
            mediaFormatter.SupportedMediaTypes));
        }
    }
})
.AddXmlDataContractSerializerFormatters()
.AddXmlSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>
c.SwaggerDoc("v1",new() { Title = "Northwind Service API", Version = "v1" })
);

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.All;
    options.RequestBodyLogLimit = 4096; // default is 32k
    options.ResponseBodyLogLimit = 4096; // default is 32k
});

builder.Services.AddHealthChecks().AddDbContextCheck<NorthwindContext>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

var app = builder.Build();
app.UseHttpLogging();
app.UseCors(ops =>
{
    ops.WithMethods("GET","POST","PUT","DELETE");
    ops.WithOrigins("https://localhost:5001");
});
app.UseMiddleware<SecurityHeaders>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "Northwind Service API Version 1");
        c.SupportedSubmitMethods(new[] {SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Delete});
    }
    );
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseHealthChecks("/howdoufeel");

app.MapControllers();

app.Run();
