using Hermes.Models;
using MySqlConnector;

namespace Hermes.DataAccess
{
	public class HermesContext : IHermesContext
	{
		private string ConnectionString;

		public HermesContext(string connectionString)
		{
			ConnectionString = connectionString;
		}

		#region Creation/update des tables

		/// <summary>
		/// Permet de créer les tables pour le blog
		/// </summary>
		/// <returns></returns>
		public async Task CreateTablesAsync(string pathSql)
		{
			try
			{
				string cmd = await File.ReadAllTextAsync(pathSql);
				await ExecuteCoreAsync(cmd);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Permet d'éxécuter un script pour mettre à jour la base de donnée.
		/// </summary>
		/// <param name="pathSql"></param>
		/// <returns></returns>
		public async Task UpdateDatabaseAsync(string pathSql)
		{
			try
			{
				string cmd = await File.ReadAllTextAsync(pathSql);
				await ExecuteCoreAsync(cmd);
			}
			catch (Exception)
			{
				throw;
			}
		}
		
		#endregion

		#region Technos Table

		/// <summary>
		/// Permet de charger toutes les technos.
		/// </summary>
		/// <returns></returns>
		public async Task<List<Techno>> LoadTechnos()
		{
			var commandText = @"SELECT id, nom, commentaire "
				 + "FROM technos;";

			Func<MySqlCommand, Task<List<Techno>>> funcCmd = async (cmd) =>
			{
				List<Techno> allTechnos = new List<Techno>();
				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						object tempContent = reader.GetValue(2);

						allTechnos.Add(new Techno
						{
							Id = reader.GetUInt32(0),
							NomTech = reader.GetString(1),
							Commentaire = ConvertFromDBVal<string>(tempContent)
						});
					}
				}

				return allTechnos;
			};

			List<Techno> allTechs = new List<Techno>();
			try
			{
				allTechs = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return allTechs;
		}

		/// <summary>
		/// Ajout d'une nouvelle techno
		/// </summary>
		/// <param name="newTechno"></param>
		/// <returns>Retourne l'ID de la techno</returns>
		public async Task Add(Techno newTechno)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					string command = "INSERT INTO technos (nom, commentaire) VALUES (@nom, @commentaire);";

					using (var cmd = new MySqlCommand(command, conn))
					{
						cmd.Parameters.AddWithValue("@nom", newTechno.NomTech);
						cmd.Parameters.AddWithValue("@commentaire", newTechno.Commentaire);

						conn.Open();
						await cmd.ExecuteNonQueryAsync();
						newTechno.Id = await GetLastIdAsync(cmd);
						conn.Close();
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Permet de mettre à jour une ligne d'une Techno.
		/// </summary>
		/// <param name="techno"></param>
		/// <returns></returns>
		public async Task Update(Techno techno)
		{
			using (var conn = new MySqlConnection(ConnectionString))
			{
				var commandUpdateCompetence = @$"UPDATE technos SET nom=@nom, commentaire=@commentaire"
									  + $" WHERE id={techno.Id};";

				using (var cmd = new MySqlCommand(commandUpdateCompetence, conn))
				{
					cmd.Parameters.AddWithValue("@nom", techno.NomTech);
					cmd.Parameters.AddWithValue("@commentaire", techno.Commentaire);

					conn.Open();
					await cmd.ExecuteNonQueryAsync();
					conn.Close();
				}
			}
		}

		/// <summary>
		/// Charge les informations des technos demandées
		/// </summary>
		/// <param name="idsTechnos"></param>
		/// <returns></returns>
		public async Task<List<Techno>> GetTechnos(List<uint> idsTechnos)
		{
			string commandText = string.Empty;

			// Aucun Techno dans la liste
			if (idsTechnos.Count == 0)
			{
				return new List<Techno>();
			}

			if (idsTechnos.Count > 1)
			{
				string ids = string.Join(",", idsTechnos);

				commandText = @"SELECT id, nom, commentaire "
				 + "FROM technos "
				 + $"WHERE id IN ({ids});";
			}
			else
			{
				commandText = @"SELECT id, nom, commentaire "
				 + "FROM technos "
				 + $"WHERE id = {idsTechnos[0]};";
			}

			Func<MySqlCommand, Task<List<Techno>>> funcCmd = async (cmd) =>
			{
				List<Techno> technos = new List<Techno>();
				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						technos
						.Add(new Techno
						{
							Id = reader.GetUInt32(0),
							NomTech = reader.GetString(1),
							Commentaire = reader.GetString(2)
						});
					}
				}

				return technos;
			};

			List<Techno> technos = new List<Techno>();
			try
			{
				technos = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return technos;
		}

		#endregion

		#region Competences Table

		public async Task<List<Competence>> LoadCompetences()
		{
			var commandText = @"SELECT id, nom, commentaire "
				 + "FROM competences;";

			Func<MySqlCommand, Task<List<Competence>>> funcCmd = async (cmd) =>
			{
				List<Competence> allCompetences = new List<Competence>();
				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						object tempContent = reader.GetValue(2);

						allCompetences.Add(new Competence
						{
							Id = reader.GetUInt32(0),
							Nom = reader.GetString(1),
							Commentaire = ConvertFromDBVal<string>(tempContent)
						});
					}
				}

				return allCompetences;
			};

			List<Competence> allCompetences = new List<Competence>();
			try
			{
				allCompetences = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return allCompetences;
		}

		public async Task Add(Competence newCompetence)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					string command = "INSERT INTO competences (nom, commentaire) VALUES (@nom, @commentaire);";

					using (var cmd = new MySqlCommand(command, conn))
					{
						cmd.Parameters.AddWithValue("@nom", newCompetence.Nom);
						cmd.Parameters.AddWithValue("@commentaire", newCompetence.Commentaire);

						conn.Open();
						await cmd.ExecuteNonQueryAsync();
						newCompetence.Id = await GetLastIdAsync(cmd);
						conn.Close();
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task Update(Competence competence)
		{
			using (var conn = new MySqlConnection(ConnectionString))
			{
				var commandUpdateCompetence = @$"UPDATE competences SET nom=@nom, commentaire=@commentaire"
									  + $" WHERE id={competence.Id};";

				using (var cmd = new MySqlCommand(commandUpdateCompetence, conn))
				{
					cmd.Parameters.AddWithValue("@nom", competence.Nom);
					cmd.Parameters.AddWithValue("@commentaire", competence.Commentaire);

					conn.Open();
					await cmd.ExecuteNonQueryAsync();
					conn.Close();
				}
			}
		}

		public async Task<List<Competence>> GetCompetences(List<uint> idsCompetences)
		{
			string commandText = string.Empty;

			// Aucune compétence dans la liste
			if (idsCompetences.Count == 0)
			{
				return new List<Competence>();
			}

			if (idsCompetences.Count > 1)
			{
				string ids = string.Join(",", idsCompetences);

				commandText = @"SELECT id, nom, commentaire "
				 + "FROM competences "
				 + $"WHERE id IN ({ids});";
			}
			else
			{
				commandText = @"SELECT id, nom, commentaire "
				 + "FROM competences "
				 + $"WHERE id = {idsCompetences[0]};";
			}

			Func<MySqlCommand, Task<List<Competence>>> funcCmd = async (cmd) =>
			{
				List<Competence> competences = new List<Competence>();
				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						competences
						.Add(new Competence
						{
							Id = reader.GetUInt32(0),
							Nom = reader.GetString(1),
							Commentaire = reader.GetString(2)
						});
					}
				}

				return competences;
			};

			List<Competence> competences = new List<Competence>();
			try
			{
				competences = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return competences;
		}

		public async Task<List<uint>> GetCompetencesByIdConsultant(uint idConsultant)
		{
			
		}
		
		#endregion

		#region Societes Table

		public async Task<List<Societe>> LoadSocietes()
		{
			var commandText = @"SELECT id, nom, commentaire "
				 + "FROM societes;";

			Func<MySqlCommand, Task<List<Societe>>> funcCmd = async (cmd) =>
			{
				List<Societe> allSocietes = new List<Societe>();
				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						object tempContent = reader.GetValue(2);

						allSocietes.Add(new Societe
						{
							IdSociete = reader.GetUInt32(0),
							Nom = reader.GetString(1),
							Commentaire = ConvertFromDBVal<string>(tempContent)
						});
					}
				}

				return allSocietes;
			};

			List<Societe> allSocietes = new List<Societe>();
			try
			{
				allSocietes = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return allSocietes;
		}

		public async Task Add(Societe newSociete)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					string command = "INSERT INTO societes (nom, commentaire) VALUES (@nom, @commentaire);";

					using (var cmd = new MySqlCommand(command, conn))
					{
						cmd.Parameters.AddWithValue("@nom", newSociete.Nom);
						cmd.Parameters.AddWithValue("@commentaire", newSociete.Commentaire);

						conn.Open();
						await cmd.ExecuteNonQueryAsync();
						newSociete.IdSociete = await GetLastIdAsync(cmd);
						conn.Close();
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task Update(Societe newSociete)
		{
			using (var conn = new MySqlConnection(ConnectionString))
			{
				var commandUpdateCompetence = @$"UPDATE societes SET nom=@nom, commentaire=@commentaire"
									  + $" WHERE id={newSociete.IdSociete};";

				using (var cmd = new MySqlCommand(commandUpdateCompetence, conn))
				{
					cmd.Parameters.AddWithValue("@nom", newSociete.Nom);
					cmd.Parameters.AddWithValue("@commentaire", newSociete.Commentaire);

					conn.Open();
					await cmd.ExecuteNonQueryAsync();
					conn.Close();
				}
			}
		}

		#endregion

		#region Consultants Table

		public async Task<List<Consultant>> LoadConsultants()
		{
			var commandText = @"SELECT id, nom, prenom, urlphoto "
			 + "FROM consultants;";

			Func<MySqlCommand, Task<List<Consultant>>> funcCmd = async (cmd) =>
			{
				List<Consultant> allConsultants = new List<Consultant>();
				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						allConsultants.Add(new Consultant
						{
							Id = reader.GetUInt32(0),
							Nom = reader.GetString(1),
							Prenom = reader.GetString(2),
							UrlPhoto = ConvertFromDBVal<string>(reader.GetValue(3))
						});
					}
				}

				return allConsultants;
			};

			List<Consultant> allConsultants = new List<Consultant>();
			try
			{
				allConsultants = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return allConsultants;
		}

		public async Task<List<Consultant>> LoadConsultants(List<uint> idConsultants)
		{
			string commandText = string.Empty;

			// Aucun consultant ne répond aux critères, on retourne une liste vide
			if (idConsultants.Count == 0)
			{
				return new List<Consultant>();
			}
			
			if(idConsultants.Count > 1)
			{
				string ids = string.Join(",", idConsultants);

				commandText = @"SELECT id, nom, prenom, urlphoto "
				 + "FROM consultants "
				 + $"WHERE id IN ({ids});";
			}
			else
			{
				commandText = @"SELECT id, nom, prenom, urlphoto "
				 + "FROM consultants "
				 + $"WHERE id = {idConsultants[0]};";
			}

			Func<MySqlCommand, Task<List<Consultant>>> funcCmd = async (cmd) =>
			{
				List<Consultant> allConsultants = new List<Consultant>();
				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						allConsultants.Add(new Consultant
						{
							Id = reader.GetUInt32(0),
							Nom = reader.GetString(1),
							Prenom = reader.GetString(2),
							UrlPhoto = ConvertFromDBVal<string>(reader.GetValue(3))
						});
					}
				}

				return allConsultants;
			};

			List<Consultant> allConsultants = new List<Consultant>();
			try
			{
				allConsultants = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return allConsultants;
		}

		public async Task Add(Consultant consultant)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					string command = "INSERT INTO consultants (nom, prenom, urlphoto, minicv) "
					+ "VALUES (@nom, @prenom, @urlphoto, @minicv);";

					using (var cmd = new MySqlCommand(command, conn))
					{
						cmd.Parameters.AddWithValue("@nom", consultant.Nom);
						cmd.Parameters.AddWithValue("@prenom", consultant.Prenom);
						cmd.Parameters.AddWithValue("@urlphoto", consultant.UrlPhoto);
						cmd.Parameters.AddWithValue("@minicv", consultant.MiniCv);

						conn.Open();
						await cmd.ExecuteNonQueryAsync();
						consultant.Id = await GetLastIdAsync(cmd);
						conn.Close();
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}		

		public async Task UpdatePhotoConsultant(uint id, string urlPhoto)
		{
			using (var conn = new MySqlConnection(ConnectionString))
			{
				var commandUpdateCompetence = @$"UPDATE consultants SET urlphoto=@urlphoto"
									  + $" WHERE id={id};";

				using (var cmd = new MySqlCommand(commandUpdateCompetence, conn))
				{
					cmd.Parameters.AddWithValue("@urlphoto", urlPhoto);

					conn.Open();
					await cmd.ExecuteNonQueryAsync();
					conn.Close();
				}
			}
		}


		public async Task AddTechnoToConsultant(uint idConsultant, IEnumerable<uint> listTechnos)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					string command = "INSERT INTO consultanttechnos (idconsultant, idtechno) "
					+ "VALUES (@consultant, @tech);";

					using (var cmd = new MySqlCommand(command, conn))
					{
						conn.Open();
						
						foreach (var tech in listTechnos)
						{
							if (cmd.Parameters.Count > 0)
							{
								cmd.Parameters.Clear();
							}

							cmd.Parameters.AddWithValue("@consultant", idConsultant);
							cmd.Parameters.AddWithValue("@tech", tech);

							await cmd.ExecuteNonQueryAsync();
						}
						
						conn.Close();
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task AddCompetenceToConsultant(uint idConsultant, IEnumerable<uint> listCompetences)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					string command = "INSERT INTO consultantcompetences (idconsultant, idcompetence) "
					+ "VALUES (@consultant, @comp);";

					using (var cmd = new MySqlCommand(command, conn))
					{
						cmd.Parameters.AddWithValue("@consultant", idConsultant);

						conn.Open();
						
						foreach (var comp in listCompetences)
						{
							if (cmd.Parameters.Count > 0)
							{
								cmd.Parameters.Clear();
							}

							cmd.Parameters.AddWithValue("@consultant", idConsultant);
							cmd.Parameters.AddWithValue("@comp", comp);

							await cmd.ExecuteNonQueryAsync();
						}
						
						conn.Close();
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task<List<uint>> GetConsultantByCompetence(IEnumerable<uint> idsComp)
		{
			string ids = string.Join(",", idsComp);

			string command = "SELECT idconsultant "
			+ "FROM consultantcompetences "
			+ $"WHERE idcompetence IN ({ids});";

			Func<MySqlCommand, Task<List<uint>>> funcCmd = async (cmd) =>
			{
				List<uint> consultantsWithCompetence = new List<uint>();
				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						consultantsWithCompetence.Add(reader.GetUInt32(0));
					}
				}

				return consultantsWithCompetence;
			};

			List<uint> consultantsWithCompetence = new List<uint>();
			try
			{
				consultantsWithCompetence = await GetCoreAsync(command, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return consultantsWithCompetence;
		}

		public async Task<List<uint>> GetConsultantByTechno(IEnumerable<uint> idsTechno)
		{
			string ids = string.Join(",", idsTechno);

			string command = "SELECT idconsultant "
			+ "FROM consultanttechnos "
			+ $"WHERE idtechno IN ({ids});";

			Func<MySqlCommand, Task<List<uint>>> funcCmd = async (cmd) =>
			{
				List<uint> consultantsWithTechno = new List<uint>();
				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						consultantsWithTechno.Add(reader.GetUInt32(0));
					}
				}

				return consultantsWithTechno;
			};

			List<uint> consultantsWithTechno = new List<uint>();
			try
			{
				consultantsWithTechno = await GetCoreAsync(command, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return consultantsWithTechno;
		}

		public async Task<Consultant> GetConsultant(uint id)
		{
			var commandText = @"SELECT id, nom, prenom, urlphoto "
			 + $"FROM consultants WHERE id={id};";

			Func<MySqlCommand, Task<Consultant>> funcCmd = async (cmd) =>
			{
				Consultant consultantSelected = new Consultant();
				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						consultantSelected = new Consultant
						{
							Id = reader.GetUInt32(0),
							Nom = reader.GetString(1),
							Prenom = reader.GetString(2),
							UrlPhoto = ConvertFromDBVal<string>(reader.GetValue(3))
						};
					}
				}

				return consultantSelected;
			};

			Consultant consultantSelected = new Consultant();
			try
			{
				consultantSelected = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return consultantSelected;
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Execute une commande qui n'attend pas de retour.
		/// </summary>
		/// <param name="commandSql"></param>
		/// <returns></returns>
		private async Task<int> ExecuteCoreAsync(string commandSql)
		{
			using (var conn = new MySqlConnection(ConnectionString))
			{
				MySqlCommand cmd = new MySqlCommand(commandSql, conn);

				conn.Open();
				return await cmd.ExecuteNonQueryAsync();
			}
		}

		/// <summary>
		/// Execute une commande avec le retour passé.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="commandSql"></param>
		/// <param name="func"></param>
		/// <returns></returns>
		private async Task<T> GetCoreAsync<T>(string commandSql, Func<MySqlCommand, Task<T>> func)
			where T : new()
		{
			T result = new T();

			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					MySqlCommand cmd = new MySqlCommand(commandSql, conn);
					conn.Open();
					result = await func.Invoke(cmd);
				}
			}
			catch (Exception)
			{
				throw;
			}

			return result;
		}

		/// <summary>
		/// Permet de gérer les retours de valeur null de la BDD
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		private static T ConvertFromDBVal<T>(object obj)
		{
			if (obj == null || obj == DBNull.Value)
			{
				return default(T);
			}
			else
			{
				return (T)obj;
			}
		}

		/// <summary>
		/// Pour récupérer le dernier ID inséré.
		/// </summary>
		/// <param name="mySqlCommand"></param>
		/// <returns></returns>
		private async Task<uint> GetLastIdAsync(MySqlCommand mySqlCommand)
		{
			uint id = 0;
			try
			{
				string commandId = "SELECT LAST_INSERT_ID();";
				mySqlCommand.CommandText = commandId;
				id = Convert.ToUInt32(await mySqlCommand.ExecuteScalarAsync());
			}
			catch (Exception)
			{
				throw;
			}

			return id;
		}


		
		

		



		#endregion
	}
}
