using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Alsedi28
{
	public static class AspSqlProceduresUpdater
	{
		/// <summary>
		/// Updates all user stored procedures.
		/// </summary>
		/// <remarks>
		/// Removes all procedures and creates all over again.
		/// You can see which procedures will be deleted by performing a query in the desired database:
		/// SELECT [name] FROM sysobjects WHERE [type] = 'P' AND category = 0
		/// </remarks>
		/// <param name="сonnectionString">
		/// Database connection string. Example: "Server=localhost\SQLEXPRESS;Database=DBConnection;Trusted_Connection=True;".
		/// </param>
		/// <param name="pathToProcedureFolder">
		/// The absolute path to the root directory of procedures. All files with .sql extension will be taken and executed.
		/// You can use HttpContext.Current.Server.MapPath with relative path for build absolute path.
		/// </param>
		public static void UpdateProcedures(string сonnectionString, string pathToProcedureFolder)
		{
			if (string.IsNullOrEmpty(сonnectionString))
				throw new ArgumentNullException(nameof(сonnectionString));

			if (string.IsNullOrEmpty(pathToProcedureFolder))
				throw new ArgumentNullException(nameof(pathToProcedureFolder));

			using (var connection = new SqlConnection(сonnectionString))
			{
				connection.Open();

				DropProcedures(connection);

				List<string> procedures = GetAllProcedures(pathToProcedureFolder);

				foreach (var procedure in procedures)
					CreateProcedure(connection, procedure);

				connection.Close();
			}
		}

		/// <summary>
		/// Removes all user stored procedures.
		/// </summary>
		private static void DropProcedures(SqlConnection connection)
		{
			string dropAllNonSystemStoredProceduresQuery = @"
				DECLARE @name VARCHAR(128)
				DECLARE @SQL VARCHAR(254)

				SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'P' AND category = 0 ORDER BY [name])

				WHILE @name is not null
				BEGIN
					SELECT @SQL = 'DROP PROCEDURE [dbo].[' + RTRIM(@name) +']'
					EXEC (@SQL)
					SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'P' AND category = 0 AND [name] > @name ORDER BY [name])
				END";

			using (var command = new SqlCommand(dropAllNonSystemStoredProceduresQuery, connection))
			{
				command.CommandType = CommandType.Text;

				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					string message = ex.InnerException?.Message ?? ex.Message;
					throw new Exception($"Fatal error: drop all non system stored procedures. Message: {message}");
				}
			}
		}


		/// <summary>
		/// Create new user stored procedures.
		/// </summary>
		private static void CreateProcedure(SqlConnection connection, string procedure)
		{
			using (var command = new SqlCommand(procedure, connection))
			{
				command.CommandType = CommandType.Text;

				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					string message = ex.InnerException?.Message ?? ex.Message;
					throw new Exception($"Fatal error: create new stored procedures. Message: {message}");
				}
			}
		}

		/// <summary>
		/// Get the list of user stored procedures.
		/// </summary>
		private static List<string> GetAllProcedures(string pathToFolder)
		{
			string[] filesNames;

			try
			{
				filesNames = Directory.GetFiles(pathToFolder, "*.sql", SearchOption.AllDirectories);
			}
			catch (DirectoryNotFoundException ex)
			{
				throw new Exception($"Fatal error: get files from directory : '{pathToFolder}'. Message: {ex.Message}.");
			}

			var procedures = new List<string>(filesNames.Length);

			for (int i = 0; i < filesNames.Length; i++)
				procedures.Add(File.ReadAllText(filesNames[i]));

			return procedures;
		}
	}
}