using movieuniverse;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

public class FilmManager
{
    private string mysqlCon;
    public bool canUpdate = false;
    public event EventHandler<bool> UpdateCompleted;
    public FilmManager(string connectionString)
    {
        mysqlCon = connectionString;
    }

    public void AddFilm(Movie movie)
    {
        if (string.IsNullOrWhiteSpace(movie.title))
        {
            MessageBox.Show("Будь ласка, введіть назву фільму.");
            return;
        }

        if (string.IsNullOrWhiteSpace(movie.genre))
        {
            MessageBox.Show("Будь ласка, виберіть жанр фільму.");
            return;
        }

        if (string.IsNullOrWhiteSpace(movie.producer))
        {
            MessageBox.Show("Будь ласка, введіть ім'я режисера.");
            return;
        }

        if (string.IsNullOrWhiteSpace(movie.actors))
        {
            MessageBox.Show("Будь ласка, введіть ім'я акторів.");
            return;
        }

        if (movie.dateFilm.Date < DateTime.Today)
        {
            MessageBox.Show("Дата сеансу не може бути меншою за поточну дату.");
            return;
        }

        DateTime currentTime = DateTime.Now;
        DateTime selectedDateTime = movie.dateFilm.Date.Add(movie.timeFilm.TimeOfDay);
        if (selectedDateTime < currentTime)
        {
            MessageBox.Show("Час проведення сеансу не може бути меншим за поточний час.");
            return;
        }

        if (movie.image == null)
        {
            MessageBox.Show("Будь ласка завантажте зображення.");
            return;
        }

        byte[] imageData;
        using (MemoryStream ms = new MemoryStream())
        {
            movie.image.Save(ms, movie.image.RawFormat);
            imageData = ms.ToArray();
        }

        try
        {
            using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
            {
                mySqlConnection.Open();
                string query = "INSERT INTO AfishaFilm (title, datefilm, timefilm, ticketcost, cinemahall, genre, producer, actors, subtitles, duration, image) " +
                                "VALUES (@title, @datefilm, @timefilm, @ticketcost, @cinemahall, @genre, @producer, @actors, @subtitles, @duration, @image)";
                MySqlCommand sqlCommand = new MySqlCommand(query, mySqlConnection);
                sqlCommand.Parameters.AddWithValue("@title", movie.title);
                sqlCommand.Parameters.AddWithValue("@datefilm", movie.dateFilm);
                sqlCommand.Parameters.AddWithValue("@timefilm", movie.timeFilm);
                sqlCommand.Parameters.AddWithValue("@ticketcost", movie.ticketCost);
                sqlCommand.Parameters.AddWithValue("@cinemahall", movie.cinemaHall);
                sqlCommand.Parameters.AddWithValue("@genre", movie.genre);
                sqlCommand.Parameters.AddWithValue("@producer", movie.producer);
                sqlCommand.Parameters.AddWithValue("@actors", movie.actors);
                sqlCommand.Parameters.AddWithValue("@subtitles", movie.subtitles);
                sqlCommand.Parameters.AddWithValue("@duration", movie.duration);
                sqlCommand.Parameters.AddWithValue("@image", imageData);
                sqlCommand.ExecuteNonQuery();
                MessageBox.Show("Дані успішно додані в базу даних.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при додаванні даних в базу даних: " + ex.Message);
        }
    }
    public void UpdateFilm(int selectedRecordId, Movie movie)
    {

        if (string.IsNullOrWhiteSpace(movie.title))
        {
            MessageBox.Show("Будь ласка, введіть назву фільму.");
            return;
        }

        if (string.IsNullOrWhiteSpace(movie.genre))
        {
            MessageBox.Show("Будь ласка, виберіть жанр фільму.");
            return;
        }

        if (string.IsNullOrWhiteSpace(movie.producer))
        {
            MessageBox.Show("Будь ласка, введіть ім'я режисера.");
            return;
        }

        if (string.IsNullOrWhiteSpace(movie.actors))
        {
            MessageBox.Show("Будь ласка, введіть ім'я акторів.");
            return;
        }

        if (movie.dateFilm.Date < DateTime.Today)
        {
            MessageBox.Show("Дата сеансу не може бути меншою за поточну дату.");
            return;
        }

        DateTime currentTime = DateTime.Now;
        DateTime selectedDateTime = movie.dateFilm.Date.Add(movie.timeFilm.TimeOfDay);
        if (selectedDateTime < currentTime)
        {
            MessageBox.Show("Час проведення сеансу не може бути меншим за поточний час.");
            return;
        }
        DateTime currentDateTime = DateTime.Now;
        if (selectedDateTime <= currentDateTime)
        {
            MessageBox.Show("Сеанс уже идет, поэтому обновление невозможно.");
            return;
        }

        try
        {
            using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
            {
                mySqlConnection.Open();
                string query = "UPDATE AfishaFilm SET title = @title, datefilm = @datefilm, timefilm = @timefilm, ticketcost = @ticketcost, " +
                                "cinemahall = @cinemahall, genre = @genre, producer = @producer, actors = @actors, subtitles = @subtitles, duration = @duration " +
                                "WHERE id = @id";
                MySqlCommand sqlCommand = new MySqlCommand(query, mySqlConnection);
                sqlCommand.Parameters.AddWithValue("@id", selectedRecordId);
                sqlCommand.Parameters.AddWithValue("@title", movie.title);
                sqlCommand.Parameters.AddWithValue("@datefilm", movie.dateFilm);
                sqlCommand.Parameters.AddWithValue("@timefilm", movie.timeFilm);
                sqlCommand.Parameters.AddWithValue("@ticketcost", movie.ticketCost);
                sqlCommand.Parameters.AddWithValue("@cinemahall", movie.cinemaHall);
                sqlCommand.Parameters.AddWithValue("@genre", movie.genre);
                sqlCommand.Parameters.AddWithValue("@producer", movie.producer);
                sqlCommand.Parameters.AddWithValue("@actors", movie.actors);
                sqlCommand.Parameters.AddWithValue("@subtitles", movie.subtitles);
                sqlCommand.Parameters.AddWithValue("@duration", movie.duration);
                sqlCommand.ExecuteNonQuery();
                MessageBox.Show("Дані успішно оновлені в базі даних.");
                canUpdate = true;
                UpdateCompleted?.Invoke(this, canUpdate);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при оновленні запису в базі даних: " + ex.Message);
            return;
        }
    }
    public void DeleteFilm(int selectedRecordId)
    {
        try
        {
            if (selectedRecordId != 0)
            {
                string deleteQuery = $"DELETE FROM AfishaFilm WHERE id = {selectedRecordId}";
                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    MySqlCommand sqlCommand = new MySqlCommand(deleteQuery, mySqlConnection);
                    mySqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }

                MessageBox.Show("Запис успішно видалений.");
            }
            else
            {
                MessageBox.Show("Оберіть запис для видалення.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при видаленні запису: " + ex.Message);
        }
    }
    public void SortMovies(string sortOrder, string sortBy, string titleFilter, string dateFilter, Control flowLayoutPanel, DataLoader dataLoader, BookingManager bookingManager, string userLogin)
    {
        try
        {
            string query = $"SELECT cinemahall, genre, producer, actors, title, datefilm, timefilm, ticketcost, image, duration " +
                    $"FROM AfishaFilm " +
                    $"WHERE 1=1 {titleFilter} {dateFilter} " +
                    $"ORDER BY {sortBy} {sortOrder}";

            using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
            {
                MySqlCommand sqlCommand = new MySqlCommand(query, mySqlConnection);
                mySqlConnection.Open();
                using (MySqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    dataLoader.UpdateMoviePanels(reader, (FlowLayoutPanel)flowLayoutPanel, bookingManager, userLogin);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка: " + ex.Message);
        }
    }

}