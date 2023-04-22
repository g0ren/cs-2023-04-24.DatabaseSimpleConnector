using MySql.Data.MySqlClient;

namespace DataBaseTest
{

    class Program
    {
        // Credentials
        static string server = "db4free.net";
        static string database = "mikhail_db";
        static string userid = "mikhail";
        static string password = "S8nluvzu";

        static async Task<string[]> MakeQuery(string query)
        {
            using (MySqlDatabaseConnector connector =
                new MySqlDatabaseConnector(server, database, userid, password))
            {
                try
                {
                    string[] response = await connector.SendQuery(query);
                    return response;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return new string[] { };
            }
        }
        static async Task ListCharacters()
        {
            string query =
                @"SELECT c.NAME AS ""Character Name"", cc.NAME AS ""Class Name"" " +
                "FROM Characters AS c, CharactersClass AS cc " +
                "WHERE c.CharacterClassId = cc.Id;";
            string[] response = await MakeQuery(query);
            foreach (var line in response)
            {
                Console.WriteLine(line);
            }
        }

        static async Task AddCharacter(string name, string className)
        {
            string[] currentClass = await MakeQuery(
                "SELECT Id " +
                "FROM CharactersClass " +
                @"WHERE Name = """ + className + @""";"
                );
            if (currentClass.Length == 0)
            {
                _ = await MakeQuery("INSERT INTO CharactersClass(Name) " +
                    @"VALUES (""" + className + @""");"
                    );
                currentClass = await MakeQuery(
                "SELECT Id " +
                "FROM CharactersClass " +
                @"WHERE Name = """ + className + @""";"
                );
            }
            _ = await MakeQuery(
                "INSERT INTO Characters(Name, CharacterClassId) " +
                @"VALUES (""" +name + @""", " + $"{currentClass[0]});"
                );
            
        }

        public static async Task Main()
        {
            int command = 0;

            while (true)
            {
                Console.WriteLine("Enter command:");
                Console.WriteLine("1. List current characters and their classes");
                Console.WriteLine("2. Add a character");
                Console.WriteLine("0. Quit");

                command = Convert.ToInt32(Console.ReadLine());
                switch (command)
                {
                    case 1:
                        await ListCharacters();
                        break;
                    case 2:
                        Console.WriteLine("Enter the character name");
                        string name = Console.ReadLine();
                        Console.WriteLine("Enter the character class");
                        string className = Console.ReadLine();
                        await AddCharacter(name, className);
                        break;
                    case 0:
                        return;
                }
            }

        }
    }
}