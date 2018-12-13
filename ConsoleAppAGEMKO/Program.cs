using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace ConsoleAppAGEMKO
{
    class Program
    {
        static void Main(string[] args)
        {

            testc();
            Console.WriteLine("Hello World!");
        }




        private static void testc()
        {

            //MySqlConnection conn;
            string myConnectionString = "server=127.0.0.1;uid=root;pwd=giorgos5756;database=mydb";

            //try
            //{
            //    conn = new MySql.Data.MySqlClient.MySqlConnection();
            //    conn.ConnectionString = myConnectionString;
            //    conn.Open();
            //}
            //catch (MySql.Data.MySqlClient.MySqlException ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(myConnectionString))
            {
                //conn.ConnectionString = myConnectionString;
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM mydb.individualcategory;", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                conn.Close();
            }
           
        }


        
    }
    
}
