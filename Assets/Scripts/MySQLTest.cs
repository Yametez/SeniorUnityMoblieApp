using MySql.Data.MySqlClient;
using UnityEngine;

public class MySQLTest : MonoBehaviour
{
    void Start()
    {
        string connStr = "server=localhost;user=root;database=game_db;port=3306;password=2545";
        MySqlConnection conn = new MySqlConnection(connStr);
        try
        {
            conn.Open();
            Debug.Log("Connection successful!");
        }
        catch (MySqlException ex)
        {
            Debug.LogError("Error: " + ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }
}
