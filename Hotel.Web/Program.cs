namespace Hotel.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Agregado el 13/04/2023 para el Session
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                //options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.IdleTimeout = TimeSpan.FromMinutes(20); //Se cierra la sesion luego de 20 minutos
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            //

            var app = builder.Build();

            

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //Agregado el 13/04/2023 para el Session
            app.UseSession();
            //

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}