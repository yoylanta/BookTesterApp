using BookTesterApp.Presentation.Module;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureSerilog(builder.Configuration);
builder.Services.AddRazorPages();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapRazorPages();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}"
);

app.Run();