using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SON_eStore.Models;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using System.Globalization;
using System.Threading.Tasks;
using System.Data.Entity;
namespace SON_eStore.Controllers
{
    public class ExcelReportController : Controller
    {
        // GET: ExcelReport
        public ActionResult item_in_stock (){
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[4]{
                new DataColumn("Item ID"),
                new DataColumn("Item Name"),
                new DataColumn("Category"),
                new DataColumn("Quantity in Store")                
                });
                var data = db.product.Where(p => p.opening_stock_qty>0).ToList();
                //get data from db
                // stdate = !string.IsNullOrEmpty(f["startdate"])? Convert.ToDateTime(f["startdate"]): ;
                foreach (var item in data)
                {
                    dt.Rows.Add(item.id, item.product_name, item.category.category_name,
                        item.opening_stock_qty +" "+item.item_base_unit);
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "item_in_Stock" + DateTime.UtcNow.ToShortDateString() + ".xlsx");
                    }
                }
               
            }
        }
        public ActionResult item_out_of_stock()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[4]{
                new DataColumn("Item ID"),
                new DataColumn("Item Name"),
                new DataColumn("Category"),
                new DataColumn("Quantity in Store")                
                });
                var data = db.product.Where(p => p.opening_stock_qty > 0).ToList();
                //get data from db
                // stdate = !string.IsNullOrEmpty(f["startdate"])? Convert.ToDateTime(f["startdate"]): ;



                foreach (var item in data)
                {
                    dt.Rows.Add(item.id, item.product_name, item.category.category_name,
                        item.opening_stock_qty + " " + item.item_base_unit);
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "item_out_of_stock" + DateTime.UtcNow.ToShortDateString() + ".xlsx");
                    }
                }

            }
        }
        public ActionResult Running_out_of_stock()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[4]{
                new DataColumn("Item ID"),
                new DataColumn("Item Name"),
                new DataColumn("Category"),
                new DataColumn("Quantity in Store")                
                });
                var data = db.product.Where(p => p.opening_stock_qty <= p.stock_reorder_alert_qty && p.opening_stock_qty>0).ToList();
                //get data from db
                // stdate = !string.IsNullOrEmpty(f["startdate"])? Convert.ToDateTime(f["startdate"]): ;
                foreach (var item in data)
                {
                    dt.Rows.Add(item.id, item.product_name, item.category.category_name,
                        item.opening_stock_qty + " " + item.item_base_unit);
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Running_out_of_stock_" + DateTime.UtcNow.ToShortDateString() + ".xlsx");
                    }
                }

            }
        }
        public ActionResult item_supplies_rpt(FormCollection f)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[9]{
                new DataColumn("STOCK ID"),
                new DataColumn("ITEM NAME"),
                new DataColumn("QTY. SUPPLIED"),
                new DataColumn("QTY. SUPPLIED (in base unit)"),
                new DataColumn("UNIT PRICE"),
                new DataColumn("AMOUNT"),
                new DataColumn("SUPPLIER"),   
                new DataColumn("SUPPLIED DATE"),
                new DataColumn("CREATED DATE"),
                });
                var data = db.stock_in_items.ToList(); 

                var sdt = f["startdate"];
                    var edt = f["enddate"];                   
                  string supplier = f["supplier"];
                //get data from db
                // stdate = !string.IsNullOrEmpty(f["startdate"])? Convert.ToDateTime(f["startdate"]): ;
                if (!string.IsNullOrEmpty(sdt) && !string.IsNullOrEmpty(edt))
                {
                    DateTime sdate = DateTime.ParseExact(sdt, "d-M-yyyy", CultureInfo.InvariantCulture);
                    DateTime edate =  DateTime.ParseExact(edt,"d-M-yyyy", CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(supplier))
                    { 
                        data = db.stock_in_items.Where(d => d.supplier_id == supplier && d.supplied_date >= sdate && d.supplied_date <= edate).ToList();
                    }else
                    {
                        data = db.stock_in_items.Where(d => d.supplied_date >= sdate && d.supplied_date <= edate).ToList();
                    }
                  
                }
                if (!string.IsNullOrEmpty(supplier))
                {                   
                    
                    if((!string.IsNullOrEmpty(sdt) && !string.IsNullOrEmpty(edt))){
                          DateTime sdate = DateTime.ParseExact(sdt, "d-M-yyyy", CultureInfo.InvariantCulture);
                    DateTime edate =  DateTime.ParseExact(edt,"d-M-yyyy", CultureInfo.InvariantCulture);
                          data = db.stock_in_items.Where(d => d.supplier_id == supplier && d.supplied_date >= sdate && d.supplied_date <= edate).ToList();
                    }
                    else{
                         data = db.stock_in_items.Where(d => d.supplier_id == supplier).ToList();
                    }
                   
                }

                foreach (var item in data)
                {
                    dt.Rows.Add(item.id, item.product_name, item.qty_supplied +" "+ item.qty_denomination, item.qty_supplied_in_base_unit + " "+ item.item_base_unit,
                      item.unitPrice, item.Amount,  item.supplier_name, Convert.ToDateTime(item.supplied_date).ToString("dd MMM,yyy"), Convert.ToDateTime(item.Created_date).ToString("dd MMM,yyy"));
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "item_supplies_rpt_" + DateTime.UtcNow.ToShortDateString() + ".xlsx");
                    }
                }
                
            }
        }

        public ActionResult Requested_iTem_RPT(FormCollection f)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[11]{
                new DataColumn("ORDER ID"),
                new DataColumn("ITEM NAME"),
                new DataColumn("REQUEST FROM"),
                new DataColumn("UNIT"),
                new DataColumn("QTY. REQUESTED"),
                  //new DataColumn("QUANTITY REQUESTED (in base unit)"),
                new DataColumn("QTY. APPROVED (in base unit)"),
                new DataColumn("QTY. AVAILABLE (in store)"),
                new DataColumn("REQUESTED DATE"),   
                new DataColumn("APPROVED DATE"),
                new DataColumn("REQUESTED BY"),
                new DataColumn("APPROVED BY")
                });
                var data = db.store_requisition.Where(s=>s.request_status.ToLower()=="approved".ToLower()).ToList().OrderBy(o=>o.R_order_no);

                var sdt = f["startdate"];
                var edt = f["enddate"];
                string dept = f["dept"];
                string item = f["item"];
                string state = f["state"];
                string region = f["region"];
                string requestType = f["requestType"];
                var deptname = (!string.IsNullOrEmpty(dept))?db.department.Find(dept).dept_name:null;
                var itemname = (!string.IsNullOrEmpty(item))?db.product.Find(item).product_name:null;
                ViewBag.item = "false";
                //get data from db
                // stdate = !string.IsNullOrEmpty(f["startdate"])? Convert.ToDateTime(f["startdate"]): ;
                if (!string.IsNullOrEmpty(sdt) && !string.IsNullOrEmpty(edt))
                {
                    DateTime sdate = DateTime.ParseExact(sdt, "d-M-yyyy", CultureInfo.InvariantCulture);
                    DateTime edate = DateTime.ParseExact(edt, "d-M-yyyy", CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(dept))
                    {
                        data = db.store_requisition.Where(d => d.dept_id == dept && DbFunctions.TruncateTime(d.Approve_dt) >= sdate && DbFunctions.TruncateTime(d.Approve_dt) <= edate && d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o=>o.Approve_dt);
                    }
                    else if(!string.IsNullOrEmpty(state))
                    {
                           var stateoffice = state;
                           data = db.store_requisition.Where(d => d.state_office == stateoffice && DbFunctions.TruncateTime(d.Approve_dt) >= sdate && DbFunctions.TruncateTime(d.Approve_dt) <= edate && d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o => o.Approve_dt);
                    }
                    else if (!string.IsNullOrEmpty(item))
                    {
                        ViewBag.item = "true";
                        data = db.store_requisition.Where(d => d.product_id == item && DbFunctions.TruncateTime(d.Approve_dt) >= sdate && DbFunctions.TruncateTime(d.Approve_dt) <= edate && d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o => o.Approve_dt).ThenByDescending(o=>o.Item_Qty_In_Store_After_Approval);
                    }
                    else if (!string.IsNullOrEmpty(region))
                    {
                        data = db.store_requisition.Where(d => DbFunctions.TruncateTime(d.Approve_dt) >= sdate && DbFunctions.TruncateTime(d.Approve_dt) <= edate && d.request_status.ToLower() == "approved".ToLower() && d.regional_office == region).ToList().OrderBy(o => o.Approve_dt);
                        return View(data);
                    }
                    else if (!string.IsNullOrEmpty(requestType))
                    {
                        data = db.store_requisition.Where(d => DbFunctions.TruncateTime(d.Approve_dt) >= sdate && DbFunctions.TruncateTime(d.Approve_dt) <= edate && d.request_status.ToLower() == "approved".ToLower() && d.request_type == requestType).ToList().OrderBy(o => o.Approve_dt);
                        return View(data);
                    }
                    else
                    {
                        data = db.store_requisition.Where(d => DbFunctions.TruncateTime(d.Approve_dt) >= sdate && DbFunctions.TruncateTime(d.Approve_dt) <= edate && d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o => o.Approve_dt);
                    }

                }
                else if (!string.IsNullOrEmpty(dept))
                {
                    data = db.store_requisition.Where(d => d.dept_id == dept  && d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o => o.Approve_dt);
                }
                else if (!string.IsNullOrEmpty(state))
                {
                    var stateoffice = state;

                    data = db.store_requisition.Where(d => d.state_office == stateoffice && d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o => o.Approve_dt);
                   

                }
                else if (!string.IsNullOrEmpty(item))
                {
                    ViewBag.item = "true";
                    data = db.store_requisition.Where(d => d.product_id == item && d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o => o.Approve_dt).ThenByDescending(o=>o.Item_Qty_In_Store_After_Approval);
                    
                }
                else if (!string.IsNullOrEmpty(requestType))
                {
                    data = db.store_requisition.Where(d => d.request_status.ToLower() == "approved".ToLower() && d.request_type == requestType).ToList().OrderBy(o => o.Approve_dt);
                   
                }
                else if (!string.IsNullOrEmpty(region))
                {
                    data = db.store_requisition.Where(d => d.request_status.ToLower() == "approved".ToLower() && d.regional_office == region).ToList().OrderBy(o => o.R_order_no);
                   
                }

                if (!string.IsNullOrEmpty(state))
                {
                    foreach (var items in data)
                    {
                        dt.Rows.Add(items.S_R_V_No, items.product_name, items.state_office,items.unit,items.Requested_qty_unit_value +" "+ items.Requested_qty_unit, items.qty_allocated + " " + items.item_base_unit, items.Item_Qty_In_Store_After_Approval +" "+items.item_base_unit,
                         Convert.ToDateTime(items.Request_dt).ToString("dd MMM,yyy"),  Convert.ToDateTime(items.Approve_dt).ToString("dd MMM,yyy"),
                         items.reqst_staff_name,items.approve_by);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Item_Request_Report_for_" + state + "_office_" + DateTime.UtcNow.ToString("dd-MMM-yyy") + ".xlsx");
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(dept))
                {
                    foreach (var items in data)
                    {
                        dt.Rows.Add(items.S_R_V_No, items.product_name, items.department, items.unit, items.Requested_qty_unit_value + " " + items.Requested_qty_unit,  items.qty_allocated + " " + items.item_base_unit, items.Item_Qty_In_Store_After_Approval + " " + items.item_base_unit,
                         Convert.ToDateTime(items.Request_dt).ToString("dd MMM,yyy"), Convert.ToDateTime(items.Approve_dt).ToString("dd MMM,yyy"),
                         items.reqst_staff_name, items.approve_by);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Item_Request_Report_for_"+deptname+"_" + DateTime.UtcNow.ToString("dd-MMM-yyy") + ".xlsx");
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(item))
                {
                    foreach (var items in data)
                    {
                        dt.Rows.Add(items.S_R_V_No, items.product_name, items.department + items.state_office + items.regional_office, items.unit, items.Requested_qty_unit_value + " " + items.Requested_qty_unit,  items.qty_allocated + " " + items.item_base_unit, items.Item_Qty_In_Store_After_Approval + " " + items.item_base_unit,
                         Convert.ToDateTime(items.Request_dt).ToString("dd MMM,yyy"), Convert.ToDateTime(items.Approve_dt).ToString("dd MMM,yyy"),
                         items.reqst_staff_name, items.approve_by);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Item_Request_Report_for_" + itemname + "_" + DateTime.UtcNow.ToString("dd-MMM-yyy") + ".xlsx");
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(region))
                {
                    foreach (var items in data)
                    {
                        dt.Rows.Add(items.S_R_V_No, items.product_name, items.regional_office, items.unit, items.Requested_qty_unit_value + " " + items.Requested_qty_unit,  items.qty_allocated + " " + items.item_base_unit, items.Item_Qty_In_Store_After_Approval + " " + items.item_base_unit,
                         Convert.ToDateTime(items.Request_dt).ToString("dd MMM,yyy"), Convert.ToDateTime(items.Approve_dt).ToString("dd MMM,yyy"),
                         items.reqst_staff_name, items.approve_by);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Item_Request_Report_for_" + region + "_" + DateTime.UtcNow.ToString("dd-MMM-yyy") + ".xlsx");
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(requestType))
                {
                    foreach (var items in data)
                    {
                        dt.Rows.Add(items.S_R_V_No, items.product_name, items.department + items.state_office + items.regional_office, items.unit, items.Requested_qty_unit_value + " " + items.Requested_qty_unit,  items.qty_allocated + " " + items.item_base_unit, items.Item_Qty_In_Store_After_Approval + " " + items.item_base_unit,
                         Convert.ToDateTime(items.Request_dt).ToString("dd MMM,yyy"), Convert.ToDateTime(items.Approve_dt).ToString("dd MMM,yyy"),
                         items.reqst_staff_name, items.approve_by);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Item_Request_Report_for_" + requestType + "_" + DateTime.UtcNow.ToString("dd-MMM-yyy") + ".xlsx");
                        }
                    }
                }
                else
                {
                    foreach (var items in data)
                    {
                        dt.Rows.Add(items.S_R_V_No, items.product_name, items.department + items.state_office +items.regional_office, items.unit, items.Requested_qty_unit_value + " " + items.Requested_qty_unit,  items.qty_allocated + " " + items.item_base_unit, items.Item_Qty_In_Store_After_Approval + " " + items.item_base_unit,
                         Convert.ToDateTime(items.Request_dt).ToString("dd MMM,yyy"), Convert.ToDateTime(items.Approve_dt).ToString("dd MMM,yyy"),
                         items.reqst_staff_name, items.approve_by);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Item_Request_Report_" + DateTime.UtcNow.ToString("dd-MMM-yyy") + ".xlsx");
                        }
                    }
                }
                
                

            }
        }

        public ActionResult activity_log(FormCollection f)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[3]{
                new DataColumn("NAME"),
                new DataColumn("OPERATION"),
                new DataColumn("DATE"),                  
                });
                var data = db.usersActivitiesLog.ToList();

                var sdt = f["startdate"];
                var edt = f["enddate"];
                string ddlusers = f["ddlusers"];
                var user = (!string.IsNullOrEmpty(ddlusers))?db.Users.Find(ddlusers):null;
                //get data from db
                // stdate = !string.IsNullOrEmpty(f["startdate"])? Convert.ToDateTime(f["startdate"]): ;
                if (!string.IsNullOrEmpty(sdt) && !string.IsNullOrEmpty(edt))
                {
                    DateTime sdate = DateTime.ParseExact(sdt, "d-M-yyyy", CultureInfo.InvariantCulture);
                    DateTime edate = DateTime.ParseExact(edt, "d-M-yyyy", CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(ddlusers))
                    {
                        data = db.usersActivitiesLog.Where(d => d.username == user.UserName && DbFunctions.TruncateTime(d.date) >= sdate && DbFunctions.TruncateTime(d.date) <= edate).ToList();
                    }
                    else
                    {
                        data = db.usersActivitiesLog.Where(d=>DbFunctions.TruncateTime(d.date) >= sdate && DbFunctions.TruncateTime(d.date) <= edate).ToList();
                    }

                }
                if (!string.IsNullOrEmpty(ddlusers))
                {                 
                        data = db.usersActivitiesLog.Where(d => d.username == user.UserName).ToList();
                }

                foreach (var item in data)
                {
                    dt.Rows.Add(item.name, item.operation,
                         Convert.ToDateTime(item.date).ToString("dd MMM,yyy"));
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "activity_log_" + DateTime.UtcNow.ToShortDateString() + ".xlsx");
                    }
                }

            }
        }
    }
}