using Hermes.Models;
using System.ComponentModel.DataAnnotations;

namespace Hermes.ValidationModels
{
	public class ReferencielValidation
	{
		[Required(ErrorMessage = "Il faut un Nom")]
		[MaxLength(100, ErrorMessage = "Maximum 100 caractères")]
		public string Nom { get; set; }

		[MaxLength(250, ErrorMessage = "Maximum 250 caractères")]
		public string Commentaire { get; set; }
	}

	public static class ReferencielValidationExtension
	{
		public static Competence ToCompetence(this ReferencielValidation competence)
		{
			return new Competence
			{
				Nom = competence.Nom,
				Commentaire = competence.Commentaire
			};
		}

		public static Techno ToTechno(this ReferencielValidation techno)
		{
			return new Techno
			{
				NomTech = techno.Nom,
				Commentaire = techno.Commentaire
			};
		}

		public static Societe ToSociete(this ReferencielValidation societe)
		{
			return new Societe
			{
				Nom = societe.Nom,
				Commentaire = societe.Commentaire
			};
		}
	}
}
