using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml.Linq;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection connection = new SqlConnection("Data Source=DESKTOP-ADCA5CP;Initial Catalog=master;Integrated Security=true;");

        public HomeController()
        {
            string sql = "Db_Login";

            try
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand($"SELECT database_id FROM sys.databases WHERE Name = '{sql}'", connection))
                {

                    object result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        cmd.CommandText = $"CREATE DATABASE {sql}";
                        cmd.ExecuteNonQuery();

                    }


                }
                connection.Close();

            }
            catch (Exception ex)
            {
                var m = ex.Message;
            }


        }




        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Index(UserInfoDTO user)
        {

            //string tableName = "tbl_UserInfo";
            //  ایجاد جدول
            //using (SqlConnection conn = new SqlConnection("Data Source=DESKTOP-ADCA5CP;Initial Catalog=Db_Login;Integrated Security=true;"))
            //{
            //    conn.Open();
            //    // Check if table exists
            //    using (SqlCommand cmd = new SqlCommand($"IF OBJECT_ID(N'{tableName}', N'U') IS NOT NULL SELECT 1 ELSE SELECT 0", conn))
            //    {
            //        int result = (int)cmd.ExecuteScalar();

            //        // If table does not exist, create it
            //        if (result == 0)
            //        {
            //            cmd.CommandText = $"CREATE TABLE {tableName} (ID INT, Name NVARCHAR(50),Family NVARCHAR(50))";
            //            cmd.ExecuteNonQuery();
            //        }
            //    }
            //    conn.Close();
            //}

            // ذخیره در دیتابیس و جلو گیری از ذخیره مقدار تکراری
            string tableName = "tbl_UserInfo";
            using (SqlConnection conn = new SqlConnection("Data Source=DESKTOP-ADCA5CP;Initial Catalog=Db_Login;Integrated Security=true;"))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand($"IF NOT EXISTS (SELECT * FROM {tableName} WHERE Name = @name AND Family = @family) BEGIN INSERT INTO {tableName} (Name, Family) VALUES (@name, @family) END", conn))
                {
                    cmd.Parameters.AddWithValue("@name", user.Name);
                    cmd.Parameters.AddWithValue("@family", user.Family);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }

            return View();
        }

        public ActionResult ShowUser()
        {
            List<UserInfoDTO> userInfoList = new List<UserInfoDTO>();

            using (SqlConnection conn = new SqlConnection("Data Source=DESKTOP-ADCA5CP;Initial Catalog=Db_Login;Integrated Security=true;"))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_UserInfo", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserInfoDTO userInfo = new UserInfoDTO
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader["Name"].ToString(),
                                Family = reader["Family"].ToString()
                            };

                            userInfoList.Add(userInfo);
                        }
                    }
                }
            }

            return View(userInfoList);
        }

       
        public IActionResult EditUser(int Id)
        {
            EditUserInfo userInfo = null;

            try
            {
                using (SqlConnection conn = new SqlConnection("Data Source=DESKTOP-ADCA5CP;Initial Catalog=Db_Login;Integrated Security=true;"))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_UserInfo WHERE ID = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", Id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userInfo = new EditUserInfo
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader["Name"].ToString(),
                                    Family = reader["Family"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return View(userInfo);
        }

        [HttpPost]
        public IActionResult EditUser(EditUserInfo editUser)
        {
            List<EditUserInfo> userInfoList = new List<EditUserInfo>();
            try
            {
                using (SqlConnection conn = new SqlConnection("Data Source=DESKTOP-ADCA5CP;Initial Catalog=Db_Login;Integrated Security=true;"))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE tbl_UserInfo SET [Name] = @name, Family = @family WHERE ID = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", editUser.Id);
                        cmd.Parameters.AddWithValue("@name", editUser.Name);
                        cmd.Parameters.AddWithValue("@family", editUser.Family);

                        cmd.ExecuteNonQuery();
                    }


                }
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToAction("ShowUser", "Home");
        }




        public IActionResult Delete(int Id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection("Data Source=DESKTOP-ADCA5CP;Initial Catalog=Db_Login;Integrated Security=true;"))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM tbl_UserInfo WHERE Id = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", Id);

                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }


            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
