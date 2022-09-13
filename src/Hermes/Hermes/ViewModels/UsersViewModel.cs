using Hermes.Components.Dialogs;
using Hermes.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MudBlazor;

namespace Hermes.ViewModels
{
	public class UsersViewModel : IUsersViewModel
	{
		#region Properties

		private ApplicationDbContext AppContext { get; set; }

		public NavigationManager Navigation { get; set; }

		public UserManager<IdentityUser> UserManager { get; set; }

		public bool ShowResetMdp { get; set; }

		public bool IsLoading { get; private set; }

		public List<UserView> AllUsers { get; set; }

		public Action StateHasChanged { get; private set; }

		private string ManagerRole; //= "MANAGER";
		private string MemberRole; //= "MEMBER";

		private IDialogService dialogService;

		#endregion

		#region Constructeur

		public UsersViewModel(ApplicationDbContext appContext, UserManager<IdentityUser> userManager, NavigationManager navigation, IDialogService dialogSvc)
		{
			ShowResetMdp = false;
			dialogService = dialogSvc;

			ManagerRole = Role.Manager.ToString().ToUpper();
			MemberRole = Role.Member.ToString().ToUpper();

			AppContext = appContext;
			UserManager = userManager;
			Navigation = navigation;

			AllUsers = GetAllUser().ToList();
		}

		#endregion

		public async Task LoadUsers()
		{
			try
			{
				AllUsers = new List<UserView>();

				// Récupération des ids pour les rôles de membre et manager.
				IEnumerable<string> idsRoles = AppContext.Roles.Where(x => x.NormalizedName == ManagerRole
																		|| x.NormalizedName == MemberRole)
																.Select(x => x.Id)
																.ToList();

				// Récupération des utilisateurs ayant pour ces rôles.
				IEnumerable<string> idUserRole = AppContext.UserRoles.Where(x => idsRoles.Contains(x.RoleId))
					.Select(x => x.UserId)
					.ToList();

				// Récupération des utilisateurs.
				IEnumerable<IdentityUser> usersTemp = AppContext.Users.Where(x => idUserRole.Contains(x.Id)).ToList();

				foreach (var user in usersTemp)
				{
					string roleId = AppContext.UserRoles.Where(x => x.UserId == user.Id)
											.Select(x => x.RoleId)
											.FirstOrDefault();

					string role = AppContext.Roles.Where(x => x.Id == roleId)
													.Select(x => x.NormalizedName)
													.FirstOrDefault();

					UserView userView = new UserView();
					userView.IdUser = user.Id;
					userView.UserName = user.UserName;
					userView.Email = user.Email;
					userView.Role = role;
					userView.IdentityModel = user;

					AllUsers.Add(userView);
				}
			}
			catch (Exception exception)
			{
				Log.Error(exception, "GestionUserPage - Erreur dans la récupération de la liste des utilisateurs Membre et Manager.");
				AllUsers = new List<UserView>();
			}
		}

		public void SetStateHasChanged(Action stateHasChanged)
		{
			StateHasChanged = stateHasChanged;
		}

		/// <summary>
		/// Au changement de rôle.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="idUser"></param>
		public void OnChangeRole(ChangeEventArgs e, string idUser)
		{
			try
			{
				//var selectedValue = e.Value.ToString();
				//UserView currentUser = AllUsers.Where(x => x.User.Id == idUser).FirstOrDefault();

				//if (string.IsNullOrEmpty(selectedValue))
				//	return;

				//if (selectedValue == "Inactif")
				//{
				//	UserManager.RemoveFromRoleAsync(currentUser.User, currentUser.Role);
				//}
				//else
				//{
				//	UserManager.RemoveFromRoleAsync(currentUser.User, currentUser.Role);
				//	UserManager.AddToRoleAsync(currentUser.User, selectedValue);
				//}

				//AllUsers = GetAllUser().ToList();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Erreur sur changement de role.");
			}

		}

		public void DeleteUser(string idUser)
		{
			//if (AppContext.Users.Any(x => x.Id == idUser))
			//{
			//	var user = AppContext.Users.FirstOrDefault(x => x.Id == idUser);

			//	AppContext.Users.Remove(user);
			//	AppContext.SaveChanges();

			//	AllUsers.RemoveAll(x => x.User.Id == idUser);
			//}
		}

		public async void ResetChangeMdp(string idUser)
		{
			string login = await SetNewPassword(idUser, "Azerty123!");
			
			var parameters = new DialogParameters();
			parameters.Add("LoginUser", login);

			var options = new DialogOptions { CloseOnEscapeKey = true };

			dialogService.Show<ResetPasswordDialog>("Réinitialisation du mot de passe", parameters, options);
		}

		public async void EditUser(string idUser)
		{
			UserView userSelected = AllUsers.FirstOrDefault(x => x.IdUser == idUser);

			var parameters = new DialogParameters();
			parameters.Add("RoleActuel", userSelected.Role);

			var options = new DialogOptions { CloseOnEscapeKey = true };
			var dialog = dialogService.Show<ChangeRoleDialog>("Changement de rôle", parameters, options);
			var result = await dialog.Result;

			if(!result.Cancelled)
			{
				Role roleSelected = (Role)result.Data;

				// Faire la sauvegarde
				await UserManager.RemoveFromRoleAsync(userSelected.IdentityModel, userSelected.Role);
				await UserManager.AddToRoleAsync(userSelected.IdentityModel, roleSelected.ToString());

				userSelected.Role = roleSelected.ToString().ToUpper();
				StateHasChanged.Invoke();
			}
		}

		#region Private Methods

		private async Task<string> SetNewPassword(string idUser, string newPassword)
		{
			ShowResetMdp = false;
			string loginUser = string.Empty;

			try
			{
				IdentityUser userSelected = AppContext.Users.Where(x => x.Id == idUser).FirstOrDefault();

				await UserManager.RemovePasswordAsync(userSelected);
				await UserManager.AddPasswordAsync(userSelected, newPassword);

				loginUser = userSelected.UserName;
			}
			catch (Exception)
			{
				Log.Error("Erreur sur REINIT de mot de passe");
			}

			return loginUser;
		}


		/// <summary>
		/// Retourne la liste des Managers et des membres.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<UserView> GetAllUser()
		{
			List<UserView> usersList = new List<UserView>();

			try
			{
				// Récupération des ids pour les rôles de membre et manager.
				IEnumerable<string> idsRoles = AppContext.Roles.Where(x => x.NormalizedName == ManagerRole
																		|| x.NormalizedName == MemberRole)
																.Select(x => x.Id)
																.ToList();

				// Récupération des utilisateurs ayant pour ces rôles.
				IEnumerable<string> idUserRole = AppContext.UserRoles.Where(x => idsRoles.Contains(x.RoleId))
					.Select(x => x.UserId)
					.ToList();

				// Récupération des utilisateurs.
				IEnumerable<IdentityUser> usersTemp = AppContext.Users.Where(x => idUserRole.Contains(x.Id)).ToList();

				foreach (var user in usersTemp)
				{
					string roleId = AppContext.UserRoles.Where(x => x.UserId == user.Id)
											.Select(x => x.RoleId)
											.FirstOrDefault();

					string role = AppContext.Roles.Where(x => x.Id == roleId)
													.Select(x => x.NormalizedName)
													.FirstOrDefault();

					UserView userView = new UserView();
					userView.IdUser = user.Id;
					userView.UserName = user.UserName;
					user.Email = user.Email;
					userView.Role = role;

					usersList.Add(userView);
				}
			}
			catch (Exception exception)
			{
				Log.Error(exception, "GestionUserPage - Erreur dans la récupération de la liste des utilisateurs Membre et Manager.");
			}

			return usersList;
		}

		#endregion
	}

	public class UserView
	{
		public string IdUser { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		
		public string Role { get; set; }

		public IdentityUser IdentityModel { get; set; }
	}
}