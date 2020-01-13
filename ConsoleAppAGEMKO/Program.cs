using ConsoleAppAGEMKO.model;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Linq;
using ConsoleAppAGEMKO.DataBase;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ConsoleAppAGEMKO
{
    class Program
    {
        //Scaffold-DbContext "Server=DEV-STAVROU;Database=AGEMKO;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir DataBase -f
        private static Businesses selectedBusiness = new Businesses();

        static void Main(string[] args)
        {
            //ReadExcelFile();
            ReadExcelFileForSqlScript();
            CreateScript();
        }

        private static void ReadExcelFileForSqlScript()
        {

            AGEMKOContext _context = new AGEMKOContext();

            string filePath = System.AppDomain.CurrentDomain.BaseDirectory;
            filePath = Path.Combine(filePath, "NewReceivedExcel.xlsx");
            DataSet result = new DataSet();
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
                result = excelReader.AsDataSet();
                //...
                //4. DataSet - Create column names from first row
                //5. Data Reader methods
                //6. Free resources (IExcelDataReader is IDisposable)
                excelReader.Close();

                foreach (DataRow row in result.Tables[0].Rows)
                {

                    if (!row.IsNull(10))
                    {
                        MainTable record = new MainTable();
                        string katigoria = row.IsNull(1) ? string.Empty : row[1].ToString();
                        string katastasi = row.IsNull(3) ? string.Empty : row[3].ToString();
                        string afm = row.IsNull(5) ? string.Empty : row[5].ToString();
                        string epwnumia = row[6].ToString().Trim();
                        string diakritosTitlos = row.IsNull(7) ? string.Empty : row[7].ToString();
                        string drastiriotita = row.IsNull(9) ? string.Empty : row[9].ToString();
                        string address = row[10].ToString().Trim();
                        string email = row.IsNull(14) ? string.Empty : row[14].ToString();
                        DateTime year = row.IsNull(16) ? DateTime.Now : Convert.ToDateTime(row[16].ToString());
                        string telephone = row.IsNull(14) ? string.Empty : row[18].ToString().Replace(',','|');

                        record.Catigoria = katigoria.Trim();
                        record.Katastasi = katastasi.Trim(); ;
                        record.Afm = afm.Trim();
                        record.Epwnumia = epwnumia.Trim();
                        record.DiakritosTitlos = diakritosTitlos.Trim();
                        record.Drastiriotita = drastiriotita.Trim();
                        record.Address = address.Trim();
                        record.Email = email.Trim();
                        record.Year = year;
                        record.Telephone = telephone;
                        _context.MainTable.Add(record);
                        _context.SaveChanges();
                    }
                }
            }

            DataSet result2 = new DataSet();
            string filePath2 = System.AppDomain.CurrentDomain.BaseDirectory;
            filePath2 = Path.Combine(filePath2, "All.xlsx");
            if (File.Exists(filePath2))
            {
                FileStream stream = File.Open(filePath2, FileMode.Open, FileAccess.Read);
                //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                //...
                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                //...
                //3. DataSet - The result of each spreadsheet will be created in the result.Tables
                result2 = excelReader.AsDataSet();
                //...
                //4. DataSet - Create column names from first row
                //5. Data Reader methods
                //6. Free resources (IExcelDataReader is IDisposable)
                excelReader.Close();


                foreach (DataRow row in result2.Tables[0].Rows)
                {
                    string addresskey = row[0].ToString();
                    string lat = row[1].ToString();
                    string longitude = row[2].ToString();

                    if (!row.IsNull(0))
                    {
                        MainTable record = _context.MainTable.FirstOrDefault(x => x.Address.Equals(row[0].ToString()));

                        if (record != default(MainTable))
                        {
                            record.Latitude = lat;
                            record.Longitude = longitude;
                            _context.SaveChanges();
                        }
                        else
                        {
                            if (!row.IsNull(5) && !row.IsNull(6))
                            {
                                record = _context.MainTable.FirstOrDefault(x => x.Address.Equals(row[6].ToString().Trim()) && x.DiakritosTitlos.Equals(row[5].ToString().Trim()));
                                if (record != default(MainTable))
                                {
                                    record.Latitude = lat;
                                    record.Longitude = longitude;
                                    _context.SaveChanges();
                                }
                            }

                        }
                    }
                }
            }
        }


        private static void CreateScript()
        {
            AGEMKOContext _context = new AGEMKOContext();
            /*
                      * 1= ΕΠΙΜΕΡΟΥΣ ΚΑΤΗΓΟΡΙΑ
                      * 6 = ΕΠΩΝΥΜΙΑ	
                      * 7 = ΔΙΑΚΡΙΤΙΚΟΣ ΤΙΤΛΟΣ
                      * 10 = ΣΤΟΙΧΕΙΑ Δ/ΝΣΗΣ
                      * 14 = email
                      * 
                      * Διακριτικό τίτλο
                         Επωνυμία
                         Κατηγορία
                         Ετος ίδρυσης
                         Στοιχεία επικοινωνίας (δ/νση, τηλ, email)
                      */
            StringBuilder sbwpgmza = new StringBuilder();
            StringBuilder sbemail = new StringBuilder();
            StringBuilder sbyear = new StringBuilder();
            StringBuilder sbtel = new StringBuilder();
            List<MainTable> listOfRecords = _context.MainTable.ToList();
            
            foreach (MainTable record in listOfRecords)
            {
                //style='list-style: none'
                string description = string.Format("<div><ul><li><b>Δ.ΤΙΤΛΟΣ:</b> {0}</li><li><b>ΔΡΑΣΤΗΡΙΟΤΗΤΑ:</b> {1}</li><li><b>ΕΙΔΟΣ:</b> {2}</li></ul></div>", record.DiakritosTitlos.Replace("'", "''"), record.Drastiriotita.Replace("'", "''"), record.Catigoria.Replace("'", "''"));

                //var output = Regex.Replace(record.Address.Replace("'", "''"), @"[\d]{3,7}", string.Empty);
                var output = Regex.Replace(record.Address.Replace("'", "''"), @"[\d]", string.Empty);

                string query = string.Format(@"
                                             SET @g = 'POINT({0} {1})';
                                             INSERT INTO `wpct_3_wpgmza`(`id`, `map_id`, `address`, `description`, `pic`, `link`, `icon`, `lat`, `lng`, `anim`, `title`, `infoopen`, `category`, `approved`, `retina`, `type`, `did`, `other_data`, `latlng`) VALUES 
                                             ({2}, 1, '{3}', '{4}', '', '', '', '{0}', '{1}', '0', '{5}', '0', '0', 1, 0, 0, '', '{6}', ST_PointFromText(@g));
                                             "
               , !string.IsNullOrWhiteSpace(record.Latitude) ? record.Latitude.Replace(',','.') : string.Empty
               , !string.IsNullOrWhiteSpace(record.Longitude) ? record.Longitude.Replace(',', '.') : string.Empty
               , record.Id
               , output //record.Address.Replace("'", "''")
               , description
               , record.Epwnumia.Replace("'", "''")
               , "a:1:{i:0;s:1:''0'';}");

                sbwpgmza.Append(query);
                sbwpgmza.AppendLine();

                if (!string.IsNullOrWhiteSpace(record.Email))
                {
                    string emailInsert = string.Format("INSERT INTO `wpct_3_wpgmza_markers_has_custom_fields`(`field_id`, `object_id`, `value`) VALUES (2,{0},'{1}');", record.Id, record.Email);

                    sbemail.AppendLine(emailInsert);
                }

                if (!string.IsNullOrWhiteSpace(record.Email))
                {
                    string yearInsert = string.Format("INSERT INTO `wpct_3_wpgmza_markers_has_custom_fields`(`field_id`, `object_id`, `value`) VALUES (1,{0},'{1}');", record.Id, record.Year.Value.Year);

                    sbyear.AppendLine(yearInsert);
                }

                if (!string.IsNullOrWhiteSpace(record.Telephone))
                {
                    string tel = string.Format("INSERT INTO `wpct_3_wpgmza_markers_has_custom_fields`(`field_id`, `object_id`, `value`) VALUES (3,{0},'{1}');", record.Id, record.Telephone.Trim());

                    sbtel.AppendLine(tel);
                }
            }

            sbwpgmza.Append(sbemail);
            sbwpgmza.Append(sbyear);
            sbwpgmza.Append(sbtel);

            using (StreamWriter writer = new StreamWriter($"FinalSqlScript{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}.txt"))
            {
                writer.Write(sbwpgmza.ToString());
            }
        }

        private static void ReadExcelFile()
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory;
            filePath = Path.Combine(filePath, "agemko.xlsx");

            if (File.Exists(filePath))
            {
                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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

    }

}
