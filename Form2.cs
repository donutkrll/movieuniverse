using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
        private FilmManager filmManager;
        public string query;
        private DataLoader dataLoader;
        private UserDataLoader userdataLoader;
        private BookingManager bookingManager;
        private Movie movie;
        private UserData userData;
        private string userLogin;
        public Form2()
        {
            InitializeComponent();
            filmManager = new FilmManager(mysqlCon);
            dataLoader = new DataLoader();
            userdataLoader = new UserDataLoader();
            bookingManager = new BookingManager();
            movie = new Movie();
            userData = new UserData();
            LoadData();
            bookingManager.LoadBookingStatus();

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
            query = "SELECT cinemahall, genre, title, datefilm, timefilm, ticketcost, image, duration, producer, actors FROM AfishaFilm ORDER BY datefilm, timefilm ASC";

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
            filmManager.UpdateCompleted += FilmManager_UpdateCompleted;

            flowLayoutPanel.Margin = new Padding(0, 0, 0, 27);

        }

        public DataGridView DataGridView1
        {
            get { return dataGridView1; }
            set { dataGridView1 = value; }
        }

        public DataGridView DataGridView2
        {
            get { return dataGridView2; }
            set { dataGridView2 = value; }
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
                dataLoader.DeletePastSessions();
            }
        }
        private void LoadData()
        {
            DataTable dataTable = dataLoader.LoadData();
            dataGridView1.DataSource = dataTable;
            dataGridView2.DataSource = dataTable;
        }
        public void LoadUserData()
        {
            DataTable userdataTable = userdataLoader.LoadUserData();
            dataGridView3.DataSource = userdataTable;
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
            dataLoader.DeletePastSessions();
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
            dataLoader.DeletePastSessions();
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
            dataLoader.DeletePastSessions();
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
            filmManager.DeleteFilm(selectedRecordId2);
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли у нас уже объект Movie
            if (movie == null)
            {
                MessageBox.Show("Ошибка: Объект Movie не инициализирован.");
                return;
            }

            // Обновляем свойства объекта movie данными из текстовых полей и других элементов управления формы
            movie.title = textBox1.Text.Trim();
            movie.genre = comboBox1.Text.Trim();
            movie.producer = textBox3.Text.Trim();
            movie.actors = textBox4.Text.Trim();
            movie.dateFilm = dateTimePicker1.Value;
            movie.timeFilm = dateTimePicker2.Value;
            decimal ticketCost;
            if (!decimal.TryParse(textBox2.Text, out ticketCost))
            {
                MessageBox.Show("Будь ласка, введіть коректну ціну білету.");
                return;
            }
            movie.ticketCost = ticketCost;
            int cinemaHall;
            if (!int.TryParse(comboBox2.Text.Trim(), out cinemaHall))
            {
                MessageBox.Show("Будь ласка, введіть коректний номер залу.");
                return;
            }
            movie.cinemaHall = cinemaHall;
            movie.duration = dateTimePicker3.Value.TimeOfDay;
            movie.subtitles = checkBox1.Checked;
            movie.image = pictureBox.Image;

            // Вызываем метод AddFilm для добавления нового фильма,
            // передавая в него объект movie
            filmManager.AddFilm(movie);

            // Очищаем поля после добавления
            textBox1.Clear();
            comboBox1.SelectedIndex = -1;
            textBox3.Clear();
            textBox4.Clear();
            textBox2.Clear();
            comboBox2.SelectedIndex = -1;
            dateTimePicker3.Value = DateTime.Now.Date;
            checkBox1.Checked = false;
            pictureBox.Image = null;

            // Перезагружаем данные
            LoadData();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Создаем объект Movie и заполняем его данными из элементов управления на форме
            Movie movieToUpdate = new Movie();
            movieToUpdate.title = textBox8.Text.Trim();
            movieToUpdate.genre = comboBox4.Text.Trim();
            movieToUpdate.producer = textBox6.Text.Trim();
            movieToUpdate.actors = textBox5.Text.Trim();
            movieToUpdate.dateFilm = dateTimePicker6.Value;
            movieToUpdate.timeFilm = dateTimePicker5.Value;
            decimal ticketCost;
            if (!decimal.TryParse(textBox7.Text, out ticketCost))
            {
                MessageBox.Show("Будь ласка, введіть коректну ціну білету.");
                return;
            }
            movieToUpdate.ticketCost = ticketCost;
            int cinemaHall;
            if (!int.TryParse(comboBox3.Text.Trim(), out cinemaHall))
            {
                MessageBox.Show("Будь ласка, введіть коректний номер залу.");
                return;
            }
            movieToUpdate.cinemaHall = cinemaHall;
            movieToUpdate.duration = dateTimePicker4.Value.TimeOfDay;
            movieToUpdate.subtitles = checkBox2.Checked;

            // Вызываем метод UpdateFilm, передавая ему объект Movie
            filmManager.UpdateFilm(selectedRecordId, movieToUpdate);
            LoadData();
        }

        private void FilmManager_UpdateCompleted(object sender, bool e)
        {

            if (e)
            {
                textBox8.Clear();
                comboBox4.SelectedIndex = -1;
                textBox6.Clear();
                textBox5.Clear();
                textBox7.Clear();
                comboBox3.SelectedIndex = -1;
                dateTimePicker4.Value = DateTime.Now.Date;
                checkBox2.Checked = false;
            }
            else
            {
                MessageBox.Show("Помилка при онновленны запису!");
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
       
        private void SearchButton_Click(object sender, EventArgs e)
        {
            SortMovies();
            System.Windows.Forms.TextBox searchTitleTextBox = (System.Windows.Forms.TextBox)flowLayoutPanel.Controls.Find("searchTitleTextBox", true).FirstOrDefault();

            if (searchTitleTextBox != null)
            {
                searchTitleTextBox.Text = "";
            }
        }
        private void SortOptionChanged(object sender, EventArgs e)
        {
            SortMovies();
        }
        private void SortMovies()
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
            filmManager.SortMovies(sortOrder, sortBy, titleFilter, dateFilter, flowLayoutPanel, dataLoader, bookingManager, userLogin);
        }
        private void переглядСеансівToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataLoader.DeletePastSessions();
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
                    dataLoader.UpdateMoviePanels(reader, flowLayoutPanel, bookingManager, userLogin);
                }

                sortComboBox.SelectedIndexChanged += SortOptionChanged;
                ascendingRadioButton.CheckedChanged += SortOptionChanged;
                descendingRadioButton.CheckedChanged += SortOptionChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ПомилкаASASD: " + ex.Message);
            }

            flowLayoutPanel.Visible = true;
            groupPanel.Visible = false;
            groupPanel2.Visible = false;
            groupPanel3.Visible = false;
            groupPanel4.Visible = false;
            groupPanel5.Visible = false;
            flowLayoutPanel.Margin = new Padding(0, 54, 0, 0);
        }

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
            dataLoader.DeletePastSessions();
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

        private void label38_Click(object sender, EventArgs e)
        {
            panel6.Visible = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            maskedTextBox1.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            if (string.IsNullOrWhiteSpace(maskedTextBox1.Text.Trim()))
            {
                MessageBox.Show("Введите коректну картку.");
                return;
            }
            if (decimal.TryParse(textBox10.Text, out decimal result) && result > 0)
            {
                UserDataLoader dataLoader = new UserDataLoader();

                if (dataLoader.UpdateUserBalance(userLogin, result))
                {
                    MessageBox.Show("Баланс оновлено.");
                    UpdateUserLabels(userLogin);
                    panel6.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("Введіть коректне значення грошей, яке Ви хочете додати на баланс.");
            }
        }
    }
}