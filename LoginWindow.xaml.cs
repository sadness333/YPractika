using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using YPractika.users;

namespace YPractika
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Button_ClickAuth(object sender, RoutedEventArgs e)
        {
            string login = loginTextBox.Text;
            string password = passwordBox.Password;

            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Users WHERE login = @login AND password = @password", connection);
                adapter.SelectCommand.Parameters.AddWithValue("@login", login);
                adapter.SelectCommand.Parameters.AddWithValue("@password", password);

                DataTable table = new DataTable();
                adapter.Fill(table);

                if (table.Rows.Count > 0)
                {
                    DataRow row = table.Rows[0];
                    string role = row["role"].ToString();


                    switch (role)
                    {
                        case "admin":
                            MessageBox.Show("Успешная авторизация!");
                            WindowMenuAdmin windowMenuAdmin = new WindowMenuAdmin();
                            windowMenuAdmin.Show();
                            this.Close();
                            break;
                        case "worker":
                            MessageBox.Show("Успешная авторизация!");
                            string workerid = row["userId"].ToString();
                            Worker.workerId = int.Parse(workerid);
                            WindowMenuWorker windowMenuWorker = new WindowMenuWorker();
                            windowMenuWorker.Show();
                            this.Close();
                            break;
                        case "user":
                            MessageBox.Show("Успешная авторизация!");
                            string userid = row["userId"].ToString();
                            Client.UserId = int.Parse(userid);
                            WindowMenuClient windowMenuClient = new WindowMenuClient();
                            windowMenuClient.Show();
                            this.Close();
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Неправильный логин или пароль");
                }
            }
        }

        private void Button_ClickBack(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

       
    }
}
