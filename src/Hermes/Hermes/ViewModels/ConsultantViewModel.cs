namespace Hermes.ViewModels
{
	public class ConsultantViewModel : BaseViewModel, IConsultantViewModel
	{
		
	

		public ConsultantViewModel(IHermesContext contextHermes, ISnackbar snackbar) 
			: base(contextHermes, snackbar)
		{
			
		}
		

		#region Implement IConsultantViewModel

		public bool IsLoading { get; private set; }

		public ConsultantViewObject ConsultantView { get; private set; }

		
		public async Task LoadConsultant(uint idConsultant)
		{
			try
			{
				IsLoading = true;
				ConsultantView = await DbContext.GetAllInfoConsultant(idConsultant);				
				IsLoading = false;
			}
			catch (Exception ex)
			{
				this.Error("Erreur sur le chargement des informations du consultant", ex);
			}
		}

		#endregion
	}
}
