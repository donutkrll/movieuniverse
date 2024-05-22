using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

public class BookingManager
{
    private Dictionary<string, bool[]> bookingStatus = new Dictionary<string, bool[]>();
    private string bookingFilePath = @"D:\KHAI\kursova\bookingStatus.csv";

    public decimal GetTicketPriceFromDB(string title, DateTime sessionTime, string hallNumber)
    {
        decimal ticketPrice = 0;
        string mysqlCon = "server=localhost; user=root; database=filmuniverse; password=babych612";
        string query = "SELECT ticketcost FROM AfishaFilm WHERE title = @title AND CONCAT(DATE_FORMAT(datefilm, '%Y-%m-%d'), ' ', DATE_FORMAT(timefilm, '%H:%i')) = @sessionTime AND cinemahall = @hallNumber";

        using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
        {
            MySqlCommand command = new MySqlCommand(query, mySqlConnection);
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@sessionTime", sessionTime.ToString("yyyy-MM-dd HH:mm"));
            command.Parameters.AddWithValue("@hallNumber", hallNumber);

            try
            {
                mySqlConnection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ticketPrice = reader.GetDecimal("ticketcost");
                    }
                    else
                    {
                        Console.WriteLine("Запис не знайдено.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
        return ticketPrice;
    }

    public decimal GetUserWallet(string userLogin)
    {
        decimal userWallet = 0;
        string mysqlCon = "server=localhost; user=root; database=filmuniverse; password=babych612";
        string query = "SELECT wallet FROM user WHERE login = @login";

        using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
        {
            MySqlCommand command = new MySqlCommand(query, mySqlConnection);
            command.Parameters.AddWithValue("@login", userLogin);

            try
            {
                mySqlConnection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("wallet")))
                        {
                            userWallet = reader.GetDecimal("wallet");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
        }

        return userWallet;
    }

    public void UpdateUserWallet(string userLogin, decimal newWalletValue)
    {
        string mysqlCon = "server=localhost; user=root; database=filmuniverse; password=babych612";
        string query = "UPDATE user SET wallet = @wallet WHERE login = @login";

        using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
        {
            MySqlCommand command = new MySqlCommand(query, mySqlConnection);
            command.Parameters.AddWithValue("@login", userLogin);
            command.Parameters.AddWithValue("@wallet", newWalletValue);

            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
        }
    }

    public void ShowBookingDialog(string title, string sessionTimeString, string hallNumber, string userLogin)
    {
        DateTime sessionTime;
        if (!DateTime.TryParseExact(sessionTimeString, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out sessionTime))
        {
            MessageBox.Show("Неверный формат даты и времени сеанса.");
            return;
        }
        string sessionId = $"{title}_{sessionTime}_{hallNumber}";

        Form bookingForm = new Form();
        bookingForm.Text = $"Бронювання місць - {title}";

        TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
        {
            RowCount = 5,
            ColumnCount = 15,
            Dock = DockStyle.Fill,
            AutoSize = true,
            Padding = new Padding(10),
        };

        if (!bookingStatus.ContainsKey(sessionId))
        {
            bookingStatus[sessionId] = new bool[5 * 15];
        }

        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 15; col++)
            {
                int seatNumber = row * 15 + col;
                System.Windows.Forms.Button seatButton = new System.Windows.Forms.Button
                {
                    Text = (seatNumber + 1).ToString(),
                    BackColor = bookingStatus[sessionId][seatNumber] ? Color.Red : Color.Green,
                    Width = 40,
                    Height = 40,
                    Margin = new Padding(5),
                };

                seatButton.Click += (sender, e) =>
                {
                    System.Windows.Forms.Button btn = (System.Windows.Forms.Button)sender;
                    int index = int.Parse(btn.Text) - 1;
                    if (btn.BackColor == Color.Green)
                    {
                        decimal ticketPrice = GetTicketPriceFromDB(title, sessionTime, hallNumber);
                        decimal userWallet = GetUserWallet(userLogin);
                        MessageBox.Show($"Баланс: {userWallet}, Ціна билету: {ticketPrice}");

                        if (userWallet >= ticketPrice)
                        {
                            UpdateUserWallet(userLogin, userWallet - ticketPrice);
                            btn.BackColor = Color.Red;
                            bookingStatus[sessionId][index] = true;
                            MessageBox.Show($"Місце {btn.Text} заброньовано.");
                            SaveBookingStatus();
                            CreateHTMLTicket(title, sessionTime, hallNumber, ticketPrice, index + 1, userLogin);
                        }
                        else
                        {
                            MessageBox.Show("На вашому рахунку недостатньо коштів для бронювання цього місця.");
                        }
                    }
                    else if (btn.BackColor == Color.Red)
                    {
                        MessageBox.Show($"Місце {btn.Text} вже зайняте.");
                    }
                };

                tableLayoutPanel.Controls.Add(seatButton, col, row);
            }
        }

        bookingForm.Controls.Add(tableLayoutPanel);
        bookingForm.AutoSize = true;
        bookingForm.StartPosition = FormStartPosition.CenterParent;
        bookingForm.ShowDialog();
    }

    private void SaveBookingStatus()
    {
        using (var writer = new StreamWriter(bookingFilePath, false))
        {
            foreach (var entry in bookingStatus)
            {
                writer.WriteLine($"{entry.Key},{string.Join(",", entry.Value.Select(b => b ? "1" : "0"))}");
            }
        }
    }

    public void LoadBookingStatus()
    {
        if (File.Exists(bookingFilePath))
        {
            using (var reader = new StreamReader(bookingFilePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var parts = line.Split(',');
                    var sessionId = parts[0];
                    var seats = parts.Skip(1).Select(p => p == "1").ToArray();
                    bookingStatus[sessionId] = seats;
                }
            }
        }
    }

    private void CreateHTMLTicket(string title, DateTime sessionTime, string hallNumber, decimal ticketPrice, int seatNumber, string userLogin)
    {
        string userName, userSurname, userEmail, userPhoneNumber;
        GetUserDetails(userLogin, out userName, out userSurname, out userEmail, out userPhoneNumber);

        StringBuilder htmlBuilder = new StringBuilder();
        htmlBuilder.AppendLine("<!DOCTYPE html>");
        htmlBuilder.AppendLine("<html lang=\"en\">");
        htmlBuilder.AppendLine("<head>");
        htmlBuilder.AppendLine("    <meta charset=\"UTF-8\">");
        htmlBuilder.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        htmlBuilder.AppendLine("    <title>Квиток</title>");
        htmlBuilder.AppendLine("</head>");
        htmlBuilder.AppendLine("<body>");
        htmlBuilder.AppendLine("    <table border=\"1\">");
        htmlBuilder.AppendLine("        <tr><th>Назва фільму</th><td>" + title + "</td></tr>");
        htmlBuilder.AppendLine("        <tr><th>Дата сеансу</th><td>" + sessionTime.ToString("dd.MM.yyyy") + "</td></tr>");
        htmlBuilder.AppendLine("        <tr><th>Час сеансу</th><td>" + sessionTime.ToString("HH:mm") + "</td></tr>");
        htmlBuilder.AppendLine("        <tr><th>Зала</th><td>" + hallNumber + "</td></tr>");
        htmlBuilder.AppendLine("        <tr><th>Ціна</th><td>" + ticketPrice.ToString() + "</td></tr>");
        htmlBuilder.AppendLine("        <tr><th>Номер сидіння</th><td>" + seatNumber + "</td></tr>");
        htmlBuilder.AppendLine("        <tr><th>Ім'я</th><td>" + userName + "</td></tr>");
        htmlBuilder.AppendLine("        <tr><th>Прізвище</th><td>" + userSurname + "</td></tr>");
        htmlBuilder.AppendLine("        <tr><th>Email</th><td>" + userEmail + "</td></tr>");
        htmlBuilder.AppendLine("        <tr><th>Телефон</th><td>" + userPhoneNumber + "</td></tr>");
        htmlBuilder.AppendLine("    </table>");
        htmlBuilder.AppendLine("</body>");
        htmlBuilder.AppendLine("</html>");

        File.WriteAllText(@"D:\KHAI\kursova\Ticket_" + userLogin + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".html", htmlBuilder.ToString());
    }

    private void GetUserDetails(string userLogin, out string name, out string surname, out string email, out string phoneNumber)
    {
        string mysqlCon = "server=localhost; user=root; database=filmuniverse; password=babych612";
        string query = "SELECT name, surname, login, email, phone_number, wallet FROM user WHERE login = @login";

        using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
        {
            MySqlCommand command = new MySqlCommand(query, mySqlConnection);
            command.Parameters.AddWithValue("@login", userLogin);

            try
            {
                mySqlConnection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        name = reader.GetString("name");
                        surname = reader.GetString("surname");
                        email = reader.GetString("email");
                        phoneNumber = reader.GetString("phone_number");
                    }
                    else
                    {
                        throw new Exception("Користувача не знайдено.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
                name = surname = email = phoneNumber = string.Empty;
            }
        }
    }
}
