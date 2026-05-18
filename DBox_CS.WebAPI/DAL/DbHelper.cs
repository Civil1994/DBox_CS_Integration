using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBox_CS.WebAPI.Models;
//using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DBox_CS.WebAPI.DAL
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            //_connectionString = configuration.GetConnectionString("HCMSConnection")
            //    ?? throw new ArgumentNullException("HCMSConnection not found in appsettings.json");
        }

        public void SaveEmployee(EmployeeRequest employee)
        {
            //using var connection = new SqlConnection(_connectionString);
            //using var command = new SqlCommand("usp_SaveEmployee1", connection)
            //{
            //    CommandType = CommandType.StoredProcedure
            //};

            //command.Parameters.AddWithValue("@employee_id", employee.EmployeeId ?? (object)DBNull.Value);
            //command.Parameters.AddWithValue("@first_name", employee.FirstName ?? (object)DBNull.Value);
            //command.Parameters.AddWithValue("@last_name", employee.LastName ?? (object)DBNull.Value);
            //command.Parameters.AddWithValue("@date_of_joining", employee.DateOfJoining ?? (object)DBNull.Value);

            //connection.Open();
            //command.ExecuteNonQuery();
        }
    }
}
