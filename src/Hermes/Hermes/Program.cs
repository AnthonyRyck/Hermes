using Hermes.Areas.Identity;
using Hermes.Data;
using Hermes.DataAccess;
using Hermes.ViewModels;
using Hermes.ViewModels.Settings;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MudBlazor;
using MudBlazor.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

#if RELEASEDOCKER
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

// Pour MudBlazor
builder.Services.AddMudServices(config => 
{
	config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

	config.SnackbarConfiguration.PreventDuplicates = false;
	config.SnackbarConfiguration.NewestOnTop = false;
	config.SnackbarConfiguration.ShowCloseIcon = true;
	config.SnackbarConfiguration.VisibleStateDuration = 10000;
	config.SnackbarConfiguration.HideTransitionDuration = 500;
	config.SnackbarConfiguration.ShowTransitionDuration = 500;
	config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

// Service de l'application
builder.Services.AddSingleton<IHermesContext>(new HermesContext(connectionDb));
builder.Services.AddScoped<IUsersViewModel, UsersViewModel>();
builder.Services.AddScoped<ITechnosViewModel, TechnosViewModel>();
builder.Services.AddScoped<ICompetenceViewModel, CompetenceViewModel>();
builder.Services.AddScoped<ISocieteViewModel, SocieteViewModel>();
builder.Services.AddScoped<IAgenceViewModel, AgenceViewModel>();
builder.Services.AddScoped<INouveauConsultantViewModel, NouveauConsultantViewModel>();
builder.Services.AddScoped<IConsultantViewModel, ConsultantViewModel>();

// Augmentation de la taille des messages pour les images.
builder.Services.AddSignalR(e => {
	e.MaximumReceiveMessageSize = 102400000;
});

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

// Pour forcer l'application en Français.
var cultureInfo = new CultureInfo("fr-Fr");
cultureInfo.NumberFormat.CurrencySymbol = "€";

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

	// Vrai si la base de données est créée, false si elle existait déjà.
	if (db.Database.EnsureCreated())
	{
		DataInitializer.InitData(roleManager, userManager).Wait();

		// Pour créer le schéma de la base
		string pathSql = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "ConsultantsDb.sql");
		await hermesCtx.CreateTablesAsync(pathSql);
	}

	// Pour mettre à jour la base de donnée.
	//string updateSql = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Script", "UpdateConsultantsDb.sql");
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

// Chemin pour stocker les images des consultants
string pathImages = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesHermes.IMAGES);
if (!Directory.Exists(pathImages))
	Directory.CreateDirectory(pathImages);

string pathConsultantsImg = Path.Combine(pathImages, ConstantesHermes.IMG_CONSULTANTS);
if (!Directory.Exists(pathConsultantsImg))
	Directory.CreateDirectory(pathConsultantsImg);


app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(pathConsultantsImg),
	RequestPath = ConstantesHermes.REQUEST_PATH_IMG
});

app.Run();