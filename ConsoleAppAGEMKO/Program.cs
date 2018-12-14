using ExcelDataReader;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ConsoleAppAGEMKO
{
    class Program
    {

        private static List<string> RegistryTypeList = new List<string>();
        private static List<string> IndividualCategoryList = new List<string>();
        private static List<string> StatusList = new List<string>();
        private static List<string> MainActivityList = new List<string>();
        private static List<string> RegionList = new List<string>();
        private static Dictionary<int, List<string>> RegionalUnityList = new Dictionary<int, List<string>>();
        private static Dictionary<int, List<string>> MunicipalityList = new Dictionary<int, List<string>>();
        private static List<string> RepresentativeList = new List<string>();
        private static List<Business> bussList = new List<Business>();
        private static Business selectedBusiness = new Business();
        static void Main(string[] args)
        {
            ReadExcelFile();
            testc();
            Console.WriteLine("Hello World!");
        }



        private static void ReadExcelFile()
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory;
            filePath = Path.Combine(filePath, "agemko.xlsx");

            if (File.Exists(filePath))
            {
                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                //...
                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                //...
                //3. DataSet - The result of each spreadsheet will be created in the result.Tables
                DataSet result = excelReader.AsDataSet();
                //...
                //4. DataSet - Create column names from first row
                //5. Data Reader methods
                //6. Free resources (IExcelDataReader is IDisposable)
                excelReader.Close();
                if (result.Tables.Count > 0)
                {
                    DataTable tbl = result.Tables[0];
                    for (int x = 5; x > -1; x--)
                    {
                        tbl.Rows.RemoveAt(x);
                    }

                    tbl.AcceptChanges();

                    
                    foreach (DataRow row in tbl.Rows)
                    {
                        selectedBusiness = new Business();

                        foreach (DataColumn column in tbl.Columns)
                        {

                            switch (column.Ordinal)
                            {

                                case 0:
                                    RegistryType(row[column]);
                                    break;
                                case 1:
                                    IndividualCategory(row[column]);
                                    break;
                                case 3:
                                    Status(row[column]);
                                    break;
                                case 9:
                                    MainActivity(row[column]);
                                    break;
                                case 11:
                                    Region(row[column]);
                                    break;
                                case 12:
                                    RegionalUnity(row[column.Ordinal - 1], row[column]);
                                    break;
                                case 13:
                                    Municipality(row[column.Ordinal - 1], row[column]);
                                    break;
                                case 15:
                                    Representative(row[column]);
                                    break;
                            }

                        }

                        Business(row);
                        bussList.Add(selectedBusiness);
                    }
                }
            }
        }

        private static void Business(DataRow v)
        {
            selectedBusiness.AGEMKO = v[2].ToString();
            selectedBusiness.AMKE = v[4].ToString();
            selectedBusiness.AFM = v[5].ToString();
            selectedBusiness.Epwnumia = v[6].ToString();
            selectedBusiness.Title = v[7].ToString();
            selectedBusiness.NumOfMembers = Convert.ToInt32(v[8].ToString());
            selectedBusiness.Address = v[10].ToString();
            selectedBusiness.Email = v[14].ToString();
            selectedBusiness.RegisterDate = v.IsNull(16) ? default(DateTime?) : Convert.ToDateTime(v[16].ToString());
            selectedBusiness.RegisterDate = v.IsNull(17) ? default(DateTime?) : Convert.ToDateTime(v[17].ToString());

        }

        private static void Representative(object v)
        {
            string element = string.Empty;
            if (!string.IsNullOrWhiteSpace(checkIfexistAndReturn(v, RepresentativeList)))
            {
                RepresentativeList.Add(v.ToString());
                selectedBusiness.Representative = RepresentativeList.IndexOf(v.ToString()) + 1;
            }
                
        }

        private static void Municipality(object v, object v1)
        {
            int indexOf = RegionList.IndexOf(v.ToString());

            if (indexOf > -1)
            {
                string element = string.Empty;
                if (!string.IsNullOrWhiteSpace(checkIfexistAndReturn(v1, MunicipalityList, indexOf)))
                {
                    MunicipalityList[indexOf].Add(v1.ToString());
                    selectedBusiness.Municipality = indexOf + 1;
                }
            }
        }

        private static void RegionalUnity(object v, object v1)
        {
            int indexOf = RegionList.IndexOf(v.ToString());

            if (indexOf > -1)
            {

                if(RegionalUnityList.Count > 0)
                {
                    string element = string.Empty;
                    if (!string.IsNullOrWhiteSpace(checkIfexistAndReturn(v1, RegionalUnityList, indexOf)))
                    {
                        RegionalUnityList[indexOf].Add(v1.ToString());
                        selectedBusiness.RegionalUnity = indexOf + 1;
                    }
                }
                else
                {
                    List<string> itemInList = new List<string>();
                    itemInList.Add(v1.ToString());
                    RegionalUnityList.Add(indexOf, itemInList);
                }

                
            }
        }

        private static void Region(object v)
        {
            string element = string.Empty;
            if (!string.IsNullOrWhiteSpace(checkIfexistAndReturn(v, RegionList)))
            {
                RegionList.Add(v.ToString());
                selectedBusiness.Region = RegionList.IndexOf(v.ToString()) + 1;
            }   
        }

        private static void MainActivity(object v)
        {
            string element = string.Empty;
            if (!string.IsNullOrWhiteSpace(checkIfexistAndReturn(v, MainActivityList)))
            {
                MainActivityList.Add(v.ToString());
                selectedBusiness.MainCategory = MainActivityList.IndexOf(v.ToString()) + 1;
            }   
        }

        private static void Status(object v)
        {
            string element = string.Empty;
            if (!string.IsNullOrWhiteSpace(checkIfexistAndReturn(v, StatusList)))
            {
                StatusList.Add(v.ToString());
                selectedBusiness.Status = StatusList.IndexOf(v.ToString()) + 1;
            }
                
        }

        private static void IndividualCategory(object v)
        {
            string element = string.Empty;
            if (!string.IsNullOrWhiteSpace(checkIfexistAndReturn(v, IndividualCategoryList)))
            {
                IndividualCategoryList.Add(v.ToString());
                selectedBusiness.IndividualCategory = IndividualCategoryList.IndexOf(v.ToString()) + 1;
            }   
        }

        private static void RegistryType(object v)
        {
            string element = string.Empty;
            if (!string.IsNullOrWhiteSpace(checkIfexistAndReturn(v, RegistryTypeList)))
            {
                RegistryTypeList.Add(v.ToString());
                selectedBusiness.RegistryType = RegistryTypeList.IndexOf(v.ToString()) + 1;
            }   
        }


        /// <summary>
        /// Check if Element is Contained to List or Not
        /// </summary>
        /// <param name="v"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private static string checkIfexistAndReturn(object v, List<string> list)
        {
            string element = string.Empty;
            if (v != null && !string.IsNullOrWhiteSpace(v.ToString().Trim()))
            {
                if (list.Count > 0)
                {
                    int index = list.IndexOf(v.ToString().Trim());

                    if (index == -1)
                    {
                        element = v.ToString();
                    }

                }
                else
                {
                    element = v.ToString();
                }
            }

            return element;
        }

        private static string checkIfexistAndReturn(object v, Dictionary<int, List<string>> list, int indexOf)
        {
            string element = string.Empty;
            if (v != null && !string.IsNullOrWhiteSpace(v.ToString().Trim()))
            {
                if (list.Count > 0)
                {
                    int index = list[indexOf].IndexOf(v.ToString().Trim());

                    if (index == -1)
                    {
                        element = v.ToString();
                    }

                }
                else
                {
                    element = v.ToString();
                }
            }

            return element;
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
