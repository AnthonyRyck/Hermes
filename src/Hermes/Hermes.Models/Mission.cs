using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hermes.Models
{
	public class Mission
	{
		public uint Id { get; set; }
		public uint SocieteId { get; set; }
		public uint ConsultantId { get; set; }
		public DateTime DateDebut { get; set; }
		public DateTime? DateFin { get; set; }
		public string Commentaire { get; set; }
	}
}
