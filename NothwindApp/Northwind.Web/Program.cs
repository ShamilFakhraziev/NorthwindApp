using Northwind.Web;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(wb=>
    {
        wb.UseStartup<Startup>();
    }).Build().Run();
Console.WriteLine("This executes after the web server has stopped!");