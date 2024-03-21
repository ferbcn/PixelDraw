using Microsoft.EntityFrameworkCore;
using MyWebApplication.Data;

var builder = WebApplication.CreateBuilder(args);
// Read the connection string from the appsettings.json file
// var DB_CONN_STRING = builder.Configuration["PostgreSQL:ConnectionString"];
// Read environmental variable DB_STRING
var DB_CONN_STRING = Environment.GetEnvironmentVariable("DB_STRING");
builder.Services.AddDbContext<MyWebApplicationContext>(options =>
    // options.UseSqlServer(builder.Configuration.GetConnectionString("MyWebApplicationContext") ?? throw new InvalidOperationException("Connection string 'MyWebApplicationContext' not found.")));
    options.UseNpgsql(DB_CONN_STRING ?? throw new InvalidOperationException("Connection string 'MyPostgreSQLContext' not found."))
);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

// options for websockets
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.MapControllerRoute(
    name: "socket",
    pattern: "{controller=WebSocket}/{action=Index}/");

app.Run();
