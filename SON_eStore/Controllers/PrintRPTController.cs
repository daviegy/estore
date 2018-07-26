using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SON_eStore.Models;
using System.Globalization;
using System.Data.Entity;
using System.Web.Http.Cors;

namespace SON_eStore.Controllers
{
    [EnableCors("*", "*", "*")]
    [Authorize(Roles = "StoreAdmin,StoreKeeper")]
    public class PrintRPTController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: PrintRPT
        public ActionResult Print_item_in_stock()
        {
            var items = db.product.Where(p => p.opening_stock_qty > 0);
            ViewBag.itemdata = items.ToList();
            return View(items.Select(p => new itemsViewModel
            {
                id = p.id,
                category_name = p.category.category_name,
                qtyAvailable = p.opening_stock_qty,
                product_name = p.product_name,
                item_base_unit = p.item_base_unit
            }).ToList());
          //  return View();
        }
        public ActionResult Print_item_Out_stock()
        {
            var items = db.product.Where(p => p.opening_stock_qty <= 0);
            ViewBag.itemdata = items.ToList();
            return View(items.Select(p => new itemsViewModel
            {
                id = p.id,
                category_name = p.category.category_name,
                qtyAvailable = p.opening_stock_qty,
                product_name = p.product_name
            }).ToList());
            //  return View();
        }
        public ActionResult Print_Runing_Out_stock()
        {
            var items = db.product.Where(p => p.opening_stock_qty <= p.stock_reorder_alert_qty && p.opening_stock_qty>0);
            ViewBag.itemdata = items.ToList();
            return View(items.Select(p => new itemsViewModel
            {
                id = p.id,
                category_name = p.category.category_name,
                qtyAvailable = p.opening_stock_qty,
                product_name = p.product_name
            }).ToList());
            //  return View();
        }
        public ActionResult Print_iTems_supplies(FormCollection f)
        {
            var data = db.stock_in_items.ToList();
            var sdt = f["startdate"];
            var edt = f["enddate"];
            ViewBag.sdt = sdt;
            ViewBag.edt = edt;
            string supplier = f["supplier"];           
            //get data from db
            // stdate = !string.IsNullOrEmpty(f["startdate"])? Convert.ToDateTime(f["startdate"]): ;
            if (!string.IsNullOrEmpty(sdt) && !string.IsNullOrEmpty(edt))
            {
                DateTime sdate = DateTime.ParseExact(sdt, "d-M-yyyy", CultureInfo.InvariantCulture);
                DateTime edate = DateTime.ParseExact(edt, "d-M-yyyy", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(supplier))
                {
                    var sp = (!string.IsNullOrEmpty(supplier)) ? db.supplier.Find(supplier) : null;
                    ViewBag.spname = (!string.IsNullOrEmpty(sp.supplier_name)) ? sp.supplier_name : null;
                    ViewBag.addr = (!string.IsNullOrEmpty(sp.address)) ? sp.address : null;
                    ViewBag.email = (!string.IsNullOrEmpty(sp.email)) ? sp.email : null;
                    ViewBag.phone = (!string.IsNullOrEmpty(sp.phone_no)) ? sp.phone_no : null;
                    data = db.stock_in_items.Where(d => d.supplier_id == supplier && d.supplied_date >= sdate && d.supplied_date <= edate).ToList();
                    return View(data);
                }
                else
                {
                    data = db.stock_in_items.Where(d => d.supplied_date >= sdate && d.supplied_date <= edate).ToList();
                    return View(data);
                }

            }
            if (!string.IsNullOrEmpty(supplier))
            {
                var sp = (!string.IsNullOrEmpty(supplier)) ? db.supplier.Find(supplier) : null;
                ViewBag.spname = (!string.IsNullOrEmpty(sp.supplier_name)) ? sp.supplier_name : null;
                ViewBag.addr = (!string.IsNullOrEmpty(sp.address)) ? sp.address : null;
                ViewBag.email = (!string.IsNullOrEmpty(sp.email)) ? sp.email : null;
                ViewBag.phone = (!string.IsNullOrEmpty(sp.phone_no)) ? sp.phone_no : null;               
                data = db.stock_in_items.Where(d => d.supplier_id == supplier).ToList();
                return View(data);
            }
           // TempData["success"] = "i am here";
            return View(data);
        }
        public ActionResult Print_Request_Item_Rpt(FormCollection f)
        {
            var data = db.store_requisition.Where(d=> d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o => o.Approve_dt );
            var sdt = f["startdate"];
            var edt = f["enddate"];
            string dept = f["dept"];
            string item = f["item"];
            string state = f["state"];
            string region = f["region"];
            string requestType = f["requestType"];
            var deptname = (!string.IsNullOrEmpty(dept)) ? db.department.Find(dept).dept_name : null;
            var itemname = (!string.IsNullOrEmpty(item)) ? db.product.Find(item).product_name : null;
            ViewBag.deptname = deptname;
            ViewBag.itemname = itemname;
            ViewBag.state = state;
            ViewBag.sdt = sdt;
            ViewBag.edt = edt;
            ViewBag.item = "false";
            //get data from db
            // stdate = !string.IsNullOrEmpty(f["startdate"])? Convert.ToDateTime(f["startdate"]): ;
            if (!string.IsNullOrEmpty(sdt) && !string.IsNullOrEmpty(edt))
            {
                DateTime sdate = DateTime.ParseExact(sdt, "d-M-yyyy", CultureInfo.InvariantCulture);
                DateTime edate = DateTime.ParseExact(edt, "d-M-yyyy", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(dept))
                {
                    data = db.store_requisition.Where(d => d.dept_id == dept && DbFunctions.TruncateTime(d.Approve_dt) >= sdate && DbFunctions.TruncateTime(d.Approve_dt) <= edate && d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o => o.Approve_dt);
                    return View(data);

                }
                else if (!string.IsNullOrEmpty(state))
                {
                    var stateoffice = state;
                    data = db.store_requisition.Where(d => d.state_office == stateoffice && DbFunctions.TruncateTime(d.Approve_dt) >= sdate && DbFunctions.TruncateTime(d.Approve_dt) <= edate && d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o => o.Approve_dt);
                    return View(data);

                }
                else if (!string.IsNullOrEmpty(item))
                {
                    ViewBag.item = "true";
                    data = db.store_requisition.Where(d => d.product_id == item && DbFunctions.TruncateTime(d.Approve_dt) >= sdate && DbFunctions.TruncateTime(d.Approve_dt) <= edate && d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o => o.Approve_dt).ThenByDescending(o => o.Item_Qty_In_Store_After_Approval);
                    return View(data);

                }
                else if(!string.IsNullOrEmpty(region))
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
                    return View(data);
                }

            }
            else if (!string.IsNullOrEmpty(dept))
            {
                data = db.store_requisition.Where(d => d.dept_id == dept && d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o => o.Approve_dt);
                return View(data);
            }
            else if (!string.IsNullOrEmpty(state))
            {
                var stateoffice = state;

                data = db.store_requisition.Where(d => d.state_office == stateoffice && d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o => o.Approve_dt);
                return View(data);

            }
            else if (!string.IsNullOrEmpty(item))
            {
                ViewBag.item = "true";
                data = db.store_requisition.Where(d => d.product_id == item && d.request_status.ToLower() == "approved".ToLower()).ToList().OrderBy(o => o.Approve_dt).ThenByDescending(o=>o.Item_Qty_In_Store_After_Approval);
                return View(data);
            }
            else if (!string.IsNullOrEmpty(requestType))
            {
                data = db.store_requisition.Where(d => d.request_status.ToLower() == "approved".ToLower() && d.request_type == requestType).ToList().OrderBy(o => o.Approve_dt);
                return View(data);
            }
            else if (!string.IsNullOrEmpty(region))
            {
                data = db.store_requisition.Where(d => d.request_status.ToLower() == "approved".ToLower() && d.regional_office == region).ToList().OrderBy(o => o.Approve_dt);
                return View(data);
            }
            return View(data);
        }

        public ActionResult Print_Fresh_Items_Supplied_by_srv(string srv)
        {
            var data = db.stock_in_items.Where(d => d.s_r_v_no == srv).ToList();
            foreach (var item in data)
            {
                ViewBag.spid = item.supplier_id;
                ViewBag.srv = item.s_r_v_no;
                ViewBag.spdt = item.supplied_date;
                ViewBag.crdt = item.Created_date;
                ViewBag.Tamount += item.Amount;
            }
            return View (data);
        }
    }
}