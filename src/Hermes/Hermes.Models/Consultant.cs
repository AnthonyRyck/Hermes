using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hermes.Models
{
	public class Consultant
	{
		public uint Id { get; set; }
		public string Nom { get; set; }
		public string Prenom { get; set; }
		public byte[] Photo { get; set; }

		public byte[] MiniCv { get; set; }
	}
}
