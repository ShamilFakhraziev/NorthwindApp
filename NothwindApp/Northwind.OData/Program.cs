using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Packt.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.UseUrls("https://localhost:5004");

builder.Services.AddNorthwindContext();
builder.Services.AddControllers().AddOData(ops=>ops
.AddRouteComponents("catalog",GetEdmModelForCatalog())
.AddRouteComponents("ordersystem",GetEdmModelForOrderSystem())
.AddRouteComponents("v{version}",GetEdmModelForCatalog())
.Select()
.Expand() // включение $expand для навигации по сущностям
.Filter() // включение $filter
.OrderBy() // включение $orderby
.SetMaxTop(100) // включение $top
.Count() // включение $count
);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

IEdmModel GetEdmModelForCatalog()
{
    ODataConventionModelBuilder builder = new();
    builder.EntitySet<Category>("Categories");
    builder.EntitySet<Product>("Products");
    builder.EntitySet<Supplier>("Suppliers");
    return builder.GetEdmModel();
}

IEdmModel GetEdmModelForOrderSystem()
{
    ODataConventionModelBuilder builder = new();
    builder.EntitySet<Customer>("Customers");
    builder.EntitySet<Order>("Orders");
    builder.EntitySet<Employee>("Employees");
    builder.EntitySet<Product>("Products");
    builder.EntitySet<Shipper>("Shippers");
    return builder.GetEdmModel();
}