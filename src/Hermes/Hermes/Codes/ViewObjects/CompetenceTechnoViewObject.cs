namespace Hermes.Codes.ViewObjects
{
	public class CompetenceTechnoViewObject
	{
		public uint Id { get; set; }
		public string Nom { get; set; }
		public string Commentaire { get; set; }

		public TypeCompetenceTechno Type { get; set; }

		public CompetenceTechnoViewObject() {}

		public CompetenceTechnoViewObject(Techno techno)
		{
			Id = techno.Id;
			Nom = techno.NomTech;
			Commentaire = techno.Commentaire;
			Type = TypeCompetenceTechno.Techno;
		}

		public CompetenceTechnoViewObject(Competence competence)
		{
			Id = competence.Id;
			Nom = competence.Nom;
			Commentaire = competence.Commentaire;
			Type = TypeCompetenceTechno.Competence;
		}
	}


	public enum TypeCompetenceTechno
	{
		Competence,
		Techno
	}
}
