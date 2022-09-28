using Hermes.Models;

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

				var InfoConsultant = await DbContext.GetConsultant(idConsultant);
				var comp = await DbContext.GetCompetencesByIdConsultant(idConsultant);
				var trze = await DbContext.GetTechnosByIdConsultant(idConsultant);

				ConsultantView = new ConsultantViewObject()
				{
					InfoConsultant = await DbContext.GetConsultant(idConsultant),
					Competences = await DbContext.GetCompetencesByIdConsultant(idConsultant),
					Technos = await DbContext.GetTechnosByIdConsultant(idConsultant)
				};
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
