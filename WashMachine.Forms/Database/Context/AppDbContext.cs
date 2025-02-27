using System.Data.SQLite;
using System.IO;
using WashMachine.Forms.Database.Tables.Machine;

namespace WashMachine.Forms.Database.Context
{
    public abstract class AppDbContext
    {
        protected virtual void Initial()
        {

        }

        protected SQLiteConnection CreateConnection()
        {
            EnsureDatabase();
            string dbPath = GetDbPath();
            string connectionString = $"DataSource={dbPath}";
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();
            return connection;
        }

        private string GetDbPath()
        {
            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "washmachine.db");
            return dbPath;
        }

        protected void EnsureDatabase()
        {
            string path = GetDbPath();
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Dispose();
                File.WriteAllBytes(path, new byte[0]);
            }
        }

        public static Machine Machine { get; set; } = new Machine();
    }
}
