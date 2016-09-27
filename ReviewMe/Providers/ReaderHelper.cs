using System;
using System.Data;
using Newtonsoft.Json;
using Scheduler.Entities;

namespace Scheduler.Providers
{
    internal static class ReaderHelper
    {
        public static T GetColumnValue<T>(this IDataReader record, string columnName)
        {
            var value = record[columnName];
            if (value == null || value == DBNull.Value)
            {
                return default(T);
            }

            return (T)value;
        }

		public static Message ReadMessage(IDataReader dataReader)
		{
			var m = new Message
			{
				Id = dataReader.GetColumnValue<int>("Id"),
				DelayDateTime = dataReader.GetColumnValue<DateTime>("DelayDateTime"),
			};
			var mesageString = dataReader.GetColumnValue<string>("Message");

			BuildMesage(mesageString, m);

			return m;
		}


		private static void BuildMesage(string mesageString, Message m)
		{
			try
			{
				var message = JsonConvert.DeserializeObject<Message>(mesageString);

				m.Text = message.Text;
				m.Destination = message.Destination;
				m.Source = message.Source;
				m.Type = message.Type;
			}
			catch (JsonException e)
			{
				throw new Exception($"Invalid format for message with id = {m.Id}. Error: {e.Message}");
			}
		}
	}
	
}