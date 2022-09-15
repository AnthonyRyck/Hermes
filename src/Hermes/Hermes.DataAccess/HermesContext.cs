using Hermes.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hermes.DataAccess
{
	public class HermesContext : IHermesContext
	{
		private string ConnectionString;

		public HermesContext(string connectionString)
		{
			ConnectionString = connectionString;
		}

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
						allTechnos.Add(new Techno
						{
							Id = reader.GetUInt32(0),
							NomTech = reader.GetString(1),
							Commentaire = reader.GetString(2)
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

		#endregion
	}
}
