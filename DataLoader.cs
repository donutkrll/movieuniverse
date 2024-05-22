using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace movieuniverse
{
    public class DataLoader
    {
        private string mysqlCon = "server=localhost; user=root; database=filmuniverse; password=babych612"; // Потрібно визначити з'єднання з базою даних

        public DataTable LoadData()
        {
            try
            {
                MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
                string query = "SELECT id, title AS 'Назва', datefilm AS 'Дата сеансу', timefilm AS 'Час сеансу', ticketcost AS 'Ціна білету', " +
                    "cinemahall AS 'Номер залу', genre AS 'Жанр', duration AS 'Тривалість', producer AS 'Режисер', " +
                    "actors AS 'Актори', subtitles AS 'Субтітри' FROM AfishaFilm";

                MySqlCommand sqlCommand = new MySqlCommand(query, mySqlConnection);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                return dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
                return null;
            }
        }

        public void DeletePastSessions()
        {
            try
            {
                DateTime thresholdTime = DateTime.Now.AddHours(-3);

                string deleteQuery = $"DELETE FROM AfishaFilm WHERE (datefilm < CURDATE()) OR (datefilm = CURDATE() AND timefilm < @thresholdTime)";

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    MySqlCommand sqlCommand = new MySqlCommand(deleteQuery, mySqlConnection);
                    sqlCommand.Parameters.Add(new MySqlParameter("@thresholdTime", thresholdTime.ToString("HH:mm:ss")));

                    mySqlConnection.Open();
                    int rowsAffected = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при видаленні минувших сеансів: " + ex.Message);
            }
        }

    }
}
