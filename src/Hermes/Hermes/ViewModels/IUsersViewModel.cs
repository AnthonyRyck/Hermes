using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace Hermes.ViewModels
{
	public interface IUsersViewModel
	{
		List<UserView> AllUsers { get; set; }
		NavigationManager Navigation { get; set; }
		UserManager<IdentityUser> UserManager { get; set; }
		bool ShowResetMdp { get; set; }

		bool IsLoading { get; }

		Task LoadUsers();

		void SetStateHasChanged(Action stateHasChanged);

		void DeleteUser(string idUser);

		void ResetChangeMdp(string idUser);

		void EditUser(string idUser);
	}
}
