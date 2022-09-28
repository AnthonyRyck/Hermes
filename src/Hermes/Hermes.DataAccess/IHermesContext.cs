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

		Task<List<Techno>> GetTechnos(List<uint> idsTechnos);

		Task<List<Techno>> GetTechnosByIdConsultant(uint idConsultant);

		#endregion

		#region Competences Table
		Task<List<Competence>> LoadCompetences();

		Task<List<Competence>> GetCompetences(List<uint> idsCompetences);

		Task<List<Competence>> GetCompetencesByIdConsultant(uint idConsultant);

		#endregion

		#region Societes Table

		Task<List<Societe>> LoadSocietes();

		#endregion

		#region Consultants Table

		Task<List<Consultant>> LoadConsultants();

		Task<List<Consultant>> LoadConsultants(List<uint> idConsultants);


		Task UpdatePhotoConsultant(uint id, string urlPhoto);
		
		Task AddTechnoToConsultant(uint id, IEnumerable<uint> list);
		Task AddCompetenceToConsultant(uint id, IEnumerable<uint> list);

		Task<List<uint>> GetConsultantByCompetence(IEnumerable<uint> idsComp);

		Task<List<uint>> GetConsultantByTechno(IEnumerable<uint> idsTechno);

		Task<Consultant> GetConsultant(uint id);

		#endregion
	}
}
