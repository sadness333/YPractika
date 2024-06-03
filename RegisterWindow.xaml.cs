using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using YPractika.users;

namespace YPractika
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Button_ClickBack(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void Button_ClickRegister(object sender, RoutedEventArgs e)
        {
            string fullName = fullNameTextBox.Text;
            string login = loginTextBox.Text;
            string email = emailTextBox.Text;
            string password = passwordBox.Password;

            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("INSERT INTO Users (login, password, role, email, fullName) VALUES (@login, @password, 'user', @email, @fullName)", connection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@fullName", fullName);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Успешная регистрация!");
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }
    }
}
