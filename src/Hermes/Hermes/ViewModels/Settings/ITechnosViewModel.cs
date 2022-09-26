namespace Hermes.ViewModels.Settings
{
    public interface ITechnosViewModel
    {
        /// <summary>
        /// Liste de toutes les technos.
        /// </summary>
        List<Techno> AllTechnos { get; }

        /// <summary>
        /// Indicateur que la page se charge.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Pour faire une recherche
        /// </summary>
		string RechercheItem { get; set; }

		Func<Techno, bool> QuickFilter { get; }

		/// <summary>
		/// Charge le tableau.
		/// </summary>
		/// <returns></returns>
		Task LoadTechnos();

        /// <summary>
        /// Permet d'ouvrir un dialog pour ajouter une nouvelle techno.
        /// </summary>
        /// <returns></returns>
        Task AddNewTechno();

        /// <summary>
        /// Permet de modifier une ligne.
        /// </summary>
        /// <param name="idTecho"></param>
        /// <returns></returns>
        Task Edit(uint idTecho);

    }
}
