using Hermes.Codes.ViewObjects;

namespace Hermes.ViewModels
{
    public interface IConsultantViewModel
    {
		/// <summary>
		/// Indicateur si la page est chargée.
		/// </summary>
		bool IsLoading { get; }

		List<Consultant> Consultants { get; }

		List<CompetenceTechnoViewObject> Filtres { get; }

		/// <summary>
		/// Charge les consultants.
		/// </summary>
		/// <returns></returns>
		Task LoadConsultants();

		/// <summary>
		/// Ouvre une nouvelle fenêtre pour ajouter un consultant.
		/// </summary>
		/// <returns></returns>
		Task AddConsultant();

		void OpenPageConsultant(int idConsultant);

		Task<IEnumerable<string>> SearchByKeyword(string motCle);

		Task OnSelectKeyword(string value);

		Task DeleteFiltre(uint id);
	}
}
