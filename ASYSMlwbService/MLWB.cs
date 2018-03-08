using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASYSMlwbService
{
    public class MLWB
    {
        public string respCode { get; set; }
        public string respMsg { get; set; }
        public getLotNumber data1 { get; set; }
        public lotNumberResult result { get; set; }
        public insertResult save { get; set; }
        public RetrieveData2Models RetrieveData2data { get; set; }
        public MLWBReport MLWBReportData { get; set; }
        public MLWBReport2 MLWBReportData2 { get; set; }
        public habwa_date habwaDateData { get; set; }
        public releasingLotnum releasingLotnumData { get; set; }
        public dbConnection dbConnectionData { get; set; }
        public details MLWBInventory { get; set; }
        public List<report> reportsResult { get; set; }
        public List<ASYS_SummaryGoodStocksRel_rpt> summaryGoodStock { get; set; }
        public List<ASYS_GetDeptUnreceived_Items> unreceivedItems { get; set; }
        public List<ASYS_SelfInventory_rpt> selfInventory { get; set; }
        public List<procASYS_MissingItems> selfInventoryMissing { get; set; }
        public List<ASYS_REMJewelryOutsource> remJewelryOutsource { get; set; }
        public List<ASYS_REMJewelryOutsourceAll> remJewelryOutsourceAll { get; set; }
        public List<ASYS_REMJewelryReturns> remJewelryReturn { get; set; }
        public List<ASYS_REMJewelryReturnsAll> remJewelryReturnAll { get; set; }
        public List<ASYS_TradeInInventory> tradeInInventory { get; set; }
        public List<ASYS_spREMInventory> spREMInventory { get; set; }
        public List<ASYS_InventoryALLConsign> inventoryallconsign { get; set; }
        public List<ASYS_ReportsInventoryUnconsignment> reportsInventoryUnconsignment { get; set; }
        public List<ASYS_InventorybyCostCenter_V5S> inventorybyCostCenter_V5S { get; set; }
        public List<ASYS_InventorybyItemcode> inventorybyItemcode { get; set; }
        public List<ASYS_InventorybyItemsource> inventorybyItemsource { get; set; }
        public List<ASYS_MLWBInventory> asys_mlwbInventory { get; set; }
        public List<ASYS_PRICINGInventory> asys_pricingInventory { get; set; }
        public List<ASYS_DISTRIInventory> asys_distriInventory { get; set; }
    }


    public class getLotNumber
    {
        public List<string> lotNumber { get; set; }
    }
    public class dbNull
    {
        public string isNull(object data)
        {
            if (Convert.IsDBNull(data))
            {
                return "0";
            }
            else
            {
                return data.ToString();
            }
        }
    }
    public class lotNumberResult
    {
        public List<string> no { get; set; }
        public List<string> ptn { get; set; }
        public List<string> allBarcode { get; set; }
        public List<string> itemSource { get; set; }
        public List<string> description { get; set; }
        public List<string> weight { get; set; }
        public List<string> karat { get; set; }
        public List<string> carat { get; set; }
        public List<string> price { get; set; }
        public string totalItems { get; set; }



    }
    public class insertResult
    {
        public string no { get; set; }
        public string ptn { get; set; }
        public string allBarcode { get; set; }
        public string itemSource { get; set; }
        public string description { get; set; }
        public string weight { get; set; }
        public string karat { get; set; }
        public string carat { get; set; }
        public string price { get; set; }
        public string totalItems { get; set; }

    }
    public class RetrieveData2Models
    {
        #region KAEREN
        public List<string> barcode { get; set; }
        public List<string> sortaction_id { get; set; }
        public List<string> alldesc { get; set; }
        
        public List<string> division { get; set; }
        public List<string> quantity { get; set; }

        public List<string> ptn { get; set; }
        public List<string> itemcode { get; set; }
        public List<string> ptnitemdesc { get; set; }
        public List<string> weight { get; set; }
        
        public List<string> karat { get; set; }
        
        public List<string> carat { get; set; }
        
        public List<string> price { get; set; }
        
        public List<string> allprice { get; set; }
        public string price_desc { get; set; }
        public string price_weight { get; set; }
        public string price_karat { get; set; }
        public string price_carat { get; set; }
        public string _ptn { get; set; }
        public string cost { get; set; }
        public string status { get; set; }

        #endregion

        public string _barcode { get; set; }
        public string _alldesc { get; set; }
        public string _weight { get; set; }
        public string _karat { get; set; }
        public string _carat { get; set; }
        public string _price { get; set; }


        public List<string> _barcode1 { get; set; }
        public List<string> _alldesc1 { get; set; }
        public List<string> _weight1 { get; set; }
        public List<string> _karat1 { get; set; }
        public List<string> _carat1 { get; set; }
        public List<string> _price1 { get; set; }
    }
    public class MLWBReport
    {
        public List<string> fullname { get; set; }
        public string _fullname { get; set; }
        public List<string> Consignee { get; set; }
        public List<string> costcenter { get; set; }
    }
    public class MLWBReport2
    {
        public string lotnum { get; set; }
        public string date { get; set; }

    }
    public class habwa_date
    {
        public string month { get; set; }
        public string year { get; set; }
        public string day { get; set; }
        public string receivedate { get; set; }
        public List<string> _month { get; set; }
        public List<string> _year { get; set; }
        public List<string> _day { get; set; }
        public List<string>  _receivedate { get; set; }
        public List<string> _releasedate { get; set; }
    }
    public class releasingLotnum
    {
        public List<string> lotnum { get; set; }
    }
    public class dbConnection
    {

        public string result { get; set; }
        public string respons { get; set; }
        public string status1 { get; set; }
        public string photoname { get; set; }
        public string status { get; set; }
        public string branchcode { get; set; }
        public string branchname { get; set; }
        public string actionclass { get; set; }
        public string sortcode { get; set; }
        public string ptn { get; set; }
        public string ptnbarcode { get; set; }
        public string Maturitydate { get; set; }
        public string Expirydate { get; set; }
        public string LoanValue { get; set; }
        public string LoanDate { get; set; }
        public string currency { get; set; }
        public string itemid { get; set; }
        public string ItemCode { get; set; }
        public string branchitemdesc { get; set; }
        public string qty { get; set; }
        public string Karatgrading { get; set; }
        public string Caratsize { get; set; }
        public string Weight { get; set; }
        public string all_cost { get; set; }
        public string all_desc { get; set; }
        public string SerialNo { get; set; }
        public string refqty { get; set; }
        public string ALL_Karat { get; set; }
        public string ALL_Carat { get; set; }
        public string ALL_Weight { get; set; }
        public string ALL_price { get; set; }
        public string cellular_cost { get; set; }
        public string watch_cost { get; set; }
        public string repair_cost { get; set; }
        public string cleaning_cost { get; set; }
        public string gold_cost { get; set; }
        public string mount_cost { get; set; }
        public string YG_cost { get; set; }
        public string WG_cost { get; set; }
        public string questionmark2 { get; set; }
        public double CostA { get; set; }
        public double CostB { get; set; }
        public double CostC { get; set; }
        public double CostD { get; set; }
        public string appraisevalue { get; set; }
        public string stat { get; set; }

    }
  
    public class details
    {
        public List<string> action_type { get; set; }
        public List<string> action_id { get; set; }
        public List<string> description { get; set; }
        public string CostDept { get; set; }
        public List<string> _CostDept { get; set; }
        public List<string> DISTRIBUTION { get; set; }
        public List<string> code { get; set; }
        public string photoname { get; set; }
        public string status { get; set; }
        public string branchcode { get; set; }
        public string branchname { get; set; }
        public string LoanValue { get; set; }
        public string actionclass { get; set; }
        public string sortcode { get; set; }
        public string itemid { get; set; }
        public int itemid1 { get; set; }
        public string Itemcode { get; set; }
        public string branchitemdesc { get; set; }
        public string qty { get; set; }
        public string Karatgrading { get; set; }
        public string Caratsize { get; set; }
        public string Weight { get; set; }
        public string all_cost { get; set; }
        public string price_desc { get; set; }
        public string serialno { get; set; }
        public string refqty { get; set; }
        public string price_karat { get; set; }
        public string price_carat { get; set; }
        public string price_weight { get; set; }
        public string ALL_price { get; set; }
        public string CostA { get; set; }
        public string CostB { get; set; }
        public string CostC { get; set; }
        public string CostD { get; set; }
        public string Gold_Cost { get; set; }
        public string Mount_Cost { get; set; }
        public string YG_Cost { get; set; }
        public string WG_Cost { get; set; }
        public string Cellular_Cost { get; set; }
        public string Repair_Cost { get; set; }
        public string Cleaning_Cost { get; set; }
        public string Watch_Cost { get; set; }
        public string userid { get; set; }
        public string barcode { get; set; }





    }
    public class report
    {
        public string COSTCENTER { get; set; }
        public string LOTNO { get; set; }
        public string REFALLBARCODE { get; set; }
        public string QTY { get; set; }
        public string SERIALNO { get; set; }
        public string DESCRIPTION { get; set; }
        public string KARAT { get; set; }
        public string WEIGHT { get; set; }
        public string CARAT { get; set; }
        public string ALL_PRICE { get; set; }
    }
    public class ASYS_SummaryGoodStocksRel_rpt
    {
        public string lotno { get; set; }
        public string reflotno { get; set; }
        public string division { get; set; }
        public string divisionname { get; set; }
        public string ptn { get; set; }
        public int itemid { get; set; }
        public string refallbarcode { get; set; }
        public string allbarcode { get; set; }
        public string itemcode { get; set; }
        public string all_desc { get; set; }
        public string itemdescription { get; set; }
        public int quantity { get; set; }
        public string actionclass { get; set; }
        public double loanvalue { get; set; }
        public double itemappraisevalue { get; set; }
        public double all_wt { get; set; }
        public string all_karat { get; set; }
        public double all_cost { get; set; }
        public double all_carat { get; set; }
        public string SerialNo { get; set; }
        public double all_price { get; set; }
        public string employee { get; set; }
        public string price_desc { get; set; }
        public string price_karat { get; set; }
        public double price_weight { get; set; }
        public double price_carat { get; set; }
        public string releasedate { get; set; }
        public string releaser { get; set; }
        public string receivedate { get; set; }
        public string receiver { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string status { get; set; }
    }
    public class ASYS_GetDeptUnreceived_Items
    {
        public string costcenter { get; set; }
        public string lotno { get; set; }
        public string refallbarcode { get; set; }
        public int qty { get; set; }
        public string serialno { get; set; }
        public string description { get; set; }
        public string karat { get; set; }
        public double weight { get; set; }
        public double carat { get; set; }
        public double all_price { get; set; }

    }
    public class ASYS_SelfInventory_rpt
    {
        public string ASITransNo { get; set; }
        public string ASIptn { get; set; }
        public string ASIallbarcode { get; set; }
        public string ASIDate { get; set; }
        public string ASIDescription { get; set; }
        public string ASISerialNo { get; set; }
        public int ASIQty { get; set; }
        public double ASIWeight { get; set; }
        public double ASIKarat { get; set; }
        public double ASICarat { get; set; }
        public double ASILoanValue { get; set; }
        public double ASISellingPrice { get; set; }
        public int ASIItemCounter { get; set; }
        public string ASICostCenter { get; set; }
        public string ASIResource { get; set; }
        public string DatePrinted { get; set; }
    }
    public class procASYS_MissingItems
    {
        public string ifNull(object data)
        {
            if (Convert.IsDBNull(data))
            {
                return "0";
            }
            else
            {
                return data.ToString();
            }
        }
        public string ASITransNo { get; set; }
        public string Expr1001 { get; set; }
        public string PTN { get; set; }
        public string ALLBARCODE { get; set; }
        public string ALL_DESC { get; set; }
        public int QTY { get; set; }
        public double ALL_WEIGHT { get; set; }
        public string ALL_KARAT { get; set; }
        public double ALL_CARAT { get; set; }
        public string SERIALNO { get; set; }
        public double LOANVALUE { get; set; }
        public double ALL_PRICE { get; set; }
        public string RESOURCE { get; set; }
        public string UniqueTrigger { get; set; }
        public string DatePrinted { get; set; }

    }
    public class ASYS_REMJewelryOutsource
    {
        public string TYPE { get; set; }
        public string SupplierCode { get; set; }
        public string ALLbarcode { get; set; }
        public string ALL_desc { get; set; }
        public int Qty { get; set; }
        public string KaratGrading { get; set; }
        public double Weight { get; set; }
        public double CaratSize { get; set; }
        public double ALL_Cost { get; set; }
        public string itemcode { get; set; }
    }
    public class ASYS_REMJewelryOutsourceAll
    {
        public string Status { get; set; }
        public string actionclass { get; set; }
        public string TYPE { get; set; }
        public string SupplierCode { get; set; }
        public string ALLbarcode { get; set; }
        public string ALL_desc { get; set; }
        public int Qty { get; set; }
        public string KaratGrading { get; set; }
        public double Weight { get; set; }
        public double CaratSize { get; set; }
        public string SerialNo { get; set; }
        public double ALL_Cost { get; set; }
        public string Itemcode { get; set; }
    }
    public class ASYS_REMJewelryReturns
    {
        public string TYPE { get; set; }
        public string ALLBarcode { get; set; }
        public string SupplierCode { get; set; }
        public string ALL_desc { get; set; }
        public int Qty { get; set; }
        public string KaratGrading { get; set; }
        public double Weight { get; set; }
        public double CaratSize { get; set; }
        public double ALL_Cost { get; set; }
        public string Itemcode { get; set; }


    }
    public class ASYS_REMJewelryReturnsAll
    {
        public string ALLbarcode { get; set; }
        public string SupplierCode { get; set; }
        public string ALL_desc { get; set; }
        public int Qty { get; set; }
        public string price_karat { get; set; }
        public double price_weight { get; set; }
        public double price_carat { get; set; }
        public string SerialNo { get; set; }
        public double cost { get; set; }
        public string Itemcode { get; set; }


    }
    public class ASYS_TradeInInventory
    {
        public string Division { get; set; }
        public string Divisionname { get; set; }
        public string Transaction_No { get; set; }
        public double Appraisal_Amount { get; set; }
        public string Reflotno { get; set; }
        public string Itemcode { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Karat { get; set; }
        public double Carat { get; set; }
        public double Weight { get; set; }
        public string ReceiveDate { get; set; }
        public string Receiver { get; set; }
        public string ReleaseDate { get; set; }
        public string Releaser { get; set; }
        public string Status { get; set; }

    }
    public class ASYS_spREMInventory
    {
        public string branchcode { get; set; }
        public string branchname { get; set; }
        public string refallbarcode { get; set; }
        public string ptn { get; set; }
        public double loanvalue { get; set; }
        public string refitemcode { get; set; }
        public string all_desc { get; set; }
        public string karatgrading { get; set; }
        public double caratsize { get; set; }
        public string SerialNo { get; set; }
        public int Qty { get; set; }
        public double weight { get; set; }
        public string actionclass { get; set; }
        public string sortcode { get; set; }
        public string sortname { get; set; }
        public string sortername { get; set; }
        public string sortdate { get; set; }
        public double cost { get; set; }


    }
    public class ASYS_InventoryALLConsign
    {
        public string consignee { get; set; }
        public string consigncode { get; set; }
        public string consignname { get; set; }
        public string consigndate { get; set; }
        public string allbarcode { get; set; }
        public int qty { get; set; }
        public string description { get; set; }
        public double grams { get; set; }
        public string karat { get; set; }
        public double carat { get; set; }
        public string serialno { get; set; }
        public double price { get; set; }

    }
    public class ASYS_ReportsInventoryUnconsignment
    {
        public int RefQty { get; set; }
        public string RefALLBarcode { get; set; }
        public string Price_desc { get; set; }
        public double Price_weight { get; set; }
        public string Price_karat { get; set; }
        public double Price_carat { get; set; }
        public string SerialNo { get; set; }
        public double ALL_price { get; set; }


    }
    public class ASYS_InventorybyCostCenter_V5S
    {
        public string Branchcode { get; set; }
        public string Branchname { get; set; }
        public string Costcenter { get; set; }
        public string PTN { get; set; }
        public string refallbarcode { get; set; }
        public string refitemcode { get; set; }
        public int refqty { get; set; }
        public string desc { get; set; }
        public string karat { get; set; }
        public double carat { get; set; }
        public string SerialNo { get; set; }
        public double weight { get; set; }
        public double cost { get; set; }
        public string date { get; set; }
        public double loanvalue { get; set; }
        public string transNo { get; set; }






    }
    public class ASYS_InventorybyItemcode
    {
        public string Branchcode { get; set; }
        public string Branchname { get; set; }
        public string Costcenter { get; set; }
        public string ptn { get; set; }
        public string refallbarcode { get; set; }
        public string refitemcode { get; set; }
        public int refqty { get; set; }
        public string desc { get; set; }
        public string karat { get; set; }
        public double carat { get; set; }
        public double weight { get; set; }
        public double cost { get; set; }
        public string date { get; set; }

    }
    public class ASYS_InventorybyItemsource
    {
        public string Branchcode { get; set; }
        public string Branchname { get; set; }
        public string Costcenter { get; set; }
        public string ptn { get; set; }
        public string refallbarcode { get; set; }
        public string itemsource { get; set; }
        public string refitemcode { get; set; }
        public int refqty { get; set; }
        public string desc { get; set; }
        public string karat { get; set; }
        public double carat { get; set; }
        public double weight { get; set; }
        public double cost { get; set; }
        public string date { get; set; }

    }
    public class ASYS_MLWBInventory
    {
        public string receiver { get; set; }
        public string employee { get; set; }
        public string receivedate { get; set; }
        public string reflotno { get; set; }
        public string lotno { get; set; }
        public string refallbarcode { get; set; }
        public string allbarcode { get; set; }
        public string ptn { get; set; }
        public int itemid { get; set; }
        public string ptnbarcode { get; set; }
        public string branchcode { get; set; }
        public string branchname { get; set; }
        public double loanvalue { get; set; }
        public string refitemcode { get; set; }
        public string branchitemdesc { get; set; }
        public int refqty { get; set; }
        public int qty { get; set; }
        public string karatgrading { get; set; }
        public double caratsize { get; set; }
        public double weight { get; set; }
        public string actionclass { get; set; }
        public string sortcode { get; set; }
        public string all_desc { get; set; }
        public string all_karat { get; set; }
        public double all_carat { get; set; }
        public string SerialNo { get; set; }
        public double all_cost { get; set; }
        public double all_weight { get; set; }
        public double appraisevalue { get; set; }
        public string currency { get; set; }
        public string photoname { get; set; }
        public string price_desc { get; set; }
        public string price_karat { get; set; }
        public double price_weight { get; set; }
        public double price_carat { get; set; }
        public double all_price { get; set; }
        public double cellular_cost { get; set; }
        public double watch_cost { get; set; }
        public double repair_cost { get; set; }
        public double cleaning_cost { get; set; }
        public double gold_cost { get; set; }
        public double mount_cost { get; set; }
        public double yg_cost { get; set; }
        public double wg_cost { get; set; }
        public string costdate { get; set; }
        public string costname { get; set; }
        public string releasedate { get; set; }
        public string releaser { get; set; }
        public string maturitydate { get; set; }
        public string expirydate { get; set; }
        public string status { get; set; }
        public string month { get; set; }
        public string year { get; set; }


    }
    public class ASYS_PRICINGInventory
    {
        public string receiver { get; set; }
        public string employee { get; set; }
        public string receivedate { get; set; }
        public string reflotno { get; set; }
        public string lotno { get; set; }
        public string refallbarcode { get; set; }
        public string allbarcode { get; set; }
        public string ptn { get; set; }
        public int itemid { get; set; }
        public string ptnbarcode { get; set; }
        public string branchcode { get; set; }
        public string branchname { get; set; }
        public double loanvalue { get; set; }
        public string refitemcode { get; set; }
        public string branchitemdesc { get; set; }
        public int refqty { get; set; }
        public int qty { get; set; }
        public string karatgrading { get; set; }
        public double caratsize { get; set; }
        public double weight { get; set; }
        public string actionclass { get; set; }
        public string sortcode { get; set; }
        public string all_desc { get; set; }
        public string all_karat { get; set; }
        public double all_carat { get; set; }
        public string SerialNo { get; set; }
        public double all_cost { get; set; }
        public double all_weight { get; set; }
        public double appraisevalue { get; set; }
        public string currency { get; set; }
        public string photoname { get; set; }
        public string price_desc { get; set; }
        public string price_karat { get; set; }
        public double price_weight { get; set; }
        public double price_carat { get; set; }
        public double all_price { get; set; }
        public double cellular_cost { get; set; }
        public double watch_cost { get; set; }
        public double repair_cost { get; set; }
        public double cleaning_cost { get; set; }
        public double gold_cost { get; set; }
        public double mount_cost { get; set; }
        public double yg_cost { get; set; }
        public double wg_cost { get; set; }
        public string costdate { get; set; }
        public string costname { get; set; }
        public string releasedate { get; set; }
        public string releaser { get; set; }
        public string maturitydate { get; set; }
        public string expirydate { get; set; }
        public string status { get; set; }
        public string month { get; set; }
        public string year { get; set; }
    }
    public class ASYS_DISTRIInventory
    {
        public string receiver { get; set; }
        public string employee { get; set; }
        public string receivedate { get; set; }
        public string lotno { get; set; }
        public string allbarcode { get; set; }
        public string division { get; set; }
        public string divisionname { get; set; }
        public string itemcode { get; set; }
        public int quantity { get; set; }
        public string actionclass { get; set; }
        public string sortcode { get; set; }
        public string currency { get; set; }
        public string pricedesc { get; set; }
        public string pricekarat { get; set; }
        public double priceweight { get; set; }
        public double pricecarat { get; set; }
        public string SerialNo { get; set; }
        public double allprice { get; set; }
        public string status { get; set; }
        public string month { get; set; }
        public string year { get; set; }

    }
  
}