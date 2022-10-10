using BlazorDownloadFile;
using Hermes.Models;
using Microsoft.JSInterop;

namespace Hermes.ViewModels
{
	public class ConsultantViewModel : BaseViewModel, IConsultantViewModel
	{
		private IBlazorDownloadFileService DownloadFileService;


		public ConsultantViewModel(IHermesContext contextHermes, ISnackbar snackbar, IBlazorDownloadFileService downloadFileService) 
			: base(contextHermes, snackbar)
		{
			DownloadFileService = downloadFileService;
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

		public async Task DownloadDossierCompetence()
		{
			try
			{
				var dossier = await DbContext.GetDossierCompetence(ConsultantView.InfoConsultant.Id);
				
				if (dossier != null)
				{
					await DownloadFileService.DownloadFile(ConsultantView.InfoConsultant.FileName, dossier, "application/octet-stream");
				}
			}
			catch (Exception ex)
			{
				this.Error("Erreur sur le téléchargement du dossier de compétence", ex);
			}
		}

		#endregion
	}
}
