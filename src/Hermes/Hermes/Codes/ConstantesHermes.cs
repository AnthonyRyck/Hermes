using Hermes.Models;

namespace Hermes.Codes
{
	public static class ConstantesHermes
	{
		public const string IMAGES = "images";
		public const string IMG_CONSULTANTS = "consultantsimg";
		public const string REQUEST_PATH_IMG = "/consultantsimg";
		public const string NO_CONSULTANT_IMG = "noconsultant.png";

		/// <summary>
		/// Taille de 10 Mo en Bit
		/// </summary>
		public const int MAX_SIZE_PHOTO = 80000000;

		#region Extensions Image

		public const string EXTENSION_IMAGE_JPG = ".jpg";
		public const string EXTENSION_IMAGE_JPEG = ".jpeg";
		public const string EXTENSION_IMAGE_PNG = ".png";
		public const string EXTENSION_IMAGE_GIF = ".gif";
		public const string EXTENSION_IMAGE_BMP = ".bmp";
		public const string EXTENSION_IMAGE_ICO = ".ico";
		public const string EXTENSION_IMAGE_SVG = ".svg";

		#endregion

		#region MIME Types Images

		public const string MIME_JPG = "image/jpg";
		public const string MIME_JPEG = "image/jpeg";
		public const string MIME_GIF = "image/gif";
		public const string MIME_PNG = "image/png";

		#endregion

		#region Role pour les comptes

		public const string ROLE_ADMIN = "Admin";
		public const string ROLE_AUTEUR = "Manager";

		#endregion

		public static string SetPathImageConsultants(string fileName)
		{
			return REQUEST_PATH_IMG + "/" + $"{fileName}";
		}
	}
}
