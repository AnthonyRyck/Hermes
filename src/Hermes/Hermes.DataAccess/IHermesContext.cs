using Hermes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hermes.DataAccess
{
	public interface IHermesContext
	{
		#region Toutes les méthodes Add des tables

		Task Add(Techno nouvelleTechno);

		#endregion

		#region Toutes les méthodes Update des tables

		Task Update(Techno techno);

		#endregion


		#region Technos Table

		Task<List<Techno>> LoadTechnos();

		#endregion
	}
}
