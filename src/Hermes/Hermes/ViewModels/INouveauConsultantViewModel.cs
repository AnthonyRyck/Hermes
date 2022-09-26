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

		IEnumerable<Competence> Competences { get; }

		void OnSelectTechno(string value);

		string UrlPhoto { get; }

		Task LoadDatas();

		Task UploadPhoto(InputFileChangeEventArgs e);

		Task Add();

		void Cancel();

		void SetStateHasChanged(Action changed);


		Task<IEnumerable<string>> SearchTechno(string value);

		void DeleteTech(uint id);
	}
}
