using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace Hermes.ValidationModels
{
    public class ConsultantValidation
    {
		[Required(ErrorMessage = "Il faut un nom")]
		[MaxLength(100, ErrorMessage = "Maximum 100 caractères")]
		public string Nom { get; set; }

		[Required(ErrorMessage = "Il faut un prénom")]
		[MaxLength(100, ErrorMessage = "Maximum 100 caractères")]
		public string Prenom { get; set; }

		public string UrlImage { get; set; }

		public IBrowserFile Photo { get; set; }
		public string ExtensionFile { get; set; }

		public byte[] MiniCv { get; set; }
	}

	public static class ConsultantValidationExtension
	{
		public static Consultant ToConsultant(this ConsultantValidation source)
		{
			return new Consultant()
			{
				Nom = source.Nom,
				Prenom = source.Prenom,
				MiniCv = source.MiniCv,
				UrlPhoto = source.UrlImage
			};
		}
	}
}
