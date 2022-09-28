namespace Hermes.Codes.ViewObjects
{
    public class ConsultantViewObject
    {
	    /// <summary>
        /// Information sur le consultant.
        /// </summary>
        public Consultant InfoConsultant { get; set; }

		/// <summary>
        /// Liste des technos qu'il maitrise.
        /// </summary>
        public List<Techno> Technos { get; set; }

        /// <summary>
        /// Liste des compétences qu'il maitrise.
        /// </summary>
        public List<Competence> Competences { get; set; }
    }
}
