using HermesWebAssembly.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace HermesWebAssembly.Server.Data
{
	public class DataInitializer
	{
		private const string ROOT_USER = "root";

		public static async Task InitData(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
		{
			var roles = Enum.GetNames(typeof(Role));

			foreach (var role in roles)
			{
				// User est juste pour l'affichage.
				if (role == Role.SansRole.ToString())
					continue;

				if (!await roleManager.RoleExistsAsync(role))
				{
					await roleManager.CreateAsync(new IdentityRole(role));
				}
			}

			// Création de l'utilisateur Root.
			ApplicationUser user = await userManager.FindByNameAsync(ROOT_USER);

			if (user == null)
			{
				var poweruser = new ApplicationUser
				{
					UserName = ROOT_USER,
					Email = "root@email.com",
					EmailConfirmed = true
				};
				string userPwd = "Azerty123!";
				var createPowerUser = await userManager.CreateAsync(poweruser, userPwd);
				if (createPowerUser.Succeeded)
				{
					await userManager.AddToRoleAsync(poweruser, Role.Admin.ToString());
				}
			}
		}

	}
}
