using UnityEngine;
using MySql.Data.MySqlClient;
using System;

public class DBManager : MonoBehaviour
{
    private static string server = "localhost";
    private static string database = "game_db";
    private static string uid = "root";
    private static string password = "2545";
    private static MySqlConnection connection;

    public static bool IsConnected()
    {
        return connection != null;
    }

    public static void Connect()
    {
        string connectionString = $"Server={server};Database={database};Uid={uid};Pwd={password};";

        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            Debug.Log("Successfully connected to database.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Cannot connect to database: {e.Message}");
        }
    }

    public static void Close()
    {
        if (connection != null)
        {
            connection.Close();
            connection = null;
        }
    }

    // ฟังก์ชันสำหรับ Login
    public static bool ValidateLogin(string email, string password)
    {
        if (connection == null)
        {
            Connect();
        }

        try
        {
            string query = "SELECT * FROM users WHERE Email=@email AND Password=@password";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                return reader.HasRows;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error validating login: {e.Message}");
            return false;
        }
    }
} 