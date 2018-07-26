using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SON_eStore.Models
{
    public class logedInUser
    {
        public string name { get; set; }
        public string rolename { get; set; }
        public string reg_date { get; set; }
    }
    public class itemsViewModel
    {
        public string id { get; set; }
        public string product_name { get; set; }
        public string category_name { get; set; }
        public string item_base_unit { get; set; }
        public int qtyAvailable { get; set; }
        public string desc { get; set; }

        public string serial_no { get; set; }

        public int qtyReorderAlertValue { get; set; }

        public decimal? unitPrice { get; set; }
        public string is_Item_In_Store { get;set; }
        public string catid { get; set; }
    }
    public class deptviewModel
    {
        public string id { get; set; }
        public string dept_name { get; set; }
        public int reg_unit_size { get; set; }
        public int reg_staff_size { get; set; }
    }
    public class UnitsviewModel
    {
        public string id { get; set; }
        public string unit_name { get; set; }
        public string dept_id { get; set; }
        public string dept_name { get; set; }
    }
    public class StaffViewModel
    {
        public string id { get; set; }
        public string staff_id { get; set; }
        public string Name { get;set;}
        public string staffType { get; set; }
        public string stateOffice { get; set; }
        public string regionalOffice { get; set; }
        public string dept_name { get; set; }
        public string unit_name { get; set; }
        public string dept_id { get; set; }
        public string unit_id {get;set;}
        public string phone { get; set; }
        public string email { get; set; }
        public string Fname { get; set; }        
        public string Lname { get;set ;}
        public string role_id { get; set; }
        public string rolename { get; set; }
        public string username { get; set; }

        public string rank { get; set; }
    }
    public class storeRequestViewModel {
        public int Requested_qty_unit_value { get; set; }

        public string s_r_v_no { get; set; }

        public string rq_orderno { get; set; }
        public string item_name { get; set; }
        public int qty_requested { get; set; }
        public int qty_allocated { get; set; }
        public int qty_suppled { get; set; }
        public string Rq_staff_name { get; set; }
        public string dept_name { get; set; }
        public string unit_name { get; set; }
        public string state_office { get; set; }
        public string request_type { get; set; }
        public string request_status { get; set; }
        public string created_by { get; set; } // name of the store keeper who created the request
        public string approved_by { get; set; }//name of the store admin who approved request.
        public DateTime? request_date { get; set; }
        public DateTime? approved_date { get; set; }
        public DateTime? created_date { get; set; }
        public int qty_available { get; set; }
        public int rid { get; set; }
        public string item_id { get; set; }
        public string Requested_qty_unit { get; set; }
        public string item_base_unit { get; set; }
        public string srv { get;  set; }
    }
    public class storeRequestListByORDERID_vm {
        public string orderid { get; set; }
        public string request_type { get; set; }
        public string requesting_state { get; set; }
        public string requesting_dept { get; set; }
        public string requesting_region { get; set; }
        public string unit { get; set; }
       // public string requesting_staff { get; set; }
        public string request_status { get; set; }
        public int total_item_requested { get; set; }

        public DateTime? approved_date { get; set; }

        public DateTime? request_date { get; set; }

        public DateTime? created_date { get; set; }

        public string reqst_staff_name { get; set; }
        public string srv { get; set; }
    }
    public class frequenttlyRequestedItem
    {
      //  public string item_id { get; set; }
        public string item_name { get; set; }
        public int totalRequested { get; set; }       
    }
    //Special  class for grouping
    public class Group<T, K>
    {
        public K key;
        public IEnumerable<T> values;
    }
    public class state
    {
        public int id { get; set; }
        public string name { get; set; }      
        
    }
    public class Region
    {
        public string id { get; set; }
        public string name { get; set; }

    }
    public class RequestType
    {
        public string id { get; set; }
        public string name { get; set; }

    }
    public class storeSuppliesViewModel
    {
        public string srv { get; set; }
      
        public string supplier_name { get; set; }
       
        public string product_name { get; set; }
        public int qty_supplied { get; set; }
        public string qty_denomination { get; set; }
        public string item_base_unit { get; set; }
        public int qty_supplied_in_base_unit { get; set; }
        public decimal? unitPrice { get; set; }
        public decimal? Amount { get; set; }
        public decimal? totalAmount { get; set; }
        public DateTime? supplied_date { get; set; }
        public DateTime? Created_date { get; set; }
        public int total_item_supplied { get; set; }
    }


}