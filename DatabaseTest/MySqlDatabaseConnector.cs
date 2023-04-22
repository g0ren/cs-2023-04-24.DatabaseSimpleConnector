using MySql.Data.MySqlClient;
using System.Text;

namespace DataBaseTest
{
    class MySqlDatabaseConnector : IDisposable
    {
        MySqlConnection? _connection;
        public MySqlDatabaseConnector(string server, string database, string userid, string password)
        {
            MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder()
            {
                Server = server,
                Database = database,
                UserID = userid,
                Password = password
            };

            _connection = new MySqlConnection(mySqlConnectionStringBuilder.ToString());
        }

        public async Task<string[]> SendQuery(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, _connection);
            string[] response = new string[] { };
            _connection?.OpenAsync();
            using (MySqlDataReader reader = await cmd.ExecuteReaderAsync() as MySqlDataReader)
            {
                StringBuilder builder = new StringBuilder();
                while (await reader.ReadAsync())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        builder.Append(reader.GetString(i));
                        builder.Append('\t');
                    }
                    response = response.Append(builder.ToString()).ToArray();
                    builder.Clear();
                }
            }
            _ = Close();
            return response;
        } 

        public async Task Close()
        {
            if (_connection != null)
                await _connection.CloseAsync();
        }

        public void Dispose()
        {
            _ = Close();
        }

    }
}