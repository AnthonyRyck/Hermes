using HermesWebAssembly.Server.Data;
using HermesWebAssembly.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Serilog.Context;
using Serilog.Events;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#if (RELEASEDOCKER)
    string connectionDb = builder.Configuration.GetConnectionString("SqlConnection");

    // *** Dans le cas ou une utilisation avec DOCKER
    string databaseAddress = Environment.GetEnvironmentVariable("DB_HOST");
    string login = Environment.GetEnvironmentVariable("LOGIN_DB");
    string mdp = Environment.GetEnvironmentVariable("PASSWORD_DB");
    string dbName = Environment.GetEnvironmentVariable("DB_NAME");
	string numPort = Environment.GetEnvironmentVariable("NUM_PORT");

    connectionDb = connectionDb.Replace("USERNAME", login)
                            .Replace("YOURPASSWORD", mdp)
                            .Replace("YOURDB", dbName)
                            .Replace("YOURDATABASE", databaseAddress)
							.Replace("YOURPORT", numPort);
#elif DEBUG
string connectionDb = "server=127.0.0.1;port=3305;user id=root;password=PassHermesDb;database=hermesdb";
#else
	string connectionDb = builder.Configuration.GetConnectionString("SqlConnection");
#endif

var serverVersion = ServerVersion.AutoDetect(connectionDb);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseMySql(connectionDb, serverVersion));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

// Service de l'application
builder.Services.AddSingleton(new HermesContext(connectionDb));

builder.Services.AddIdentityServer()
	.AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

builder.Services.AddAuthentication()
	.AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

// Ajout dans la base de l'utilisateur "root"
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

	// Vrai si la base de donnees est a creer, false si elle existait deja.
	if (db.Database.EnsureCreated())
	{
		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

		// Ajout dans la base de l'utilisateur "root"
		await DataInitializer.InitData(roleManager, userManager);
	}

	// Pour créer le schéma de la base
	var hermesCtx = scope.ServiceProvider.GetService<HermesContext>();
	string pathSql = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Scripts", "ConsultantsDb.sql");
	await hermesCtx.CreateTablesAsync(pathSql);

	// Pour mettre à jour la base de donnée.
	//string updateSql = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Script", "UpdateConsultantsDb.sql");
	//await hermesCtx.UpdateDatabaseAsync(updateSql);
}

// Pour les logs.
// ATTENTION : il faut que la table Logs (créé par Serilog) soit faites APRES
// la création des tables ASP, sinon "db.Database.EnsureCreated" considère que la
// base est déjà créée.
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
	.MinimumLevel.Override("System", LogEventLevel.Warning)
	.WriteTo.MySQL(connectionDb, "Logs")
	.CreateLogger();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
