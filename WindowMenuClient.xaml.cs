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
    /// Логика взаимодействия для WindowMenuClient.xaml
    /// </summary>
    public partial class WindowMenuClient : Window
    {
        public WindowMenuClient()
        {
            InitializeComponent();
            LoadRequests();
        }

        private void LoadRequests()
        {

                string query = "SELECT nameEquip, RequestId, DateAdded, Status, FaultType FROM Requests WHERE ClientId = @ClientId";

                using (SqlConnection connection = new SqlConnection(Admin.connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ClientId", Client.UserId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    RequestsListBox.ItemsSource = dataTable.DefaultView;
                }
        }

        private void LoadComments(int requestId)
        {
            string query = "SELECT TextComment FROM Comments WHERE RequestId = @RequestId";

            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RequestId", requestId);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                CommentsListBox.ItemsSource = dataTable.DefaultView;
                CommentsListBox.DisplayMemberPath = "TextComment";
            }
        }

        private void Button_ClickAdd(object sender, RoutedEventArgs e)
        {
            WindowAddRequest windowAddRequest = new WindowAddRequest();
            windowAddRequest.Show();
            this.Close();
        }


       

        private void Button_ClickBack(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Button_ClickDelete(object sender, RoutedEventArgs e)
        {
            if (RequestsListBox.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)RequestsListBox.SelectedItem;
                int requestId = Convert.ToInt32(selectedRow["RequestId"]);

                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить заявку?", "Удаление заявки", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(Admin.connectionString))
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand("DELETE FROM Comments WHERE RequestId = @RequestId", connection);
                        command.Parameters.AddWithValue("@RequestId", requestId);
                        command.ExecuteNonQuery();

                        command = new SqlCommand("DELETE FROM Requests WHERE RequestId = @RequestId", connection);
                        command.Parameters.AddWithValue("@RequestId", requestId);
                        command.ExecuteNonQuery();
                    }

                    LoadRequests();
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали заявку для удаления.");
            }
        }


        private void RequestsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RequestsListBox.SelectedItem is DataRowView selectedRow)
            {
                int requestId = Convert.ToInt32(selectedRow["RequestId"]);
                LoadComments(requestId);
            }
        }
    }
}