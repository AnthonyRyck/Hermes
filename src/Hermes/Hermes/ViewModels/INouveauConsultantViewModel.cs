using Hermes.ValidationModels;
using Microsoft.AspNetCore.Components.Forms;

namespace Hermes.ViewModels
{
    public interface INouveauConsultantViewModel
    {
		ConsultantValidation ValidationForm { get; set; }

		EditContext EditContextValidation { get; set; }
		
		string UrlPhoto { get; }

		Task UploadPhoto(InputFileChangeEventArgs e);

		Task Add();

		void Cancel();

		void SetStateHasChanged(Action changed);
	}
}
