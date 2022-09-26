using Hermes.Pages;
using Hermes.ValidationModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Xml.Linq;

namespace Hermes.ViewModels
{
	public class NouveauConsultantViewModel : BaseViewModel, INouveauConsultantViewModel
	{
		private readonly NavigationManager Nav;
		private Action StateHasChanged;

		public NouveauConsultantViewModel(IHermesContext contextHermes, ISnackbar snackbar, NavigationManager navigation) 
			: base(contextHermes, snackbar)
		{
			ValidationForm = new ConsultantValidation();
			EditContextValidation = new EditContext(ValidationForm);
			Nav = navigation;
		}


		#region Implement INouveauConsultantViewModel

		public ConsultantValidation ValidationForm { get; set; }

		public EditContext EditContextValidation { get; set; }

		public string UrlPhoto { get; private set; }

		public async Task UploadPhoto(InputFileChangeEventArgs e)
		{
			IBrowserFile file = e.File;

			if (file.ContentType == ConstantesHermes.MIME_JPEG
				|| file.ContentType == ConstantesHermes.MIME_PNG
				|| file.ContentType == ConstantesHermes.MIME_JPG)
			{
				if (file.Size < ConstantesHermes.MAX_SIZE_PHOTO)
				{
					UrlPhoto = await SaveTempPhoto(file);
					StateHasChanged.Invoke();
				}
				else
				{
					Notification.Add("La taille de la photo doit être inférieur à 10 Mo", Severity.Error);
				}
			}
			else
			{
				Notification.Clear();
				Notification.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
				Notification.Add("Je suis là", Severity.Info);
			}
		}

		public async Task Add()
		{
			try
			{
				if (!EditContextValidation.Validate())
					return;

				var newConsultant = ValidationForm.ToConsultant();
				await DbContext.Add(newConsultant);

				// Sauvegarder la photo en fonction de l'ID du consultant
				// Renommer la photo.
				newConsultant.UrlPhoto = RenameTempPhoto(newConsultant.Id);
				await DbContext.UpdatePhotoConsultant(newConsultant.Id, newConsultant.UrlPhoto);

				Success($"Consultant {newConsultant.Nom} {newConsultant.Prenom} ajouté");
				InitData();
			}
			catch (Exception ex)
			{
				Error("Erreur lors de l'ajout d'un consultant", ex);
			}
		}


		public void Cancel()
		{
			// Dans la cas ou pas encore de photo temporaire de mise.
			DeleteTempPhotoSiBesoin(guidTempPhoto.ToString());
			
			ValidationForm = new ConsultantValidation();
			Nav.NavigateTo($"/consultants", true);
		}

		public void SetStateHasChanged(Action changed)
		{
			StateHasChanged = changed;
		}

		#endregion

		#region Private methods

		private Guid guidTempPhoto;

		private void InitData()
		{
			UrlPhoto = String.Empty;
			ValidationForm = new ConsultantValidation();
			EditContextValidation = new EditContext(ValidationForm);
			guidTempPhoto = new Guid();
		}

		private async Task<string> SaveTempPhoto(IBrowserFile photo)
		{
			string pathImg = string.Empty;

			DeleteTempPhotoSiBesoin(guidTempPhoto.ToString());
			guidTempPhoto = Guid.NewGuid();

			string tempName = guidTempPhoto.ToString();
			string extensionPhoto = Path.GetExtension(photo.Name);

			if (photo != null)
			{
				var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesHermes.IMAGES, ConstantesHermes.IMG_CONSULTANTS, $"{tempName}{extensionPhoto}");
				using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
				{
					await photo.OpenReadStream(photo.Size + 1000).CopyToAsync(fileStream);
				}

				pathImg = ConstantesHermes.SetPathImageConsultants($"{tempName}{extensionPhoto}");
			}

			return pathImg;
		}


		private string RenameTempPhoto(uint id)
		{
			string extension = Path.GetExtension(UrlPhoto);
			string name = Path.GetFileNameWithoutExtension(UrlPhoto);
			
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesHermes.IMAGES, ConstantesHermes.IMG_CONSULTANTS, $"{name}{extension}");

			string nouveauNom = path.Replace(name, id.ToString());

			FileInfo file = new FileInfo(path);
			file.MoveTo(nouveauNom);

			return ConstantesHermes.SetPathImageConsultants($"{id}{extension}");
		}

		private void DeleteTempPhotoSiBesoin(string guidTemp)
		{
			// Dans la cas ou pas encore de photo temporaire de mise.
			if (guidTempPhoto.ToString() != new Guid().ToString())
			{
				var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesHermes.IMAGES, ConstantesHermes.IMG_CONSULTANTS);
				DirectoryInfo recherche = new DirectoryInfo(path);
				var fileToDelete = recherche.GetFiles().Where(x => x.Name.Contains(guidTemp)).FirstOrDefault();
	
				fileToDelete?.Delete();
			}
		}

		#endregion

	}
}