using Hermes.Areas.Identity;
using Hermes.Data;
using Hermes.DataAccess;
using Hermes.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using System.Globalization;

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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseMySql(connectionDb, ServerVersion.AutoDetect(connectionDb)));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

// Service de l'application
builder.Services.AddSingleton(new HermesContext(connectionDb));
builder.Services.AddScoped<IUsersViewModel, UsersViewModel>();

// Pour MudBlazor
builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

// Pour forcer l'application en Fran�ais.
var cultureInfo = new CultureInfo("fr-Fr");
cultureInfo.NumberFormat.CurrencySymbol = "�";

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Ajout dans la base de l'utilisateur "root"
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

	var hermesCtx = scope.ServiceProvider.GetService<HermesContext>();

	// Vrai si la base de donn�es est cr��e, false si elle existait d�j�.
	if (db.Database.EnsureCreated())
	{
		DataInitializer.InitData(roleManager, userManager).Wait();

		// Pour cr�er le sch�ma de la base
		string pathSql = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "ConsultantsDb.sql");
		await hermesCtx.CreateTablesAsync(pathSql);
	}

	// Pour mettre � jour la base de donn�e.
	//string updateSql = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Script", "UpdateConsultantsDb.sql");
	//await hermesCtx.UpdateDatabaseAsync(updateSql);
}

// Pour les logs.
// ATTENTION : il faut que la table Logs (cr�� par Serilog) soit faites APRES
// la cr�ation des tables ASP, sinon "db.Database.EnsureCreated" consid�re que la
// base est d�j� cr��e.
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
	.MinimumLevel.Override("System", LogEventLevel.Warning)
	.WriteTo.MySQL(connectionDb, "Logs")
	.CreateLogger();

app.Run();