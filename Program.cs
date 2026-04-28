using Microsoft.EntityFrameworkCore;
using DoAn.Components;
using DoAn.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddMvcOptions(options =>
    {
        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
            _ => "Vui lòng nhập dữ liệu!"
        );

        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
            (value, fieldName) => $"Giá trị '{value}' không hợp lệ!"
        );

        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
            value => $"Giá trị '{value}' không hợp lệ!"
        );
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

builder.Services.AddSession();

builder.Services.AddTransient<MenuViewComponent>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();