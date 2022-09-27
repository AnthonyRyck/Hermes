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

		Task Add(Competence competence);

		Task Add(Societe societe);

		Task Add(Consultant consultant);

		#endregion

		#region Toutes les méthodes Update des tables

		Task Update(Techno techno);

		Task Update(Competence competence);

		Task Update(Societe societe);

		#endregion


		#region Technos Table

		Task<List<Techno>> LoadTechnos();

		#endregion

		#region Competences Table
		Task<List<Competence>> LoadCompetences();

		#endregion

		#region Societes Table

		Task<List<Societe>> LoadSocietes();

		#endregion

		#region Consultants Table

		Task<List<Consultant>> LoadConsultants();
		
		Task UpdatePhotoConsultant(uint id, string urlPhoto);
		
		Task AddTechnoToConsultant(uint id, IEnumerable<uint> list);
		Task AddCompetenceToConsultant(uint id, IEnumerable<uint> list);

		#endregion
	}
}
