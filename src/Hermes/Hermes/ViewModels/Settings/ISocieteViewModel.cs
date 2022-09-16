using Hermes.Models;

namespace Hermes.ViewModels.Settings
{
	public interface ISocieteViewModel
	{
		/// <summary>
		/// Liste de toutes les sociétés.
		/// </summary>
		List<Societe> AllData { get; }

		/// <summary>
		/// Indicateur que la page se charge.
		/// </summary>
		bool IsLoading { get; }

		/// <summary>
		/// Mot clé de recherche
		/// </summary>
		string RechercheItem { get; set; }

		/// <summary>
		/// Fonction pour la recherche
		/// </summary>
		Func<Societe, bool> QuickFilter { get; }

		/// <summary>
		/// Charge le tableau.
		/// </summary>
		/// <returns></returns>
		Task Load();

		/// <summary>
		/// Permet d'ouvrir un dialog pour ajouter une nouvelle ligne.
		/// </summary>
		/// <returns></returns>
		Task AddNew();

		/// <summary>
		/// Permet de modifier une ligne.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task Edit(uint id);
	}
}
