using MySql.Data;
using MySql.Data.MySqlClient;

namespace App_Server
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }

    public class DB
    {
        private static MySqlConnection? conn;
        public static void Connect()
        {
            string connStr =
                "server=localhost;" +
                "user=root;" +
              //  "database=appDB;" +
                "port=3306;" +
                "password=admin";

            conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                Console.WriteLine("Connected to MySQL");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static void Close()
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
                Console.WriteLine("Disconnected from MySQL");
            }
        }

        public static void initDB()
        {
            if (conn == null || conn.State != System.Data.ConnectionState.Open)
            {
                Console.WriteLine("DB not connected");
            }
            else
            {
                string query = "CREATE DATABASE IF NOT EXISTS appDB";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                query = "USE appDB";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                query = "CREATE TABLE IF NOT EXISTS User (" +
                    "id INT AUTO_INCREMENT PRIMARY KEY," +
                    "firstName VARCHAR(50) NOT NULL," +
                    "lastName VARCHAR(50) NOT NULL," +
                    "email VARCHAR(50) NOT NULL UNIQUE," +
                    "password VARCHAR(255) NOT NULL," +
                    "picturePath VARCHAR(255) DEFAULT ''," +
                    "friends JSON," +
                    "location VARCHAR(255)," +
                    "occupation VARCHAR(255)," +
                    "viewedProfile INT DEFAULT 0," +
                    "impressions INT DEFAULT 0," +
                    "createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP)";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                query = "CREATE TABLE IF NOT EXISTS Post (" +
                    "id INT AUTO_INCREMENT PRIMARY KEY," +
                    "userId INT NOT NULL," +
                    "firstName VARCHAR(50) NOT NULL," +
                    "lastName VARCHAR(50) NOT NULL," +
                    "location VARCHAR(255)," +
                    "description TEXT," +
                    "picturePath VARCHAR(255)," +
                    "userPicturePath VARCHAR(255)," +
                    "likes JSON," +
                    "comments JSON," +
                    "timestamps BOOLEAN DEFAULT TRUE," +
                    "createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP," +
                    "FOREIGN KEY (userId) REFERENCES User(id) ON DELETE CASCADE)";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                Console.WriteLine("DB initialized");
            }
            }
    }
}