using Hermes.Models;
using System.ComponentModel.DataAnnotations;

namespace Hermes.ValidationModels
{
    public class CompetenceValidation
    {
		[Required(ErrorMessage = "Il faut un nom de compétence")]
		[MaxLength(100, ErrorMessage = "Maximum 100 caractères")]
		public string Nom { get; set; }

		[MaxLength(250, ErrorMessage = "Maximum 250 caractères")]
		public string Commentaire { get; set; }
	}

	public static class CompetenceValidationExtension
	{
		public static Competence ToCompetence(this CompetenceValidation competence)
		{
			return new Competence
			{
				Nom = competence.Nom,
				Commentaire = competence.Commentaire
			};
		}
	}
}
