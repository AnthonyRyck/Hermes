using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Microsoft.JSInterop;

namespace Hermes.ViewModels
{
    public class ConsultantViewModel : BaseViewModel, IConsultantViewModel
    {
		private readonly NavigationManager Nav;

		public ConsultantViewModel(IHermesContext contextHermes, ISnackbar snackbar, NavigationManager navigationManager) 
            : base(contextHermes, snackbar)
        {
			Nav = navigationManager;
		}

		#region Implement IConsultantViewModel

		public bool IsLoading { get; private set; }

		public List<Consultant> Consultants { get; private set; }



		public async Task LoadConsultants()
		{
			try
			{
				IsLoading = true;
				Consultants = await DbContext.LoadConsultants();
				IsLoading = false;
			}
			catch (Exception ex)
			{
				Error("Erreur lors du chargement des consultants", ex);
			}
		}


		public async Task AddConsultant()
		{
			try
			{
				// ouvrir la nouvelle page.
				Nav.NavigateTo($"/addconsultant", true);
			}
			catch (Exception ex)
			{
				Error("Erreur lors de l'ajout d'un consultant", ex);
			}
		}

		public async void OpenPageConsultant(int idConsultant)
		{
			//await JSRuntime.InvokeAsync<object>("open", $"/post/{idPost}/{urlTitle}", "_blank");
		}

		#endregion

		#region Private Methods



		#endregion
	}
}
