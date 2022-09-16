using Hermes.Models;

namespace Hermes.ViewModels
{
    public interface ICompetenceViewModel
	{
		/// <summary>
		/// Liste de toutes les compétences.
		/// </summary>
		List<Competence> AllCompetence { get; }

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
		Func<Competence, bool> QuickFilter { get; }

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
