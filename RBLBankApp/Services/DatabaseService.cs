// ===============================================
// FILE: Services/DatabaseService.cs
// ===============================================

using System;
using System.Data;
using System.Data.SqlClient;

namespace RBLBankApp.Services
{
    public class DatabaseService
    {
        // =========================================================
        // DATABASE CONNECTION STRING
        // =========================================================
        // Definition:
        // This string is used to connect the application
        // to SQL Server Database.
        //
        // Approach:
        // - Uses SQL Authentication
        // - Opens connection whenever needed
        // - Closes automatically using "using"
        //
        // Time Complexity:
        // O(1)
        // =========================================================

        private readonly string connectionString =
            @"Data Source=IMIM-SW-RNT-L11\SQLEXPRESSPDS;
              Initial Catalog=RBLBankDB;
              User ID=sa;
              Password=pds@123;
              TrustServerCertificate=True;";

        // =========================================================
        // EXECUTE SELECT QUERY
        // =========================================================
        // Definition:
        // Executes SELECT query and returns DataTable.
        //
        // Time Complexity:
        // O(n)
        // =========================================================

        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // =========================================================
        // INSERT / UPDATE / DELETE
        // =========================================================
        // Definition:
        // Executes INSERT, UPDATE, DELETE.
        //
        // Time Complexity:
        // O(1)
        // =========================================================

        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    con.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // =========================================================
        // EXECUTE SCALAR
        // =========================================================
        // Definition:
        // Returns single value from database.
        //
        // Time Complexity:
        // O(1)
        // =========================================================

        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    con.Open();

                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}