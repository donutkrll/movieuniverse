using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace movieuniverse
{

    public class DataLoader : IMoviePanelUpdater
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

        public void UpdateMoviePanels(MySqlDataReader reader, FlowLayoutPanel flowLayoutPanel, BookingManager bookingManager, string userLogin)
        {
            // Удалить только панели фильмов из FlowLayoutPanel
            for (int i = flowLayoutPanel.Controls.Count - 1; i >= 0; i--)
            {
                if (flowLayoutPanel.Controls[i].Tag != null && flowLayoutPanel.Controls[i].Tag.ToString() == "MoviePanel")
                {
                    flowLayoutPanel.Controls.RemoveAt(i);
                }
            }

            while (reader.Read())
            {
                Panel moviePanel = new Panel
                {
                    Tag = "MoviePanel",
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(5),
                    Width = flowLayoutPanel.Width - 25,
                    Padding = new Padding(5),
                    Height = 215,
                    BackColor = Color.FromArgb(228, 210, 148)
                };

                PictureBox pictureBox = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 150,
                    Height = 200,
                    Margin = new Padding(0, 0, 10, 0)
                };
                if (reader["image"] != DBNull.Value && ((byte[])reader["image"]).Length > 0)
                {
                    pictureBox.Image = ByteArrayToImage((byte[])reader["image"]); // Загружаем изображение из базы данных
                }
                else
                {
                    pictureBox.Image = null;
                }

                Label titleLabel = new Label
                {
                    Text = reader["title"].ToString(),
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    AutoSize = true,
                    Padding = new Padding(0, 0, 0, 10)
                };

                Label genreLabel = new Label
                {
                    Text = $"Жанр: {reader["genre"].ToString()}",
                    Font = new Font("Arial", 10),
                    AutoSize = true,
                    Padding = new Padding(0, 0, 0, 10)
                };

                Label hallLabel = new Label
                {
                    Text = $"Номер залу: {reader["cinemahall"].ToString()}",
                    Font = new Font("Arial", 10),
                    AutoSize = true,
                    Padding = new Padding(0, 0, 0, 10)
                };

                Label producerLabel = new Label
                {
                    Text = $"Режисер: {reader["producer"].ToString()}",
                    Font = new Font("Arial", 10),
                    AutoSize = true,
                    Padding = new Padding(0, 0, 0, 10)
                };

                Label actorsLabel = new Label
                {
                    Text = $"Актори: {reader["actors"].ToString()}",
                    Font = new Font("Arial", 10),
                    AutoSize = true,
                    Padding = new Padding(0, 0, 0, 10)
                };

                Label durationLabel = new Label
                {
                    Text = $"Тривалість: {reader.GetTimeSpan("duration").ToString(@"hh\:mm\:ss")}",
                    Font = new Font("Arial", 10),
                    AutoSize = true,
                    Padding = new Padding(0, 0, 0, 10),
                    TextAlign = ContentAlignment.BottomLeft
                };

                Label timeLabel = new Label
                {
                    Text = $"Дата проведення сеансу: {((DateTime)reader["datefilm"]).ToShortDateString()}, Час проведення: {((TimeSpan)reader["timefilm"]).ToString(@"hh\:mm")}",
                    Font = new Font("Arial", 10),
                    AutoSize = true,
                    Padding = new Padding(0, 0, 0, 10),
                    TextAlign = ContentAlignment.BottomLeft
                };

                Label priceLabel = new Label
                {
                    Text = $"Ціна квитка: {reader["ticketcost"].ToString()}",
                    Font = new Font("Arial", 10),
                    AutoSize = true,
                    Padding = new Padding(0, 0, 0, 5),
                    TextAlign = ContentAlignment.BottomRight,
                    Cursor = Cursors.Hand
                };

                string movieTitle = reader["title"].ToString();
                string sessionTime = $"{((DateTime)reader["datefilm"]).ToShortDateString()} {((TimeSpan)reader["timefilm"]).ToString(@"hh\:mm")}";
                string hallNumber = reader["cinemahall"].ToString();
                if(userLogin != "guest")
                {
                    priceLabel.Click += (sender, e) =>
                    {
                        bookingManager.ShowBookingDialog(movieTitle, sessionTime, hallNumber, userLogin);
                    };
                }

                TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
                {
                    ColumnCount = 2,
                    RowCount = 9,
                    Dock = DockStyle.Fill
                };
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                tableLayoutPanel.Controls.Add(pictureBox, 0, 0);
                tableLayoutPanel.SetRowSpan(pictureBox, 9);

                tableLayoutPanel.Controls.Add(titleLabel, 1, 0);
                tableLayoutPanel.Controls.Add(genreLabel, 1, 1);
                tableLayoutPanel.Controls.Add(hallLabel, 1, 2);
                tableLayoutPanel.Controls.Add(producerLabel, 1, 3);
                tableLayoutPanel.Controls.Add(actorsLabel, 1, 4);
                tableLayoutPanel.Controls.Add(durationLabel, 1, 5);
                tableLayoutPanel.Controls.Add(timeLabel, 1, 6);
                tableLayoutPanel.Controls.Add(priceLabel, 1, 7);

                durationLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                timeLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                priceLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                moviePanel.Controls.Add(tableLayoutPanel);

                flowLayoutPanel.Controls.Add(moviePanel);
            }
        }

        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (var ms = new System.IO.MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }
    }
}
