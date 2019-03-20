using ConsoleAppAGEMKO.model;
using ExcelDataReader;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Linq;

namespace ConsoleAppAGEMKO
{
    class Program
    {

        private static Businesses selectedBusiness = new Businesses();
        static void Main(string[] args)
        {
            //ReadExcelFile();
            ReadExcelFileForSqlScript();
            //testc();
            Console.WriteLine("Hello World!");
        }

        private static void ReadExcelFileForSqlScript()
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory;
            filePath = Path.Combine(filePath, "All.xlsx");

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
                               
                if(result.Tables.Count > 0)
                {
                    DataTable dt = result.Tables[0];
                    StringBuilder sb = new StringBuilder();
                    int counter = 1000;
                    foreach(DataRow row in dt.Rows)
                    {
                        if (!row.IsNull(0) && !row.IsNull(1) && !row.IsNull(2) && !row[0].ToString().Equals("addresskey"))
                        {
                            string query = string.Format(@"
                            SET @g = 'POINT({0} {1})';
                            INSERT INTO `wpct_wpgmza`(`id`, `map_id`, `address`, `description`, `pic`, `link`, `icon`, `lat`, `lng`, `anim`, `title`, `infoopen`, `category`, `approved`, `retina`, `type`, `did`, `other_data`, `latlng`) VALUES 
                            ({4}, 1, '{3}', '{3}', '', '', '', '{0}', '{1}', '0', '', '0', '', 1, 0, 0, '', '', ST_PointFromText(@g));
                            "
                           , row[1].ToString()
                           , row[2].ToString()
                           , row[0].ToString().Replace(',', ' ')
                           , row[4].ToString().Replace(',', ' ').Replace("'", " ")
                           , counter++);

                            sb.Append(query);
                        }
                    }

                    using (StreamWriter writer = new StreamWriter($"FinalSqlScript{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}.txt"))
                    {
                        writer.Write(sb.ToString());
                    }
                }
            }

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
                        selectedBusiness = new Businesses();

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

                    }
                }
            }
        }

        private static void Business(DataRow v)
        {
            selectedBusiness.BusinessesAgemko = v[2].ToString();
            selectedBusiness.BusinessesAmke = v[4].ToString();
            selectedBusiness.BusinessesVat = v[5].ToString();
            selectedBusiness.BusinessesDescr = v[6].ToString();
            selectedBusiness.BusinessesDistinctTitle = v[7].ToString();
            selectedBusiness.BusinessesNumMembers = v.IsNull(8) ? default(int?) : Convert.ToInt32(v[8].ToString());
            selectedBusiness.BusinessesAddress = v[10].ToString();
            selectedBusiness.BusinessesEmail = v[14].ToString();
            selectedBusiness.BusinessesRegisterDate = v.IsNull(16) ? default(DateTime?) : Convert.ToDateTime(v[16].ToString());
            selectedBusiness.BusinessesReviewDate = v.IsNull(17) ? default(DateTime?) : Convert.ToDateTime(v[17].ToString());

            using (mydbContext context = new mydbContext())
            {
                Businesses rType = new Businesses();

                if (string.IsNullOrWhiteSpace(selectedBusiness.BusinessesVat))
                {
                    if (!string.IsNullOrWhiteSpace(selectedBusiness.BusinessesAgemko))
                    {
                        rType = context.Businesses.FirstOrDefault(x => x.BusinessesAgemko.Equals(selectedBusiness.BusinessesAgemko));
                        if (rType == default(Businesses))
                        {
                            int? maxID = context.Businesses.Count().Equals(0) ? default(int?) : context.Businesses.Max(x => x.BusinessesId);
                            selectedBusiness.BusinessesId = maxID.HasValue ? maxID.Value + 1 : 1;
                            context.Businesses.Add(selectedBusiness);
                        }


                    }
                }
                else
                {
                    rType = context.Businesses.FirstOrDefault(x => x.BusinessesVat.Equals(selectedBusiness.BusinessesVat));
                    if (rType == default(Businesses))
                    {
                        int? maxID = context.Businesses.Count().Equals(0) ? default(int?) : context.Businesses.Max(x => x.BusinessesId);
                        selectedBusiness.BusinessesId = maxID.HasValue ? maxID.Value + 1 : 1;
                        context.Businesses.Add(selectedBusiness);
                    }
                }

                rType.StatusStatusId = selectedBusiness.StatusStatusId;
                rType.RepresentativeRepresentativeId = selectedBusiness.RepresentativeRepresentativeId;

                context.SaveChanges();
            }

        }

        private static void Representative(object v)
        {
            if (v != null && !string.IsNullOrWhiteSpace(v.ToString()))
            {
                using (mydbContext context = new mydbContext())
                {
                    Representative rType = new Representative();
                    rType = context.Representative.FirstOrDefault(x => x.RepresentativeFullName.Equals(v.ToString().Trim()));
                    if (rType == default(Representative))
                    {
                        rType = new Representative();
                        int? maxID = context.Representative.Count().Equals(0) ? default(int?) : context.Representative.Max(x => x.RepresentativeId);
                        rType.RepresentativeId = maxID.HasValue ? maxID.Value + 1 : 1;
                        rType.RepresentativeFullName = v.ToString().Trim();
                        context.Representative.Add(rType);
                        context.SaveChanges();
                    }
                    selectedBusiness.RepresentativeRepresentativeId = rType.RepresentativeId;
                }
            }

        }

        private static void Municipality(object vPreviousColumn, object v1)
        {
            if (v1 != null && !string.IsNullOrWhiteSpace(v1.ToString()))
            {
                using (mydbContext context = new mydbContext())
                {
                    Regionalunity regType = new Regionalunity();
                    regType = context.Regionalunity.FirstOrDefault(x => x.RegionalUnityDescr.Equals(vPreviousColumn.ToString().Trim()));
                    if (regType == default(Regionalunity))
                    {
                        regType = new Regionalunity();
                        int? maxID = context.Regionalunity.Count().Equals(0) ? default(int?) : context.Regionalunity.Max(x => x.RegionalUnityId);
                        regType.RegionalUnityId = maxID.HasValue ? maxID.Value + 1 : 1;
                        regType.RegionalUnityDescr = vPreviousColumn.ToString().Trim();
                        context.Regionalunity.Add(regType);
                        context.SaveChanges();
                    }


                    Municipality rType = new Municipality();
                    rType = context.Municipality.FirstOrDefault(x => x.MunicipalityDescr.Equals(v1.ToString().Trim()));
                    if (rType == default(Municipality))
                    {
                        rType = new Municipality();
                        int? maxID = context.Municipality.Count().Equals(0) ? default(int?) : context.Municipality.Max(x => x.MunicipalityId);
                        rType.MunicipalityId = maxID.HasValue ? maxID.Value + 1 : 1;
                        rType.MunicipalityDescr = v1.ToString().Trim();
                        rType.RegionalUnityRegionalUnityId = regType.RegionalUnityId;
                        context.Municipality.Add(rType);
                        context.SaveChanges();
                    }
                    selectedBusiness.MunicipalityMunicipalityId = rType.MunicipalityId;
                }
            }
        }

        private static void RegionalUnity(object vPreviousColumn, object v1)
        {
            if (v1 != null && !string.IsNullOrWhiteSpace(v1.ToString()))
            {
                using (mydbContext context = new mydbContext())
                {
                    Region regType = new Region();
                    regType = context.Region.FirstOrDefault(x => x.RegionDescr.Equals(vPreviousColumn.ToString().Trim()));
                    if (regType == default(Region))
                    {
                        regType = new Region();
                        int? maxID = context.Region.Count().Equals(0) ? default(int?) : context.Region.Max(x => x.RegionId);
                        regType.RegionId = maxID.HasValue ? maxID.Value + 1 : 1;
                        regType.RegionDescr = vPreviousColumn.ToString().Trim();
                        context.Region.Add(regType);
                        context.SaveChanges();
                    }


                    Regionalunity rType = new Regionalunity();
                    rType = context.Regionalunity.FirstOrDefault(x => x.RegionalUnityDescr.Equals(v1.ToString().Trim()));
                    if (rType == default(Regionalunity))
                    {
                        rType = new Regionalunity();
                        int? maxID = context.Regionalunity.Count().Equals(0) ? default(int?) : context.Regionalunity.Max(x => x.RegionalUnityId);
                        rType.RegionalUnityId = maxID.HasValue ? maxID.Value + 1 : 1;
                        rType.RegionalUnityDescr = v1.ToString().Trim();
                        rType.RegionRegionId = regType.RegionId;
                        context.Regionalunity.Add(rType);
                        context.SaveChanges();
                    }
                    selectedBusiness.RegionalUnityRegionalUnityId = rType.RegionalUnityId;
                }
            }
        }

        private static void Region(object v)
        {
            if (v != null && !string.IsNullOrWhiteSpace(v.ToString()))
            {
                using (mydbContext context = new mydbContext())
                {
                    Region rType = new Region();
                    rType = context.Region.FirstOrDefault(x => x.RegionDescr.Equals(v.ToString().Trim()));
                    if (rType == default(Region))
                    {
                        rType = new Region();
                        int? maxID = context.Region.Count().Equals(0) ? default(int?) : context.Region.Max(x => x.RegionId);
                        rType.RegionId = maxID.HasValue ? maxID.Value + 1 : 1;
                        rType.RegionDescr = v.ToString().Trim();
                        context.Region.Add(rType);
                        context.SaveChanges();
                    }
                    selectedBusiness.RegionRegionId = rType.RegionId;
                }
            }
        }

        private static void MainActivity(object v)
        {
            if (v != null && !string.IsNullOrWhiteSpace(v.ToString()))
            {
                using (mydbContext context = new mydbContext())
                {
                    Mainactivity rType = new Mainactivity();
                    rType = context.Mainactivity.FirstOrDefault(x => x.MainActivityDescr.Equals(v.ToString().Trim()));
                    if (rType == default(Mainactivity))
                    {
                        rType = new Mainactivity();
                        int? maxID = context.Mainactivity.Count().Equals(0) ? default(int?) : context.Mainactivity.Max(x => x.MainActivityId);
                        rType.MainActivityId = maxID.HasValue ? maxID.Value + 1 : 1;
                        rType.MainActivityDescr = v.ToString().Trim();
                        context.Mainactivity.Add(rType);
                        context.SaveChanges();
                    }
                    selectedBusiness.MainActivityMainActivityId = rType.MainActivityId;
                }
            }
        }

        private static void Status(object v)
        {
            if (v != null && !string.IsNullOrWhiteSpace(v.ToString()))
            {
                using (mydbContext context = new mydbContext())
                {
                    Status rType = new Status();
                    rType = context.Status.FirstOrDefault(x => x.StatusDescr.Equals(v.ToString().Trim()));
                    if (rType == default(Status))
                    {
                        rType = new Status();
                        int? maxID = context.Status.Count().Equals(0) ? default(int?) : context.Status.Max(x => x.StatusId);
                        rType.StatusId = maxID.HasValue ? maxID.Value + 1 : 1;
                        rType.StatusDescr = v.ToString().Trim();
                        context.Status.Add(rType);
                        context.SaveChanges();
                    }
                    selectedBusiness.StatusStatusId = rType.StatusId;
                }
            }
        }

        private static void IndividualCategory(object v)
        {
            if (v != null && !string.IsNullOrWhiteSpace(v.ToString()))
            {
                using (mydbContext context = new mydbContext())
                {
                    Individualcategory rType = new Individualcategory();
                    rType = context.Individualcategory.FirstOrDefault(x => x.IndividualCategoryDescr.Equals(v.ToString().Trim()));
                    if (rType == default(Individualcategory))
                    {
                        rType = new Individualcategory();
                        int? maxID = context.Individualcategory.Count().Equals(0) ? default(int?) : context.Individualcategory.Max(x => x.IndividualCategoryId);
                        rType.IndividualCategoryId = maxID.HasValue ? maxID.Value + 1 : 1;
                        rType.IndividualCategoryDescr = v.ToString().Trim();
                        context.Individualcategory.Add(rType);
                        context.SaveChanges();
                    }
                    selectedBusiness.IndividualCategoryIndividualCategoryId = rType.IndividualCategoryId;
                }

            }
        }

        private static void RegistryType(object v)
        {
            if (v != null && !string.IsNullOrWhiteSpace(v.ToString()))
            {
                using (mydbContext context = new mydbContext())
                {
                    Registrytype rType = new Registrytype();
                    rType = context.Registrytype.FirstOrDefault(x => x.RegistryTypeDescr.Equals(v.ToString().Trim()));
                    if (rType == default(Registrytype))
                    {
                        rType = new Registrytype();
                        int? maxID = context.Registrytype.Count().Equals(0) ? default(int?) : context.Registrytype.Max(x => x.RegistryTypeId);
                        rType.RegistryTypeId = maxID.HasValue ? maxID.Value + 1 : 1;
                        rType.RegistryTypeDescr = v.ToString().Trim();
                        context.Registrytype.Add(rType);
                        context.SaveChanges();
                    }
                    selectedBusiness.RegistryTypeRegistryTypeId = rType.RegistryTypeId;
                }

            }
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
