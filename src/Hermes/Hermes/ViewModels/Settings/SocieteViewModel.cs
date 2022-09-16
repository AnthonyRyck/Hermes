using Hermes.Components.Dialogs;
using Hermes.DataAccess;
using Hermes.Models;
using Hermes.ValidationModels;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace Hermes.ViewModels.Settings
{
	public class SocieteViewModel : BaseViewModel, ISocieteViewModel
	{
		private ReferencielValidation SocieteValidation;
		private readonly IDialogService DialogService;
		private EditContext EditContextValidation;

		public SocieteViewModel(IHermesContext contextHermes, ISnackbar snackbar, IDialogService dialogService)
			: base(contextHermes, snackbar)
		{
			DialogService = dialogService;
			AllData = new List<Societe>();
			IsLoading = true;
			InitValidation();
		}

		#region ICompetenceViewModel

		public List<Societe> AllData { get; set; }

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

		Func<Societe, bool> ISocieteViewModel.QuickFilter => societe =>
		{
			if (string.IsNullOrWhiteSpace(RechercheItem))
				return true;

			if (societe.Nom.Contains(RechercheItem, StringComparison.OrdinalIgnoreCase))
				return true;

			if ((societe.Commentaire ?? string.Empty).Contains(RechercheItem, StringComparison.OrdinalIgnoreCase))
				return true;

			return false;
		};

		public async Task Load()
		{
			try
			{
				AllData = await DbContext.LoadSocietes();
				IsLoading = false;
			}
			catch (Exception ex)
			{
				Error("Erreur sur le chargement des sociétés", "Erreur de chargement", ex);
			}
		}

		public async Task AddNew()
		{
			try
			{
				var result = await OpenDialog("Ajout", SocieteValidation);

				if (!result.Cancelled)
				{
					var newSociete = ((ReferencielValidation)result.Data).ToSociete();
					await DbContext.Add(newSociete);

					AllData.Add(newSociete);
					string message = $"Société {newSociete.Nom} ajoutée";
					Success(message, message);
				}
			}
			catch (Exception ex)
			{
				Error("Erreur sur l'ajout de la société", "Erreur d'ajout", ex);
			}

			InitValidation();
		}

		public async Task Edit(uint id)
		{
			Societe societeSelected = AllData.Find(t => t.IdSociete == id);
			ReferencielValidation cptToEdit = new ReferencielValidation()
			{
				Nom = societeSelected.Nom,
				Commentaire = societeSelected.Commentaire
			};
			EditContextValidation = new EditContext(cptToEdit);

			var result = await OpenDialog("Modification", cptToEdit);

			if (!result.Cancelled)
			{
				var resultValidation = (ReferencielValidation)result.Data;

				societeSelected.Nom = resultValidation.Nom;
				societeSelected.Commentaire = resultValidation.Commentaire;

				await DbContext.Update(societeSelected);
				string msg = $"Société {societeSelected.Nom} modifiée";
				Success(msg, msg);
			}
		}

		#endregion

		#region Private methods

		private void InitValidation()
		{
			SocieteValidation = new ReferencielValidation();
			EditContextValidation = new EditContext(SocieteValidation);
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
