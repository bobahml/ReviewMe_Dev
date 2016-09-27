using Scheduler.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Scheduler.Providers
{
	public interface IMessageDataProvider
	{
		void DeleteMessages(IEnumerable<Message> messages);
		ICollection<Message> GetMessagesByDate(DateTime delayDateTime, int count);
	}

	internal class MessageDataProvider : IMessageDataProvider
	{
		private readonly string _databaseConnection;

		public MessageDataProvider()
		{
			_databaseConnection = ConfigurationManager.ConnectionStrings["ForReview"].ConnectionString;
		}

		public ICollection<Message> GetMessagesByDate(DateTime delayDateTime, int count)
		{
			var list = new List<Message>();
			using (var sqlConnection = new SqlConnection(_databaseConnection))
			{
				using (var command = new SqlCommand("[dbo].[GetMessagesByDate]", sqlConnection)
				{
					CommandType = CommandType.StoredProcedure
				})
				{
					command.Parameters.Add(new SqlParameter("@maxDelayDateTime", delayDateTime));
					command.Parameters.Add(new SqlParameter("@count", count));
				
					sqlConnection.Open();
					using (var dr = command.ExecuteReader())
					{
						while (dr.Read() && list.Count < count)
						{
							var msg = ReaderHelper.ReadMessage(dr);
							list.Add(msg); 
						}
					}
					sqlConnection.Close();
				}
			}

			return list;
		}
		private static DataTable CreateIdDataTable(IEnumerable<int> ids)
		{
			var table = new DataTable();
			table.Columns.Add("id", typeof(int));
			foreach (var id in ids)
			{
				table.Rows.Add(id);
			}
			return table;
		}

		public void DeleteMessages(IEnumerable<Message> messages)
		{
			using (var sqlConnection = new SqlConnection(_databaseConnection))
			{
				using (var command = new SqlCommand("[dbo].[DeleteMessagesById]", sqlConnection)
				{
					CommandType = CommandType.StoredProcedure,
				})
				{
					var parameter = new SqlParameter("@ids", CreateIdDataTable(messages.Select(m => (int) m.Id)))
					{
						SqlDbType = SqlDbType.Structured,
						TypeName = "[dbo].[MessageIdsType]"
					};
					command.Parameters.Add(parameter);
					sqlConnection.Open();
					command.ExecuteNonQuery();
					sqlConnection.Close();
				}
			}
		}
	}
}
