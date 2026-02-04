using TaskManager.Web.Extensions;
using TaskManager.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ******* Me levanto sin builder.Services.AddControllersWithViews(); *********
// Add services to the container.
//builder.Services.AddRazorPages(); //Va en lugar de addrazorpages
// Registro del cliente tipado

//builder.Services.AddHttpClient<ITaskApiClient, TaskApiClient>(); //Se agrega y si el proyecto crece esta no se modifica 
builder.Services.AddApiClients();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
//app.MapRazorPages()
//   .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tasks}/{action=Index}/{id?}");

app.Run();
