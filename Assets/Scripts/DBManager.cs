using UnityEngine;
using MySql.Data.MySqlClient;
using System;

public static class DBManager
{
    private static string connStr = "server=localhost;user=root;database=game_db;port=3306;password=2545";

    public static UserData GetUserData(string email, string password)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                string sql = "SELECT * FROM users WHERE Email = @email AND Password = @password";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@password", password);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserData
                        {
                            email = reader["Email"].ToString(),
                            name = reader["Name"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Database error: {ex.Message}");
            }
        }
        return null;
    }
} 