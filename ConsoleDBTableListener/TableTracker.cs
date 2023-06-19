using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDBTableListener
{
    internal class TableTracker
    {
        private string _connectionString;
        private string _tableName;
        private long? _previousRowsCount;
        private long? _currentRowsCount;
        private Task _timer;
        public delegate void RecordsFound();
        public event RecordsFound OnRecordsFound;
        public TableTracker(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
            _previousRowsCount = Count();

            _timer = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    _currentRowsCount = Count();
                    if (_currentRowsCount != null)
                    {
                        if (_previousRowsCount !=null)
                        {
                            Console.WriteLine($"{DateTime.Now} Произведён опрос таблицы");
                            if (_currentRowsCount > _previousRowsCount)
                            {
                                Console.WriteLine($"{DateTime.Now} Добавлено {_currentRowsCount - _previousRowsCount} строк");
                                _previousRowsCount = _currentRowsCount;
                                OnRecordsFound?.Invoke();
                            }
                        }
                        else
                        {
                            _previousRowsCount = _currentRowsCount;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{DateTime.Now} Произошла ошибка при получении количества записей в таблице");
                    }

                    Thread.Sleep(3000);
                }
            });

        }

        private long? Count()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "SELECT SUM(row_count) " +
                                                    "FROM sys.dm_db_partition_stats " +
                                                    "WHERE object_id = OBJECT_ID('Tickets')";
                            var reader = command.ExecuteScalar();
                            return (long)reader;
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"{DateTime.Now} Произошла ошибка при получении количества записей в таблице");
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
            }
            catch (Exception)
            {
                Console.WriteLine($"{DateTime.Now} Произошла ошибка при открытии соединения");
                return null;
            }
        }
    }
}
