namespace Hermes.ViewModels
{
	public interface IConsultantViewModel
	{
		/// <summary>
		/// Indique que la page est en chargement.
		/// </summary>
		bool IsLoading { get; }

		/// <summary>
		/// Toutes les informations sur le consultant.
		/// </summary>
		ConsultantViewObject ConsultantView { get; }


		/// <summary>
		/// Charge les informations du consultant.
		/// </summary>
		/// <param name="idConsultant"></param>
		/// <returns></returns>
		Task LoadConsultant(uint idConsultant);
	}
}
