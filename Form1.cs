using MySql.Data.MySqlClient;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace movieuniverse
{
    public partial class Form1 : Form
    {
        private Form2 form2;
        private UserData userData;
        public Form1()
        {
            InitializeComponent();
            form2 = new Form2();
            userData = new UserData();
            string mysqlCon = "server=localhost; user=root; database=filmuniverse; password=babych612";
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
            try
            {
                mySqlConnection.Open();
               // MessageBox.Show("Database connection success");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string password = textBox2.Text;

            string mysqlCon = "server=localhost; user=root; database=filmuniverse; password=babych612";
            string query = "SELECT COUNT(*) FROM user WHERE login = @login AND password = @password";

            using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
            {
                MySqlCommand command = new MySqlCommand(query, mySqlConnection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);

                try
                {
                    mySqlConnection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("Логін та пароль вірні. Успішний вхід!");

                        // Передача логіну у Form2 та відкриття форми
                        form2.SetUserLogin(login);
                        form2.UpdateUserLabels(login);
                        form2.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Невірний логін або пароль. Перевірте введені дані або зареєструйтеся.");
                        panel1.Visible = true;
                        panel2.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            userData.name = textBox3.Text.Trim();
            userData.surname = textBox4.Text.Trim();
            userData.login = textBox5.Text.Trim();
            userData.email = textBox6.Text.Trim();
            userData.phone = maskedTextBox1.Text.Trim();
            userData.birthdate = dateTimePicker1.Value;
            userData.password = textBox8.Text;
            string confirmPassword = textBox9.Text;

            // Перевірка правильності введених даних
            if (string.IsNullOrWhiteSpace(userData.name) || !Regex.IsMatch(userData.name, @"^[\p{L}\s-']{2,}$"))
            {
                MessageBox.Show("Введіть правильне ім'я (не менше 2 символів, без цифр та символів, крім пробілу, дефісу та апострофа).");
                return;
            }

            if (string.IsNullOrWhiteSpace(userData.surname) || !Regex.IsMatch(userData.surname, @"^[\p{L}\s-']{2,}$"))
            {
                MessageBox.Show("Введіть правильне прізвище (не менше 2 символів, без цифр та символів, крім пробілу, дефісу та апострофа).");
                return;
            }

            if (string.IsNullOrWhiteSpace(userData.login) || !Regex.IsMatch(userData.login, @"^[a-zA-Z0-9]{3,}$"))
            {
                MessageBox.Show("Введіть правильний логін (не менше 3 символів, тільки латинські літери та цифри).");
                return;
            }

            if (string.IsNullOrWhiteSpace(userData.email) || !Regex.IsMatch(userData.email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                MessageBox.Show("Введіть правильну електронну пошту.");
                return;
            }

            if (string.IsNullOrWhiteSpace(userData.phone) || !Regex.IsMatch(userData.phone, @"^\+38\(0\d{2}\)-\d{3}-\d{4}$"))
            {
                MessageBox.Show("Введіть правильний номер телефону у форматі +38(0XX)-XXX-XXXX.");
                return;
            }

            if (DateTime.Today.AddYears(-14) < userData.birthdate)
            {
                MessageBox.Show("Ви повинні бути старше 14 років.");
                return;
            }

            if (string.IsNullOrWhiteSpace(userData.password) || userData.password != confirmPassword)
            {
                MessageBox.Show("Паролі не співпадають або не введені.");
                return;
            }

            // Підключення до бази даних та вставка нового запису
            string mysqlCon = "server=localhost; user=root; database=filmuniverse; password=babych612";
            string query = "INSERT INTO user (name, surname, login, email, phone_number, birthday_date, password) VALUES (@name, @surname, @login, @email, @phone, @birthdate, @password)";

            using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
            {
                MySqlCommand command = new MySqlCommand(query, mySqlConnection);
                command.Parameters.AddWithValue("@name", userData.name);
                command.Parameters.AddWithValue("@surname", userData.surname);
                command.Parameters.AddWithValue("@login", userData.login);
                command.Parameters.AddWithValue("@email", userData.email);
                command.Parameters.AddWithValue("@phone", userData.phone);
                command.Parameters.AddWithValue("@birthdate", userData.birthdate);
                command.Parameters.AddWithValue("@password", userData.password);

                try
                {
                    mySqlConnection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Реєстрація пройшла успішно!");
                        panel1.Visible = false;
                        panel2.Visible = true;
                        ClearRegistrationFields();
                    }
                    else
                    {
                        MessageBox.Show("Помилка при реєстрації.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: Користувач з таким логіном вже існує");
                }
            }
        }

        private void ClearRegistrationFields()
        {
            // Очистити поля вводу панелі реєстрації
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            maskedTextBox1.Clear();
            dateTimePicker1.Value = DateTime.Today;
            textBox8.Clear();
            textBox9.Clear();
        }

        private void label11_Click(object sender, EventArgs e)
        {
            panel1.Visible=true;
            panel2.Visible = false;

        }

        private void label12_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
            panel1.Visible = false;
        }
    }
}
