using Microsoft.Data.SqlClient;
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
using YPractika.users;

namespace YPractika
{
    /// <summary>
    /// Логика взаимодействия для WindowAddRequest.xaml
    /// </summary>
    public partial class WindowAddRequest : Window
    {
        public WindowAddRequest()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string itemName = itemNameTextBox.Text;
            string issueType = ((ComboBoxItem)issueTypeComboBox.SelectedItem)?.Content.ToString();

            if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(issueType))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            string query = "INSERT INTO Requests (ClientId, DateAdded, Status, FaultType, nameEquip) " +
                           "VALUES (@ClientId, @DateAdded, @Status, @FaultType, @nameEquip)";

            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClientId", Client.UserId);
                command.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                command.Parameters.AddWithValue("@Status", "Создано");
                command.Parameters.AddWithValue("@FaultType", issueType);
                command.Parameters.AddWithValue("@nameEquip", itemName);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            MessageBox.Show("Заявка успешно добавлена.");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowMenuClient windowMenuClient = new WindowMenuClient();
            windowMenuClient.Show();
            this.Close();
        }
    }
}
