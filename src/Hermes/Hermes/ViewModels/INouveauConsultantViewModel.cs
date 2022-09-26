using Hermes.ValidationModels;
using Microsoft.AspNetCore.Components.Forms;

namespace Hermes.ViewModels
{
    public interface INouveauConsultantViewModel
    {
		bool IsLoading { get; }
		
		ConsultantValidation ValidationForm { get; set; }

		EditContext EditContextValidation { get; set; }

		
		IEnumerable<string> Technos { get; }

		List<Techno> TechnoSelected { get; }
		Task<IEnumerable<string>> SearchTechno(string value);

		void OnSelectTechno(string value);
		void DeleteTech(uint id);
		


		IEnumerable<string> Competences { get; }

		List<Competence> CompetencesSelected { get; }

		Task<IEnumerable<string>> SearchCompetence(string value);
		void OnSelectCompetence(string value);

		void DeleteCompetence(uint id);

		string UrlPhoto { get; }

		Task LoadDatas();

		Task UploadPhoto(InputFileChangeEventArgs e);

		Task Add();

		void Cancel();

		void SetStateHasChanged(Action changed);



	}
}
