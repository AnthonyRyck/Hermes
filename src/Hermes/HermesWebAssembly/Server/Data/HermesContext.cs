using MySql.Data.MySqlClient;

namespace HermesWebAssembly.Server.Data
{
	public class HermesContext
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

		#endregion
	}
}
