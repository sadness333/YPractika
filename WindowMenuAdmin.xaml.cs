using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для WindowMenuAdmin.xaml
    /// </summary>
    public partial class WindowMenuAdmin : Window
    {

        private DataTable ordersTable;
        private DataTable workersTable;


        public WindowMenuAdmin()
        {
            InitializeComponent();
            UpdateFaultTypeStatistics();
            LoadWorkers();
            LoadOrders();
        }

        private void Button_ClickConfirm(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите заказ для назначения сотрудника.");
                return;
            }

            if (WorkersComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника для назначения на заказ.");
                return;
            }

            DataRowView selectedOrder = (DataRowView)OrdersGrid.SelectedItem;
            int orderId = (int)selectedOrder["requestId"];
            int workerId = (int)WorkersComboBox.SelectedValue;

            UpdateOrderWorker(orderId, workerId);
            LoadOrders();

            MessageBox.Show("Сотрудник успешно назначен на заказ.");
        }

        private DataTable LoadWorkersData()
        {
            workersTable = new DataTable();
            string query = "SELECT userId, fullName FROM Users WHERE role = 'worker'";
            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    workersTable.Load(reader);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
            return workersTable;
        }

        private void LoadWorkers()
        {
            DataTable workersTable = LoadWorkersData();
            if (workersTable.Rows.Count > 0)
            {
                WorkersComboBox.ItemsSource = workersTable.DefaultView;
                WorkersComboBox.DisplayMemberPath = "fullName";
                WorkersComboBox.SelectedValuePath = "userId";
            }
        }


        private void LoadOrders()
        {
            ordersTable = new DataTable();
            string query = "SELECT r.requestId, r.faultType, r.status, r.workerId, r.nameEquip, w.fullName " +
                   "FROM Requests r " +
                   "LEFT JOIN Users w ON r.workerId = w.userId"; 
            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(ordersTable);

                    var ordersView = new DataView(ordersTable);
                    OrdersGrid.ItemsSource = ordersView;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

       

        private void UpdateOrderWorker(int orderId, int workerId)
        {
            string query = "UPDATE Requests SET workerId = @workerId WHERE requestId = @requestId";
            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@workerId", workerId);
                command.Parameters.AddWithValue("@requestId", orderId);

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

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private int CountOrders()
        {
            int orderCount = 0;
            string query = "SELECT COUNT(*) FROM Requests";
            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    orderCount = (int)command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error counting orders: " + ex.Message);
                }
            }
            return orderCount;
        }


        private double CalculateAverageExecutionTime()
        {
            double averageTime = 0;
            string query = "SELECT AVG(DATEDIFF(day, DateAdded, DateEnded)) FROM Requests WHERE DateAdded IS NOT NULL AND DateEnded IS NOT NULL";
            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        averageTime = Convert.ToDouble(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
            return averageTime;
        }
        private Dictionary<string, int> GetFaultTypeStatistics()
        {
            Dictionary<string, int> faultTypeStatistics = new Dictionary<string, int>();
            string query = "SELECT faultType, COUNT(*) AS Count FROM Requests GROUP BY faultType";
            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string faultType = reader["faultType"].ToString();
                        int count = Convert.ToInt32(reader["Count"]);
                        faultTypeStatistics.Add(faultType, count);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error getting fault type statistics: " + ex.Message);
                }
            }
            return faultTypeStatistics;
        }

        private void UpdateFaultTypeStatistics()
        {
            Dictionary<string, int> faultTypeStatistics = GetFaultTypeStatistics();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Статистика по типам неисправностей:");

            foreach (var pair in faultTypeStatistics)
            {
                sb.AppendLine($"{pair.Key}: {pair.Value} шт.");
            }

            FaultTypesTextBlock.Text = sb.ToString();
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            int orderCount = CountOrders();
            double averageTime = CalculateAverageExecutionTime();

            TotalRequestsTextBlock.Text = $"Количество заказов: {orderCount} шт.";
            AverageTimeTextBlock.Text = $"Среднее время выполнения заказа: {averageTime} дней";
        }

        private void LoadOrders(string searchQuery = "")
        {
            ordersTable = new DataTable();
            string query = "SELECT r.requestId, r.faultType, r.status, r.workerId, r.nameEquip, w.fullName " +
                           "FROM Requests r " +
                           "LEFT JOIN Users w ON r.workerId = w.userId";

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query += " WHERE r.requestId LIKE @searchQuery OR r.nameEquip LIKE @searchQuery";
            }

            using (SqlConnection connection = new SqlConnection(Admin.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    command.Parameters.AddWithValue("@searchQuery", "%" + searchQuery + "%");
                }

                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(ordersTable);

                    var ordersView = new DataView(ordersTable);
                    OrdersGrid.ItemsSource = ordersView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void Button_ClickSearch(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchTextBox.Text.Trim();

            if (string.IsNullOrEmpty(searchQuery))
            {
                MessageBox.Show("Поле поиска не может быть пустым. Введите текст для поиска.");
                LoadOrders();
                return;
            }

            LoadOrders(searchQuery);
        }



    }
}