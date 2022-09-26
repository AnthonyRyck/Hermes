using Hermes.ValidationModels;
using Microsoft.AspNetCore.Components.Forms;

namespace Hermes.ViewModels.Settings
{
	public class CompetenceViewModel : BaseViewModel, ICompetenceViewModel
	{
		private ReferencielValidation CompetenceValidation;
		private readonly IDialogService DialogService;
		private EditContext EditContextValidation;

		public CompetenceViewModel(IHermesContext contextHermes, ISnackbar snackbar, IDialogService dialogService)
			: base(contextHermes, snackbar)
		{
			DialogService = dialogService;
			AllCompetence = new List<Competence>();
			IsLoading = true;
			InitValidation();
		}

		#region ICompetenceViewModel

		public List<Competence> AllCompetence { get; set; }

		public bool IsLoading { get; private set; }

		public string RechercheItem { get; set; }


		public Func<Competence, bool> QuickFilter => cpt =>
		{
			if (string.IsNullOrWhiteSpace(RechercheItem))
				return true;

			if (cpt.Nom.Contains(RechercheItem, StringComparison.OrdinalIgnoreCase))
				return true;

			if ((cpt.Commentaire ?? string.Empty).Contains(RechercheItem, StringComparison.OrdinalIgnoreCase))
				return true;

			return false;
		};


		public async Task Load()
		{
			try
			{
				AllCompetence = await DbContext.LoadCompetences();
				IsLoading = false;
			}
			catch (Exception ex)
			{
				Error("Erreur sur le chargement des Competences", "Erreur de chargement", ex);
			}
		}

		public async Task AddNew()
		{
			try
			{
				var result = await OpenDialog("Ajout", CompetenceValidation);

				if (!result.Cancelled)
				{
					var newCompt = ((ReferencielValidation)result.Data).ToCompetence();
					await DbContext.Add(newCompt);

					AllCompetence.Add(newCompt);
					string message = $"Compétence {newCompt.Nom} ajoutée";
					Success(message, message);
				}
			}
			catch (Exception ex)
			{
				Error("Erreur sur l'ajout de la compétence", "Erreur d'ajout", ex);
			}

			InitValidation();
		}

		public async Task Edit(uint id)
		{
			Competence competenceSelected = AllCompetence.Find(t => t.Id == id);
			ReferencielValidation cptToEdit = new ReferencielValidation()
			{
				Nom = competenceSelected.Nom,
				Commentaire = competenceSelected.Commentaire
			};
			EditContextValidation = new EditContext(cptToEdit);

			var result = await OpenDialog("Modification", cptToEdit);

			if (!result.Cancelled)
			{
				var resultValidation = (ReferencielValidation)result.Data;

				competenceSelected.Nom = resultValidation.Nom;
				competenceSelected.Commentaire = resultValidation.Commentaire;

				await DbContext.Update(competenceSelected);
				string msg = $"Compétence {competenceSelected.Nom} modifiée";
				Success(msg, msg);
			}
		}

		#endregion

		#region Private methods

		private void InitValidation()
		{
			CompetenceValidation = new ReferencielValidation();
			EditContextValidation = new EditContext(CompetenceValidation);
		}

		private Task<DialogResult> OpenDialog(string titre, ReferencielValidation modelValidation)
		{
			var parameters = new DialogParameters();
			parameters.Add("ValidationForm", modelValidation);
			parameters.Add("EditContextValidation", EditContextValidation);

			var dialog = DialogService.Show<CompetenceDialog>(titre, parameters);
			return dialog.Result;
		}

		#endregion
	}
}
