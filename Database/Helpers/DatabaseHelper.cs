using System.Data.SQLite;

namespace EnsekTechTest.Database.Helpers
{
    public static class DatabaseHelper
    {
        private static string databaseFileLocation = @"..\..\EnsekTechTest\Database\EnsekTestDb.db";
        private static string connectionString = $"Data source={databaseFileLocation};Version=3;";

        public static void InitializeDatabase()
        {
            if (!File.Exists(databaseFileLocation))
            {
                SQLiteConnection.CreateFile(databaseFileLocation);

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string createMeters = @"
                        CREATE TABLE [Accounts] (
                            [AccountId] bigint NOT NULL,
                            [FirstName] text NOT NULL,
                            [LastName] text NOT NULL,
                            CONSTRAINT [sqlite_master_PK_Accounts] PRIMARY KEY ([AccountId])
                        );";

                    string createAccounts = @"
                        CREATE TABLE [Meters] (
                            [MeterId] INTEGER PRIMARY KEY AUTOINCREMENT,
                            [AccountId] bigint NOT NULL,
                            [LastMeterReadingTime] text NOT NULL,
                            [MeterReadValue] bigint NOT NULL,
                            CONSTRAINT [FK_Meters_Accounts] FOREIGN KEY ([AccountId]) REFERENCES [Accounts]([AccountId]) ON DELETE CASCADE
                        );";

                    string insertAccounts = @"
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(1234,'Freya','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(1239,'Noddy','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(1240,'Archie','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(1241,'Lara','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(1242,'Tim','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(1243,'Graham','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(1244,'Tony','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(1245,'Neville','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(1246,'Jo','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(1247,'Jim','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(1248,'Pam','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(2233,'Barry','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(2344,'Tommy','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(2345,'Jerry','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(2346,'Ollie','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(2347,'Tara','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(2348,'Tammy','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(2349,'Simon','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(2350,'Colin','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(2351,'Gladys','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(2352,'Greg','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(2353,'Tony','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(2355,'Arthur','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(2356,'Craig','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(4534,'JOSH','TEST');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(6776,'Laura','Test');
                        INSERT INTO [Accounts]([AccountId],[FirstName],[LastName])VALUES(8766,'Sally','Test');
                        ";

                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = createMeters;
                        command.ExecuteNonQuery();

                        command.CommandText = createAccounts;
                        command.ExecuteNonQuery();

                        command.CommandText = insertAccounts;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
