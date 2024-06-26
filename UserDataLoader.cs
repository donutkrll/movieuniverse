﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace movieuniverse
{
    public class UserDataLoader
    {
        private string mysqlCon = "server=localhost; user=root; database=filmuniverse; password=babych612";

        public DataTable LoadUserData()
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
                System.Data.DataTable userdataTable = new System.Data.DataTable();
                dataAdapter.Fill(userdataTable);
                mySqlConnection.Close();
                return userdataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
                return null;
            }

        }

        public bool UpdateUserBalance(string userLogin, decimal balanceToAdd)
        {
            string query = "UPDATE user SET wallet = COALESCE(wallet, 0) + @balanceToAdd WHERE login = @login";

            using (MySqlConnection con = new MySqlConnection(mysqlCon))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    try
                    {
                        con.Open();

                        // Добавление параметров в SQL запрос
                        cmd.Parameters.AddWithValue("@balanceToAdd", balanceToAdd);
                        cmd.Parameters.AddWithValue("@login", userLogin); // Замените на актуальный логин
                                                                          // Выполнение запроса
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Проверка, что запрос обновил строки
                        if (rowsAffected > 0)
                        {
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Помилка оновлення балансу.");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Помилка: " + ex.Message);
                        return false;
                    }
                }
            }
        }
        public bool RegisterUser(UserData userData)
        {
            string name = userData.name;
            string surname = userData.surname;
            string login = userData.login;
            string email = userData.email;
            string phone = userData.phone;
            DateTime birthdate = userData.birthdate;
            string password = userData.password;

            // Ваша проверка данных здесь...

            // Підключення до бази даних та вставка нового запису
            string query = "INSERT INTO user (name, surname, login, email, phone_number, birthday_date, password) VALUES (@name, @surname, @login, @email, @phone, @birthdate, @password)";

            using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
            {
                MySqlCommand command = new MySqlCommand(query, mySqlConnection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@surname", surname);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@phone", phone);
                command.Parameters.AddWithValue("@birthdate", birthdate);
                command.Parameters.AddWithValue("@password", password);

                try
                {
                    mySqlConnection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Реєстрація пройшла успішно!");
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Помилка при реєстрації.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: " + ex.Message);
                    return false;
                }
            }
        }
    }
}
