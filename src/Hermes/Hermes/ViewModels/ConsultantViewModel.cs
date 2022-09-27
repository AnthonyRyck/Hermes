using Google.Protobuf.WellKnownTypes;
using Hermes.Codes.ViewObjects;
using Hermes.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Microsoft.JSInterop;

namespace Hermes.ViewModels
{
    public class ConsultantViewModel : BaseViewModel, IConsultantViewModel
    {
		private readonly NavigationManager Nav;
		private IEnumerable<CompetenceTechnoViewObject> CompetenceTechnoViews;

		public ConsultantViewModel(IHermesContext contextHermes, ISnackbar snackbar, NavigationManager navigationManager) 
            : base(contextHermes, snackbar)
        {
			Nav = navigationManager;
			AllKeywords = new List<string>();
		}

		#region Implement IConsultantViewModel

		public bool IsLoading { get; private set; }

		public List<Consultant> Consultants { get; private set; }

		private List<string> AllKeywords;

		public List<CompetenceTechnoViewObject> Filtres { get; private set; } = new List<CompetenceTechnoViewObject>();


		public async Task LoadConsultants()
		{
			try
			{
				IsLoading = true;
				
				var Competences = await DbContext.LoadCompetences();
				var Technos = await DbContext.LoadTechnos();

				CompetenceTechnoViews = Competences.Select(x => new CompetenceTechnoViewObject(x)).ToList()
												.Concat(Technos.Select(x => new CompetenceTechnoViewObject(x))).ToList();

				AllKeywords = CompetenceTechnoViews.Select(x => x.Nom).ToList();
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


		public async Task<IEnumerable<string>> SearchByKeyword(string motCle)
		{
			await Task.Delay(5);
			List<string> result = new List<string>();

			try
			{
				if (!string.IsNullOrEmpty(motCle))
					result = AllKeywords.Where(x => x.Contains(motCle, StringComparison.InvariantCultureIgnoreCase))
										.Select(x => x)
										.ToList();
			}
			catch (Exception ex)
			{
				Error($"Erreur sur la recherche de {motCle}", ex);
			}

			return result;
		}


		public async Task OnSelectKeyword(string value)
		{
			var whatSelected = CompetenceTechnoViews.FirstOrDefault(x => x.Nom == value);
			Filtres.Add(whatSelected);

			// Faire la recherche dans les consultants.
			await GetConsultantsByFiltre();

			//StateHasChanged.Invoke();
		}

		public async Task DeleteFiltre(uint id)
		{
			var whatSelected = Filtres.FirstOrDefault(x => x.Id == id);
			Filtres.Remove(whatSelected);

			// Faire la recherche dans les consultants.
			if(Filtres.Any())
				await GetConsultantsByFiltre();
			else
				Consultants = await DbContext.LoadConsultants();
			//StateHasChanged.Invoke();
		}

		#endregion

		#region Private Methods

		private async Task GetConsultantsByFiltre()
		{
			List<uint> idConsultantCompetence = new List<uint>();
			List<uint> idConsultantTechnos = new List<uint>();

			var idsComp = Filtres.Where(x => x.Type == TypeCompetenceTechno.Competence).Select(x => x.Id).ToList();
			var idsTechno = Filtres.Where(x => x.Type == TypeCompetenceTechno.Techno).Select(x => x.Id).ToList();

			if(idsComp.Any())
				idConsultantCompetence = await DbContext.GetConsultantByCompetence(idsComp);

			if (idsTechno.Any())
				idConsultantTechnos = await DbContext.GetConsultantByTechno(idsTechno);

			List<uint> result = idConsultantCompetence.Concat(idConsultantTechnos).Distinct().ToList();

			Consultants = await DbContext.LoadConsultants(result);
		}

		#endregion
	}
}
