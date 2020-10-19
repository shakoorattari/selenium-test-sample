using System;
using System.Collections.Generic;
using System.Configuration;
using Entity;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using System.Data.SqlTypes;

namespace DL
{
    public class DAmazon
    {
        string conStr = ConfigurationManager.ConnectionStrings["AmazonStockTracking"].ConnectionString;
        public List<MainASINs> getEnabledASINs(int groupId)
        {
            List<MainASINs> lstmainASINs = new List<MainASINs>();
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("USP_SelectEnabledASINs", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@groupId", groupId));

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            MainASINs mainASINs = new MainASINs();
                            mainASINs.ASIN = (string)rdr["ASIN"];
                            mainASINs.Enabled = (bool)rdr["Enabled"];
                            mainASINs.CreatedDate = (string)rdr["CreatedDate"];
                            mainASINs.UpdateDate = (rdr["UpdateDate"] != DBNull.Value) ? (string)rdr["UpdateDate"] : "";

                            lstmainASINs.Add(mainASINs);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }                
            }
            return lstmainASINs;
        }

        public List<MainASINs> getEnabledASINs()
        {
            List<MainASINs> lstmainASINs = new List<MainASINs>();
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("USP_SelectEnabledASINs_NoGroup", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            MainASINs mainASINs = new MainASINs();
                            mainASINs.ASIN = (string)rdr["ASIN"];
                            mainASINs.Enabled = (bool)rdr["Enabled"];
                            mainASINs.CreatedDate = (string)rdr["CreatedDate"];
                            mainASINs.UpdateDate = (rdr["UpdateDate"] != DBNull.Value) ? (string)rdr["UpdateDate"] : "";

                            lstmainASINs.Add(mainASINs);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            return lstmainASINs;
        }

        public bool InsertTrackingRecord(CSObject cSObject)
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("USP_InsertTrackingRecord", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@ASIN", cSObject.ASIN));
                    cmd.Parameters.Add(new SqlParameter("@TotalStockCount", cSObject.TotalStockCount));
                    cmd.Parameters.Add(new SqlParameter("@NoOfSellers", cSObject.NoOfSellers));
                    cmd.Parameters.Add(new SqlParameter("@totalFBAStock", cSObject.totalFBAStock));
                    cmd.Parameters.Add(new SqlParameter("@totalFBMStock", cSObject.totalFBMStock));
                    cmd.Parameters.Add(new SqlParameter("@totalAMZStock", cSObject.totalAMZStock));

                    DataTable dtSellerDetails = ConvertListToDataTable(cSObject.SellerDetails);
                    SqlParameter sqlParam = cmd.Parameters.AddWithValue("@CSSellersObject", dtSellerDetails);
                    sqlParam.SqlDbType = SqlDbType.Structured;

                    return cmd.ExecuteNonQuery() > 1;

                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }

        public DataTable ConvertListToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        public bool Insert_tbltest()
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("USP_Insert_tbltest", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    string tst = "Test String at " + DateTime.Now.ToString();
                    cmd.Parameters.Add(new SqlParameter("@testString", tst));

                    return cmd.ExecuteNonQuery() > 1;

                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}
