
using System.Data.SqlClient;

try
{
    SqlConnection connection = new SqlConnection("Data Source=DESKTOP-ADCA5CP;Initial Catalog=master;Integrated Security=true;");
    connection.Open();
    SqlCommand cmd = new SqlCommand("CREATE DATABASE Db_Test", connection);
    cmd.ExecuteNonQuery();
    connection.Close();


    Console.WriteLine("good");
}
catch (Exception ex)
{

    Console.WriteLine(ex.Message);
}




