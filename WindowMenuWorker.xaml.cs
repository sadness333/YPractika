using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using YPractika.users;

namespace YPractika
{
    public partial class WindowMenuWorker : Window
    {

        public WindowMenuWorker()
        {
            InitializeComponent();
            LoadAssignedRequests();
        }

        private void LoadAssignedRequests()
        {
            string query = "SELECT * FROM Requests WHERE workerId = @workerId AND status != 'Выполнено'";
            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@workerId", Worker.workerId);
                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    AssignedRequestsGrid.ItemsSource = dataTable.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading assigned requests: " + ex.Message);
                }
            }
        }



        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (AssignedRequestsGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)AssignedRequestsGrid.SelectedItem;
                int requestId = (int)selectedRow["requestId"];
                string status = selectedRow["status"].ToString();
                if (status != "Выполнено")
                {
                    UpdateRequestStatus(requestId, status);

                }
                else 
                {
                        MessageBoxResult result = MessageBox.Show("Вы уверены, что завершили починку устройства? После смена статуса вы потеряете доступ к заявке", "Окончание починки", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            UpdateRequestStatus(requestId, status);
                            UpdateRequestDate(requestId);
                            LoadAssignedRequests();


                        }

                }

            }
        }

        private void UpdateRequestDate(int requestId)
        {
            string query = "UPDATE Requests SET DateEnded = @DateEnded WHERE requestId = @RequestId";
            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@RequestId", requestId);
                command.Parameters.AddWithValue("@DateEnded", DateTime.Now);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }



        private void UpdateRequestStatus(int requestId, string status)
        {
            string query = "UPDATE Requests SET status = @status WHERE requestId = @requestId";
            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@requestId", requestId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Успешное сохранение");
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при изменении заявки: " + ex.Message);
                }
            }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void AddComment_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)AssignedRequestsGrid.SelectedItem;
            int requestId = (int)selectedRow["requestId"];
            int workerId = Worker.workerId; 
            string textComment = NewCommentTextBox.Text; 

            if (string.IsNullOrWhiteSpace(textComment))
            {
                MessageBox.Show("Введите комментарий.");
                return;
            }

            InsertComment(requestId, workerId, textComment);
            LoadCommentsForRequest(requestId);
        }

        private void InsertComment(int requestId, int workerId, string textComment)
        {
            string query = "INSERT INTO Comments (requestId, workerId, textComment) VALUES (@requestId, @workerId, @textComment)";
            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@requestId", requestId);
                command.Parameters.AddWithValue("@workerId", workerId);
                command.Parameters.AddWithValue("@textComment", textComment);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Комментарий добавлен.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void AssignedRequestsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssignedRequestsGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)AssignedRequestsGrid.SelectedItem;
                int requestId = (int)selectedRow["requestId"];
                LoadCommentsForRequest(requestId);
            }
        }
        private void LoadCommentsForRequest(int requestId)
        {
            string query = "SELECT textComment FROM Comments WHERE requestId = @requestId"; 
            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@requestId", requestId);
                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    CommentsListBox.Items.Clear();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string comment = row["textComment"].ToString();
                        CommentsListBox.Items.Add(comment);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading comments: " + ex.Message);
                }
            }
        }
    }
}
