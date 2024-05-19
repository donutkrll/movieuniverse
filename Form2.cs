using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace movieuniverse
{
    public partial class Form2 : Form
    {
        public bool sortingpanelCreated;
        public Panel groupPanel;
        public Panel groupPanel2;
        public Panel groupPanel3;
        public Panel groupPanel4;
        public Panel groupPanel5;
        public FlowLayoutPanel flowLayoutPanel;
        public PictureBox pictureBox;
        private byte[] imageData;
        public System.Windows.Forms.ComboBox sortComboBox;
        public RadioButton ascendingRadioButton;
        public RadioButton descendingRadioButton;
        string mysqlCon = "server=localhost; user=root; database=filmuniverse; password=babych612";
        public string query;

        private string userLogin;
        public Form2()
        {
            InitializeComponent();
            LoadData();
            LoadBookingStatus();
            groupPanel = panel1;
            groupPanel2 = panel2;
            groupPanel3 = panel3;
            groupPanel4 = panel4;
            groupPanel5 = panel5;
            flowLayoutPanel = flowLayoutPanel1;
            pictureBox = pictureBox1;
            sortComboBox = new System.Windows.Forms.ComboBox();
            ascendingRadioButton = new RadioButton();
            descendingRadioButton = new RadioButton();
            sortingpanelCreated = false;
            query = "SELECT cinemahall, genre, title, datefilm, timefilm, ticketcost, image, duration FROM AfishaFilm ORDER BY datefilm, timefilm ASC";

            додаванняToolStripMenuItem.Click += додаванняToolStripMenuItem_Click;
            редагуванняToolStripMenuItem.Click += редагуванняToolStripMenuItem_Click;
            переглядСеансівToolStripMenuItem.Click += переглядСеансівToolStripMenuItem_Click;
            переглядЮзерівToolStripMenuItem.Click += переглядЮзерівToolStripMenuItem_Click;
            видаленняToolStripMenuItem.Click += видаленняToolStripMenuItem_Click;
            особистийКабінетToolStripMenuItem.Click += особистийКабінетToolStripMenuItem_Click;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            dataGridView1.CellClick += dataGridView1_CellClick;
            sortComboBox.SelectedIndexChanged += SortOptionChanged;
            ascendingRadioButton.CheckedChanged += SortOptionChanged;
            descendingRadioButton.CheckedChanged += SortOptionChanged;

            flowLayoutPanel.Margin = new Padding(0, 0, 0, 27);

        }
        public void SetUserLogin(string login)
        {
            userLogin = login;
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            if (userLogin != "admin")
            {
                додаванняToolStripMenuItem.Visible = false;
                редагуванняToolStripMenuItem.Visible = false;
                видаленняToolStripMenuItem.Visible = false;
                переглядЮзерівToolStripMenuItem.Visible = false;
                DeletePastSessions();
            }
        }
        private void LoadData()
        {
            try
            {
                MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
                string query = "SELECT id, title AS 'Назва', datefilm AS 'Дата сеансу', timefilm AS 'Час сеансу', ticketcost AS 'Ціна білету', " +
               "cinemahall AS 'Номер залу', genre AS 'Жанр', duration AS 'Тривалість', producer AS 'Режисер', " +
               "actors AS 'Актори', subtitles AS 'Субтітри' FROM AfishaFilm";

                MySqlCommand sqlCommand = new MySqlCommand(query, mySqlConnection);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCommand);
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataAdapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
                dataGridView2.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.ReadOnly = true;
            }
        }
        private void LoadUserData()
        {
            try
            {
                MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
                string query = "SELECT login AS 'Логін', email AS 'Електронна пошта', " +
                               "name AS 'Ім''я', surname AS 'Прізвище', " +
                               "phone_number AS 'Номер телефону', birthday_date AS 'Дата народження', banned AS 'Заблокований' FROM user";

                mySqlConnection.Open();
                MySqlCommand sqlCommand = new MySqlCommand(query, mySqlConnection);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCommand);
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataAdapter.Fill(dataTable);
                dataGridView3.DataSource = dataTable;
                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
            foreach (DataGridViewColumn column in dataGridView3.Columns)
            {
                column.ReadOnly = true;
            }
        }
        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView3.Rows[e.RowIndex];
                label25.Text = row.Cells["Логін"].Value.ToString();
            }
        }

        private void додаванняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupPanel.Visible = true;
            groupPanel2.Visible = false;
            flowLayoutPanel.Visible = false;
            groupPanel3.Visible = false;
            groupPanel4.Visible = false;
            groupPanel5.Visible = false;
            DeletePastSessions();
        }
        private void видаленняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupPanel.Visible = false;
            groupPanel2.Visible = false;
            flowLayoutPanel.Visible = false;
            groupPanel3.Visible = true;
            groupPanel4.Visible = false;
            groupPanel5.Visible = false;
            LoadData();
            DeletePastSessions();
        }

        private void редагуванняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupPanel2.Visible = true;
            groupPanel.Visible = false;
            flowLayoutPanel.Visible = false;
            groupPanel3.Visible = false;
            groupPanel4.Visible = false;
            groupPanel5.Visible = false;

            LoadData();
            DeletePastSessions();
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                textBox8.Text = selectedRow.Cells["Назва"].Value.ToString();
                dateTimePicker6.Value = Convert.ToDateTime(selectedRow.Cells["Дата сеансу"].Value);
                dateTimePicker5.Value = DateTime.Today.Add(TimeSpan.Parse(selectedRow.Cells["Час сеансу"].Value.ToString()));
                textBox7.Text = selectedRow.Cells["Ціна білету"].Value.ToString();
                comboBox3.Text = selectedRow.Cells["Номер залу"].Value.ToString();
                comboBox4.Text = selectedRow.Cells["Жанр"].Value.ToString();
                dateTimePicker4.Value = DateTime.Today.Add(TimeSpan.Parse(selectedRow.Cells["Тривалість"].Value.ToString()));
                textBox6.Text = selectedRow.Cells["Режисер"].Value.ToString();
                textBox5.Text = selectedRow.Cells["Актори"].Value.ToString();
                checkBox2.Checked = Convert.ToBoolean(selectedRow.Cells["Субтітри"].Value);
            }
        }

        private int selectedRecordId;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                selectedRecordId = Convert.ToInt32(selectedRow.Cells["id"].Value);
                textBox8.Text = selectedRow.Cells["Назва"].Value.ToString();
                dateTimePicker6.Value = Convert.ToDateTime(selectedRow.Cells["Дата сеансу"].Value);
                dateTimePicker5.Value = DateTime.Today + TimeSpan.Parse(selectedRow.Cells["Час сеансу"].Value.ToString());
                textBox7.Text = selectedRow.Cells["Ціна білету"].Value.ToString();
                comboBox3.Text = selectedRow.Cells["номер залу"].Value.ToString();
                comboBox4.Text = selectedRow.Cells["жанр"].Value.ToString();
                dateTimePicker4.Value = DateTime.Today + TimeSpan.Parse(selectedRow.Cells["тривалість"].Value.ToString());
                textBox6.Text = selectedRow.Cells["режисер"].Value.ToString();
                textBox5.Text = selectedRow.Cells["актори"].Value.ToString();
                checkBox2.Checked = Convert.ToBoolean(selectedRow.Cells["субтітри"].Value);
            }
        }

        private int selectedRecordId2;

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView2.Rows[e.RowIndex];
                selectedRecordId2 = Convert.ToInt32(selectedRow.Cells["id"].Value);
                label23.Text = selectedRecordId2.ToString();
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView2.Rows[e.RowIndex];
                selectedRecordId2 = Convert.ToInt32(selectedRow.Cells["id"].Value);
                label23.Text = selectedRecordId2.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedRecordId2 != 0)
                {
                    string deleteQuery = $"DELETE FROM AfishaFilm WHERE id = {selectedRecordId2}";
                    using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                    {
                        MySqlCommand sqlCommand = new MySqlCommand(deleteQuery, mySqlConnection);
                        mySqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }

                    LoadData();
                    MessageBox.Show("Запись успешно удалена.");
                    label3.Text = "";
                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении записи: " + ex.Message);
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
                MessageBox.Show("Ошибка при удалении прошедших сеансов: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string title = textBox1.Text.Trim();
            string genre = comboBox1.Text.Trim();
            string producer = textBox3.Text.Trim();
            string actors = textBox4.Text.Trim();

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

            DateTime dateFilm = dateTimePicker1.Value;
            DateTime timeFilm = dateTimePicker2.Value;

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

            decimal ticketCost;

            if (!decimal.TryParse(textBox2.Text, out ticketCost))
            {
                MessageBox.Show("Будь ласка, введіть коректну ціну білету.");
                return;
            }

            int cinemaHall;
            if (!int.TryParse(comboBox2.Text.Trim(), out cinemaHall))
            {
                MessageBox.Show("Будь ласка, введіть коректний номер залу.");
                return;
            }

            TimeSpan duration = dateTimePicker3.Value.TimeOfDay;
            bool subtitles = checkBox1.Checked;

            Image image = pictureBox.Image;

            if (image == null)
            {
                MessageBox.Show("Пожалуйста, загрузите изображение.");
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
                MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
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
                textBox1.Clear();
                comboBox1.SelectedIndex = -1;
                textBox3.Clear();
                textBox4.Clear();
                textBox2.Clear();
                comboBox2.SelectedIndex = -1;
                dateTimePicker3.Value = DateTime.Now.Date;
                checkBox1.Checked = false;
                pictureBox.Image = null;
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при додаванні даних в базу даних: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string title = textBox8.Text.Trim();
            string genre = comboBox4.Text.Trim();
            string producer = textBox6.Text.Trim();
            string actors = textBox5.Text.Trim();

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

            DateTime dateFilm = dateTimePicker6.Value;
            DateTime timeFilm = dateTimePicker5.Value;

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

            decimal ticketCost;

            if (!decimal.TryParse(textBox7.Text, out ticketCost))
            {
                MessageBox.Show("Будь ласка, введіть коректну ціну білету.");
                return;
            }

            int cinemaHall;
            if (!int.TryParse(comboBox3.Text.Trim(), out cinemaHall))
            {
                MessageBox.Show("Будь ласка, введіть коректний номер залу.");
                return;
            }

            TimeSpan duration = dateTimePicker4.Value.TimeOfDay;
            bool subtitles = checkBox2.Checked;

            try
            {
                MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
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
                textBox8.Clear();
                comboBox4.SelectedIndex = -1;
                textBox6.Clear();
                textBox5.Clear();
                textBox7.Clear();
                comboBox3.SelectedIndex = -1;
                dateTimePicker4.Value = DateTime.Now.Date;
                checkBox2.Checked = false;
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при оновленні запису в базі даних: " + ex.Message);
            }
        }

        public void UploadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Image = Image.FromFile(openFileDialog.FileName);

                // Сохраняем изображение в виде массива байтов
                using (MemoryStream ms = new MemoryStream())
                {
                    pictureBox.Image.Save(ms, pictureBox.Image.RawFormat);
                    imageData = ms.ToArray();
                }
            }
            else
            {
                MessageBox.Show("Не удалось загрузить изображение.");
                return;
            }
        }
        private void SortOptionChanged(object sender, EventArgs e)
        {
            SortMovies();
        }
        private void SearchButton_Click(object sender, EventArgs e)
        {
            SortMovies();
            System.Windows.Forms.TextBox searchTitleTextBox = (System.Windows.Forms.TextBox)flowLayoutPanel.Controls.Find("searchTitleTextBox", true).FirstOrDefault();

            if (searchTitleTextBox != null)
            {
                searchTitleTextBox.Text = "";
            }
        }

        private void SortMovies()
        {
            try
            {
                string sortOrder = ascendingRadioButton.Checked ? "ASC" : "DESC";
                string sortBy = "";

                if (sortComboBox.SelectedItem != null)
                {
                    switch (sortComboBox.SelectedItem.ToString())
                    {
                        case "Назва":
                            sortBy = "title";
                            break;
                        case "Тривалість":
                            sortBy = "duration";
                            break;
                        case "Дата проведення/Час сеансу":
                            sortBy = "datefilm, timefilm";
                            break;
                        case "Ціна":
                            sortBy = "ticketcost";
                            break;
                        case "Жанр":
                            sortBy = "genre";
                            break;
                        case "Номер залу":
                            sortBy = "cinemahall";
                            break;
                        default:
                            sortBy = "datefilm, timefilm";
                            break;
                    }
                }
                else
                {
                    sortBy = "datefilm, timefilm";
                }

                string titleFilter = "";
                string dateFilter = "";
                string searchTitle = ((System.Windows.Forms.TextBox)flowLayoutPanel.Controls.Find("searchTitleTextBox", true).FirstOrDefault())?.Text;
                DateTime? searchDate = ((DateTimePicker)flowLayoutPanel.Controls.Find("searchDatePicker", true).FirstOrDefault())?.Value;

                if (!string.IsNullOrEmpty(searchTitle))
                {
                    titleFilter = $"AND title LIKE '%{searchTitle}%'";
                }

                if (searchDate.HasValue)
                {
                    dateFilter = $"AND datefilm = '{searchDate.Value.ToString("yyyy-MM-dd")}'";
                }

                query = $"SELECT cinemahall, genre, producer, actors, title, datefilm, timefilm, ticketcost, image, duration " +
                        $"FROM AfishaFilm " +
                        $"WHERE 1=1 {titleFilter} {dateFilter} " +
                        $"ORDER BY {sortBy} {sortOrder}";

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    MySqlCommand sqlCommand = new MySqlCommand(query, mySqlConnection);
                    mySqlConnection.Open();
                    using (MySqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        UpdateMoviePanels(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
        }

        // Словарь для хранения состояния бронирования мест
        private Dictionary<string, bool[]> bookingStatus = new Dictionary<string, bool[]>();

        public decimal GetTicketPriceFromDB(string title, DateTime sessionTime, string hallNumber)
        {
            Console.WriteLine("Начало выполнения метода GetTicketPriceFromDB");
            decimal ticketPrice = 0;
            string mysqlCon = "server=localhost; user=root; database=filmuniverse; password=babych612";
            string query = "SELECT ticketcost FROM AfishaFilm WHERE title = @title AND DATE_FORMAT(timefilm, '%Y-%m-%d %H:%i') = @sessionTime AND cinemahall = @hallNumber";

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
                            Console.WriteLine($"Найдена цена билета: {ticketPrice}");
                        }
                        else
                        {
                            Console.WriteLine("Запись не найдена.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }
            Console.WriteLine("Конец выполнения метода GetTicketPriceFromDB");
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

        private void ShowBookingDialog(string title, string sessionTimeString, string hallNumber, string userLogin)
        {
            // Преобразуем строку sessionTimeString в DateTime
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

            // Проверяем, есть ли уже сохраненное состояние для данного сеанса
            if (!bookingStatus.ContainsKey(sessionId))
            {
                // Если нет, создаем новый массив состояний мест
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
                            MessageBox.Show($"Баланс: {userWallet}, Цена билета: {ticketPrice}");

                            if (userWallet >= ticketPrice)
                            {
                                UpdateUserWallet(userLogin, userWallet - ticketPrice);
                                btn.BackColor = Color.Red;
                                bookingStatus[sessionId][index] = true;
                                MessageBox.Show($"Місце {btn.Text} заброньовано.");
                                SaveBookingStatus();
                                CreateHTMLTicket(title, sessionTime, hallNumber, ticketPrice, index + 1, userLogin);
                                MessageBox.Show($"Поточний баланс: {userWallet}");
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

        private string bookingFilePath = @"D:\KHAI\kursova\bookingStatus.csv";

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

        private void LoadBookingStatus()
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

        public void UpdateMoviePanels(MySqlDataReader reader)
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
                Panel moviePanel = new Panel();
                moviePanel.Tag = "MoviePanel"; //тэг для идентификации панели
                moviePanel.BackColor = Color.LightGray;
                moviePanel.BorderStyle = BorderStyle.FixedSingle;
                moviePanel.Margin = new Padding(5);
                moviePanel.Width = flowLayoutPanel.Width - 25;
                moviePanel.Padding = new Padding(5); //  отступы внутри панели
                moviePanel.Height = 215; // высота панели
                moviePanel.BackColor = Color.FromArgb(228, 210, 148);


                // PictureBox для изображения фильма
                PictureBox pictureBox = new PictureBox();
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.Width = 150; // ширина для картинки
                pictureBox.Height = 200; //высота для картинки
                if (reader["image"] != DBNull.Value && ((byte[])reader["image"]).Length > 0)
                {
                    pictureBox.Image = ByteArrayToImage((byte[])reader["image"]); // Загружаем изображение из базы данных
                }
                else
                { 
                    pictureBox.Image = null; 
                }
                pictureBox.Margin = new Padding(0, 0, 10, 0); // Добавляем отступ справа

                //метки
                Label titleLabel = new Label();
                titleLabel.Text = reader["title"].ToString();
                titleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
                titleLabel.AutoSize = true;
                titleLabel.Padding = new Padding(0, 0, 0, 10);

                Label genreLabel = new Label();
                genreLabel.Text = $"Жанр: {reader["genre"].ToString()}";
                genreLabel.Font = new Font("Arial", 10);
                genreLabel.AutoSize = true;
                genreLabel.Padding = new Padding(0, 0, 0, 10);

                Label hallLabel = new Label();
                hallLabel.Text = $"Номер залу: {reader["cinemahall"].ToString()}";
                hallLabel.Font = new Font("Arial", 10);
                hallLabel.AutoSize = true;
                hallLabel.Padding = new Padding(0, 0, 0, 10);

                Label producerLabel = new Label();
                producerLabel.Text = $"Режисер: {reader["producer"].ToString()}";
                producerLabel.Font = new Font("Arial", 10);
                producerLabel.AutoSize = true;
                producerLabel.Padding = new Padding(0, 0, 0, 10);

                Label actorsLabel = new Label();
                actorsLabel.Text = $"Актори: {reader["actors"].ToString()}";
                actorsLabel.Font = new Font("Arial", 10);
                actorsLabel.AutoSize = true;
                actorsLabel.Padding = new Padding(0, 0, 0, 10);

                Label durationLabel = new Label();
                durationLabel.Text = $"Тривалість: {reader.GetTimeSpan("duration").ToString(@"hh\:mm\:ss")}";
                durationLabel.Font = new Font("Arial", 10);
                durationLabel.AutoSize = true;
                durationLabel.Padding = new Padding(0, 0, 0, 10);
                durationLabel.TextAlign = ContentAlignment.BottomLeft;

                Label timeLabel = new Label();
                timeLabel.Text = $"Дата проведення сеансу: {((DateTime)reader["datefilm"]).ToShortDateString()}, Час проведення: {((TimeSpan)reader["timefilm"]).ToString(@"hh\:mm")}";
                timeLabel.Font = new Font("Arial", 10);
                timeLabel.AutoSize = true;
                timeLabel.Padding = new Padding(0, 0, 0, 10);
                timeLabel.TextAlign = ContentAlignment.BottomLeft;

                Label priceLabel = new Label();
                priceLabel.Text = $"Ціна квитка: {reader["ticketcost"].ToString()}";
                priceLabel.Font = new Font("Arial", 10);
                priceLabel.AutoSize = true;
                priceLabel.Padding = new Padding(0, 0, 0, 5);
                priceLabel.TextAlign = ContentAlignment.BottomRight; priceLabel.Cursor = Cursors.Hand;
                string title = reader["title"].ToString();
                string ticketCost = reader["ticketcost"].ToString();

                string movieTitle = reader["title"].ToString();
                string sessionTime = $"{((DateTime)reader["datefilm"]).ToShortDateString()} {((TimeSpan)reader["timefilm"]).ToString(@"hh\:mm")}";
                string hallNumber = reader["cinemahall"].ToString();

                priceLabel.Click += (sender, e) => {
                    ShowBookingDialog(movieTitle, sessionTime, hallNumber, userLogin);
                };

                // Создаем TableLayoutPanel для размещения элементов
                TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                tableLayoutPanel.ColumnCount = 2;
                tableLayoutPanel.RowCount = 9;
                tableLayoutPanel.Dock = DockStyle.Fill;
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F)); // ширина столбца для картинки
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // шрина столбца для текста
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
                tableLayoutPanel.SetRowSpan(pictureBox, 9); // Объединяем строки для картинки

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
        private void переглядСеансівToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeletePastSessions();
            try
            {
                if (sortingpanelCreated == false)
                {
                    Panel sortPanel = new Panel();
                    sortPanel.AutoSize = true;
                    sortPanel.Dock = DockStyle.Bottom;

                    Label sortLabel = new Label();
                    sortLabel.Text = "Сортування по:";
                    sortLabel.AutoSize = true;
                    sortLabel.Location = new Point(10, 10);

                    if (sortComboBox.Items.Count == 0)
                    {
                        sortComboBox.Items.AddRange(new string[] { "Назва", "Тривалість", "Дата проведення/Час сеансу", "Ціна", "Жанр", "Номер залу" });
                    }

                    sortComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                    sortComboBox.Location = new Point(sortLabel.Right + 5, sortLabel.Top);

                    if (ascendingRadioButton.Parent == null)
                    {
                        ascendingRadioButton.Text = "За зростанням";
                        ascendingRadioButton.Checked = true;
                        ascendingRadioButton.AutoSize = true;
                        ascendingRadioButton.Location = new Point(sortComboBox.Right + 10, sortLabel.Top);
                    }

                    if (descendingRadioButton.Parent == null)
                    {
                        descendingRadioButton.Text = "За спаданням";
                        descendingRadioButton.AutoSize = true;
                        descendingRadioButton.Location = new Point(ascendingRadioButton.Right + 10, sortLabel.Top);
                    }

                    Label searchTitleLabel = new Label();
                    searchTitleLabel.Text = "Пошук по назві:";
                    searchTitleLabel.AutoSize = true;
                    searchTitleLabel.Location = new Point(10, sortLabel.Bottom + 20);

                    System.Windows.Forms.TextBox searchTitleTextBox = new System.Windows.Forms.TextBox();
                    searchTitleTextBox.Name = "searchTitleTextBox";
                    searchTitleTextBox.Location = new Point(searchTitleLabel.Right + 5, searchTitleLabel.Top);

                    System.Windows.Forms.Button searchButton = new System.Windows.Forms.Button();
                    searchButton.Text = "Пошук";
                    searchButton.AutoSize = true;
                    searchButton.Location = new Point(searchTitleTextBox.Right + 20, searchTitleLabel.Top);
                    searchButton.Click += new EventHandler(SearchButton_Click);

                    sortPanel.Margin = new Padding(0, 23, 0, 0);
                    sortPanel.Controls.Add(sortLabel);
                    sortPanel.Controls.Add(sortComboBox);
                    sortPanel.Controls.Add(ascendingRadioButton);
                    sortPanel.Controls.Add(descendingRadioButton);
                    sortPanel.Controls.Add(searchTitleLabel);
                    sortPanel.Controls.Add(searchTitleTextBox);
                    sortPanel.Controls.Add(searchButton);

                    flowLayoutPanel.Controls.Add(sortPanel);
                    sortingpanelCreated = true;
                }

                SortMovies();

                MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);

                MySqlCommand sqlCommand = new MySqlCommand(query, mySqlConnection);
                mySqlConnection.Open();
                using (MySqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    UpdateMoviePanels(reader);
                }

                sortComboBox.SelectedIndexChanged += SortOptionChanged;
                ascendingRadioButton.CheckedChanged += SortOptionChanged;
                descendingRadioButton.CheckedChanged += SortOptionChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }

            flowLayoutPanel.Visible = true;
            groupPanel.Visible = false;
            groupPanel2.Visible = false;
            groupPanel3.Visible = false;
            groupPanel4.Visible = false;
            groupPanel5.Visible = false;
            flowLayoutPanel.Margin = new Padding(0, 54, 0, 0);
        }


        // Метод для преобразования массива байтов в изображение
        private Image ByteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
        }

        private void переглядЮзерівToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupPanel4.Visible = true;
            flowLayoutPanel.Visible = false;
            groupPanel.Visible = false;
            groupPanel2.Visible = false;
            groupPanel3.Visible = false;
            groupPanel5.Visible = false;
            LoadUserData();
            DeletePastSessions();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string selectedLogin = label25.Text;
            if (string.IsNullOrEmpty(selectedLogin))
            {
                MessageBox.Show("Будь ласка, виберіть користувача.");
                return;
            }

            try
            {
                MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
                mySqlConnection.Open();

                // Отримуємо поточне значення banned
                string querySelect = "SELECT banned FROM user WHERE login = @login";
                MySqlCommand sqlCommandSelect = new MySqlCommand(querySelect, mySqlConnection);
                sqlCommandSelect.Parameters.AddWithValue("@login", selectedLogin);

                bool isBanned = Convert.ToBoolean(sqlCommandSelect.ExecuteScalar());

                // Змінюємо значення banned на протилежне
                bool newBannedValue = !isBanned;
                string queryUpdate = "UPDATE user SET banned = @banned WHERE login = @login";
                MySqlCommand sqlCommandUpdate = new MySqlCommand(queryUpdate, mySqlConnection);
                sqlCommandUpdate.Parameters.AddWithValue("@banned", newBannedValue);
                sqlCommandUpdate.Parameters.AddWithValue("@login", selectedLogin);

                sqlCommandUpdate.ExecuteNonQuery();
                mySqlConnection.Close();

                MessageBox.Show(newBannedValue ? "Користувач заблокований." : "Користувач розблокований.");

                // Оновлюємо дані в dataGridView3
                LoadUserData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string searchLogin = textBox9.Text.Trim();

            try
            {
                MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
                string query;

                if (string.IsNullOrEmpty(searchLogin))
                {
                    query = "SELECT login AS 'Логін', email AS 'Електронна пошта', " +
                            "name AS 'Ім''я', surname AS 'Прізвище', " +
                            "phone_number AS 'Номер телефону', birthday_date AS 'Дата народження', " +
                            "banned AS 'Заблокований' FROM user";
                }
                else
                {
                    query = "SELECT login AS 'Логін', email AS 'Електронна пошта', " +
                            "name AS 'Ім''я', surname AS 'Прізвище', " +
                            "phone_number AS 'Номер телефону', birthday_date AS 'Дата народження', " +
                            "banned AS 'Заблокований' FROM user WHERE login LIKE @login";
                }

                mySqlConnection.Open();
                MySqlCommand sqlCommand = new MySqlCommand(query, mySqlConnection);

                if (!string.IsNullOrEmpty(searchLogin))
                    sqlCommand.Parameters.AddWithValue("@login", "%" + searchLogin + "%");

                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCommand);
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataAdapter.Fill(dataTable);
                dataGridView3.DataSource = dataTable;
                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
        }

        private void особистийКабінетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupPanel2.Visible = false;
            groupPanel.Visible = false;
            flowLayoutPanel.Visible = false;
            groupPanel3.Visible = false;
            groupPanel4.Visible = false;
            groupPanel5.Visible = true;
            UpdateUserLabels(userLogin);
        }
        public void UpdateUserLabels(string userLogin)
        {
            label38.Visible = userLogin != "guest";
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
                            label27.Text = reader.GetString("name");
                            label28.Text = reader.GetString("surname");
                            label30.Text = reader.GetString("login");
                            label32.Text = reader.GetString("email");
                            label34.Text = reader.GetString("phone_number");
                            if (reader.IsDBNull(reader.GetOrdinal("wallet")))
                            {
                              label36.Text = "0"; // Если валлет равен null, устанавливаем текст на "0"
                            }
                            else
                            {
                              label36.Text = reader.GetDecimal("wallet").ToString();
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: " + ex.Message);
                }
            }
        }
        private void CreateHTMLTicket(string title, DateTime sessionTime, string hallNumber, decimal ticketPrice, int seatNumber, string userLogin)
        {
            // Получаем данные пользователя из базы данных
            string userName, userSurname, userEmail, userPhoneNumber;
            GetUserDetails(userLogin, out userName, out userSurname, out userEmail, out userPhoneNumber);

            // Создаем HTML-документ
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

        private void label38_Click(object sender, EventArgs e)
        {
            panel6.Visible = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            maskedTextBox1.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            if (string.IsNullOrEmpty(maskedTextBox1.Text.Trim()))
            {
                MessageBox.Show("Введите коректну картку.");
                return;
            }
            if (decimal.TryParse(textBox10.Text, out decimal result) && result > 0)
            {
                // Создание строки подключения к базе данных
                string connectionString = "server=localhost; user=root; database=filmuniverse; password=babych612";

                // SQL запрос на обновление баланса
                string query = "UPDATE user SET wallet = wallet + @balanceToAdd WHERE login = @login";

                // Использование блока using для автоматического закрытия соединения
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        // Открытие соединения
                        con.Open();

                        // Добавление параметров в SQL запрос
                        cmd.Parameters.AddWithValue("@balanceToAdd", result);
                        cmd.Parameters.AddWithValue("@login", userLogin); // Замените на актуальный логин

                        // Выполнение запроса
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Проверка, что запрос обновил строки
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Баланс оновлено.");
                            UpdateUserLabels(userLogin);
                            panel6.Visible = false;
                        }
                        else
                        {
                            MessageBox.Show("Помилка оновлення балансу.");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Введите коректне значення грошей яке ви хочете додати на баланс.");
            }
        }
    }
}
