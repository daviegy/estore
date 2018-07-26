using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SON_eStore.Models
{
    public class category
    {
        [Key]
        public string id { get; set; }
        public string category_name { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<products> products { get; set; }
    }

    //[JsonObject(IsReference = true)]
    public class products
    {
        [Key]
        public string id { get; set; }
        public string cat_id { get; set; }
        public string product_name { get; set; }
        public string p_descripition { get; set; }
        public string serial_no { get; set; }
        //opening stock means opening balance or Actual qty available
        public int opening_stock_qty { get; set; }
        
        //current stock balance is use to manage requisitions and update
        // base on Actual stock - qty allocated pending approval
        public int current_stock_pending_approval {get;set;}

        public int total_item_allocated_pending_approval { get; set; }
        //this next line is use to manage products availability level
        //once products get this level store keeper is alert to make reorder request
        public int stock_reorder_alert_qty { get; set; }
        public string item_base_unit { get; set; }// this is the item unit of measurement
        //unit price of products
        public decimal? unitPrice { get; set; }
        [ForeignKey("cat_id")]
        public virtual category category { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Stock_In_Items> stock_in_items { get; set; }
    }
    //public class state
    //{
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int id { get; set; }
    //    public string state_name { get; set; }
    //}
    public class Department
    {
        [Key]
        public string id { get; set; }
        public string dept_name { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<dept_units> dept_units { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<ApplicationUser> users { get; set; }
    }
    public class dept_units
    {
        [Key]
        public string id { get; set; }
        public string dept_id { get; set; }
        public string unit_name { get; set; }
        [ForeignKey("dept_id")]
        public virtual Department department { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<ApplicationUser> users { get; set; }
    }
    public class suppliers
    {
        public string id { get; set; }
        public string supplier_name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string phone_no { get; set; }
        public string email { get; set; }
        public DateTime? reg_date { get; set; }
        //public string product_supplied { get; set; }
        //public int qty_supplied { get; set; }
        //// this stands for the flw
        ////store receipt voucher number                
        //public string S_R_V_No { get; set; }// this is gotten from manual form
        //public DateTime? supplied_dt { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Stock_In_Items> stock_in_items { get; set; }
    }
    public class Stock_In_Items
    { 
        
        public int id { get; set; }
        public string s_r_v_no { get; set; }
        public string supplier_id { get; set; }
        public string supplier_name { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public int qty_supplied { get; set; }
        public string qty_denomination { get; set; }
        public string item_base_unit { get; set; }
        public int qty_supplied_in_base_unit { get; set; }
        public decimal? unitPrice { get; set; }
        public decimal? Amount { get; set; }
        public decimal? totalAmount { get; set; }
        public DateTime? supplied_date { get; set; }
         [ForeignKey("supplier_id")]
        public virtual suppliers supplier { get; set; }
        // [ForeignKey("product_id")]
        //public virtual products product { get; set; }
         public string Recieved_by { get; set; }
        public DateTime? Created_date { get;  set; }
    }
    public class item_supplied_cart
    {
        public int id { get; set; }
        public string s_r_v_no { get; set; }
        public string supplier { get; set; }
        public string supplier_id { get; set; }
        public string item_id { get; set; }
        public string item_name { get; set; }       
        public int qtySupplied { get; set; }
        public string qty_denomination { get; set; }
        public int qty_supplied_in_base_unit { get; set; }
        public string item_base_unit { get; set; }
        public decimal unit_price { get; set; }
        public decimal total_amount_per_item { get; set; }
        public string supplied_date { get; set; }
    }
    public class store_requisitionTb
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
         public int R_ID { get; set; }
        // this stands for the flw
        //store requisiton voucher number
        //store issued voucher number  
        public string R_order_no { get; set; }//similar to s.r.v.no except that this is automatically generated
        public string S_R_V_No { get; set; } // this is gotten from manual form
        public string product_id { get; set; }
        public string product_name { get; set; }
        public int Item_Qty_In_Store_After_Approval { get; set; }
        public string Requested_qty_unit { get; set; }
        public int Requested_qty_unit_value { get; set; }
        public int conversion_value { get; set; }
        public string item_base_unit { get; set; }
        public int qty_requested { get; set; }
        public int qty_allocated { get; set; }
        public int qty_supplied { get; set; }
        //requesting staff information begins
        public string reqst_staff_id { get; set; }
        public string reqst_staff_name { get; set; }
        public string dept_id { get; set; }       
        public string department { get; set; }
        public string unit_id { get; set; }
        public string unit { get; set; }
        public string request_type { get; set; } //value= state or internal
        public string state_office { get; set; }
        public string isLabRequest { get; set; }//this is to show if the request is from state laboratory
        public string regional_office { get; set; }
        //requesting staff information end
        public DateTime? Request_dt { get; set; }//product/item request date
        public DateTime? Approve_dt { get; set; }// product/item approved date
        public DateTime? Recieved_dt { get; set; }//product/item recieved date
        public string request_status { get; set; }// processing  or completed
        public string created_by { get; set; }//store keeper name
        public string approve_by { get; set; }//store admin name
        public string received_by { get; set; }// name of the person receiving  the item, just incase the
                                                // is not the person who created the request that is receiving it.


        public DateTime? Created_Date { get; set; }
    }
    public class UsersActivitiesLog
    {
        public int id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string userRole { get; set; }
        public string operation { get; set; }
        public DateTime? date { get; set; }
    }
    public class cartViewModel
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string cart_id { get; set; }
        public string item_id { get; set; }
        public string item_name { get; set; }
        public string Requested_qty_unit { get; set; }
        public int Requested_qty_unit_value { get; set; }
        public int conversion_value { get; set; }
        public string item_base_unit { get; set; }
        public int Qty_Requested { get; set; }
        public int Qty_Allocated { get; set; }
        public string Request_type { get; set; }
        public string State { get; set; }
        //this is to show if the request is from state laboratory
        public string isLabRequest { get; set; }
        public string Region { get; set; }
        public string dept_id { get; set; }
        public string dept_name { get; set; }
        public string unit_id { get; set; }
        public string unit_name { get; set; }
        public string staff_id { get; set; }
        public string staff_name { get; set; }
        public string s_r_v_no { get; set; } //Store Requisition Number: manually generate number on store requsition invoice

        public string request_status { get; set; }
        public string requested_date { get; set; }
    }
    //public class borrowcartViewModel
    //{
    //    [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
    //    public int id { get; set; }
    //    public string cart_id { get; set; }
    //    public string item_id { get; set; }
    //    public string item_name { get; set; }
    //    public string Requested_qty_unit { get; set; }
    //    public int Requested_qty_unit_value { get; set; }
    //    public int conversion_value { get; set; }
    //    public string item_base_unit { get; set; }
    //    public int Qty_Requested { get; set; }
    //    public int Qty_Allocated { get; set; }
    //    public string Request_type { get; set; }
    //    public string State { get; set; }
    //    public string Region { get; set; }
    //    public string dept_id { get; set; }
    //    public string dept_name { get; set; }
    //    public string unit_id { get; set; }
    //    public string unit_name { get; set; }
    //    public string staff_id { get; set; }
    //    public string staff_name { get; set; }
    //    public string s_r_v_no { get; set; } //Store Requisition Number: manually generate number on store requsition invoice

    //    public string request_status { get; set; }
    //    public string requested_date { get; set; }
    //}
    public class conversionTable
    {
        public int id { get; set; }
        public string item_id { get; set; }
        public string item_name { get; set; }
        public string master_unit { get; set; }
        public string master_unit_value { get; set;}
        public string base_unit { get; set; }
        public string base_unit_value { get; set; }
    }
}