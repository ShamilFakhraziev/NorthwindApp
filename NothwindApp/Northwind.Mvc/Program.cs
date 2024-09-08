using Northwind.Mvc.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Northwind.Mvc.Data;
using Packt.Shared;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient(name: "Northwind.WebApi",
configureClient: options =>
{
    options.BaseAddress = new Uri("https://localhost:5002/");
    options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1.0));
});
builder.Services.AddHttpClient(name: "Minimal.WebApi",
configureClient: options =>
{
    options.BaseAddress = new Uri("https://localhost:5003/");
    options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1.0));
});
builder.Services.AddHttpClient("Northwind.OData",ops=>
{
    ops.BaseAddress = new Uri("https://localhost:5004");
    ops.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json",1.0));
}
);

builder.Services.AddHttpClient("Northwind.GraphQL", ops =>
{
    ops.BaseAddress = new Uri("https://localhost:5005");
    ops.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1.0));
});
builder.Services.AddSignalR();
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

string sqlServerConnection = builder.Configuration.GetConnectionString("NorthwindConnection");
builder.Services.AddNorthwindContext(sqlServerConnection);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapHub<ChatHub>("/chat");

app.Run();
