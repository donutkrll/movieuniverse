using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

public class FilmManager
{
    private string mysqlCon;
    public bool canUpdate = false;
    public event EventHandler<bool> UpdateCompleted;
    public FilmManager(string connectionString)
    {
        mysqlCon = connectionString;
    }

    public void AddFilm(string title, string genre, string producer, string actors, DateTime dateFilm, DateTime timeFilm, decimal ticketCost, int cinemaHall, TimeSpan duration, bool subtitles, Image image)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            MessageBox.Show("Будь ласка, введіть назву фільму.");
            return;
        }

        if (string.IsNullOrWhiteSpace(genre))
        {
            MessageBox.Show("Будь ласка, виберіть жанр фільму.");
            return;
        }

        if (string.IsNullOrWhiteSpace(producer))
        {
            MessageBox.Show("Будь ласка, введіть ім'я режисера.");
            return;
        }

        if (string.IsNullOrWhiteSpace(actors))
        {
            MessageBox.Show("Будь ласка, введіть ім'я акторів.");
            return;
        }

        if (dateFilm.Date < DateTime.Today)
        {
            MessageBox.Show("Дата сеансу не може бути меншою за поточну дату.");
            return;
        }

        DateTime currentTime = DateTime.Now;
        DateTime selectedDateTime = dateFilm.Date.Add(timeFilm.TimeOfDay);
        if (selectedDateTime < currentTime)
        {
            MessageBox.Show("Час проведення сеансу не може бути меншим за поточний час.");
            return;
        }

        if (image == null)
        {
            MessageBox.Show("Будь ласка завантажте зображення.");
            return;
        }

        byte[] imageData;
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
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
                sqlCommand.Parameters.AddWithValue("@title", title);
                sqlCommand.Parameters.AddWithValue("@datefilm", dateFilm);
                sqlCommand.Parameters.AddWithValue("@timefilm", timeFilm);
                sqlCommand.Parameters.AddWithValue("@ticketcost", ticketCost);
                sqlCommand.Parameters.AddWithValue("@cinemahall", cinemaHall);
                sqlCommand.Parameters.AddWithValue("@genre", genre);
                sqlCommand.Parameters.AddWithValue("@producer", producer);
                sqlCommand.Parameters.AddWithValue("@actors", actors);
                sqlCommand.Parameters.AddWithValue("@subtitles", subtitles);
                sqlCommand.Parameters.AddWithValue("@duration", duration);
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
    public void UpdateFilm(int selectedRecordId, string title, string genre, string producer, string actors, DateTime dateFilm, DateTime timeFilm, decimal ticketCost, int cinemaHall, TimeSpan duration, bool subtitles)
    {

        if (string.IsNullOrWhiteSpace(title))
        {
            MessageBox.Show("Будь ласка, введіть назву фільму.");
            return;
        }

        if (string.IsNullOrWhiteSpace(genre))
        {
            MessageBox.Show("Будь ласка, виберіть жанр фільму.");
            return;
        }

        if (string.IsNullOrWhiteSpace(producer))
        {
            MessageBox.Show("Будь ласка, введіть ім'я режисера.");
            return;
        }

        if (string.IsNullOrWhiteSpace(actors))
        {
            MessageBox.Show("Будь ласка, введіть ім'я акторів.");
            return;
        }

        if (dateFilm.Date < DateTime.Today)
        {
            MessageBox.Show("Дата сеансу не може бути меншою за поточну дату.");
            return;
        }

        DateTime currentTime = DateTime.Now;
        DateTime selectedDateTime = dateFilm.Date.Add(timeFilm.TimeOfDay);
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
                sqlCommand.Parameters.AddWithValue("@title", title);
                sqlCommand.Parameters.AddWithValue("@datefilm", dateFilm);
                sqlCommand.Parameters.AddWithValue("@timefilm", timeFilm);
                sqlCommand.Parameters.AddWithValue("@ticketcost", ticketCost);
                sqlCommand.Parameters.AddWithValue("@cinemahall", cinemaHall);
                sqlCommand.Parameters.AddWithValue("@genre", genre);
                sqlCommand.Parameters.AddWithValue("@producer", producer);
                sqlCommand.Parameters.AddWithValue("@actors", actors);
                sqlCommand.Parameters.AddWithValue("@subtitles", subtitles);
                sqlCommand.Parameters.AddWithValue("@duration", duration);
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
}