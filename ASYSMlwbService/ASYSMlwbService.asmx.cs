using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using log4net;
using log4net.Config;

namespace ASYSMlwbService
{
    /// <summary>
    /// Summary description for ASYSMlwbService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ASYSMlwbService : System.Web.Services.WebService
    {
        public string var;
        public string date;
        public string database;
        public string desc, cost, serialNum, qty, weight, karat, carat, priceDesc, priceWeight, priceKarat, priceCarat, cellularcost, watchCost, repairCost, cleaningCost, goldCost, mountCost, allCost, ygCost, wgCost, barcode;
        public string fetch, code;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public bool flag = false;
        public void checker(string lotnum)
        {

          
                try
                {
                   
                      using (SqlConnection con = new SqlConnection(constring))
                              {

                                    con.Open();
                                    SqlDataReader read;
                                    using (SqlCommand cmd = new SqlCommand("select lotno from asys_MLWB_header where lotno='" + lotnum + "'", con))
                                    {
                                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                                        if (read.HasRows)
                                        {
                                            read.Close();
                                            flag = true;
                                        }

                                        else
                                        {
                                            flag = false;
                                        }

                                    }
                              }
                }
                catch (Exception)
                {
                    
                    throw;
                }
            

         
            
        }
        string constring = ConfigurationManager.ConnectionStrings["REMS"].ConnectionString;
        string remsVismin = ConfigurationManager.ConnectionStrings["remsVISMIN"].ConnectionString;
        string remsLuzon = ConfigurationManager.ConnectionStrings["REMSLUZON"].ConnectionString;
        public ASYSMlwbService()
        {
            XmlConfigurator.Configure();
        }
        [WebMethod]
        public MLWB RetrieveLotNumber() //Receiving combo box 
        {
            log.Info("Logging starts here: RetrieveLotNumber");
            var List = new MLWB();
            List.data1 = new getLotNumber();
            List.data1.lotNumber = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    con.Open();
                    string query = "SELECT DISTINCT reflotno as reflotno2 FROM (SELECT reflotno FROM ASYS_REM_Detail WHERE status = 'RELEASED' AND actionclass IN ('return','outsource','GoodStock','Watch','Cellular') UNION ALL SELECT reflotno FROM ASYS_REMOutsource_Detail WHERE status = 'RELEASED' AND actionclass IN ('return','outsource','GoodStock','Watch','Cellular'))a ORDER BY reflotno DESC";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandTimeout = 0;
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.CommandTimeout = 0;
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                List.data1.lotNumber.Add(rdr["reflotno2"].ToString());//.ToUpper());
                            }
                        }
                        con.Close();
                        return new MLWB { respCode = "1", respMsg = "Success", data1 = List.data1 };
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("RetrieveLotNumber: " + ex.Message);
                return new MLWB { respCode = "0", respMsg = "Error: " + ex.Message };
            }
        }
        [WebMethod]
        public DataSet RetrieveData(String lotnum) //Receiving
        {
            log.Info("Logging starts here: RetrieveData");
            DataSet ds = new DataSet();
            dbNull models = new dbNull();
            string query = "SELECT case when ptn IS NULL then '0' else ptn end as ptn ,refallbarcode as barcode," +
                "itemCode as ItemID,all_desc as alldesc,all_weight as weight,all_karat as karat,all_carat as carat," +
                "all_price as price FROM REMS.DBO.ASYS_REM_detail WHERE reflotno =" + lotnum + " AND status='RELEASED' " +
                "UNION ALL SELECT case when ptn IS NULL then '0' else ptn end as ptn, refallbarcode as barcode," +
                "itemCode as ItemID,all_desc as alldesc,all_weight as weight,all_karat as karat,all_carat as carat," +
                "all_price as price FROM REMS.DBO.ASYS_remoutsource_detail WHERE reflotno =" + lotnum + "  AND status='RELEASED'";
            string query1 = "SELECT case when ptn IS NULL then '0' else ptn end as ptn ,refallbarcode as barcode,itemCode as ItemID,all_desc as alldesc,all_weight as weight,all_karat as karat,all_carat as carat, case when all_price is NULL then '0' else all_price end  all_price FROM REMS.DBO.ASYS_REM_detail WHERE reflotno =" + lotnum + "AND status='RELEASED' AND actionclass IN ('return','outsource','goodstock','cellular','watch') UNION ALL SELECT case when ptn IS NULL then '0' else ptn end as ptn, refallbarcode as barcode,itemCode as ItemID,all_desc as alldesc,all_weight as weight,all_karat as karat,all_carat as carat,case when all_price is NULL then '0' else all_price end  all_price FROM REMS.DBO.ASYS_remoutsource_detail WHERE reflotno =" + lotnum + "   AND status='RELEASED' AND actionclass IN ('return','outsource','goodstock','cellular','watch')";

            using (SqlConnection con2 = new SqlConnection(constring))
            {
                con2.Open();
                try
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(query1, con2))
                    {
                        models.isNull(da.Fill(ds, "ptn"));
                        da.Fill(ds, ("ptn").Trim());
                        da.Dispose();
                    }
                    con2.Close();
                    return ds;
                }
                catch (Exception ex)
                {
                    log.Error("RetrieveData: " + ex.Message);
                    con2.Close();
                    ex.Message.ToString();
                    return null;
                }
            }
        }
        [WebMethod]
        public MLWB RetrieveData2()//(string lotno, string ID, string lotno2, string ID2) //Receiving display data
        {
            log.Info("Logging sstarts here: RetrieveData2");
            var List = new MLWB();
            List.RetrieveData2data = new RetrieveData2Models();
            List.RetrieveData2data.barcode = new List<string>();
            List.RetrieveData2data.alldesc = new List<string>();
            List.RetrieveData2data.weight = new List<string>();
            List.RetrieveData2data.karat = new List<string>();
            List.RetrieveData2data.carat = new List<string>();
            List.RetrieveData2data.price = new List<string>();
            var strings = new MLWB();
            strings.RetrieveData2data = new RetrieveData2Models();
            //dbNull models = new dbNull();

            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    //using (SqlCommand cmd = new SqlCommand("SELECT refallbarcode as barcode, all_desc as alldesc," +
                    //    " all_weight as weight, all_karat as karat, all_carat as carat, all_price as price FROM " +
                    //    "REMS.DBO.ASYS_REM_detail WHERE reflotno ='" + lotno + "' AND refallbarcode= " +
                    //    "'" + ID + "' AND status='RELEASED' UNION ALL SELECT refallbarcode as barcode," +
                    //    " all_desc as alldesc, all_weight as weight, all_karat as karat, all_carat as carat," +
                    //    " all_price as price FROM REMS.DBO.ASYS_remoutsource_detail WHERE reflotno " +
                    //    "='" + lotno2 + "' AND refallbarcode='" + ID2 + "' AND status='RELEASED'", con))

                    using (SqlCommand cmd = new SqlCommand("SELECT refallbarcode as barcode, all_desc as alldesc, all_weight as weight, all_karat as karat, all_carat as carat, all_price as price FROM REMS.DBO.ASYS_REM_detail WHERE reflotno ='2017100180' AND refallbarcode= '10230216201600089' AND status='RELEASED' UNION ALL SELECT refallbarcode as barcode, all_desc as alldesc, all_weight as weight, all_karat as karat, all_carat as carat, all_price as price FROM REMS.DBO.ASYS_remoutsource_detail WHERE reflotno ='2017100180' AND refallbarcode in ('10230216201600089','10330321201600371') AND status='RELEASED'", con)) 

                    //SELECT refallbarcode as barcode, all_desc as alldesc, all_weight as weight, all_karat as karat, all_carat as carat, all_price as price FROM REMS.DBO.ASYS_REM_detail WHERE reflotno ='2017100180' AND refallbarcode= '10230216201600089' AND status='RELEASED' UNION ALL SELECT refallbarcode as barcode, all_desc as alldesc, all_weight as weight, all_karat as karat, all_carat as carat, all_price as price FROM REMS.DBO.ASYS_remoutsource_detail WHERE reflotno ='2017100180' AND refallbarcode in ('10230216201600089','10330321201600371') AND status='RELEASED'
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (dr.Read() == true)
                        {
                      

                            List.RetrieveData2data.barcode.Add((dr["barcode"]).ToString().Trim());
                            List.RetrieveData2data.alldesc.Add((dr["alldesc"]).ToString().Trim());
                            List.RetrieveData2data.weight.Add((dr["weight"]).ToString().Trim());
                            List.RetrieveData2data.karat.Add((dr["karat"]).ToString().Trim());
                            List.RetrieveData2data.carat.Add((dr["carat"]).ToString().Trim());
                            List.RetrieveData2data.price.Add((dr["price"]).ToString().Trim());

                  


                        }
                        dr.Close();
                        con.Close();
                        return new MLWB { respCode = "1", respMsg = "Success!", RetrieveData2data = List.RetrieveData2data };//
                    }
                }
                catch (Exception ex)
                {
                    log.Error("RetrieveData2: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error: " + ex.Message };
                }
            }
        }

       
        [WebMethod]
        public MLWB costcenters()
        {
            log.Info("Logging starts here: costcenters");
            details data = new details();

            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
             
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("Select * from tbl_CostCenter", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (read.Read())
                        {
                            data.CostDept = read["CostDept"].ToString().Trim();
                        }
                    }
                    return new MLWB { respCode = "1", respMsg = "Success", MLWBInventory = data };
                }
                catch (Exception ex)
                {
                    log.Error("costcenters: " +ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB saveData(string userLog, string[][] DgEntry, string lotnum)//Receiving 
        {
            log.Info("Logging starts here: saveData");
            SqlTransaction tran;
            
            checker(lotnum);

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                tran = con.BeginTransaction();
                try
                {

                    #region 
                    //SqlDataReader read;
                    //using (SqlCommand cmd = new SqlCommand("select lotno from asys_MLWB_header where lotno='" + lotnum + "'", con, tran))
                    //{
                    //    read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    //    if (read.HasRows)
                    //    {
                    //        read.Close();
                    //        flag = true;
                    //    }

                    //    else
                    //    {
                    //        read.Close();
                    //        con.Open();
                    //        using (SqlCommand cmd1 = new SqlCommand("Insert into asys_MLWB_header (lotno) Values ('" + lotnum + "')", con, tran))
                    //        {

                    //            cmd1.ExecuteNonQuery();
                    //        }

                    //    }

                    //}

                    #endregion 
                    if (flag == true)
                    {
                        
                    }
                    else
                    {
                        using (SqlCommand cmd1 = new SqlCommand("Insert into asys_MLWB_header (lotno) Values ('" + lotnum + "')", con, tran))
                        {

                            cmd1.ExecuteNonQuery();
                            flag = false;
                        }
                    }
                   

                    for (int i = 0; i <= DgEntry.Count() - 1; i++)
                    {
                        
                        if (DgEntry[i][0] != null)
                        {
                            
                            using (SqlCommand cmd = new SqlCommand("INSERT INTO [REMS].dbo.ASYS_BarcodeHistory (lotno,refallbarcode, allbarcode,itemcode," +
                               "[description],karat,carat,SerialNo,weight,currency,price,cost,custodian,trandate,status,costcenter," +
                               "empname) SELECT lotno,refallbarcode, allbarcode,itemcode,[description],karat,carat,SerialNo,weight," +
                               "currency,price,cost,custodian,trandate,status,costcenter,empname from (SELECT TOP 1 RefLotno as lotno," +
                               " RefALLBarcode, allbarcode, RefItemcode as itemcode, ALL_Desc as [description], ALL_Karat as karat," +
                               " ALL_Carat as carat, SerialNo, ALL_Weight as weight, Currency , ALL_price as price, ALL_Cost as cost," +
                               " SorterName as custodian,getdate() as trandate,'RECEIVED' as status,'MLWB' as costcenter,'" + userLog +
                               "' as empname FROM dbo.ASYS_REM_Detail WHERE refallbarcode = '" + DgEntry[i][0] + "' AND " +
                               "status = 'RELEASED' UNION ALL SELECT TOP 1 RefLotno as lotno, RefALLBarcode, allbarcode, RefItemcode as itemcode, " +
                               "ALL_Desc as [description], ALL_Karat as karat, ALL_Carat as carat, SerialNo, ALL_Weight as weight, " +
                               "Currency , ALL_price as price, ALL_Cost as cost ,'" + userLog + "' as custodian,getdate() as " +
                               "trandate,'RECEIVED' as status,'MLWB' as costcenter,'" + userLog + "' as empname FROM dbo.ASYS_REMOutsource_Detail WHERE" +
                               " refallbarcode = '" + DgEntry[i][0] + "' AND status = 'RELEASED' )a", con, tran))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            string query = "INSERT INTO ASYS_MLWB_Detail (reflotno,lotno,refallbarcode, " +
                                "allbarcode,ptn,itemid,ptnbarcode,branchcode,branchname,loanvalue,refitemcode,itemcode,branchitemdesc, " +
                                "refqty,qty,karatgrading,caratsize,weight,actionclass,sortcode,all_desc,all_karat,all_carat,SerialNo, " +
                                "all_cost,all_weight,appraisevalue,currency,photoname,price_desc,price_karat,price_weight,price_carat, " +
                                "all_price,cellular_cost,watch_cost,cleaning_cost,gold_cost,mount_cost,yg_cost,wg_cost,costdate,costname, " +
                                "maturitydate,expirydate,loandate,receivedate,receiver,custodian,status)SELECT reflotno,lotno,refallbarcode, " +
                                "allbarcode,ptn,itemid,ptnbarcode,branchcode,branchname,loanvalue,refitemcode,itemcode,branchitemdesc, " +
                                "refqty,qty,karatgrading,caratsize,weight,actionclass,sortcode,all_desc,all_karat,all_carat,SerialNo, " +
                                "all_cost,all_weight,appraisevalue,currency,photoname,price_desc,price_karat,price_weight,price_carat, " +
                                "all_price,cellular_cost,watch_cost,cleaning_cost,gold_cost,mount_cost,yg_cost,wg_cost,costdate,costname, " +
                                "maturitydate,expirydate,loandate,receivedate,receiver,custodian,'RECEIVED' as status " +
                                "FROM ( SELECT TOP 1 dbo.ASYS_REM_Detail.ID, dbo.ASYS_REM_Detail.RefLotno, " +
                                "dbo.ASYS_REM_Detail.RefLotno as lotno,dbo.ASYS_REM_Detail.RefALLBarcode,dbo.ASYS_REM_Detail.RefALLBarcode as allbarcode, " +
                                " dbo.ASYS_REM_Detail.PTN, dbo.ASYS_REM_Detail.itemid, dbo.ASYS_REM_Header.PTNBarcode, " +
                                "dbo.ASYS_REM_Header.BranchCode, dbo.ASYS_REM_Header.BranchName, dbo.ASYS_REM_header.loanvalue, " +
                                "dbo.ASYS_REM_Detail.RefItemcode, dbo.ASYS_REM_Detail.RefItemcode as itemcode,dbo.ASYS_REM_Detail.BranchItemDesc," +
                                " dbo.ASYS_REM_Detail.RefQty,dbo.ASYS_REM_Detail.RefQty as qty, dbo.ASYS_REM_Detail.KaratGrading, " +
                                "dbo.ASYS_REM_Detail.CaratSize, dbo.ASYS_REM_Detail.Weight, dbo.ASYS_REM_Detail.ActionClass, dbo.ASYS_REM_Detail.SortCode, " +
                                " dbo.ASYS_REM_Detail.ALL_Desc,  dbo.ASYS_REM_Detail.ALL_Karat, dbo.ASYS_REM_Detail.ALL_Carat, " +
                                "dbo.ASYS_REM_Detail.SerialNo, dbo.ASYS_REM_Detail.ALL_Cost, dbo.ASYS_REM_Detail.ALL_Weight, " +
                                "dbo.ASYS_REM_detail.AppraiseValue, dbo.ASYS_REM_Detail.Currency, dbo.ASYS_REM_Detail.PhotoName, " +
                                "dbo.ASYS_REM_Detail.Price_Desc, dbo.ASYS_REM_Detail.Price_karat, dbo.ASYS_REM_Detail.Price_weight, " +
                                "dbo.ASYS_REM_Detail.Price_carat, dbo.ASYS_REM_Detail.all_price, dbo.ASYS_REM_Detail.Cellular_cost, " +
                                "dbo.ASYS_REM_Detail.Watch_cost, dbo.ASYS_REM_Detail.Repair_cost, dbo.ASYS_REM_Detail.Cleaning_cost, " +
                                "dbo.ASYS_REM_Detail.Gold_cost, dbo.ASYS_REM_Detail.Mount_cost, dbo.ASYS_REM_Detail.YG_cost, " +
                                "dbo.ASYS_REM_Detail.WG_cost, dbo.ASYS_REM_Detail.CostDate, dbo.ASYS_REM_Detail.CostName, " +
                                "dbo.ASYS_REM_Header.MaturityDate, dbo.ASYS_REM_Header.ExpiryDate,  dbo.ASYS_REM_Header.LoanDate,getdate() as receivedate," +
                                "'" + userLog + "' as receiver,'" + userLog + "' as custodian " +
                                "FROM dbo.ASYS_REM_Detail  INNER JOIN dbo.ASYS_REM_Header  ON dbo.ASYS_REM_Detail.PTN = dbo.ASYS_REM_Header.PTN and " +
                                "dbo.ASYS_REM_Detail.Lotno = dbo.ASYS_REM_Header.Lotno WHERE refallbarcode = '" + DgEntry[i][0] + "' and " +
                                "reflotno = '" + lotnum + "' AND status = 'RELEASED' ORDER BY dbo.ASYS_REM_Detail.ID DESC " +
                                "union all SELECT TOP 1 '' AS ID, dbo.ASYS_REMOutsource_Detail.RefLotno, dbo.ASYS_REMOutsource_Detail.RefLotno as lotno, " +
                                "dbo.ASYS_REMOutsource_Detail.RefALLBarcode,dbo.ASYS_REMOutsource_Detail.RefALLBarcode as allbarcode, " +
                                "'0' as PTN, dbo.ASYS_REMOutsource_Detail.itemid,dbo.ASYS_REMOutsource_detail.PTNBarcode, '0' as branchcode, " +
                                " 'NONE' as branchname,dbo.ASYS_REMOutsource_Detail.loanvalue, dbo.ASYS_REMOutsource_Detail.RefItemcode, " +
                                "dbo.ASYS_REMOutsource_Detail.RefItemcode as itemcode, dbo.ASYS_REMOutsource_Detail.BranchItemDesc, " +
                                "dbo.ASYS_REMOutsource_Detail.RefQty,dbo.ASYS_REMOutsource_Detail.RefQty as qty, dbo.ASYS_REMOutsource_Detail.KaratGrading, " +
                                "dbo.ASYS_REMOutsource_Detail.CaratSize, dbo.ASYS_REMOutsource_Detail.Weight, dbo.ASYS_REMOutsource_Detail.ActionClass, " +
                                " dbo.ASYS_REMOutsource_Detail.SortCode, dbo.ASYS_REMOutsource_Detail.ALL_Desc, dbo.ASYS_REMOutsource_Detail.ALL_Karat, " +
                                " dbo.ASYS_REMOutsource_Detail.ALL_Carat, dbo.ASYS_REMOutsource_Detail.SerialNo, dbo.ASYS_REMOutsource_Detail.ALL_Cost, " +
                                "dbo.ASYS_REMOutsource_Detail.ALL_Weight, dbo.ASYS_REMOutsource_detail.AppraiseValue, dbo.ASYS_REMOutsource_Detail.Currency, " +
                                "dbo.ASYS_REMOutsource_Detail.PhotoName, dbo.ASYS_REMOutsource_Detail.Price_Desc, dbo.ASYS_REMOutsource_Detail.Price_karat, " +
                                "dbo.ASYS_REMOutsource_Detail.Price_weight, dbo.ASYS_REMOutsource_Detail.Price_carat, dbo.ASYS_REMOutsource_Detail.all_price," +
                                "dbo.ASYS_REMOutsource_Detail.Cellular_cost, dbo.ASYS_REMOutsource_Detail.Watch_cost, dbo.ASYS_REMOutsource_Detail.Repair_cost," +
                                " dbo.ASYS_REMOutsource_Detail.Cleaning_cost, dbo.ASYS_REMOutsource_Detail.Gold_cost, dbo.ASYS_REMOutsource_Detail.Mount_cost, " +
                                " dbo.ASYS_REMOutsource_Detail.YG_cost, dbo.ASYS_REMOutsource_Detail.WG_cost, dbo.ASYS_REMOutsource_Detail.CostDate, " +
                                "dbo.ASYS_REMOutsource_Detail.CostName, dbo.ASYS_REMOutsource_detail.MaturityDate, dbo.ASYS_REMOutsource_detail.ExpiryDate,  " +
                                "dbo.ASYS_REMOutsource_detail.LoanDate, getdate() as receivedate,'" + userLog + "' as receiver," +
                                "'" + userLog + "' as custodian FROM dbo.ASYS_REMOutsource_Detail  WHERE " +
                                "refallbarcode = '" + DgEntry[i][0] + "' and reflotno = '" + lotnum + "' AND status = 'RELEASED')a";
                            using (SqlCommand cmd = new SqlCommand(query, con, tran))
                            {
                                cmd.CommandTimeout = 1000000;
                                cmd.ExecuteNonQuery();

                            }
                            using (SqlCommand cmd = new SqlCommand("UPDATE ASYS_REMoutsource_detail SET status = 'RECMLWB' where  refallbarcode = '" + DgEntry[i][0] + "' and  status ='RELEASED'", con, tran))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            using (SqlCommand cmd = new SqlCommand("UPDATE ASYS_REM_detail SET status = 'RECMLWB' where  refallbarcode = '" + DgEntry[i][0] + "' and  status ='RELEASED'", con, tran))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            using (SqlCommand cmd = new SqlCommand("UPDATE REMSLUZON.dbo.asys_remoutsource_detail SET status = 'RECMLWB' where refallbarcode = '" + DgEntry[i][0] + "' and  status = 'RELEASED' ", con, tran))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            using (SqlCommand cmd = new SqlCommand("UPDATE REMSLUZON.dbo.asys_REM_detail SET status = 'RECMLWB' where  refallbarcode = '" + DgEntry[i][0] + "'  and  status ='RELEASED' ", con, tran))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            using (SqlCommand cmd = new SqlCommand("UPDATE REMSVISAYAS.dbo.asys_remoutsource_detail SET status = 'RECMLWB' where  refallbarcode = '" + DgEntry[i][0] + "' and status ='RELEASED'", con, tran))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            using (SqlCommand cmd = new SqlCommand("UPDATE REMSVISAYAS.dbo.asys_REM_detail SET status = 'RECMLWB' where  refallbarcode = '" + DgEntry[i][0] + "' and  status ='RELEASED'", con, tran))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd = new SqlCommand("UPDATE REMSMINDANAO.dbo.asys_remoutsource_detail SET status = 'RECMLWB' where  refallbarcode = '" + DgEntry[i][0] + "' and status ='RELEASED'", con, tran))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd = new SqlCommand("UPDATE REMSMINDANAO.dbo.asys_REM_detail SET status = 'RECMLWB' where  refallbarcode = '" + DgEntry[i][0] + "' and  status ='RELEASED'", con, tran))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd = new SqlCommand("UPDATE REMSSHOWROOM.dbo.asys_remoutsource_detail SET status = 'RECMLWB' where  refallbarcode = '" + DgEntry[i][0] + "' and status ='RELEASED'", con, tran))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd = new SqlCommand("UPDATE REMSSHOWROOM.dbo.asys_REM_detail SET status = 'RECMLWB' where  refallbarcode = '" + DgEntry[i][0] + "' and  status ='RELEASED'", con, tran))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    
                    tran.Commit();
                    //tran.Rollback();
                    con.Close();
                    return new MLWB { respCode = "1", respMsg = "Success" };
                }
                catch (Exception ex)
                {
                    log.Error("saveData: "+ ex.Message);
                    tran.Rollback();
                    return new MLWB { respCode = "0", respMsg = "service error: " + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB MLWBReport() //Receiving
        {
            log.Info("Log in starts here: MLWBReport");
            var List = new MLWB();
            List.MLWBReportData = new MLWBReport();
            List.MLWBReportData.fullname = new List<string>();

            using (SqlConnection con = new SqlConnection(constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("select distinct upper(rtrim(fullname)) as fullname,blocked,job_title from vw_humresall where job_title in ('ALLBOSMAN', 'mlwb') and blocked =1  and fullname not in ('GENELOUIE NEMENO','PORTIA BRIONES','SHERYL LOVE ONG','ROWENA VILLENA') order by fullname", con))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.CommandTimeout = 0;
                        {
                            while (rdr.Read())
                            {
                                List.MLWBReportData.fullname.Add(rdr["fullname"].ToString());
                            }
                            rdr.Close();
                            con.Close();
                        }
                    }
                    return new MLWB { respCode = "1", respMsg = "Success", MLWBReportData = List.MLWBReportData };
                }
                catch (Exception ex)
                {
                    log.Error("MLWBReport: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error: " + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB MLWBReport2(string lotnum)//Receiving
        {
            log.Info("Logging starts here: MLWBReport2");
            MLWBReport2 model = new MLWBReport2();
            SqlDataReader rdr;
            
            using (SqlConnection con = new SqlConnection(constring))
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("select top 1 reflotno as lotno, receivedate from ASYS_MLWB_detail where status = 'received' and reflotno = '" + lotnum + "' order by receivedate desc", con))
                    {
                        rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.CommandTimeout = 0;
                        {
                            if (rdr.Read())
                            {
                                model.lotnum = rdr["lotno"].ToString();
                                model.date = string.IsNullOrEmpty(rdr["receivedate"].ToString()) ? "" : Convert.ToDateTime(rdr["receivedate"]).ToString("MM/dd/yyyy");
                            }
                        }
                        con.Close();
                        rdr.Close();
                        return new MLWB { respCode = "1", respMsg = "Success", MLWBReportData2 = model };
                    }
                }
                catch (Exception ex)
                {
                    log.Error("MLWBReport2: "+ ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error: " + ex.Message };
                }

        }
        [WebMethod]
        public MLWB MLWBReport3()//Receiving
        {
            log.Info("Logging stars here: MLWBReport3");
            MLWBReport2 model = new MLWBReport2();


            using (SqlConnection con = new SqlConnection(constring))
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("select top 1 reflotno as lotno,releasedate from ASYS_MLWB_detail where status = 'released' order by releasedate desc", con))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.CommandTimeout = 0;
                        {
                            if (rdr.Read())
                            {
                                model.date = rdr["releasedate"].ToString();
                                model.lotnum = rdr["lotno"].ToString();


                            }
                        }
                        return new MLWB { respCode = "1", respMsg = "Success", MLWBReportData2 = model };

                    }
                }
                catch (Exception ex)
                {
                    log.Error("MLWBReport3: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error: " + ex.Message };
                }

        }
        [WebMethod]
        public MLWB habwa_date(string sDB)//Receiving
        {
            log.Info("Logging starts here: habwa_date");
            habwa_date data = new habwa_date();
            SqlDataReader read;
            if (sDB == "")
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    con.Open();
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("select cast(month(getdate()) as int) as month, cast(year(getdate()) as int) as year", con))
                        {
                            read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                            if (read.Read())
                            {
                                data.month = read["month"].ToString();
                                data.year = read["year"].ToString();
                            }

                        }
                        return new MLWB { respCode = "1", respMsg = "Success", habwaDateData = data };
                    }
                    catch (Exception ex)
                    {
                        log.Error("habwa_date: " + ex.Message);
                        return new MLWB { respCode = "0", respMsg = "service error: " + ex.Message };
                    }

                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    con.Open();
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("use[REMS" + sDB + "]select cast(month(getdate()) as int) as month, cast(year(getdate()) as int) as year", con))
                        {
                            read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                            if (read.Read())
                            {
                                data.month = read["month"].ToString();
                                data.year = read["year"].ToString();

                            }
                        }
                        return new MLWB { respCode = "1", respMsg = "Success", habwaDateData = data };
                    }
                    catch (Exception ex)
                    {

                        return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                    }
                }
            }

        }
        [WebMethod]
        public MLWB cboreceive_SelectedIndexChanged(string cboreceive)//Receiving
        {
            log.Info("Logging strats here: cboreceive_SelectedIndexChanged");
            MLWBReport data = new MLWBReport();


            using (SqlConnection con = new SqlConnection(constring))
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Select rtrim(fullname) as fullname2 from  dbo.vw_humresall() where fullname='" + cboreceive + "'", con))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.CommandTimeout = 0;
                        {
                            while (rdr.Read())
                            {
                                data._fullname = rdr["fullname"].ToString();
                            }
                        }
                        return new MLWB { respCode = "1", respMsg = "Success", MLWBReportData = data };

                    }
                }
                catch (Exception ex)
                {
                    log.Error("cboreceive_SelectedIndexChanged: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error: " + ex.Message };
                }
        }
        [WebMethod]
        public MLWB  receiver()
        {
            log.Info("Logging strats here: receiver");
            var list = new MLWB();
            list.MLWBReportData = new MLWBReport();
            list.MLWBReportData.fullname = new List<string>();

            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("select distinct fullname from vw_humresall where job_title = 'MLWB' order by fullname asc", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (read.Read())
                        {
                            list.MLWBReportData.fullname.Add(read["fullname"].ToString().Trim());
                        }
                    }
                    return new MLWB { respCode = "1", respMsg = "Success", MLWBReportData = list.MLWBReportData };
                }
                catch (Exception ex)
                {
                    log.Error("receiver: " +ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB DetailList(string DgEntry)
        {
            log.Info("Logging starts here: DetailList");
            var list = new MLWB();
            list.RetrieveData2data = new RetrieveData2Models();
            list.RetrieveData2data.division = new List<string>();
            list.RetrieveData2data.ptn = new List<string>();
            list.RetrieveData2data.itemcode = new List<string>();
            list.RetrieveData2data.alldesc = new List<string>();
            list.RetrieveData2data.ptnitemdesc = new List<string>();
            list.RetrieveData2data.quantity = new List<string>();
            list.RetrieveData2data.barcode = new List<string>();


            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                {
                    try
                    {
                        for (int i = 0; i <= DgEntry.Count() - 1; i++)

                            using (SqlCommand cmd = new SqlCommand("Select distinct asys_REM_detail.refLOTNO AS lotno, asys_REM_header.branchname as division, asys_REM_detail.ptn as ptn, asys_REM_detail.itemcode as itemcode,asys_REM_detail.branchitemdesc as ptnitemdesc, asys_REM_detail.qty as quantity, asys_REM_detail.refallbarcode as barcode,all_desc from asys_REM_header inner join asys_REM_detail on asys_REM_header.ptn = asys_REM_detail.ptn where asys_REM_detail.refallBarcode = '" + DgEntry + "' union all Select refLOTNO AS lotno, branchcode as division, ptn as ptn, itemcode as itemcode, branchitemdesc as ptnitemdesc,qty as quantity, refallbarcode as barcode, all_desc as all_desc from asys_remoutsource_detail where refallBarcode ='" + DgEntry + "'", con))
                            {
                                read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                                if (read.Read())
                                {
                                    list.RetrieveData2data.division.Add(read["division"].ToString().Trim());
                                    list.RetrieveData2data.ptn.Add(read["ptn"].ToString().Trim());
                                    list.RetrieveData2data.itemcode.Add(read["itemcode"].ToString().Trim());
                                    list.RetrieveData2data.ptnitemdesc.Add(read["ptnitemdesc"].ToString().Trim());
                                    list.RetrieveData2data.alldesc.Add(read["all_desc"].ToString().Trim());
                                    list.RetrieveData2data.quantity.Add(read["quantity"].ToString().Trim());
                                    list.RetrieveData2data.barcode.Add(read["Barcode"].ToString().Trim());

                                }


                            }
                        return new MLWB { respCode = "1", respMsg = "Success", RetrieveData2data = list.RetrieveData2data };
                    }
                    catch (Exception ex)
                    {
                        log.Error("DetailList: " +ex.Message);
                        return new MLWB { respCode = "0", respMsg = "Service Error:" + ex.Message };
                    }
                }
            }
        }
        [WebMethod]
        public MLWB ReleasingLotnum() //Releasing
        {
            log.Info("logging starts here: ReleasingLotnum");
            try
            {
                var list2 = new MLWB();
                list2.releasingLotnumData = new releasingLotnum();
                list2.releasingLotnumData.lotnum = new List<string>();


                using (SqlConnection con = new SqlConnection(constring))
                {
                    con.Open();
                    string query = "Select distinct LOTno from asys_MLWB_detail where status = 'RECEIVED' order by lotno desc";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandTimeout = 0;
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.CommandTimeout = 0;

                        while (rdr.Read())
                        {
                            list2.releasingLotnumData.lotnum.Add(rdr["LOTno"].ToString());//.ToUpper());
                        }
                    }
                    con.Close();
                    return new MLWB { respCode = "1", respMsg = "Success", releasingLotnumData = list2.releasingLotnumData };
                }

            }

            catch (Exception ex)
            {
                log.Error("ReleasingLotnum: " + ex.Message);
                return new MLWB { respCode = "0", respMsg = "Error: " + ex.Message };
            }
        }
        [WebMethod]
        public MLWB GenerateLot()
        {
            log.Info("Logging starts here: GenerateLot");
            SqlDataReader read;
            var list2 = new MLWB();
            list2.releasingLotnumData = new releasingLotnum();
            list2.releasingLotnumData.lotnum = new List<string>();

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("select lotno from ASYS_Lotno_gen", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.Read())
                        {
                            list2.releasingLotnumData.lotnum.Add(read["lotno"].ToString());
                        }
                    }
                    return new MLWB { respCode = "1", respMsg = "success!", releasingLotnumData = list2.releasingLotnumData };
                }
                catch (Exception ex)
                {
                    log.Error("GenerateLot" + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Error: " + ex.Message };
                }


            }

        }
        [WebMethod]
        public MLWB lotnorefresh() //Releasing
        {

            log.Info("Logging starts here: lotnorefresh");
            try
            {
                var list2 = new MLWB();
                list2.releasingLotnumData = new releasingLotnum();
                list2.releasingLotnumData.lotnum = new List<string>();


                using (SqlConnection con = new SqlConnection(constring))
                {
                    con.Open();
                    string query = "Select distinct reflotno from ASYS_REM_Detail  where status = 'RECEIVED'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandTimeout = 0;
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.CommandTimeout = 0;

                        while (rdr.Read())
                        {
                            list2.releasingLotnumData.lotnum.Add(rdr["reflotno"].ToString());//.ToUpper());
                        }
                    }
                    con.Close();
                    return new MLWB { respCode = "1", respMsg = "Success", releasingLotnumData = list2.releasingLotnumData };
                }

            }

            catch (Exception ex)
            {
                log.Error("lotnorefresh: " + ex.Message);
                return new MLWB { respCode = "0", respMsg = "Error: " + ex.Message };
            }
        }
        [WebMethod]
        public MLWB getBarcodeDetails(string cmbbarcode) //Releasing
        {

            log.Info("Logging starts here: getBarcodeDetails");
            var List = new MLWB();
            List.RetrieveData2data = new RetrieveData2Models();
            SqlDataReader rdr;
            try
            {

                using (SqlConnection con = new SqlConnection(constring))
                {
                    con.Open();
                    string query = "SELECT RefallBarcode, PTN, RefItemcode, RefQty, ALL_desc as [desc], ALL_karat as karat, ALL_carat as carat, SerialNo, ALL_price as cost, ALL_Weight as wt,status FROM dbo.ASYS_MLWB_detail WHERE refallbarcode = '" + cmbbarcode + "' AND status = 'RECEIVED'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {


                        cmd.CommandTimeout = 0;
                        rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (rdr.HasRows)
                        {

                            while (rdr.Read())
                            {
                                List.RetrieveData2data._barcode = rdr["refallbarcode"].ToString();
                                List.RetrieveData2data.status = rdr["status"].ToString();
                                List.RetrieveData2data._alldesc = rdr["desc"].ToString();
                                List.RetrieveData2data._weight = rdr["wt"].ToString();
                                List.RetrieveData2data._karat = rdr["karat"].ToString();
                                List.RetrieveData2data._carat = rdr["carat"].ToString();
                                List.RetrieveData2data.cost = rdr["cost"].ToString();
                                List.RetrieveData2data._ptn = rdr["ptn"].ToString();
                                if (List.RetrieveData2data.cost == "null" || List.RetrieveData2data.cost == "" || List.RetrieveData2data.cost == "0")
                                {
                                    List.RetrieveData2data.cost = "0.00";
                                }



                                fetch = "Success";
                                code = "1";

                            }


                        }
                        else
                        {
                            fetch = "No data found";
                            code = "0";
                        }
                    }
                    con.Close();

                }
                return new MLWB { respCode = code, respMsg = fetch, RetrieveData2data = List.RetrieveData2data };
            }


            catch (Exception ex)
            {
                log.Error("getBarcodeDetails: " + ex.Message);
                return new MLWB { respCode = "0", respMsg = "Error: " + ex.Message };
            }
        }
        [WebMethod]
        public MLWB cb_KeyPress() //Releasing
        {
            log.Info("Logging starts here: cb_KeyPress");
            var List = new MLWB();
            List.data1 = new getLotNumber();
            try
            {

                using (SqlConnection con = new SqlConnection(constring))
                {
                    con.Open();
                    string query = "Select distinct refLOTno from asys_MLWB_detail where status = 'RECEIVED' ";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandTimeout = 0;
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.CommandTimeout = 0;

                        while (rdr.Read())
                        {

                            List.data1.lotNumber.Add(rdr["refLOTno"].ToString());
                        }
                    }
                    con.Close();
                    return new MLWB { respCode = "1", respMsg = "Success", data1 = List.data1 };
                }

            }

            catch (Exception ex)
            {
                log.Error("cb_KeyPress: " + ex.Message);
                return new MLWB { respCode = "0", respMsg = "Error: " + ex.Message };
            }
        }
        [WebMethod]
        public MLWB saveblehtrue(string templot, string[][] DgEntry, string userlog, string userlog1) //Releasing
        {
            log.Info("Logging starts here: saveblehtrue");
            SqlTransaction tran;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("Update ASYS_Lotno_Gen set lotno ='" + templot + "' WHERE BusinessCenter ='MLWB'", con, tran))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    for (int i = 0; i <= DgEntry.Count() - 1; i++)
                    {
                        if (DgEntry[i][0] != null)

                            using (SqlCommand cmd = new SqlCommand("insert into asys_barcodehistory (lotno, refallbarcode,allbarcode, itemcode, [description], karat, carat, SerialNo, weight, currency, price, cost, empname, custodian, trandate, costcenter, consignto, status ) select '" + templot + "', refallbarcode as refallbarcode, refallbarcode as allbarcode, refitemcode, all_desc, price_karat, price_carat,SerialNo, price_weight, currency, all_price, all_cost,'" + userlog + "' as  empname, '" + userlog + "' as custodian, getdate() as trandate, 'MLWB', 'NULL', 'RELEASED' from asys_MLWB_detail where refallbarcode='" + DgEntry + "' and status = 'RECEIVED'", con))
                            {
                                cmd.ExecuteNonQuery();
                            }

                        using (SqlCommand cmd = new SqlCommand("Update asys_MLWB_detail  set reflotno ='" + templot + "', releasedate =getdate(), releaser='" + userlog1 + "', status = 'RELEASED' where  refallbarcode='" + DgEntry + "'  and status = 'RECEIVED'", con))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                    return new MLWB { respCode = "1", respMsg = "success!" };
                }
                catch (Exception ex)
                {
                    log.Error("saveblehtrue: " + ex.Message);
                    tran.Rollback();
                    return new MLWB { respCode = "0", respMsg = "Service Error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB releaser()
        {
            log.Info("Logging starts here: releaser");
            var list = new MLWB();
            list.MLWBReportData = new MLWBReport();
            list.MLWBReportData.fullname = new List<string>();


            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("select fullname from vw_humresvismin where job_title = 'MLWB'", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (read.Read())
                        {
                            list.MLWBReportData.fullname.Add(read["fullname"].ToString());
                        }

                    }
                    return new MLWB { respCode = "1", respMsg = "success", MLWBReportData = list.MLWBReportData };
                }
                catch (Exception ex)
                {
                    log.Error("releaser: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB CallCostCenter()
        {
            log.Info("Logging starts here: CallCostCenter");
            details data = new details();
            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("Select * from vw_CostCenter", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (read.Read())
                        {
                            data.CostDept = read["CostDept"].ToString();
                        }
                    }
                    return new MLWB { respCode = "1", respMsg = "Success", MLWBInventory = data };
                }
                catch (Exception ex)
                {
                    log.Error("CallCostCenter: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB RetrieveAction()
        {
            log.Info("Logging starts here: RetrieveAction");
            var list = new MLWB();
            list.RetrieveData2data = new RetrieveData2Models();
            list.RetrieveData2data.sortaction_id = new List<string>();

            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("select a.sortaction_id, b.action_type from (select distinct sortaction_id from tbl_receiving_header where status_id = 2 or status_id = 3) a inner join tbl_action b on a.sortaction_id = b.action_id  order by b.action_type", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                list.RetrieveData2data.sortaction_id.Add(read["sortaction_id"].ToString());
                            }
                        }

                    }

                    return new MLWB { respCode = "1", respMsg = "Success", RetrieveData2data = list.RetrieveData2data };
                }
                catch (Exception ex)
                {
                    log.Error("RetrieveAction: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public dbConnection DatabaseConnections(string db, string barcode)//Edit Item
        {
            log.Info("Logging starts here: DatabaseConnections");
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("USE [" + db + "] IF EXISTS(SELECT refallbarcode FROM ASYS_REM_Detail WHERE refallbarcode = '" + barcode + "') " +
                         "SELECT refallbarcode FROM ASYS_REM_Detail WHERE refallbarcode = '" + barcode + "'  ELSE  " +
                         "SELECT refallbarcode FROM ASYS_REMOutsource_Detail WHERE refallbarcode = '" + barcode + "'", con))
                    {
                        cmd.CommandTimeout = 30000;
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {

                        }
                    }
                    return new dbConnection { respons = "1", result = "success: " };
                }
                catch (Exception ex)
                {
                    log.Error("DatabaseConnections: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }



        }
        [WebMethod]
        public dbConnection getOutsourceData(string barcode) //Edit Item
        {
            log.Info("Logging starts here: getOutsourceData");
            dbConnection display = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT itemid, ptn, ptnbarcode, branchcode, branchname, loanvalue, refitemcode, itemcode, branchitemdesc, " +
                        "refqty, qty, karatgrading, caratsize, SerialNo, weight, actionclass, sortcode, all_desc, all_karat, " +
                        "all_carat, all_weight, currency, all_cost, photoname, all_price, appraisevalue, cellular_cost, " +
                        "watch_cost, repair_cost, cleaning_cost, gold_cost, mount_cost, yg_cost, wg_cost, status, " +
                        "SerialNo FROM ASYS_REMOutsource_Detail WHERE refallbarcode='" + barcode + "' AND status NOT IN ('RELEASED','RECMLWB')", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            display.status1 = dr["STATUS"].ToString();
                            display.photoname = dr["photoname"].ToString();
                            display.status = dr["status"].ToString();
                            display.branchcode = dr["branchcode"].ToString();
                            display.branchname = dr["branchname"].ToString();
                            display.actionclass = dr["actionclass"].ToString();
                            display.ptn = dr["ptn"].ToString();
                            display.ptnbarcode = dr["ptnbarcode"].ToString();
                            display.appraisevalue = dr["appraisevalue"].ToString();
                            display.LoanValue = dr["loanvalue"].ToString();
                            display.sortcode = dr["sortcode"].ToString();
                            display.currency = dr["currency"].ToString();
                            display.itemid = dr["itemid"].ToString();
                            display.ItemCode = dr["itemcode"].ToString();
                            display.branchitemdesc = dr["branchitemdesc"].ToString();
                            display.qty = dr["qty"].ToString();
                            display.Karatgrading = dr["karatgrading"].ToString();
                            display.Caratsize = dr["caratsize"].ToString();
                            display.Weight = dr["weight"].ToString();
                            display.all_cost = dr["all_cost"].ToString();
                            display.all_desc = dr["all_desc"].ToString();
                            display.SerialNo = dr["SerialNo"].ToString();
                            display.ALL_Karat = dr["ALL_Karat"].ToString();
                            display.ALL_Carat = dr["ALL_Carat"].ToString();
                            display.ALL_Weight = dr["ALL_Weight"].ToString();
                            display.ALL_price = dr["ALL_price"].ToString();
                            display.cellular_cost = dr["cellular_cost"].ToString();
                            display.watch_cost = dr["watch_cost"].ToString();
                            display.repair_cost = dr["repair_cost"].ToString();
                            display.cleaning_cost = dr["cleaning_cost"].ToString();
                            display.gold_cost = dr["gold_cost"].ToString();
                            display.mount_cost = dr["mount_cost"].ToString();
                            display.YG_cost = dr["YG_cost"].ToString();
                            display.WG_cost = dr["WG_cost"].ToString();
                        }
                    }

                    return new dbConnection { respons = "1", result = "success: " };
                }
                catch (Exception ex)
                {
                    log.Error("getOutsourceData: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }


        }
        [WebMethod]
        public dbConnection getREMData(string barcode)//Edit Item
        {
            log.Info("Logging starts here: getREMData");
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT 1 as stat,itemid,actionclass,sortcode,status,ptn,refallbarcode,refitemcode,itemcode,branchitemdesc, " +
                        "refqty,qty,karatgrading,caratsize,weight,all_desc,SerialNo,all_karat,all_carat,all_weight,currency,all_cost, " +
                        "photoname,all_price,cellular_cost,watch_cost,repair_cost,cleaning_cost,Gold_cost,mount_cost,yg_cost,wg_cost, " +
                        "appraisevalue,'0' AS loanvalue FROM [REMS].DBO.ASYS_REM_Detail " +
                        "WHERE refALLBarcode = '" + barcode + "' AND status NOT IN ('RELEASED','RECMLWB') " +
                        "UNION " +
                        "SELECT 0 as stat,itemid,actionclass,sortcode,status,ptn,refallbarcode,refitemcode,itemcode,branchitemdesc," +
                        "refqty,qty,karatgrading,caratsize,weight,all_desc,SerialNo,all_karat,all_carat,all_weight,currency,all_cost," +
                        "photoname,all_price,cellular_cost,watch_cost,repair_cost,cleaning_cost,Gold_cost,mount_cost,yg_cost,wg_cost, " +
                        "appraisevalue,'0' AS loanvalue FROM [REMS].DBO.ASYS_REMOUTSOURCE_Detail " +
                        "WHERE refALLBarcode = '" + barcode + "' AND status NOT IN ('RELEASED','RECMLWB')", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {

                        }
                    }

                    return new dbConnection { respons = "1", result = "success: " };
                }
                catch (Exception ex)
                {
                    log.Error("getREMData: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }


        }
        [WebMethod]
        public dbConnection getMLWBData(string barcode)//Edit Item
        {
            log.Info("Logging starts here: getMLWBData");
            dbConnection display = new dbConnection();
            //MLWBViewgetMLWBData display = new MLWBViewgetMLWBData();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT RefLotno, RefallBarcode, PTN, ItemID, PTNBarcode, BranchCode, BranchName, Loanvalue, RefItemcode, Itemcode, BranchItemDesc, RefQty, Qty, " +
                            "KaratGrading, CaratSize, Weight,Actionclass,Sortcode, ALL_desc, SerialNo, ALL_karat, ALL_carat, ALL_Cost, ALL_Weight,currency, PhotoName,all_price, " +
                            "Cellular_cost, Watch_cost, Repair_cost,Cleaning_cost, Gold_cost, Mount_cost, YG_cost, WG_cost, MaturityDate, ExpiryDate, LoanDate, Status " +
                            "FROM DBO.ASYS_MLWB_Detail WHERE refallbarcode = '" + barcode + "' AND status <> 'RECPRICING' ", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.CommandTimeout = 30000;
                        if (dr.Read())
                        {
                            display.status1 = dr["STATUS"].ToString();
                            display.photoname = dr["photoname"].ToString();
                            display.status = dr["status"].ToString();
                            display.branchcode = dr["branchcode"].ToString();
                            display.branchname = dr["branchname"].ToString();
                            display.actionclass = dr["actionclass"].ToString();
                            display.sortcode = dr["sortcode"].ToString();
                            display.ptn = dr["ptn"].ToString();
                            display.ptnbarcode = dr["ptnbarcode"].ToString();
                            display.Maturitydate = dr["Maturitydate"].ToString();
                            display.Expirydate = dr["Expirydate"].ToString();
                            display.LoanValue = dr["LoanValue"].ToString();
                            display.currency = dr["currency"].ToString();
                            display.itemid = dr["itemid"].ToString();
                            display.ItemCode = dr["ItemCode"].ToString();
                            display.branchitemdesc = dr["branchitemdesc"].ToString();
                            display.qty = dr["qty"].ToString();
                            display.Karatgrading = dr["Karatgrading"].ToString();
                            display.Caratsize = dr["Caratsize"].ToString();
                            display.Weight = dr["Weight"].ToString();
                            display.all_cost = dr["all_cost"].ToString();
                            display.all_desc = dr["all_desc"].ToString();
                            display.SerialNo = dr["SerialNo"].ToString();
                            display.refqty = dr["refqty"].ToString();
                            display.ALL_Karat = dr["ALL_Karat"].ToString();
                            display.ALL_Carat = dr["ALL_Carat"].ToString();
                            display.ALL_Weight = dr["ALL_Weight"].ToString();
                            display.all_cost = dr["all_cost"].ToString();
                            display.cellular_cost = dr["cellular_cost"].ToString();
                            display.watch_cost = dr["watch_cost"].ToString();
                            display.repair_cost = dr["repair_cost"].ToString();
                            display.cleaning_cost = dr["cleaning_cost"].ToString();
                            display.gold_cost = dr["gold_cost"].ToString();
                            display.mount_cost = dr["mount_cost"].ToString();
                            display.YG_cost = dr["YG_cost"].ToString();
                            display.WG_cost = dr["WG_cost"].ToString();
                            //display.questionmark2 = dr["questionmark2"].ToString();


                            //data.year = Convert.ToInt32(dr["year"]);
                            //List.RetrieveData2data.barcode.Add(models.isNull(dr["barcode"]).Trim());
                        }
                    }

                    //return new MLWB { respCode = "0", respMsg = "success: ", MLWBViewMLWB = display.STATUS};
                    return new dbConnection { respons = "1", result = "success: ", status1 = display.status1, photoname = display.photoname, status = display.status, branchcode = display.branchcode, branchname = display.branchname, actionclass = display.actionclass, sortcode = display.sortcode, ptn = display.ptn, ptnbarcode = display.ptnbarcode, Maturitydate = display.Maturitydate, Expirydate = display.Expirydate, LoanValue = display.LoanValue, LoanDate = display.LoanDate, currency = display.currency, itemid = display.itemid, ItemCode = display.ItemCode, branchitemdesc = display.branchitemdesc, qty = display.qty, Karatgrading = display.Karatgrading, Caratsize = display.Caratsize, Weight = display.Weight, all_cost = display.all_cost, all_desc = display.all_desc, SerialNo = display.SerialNo, refqty = display.refqty, ALL_Karat = display.ALL_Karat, ALL_Carat = display.ALL_Carat, ALL_Weight = display.ALL_Weight, ALL_price = display.ALL_price, cellular_cost = display.cellular_cost, watch_cost = display.watch_cost, repair_cost = display.repair_cost, cleaning_cost = display.cleaning_cost, gold_cost = display.gold_cost, mount_cost = display.mount_cost, YG_cost = display.YG_cost, WG_cost = display.WG_cost };

                }
                catch (Exception ex)
                {
                    log.Error("getMLWBData: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }


        }
        [WebMethod]
        public dbConnection MLWBDisplayAction(string actionClass)//Edit Item
        {

            log.Info("Logging starts here: MLWBDisplayAction");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT CostA, CostB, CostC, CostD FROM tbl_action WHERE action_type ='" + actionClass + "'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {

                            //call.CostA = Convert.ToDouble(dr["CostA"]);
                            //call.CostB = Convert.ToDouble(dr["CostB"]);
                            //call.CostC = Convert.ToDouble(dr["CostC"]);
                            //call.CostD = Convert.ToDouble(dr["CostD"]);

                            call.CostA = string.IsNullOrEmpty(dr["CostA"].ToString()) ? 0 : Convert.ToDouble(dr["CostA"]);
                            call.CostB = string.IsNullOrEmpty(dr["CostB"].ToString()) ? 0 : Convert.ToDouble(dr["CostB"]);
                            call.CostC = string.IsNullOrEmpty(dr["CostC"].ToString()) ? 0 : Convert.ToDouble(dr["CostC"]);
                            call.CostD = string.IsNullOrEmpty(dr["CostD"].ToString()) ? 0 : Convert.ToDouble(dr["CostD"]);



                        }

                    }
                    return new dbConnection { respons = "1", result = "success: ", CostA = call.CostA, CostB = call.CostB, CostC = call.CostC, CostD = call.CostD };

                }
                catch (Exception ex)
                {
                    log.Error("MLWBDisplayAction: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection goodstockOutsource(string barcode)//Edit Item
        {

            log.Info("Logging starts here: goodstockOutsource");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Gold_Cost, Mount_Cost, YG_Cost, WG_Cost, ALL_Cost FROM ASYS_MLWB_DEtail WHERE refAllbarcode ='" + barcode + "' AND status <> 'RECPRICING'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {


                            //call.gold_cost = dr["Gold_Cost"].ToString();
                            //call.mount_cost = dr["Mount_Cost"].ToString();
                            //call.YG_cost = dr["YG_Cost"].ToString();
                            //call.WG_cost = dr["WG_Cost"].ToString();
                            //call.all_cost = dr["ALL_Cost"].ToString();

                            call.gold_cost = string.IsNullOrEmpty(dr["Gold_Cost"].ToString()) ? "0.00" : dr["Gold_Cost"].ToString().Trim();
                            call.mount_cost = string.IsNullOrEmpty(dr["Mount_Cost"].ToString()) ? "0.00" : dr["Mount_Cost"].ToString().Trim();
                            call.YG_cost = string.IsNullOrEmpty(dr["YG_Cost"].ToString()) ? "0.00" : dr["YG_Cost"].ToString().Trim();
                            call.WG_cost = string.IsNullOrEmpty(dr["WG_Cost"].ToString()) ? "0.00" : dr["WG_Cost"].ToString().Trim();
                            call.all_cost = string.IsNullOrEmpty(dr["ALL_Cost"].ToString()) ? "0.00" : dr["ALL_Cost"].ToString().Trim();

                        }
                    }
                    return new dbConnection { respons = "1", result = "success ", gold_cost = call.gold_cost, mount_cost = call.mount_cost, YG_cost = call.YG_cost, WG_cost = call.WG_cost, all_cost = call.all_cost };
                }

                catch (Exception ex)
                {
                    log.Error("goodstockOutsource: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection cellular(string barcode)//Edit Item
        {
            log.Info("Logging starts here: cellular");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Cellular_Cost, Repair_Cost, Cleaning_Cost,ALL_Cost FROM ASYS_MLWB_DEtail WHERE refAllbarcode ='" + barcode + "' AND status <> 'RECPRICING'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            //call.cellular_cost = dr["Cellular_Cost"].ToString();
                            //call.repair_cost = dr["Repair_Cost"].ToString();
                            //call.cleaning_cost = dr["Cleaning_Cost"].ToString();
                            //call.all_cost = dr["ALL_Cost"].ToString();

                            call.cellular_cost = string.IsNullOrEmpty(dr["Cellular_Cost"].ToString()) ? "0.00" : dr["Cellular_Cost"].ToString().Trim();
                            call.repair_cost = string.IsNullOrEmpty(dr["Repair_Cost"].ToString()) ? "0.00" : dr["Repair_Cost"].ToString().Trim();
                            call.cleaning_cost = string.IsNullOrEmpty(dr["Cleaning_Cost"].ToString()) ? "0.00" : dr["Cleaning_Cost"].ToString().Trim();
                            call.all_cost = string.IsNullOrEmpty(dr["ALL_Cost"].ToString()) ? "0.00" : dr["ALL_Cost"].ToString().Trim();

                        }
                    }
                    return new dbConnection { respons = "1", result = "success ", cellular_cost = call.cellular_cost, repair_cost = call.repair_cost, cleaning_cost = call.cleaning_cost, all_cost = call.all_cost };
                }
                catch (Exception ex)
                {
                    log.Error("cellular: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection watch(string barcode)//Edit Item
        {
            log.Info("Logging starts here: watch");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Watch_Cost, Repair_Cost, Cleaning_Cost, ALL_Cost FROM ASYS_MLWB_DEtail WHERE refAllbarcode ='" + barcode + "' AND status <> 'RECPRICING'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            //call.watch_cost = dr["Watch_Cost"].ToString();
                            //call.repair_cost = dr["Repair_Cost"].ToString();
                            //call.cleaning_cost = dr["Cleaning_Cost"].ToString();
                            //call.all_cost = dr["ALL_Cost"].ToString();

                            
                            call.watch_cost = string.IsNullOrEmpty(dr["Watch_Cost"].ToString()) ? "0.00" : dr["Watch_Cost"].ToString().Trim();
                            call.repair_cost = string.IsNullOrEmpty(dr["Repair_Cost"].ToString()) ? "0.00" : dr["Repair_Cost"].ToString().Trim();
                            call.repair_cost = string.IsNullOrEmpty(dr["Cleaning_Cost"].ToString()) ? "0.00" : dr["Cleaning_Cost"].ToString().Trim();
                            call.repair_cost = string.IsNullOrEmpty(dr["ALL_Cost"].ToString()) ? "0.00" : dr["ALL_Cost"].ToString().Trim();

                        }
                    }
                    return new dbConnection { respons = "1", result = "success ", watch_cost = call.watch_cost, repair_cost = call.repair_cost, cleaning_cost = call.cleaning_cost, all_cost = call.all_cost };
                }
                catch (Exception ex)
                {
                    log.Error("watch: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection forcosting(string ptn)//Edit Item
        {
            log.Info("Logging starts here: forcosting");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT branchcode, branchname, ptn, ptnbarcode, loanvalue, maturitydate, expirydate, loandate FROM ASYS_REM_Header WHERE ptn = '" + ptn + "'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.branchcode = dr["branchcode"].ToString();
                            call.branchname = dr["branchname"].ToString();
                            //call.LoanValue = dr["loanvalue"].ToString(); 
                            call.LoanValue = string.IsNullOrEmpty(dr["LoanValue"].ToString()) ? "0.00" : dr["LoanValue"].ToString().Trim();
                            call.LoanDate = dr["LoanDate"].ToString();
                            //call.Maturitydate = dr["MaturityDate"].ToString();
                            //call.Expirydate = dr["ExpiryDate"].ToString();
                            call.Maturitydate = Convert.ToDateTime(dr["MaturityDate"]).ToString("MMMM dd, yyyy").Trim();
                            call.Expirydate = Convert.ToDateTime(dr["ExpiryDate"]).ToString("MMMM dd, yyyy").Trim();
                            call.ptnbarcode = dr["ptnbarcode"].ToString();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success", branchcode = call.branchcode, branchname = call.branchname, LoanValue = call.LoanValue, LoanDate = call.LoanDate, Maturitydate = call.Maturitydate, Expirydate = call.Expirydate, ptnbarcode = call.ptnbarcode };
                }
                catch (Exception ex)
                {
                    log.Error("forcosting: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection REMDisplayAction(string actionClass)//Edit Item
        {
            log.Info("Logging starts here: REMDisplayAction");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT CostA, CostB, CostC, CostD FROM tbl_action WHERE action_type ='" + actionClass + "'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {

                            //call.CostA = Convert.ToDouble(dr["CostA"]);
                            //call.CostB = Convert.ToDouble(dr["CostB"]);
                            //call.CostC = Convert.ToDouble(dr["CostC"]);
                            //call.CostD = Convert.ToDouble(dr["CostD"]);

                            call.CostA = string.IsNullOrEmpty(dr["CostA"].ToString()) ? 0 : Convert.ToDouble(dr["CostA"]);
                            call.CostB = string.IsNullOrEmpty(dr["CostB"].ToString()) ? 0 : Convert.ToDouble(dr["CostB"]);
                            call.CostC = string.IsNullOrEmpty(dr["CostC"].ToString()) ? 0 : Convert.ToDouble(dr["CostC"]);
                            call.CostD = string.IsNullOrEmpty(dr["CostD"].ToString()) ? 0 : Convert.ToDouble(dr["CostD"]);

                        }
                    }

                    return new dbConnection { respons = "1", result = "success", CostA = call.CostA, CostB = call.CostB, CostC = call.CostC, CostD = call.CostD };
                }
                catch (Exception ex)
                {
                    log.Error("REMDisplayAction: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }

        }
        [WebMethod]
        public dbConnection OVERAPPRAISED_PROCESS_COSTING(string ptn)//Edit Item
        {
            log.Info("Logging starts here: OVERAPPRAISED_PROCESS_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT ALL_Weight, ALL_Karat, ALL_Cost FROM ASYS_REM_Detail WHERE ptn = " + ptn + " AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.ALL_Weight  = string.IsNullOrEmpty(dr["ALL_Weight"].ToString()) ? "0.00" : dr["ALL_Weight"].ToString().Trim();
                            call.ALL_Karat  = string.IsNullOrEmpty(dr["ALL_Karat"].ToString()) ? "0.00" : dr["ALL_Karat"].ToString().Trim();
                            call.all_cost  = string.IsNullOrEmpty(dr["ALL_Cost"].ToString()) ? "0.00" : dr["ALL_Cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success",
                    ALL_Weight = call.ALL_Weight ,
                    ALL_Karat = call.ALL_Karat,
                    all_cost = call.all_cost 
                    };
                }
                catch (Exception ex)
                {
                    log.Info("Logging starts here: OVERAPPRAISED_PROCESS_COSTING");
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection GOODSTOCK_PROCESS_COSTING(string barcode)//Edit Item
        {
            log.Info("Logging starts here: GOODSTOCK_PROCESS_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Gold_Cost, Mount_Cost, YG_Cost, WG_Cost, ALL_Cost FROM ASYS_REM_Detail WHERE refAllbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.gold_cost = string.IsNullOrEmpty(dr["Gold_Cost"].ToString()) ? "0.00" : dr["Gold_Cost"].ToString().Trim();
                            call.mount_cost  = string.IsNullOrEmpty(dr["Mount_Cost"].ToString()) ? "0.00" : dr["Mount_Cost"].ToString().Trim();
                            call.YG_cost = string.IsNullOrEmpty(dr["YG_Cost"].ToString()) ? "0.00" : dr["YG_Cost"].ToString().Trim();
                            call.WG_cost  = string.IsNullOrEmpty(dr["WG_Cost"].ToString()) ? "0.00" : dr["WG_Cost"].ToString().Trim();
                            call.all_cost  = string.IsNullOrEmpty(dr["ALL_Cost"].ToString()) ? "0.00" : dr["ALL_Cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success",
                    gold_cost = call.gold_cost,
                    mount_cost = call.mount_cost,
                    YG_cost = call.YG_cost,
                    all_cost = call.all_cost 
                    };
                }
                catch (Exception ex)
                {
                    log.Error("GOODSTOCK_PROCESS_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection TAKENBACK_PROCESS_COSTING(string ptn, string barcode)//Edit Item
        {
            log.Info("Logging starts here: TAKENBACK_PROCESS_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT ALL_Cost FROM ASYS_REM_Detail WHERE ptn = " + ptn + " and refAllbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.all_cost  = string.IsNullOrEmpty(dr["All_cost"].ToString()) ? "0.00" : dr["All_cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success",
                        all_cost = call.all_cost 
                    };
                }
                catch (Exception ex)
                {
                    log.Error("TAKENBACK_PROCESS_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection CELLULAR_PROCESS_COSTING(string barcode)//Edit Item
        {
            log.Info("Logging starts here: CELLULAR_PROCESS_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Cellular_Cost, Repair_Cost, Cleaning_Cost,ALL_Cost FROM ASYS_REM_Detail WHERE refAllbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.cellular_cost = string.IsNullOrEmpty(dr["Cellular_cost"].ToString()) ? "0.00" : dr["Cellular_cost"].ToString().Trim();
                            call.repair_cost = string.IsNullOrEmpty(dr["Repair_cost"].ToString()) ? "0.00" : dr["Repair_cost"].ToString().Trim();
                            call.cleaning_cost = string.IsNullOrEmpty(dr["Cleaning_Cost"].ToString()) ? "0.00" : dr["Cleaning_Cost"].ToString().Trim();
                            call.all_cost  = string.IsNullOrEmpty(dr["All_cost"].ToString()) ? "0.00" : dr["All_cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success",
                    cellular_cost = call.cellular_cost,
                    repair_cost = call.repair_cost,
                    cleaning_cost = call.cleaning_cost,
                    all_cost = call.all_cost};
                }
                catch (Exception ex)
                {
                    log.Error("CELLULAR_PROCESS_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection WATCH_PROCESS_COSTING(string barcode)//Edit Item
        {
            log.Info("Logging starts here: WATCH_PROCESS_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Watch_Cost, Repair_Cost, Cleaning_Cost, ALL_Cost FROM ASYS_REM_Detail WHERE refAllbarcode ='" + barcode + "'AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.watch_cost  = string.IsNullOrEmpty(dr["Watch_Cost"].ToString()) ? "0.00" : dr["Watch_Cost"].ToString().Trim();
                            call.repair_cost  = string.IsNullOrEmpty(dr["Repair_cost"].ToString()) ? "0.00" : dr["Repair_cost"].ToString().Trim();
                            call.cleaning_cost = string.IsNullOrEmpty(dr["Cleaning_Cost"].ToString()) ? "0.00" : dr["Cleaning_Cost"].ToString().Trim();
                            call.all_cost  = string.IsNullOrEmpty(dr["All_cost"].ToString()) ? "0.00" : dr["All_cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success" ,
                    watch_cost = call.watch_cost,
                    repair_cost = call.repair_cost,
                    cleaning_cost = call.cleaning_cost,
                    all_cost = call.all_cost };
                }
                catch (Exception ex)
                {
                    log.Error("WATCH_PROCESS_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection OVERAPPRAISED_item_costing(string barcode)//Edit Item
        {
            log.Info("Logging starts here: OVERAPPRAISED_item_costing");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT ALL_Weight, ALL_Karat,ALL_Cost FROM ASYS_REMOutsource_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.ALL_Weight  = string.IsNullOrEmpty(dr["ALL_Weight"].ToString()) ? "0.00" : dr["ALL_Weight"].ToString().Trim();
                            call.ALL_Karat  = string.IsNullOrEmpty(dr["ALL_Karat"].ToString()) ? "0.00" : dr["ALL_Karat"].ToString().Trim();
                            
                            call.all_cost  = string.IsNullOrEmpty(dr["All_cost"].ToString()) ? "0.00" : dr["All_cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success" ,
                    ALL_Weight = call.ALL_Weight ,
                    ALL_Karat = call.ALL_Karat ,
                    all_cost = call.all_cost 
                    };
                }
                catch (Exception ex)
                {
                    log.Error("OVERAPPRAISED_item_costing: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection GOODSTOCK_item_costing(string barcode)//Edit Item
        {
            log.Info("Logging starts here: GOODSTOCK_item_costing");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Gold_Cost, Mount_Cost, YG_Cost, WG_Cost, ALL_Cost FROM ASYS_REMOutsource_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.gold_cost  = string.IsNullOrEmpty(dr["Gold_Cost"].ToString()) ? "0.00" : dr["Gold_Cost"].ToString().Trim();
                            call.mount_cost = string.IsNullOrEmpty(dr["Mount_Cost"].ToString()) ? "0.00" : dr["Mount_Cost"].ToString().Trim();
                            call.YG_cost  = string.IsNullOrEmpty(dr["YG_Cost"].ToString()) ? "0.00" : dr["YG_Cost"].ToString().Trim();
                            call.WG_cost = string.IsNullOrEmpty(dr["WG_Cost"].ToString()) ? "0.00" : dr["WG_Cost"].ToString().Trim();
                            call.all_cost = string.IsNullOrEmpty(dr["ALL_Cost"].ToString()) ? "0.00" : dr["ALL_Cost"].ToString().Trim();

                        }
                    }
                    return new dbConnection { respons = "1", result = "success" ,
                    gold_cost = call.gold_cost,
                    mount_cost = call.mount_cost,
                    YG_cost = call.YG_cost,
                    all_cost = call.all_cost 
                    };
                }
                catch (Exception ex)
                {
                    log.Error("GOODSTOCK_item_costing: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection TAKENBACK_item_costing(string barcode)//Edit Item
        {
            log.Info("Logging starts here: TAKENBACK_item_costing");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT  ALL_Cost FROM ASYS_REMOutsource_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.all_cost = string.IsNullOrEmpty(dr["All_cost"].ToString()) ? "0.00" : dr["All_cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success" ,
                    all_cost = call.all_cost};
                }
                catch (Exception ex)
                {
                    log.Error("TAKENBACK_item_costing: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection CELLULAR_item_costing(string barcode)//Edit Item
        {
            log.Info("Logging starts here: CELLULAR_item_costing");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Cellular_Cost, Repair_Cost, Cleaning_Cost,ALL_Cost FROM ASYS_REMOutsource_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.cellular_cost  = string.IsNullOrEmpty(dr["Cellular_cost"].ToString()) ? "0.00" : dr["Cellular_cost"].ToString().Trim();
                            call.repair_cost  = string.IsNullOrEmpty(dr["Repair_cost"].ToString()) ? "0.00" : dr["Repair_cost"].ToString().Trim();
                            call.cleaning_cost = string.IsNullOrEmpty(dr["Cleaning_Cost"].ToString()) ? "0.00" : dr["Cleaning_Cost"].ToString().Trim();
                            call.all_cost = string.IsNullOrEmpty(dr["All_cost"].ToString()) ? "0.00" : dr["All_cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success",
                    cellular_cost = call.cellular_cost,
                    repair_cost = call.repair_cost,
                    cleaning_cost = call.cleaning_cost,
                    all_cost = call.all_cost};
                }
                catch (Exception ex)
                {
                    log.Error("CELLULAR_item_costing: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection WATCH_item_costing(string barcode)//Edit Item
        {
            log.Info("Logging starts here: WATCH_item_costing");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Watch_Cost, Repair_Cost, Cleaning_Cost, ALL_Cost FROM ASYS_REMOutsource_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.watch_cost = string.IsNullOrEmpty(dr["Watch_Cost"].ToString()) ? "0.00" : dr["Watch_Cost"].ToString().Trim();
                            call.repair_cost  = string.IsNullOrEmpty(dr["Repair_cost"].ToString()) ? "0.00" : dr["Repair_cost"].ToString().Trim();
                            call.cleaning_cost = string.IsNullOrEmpty(dr["Cleaning_Cost"].ToString()) ? "0.00" : dr["Cleaning_Cost"].ToString().Trim();
                            call.all_cost = string.IsNullOrEmpty(dr["All_cost"].ToString()) ? "0.00" : dr["All_cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success" ,
                        watch_cost = call.watch_cost,
                        repair_cost = call.repair_cost ,
                        cleaning_cost = call.cleaning_cost,
                        all_cost = call.all_cost 
                    };
                }
                catch (Exception ex)
                {
                    log.Error("WATCH_item_costing: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection OVERAPPRAISED_REM_COSTING(string barcode)//Edit Item
        {
            log.Info("Logging starts here: OVERAPPRAISED_REM_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT ALL_Weight, ALL_Karat,ALL_Cost FROM ASYS_REM_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {

                            call.ALL_Weight  = string.IsNullOrEmpty(dr["ALL_Weight"].ToString()) ? "0.00" : dr["ALL_Weight"].ToString().Trim();
                            call.ALL_Karat  = string.IsNullOrEmpty(dr["ALL_Karat"].ToString()) ? "0.00" : dr["ALL_Karat"].ToString().Trim();
                            call.all_cost  = string.IsNullOrEmpty(dr["ALL_Cost"].ToString()) ? "0.00" : dr["ALL_Cost"].ToString().Trim();
                            
                        }
                    }
                    return new dbConnection { respons = "1", result = "success" ,
                        ALL_Weight = call.ALL_Weight,
                        ALL_Karat = call.ALL_Karat, 
                        all_cost = call.all_cost 
                    };
                }
                catch (Exception ex)
                {
                    log.Error("OVERAPPRAISED_REM_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection GOODSTOCK_REM_COSTING(string barcode)//Edit Item
        {

            log.Info("Logging starts here: GOODSTOCK_REM_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Gold_Cost, Mount_Cost, YG_Cost, WG_Cost, ALL_Cost FROM ASYS_REM_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.gold_cost = string.IsNullOrEmpty(dr["Gold_Cost"].ToString()) ? "0.00" : dr["Gold_Cost"].ToString().Trim();
                            call.mount_cost  = string.IsNullOrEmpty(dr["Mount_Cost"].ToString()) ? "0.00" : dr["Mount_Cost"].ToString().Trim();
                            call.YG_cost  = string.IsNullOrEmpty(dr["YG_Cost"].ToString()) ? "0.00" : dr["YG_Cost"].ToString().Trim();
                            call.WG_cost = string.IsNullOrEmpty(dr["WG_Cost"].ToString()) ? "0.00" : dr["WG_Cost"].ToString().Trim();
                            call.all_cost = string.IsNullOrEmpty(dr["ALL_Cost"].ToString()) ? "0.00" : dr["ALL_Cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success" ,
                    gold_cost = call.gold_cost,
                    mount_cost = call.mount_cost,
                    YG_cost = call.YG_cost,
                    all_cost = call.all_cost,
                    WG_cost = call.WG_cost 
                    };
                }
                catch (Exception ex)
                {
                    log.Error("GOODSTOCK_REM_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection TAKENBACK_REM_COSTING(string barcode)//Edit Item
        {
            log.Info("Logging starts here: TAKENBACK_REM_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT  ALL_Cost FROM ASYS_REM_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.all_cost = string.IsNullOrEmpty(dr["All_cost"].ToString()) ? "0.00" : dr["All_cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success",all_cost = call.all_cost };
                }
                catch (Exception ex)
                {
                    log.Error("TAKENBACK_REM_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection CELLULAR_REM_COSTING(string barcode)//Edit Item
        {
            log.Info("Logging starts here: CELLULAR_REM_COSTING");
            dbConnection call = new dbConnection();
            
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Cellular_Cost, Repair_Cost, Cleaning_Cost,ALL_Cost FROM ASYS_REM_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            //call.CostA = string.IsNullOrEmpty(dr["CostA"].ToString()) ? 0 : Convert.ToDouble(dr["CostA"]);
                            //call.CostB = string.IsNullOrEmpty(dr["CostB"].ToString()) ? 0 : Convert.ToDouble(dr["CostB"]);
                            //call.CostC = string.IsNullOrEmpty(dr["CostC"].ToString()) ? 0 : Convert.ToDouble(dr["CostC"]);
                            //call.CostD = string.IsNullOrEmpty(dr["CostD"].ToString()) ? 0 : Convert.ToDouble(dr["CostD"]);

                            //call.watch_cost = string.IsNullOrEmpty(dr["Watch_Cost"].ToString()) ? "0.00" : dr["Watch_Cost"].ToString().Trim();
                            //call.repair_cost = string.IsNullOrEmpty(dr["Repair_Cost"].ToString()) ? "0.00" : dr["Repair_Cost"].ToString().Trim();
                            //call.repair_cost = string.IsNullOrEmpty(dr["Cleaning_Cost"].ToString()) ? "0.00" : dr["Cleaning_Cost"].ToString().Trim();
                            //call.repair_cost = string.IsNullOrEmpty(dr["ALL_Cost"].ToString()) ? "0.00" : dr["ALL_Cost"].ToString().Trim();

                            call.cellular_cost = string.IsNullOrEmpty(dr["Cellular_cost"].ToString()) ? "0.00" : dr["Cellular_cost"].ToString().Trim();
                            call.repair_cost = string.IsNullOrEmpty(dr["Repair_cost"].ToString()) ? "0.00" : dr["Repair_cost"].ToString().Trim();
                            call.cleaning_cost = string.IsNullOrEmpty(dr["Cleaning_Cost"].ToString()) ? "0.00" : dr["Cleaning_Cost"].ToString().Trim();
                            call.all_cost  = string.IsNullOrEmpty(dr["All_cost"].ToString()) ? "0.00" : dr["All_cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success" ,
                                              cellular_cost = call.cellular_cost,
                                              repair_cost = call.repair_cost ,
                                              cleaning_cost = call.cleaning_cost,
                                              all_cost = call.all_cost 
                    };
                }
                catch (Exception ex)
                {
                    log.Error("CELLULAR_REM_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection WATCH_REM_COSTING(string barcode)//Edit Item
        {
            log.Info("Logging starts here: WATCH_REM_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Watch_Cost, Repair_Cost, Cleaning_Cost, ALL_Cost FROM ASYS_REM_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.watch_cost = string.IsNullOrEmpty(dr["Watch_Cost"].ToString()) ? "0.00" : dr["Watch_Cost"].ToString().Trim();
                            call.repair_cost = string.IsNullOrEmpty(dr["Repair_cost"].ToString()) ? "0.00" : dr["Repair_cost"].ToString().Trim();
                            call.cleaning_cost = string.IsNullOrEmpty(dr["Cleaning_Cost"].ToString()) ? "0.00" : dr["Cleaning_Cost"].ToString().Trim();
                            call.all_cost = string.IsNullOrEmpty(dr["All_cost"].ToString()) ? "0.00" : dr["All_cost"].ToString().Trim();
                        }

                    }
                    return new dbConnection { respons = "1", result = "success" , watch_cost= call.watch_cost,repair_cost= call.repair_cost,cleaning_cost= call.cleaning_cost,all_cost= call.all_cost};
                }
                catch (Exception ex)
                {
                    log.Error("WATCH_REM_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection OVERAPPRAISED_OUTSOURCE_COSTING(string barcode)//Edit Item
        {
            log.Info("Logging starts here: OVERAPPRAISED_OUTSOURCE_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT ALL_Weight, ALL_Karat,ALL_Cost FROM ASYS_REMOUTSOURCE_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.cellular_cost = string.IsNullOrEmpty(dr["ALL_Weight"].ToString()) ? "0.00" : dr["ALL_Weight"].ToString().Trim();
                            call.cellular_cost = string.IsNullOrEmpty(dr["ALL_Karat"].ToString()) ? "0.00" : dr["ALL_Karat"].ToString().Trim();
                            
                            call.cellular_cost = string.IsNullOrEmpty(dr["All_cost"].ToString()) ? "0.00" : dr["All_cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success" };
                }
                catch (Exception ex)
                {
                    log.Error("OVERAPPRAISED_OUTSOURCE_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection GOODSTOCK_OUTSOURCE_COSTING(string barcode)//Edit Item
        {
            log.Info("Logging starts here: GOODSTOCK_OUTSOURCE_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Gold_Cost, Mount_Cost, YG_Cost, WG_Cost, ALL_Cost FROM ASYS_REMOUTSOURCE_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.cellular_cost = string.IsNullOrEmpty(dr["Gold_Cost"].ToString()) ? "0.00" : dr["Gold_Cost"].ToString().Trim();
                            call.cellular_cost = string.IsNullOrEmpty(dr["Mount_Cost"].ToString()) ? "0.00" : dr["Mount_Cost"].ToString().Trim();
                            call.cellular_cost = string.IsNullOrEmpty(dr["YG_Cost"].ToString()) ? "0.00" : dr["YG_Cost"].ToString().Trim();
                            call.cellular_cost = string.IsNullOrEmpty(dr["WG_Cost"].ToString()) ? "0.00" : dr["WG_Cost"].ToString().Trim();
                            call.cellular_cost = string.IsNullOrEmpty(dr["All_Cost"].ToString()) ? "0.00" : dr["All_Cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success" };
                }
                catch (Exception ex)
                {
                    log.Error("GOODSTOCK_OUTSOURCE_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection TAKENBACK_OUTSOURCE_COSTING(string barcode)//Edit Item
        {
            log.Info("Logging starts here: TAKENBACK_OUTSOURCE_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT  ALL_Cost FROM ASYS_REMOUTSOURCE_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.cellular_cost = string.IsNullOrEmpty(dr["ALL_COST"].ToString()) ? "0.00" : dr["ALL_COST"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success" };
                }
                catch (Exception ex)
                {
                    log.Error("TAKENBACK_OUTSOURCE_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection CELLULAR_OUTSOURCE_COSTING(string barcode)//Edit Item
        {
            log.Info("Logging starts here: CELLULAR_OUTSOURCE_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Cellular_Cost, Repair_Cost, Cleaning_Cost,ALL_Cost FROM ASYS_REMOUTSOURCE_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.cellular_cost = string.IsNullOrEmpty(dr["Cellular_cost"].ToString()) ? "0.00" : dr["Cellular_cost"].ToString().Trim();
                            call.cellular_cost = string.IsNullOrEmpty(dr["Repair_cost"].ToString()) ? "0.00" : dr["Repair_cost"].ToString().Trim();
                            call.cellular_cost = string.IsNullOrEmpty(dr["Cleaning_Cost"].ToString()) ? "0.00" : dr["Cleaning_Cost"].ToString().Trim();
                            call.cellular_cost = string.IsNullOrEmpty(dr["All_cost"].ToString()) ? "0.00" : dr["All_cost"].ToString().Trim();

                        }
                    }
                    return new dbConnection { respons = "1", result = "success" };
                }
                catch (Exception ex)
                {
                    log.Error("CELLULAR_OUTSOURCE_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection WATCH_OUTSOURCE_COSTING(string barcode)//Edit Item
        {
            log.Info("Logging starts here: WATCH_OUTSOURCE_COSTING");
            dbConnection call = new dbConnection();
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Watch_Cost, Repair_Cost, Cleaning_Cost, ALL_Cost FROM ASYS_REMOUTSOURCE_Detail WHERE refallbarcode ='" + barcode + "' AND status <> 'RECMLWB'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.Read())
                        {
                            call.cellular_cost = string.IsNullOrEmpty(dr["Watch_Cost"].ToString()) ? "0.00" : dr["Watch_Cost"].ToString().Trim();
                            call.cellular_cost = string.IsNullOrEmpty(dr["Repair_cost"].ToString()) ? "0.00" : dr["Repair_cost"].ToString().Trim();
                            call.cellular_cost = string.IsNullOrEmpty(dr["Cleaning_Cost"].ToString()) ? "0.00" : dr["Cleaning_Cost"].ToString().Trim();
                            call.cellular_cost = string.IsNullOrEmpty(dr["All_cost"].ToString()) ? "0.00" : dr["All_cost"].ToString().Trim();
                        }
                    }
                    return new dbConnection { respons = "1", result = "success" };
                }
                catch (Exception ex)
                {
                    log.Error("WATCH_OUTSOURCE_COSTING: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public dbConnection MLWBSave(string desc, string serialNum, string qty, string all_weight, string all_karat, string all_carat, string priceDesc, string priceWeight, string priceKarat, string priceCarat, string cellularcost, string watchCost, string repairCost, string cleaningCost, string goldCost, string mountCost, string ygCost, string wgCost, string allCost, string barcode, string cost, string karat, string carat, string weight)//Edit Item//save
        {
            log.Info("Logging starts here: MLWBSave");
            SqlTransaction tran;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE ASYS_MLWB_Detail SET all_desc = '" + desc + "'," +
                        "SerialNo = '" + serialNum + "', refqty = " + qty + ", " +
                        "all_weight = " + all_weight + ", all_karat = " + all_karat + ", " +
                        "all_carat = " + all_carat + ",price_desc = '" + priceDesc + "',  " +
                        "price_weight = " + priceWeight + ", price_karat = " + priceKarat + ", " +
                        "price_carat = " + priceCarat + ", cellular_cost = " + cellularcost + ", watch_cost = " + watchCost + ", " +
                        "Repair_cost = " + repairCost + ",cleaning_cost = " + cleaningCost + ",gold_cost = " + goldCost + ", mount_cost = " + mountCost + "," +
                        "yg_cost = " + ygCost + ",wg_cost = " + wgCost + ",all_cost = " + allCost + " WHERE refallbarcode ='" + barcode + "' AND status = 'RECEIVED'", con, tran))
                    {

                        cmd.ExecuteNonQuery();

                    }

                    using (SqlCommand cmd = new SqlCommand("UPDATE ASYS_BarcodeHistory SET Description = '" + desc + "'," +
                        "SerialNo = '" + serialNum + "',Weight = " + weight + ", " +
                        "Karat = " + karat + ", Carat = " + carat + "," +
                        "cost = " + cost + " WHERE refallbarcode ='" + barcode + "' AND COSTCENTER = 'MLWB' AND status = 'RECEIVED'", con, tran))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                    return new dbConnection { respons = "1", result = "Success" };
                }
                catch (Exception ex)
                {
                    log.Error("MLWBSave: " + ex.Message);
                    tran.Rollback();
                    return new dbConnection { respons = "0", result = "Error: " + ex.Message };
                }
            }




        }
        [WebMethod]
        public dbConnection getBarcodeByLot1(string reflotno)//Releasing
        {
            log.Info("Logging starts here: getBarcodeByLot1");
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM ASYS_MLWB_detail WHERE lotno = '" + reflotno + "' AND status= 'RECEIVED'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dr.HasRows)
                        {
                            dr.Read();
                            return new dbConnection { respons = "1", result = "success" };
                        }
                        else
                        {
                            return new dbConnection { respons = "0", result = "No Data Found!" };
                        }
                    }

                }
                catch (Exception ex)
                {
                    log.Error("getBarcodeByLot1: " + ex.Message);
                    return new dbConnection { respons = "0", result = "error:  " + ex.Message };
                }
        }
        [WebMethod]
        public DataSet getBarcodeByLot2(string reflotno)//Releasing
        {
            log.Info("Logging starts here: getBarcodeByLot2");
            DataSet ds = new DataSet();
            dbNull models = new dbNull();
            //string query = "SELECT ptn, refallbarcode FROM ASYS_MLWB_Detail WHERE lotno = '" + reflotno.Trim() + "' AND status = 'RECEIVED'";

            string query = "SELECT ptn,refallbarcode,branchitemdesc as itemdesc, weight,karatgrading as karat,caratsize as carat,all_weight,all_karat,all_carat,all_cost,all_desc,case when all_price IS NULL then '0'else all_price end all_price ,price_desc,price_karat,price_carat,price_weight FROM ASYS_MLWB_Detail WHERE lotno = '" + reflotno.Trim() + "' AND status = 'RECEIVED'";
            //
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                    {

                        da.Fill(ds, ("asys_MWLB_detail").Trim());
                        da.Dispose();
                    }
                    con.Close();
                    return ds;
                }
                catch (Exception ex)
                {
                    log.Error("getBarcodeByLot2: " + ex.Message);
                    con.Close();
                    ex.Message.ToString();
                    return null;

                }
            }
        }
        [WebMethod]
        public MLWB getBarcodeByLot3(string reflotno)//, string DgEntry)
        {
            log.Info("Logging starts here: getBarcodeByLot3");
            var List = new MLWB();
            List.RetrieveData2data = new RetrieveData2Models();


            dbNull models = new dbNull();

            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    //using (SqlCommand cmd = new SqlCommand("SELECT branchitemdesc as itemdesc, weight,karatgrading as karat,caratsize as carat,all_weight,all_karat,all_carat,all_cost,all_desc,all_price,price_desc,price_karat,price_carat,price_weight from asys_MLWB_detail WHERE lotno='" + reflotno + "' AND refallbarcode ='" + DgEntry + "' AND status = 'RECEIVED'", con))
                    using (SqlCommand cmd = new SqlCommand("SELECT branchitemdesc as itemdesc, weight,karatgrading as karat,caratsize as carat,all_weight,all_karat,all_carat,all_cost,all_desc,all_price,price_desc,price_karat,price_carat,price_weight from asys_MLWB_detail WHERE lotno='" + reflotno + "' AND status = 'RECEIVED'", con))
                    {
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (dr.Read())
                        {
                            List.RetrieveData2data._price = models.isNull(dr["all_price"]).Trim();
                            List.RetrieveData2data._alldesc = models.isNull(dr["all_desc"]).Trim();
                            List.RetrieveData2data._weight = models.isNull(dr["all_weight"]).Trim();
                            List.RetrieveData2data._karat = models.isNull(dr["all_karat"]);
                            List.RetrieveData2data._carat = Convert.ToInt16(dr["all_carat"]).ToString();
                            List.RetrieveData2data.price_desc = models.isNull(dr["price_desc"]).Trim();
                            List.RetrieveData2data.price_weight = models.isNull(dr["price_weight"]).Trim();
                            List.RetrieveData2data.price_karat = models.isNull(dr["price_karat"]).Trim();
                            List.RetrieveData2data.price_carat = models.isNull(dr["price_carat"]).Trim();

                        }

                        dr.Close();
                        con.Close();

                    }

                    //while (read.Read())
                    //{
                    //    List.MLWBReportData.fullname.Add(read["fullname"].ToString().Trim());
                    //}
                    return new MLWB { respCode = "1", respMsg = "Success!", RetrieveData2data = List.RetrieveData2data };
                }
                catch (Exception ex)
                {
                    log.Error("getBarcodeByLot3: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error: " + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB cb_SelectedIndexChanged(string fullname)
        {
            log.Info("Logging starts here: cb_SelectedIndexChanged");
            MLWBReport inventory = new MLWBReport();
            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("Select rtrim(fullname) as fullname3 from dbo.vw_humresall where fullname='" + fullname + "'", con))
                        {
                            read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                            if (read.Read())
                            {
                                inventory._fullname = read["fullname"].ToString().Trim();
                                return new MLWB { respCode = "1", respMsg = "Success", MLWBReportData = inventory };
                            }
                            else
                            {
                                return new MLWB { respCode = "0", respMsg = "No data " };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error("cb_SelectedIndexChanged: " + ex.Message);
                        return new MLWB { respCode = "0", respMsg = "service Error" + ex.Message };
                    }
                }
            }

        }
        [WebMethod]
        public MLWB releaseData(string templot)//Releasing 
        {

            log.Info("Logging starts here: releaseData");
            SqlTransaction tran;


            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE ASYS_Lotno_Gen SET lotno ='" + templot + "' WHERE BusinessCenter ='MLWB'", con, tran))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                    //con.Close();

                }
                catch (Exception ex)
                {
                    log.Error("releaseData: " + ex.Message);
                    tran.Rollback();
                    return new MLWB { respCode = "0", respMsg = "service error: " + ex.Message };
                }
            }
            return new MLWB { respCode = "1", respMsg = "Success" };
        }
        [WebMethod]
        public MLWB releaseData2(string templot, string txtuser, string DgEntry, string userlog)//Releasing 
        {
            log.Info("Logging starts here: releaseData2");
            SqlTransaction tran;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO ASYS_BarcodeHistory (lotno, refallbarcode,allbarcode, itemcode, [description], karat, carat, SerialNo, weight, currency, price, cost, empname, custodian, trandate, costcenter, consignto, status ) SELECT '" + templot + "', refallbarcode as refallbarcode, refallbarcode as allbarcode, refitemcode, all_desc, price_karat, price_carat, SerialNo, price_weight, currency, all_price, all_cost,'" + txtuser + "' as  empname, receiver as custodian, getdate() as trandate, 'MLWB', 'NULL', 'RELEASED' FROM ASYS_MLWB_Detail WHERE refallbarcode='" + DgEntry + "' AND status = 'RECEIVED'", con, tran))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand("UPDATE ASYS_MLWB_detail SET reflotno ='" + templot + "', releaser = '" + userlog + "', releasedate = getdate(), status = 'RELEASED' WHERE refallbarcode='" + DgEntry + "' AND status = 'RECEIVED'", con, tran))
                    {
                        cmd.ExecuteNonQuery();
                    }


                    tran.Commit();
                    //con.Close();

                }
                catch (Exception ex)
                {
                    log.Error("releaseData2: " + ex.Message);
                    tran.Rollback();
                    return new MLWB { respCode = "0", respMsg = "service error: " + ex.Message };
                }
            }
            return new MLWB { respCode = "1", respMsg = "Success" };
        }
        [WebMethod]
        public MLWB frmMainLoad()//barcode details
        {
            log.Info("Logging starts here: frmMainLoad");
            details inventory = new details();
            inventory.action_id = new List<string>();
            inventory.action_type = new List<string>();
            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("select action_id, action_type from tbl_action where action_id in(1,3,8,11,10,2,4,7,9,5) order by action_type", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (read.Read())
                        {
                            inventory.action_id.Add(read["action_id"].ToString());
                            inventory.action_type.Add(read["action_type"].ToString());


                        }

                    }
                    return new MLWB { respCode = "1", respMsg = "Success", MLWBInventory = inventory };

                }
                catch (Exception ex)
                {
                    log.Error("frmMainLoad: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB frmMain_Load2()//barcode details
        {

            log.Info("Logging starts here: frmMain_Load2");
            details inventory = new details();
            inventory.description = new List<string>();
            inventory.code = new List<string>();
            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("select * from tbl_sortclass order by description", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (read.Read())
                        {
                            inventory.description.Add(read["description"].ToString());
                            inventory.code.Add(read["code"].ToString());

                        }


                    }

                    return new MLWB { respCode = "1", respMsg = "Success", MLWBInventory = inventory };
                }
                catch (Exception ex)
                {
                    log.Error("frmMain_Load2: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB RetrieveInfo2(string MainCost, string allbarcode)//barcode details
        {
            log.Info("Logging starts here: RetrieveInfo2");
            details inventory = new details();
            SqlDataReader read;

            string SQuery = string.Empty;
            string query = string.Empty;

            SQuery = "SELECT RefLotno, RefallBarcode, PTN, ItemID, PTNBarcode, BranchCode, BranchName, Loanvalue, RefItemcode, Itemcode, BranchItemDesc, RefQty, Qty, " +
            "KaratGrading, CaratSize, Weight, ActionClass, SortCode, ALL_desc,serialno, ALL_karat, ALL_carat, ALL_Cost, ALL_weight, PhotoName, Price_desc, Price_karat, " +
            "Price_weight, Price_carat, ALL_price, Cellular_cost, Watch_cost, Repair_cost, Cleaning_cost, Gold_cost, Mount_cost, YG_cost, WG_cost, MaturityDate, " +
            "ExpiryDate, LoanDate, Status";

            switch (MainCost)
            {
                case "MLWB":
                    query = SQuery + " FROM dbo.ASYS_MLWB_detail where refallbarcode = '" + allbarcode + "' and photoname is not null and status not in ('RELEASED', 'RECPRICING')";
                    break;

                case "PRICING":
                    query = SQuery + " FROM dbo.ASYS_PRICING_detail where refallbarcode = '" + allbarcode + "' and status not in ('RELEASED','RECDISTRI')";
                    break;
                case "DISTRI":
                    query = SQuery + " FROM dbo.ASYS_DISTRI_detail where refallbarcode = '" + allbarcode + "' and status <> 'RETURNED'";
                    break;

            }

            using (SqlConnection con = new SqlConnection(constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.Read())
                        {

                            inventory.itemid1 = Convert.ToInt32(read["ItemID"]);
                            inventory.photoname = read["photoname"].ToString().Trim();
                            inventory.status = read["status"].ToString().Trim();
                            inventory.branchcode = read["Branchcode"].ToString().Trim();
                            inventory.branchname = read["branchname"].ToString().Trim();
                            inventory.LoanValue = read["LoanValue"].ToString().Trim();
                            inventory.actionclass = read["actionclass"].ToString().Trim();
                            inventory.sortcode = read["sortcode"].ToString().Trim();
                            inventory.itemid = read["itemid"].ToString().Trim();
                            inventory.Itemcode = read["Itemcode"].ToString().Trim();
                            inventory.branchitemdesc = read["branchitemdesc"].ToString().Trim();
                            inventory.qty = read["qty"].ToString().Trim();
                            inventory.Karatgrading = read["Karatgrading"].ToString().Trim();
                            inventory.Caratsize = read["Caratsize"].ToString().Trim();
                            inventory.Weight = read["Weight"].ToString().Trim();
                            //inventory.all_cost =Convert.ToDouble(read["all_cost"]);
                            inventory.all_cost = read["all_cost"].ToString().Trim();
                            inventory.price_desc = read["price_desc"].ToString().Trim();
                            inventory.serialno = read["serialno"].ToString().Trim();
                            inventory.refqty = read["refqty"].ToString().Trim();
                            inventory.price_karat = read["price_karat"].ToString().Trim();
                            inventory.price_carat = read["price_carat"].ToString().Trim();
                            inventory.price_weight = read["price_weight"].ToString().Trim();
                            inventory.ALL_price = read["ALL_price"].ToString().Trim();
                            //inventory.ALL_price = Convert.ToDouble(read["ALL_price"].ToString().Trim());
                            if (inventory.all_cost == "NULL" || inventory.all_cost == "0" || inventory.all_cost == "")
                            {
                                inventory.all_cost = "0.00";
                            }

                            if (inventory.ALL_price == "NULL" || inventory.ALL_price == "0" || inventory.ALL_price == "")
                            {
                                inventory.ALL_price = "0.00";

                            }

                            if (inventory.LoanValue == "NULL" || inventory.LoanValue == "0" || inventory.LoanValue == "")
                            {
                                inventory.LoanValue = "0.00";

                            }

                        }
                        return new MLWB { respCode = "1", respMsg = "Success", MLWBInventory = inventory };
                    }

                }
                catch (Exception ex)
                {
                    log.Error("RetrieveInfo2: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "service Error" + ex.Message };
                }
            }

        }
        [WebMethod]
        public MLWB MLWBDisplayBarcodeDetails(string type)//barcode details
        {
            log.Info("Logging starts here: MLWBDisplayBarcodeDetails");
            details inventory = new details();

            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("select CostA, CostB, CostC, CostD from tbl_action where action_type ='" + type + "'", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.Read())
                        {
                            //inventory.CostA = Convert.ToDouble(read["CostA"]);
                            //inventory.CostB = Convert.ToDouble(read["CostB"]);
                            //inventory.CostC = Convert.ToDouble(read["CostC"]);
                            //inventory.CostD = Convert.ToDouble(read["CostD"]);

                            inventory.CostA = read["CostA"].ToString().Trim();
                            inventory.CostB = read["CostB"].ToString().Trim();
                            inventory.CostC = read["CostC"].ToString().Trim();
                            inventory.CostD = read["CostD"].ToString().Trim();


                        }
                        return new MLWB { respCode = "1", respMsg = "Success", MLWBInventory = inventory };
                    }
                }
                catch (Exception ex)
                {
                    log.Error("MLWBDisplayBarcodeDetails: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "service Error" + ex.Message };
                }
            }
            
        }
        [WebMethod]
        public MLWB goodstockBarcodeDetails(string barcode)//barcode details
        {
            log.Info("Logging starts here: goodstockBarcodeDetails");
            SqlDataReader read;
            details inventory = new details();
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("select Gold_Cost, Mount_Cost, YG_Cost, WG_Cost, ALL_Cost from ASYS_MLWB_DEtail where refAllbarcode ='" + barcode + "' ", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.Read())
                        {
                            inventory.Gold_Cost = read["Gold_Cost"].ToString();
                            inventory.Mount_Cost = read["Mount_Cost"].ToString();
                            inventory.YG_Cost = read["YG_Cost"].ToString();
                            inventory.WG_Cost = read["WG_Cost"].ToString();
                            inventory.all_cost = read["ALL_Cost"].ToString();

                            //inventory.Gold_Cost = Convert.ToDouble(read["Gold_Cost"]);
                            //inventory.Mount_Cost=Convert.ToDouble( read["Mount_Cost"]);
                            //inventory.YG_Cost=Convert.ToDouble(read["YG_Cost"]);
                            //inventory.WG_Cost = Convert.ToDouble(read["WG_Cost"]);
                            //inventory.all_cost = Convert.ToDouble(read["ALL_Cost"]);
                        }
                        return new MLWB { respCode = "1", respMsg = "Success", MLWBInventory = inventory };
                    }
                }
                catch (Exception ex)
                {
                    log.Error("goodstockBarcodeDetails: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "service Error" + ex.Message };
                }
            }

           
        }
        [WebMethod]
        public MLWB cellularBarcodeDetails(string barcode)//barcode details
        {
            log.Info("Logging starts here: cellularBarcodeDetails");
            SqlDataReader read;
            details inventory = new details();
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("select Cellular_Cost, Repair_Cost, Cleaning_Cost,ALL_Cost from ASYS_MLWB_DEtail where refAllbarcode ='" + barcode + "' ", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.Read())
                        {

                            //inventory.Cellular_Cost = Convert.ToDouble(read["Cellular_Cost"]);
                            //inventory.Repair_Cost = Convert.ToDouble(read["Repair_Cost"]);
                            //inventory.Cleaning_Cost = Convert.ToDouble(read["Cleaning_Cost"]);
                            //inventory.all_cost = Convert.ToDouble(read["ALL_Cost"]);
                            inventory.Cellular_Cost = read["Cellular_Cost"].ToString().Trim();
                            inventory.Repair_Cost = read["Repair_Cost"].ToString().Trim();
                            inventory.Cleaning_Cost = read["Cleaning_Cost"].ToString().Trim();
                            inventory.all_cost = read["ALL_Cost"].ToString().Trim();

                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("cellularBarcodeDetails: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "service Error" + ex.Message };
                }
            }

            return new MLWB { respCode = "1", respMsg = "Success", MLWBInventory = inventory };
        }
        [WebMethod]
        public MLWB watchBarcodeDetails(string barcode)//barcode details
        {
            log.Info("Logging starts here: watchBarcodeDetails");
            SqlDataReader read;
            details inventory = new details();
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("select Watch_Cost, Repair_Cost, Cleaning_Cost, ALL_Cost from ASYS_MLWB_DEtail where refAllbarcode ='" + barcode + "'", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.Read())
                        {

                            inventory.Watch_Cost = read["Watch_Cost"].ToString().Trim();
                            inventory.Repair_Cost = read["Repair_Cost"].ToString().Trim();
                            inventory.Cleaning_Cost = read["Cleaning_Cost"].ToString().Trim();
                            inventory.all_cost = read["ALL_Cost"].ToString().Trim();


                        }
                    }
                }
                catch (Exception ex)
                {

                    log.Error("watchBarcodeDetails: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "service Error" + ex.Message };
                }
            }

            return new MLWB { respCode = "1", respMsg = "Success", MLWBInventory = inventory };
        }
        [WebMethod]
        public MLWB GetResourceID(string UserPassW)
        {

            log.Info("Logging starts here: GetResourceID");
            details inventory = new details();
            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                {
                    try
                    {
                        
                        using (SqlCommand cmd = new SqlCommand("Select usr_id from dbo.vw_humresall where res_id =" + UserPassW + " and blocked = 1", con))
                        {
                            read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                            if (read.Read())
                            {
                                inventory.userid = read["usr_id"].ToString().Trim();
                                return new MLWB { respCode = "1", respMsg = "Success", MLWBInventory = inventory };
                            }
                            else
                            {
                                return new MLWB { respCode = "0", respMsg = "Error: Username and Password did not match " };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error("GetResourceID: " + ex.Message);
                        return new MLWB { respCode = "0", respMsg = "service Error" + ex.Message };
                    }
                }
            }

        }
        [WebMethod]
        public DataSet rembarcode(string costCenter, string barcode, string ptn)
        {
            log.Info("Logging starts here: rembarcode");
            DataSet ds = new DataSet();


            string SQuery = string.Empty;

            switch (costCenter.ToUpper())
            {
                case "REM":
                    {
                        if (barcode == "")
                            SQuery = "SELECT '',*,'' FROM vwASYS_REM_SelfInventory_Form WHERE ALLBARCODE = '" + barcode + "' and ptn = '" + ptn + "'";
                        else                            
                            SQuery = "SELECT '',*,'' FROM vwASYS_REM_SelfInventory_Form WHERE ALLBARCODE = '" + barcode + "'";
                    }
                    break;

                case "MLWB":
                    SQuery = "exec ASYS_spMLWB_SelfInventory '" + barcode + "'";
                    break;

                case "PRICING":
                    SQuery = "exec ASYS_spPRICING_SelfInventory '" + barcode + "'";
                    break;

                case "DISTRI":
                    SQuery = "exec ASYS_spDISTRI_SelfInventory '" + barcode + "'";
                    break;
            }
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(SQuery, con))
                    {
                        da.SelectCommand.CommandTimeout = 300000;
                        da.Fill(ds);
                        da.Dispose();
                        con.Close();

                    }
                    return ds;

                }
                catch (Exception ex)
                {
                    log.Error("rembarcode: " + ex.Message);
                    throw new Exception(ex.Message);
                }
            }

        }
        [WebMethod]
        public DataSet txtPTN_KeyPress(string ptn)
        {
            log.Info("Logging starts here: txtPTN_KeyPress");
            DataSet ds = new DataSet();

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlDataAdapter da = new SqlDataAdapter("SELECT *,'' FROM vwASYS_REM_SelfInventory_Form WHERE ptn = '" + ptn + "'", con))
                    {
                        da.Fill(ds);
                        da.Dispose();
                        con.Close();
                    }
                    return ds;
                }
                catch (Exception ex)
                {
                    log.Error("txtPTN_KeyPress: " + ex.Message);
                    throw new Exception(ex.Message);
                }
            }

        }
        [WebMethod]
        public DataSet txtTransNo_KeyPress(string ptn)
        {

            log.Info("Logging starts here: txtTransNo_KeyPress");
            DataSet ds = new DataSet();


            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlDataAdapter da = new SqlDataAdapter("exec ASYS_spTRADEIN_SelfInventory '" + ptn + "'", con))
                    {
                        da.Fill(ds);
                        da.Dispose();
                        con.Close();
                    }
                    return ds;
                }
                catch (Exception ex)
                {
                    log.Error("txtTransNo_KeyPress: " + ex.Message);
                    throw new Exception(ex.Message);
                }
            }

        }
        [WebMethod]
        public MLWB process(string costCenter, string[][] GetFieldValues)
        {
            log.Info("Logging starts here: process ");
            SqlTransaction tranCon;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                tranCon = con.BeginTransaction();

                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("EXEC ASYS_DeleteSelfInventory '" + costCenter + "'", con, tranCon))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        for (int i = 0; i <= GetFieldValues.Count() - 1; i++)
                        {
                            using (SqlCommand cmd = new SqlCommand("Insert into ASYS_SelfInventory(ASITransNo,ASIptn,ASIallBarcode,ASIDate,ASIDescription,ASIQty,ASIWeight,ASIKarat,ASICarat,ASILoanValue,ASISellingPrice,ASIItemCounter,ASICostCenter,ASIResource,ASISerialNo) Values (" + GetFieldValues[i][0] + ")", con, tranCon))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tranCon.Commit();

                    }
                    catch (Exception ex)
                    {
                        log.Error("process: " + ex.Message);
                        tranCon.Rollback();
                        return new MLWB { respCode = "0", respMsg = "service Error" + ex.Message };
                    }

                }
                return new MLWB { respCode = "1", respMsg = "Success" };
            }

        }
        [WebMethod]
        public MLWB frmMLWBReport_user(string humres2)
        {
            log.Info("Logging starts here: frmMLWBReport_user");
            var List = new MLWB();
            List.MLWBReportData = new MLWBReport();
            List.MLWBReportData.fullname = new List<string>();
            //List.data1 = new getLotNumber();
            //List.data1.lotNumber = new List<string>();
            SqlDataReader read;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("select distinct upper(rtrim(fullname)) as fullname,blocked,job_title from vw_humresall where job_title in ('ALLBOSMAN', 'mlwb') and blocked =1  and fullname not in ('GENELOUIE NEMENO','PORTIA BRIONES','SHERYL LOVE ONG','ROWENA VILLENA') order by fullname", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (read.Read())
                        {
                            List.MLWBReportData.fullname.Add(read["fullname"].ToString().Trim());
                        }

                    }
                    return new MLWB { respCode = "1", respMsg = "success", MLWBReportData = List.MLWBReportData };
                }
                catch (Exception ex)
                {
                    log.Error("frmMLWBReport_user: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB getConsignee()//inventory
        {
            log.Info("Logging starts here: getConsignee");
            var list = new MLWB();
            list.MLWBReportData = new MLWBReport();
            list.MLWBReportData.Consignee = new List<string>();
            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM ASYS_vw_GetConsignee", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                list.MLWBReportData.Consignee.Add(read["Consignee"].ToString().Trim());
                            }

                        }
                    }
                    return new MLWB { respCode = "1", respMsg = "Success", MLWBReportData = list.MLWBReportData };
                }
                catch (Exception ex)
                {
                    log.Error("getConsignee: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB call_Custodian(string costcenter)//inventory
        {
            log.Info("Logging starts here: call_Custodian");
            var list = new MLWB();
            list.MLWBReportData = new MLWBReport();
            list.MLWBReportData.fullname = new List<string>();

            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT UPPER(fullname) AS FULLNAME FROM vw_humresvismin WHERE job_title='" + costcenter + "' ORDER BY FULLNAME", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                list.MLWBReportData.fullname.Add(read["fullname"].ToString().Trim());
                            }
                        }

                    }
                    return new MLWB { respCode = "1", respMsg = "success", MLWBReportData = list.MLWBReportData };
                }
                catch (Exception ex)
                {
                    log.Error("call_Custodian: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error: " + ex.Message };

                }
            }
        }
        [WebMethod]
        public MLWB getCostCenter()//inventory
        {
            log.Info("Logging starts here: getCostCenter");
            var list = new MLWB();
            list.MLWBInventory = new details();
            list.MLWBInventory._CostDept = new List<string>();


            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("Select * from tbl_CostCenter where CostDept not in ('SHOWROOM','VIP')", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                list.MLWBInventory._CostDept.Add(read["CostDept"].ToString().Trim());

                            }
                        }

                    }
                    return new MLWB { respCode = "1", respMsg = "Success", MLWBInventory = list.MLWBInventory };
                }
                catch (Exception ex)
                {
                    log.Error("getCostCenter: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "service error: " + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB TextBox1_KeyPress(string costcenter, string result)//inventory
        {
            log.Info("Logging starts here: TextBox1_KeyPress");
            SqlDataReader read;
            string squery = string.Empty;

            if (costcenter == "MLWB")
            {
                squery = "Select substring(allbarcode, 4, 5) as refitemcode from asys_mlwb_detail where substring(allbarcode, 4, 5) ='" + result + "'";
            }
            else if (costcenter == "PRICING")
            {
                squery = "Select substring(allbarcode, 4, 5) as refitemcode from asys_pricing_detail where substring(allbarcode, 4, 5) ='" + result + "'";

            }
            else if (costcenter == "DISTRIBUTION")
            {
                squery = "Select substring(allbarcode, 4, 5) as refitemcode from asys_distri_detail where substring(allbarcode, 4, 5) ='" + result + "'";
            }
            else if (costcenter == "REM")
            {
                squery = "Select substring(allbarcode, 4, 5) as refitemcode from asys_rem_detail where substring(allbarcode, 4, 5) ='" + result + "'";
            }
            else
            {
                squery = "Select itemsource from vw_REMItemsource where itemsource = '" + result + "'";
            }

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(squery, con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.Read() == false)
                        {
                            return new MLWB { respCode = "0", respMsg = "Itemsource doesn't exist." };
                        }
                    }
                    return new MLWB { respCode = "1", respMsg = "Success!" };
                }
                catch (Exception ex)
                {
                    log.Error("TextBox1_KeyPress: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB RemInv()//inventory
        {
            log.Info("Logging starts here: RemInv");
            var list = new MLWB();
            list.MLWBInventory = new details();
            list.MLWBInventory.action_type = new List<string>();

            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT action_type FROM tbl_action WHERE action_type not in ('Appliance','AuctionBuyer','TakenBack','Release') ORDER BY action_type", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                list.MLWBInventory.action_type.Add(read["action_type"].ToString().Trim());
                            }
                        }
                        else
                        {
                            return new MLWB { respCode = "0", respMsg = "No record found:" };
                        }

                    }
                    return new MLWB { respCode = "1", respMsg = "Success!", MLWBInventory = list.MLWBInventory };
                }
                catch (Exception ex)
                {
                    log.Error("RemInv: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB releaseReport(string hlot, string process)
        {
            log.Info("Logging starts here: releaseReport");
            var list = new MLWB();
            list.summaryGoodStock = new List<ASYS_SummaryGoodStocksRel_rpt>();

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    if (process == "rev")
                    {
                        using (SqlCommand cmd = new SqlCommand("ASYS_SummaryGoodStocks_rpt", con))
                        {
                            cmd.Parameters.AddWithValue("@hlot", hlot);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 600;
                            SqlDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                            if (read.HasRows)
                            {
                                while (read.Read())
                                {
                                    var a = new ASYS_SummaryGoodStocksRel_rpt();
                                    a.lotno = read["lotno"].ToString().Trim();
                                    a.reflotno = read["reflotno"].ToString().Trim();
                                    a.division = read["division"].ToString().Trim();
                                    a.divisionname = read["divisionname"].ToString().Trim();
                                    a.ptn = read["ptn"].ToString().Trim();
                                    a.itemid = string.IsNullOrEmpty(read["itemid"].ToString()) ? 0 : Convert.ToInt32(read["itemid"]);
                                    a.refallbarcode = read["refallbarcode"].ToString().Trim();
                                    a.allbarcode = read["allbarcode"].ToString().Trim();
                                    a.itemcode = read["itemcode"].ToString().Trim();
                                    a.itemdescription = read["itemdescription"].ToString().Trim();
                                    a.quantity = string.IsNullOrEmpty(read["quantity"].ToString()) ? 0 : Convert.ToInt32(read["quantity"]);
                                    a.actionclass = read["actionclass"].ToString().Trim();
                                    a.loanvalue = string.IsNullOrEmpty(read["loanvalue"].ToString()) ? 0 : Convert.ToDouble(read["loanvalue"]);
                                    a.itemappraisevalue = string.IsNullOrEmpty(read["itemappraisevalue"].ToString()) ? 0 : Convert.ToDouble(read["itemappraisevalue"]);
                                    a.all_wt = string.IsNullOrEmpty(read["all_wt"].ToString()) ? 0 : Convert.ToDouble(read["all_wt"]);
                                    a.all_karat = read["all_karat"].ToString().Trim();
                                    a.all_cost = string.IsNullOrEmpty(read["all_cost"].ToString()) ? 0 : Convert.ToDouble(read["all_cost"]);
                                    a.all_carat = string.IsNullOrEmpty(read["all_carat"].ToString()) ? 0 : Convert.ToDouble(read["all_carat"]);
                                    a.SerialNo = read["SerialNo"].ToString().Trim();
                                    a.all_price = string.IsNullOrEmpty(read["all_price"].ToString()) ? 0 : Convert.ToDouble(read["all_price"]);
                                    a.employee = read["employee"].ToString().Trim();
                                    a.price_desc = read["price_desc"].ToString().Trim();
                                    a.price_karat = read["price_karat"].ToString().Trim();
                                    a.price_weight = string.IsNullOrEmpty(read["price_weight"].ToString()) ? 0 : Convert.ToDouble(read["price_weight"]);
                                    a.price_carat = string.IsNullOrEmpty(read["price_carat"].ToString()) ? 0 : Convert.ToDouble(read["price_carat"]);
                                    a.releasedate = string.IsNullOrEmpty(read["releasedate"].ToString()) ? "" : Convert.ToDateTime(read["releasedate"]).ToString("MMMM dd, yyyy").Trim();
                                    a.releaser = read["releaser"].ToString().Trim();
                                    a.receivedate = string.IsNullOrEmpty(read["receivedate"].ToString()) ? "" : Convert.ToDateTime(read["receivedate"]).ToString("MMMM dd, yyyy").Trim();
                                    a.receiver = read["receiver"].ToString().Trim();
                                    a.month = read["month"].ToString().Trim();
                                    a.year = read["year"].ToString().Trim();
                                    a.status = read["status"].ToString().Trim();
                                    list.summaryGoodStock.Add(a);
                                }
                                read.Close();
                                con.Close();
                                return new MLWB { respCode = "1", respMsg = "Success", summaryGoodStock = list.summaryGoodStock };
                            }
                            else
                            {
                                read.Close();
                                con.Close();
                                return new MLWB { respCode = "0", respMsg = "No records found!" };
                            }
                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand("ASYS_SummaryGoodStocksRel_rpt", con))
                        {
                            cmd.Parameters.AddWithValue("@hlot", hlot);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 600;
                            SqlDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                            if (read.HasRows)
                            {
                                while (read.Read())
                                {
                                    var a = new ASYS_SummaryGoodStocksRel_rpt();
                                    a.lotno = read["lotno"].ToString().Trim();
                                    a.reflotno = read["reflotno"].ToString().Trim();
                                    a.division = read["division"].ToString().Trim();
                                    a.divisionname = read["divisionname"].ToString().Trim();
                                    a.ptn = read["ptn"].ToString().Trim();
                                    a.itemid = string.IsNullOrEmpty(read["itemid"].ToString()) ? 0 : Convert.ToInt32(read["itemid"]);
                                    a.refallbarcode = read["refallbarcode"].ToString().Trim();
                                    a.allbarcode = read["allbarcode"].ToString().Trim();
                                    a.itemcode = read["itemcode"].ToString().Trim();
                                    a.all_desc = read["all_desc"].ToString().Trim();
                                    a.quantity = string.IsNullOrEmpty(read["quantity"].ToString()) ? 0 : Convert.ToInt32(read["quantity"]);
                                    a.actionclass = read["actionclass"].ToString().Trim();
                                    a.loanvalue = string.IsNullOrEmpty(read["loanvalue"].ToString()) ? 0 : Convert.ToDouble(read["loanvalue"]);
                                    a.itemappraisevalue = string.IsNullOrEmpty(read["itemappraisevalue"].ToString()) ? 0 : Convert.ToDouble(read["itemappraisevalue"]);
                                    a.all_wt = string.IsNullOrEmpty(read["all_wt"].ToString()) ? 0 : Convert.ToDouble(read["all_wt"]);
                                    a.all_karat = read["all_karat"].ToString().Trim();
                                    a.all_cost = string.IsNullOrEmpty(read["all_cost"].ToString()) ? 0 : Convert.ToDouble(read["all_cost"]);
                                    a.all_carat = string.IsNullOrEmpty(read["all_carat"].ToString()) ? 0 : Convert.ToDouble(read["all_carat"]);
                                    a.SerialNo = read["SerialNo"].ToString().Trim();
                                    a.all_price = string.IsNullOrEmpty(read["all_price"].ToString()) ? 0 : Convert.ToDouble(read["all_price"]);
                                    a.employee = read["employee"].ToString().Trim();
                                    a.price_desc = read["price_desc"].ToString().Trim();
                                    a.price_karat = read["price_karat"].ToString().Trim();
                                    a.price_weight = string.IsNullOrEmpty(read["price_weight"].ToString()) ? 0 : Convert.ToDouble(read["price_weight"]);
                                    a.price_carat = string.IsNullOrEmpty(read["price_carat"].ToString()) ? 0 : Convert.ToDouble(read["price_carat"]);
                                    a.releasedate = string.IsNullOrEmpty(read["releasedate"].ToString()) ? "" : Convert.ToDateTime(read["releasedate"]).ToString("MMMM dd, yyyy").Trim();
                                    a.releaser = read["releaser"].ToString().Trim();
                                    a.receivedate = string.IsNullOrEmpty(read["receivedate"].ToString()) ? "" : Convert.ToDateTime(read["receivedate"]).ToString("MMMM dd, yyyy").Trim();
                                    a.receiver = read["receiver"].ToString().Trim();
                                    a.month = read["month"].ToString().Trim();
                                    a.year = read["year"].ToString().Trim();
                                    a.status = read["status"].ToString().Trim();
                                    list.summaryGoodStock.Add(a);

                                }
                                read.Close();
                                con.Close();
                                return new MLWB { respCode = "1", respMsg = "Success", summaryGoodStock = list.summaryGoodStock };
                            }
                            else
                            {
                                read.Close();
                                con.Close();
                                return new MLWB { respCode = "0", respMsg = "No records found!" };
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("releaseReport: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB unreceivedReport(string COSTCENTER)
        {
            log.Info("Logging starts here: unreceivedReport");
            var list = new MLWB();
            list.unreceivedItems = new List<ASYS_GetDeptUnreceived_Items>();

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_GetDeptUnreceived_Items", con))
                    {
                        cmd.Parameters.AddWithValue("@COSTCENTER", COSTCENTER);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        SqlDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_GetDeptUnreceived_Items();
                                a.costcenter = read["costcenter"].ToString().Trim();
                                a.lotno = read["lotno"].ToString().Trim();
                                a.refallbarcode = read["refallbarcode"].ToString().Trim();
                                a.qty = string.IsNullOrEmpty(read["qty"].ToString()) ? 0 : Convert.ToInt32(read["qty"]);
                                a.serialno = read["serialno"].ToString().Trim();
                                a.description = read["description"].ToString().Trim();
                                a.karat = read["karat"].ToString().Trim();
                                a.weight = string.IsNullOrEmpty(read["weight"].ToString()) ? 0 : Convert.ToDouble(read["weight"]);
                                a.carat = string.IsNullOrEmpty(read["carat"].ToString()) ? 0 : Convert.ToDouble(read["carat"]);
                                a.all_price = string.IsNullOrEmpty(read["all_price"].ToString()) ? 0 : Convert.ToDouble(read["all_price"]);
                                list.unreceivedItems.Add(a);

                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", unreceivedItems = list.unreceivedItems };

                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }

                    }

                }
                catch (Exception ex)
                {
                    log.Error("unreceivedReport: " + ex.Message);
                    con.Close();

                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB rptSelfInventory(string costcenter)
        {

            log.Info("Logging starts here: rptSelfInventory");
            var list = new MLWB();
            list.selfInventory = new List<ASYS_SelfInventory_rpt>();
            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_SelfInventory_rpt", con))
                    {
                        cmd.Parameters.AddWithValue("@costcenter", costcenter);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_SelfInventory_rpt();
                                a.ASITransNo = read["ASITransNo"].ToString().Trim();
                                a.ASIptn = read["ASIptn"].ToString().Trim();
                                a.ASIallbarcode = read["ASIallbarcode"].ToString().Trim();
                                a.ASIDate = read["ASIDate"].ToString().Trim();
                                a.ASIDescription = read["ASIDescription"].ToString().Trim();
                                a.ASISerialNo = read["ASISerialNo"].ToString().Trim();
                                a.ASIQty = string.IsNullOrEmpty(read["ASIQty"].ToString()) ? 0 : Convert.ToInt32(read["ASIQty"]);
                                a.ASIWeight = string.IsNullOrEmpty(read["ASIWeight"].ToString()) ? 0 : Convert.ToDouble(read["ASIWeight"]);
                                a.ASIKarat = string.IsNullOrEmpty(read["ASIKarat"].ToString()) ? 0 : Convert.ToDouble(read["ASIKarat"]);
                                a.ASICarat = string.IsNullOrEmpty(read["ASICarat"].ToString()) ? 0 : Convert.ToDouble(read["ASICarat"]);
                                a.ASILoanValue = string.IsNullOrEmpty(read["ASILoanValue"].ToString()) ? 0 : Convert.ToDouble(read["ASILoanValue"]);
                                a.ASISellingPrice = string.IsNullOrEmpty(read["ASISellingPrice"].ToString()) ? 0 : Convert.ToDouble(read["ASISellingPrice"]);
                                a.ASIItemCounter = string.IsNullOrEmpty(read["ASIItemCounter"].ToString()) ? 0 : Convert.ToInt32(read["ASIItemCounter"]);
                                a.ASICostCenter = read["ASICostCenter"].ToString().Trim();
                                a.ASIResource = read["ASIResource"].ToString().Trim();
                                a.DatePrinted = string.IsNullOrEmpty(read["DatePrinted"].ToString()) ? null : Convert.ToDateTime(read["DatePrinted"]).ToString("MMMM dd, yyyy").Trim();
                                list.selfInventory.Add(a);

                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", selfInventory = list.selfInventory };
                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("rptSelfInventory: " + ex.Message);
                    con.Close();

                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }

        }
        [WebMethod]
        public MLWB rptSelfInventoryMissing(string costcenter)
        {
            log.Info("Logging starts here: rptSelfInventoryMissing");
            var list = new MLWB();
            list.selfInventoryMissing = new List<procASYS_MissingItems>();
            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("procASYS_MissingItems", con))
                    {
                        cmd.Parameters.AddWithValue("@costcenter", costcenter);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new procASYS_MissingItems();
                                a.ASITransNo = read["ASITransNo"].ToString() == "" ? "0" : read["ASITransNo"].ToString();
                                a.PTN = read["PTN"].ToString() == "" ? "0" : read["PTN"].ToString();
                                a.ALLBARCODE = read["ALLBARCODE"].ToString().Trim();
                                a.ALL_DESC = read["ALL_DESC"].ToString().Trim();
                                a.QTY = string.IsNullOrEmpty(read["QTY"].ToString()) ? 0 : Convert.ToInt32(read["QTY"]);
                                a.ALL_WEIGHT = string.IsNullOrEmpty(read["ALL_WEIGHT"].ToString()) ? 0 : Convert.ToDouble(read["ALL_WEIGHT"]);
                                a.ALL_KARAT = read["ALL_KARAT"].ToString().Trim();
                                a.ALL_CARAT = string.IsNullOrEmpty(read["ALL_CARAT"].ToString()) ? 0 : Convert.ToDouble(read["ALL_CARAT"]);
                                a.SERIALNO = read["SERIALNO"].ToString() == "" ? "0" : read["SERIALNO"].ToString();
                                a.LOANVALUE = string.IsNullOrEmpty(read["LOANVALUE"].ToString()) ? 0 : Convert.ToDouble(read["LOANVALUE"]);
                                a.ALL_PRICE = string.IsNullOrEmpty(read["ALL_PRICE"].ToString()) ? 0 : Convert.ToDouble(read["ALL_PRICE"]);
                                a.RESOURCE = read["RESOURCE"].ToString().Trim();
                                a.UniqueTrigger = read["UniqueTrigger"].ToString().Trim();
                                a.DatePrinted = read["DatePrinted"].ToString().Trim();
                                list.selfInventoryMissing.Add(a);


                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", selfInventoryMissing = list.selfInventoryMissing };
                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("rptSelfInventoryMissing: " + ex.Message);
                    con.Close();

                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }

        }
        [WebMethod]
        public MLWB ASYS_REMOutsource_Inventory(string SupplierCode, string ItemCode, string rptFlag)
        {
            log.Info("Logging starts here: ASYS_REMOutsource_Inventory");
            var list = new MLWB();
            list.remJewelryOutsource = new List<ASYS_REMJewelryOutsource>();
            SqlDataReader read;
            string stored;
            string identifier;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    if (SupplierCode=="")
                    {
                        identifier = "1";
                        stored = "ASYS_REMJewelryOutsourceAll";
                    }
                    else
                    {
                        identifier = "2";
                        stored = "ASYS_REMJewelryOutsource";
                    }
                    using (SqlCommand cmd = new SqlCommand(stored, con))
                    {
                        if (identifier=="1")
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 600;
                            read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@SupplierCode", SupplierCode);
                            cmd.Parameters.AddWithValue("@ItemCode", ItemCode);
                            cmd.Parameters.AddWithValue("@rptFlag", rptFlag);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 600;
                            read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        }
                        

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_REMJewelryOutsource();
                                a.TYPE = read["TYPE"].ToString().Trim();
                                a.SupplierCode = read["SupplierCode"].ToString().Trim();
                                a.ALLbarcode = read["ALLbarcode"].ToString().Trim();
                                a.ALL_desc = read["ALL_desc"].ToString().Trim();
                                a.Qty = string.IsNullOrEmpty(read["Qty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["Qty"].ToString().Trim());
                                a.KaratGrading = read["KaratGrading"].ToString().Trim();
                                a.Weight = string.IsNullOrEmpty(read["Weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["Weight"].ToString().Trim());
                                a.CaratSize = string.IsNullOrEmpty(read["CaratSize"].ToString().Trim()) ? 0 : Convert.ToDouble(read["CaratSize"].ToString().Trim());
                                a.ALL_Cost = string.IsNullOrEmpty(read["ALL_Cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["ALL_Cost"].ToString().Trim());
                                a.itemcode = read["itemcode"].ToString().Trim();
                                list.remJewelryOutsource.Add(a);

                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", remJewelryOutsource = list.remJewelryOutsource };
                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_REMOutsource_Inventory: " + ex.Message);
                    con.Close();

                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }

        }
        [WebMethod]
        public MLWB ASYS_REMJewelryOutsourceAll()
        {
            log.Info("Logging starts here: ASYS_REMJewelryOutsourceAll");
            var list = new MLWB();
            list.remJewelryOutsourceAll = new List<ASYS_REMJewelryOutsourceAll>();
            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_REMJewelryOutsourceAll", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_REMJewelryOutsourceAll();
                                a.Status = read["Status"].ToString().Trim();
                                a.actionclass = read["actionclass"].ToString().Trim();
                                a.TYPE = read["TYPE"].ToString().Trim();
                                a.SupplierCode = read["SupplierCode"].ToString().Trim();
                                a.ALLbarcode = read["ALLbarcode"].ToString().Trim();
                                a.ALL_desc = read["ALL_desc"].ToString().Trim();
                                a.Qty = string.IsNullOrEmpty(read["Qty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["Qty"]);
                                a.KaratGrading = read["KaratGrading"].ToString().Trim();
                                a.Weight = string.IsNullOrEmpty(read["Weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["Weight"]);
                                a.CaratSize = string.IsNullOrEmpty(read["CaratSize"].ToString().Trim()) ? 0 : Convert.ToDouble(read["CaratSize"]);
                                a.SerialNo = read["SerialNo"].ToString().Trim();
                                a.ALL_Cost = string.IsNullOrEmpty(read["ALL_Cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["ALL_Cost"]);
                                a.Itemcode = read["Itemcode"].ToString().Trim();
                                list.remJewelryOutsourceAll.Add(a);
                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", remJewelryOutsourceAll = list.remJewelryOutsourceAll };
                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_REMJewelryOutsourceAll: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB ASYS_REMJewelryReturn(string SupplierCode, string ItemCode, string rptFlag)
        {
            log.Info("Logging starts here: ASYS_REMJewelryReturn");
            var list = new MLWB();
            list.remJewelryReturn = new List<ASYS_REMJewelryReturns>();
            SqlDataReader read;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_REMJewelryReturns", con))
                    {
                        cmd.Parameters.AddWithValue("@SupplierCode", SupplierCode);
                        cmd.Parameters.AddWithValue("@ItemCode", ItemCode);
                        cmd.Parameters.AddWithValue("@rptFlag", rptFlag);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_REMJewelryReturns();
                                a.TYPE = read["TYPE"].ToString().Trim();
                                a.ALLBarcode = read["ALLBarcode"].ToString().Trim();
                                a.SupplierCode = read["SupplierCode"].ToString().Trim();
                                a.ALL_desc = read["ALL_desc"].ToString().Trim();
                                a.Qty = string.IsNullOrEmpty(read["Qty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["Qty"]);
                                a.KaratGrading = read["KaratGrading"].ToString().Trim();
                                a.Weight = string.IsNullOrEmpty(read["Weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["Weight"]);
                                a.CaratSize = string.IsNullOrEmpty(read["CaratSize"].ToString().Trim()) ? 0 : Convert.ToDouble(read["CaratSize"]);
                                a.ALL_Cost = string.IsNullOrEmpty(read["ALL_Cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["ALL_Cost"]);
                                a.Itemcode = read["Itemcode"].ToString().Trim();
                                list.remJewelryReturn.Add(a);
                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", remJewelryReturn = list.remJewelryReturn };
                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_REMJewelryReturn: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB ASYS_REMJewelryReturnALL()
        {
            log.Info("Logging starts here: ASYS_REMJewelryReturnALL");
            var list = new MLWB();
            list.remJewelryReturnAll = new List<ASYS_REMJewelryReturnsAll>();
            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_REMJewelryReturnALL", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_REMJewelryReturnsAll();
                                a.ALLbarcode = read["ALLbarcode"].ToString().Trim();
                                a.SupplierCode = read["SupplierCode"].ToString().Trim();
                                a.ALL_desc = read["ALL_desc"].ToString().Trim();
                                a.Qty = string.IsNullOrEmpty(read["Qty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["Qty"]);
                                a.price_karat = read["price_karat"].ToString().Trim();
                                a.price_weight = string.IsNullOrEmpty(read["price_weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["price_weight"]);
                                a.price_carat = string.IsNullOrEmpty(read["price_carat"].ToString().Trim()) ? 0 : Convert.ToDouble(read["price_carat"]);
                                a.SerialNo = read["SerialNo"].ToString().Trim();
                                a.cost = string.IsNullOrEmpty(read["cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["cost"]);
                                a.Itemcode = read["Itemcode"].ToString().Trim();
                                list.remJewelryReturnAll.Add(a);

                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", remJewelryReturnAll = list.remJewelryReturnAll };
                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_REMJewelryReturnALL: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB ASYS_TradeInInventory(string Username)
        {
            log.Info("Logging starts here: ASYS_TradeInInventory");
            var list = new MLWB();
            list.tradeInInventory = new List<ASYS_TradeInInventory>();
            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_TradeInInventory", con))
                    {
                        cmd.Parameters.AddWithValue("@Username", Username);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_TradeInInventory();
                                a.Division = read["Division"].ToString().Trim();
                                a.Divisionname = read["Divisionname"].ToString().Trim();
                                a.Transaction_No = read["Transaction_No"].ToString().Trim();
                                a.Appraisal_Amount = string.IsNullOrEmpty(read["Appraisal_Amount"].ToString().Trim()) ? 0 : Convert.ToDouble(read["Appraisal_Amount"]);
                                a.Reflotno = read["Reflotno"].ToString().Trim();
                                a.Itemcode = read["Itemcode"].ToString().Trim();
                                a.Description = read["Description"].ToString().Trim();
                                a.Quantity = string.IsNullOrEmpty(read["Quantity"].ToString().Trim()) ? 0 : Convert.ToInt32(read["Quantity"]);
                                a.Karat = read["Karat"].ToString().Trim();
                                a.Carat = string.IsNullOrEmpty(read["Carat"].ToString().Trim()) ? 0 : Convert.ToDouble(read["Carat"]);
                                a.Weight = string.IsNullOrEmpty(read["Weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["Weight"]);
                                a.ReceiveDate = read["ReceiveDate"].ToString().Trim();
                                a.Receiver = read["Receiver"].ToString().Trim();
                                a.ReleaseDate = read["ReleaseDate"].ToString().Trim();
                                a.Releaser = read["Releaser"].ToString().Trim();
                                a.Status = read["Status"].ToString().Trim();
                                list.tradeInInventory.Add(a);

                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", tradeInInventory = list.tradeInInventory };
                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_TradeInInventory: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB ASYS_REM_ReturnInventory_V5S(string pactionclass)
        {
            log.Info("Logging starts here: ASYS_REM_ReturnInventory_V5S");
            var list = new MLWB();
            list.spREMInventory = new List<ASYS_spREMInventory>();
            SqlDataReader read;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_spREMInventory_V5S", con))
                    {
                        cmd.Parameters.AddWithValue("@pactionclass", pactionclass);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_spREMInventory();
                                a.branchcode = read["branchcode"].ToString().Trim();
                                a.branchname = read["branchname"].ToString().Trim();
                                a.refallbarcode = read["refallbarcode"].ToString().Trim();
                                a.ptn = read["ptn"].ToString().Trim();
                                a.loanvalue = string.IsNullOrEmpty(read["loanvalue"].ToString().Trim()) ? 0 : Convert.ToDouble(read["loanvalue"]);
                                a.refitemcode = read["refitemcode"].ToString().Trim();
                                a.all_desc = read["all_desc"].ToString().Trim();
                                a.karatgrading = read["karatgrading"].ToString().Trim();
                                a.caratsize = string.IsNullOrEmpty(read["caratsize"].ToString().Trim()) ? 0 : Convert.ToDouble(read["caratsize"]);
                                a.SerialNo = read["SerialNo"].ToString().Trim();
                                a.Qty = string.IsNullOrEmpty(read["Qty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["Qty"]);
                                a.weight = string.IsNullOrEmpty(read["weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["weight"]);
                                a.actionclass = read["actionclass"].ToString().Trim();
                                a.sortcode = read["sortcode"].ToString().Trim();
                                a.sortername = read["sortername"].ToString().Trim();
                                a.sortdate = read["sortdate"].ToString().Trim();
                                a.cost = string.IsNullOrEmpty(read["cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["cost"]);
                                list.spREMInventory.Add(a);
                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", spREMInventory = list.spREMInventory };

                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_REM_ReturnInventory_V5S: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }

        }
        [WebMethod]
        public MLWB ASYS_REMInventory_V5S(string pactionclass)
        {
            log.Info("Logging starts here: ASYS_REMInventory_V5S");
            var list = new MLWB();
            list.spREMInventory = new List<ASYS_spREMInventory>();
            SqlDataReader read;

            if (pactionclass=="Missing Item" || pactionclass=="MISSING ITEM")
            {
                pactionclass = "missingitem";
            }
            else if (pactionclass == "Over Appraised" || pactionclass == "OVER APPRAISED")
            {
                pactionclass = "overappraised";
            }
          
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_spREMInventory_V5S", con))
                    {
                        cmd.Parameters.AddWithValue("@pactionclass", pactionclass);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_spREMInventory();
                                a.branchcode = read["branchcode"].ToString().Trim();
                                a.branchname = read["branchname"].ToString().Trim();
                                a.refallbarcode = read["refallbarcode"].ToString().Trim();
                                a.ptn = read["ptn"].ToString().Trim();
                                a.loanvalue = string.IsNullOrEmpty(read["loanvalue"].ToString().Trim()) ? 0 : Convert.ToDouble(read["loanvalue"]);
                                a.refitemcode = read["refitemcode"].ToString().Trim();
                                a.all_desc = read["all_desc"].ToString().Trim();
                                a.karatgrading = read["karatgrading"].ToString().Trim();
                                a.caratsize = string.IsNullOrEmpty(read["caratsize"].ToString().Trim()) ? 0 : Convert.ToDouble(read["caratsize"]);
                                a.SerialNo = read["SerialNo"].ToString().Trim();
                                a.Qty = string.IsNullOrEmpty(read["Qty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["Qty"]);
                                a.weight = string.IsNullOrEmpty(read["weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["weight"]);
                                a.actionclass = read["actionclass"].ToString().Trim();
                                a.sortcode = read["sortcode"].ToString().Trim();
                                a.sortername = read["sortername"].ToString().Trim();
                                a.sortdate = read["sortdate"].ToString().Trim();
                                a.cost = string.IsNullOrEmpty(read["cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["cost"]);
                                list.spREMInventory.Add(a);
                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", spREMInventory = list.spREMInventory };

                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_REMInventory_V5S: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }

        }
        [WebMethod]
        public MLWB ASYS_REMInventory(string pactionclass, string zone)
        {
            log.Info("Logging starts here: ASYS_REMInventory");
            string db;
            var list = new MLWB();
            list.spREMInventory = new List<ASYS_spREMInventory>();
            SqlDataReader read;

            if (zone == "remsVISMIN")
            {
                 db= remsVismin;
            }
            else if (zone == "REMSLUZON")
            {
                 db= remsLuzon;
            }
            else
            {
                db = constring;  
            }
            


            using (SqlConnection con = new SqlConnection(db))

            { 
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_spREMInventory", con))
                    {
                        cmd.Parameters.AddWithValue("@pactionclass", pactionclass);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_spREMInventory();
                                a.branchcode = read["branchcode"].ToString().Trim();
                                a.branchname = read["branchname"].ToString().Trim();
                                a.refallbarcode = read["refallbarcode"].ToString().Trim();
                                a.ptn = read["ptn"].ToString().Trim();
                                a.loanvalue = string.IsNullOrEmpty(read["loanvalue"].ToString().Trim()) ? 0 : Convert.ToDouble(read["loanvalue"]);
                                a.refitemcode = read["refitemcode"].ToString().Trim();
                                a.all_desc = read["all_desc"].ToString().Trim();
                                a.karatgrading = read["karatgrading"].ToString().Trim();
                                a.caratsize = string.IsNullOrEmpty(read["caratsize"].ToString().Trim()) ? 0 : Convert.ToDouble(read["caratsize"]);
                                a.SerialNo = read["SerialNo"].ToString().Trim();
                                a.Qty = string.IsNullOrEmpty(read["Qty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["Qty"]);
                                a.weight = string.IsNullOrEmpty(read["weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["weight"]);
                                a.actionclass = read["actionclass"].ToString().Trim();
                                a.sortcode = read["sortcode"].ToString().Trim();
                                a.sortername = read["sortername"].ToString().Trim();
                                a.sortdate = read["sortdate"].ToString().Trim();
                                a.cost = string.IsNullOrEmpty(read["cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["cost"]);
                                list.spREMInventory.Add(a);
                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", spREMInventory = list.spREMInventory };

                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_REMInventory_V5S: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }

        }
        [WebMethod]
        public MLWB ASYS_InventorybyConsignStockAll(string ConTYPE)
        {
            log.Info("Logging starts here: ASYS_InventorybyConsignStockAll");
            var list = new MLWB();
            list.inventoryallconsign = new List<ASYS_InventoryALLConsign>();
            SqlDataReader read;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_InventoryALLConsign", con))
                    {
                        cmd.Parameters.AddWithValue("@ConTYPE", ConTYPE);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_InventoryALLConsign();
                                a.consignee = read["consignee"].ToString().Trim();
                                a.consigncode = read["consigncode"].ToString().Trim();
                                a.consignname = read["consignname"].ToString().Trim();
                                a.consigndate = string.IsNullOrEmpty(read["consigndate"].ToString()) ? null : Convert.ToDateTime(read["consigndate"]).ToString("MMMM dd, yyyy").Trim();
                                a.allbarcode = read["allbarcode"].ToString().Trim();
                                a.qty = string.IsNullOrEmpty(read["qty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["qty"]);
                                a.description = read["description"].ToString().Trim();
                                a.grams = string.IsNullOrEmpty(read["grams"].ToString().Trim()) ? 0 : Convert.ToDouble(read["grams"]);
                                a.karat = read["karat"].ToString().Trim();
                                a.carat = string.IsNullOrEmpty(read["carat"].ToString().Trim()) ? 0 : Convert.ToDouble(read["carat"]);
                                a.serialno = read["serialno"].ToString().Trim();
                                a.price = string.IsNullOrEmpty(read["price"].ToString().Trim()) ? 0 : Convert.ToDouble(read["price"]);
                                list.inventoryallconsign.Add(a);

                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", inventoryallconsign = list.inventoryallconsign };
                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_InventorybyConsignStockAll: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB ASYS_InventorybyuNConsignStock()
        {
            log.Info("Logging starts here: ASYS_InventorybyuNConsignStock");
            var list = new MLWB();
            list.reportsInventoryUnconsignment = new List<ASYS_ReportsInventoryUnconsignment>();
            SqlDataReader read;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_ReportsInventoryUnconsignment", con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_ReportsInventoryUnconsignment();
                                a.RefQty = string.IsNullOrEmpty(read["RefQty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["RefQty"]);
                                a.RefALLBarcode = read["RefALLBarcode"].ToString().Trim();
                                a.Price_desc = read["Price_desc"].ToString().Trim();
                                a.Price_weight = string.IsNullOrEmpty(read["Price_weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["Price_weight"]);
                                a.Price_karat = read["Price_karat"].ToString().Trim();
                                a.Price_carat = string.IsNullOrEmpty(read["Price_carat"].ToString().Trim()) ? 0 : Convert.ToDouble(read["Price_carat"]);
                                a.SerialNo = read["SerialNo"].ToString().Trim();
                                a.ALL_price = string.IsNullOrEmpty(read["ALL_price"].ToString().Trim()) ? 0 : Convert.ToDouble(read["ALL_price"]);
                                list.reportsInventoryUnconsignment.Add(a);
                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", reportsInventoryUnconsignment = list.reportsInventoryUnconsignment };
                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_InventorybyuNConsignStock: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB ASYS_InventorybyCostCenter_V5S(string CostCenter)
        {
            log.Info("Logging starts here: ASYS_InventorybyCostCenter_V5S");
            var list = new MLWB();
            list.inventorybyCostCenter_V5S = new List<ASYS_InventorybyCostCenter_V5S>();
            SqlDataReader read;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_InventorybyCostCenter_V5S", con))
                    {
                        cmd.Parameters.AddWithValue("@CostCenter", CostCenter);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_InventorybyCostCenter_V5S();
                                a.Branchcode = read["Branchcode"].ToString().Trim();
                                a.Branchname = read["Branchname"].ToString().Trim();
                                a.Costcenter = read["Costcenter"].ToString().Trim();
                                a.PTN = read["PTN"].ToString().Trim();
                                a.refallbarcode = read["refallbarcode"].ToString().Trim();
                                a.refitemcode = read["refitemcode"].ToString().Trim();
                                a.refqty = string.IsNullOrEmpty(read["refqty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["refqty"]);
                                a.desc = read["desc"].ToString().Trim();
                                a.karat = read["karat"].ToString().Trim();
                                a.carat = string.IsNullOrEmpty(read["carat"].ToString().Trim()) ? 0 : Convert.ToDouble(read["carat"]);
                                a.SerialNo = read["SerialNo"].ToString().Trim();
                                a.weight = string.IsNullOrEmpty(read["weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["weight"]);
                                a.cost = string.IsNullOrEmpty(read["cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["cost"]);
                                a.date = read["date"].ToString().Trim();
                                a.loanvalue = string.IsNullOrEmpty(read["loanvalue"].ToString().Trim()) ? 0 : Convert.ToDouble(read["loanvalue"]);
                                a.transNo = read["transNo"].ToString().Trim();
                                list.inventorybyCostCenter_V5S.Add(a);
                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", inventorybyCostCenter_V5S = list.inventorybyCostCenter_V5S };
                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_InventorybyCostCenter_V5S: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB ASYS_InventorybyItemcode(string itemcode, string costcenter)
        {
            log.Info("Logging starts here: ASYS_InventorybyItemcode");
            var list = new MLWB();
            list.inventorybyItemcode = new List<ASYS_InventorybyItemcode>();
            SqlDataReader read;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_InventorybyItemcode", con))
                    {
                        cmd.Parameters.AddWithValue("@itemcode", itemcode);
                        cmd.Parameters.AddWithValue("@costcenter", costcenter);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_InventorybyItemcode();
                                a.Branchcode = read["Branchcode"].ToString().Trim();
                                a.Branchname = read["Branchname"].ToString().Trim();
                                a.Costcenter = read["Costcenter"].ToString().Trim();
                                a.ptn = read["ptn"].ToString().Trim();
                                a.refallbarcode = read["refallbarcode"].ToString().Trim();
                                a.refitemcode = read["refitemcode"].ToString().Trim();
                                a.refqty = string.IsNullOrEmpty(read["refqty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["refqty"]);
                                a.desc = read["desc"].ToString().Trim();
                                a.karat = read["karat"].ToString().Trim();
                                a.carat = string.IsNullOrEmpty(read["carat"].ToString().Trim()) ? 0 : Convert.ToDouble(read["carat"]);
                                a.weight = string.IsNullOrEmpty(read["weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["weight"]);
                                a.cost = string.IsNullOrEmpty(read["cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["cost"]);
                                a.date = read["date"].ToString().Trim();
                                list.inventorybyItemcode.Add(a);
                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success", inventorybyItemcode = list.inventorybyItemcode };
                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_InventorybyItemcode: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB ASYS_InventorybyItemsource(string itemsource)
        {
            log.Info("Logging starts here: ASYS_InventorybyItemsource");
            var list = new MLWB();
            list.inventorybyItemsource = new List<ASYS_InventorybyItemsource>();
            SqlDataReader read;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_InventorybyItemsource", con))
                    {
                        cmd.Parameters.AddWithValue("@itemsource", itemsource);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_InventorybyItemsource();
                                a.Branchcode = read["Branchcode"].ToString().Trim();
                                a.Branchname = read["Branchname"].ToString().Trim();
                                a.Costcenter = read["Costcenter"].ToString().Trim();
                                a.ptn = read["ptn"].ToString().Trim();
                                a.refallbarcode = read["refallbarcode"].ToString().Trim();
                                a.itemsource = read["itemsource"].ToString().Trim();
                                a.refitemcode = read["refitemcode"].ToString().Trim();
                                a.refqty = string.IsNullOrEmpty(read["refqty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["refqty"]);
                                a.desc = read["desc"].ToString().Trim();
                                a.karat = read["karat"].ToString().Trim();
                                a.carat = string.IsNullOrEmpty(read["carat"].ToString().Trim()) ? 0 : Convert.ToDouble(read["carat"]);
                                a.weight = string.IsNullOrEmpty(read["weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["weight"]);
                                a.cost = string.IsNullOrEmpty(read["cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["cost"]);
                                a.date = read["date"].ToString().Trim();
                                list.inventorybyItemsource.Add(a);
                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success!", inventorybyItemsource = list.inventorybyItemsource };
                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_InventorybyItemsource: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB ASYS_MLWBInventory(string receiver)
        {
            log.Info("Logging starts here: ASYS_MLWBInventory");
            var list = new MLWB();
            list.asys_mlwbInventory = new List<ASYS_MLWBInventory>();
            SqlDataReader read;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_MLWBInventory", con))
                    {
                        cmd.Parameters.AddWithValue("@receiver", receiver);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_MLWBInventory();
                                a.receiver = read["receiver"].ToString().Trim();
                                a.employee = read["employee"].ToString().Trim();
                                a.receivedate = read["receivedate"].ToString().Trim();
                                a.reflotno = read["reflotno"].ToString().Trim();
                                a.lotno = read["lotno"].ToString().Trim();
                                a.refallbarcode = read["refallbarcode"].ToString().Trim();
                                a.allbarcode = read["allbarcode"].ToString().Trim();
                                a.ptn = read["ptn"].ToString().Trim();
                                a.itemid = string.IsNullOrEmpty(read["itemid"].ToString().Trim()) ? 0 : Convert.ToInt32(read["itemid"]);
                                a.ptnbarcode = read["ptnbarcode"].ToString().Trim();
                                a.branchcode = read["branchcode"].ToString().Trim();
                                a.branchname = read["branchname"].ToString().Trim();
                                a.loanvalue = string.IsNullOrEmpty(read["loanvalue"].ToString().Trim()) ? 0 : Convert.ToDouble(read["loanvalue"]);
                                a.refitemcode = read["refitemcode"].ToString().Trim();
                                a.branchitemdesc = read["branchitemdesc"].ToString().Trim();
                                a.refqty = string.IsNullOrEmpty(read["refqty"].ToString()) ? 0 : Convert.ToInt32(read["refqty"]);
                                a.qty = string.IsNullOrEmpty(read["qty"].ToString()) ? 0 : Convert.ToInt32(read["qty"]);
                                a.karatgrading = read["karatgrading"].ToString().Trim();
                                a.caratsize = string.IsNullOrEmpty(read["caratsize"].ToString().Trim()) ? 0 : Convert.ToDouble(read["caratsize"]);
                                a.weight = string.IsNullOrEmpty(read["weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["weight"]);
                                a.actionclass = read["actionclass"].ToString().Trim();
                                a.sortcode = read["sortcode"].ToString().Trim();
                                a.all_desc = read["all_desc"].ToString().Trim();
                                a.all_karat = read["all_karat"].ToString().Trim();
                                a.all_carat = string.IsNullOrEmpty(read["all_carat"].ToString().Trim()) ? 0 : Convert.ToDouble(read["all_carat"]);
                                a.SerialNo = read["SerialNo"].ToString().Trim();
                                a.all_cost = string.IsNullOrEmpty(read["all_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["all_cost"]);
                                a.all_weight = string.IsNullOrEmpty(read["all_weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["all_weight"]);
                                a.appraisevalue = string.IsNullOrEmpty(read["appraisevalue"].ToString().Trim()) ? 0 : Convert.ToDouble(read["appraisevalue"]);
                                a.currency = read["currency"].ToString().Trim();
                                a.photoname = read["photoname"].ToString().Trim();
                                a.price_desc = read["price_desc"].ToString().Trim();
                                a.price_karat = read["price_karat"].ToString().Trim();
                                a.price_weight = string.IsNullOrEmpty(read["price_weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["price_weight"]);
                                a.price_carat = string.IsNullOrEmpty(read["price_carat"].ToString().Trim()) ? 0 : Convert.ToDouble(read["price_carat"]);
                                a.all_price = string.IsNullOrEmpty(read["all_price"].ToString().Trim()) ? 0 : Convert.ToDouble(read["all_price"]);
                                a.cellular_cost = string.IsNullOrEmpty(read["cellular_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["cellular_cost"]);
                                a.watch_cost = string.IsNullOrEmpty(read["watch_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["watch_cost"]);
                                a.repair_cost = string.IsNullOrEmpty(read["repair_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["repair_cost"]);
                                a.cleaning_cost = string.IsNullOrEmpty(read["cleaning_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["cleaning_cost"]);
                                a.gold_cost = string.IsNullOrEmpty(read["gold_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["gold_cost"]);
                                a.mount_cost = string.IsNullOrEmpty(read["mount_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["mount_cost"]);
                                a.yg_cost = string.IsNullOrEmpty(read["yg_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["yg_cost"]);
                                a.wg_cost = string.IsNullOrEmpty(read["wg_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["wg_cost"]);
                                a.costdate = read["costdate"].ToString().Trim();
                                a.costname = read["costname"].ToString().Trim();
                                a.releasedate = read["releasedate"].ToString().Trim();
                                a.releaser = read["releaser"].ToString().Trim();
                                a.maturitydate = read["maturitydate"].ToString().Trim();
                                a.expirydate = read["expirydate"].ToString().Trim();
                                a.status = read["status"].ToString().Trim();
                                a.month = read["month"].ToString().Trim();
                                a.year = read["year"].ToString().Trim();
                                list.asys_mlwbInventory.Add(a);

                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success!", asys_mlwbInventory = list.asys_mlwbInventory };


                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_MLWBInventory: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB ASYS_PRICINGInventory(string receiver)
        {
            log.Info("Logging starts here: ASYS_PRICINGInventory");
            var list = new MLWB();
            list.asys_pricingInventory = new List<ASYS_PRICINGInventory>();
            SqlDataReader read;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_PRICINGInventory", con))
                    {
                        cmd.Parameters.AddWithValue("@receiver", receiver);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_PRICINGInventory();
                                a.receiver = read["receiver"].ToString().Trim();
                                a.employee = read["employee"].ToString().Trim();
                                a.receivedate = read["receivedate"].ToString().Trim();
                                a.reflotno = read["reflotno"].ToString().Trim();
                                a.lotno = read["lotno"].ToString().Trim();
                                a.refallbarcode = read["refallbarcode"].ToString().Trim();
                                a.allbarcode = read["allbarcode"].ToString().Trim();
                                a.ptn = read["ptn"].ToString().Trim();
                                a.itemid = string.IsNullOrEmpty(read["itemid"].ToString().Trim()) ? 0 : Convert.ToInt32(read["itemid"]);
                                a.ptnbarcode = read["ptnbarcode"].ToString().Trim();
                                a.branchcode = read["branchcode"].ToString().Trim();
                                a.branchname = read["branchname"].ToString().Trim();
                                a.loanvalue = string.IsNullOrEmpty(read["loanvalue"].ToString().Trim()) ? 0 : Convert.ToDouble(read["loanvalue"]);
                                a.refitemcode = read["refitemcode"].ToString().Trim();
                                a.branchitemdesc = read["branchitemdesc"].ToString().Trim();
                                a.refqty = string.IsNullOrEmpty(read["refqty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["refqty"]);
                                a.qty = string.IsNullOrEmpty(read["qty"].ToString().Trim()) ? 0 : Convert.ToInt32(read["qty"]);
                                a.karatgrading = read["karatgrading"].ToString().Trim();
                                a.caratsize = string.IsNullOrEmpty(read["caratsize"].ToString().Trim()) ? 0 : Convert.ToDouble(read["caratsize"]);
                                a.weight = string.IsNullOrEmpty(read["weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["weight"]);
                                a.actionclass = read["actionclass"].ToString().Trim();
                                a.sortcode = read["sortcode"].ToString().Trim();
                                a.all_desc = read["all_desc"].ToString().Trim();
                                a.all_karat = read["all_karat"].ToString().Trim();
                                a.all_carat = string.IsNullOrEmpty(read["all_carat"].ToString().Trim()) ? 0 : Convert.ToDouble(read["all_carat"]);
                                a.SerialNo = read["SerialNo"].ToString().Trim();
                                a.all_cost = string.IsNullOrEmpty(read["all_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["all_cost"]);
                                a.all_weight = string.IsNullOrEmpty(read["all_weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["all_weight"]);
                                a.appraisevalue = string.IsNullOrEmpty(read["appraisevalue"].ToString().Trim()) ? 0 : Convert.ToDouble(read["appraisevalue"]);
                                a.currency = read["currency"].ToString().Trim();
                                a.photoname = read["photoname"].ToString().Trim();
                                a.price_desc = read["price_desc"].ToString().Trim();
                                a.price_karat = read["price_karat"].ToString().Trim();
                                a.price_weight = string.IsNullOrEmpty(read["price_weight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["price_weight"]);
                                a.price_carat = string.IsNullOrEmpty(read["price_carat"].ToString().Trim()) ? 0 : Convert.ToDouble(read["price_carat"]);
                                a.all_price = string.IsNullOrEmpty(read["all_price"].ToString().Trim()) ? 0 : Convert.ToDouble(read["all_price"]);
                                a.cellular_cost = string.IsNullOrEmpty(read["cellular_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["cellular_cost"]);
                                a.watch_cost = string.IsNullOrEmpty(read["watch_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["watch_cost"]);
                                a.repair_cost = string.IsNullOrEmpty(read["repair_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["repair_cost"]);
                                a.cleaning_cost = string.IsNullOrEmpty(read["cleaning_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["cleaning_cost"]);
                                a.gold_cost = string.IsNullOrEmpty(read["gold_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["gold_cost"]);
                                a.mount_cost = string.IsNullOrEmpty(read["mount_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["mount_cost"]);
                                a.yg_cost = string.IsNullOrEmpty(read["yg_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["yg_cost"]);
                                a.wg_cost = string.IsNullOrEmpty(read["wg_cost"].ToString().Trim()) ? 0 : Convert.ToDouble(read["wg_cost"]);
                                a.costdate = read["costdate"].ToString().Trim();
                                a.costname = read["costname"].ToString().Trim();
                                a.releasedate = read["releasedate"].ToString().Trim();
                                a.releaser = read["releaser"].ToString().Trim();
                                a.maturitydate = read["maturitydate"].ToString().Trim();
                                a.expirydate = read["expirydate"].ToString().Trim();
                                a.status = read["status"].ToString().Trim();
                                a.month = read["month"].ToString().Trim();
                                a.year = read["year"].ToString().Trim();
                                list.asys_pricingInventory.Add(a);

                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success!", asys_pricingInventory = list.asys_pricingInventory };


                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_PRICINGInventory: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB ASYS_DISTRIInventory(string receiver)
        {
            log.Info("Logging starts here: ASYS_DISTRIInventory");
            var list = new MLWB();
            list.asys_distriInventory = new List<ASYS_DISTRIInventory>();
            SqlDataReader read;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("ASYS_DistriInventory", con))
                    {
                        cmd.Parameters.AddWithValue("@receiver", receiver);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                var a = new ASYS_DISTRIInventory();
                                a.receiver = read["receiver"].ToString().Trim();
                                a.employee = read["employee"].ToString().Trim();
                                a.receivedate = read["receivedate"].ToString().Trim();
                                a.lotno = read["lotno"].ToString().Trim();
                                a.allbarcode = read["allbarcode"].ToString().Trim();
                                a.division = read["division"].ToString().Trim();
                                a.divisionname = read["divisionname"].ToString().Trim();
                                a.itemcode = read["itemcode"].ToString().Trim();
                                a.quantity = string.IsNullOrEmpty(read["quantity"].ToString().Trim()) ? 0 : Convert.ToInt32(read["quantity"]);
                                a.actionclass = read["actionclass"].ToString().Trim();
                                a.sortcode = read["sortcode"].ToString().Trim();
                                a.currency = read["currency"].ToString().Trim();
                                a.pricedesc = read["pricedesc"].ToString().Trim();
                                a.pricekarat = read["pricekarat"].ToString().Trim();
                                a.priceweight = string.IsNullOrEmpty(read["priceweight"].ToString().Trim()) ? 0 : Convert.ToDouble(read["priceweight"]);
                                a.pricecarat = string.IsNullOrEmpty(read["pricecarat"].ToString().Trim()) ? 0 : Convert.ToDouble(read["pricecarat"]);
                                a.SerialNo = read["SerialNo"].ToString().Trim();
                                a.allprice = string.IsNullOrEmpty(read["allprice"].ToString().Trim()) ? 0 : Convert.ToDouble(read["allprice"]);
                                a.status = read["status"].ToString().Trim();
                                a.month = read["month"].ToString().Trim();
                                a.year = read["year"].ToString().Trim();
                                list.asys_distriInventory.Add(a);

                            }
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "1", respMsg = "Success!", asys_distriInventory = list.asys_distriInventory };
                        }
                        else
                        {
                            read.Close();
                            con.Close();
                            return new MLWB { respCode = "0", respMsg = "No records found!" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ASYS_DISTRIInventory: " + ex.Message);
                    con.Close();
                    return new MLWB { respCode = "0", respMsg = "service error:" + ex.Message };
                }
            }
        }
        [WebMethod]
        public MLWB frmMLWBInventory_Load()
        {

            log.Info("Logging starts here: frmMLWBInventory_Load");
            var List = new MLWB();
            List.MLWBReportData = new MLWBReport();
            List.MLWBReportData.fullname = new List<string>();

            SqlDataReader read;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("select distinct rtrim(fullname) as fullname from vw_humresall where job_title in ('mlwb')", con))
                        {
                            read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                            while (read.Read())
                            {
                                List.MLWBReportData.fullname.Add(read["fullname"].ToString().Trim());
                                return new MLWB { respCode = "1", respMsg = "Success", MLWBReportData = List.MLWBReportData };
                            }                        
                                
                            
                        }
                        return new MLWB { respCode = "0", respMsg = "No records found!" };
                    }
                    catch (Exception ex)
                    {
                        log.Error("frmMLWBInventory_Load: " + ex.Message);
                        return new MLWB { respCode = "0", respMsg = "service Error" + ex.Message };
                    }
                }
            }

        }
        [WebMethod]
        public MLWB Label7_KeyPress(string Label7)
        {
            log.Info("Logging starts here: Label7_KeyPress");
            SqlDataReader read;
            var List = new MLWB();
            List.habwaDateData = new habwa_date();
            List.habwaDateData._month = new List<string>();
            List.habwaDateData._day = new List<string>();
            List.habwaDateData._year = new List<string>();
            List.habwaDateData._receivedate = new List<string>();

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("Select month(receivedate) as month, day(receivedate) as day, year(receivedate) as year, receivedate from asys_MLWB_detail where lotno = '" + Label7 + "' order by receivedate desc", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.Read())
                        {

                            List.habwaDateData._month.Add(read["month"].ToString().Trim());
                            List.habwaDateData._day.Add(read["day"].ToString().Trim());
                            List.habwaDateData._year.Add(read["year"].ToString().Trim());
                            List.habwaDateData._receivedate.Add(read["receivedate"].ToString().Trim());
                        }
                    }
                }
                catch (Exception ex)
                {

                    log.Error("Label7_KeyPress: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "service Error" + ex.Message };
                }
            }

            return new MLWB { respCode = "1", respMsg = "Success", habwaDateData =List.habwaDateData };
        }
        [WebMethod]
        public MLWB Label7_KeyPress2(string Label7)
        {
            log.Info("Logging starts here: Label7_KeyPress2");
            SqlDataReader read;
            var List = new MLWB();
            List.habwaDateData = new habwa_date();
            List.habwaDateData._month = new List<string>();
            List.habwaDateData._day = new List<string>();
            List.habwaDateData._year = new List<string>();
            List.habwaDateData._releasedate = new List<string>();

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("Select month(releasedate) as month, day(releasedate) as day, year(releasedate) as year, releasedate from asys_MLWB_detail where reflotno = '" + Label7 + "' and status = 'released' order by releasedate desc", con))
                    {
                        read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (read.Read())
                        {
                            
                            List.habwaDateData._month.Add(read["month"].ToString().Trim());
                            List.habwaDateData._day.Add(read["day"].ToString().Trim());
                            List.habwaDateData._year.Add(read["year"].ToString().Trim());
                            List.habwaDateData._releasedate.Add(read["releasedate"].ToString().Trim());
                        }
                    }
                }
                catch (Exception ex)
                {

                    log.Error("Label7_KeyPress2: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "service Error" + ex.Message };
                }
            }

            return new MLWB { respCode = "1", respMsg = "Success", habwaDateData = List.habwaDateData };
        }
        [WebMethod]
        public MLWB Update_Status(string department , int lotno)//Edit Item//save
        {
            log.Info("Logging starts here: Update_Status");
            SqlTransaction tran;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                tran = con.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("update ASYS_lotno_Gen set Lotno ='" + lotno + "'where BusinessCenter ='" + department + "'", con, tran))
                    {

                        cmd.ExecuteNonQuery();

                    }

               
                    tran.Commit();
                    return new MLWB { respCode = "1", respMsg = "Succescfuly Updated" };
                }
                catch (Exception ex)
                {
                    log.Error("Update_Status: " + ex.Message);
                    tran.Rollback();
                    return new MLWB { respCode = "1", respMsg = "Transaction Failed"};
                }
            }




        }
        [WebMethod]
        public MLWB callActionClass()
        {
            log.Info("Logging starts here: callActionClass");
            details inventory = new details();
            inventory.action_type = new List<string>();
            SqlDataReader rdr;

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("select * from tbl_action where action_id not in (1,2,7,12)", con))
                    {
                        rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (rdr.Read())
                        {
                           
                            inventory.action_type.Add(rdr["action_type"].ToString());

                        }

                    }
                    return new MLWB { respCode = "1", respMsg = "Success", MLWBInventory = inventory };
                }
                catch (Exception ex)
                {
                   log.Error("frmMainLoad: " + ex.Message);
                    return new MLWB { respCode = "0", respMsg = "Service Error:" + ex.Message };
                }
            }

            
        }

    }
}


