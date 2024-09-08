using GraphQL.Server;
using Northwind.GraphQL;
using Packt.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.WebHost.UseUrls("https://localhost:5005");
builder.Services.AddControllers();
builder.Services.AddScoped<NorthwindSchema>();
builder.Services.AddNorthwindContext();

builder.Services.AddGraphQL()
    .AddGraphTypes(typeof(NorthwindSchema), ServiceLifetime.Scoped)
    .AddDataLoader()
    .AddSystemTextJson();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseGraphQLPlayground();
}

app.UseGraphQL<NorthwindSchema>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
