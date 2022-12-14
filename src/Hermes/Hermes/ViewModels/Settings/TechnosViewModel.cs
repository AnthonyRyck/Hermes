using Hermes.ValidationModels;
using Microsoft.AspNetCore.Components.Forms;

namespace Hermes.ViewModels.Settings
{
    public class TechnosViewModel : BaseViewModel, ITechnosViewModel
    {
        private ReferencielValidation TechnoValidation;
        private readonly IDialogService DialogService;
        private EditContext EditContextValidation;

        public TechnosViewModel(IHermesContext contextHermes, ISnackbar snackbar, IDialogService dialogService)
        : base(contextHermes, snackbar)
        {
            InitValidation();

            DialogService = dialogService;
            AllTechnos = new List<Techno>();
            IsLoading = true;
        }


        #region Implement ITechnoViewModel


        public List<Techno> AllTechnos { get; private set; }

        public bool IsLoading { get; private set; }

		public string RechercheItem { get; set; }

		public Func<Techno, bool> QuickFilter => tech =>
		{
			if (string.IsNullOrWhiteSpace(RechercheItem))
				return true;

			if (tech.NomTech.Contains(RechercheItem, StringComparison.OrdinalIgnoreCase))
				return true;

			if ((tech.Commentaire ?? string.Empty).Contains(RechercheItem, StringComparison.OrdinalIgnoreCase))
				return true;

			return false;
		};

		public async Task LoadTechnos()
        {
            try
            {
                AllTechnos = await DbContext.LoadTechnos();
                IsLoading = false;
            }
            catch (Exception ex)
            {
                Error("Erreur sur le chargement des Technos", "Erreur de chargement", ex);
            }
        }

        public async Task AddNewTechno()
        {
            try
            {
                var result = await OpenDialog("Ajout", TechnoValidation);

                if (!result.Cancelled)
                {
                    var newTechno = ((ReferencielValidation)result.Data).ToTechno();
                    await DbContext.Add(newTechno);

                    AllTechnos.Add(newTechno);
                    string message = $"Techno {newTechno.NomTech} ajoutée";
                    Success(message, message);
                }
            }
            catch (Exception ex)
            {
                Error("Erreur sur l'ajout de la techno", "Erreur d'ajout", ex);
            }

            InitValidation();
        }

        public async Task Edit(uint idTecho)
        {
            Techno technSelected = AllTechnos.Find(t => t.Id == idTecho);
            ReferencielValidation technoToEdit = new ReferencielValidation()
            {
                Nom = technSelected.NomTech,
                Commentaire = technSelected.Commentaire
            };
            EditContextValidation = new EditContext(technoToEdit);

            var result = await OpenDialog("Modification", technoToEdit);

            if (!result.Cancelled)
            {
                var resultValidation = (ReferencielValidation)result.Data;

                technSelected.NomTech = resultValidation.Nom;
                technSelected.Commentaire = resultValidation.Commentaire;

                await DbContext.Update(technSelected);
                Success($"Techno {technSelected.NomTech} modifiée", $"Techno {technSelected.NomTech} modifiée");
            }
        }

        #endregion

        #region Private methods

        private void InitValidation()
        {
            TechnoValidation = new ReferencielValidation();
            EditContextValidation = new EditContext(TechnoValidation);
        }

        private Task<DialogResult> OpenDialog(string titre, ReferencielValidation modelValidation)
        {
            var parameters = new DialogParameters();
            parameters.Add("TechnoForm", modelValidation);
            parameters.Add("EditContextValidation", EditContextValidation);

            var dialog = DialogService.Show<TechoDialog>(titre, parameters);
            return dialog.Result;
        }

        #endregion
    }
}
