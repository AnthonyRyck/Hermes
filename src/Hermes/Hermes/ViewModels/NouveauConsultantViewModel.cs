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

		private List<Techno> AllTechnos;
		private List<Competence> AllCompetences;

		public NouveauConsultantViewModel(IHermesContext contextHermes, ISnackbar snackbar, NavigationManager navigation)
			: base(contextHermes, snackbar)
		{
			ValidationForm = new ConsultantValidation();
			EditContextValidation = new EditContext(ValidationForm);
			Nav = navigation;
			IsLoading = true;

			TechnoSelected = new List<Techno>();
			CompetencesSelected = new List<Competence>();
		}


		#region Implement INouveauConsultantViewModel

		public bool IsLoading { get; private set; }

		public ConsultantValidation ValidationForm { get; set; }

		public EditContext EditContextValidation { get; set; }

		public string UrlPhoto { get; private set; }

		public string FileName { get; private set; }

		public IEnumerable<string> Technos { get; private set; }

		public List<Techno> TechnoSelected { get; private set; }

		
		public IEnumerable<string> Competences { get; private set; }
		public List<Competence> CompetencesSelected { get; private set; }


		public async Task LoadDatas()
		{
			IsLoading = true;
			
			AllTechnos = await DbContext.LoadTechnos();
			Technos = new List<string>(AllTechnos.Select(x => x.NomTech).ToList());

			AllCompetences = await DbContext.LoadCompetences();
			Competences = new List<string>(AllCompetences.Select(x => x.Nom)).ToList();
			
			IsLoading = false;
			StateHasChanged.Invoke();
		}

		#region Ajout/Suppr Technos

		public async Task<IEnumerable<string>> SearchTechno(string value)
		{
			await Task.Delay(5);
			List<string> result = new List<string>();

			try
			{
				if (!string.IsNullOrEmpty(value))
					result = AllTechnos.Where(x => x.NomTech.Contains(value, StringComparison.InvariantCultureIgnoreCase))
										.Select(x => x.NomTech)
										.ToList();
			}
			catch (Exception ex)
			{
				Error($"Erreur sur la recherche de la techno {value}", ex);
			}
			
			return result;
		}

		public void OnSelectTechno(string value)
		{
			Techno technoSelected = AllTechnos.FirstOrDefault(x => x.NomTech == value);

			if (TechnoSelected.Contains(technoSelected))
			{
				DisplayWarning($"{value} est déjà sélectionné");
				return;
			}

			TechnoSelected.Add(technoSelected);
			StateHasChanged.Invoke();
		}

		public void DeleteTech(uint id)
		{
			TechnoSelected.RemoveAll(x => x.Id == id);
			StateHasChanged.Invoke();
		}

		#endregion

		#region Ajout/Suppr Compétences

		public async Task<IEnumerable<string>> SearchCompetence(string value)
		{
			await Task.Delay(5);
			List<string> result = new List<string>();

			try
			{
				if (!string.IsNullOrEmpty(value))
					result = AllCompetences.Where(x => x.Nom.Contains(value, StringComparison.InvariantCultureIgnoreCase))
										.Select(x => x.Nom)
										.ToList();
			}
			catch (Exception ex)
			{
				Error($"Erreur sur la recherche de la compétence {value}", ex);
			}

			return result;
		}

		public void OnSelectCompetence(string value)
		{
			Competence competenceSelected = AllCompetences.FirstOrDefault(x => x.Nom == value);

			if (CompetencesSelected.Contains(competenceSelected))
			{
				DisplayWarning($"{value} est déjà sélectionné");
				return;
			}

			CompetencesSelected.Add(competenceSelected);
			StateHasChanged.Invoke();
		}

		public void DeleteCompetence(uint id)
		{
			CompetencesSelected.RemoveAll(x => x.Id == id);
			StateHasChanged.Invoke();
		}

		#endregion

		#region Ajout de la photo
		
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
				Notification.Add("Le format de la photo doit être au format jpg, jpeg ou png", Severity.Error);
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

				// Sauvegarde des compétences et technos connues
				if (TechnoSelected.Any())
					await DbContext.AddTechnoToConsultant(newConsultant.Id, TechnoSelected.Select(x => x.Id).ToList());

				if (CompetencesSelected.Any())
					await DbContext.AddCompetenceToConsultant(newConsultant.Id, CompetencesSelected.Select(x => x.Id).ToList());

				if (DossierCompetence != null)
					await DbContext.UpdateDossierCompetence(newConsultant.Id, DossierCompetence, FileName, DateTime.Now);

				Success($"Consultant {newConsultant.Nom} {newConsultant.Prenom} ajouté");
				InitData();
			}
			catch (Exception ex)
			{
				Error("Erreur lors de l'ajout d'un consultant", ex);
			}
		}

		#endregion

		#region Ajout du dossier de compétence

		private byte[] DossierCompetence;

		public async Task UploadMiniCv(InputFileChangeEventArgs e)
		{
			IBrowserFile file = e.File;

			if (file.ContentType == ConstantesHermes.MIME_PDF)
			{
				if (file.Size < ConstantesHermes.MAX_SIZE_PHOTO)
				{
					var sourceStream = file.OpenReadStream(file.Size + 1000);
					DossierCompetence = new byte[file.Size];
					await sourceStream.ReadAsync(DossierCompetence, 0, (int)file.Size);
					FileName = file.Name; 

					StateHasChanged.Invoke();
				}
				else
				{
					Notification.Add("La taille du fichier PDF doit être inférieur à 10 Mo", Severity.Error);
				}
			}
			else
			{
				Notification.Clear();
				Notification.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
				Notification.Add("Le fichier doit être au format PDF", Severity.Info);
			}
		}
		
		#endregion

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