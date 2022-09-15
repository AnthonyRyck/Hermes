using Hermes.Models;
using System.ComponentModel.DataAnnotations;

namespace Hermes.ValidationModels
{
	public class TechnoValidation
	{
		[Required(ErrorMessage = "Il faut un nom de technologie")]
		[MaxLength(100, ErrorMessage = "Maximum 100 caractères")]
		public string Nom { get; set; }

		[MaxLength(250, ErrorMessage = "Maximum 250 caractères")]
		public string Commentaire { get; set; }
	}

	public static class TechnoValidationExtension
	{
		public static Techno ToValidation(this TechnoValidation techno)
		{
			return new Techno
			{
				NomTech = techno.Nom,
				Commentaire = techno.Commentaire
			};
		}
	}
}