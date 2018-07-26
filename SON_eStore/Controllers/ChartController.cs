using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SON_eStore.Models;
using System.Globalization;
using System.Data.Entity;
namespace SON_eStore.Controllers
{
    public class ChartController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Chart
        public ActionResult FRI_Chart()
        {

            var f_items = db.store_requisition.Where(s=>s.request_status =="Approved")
                .GroupBy(i => i.product_id).Select(s => new
                {
                    item_id = s.Key,
                    items = s.Select(c => new frequenttlyRequestedItem
                    {
                        // item_id = c.product_id,
                        item_name = c.product_name,
                        totalRequested = db.store_requisition.Where(p => p.product_id == c.product_id && p.request_status == "Approved").Sum(t => t.qty_allocated)
                    }).Distinct()
                }).ToList().Take(10);

            List<frequenttlyRequestedItem> fqItems = new List<frequenttlyRequestedItem>();
            foreach (var fitems in f_items)
            {
                //List<object> x = new List<object>();

                fqItems.AddRange(fitems.items.ToList());
            }
            if (fqItems.Count() > 0)
            { return View(fqItems.ToList()); }
            else {
                TempData["error"] = "Chart is empty, try again later.";
                return RedirectToAction("dashboard","estore"); }
            
        }

        public ActionResult ChartIndex(FormCollection f)
        {
            var sdt = f["startdate"];
            var edt = f["enddate"];
            ViewBag.sdt = sdt;
            ViewBag.edt = edt;
            if (!string.IsNullOrEmpty(sdt) && !string.IsNullOrEmpty(edt))
            {
                DateTime sdate = DateTime.ParseExact(sdt, "d-M-yyyy", CultureInfo.InvariantCulture);
                DateTime edate = DateTime.ParseExact(edt, "d-M-yyyy", CultureInfo.InvariantCulture);
                var f_items = db.store_requisition.Where(d => DbFunctions.TruncateTime(d.Approve_dt) >=DbFunctions.TruncateTime(sdate) && DbFunctions.TruncateTime(d.Approve_dt) <=DbFunctions.TruncateTime(edate) && d.request_status == "Approved")
              .GroupBy(i => i.product_id).Select(s => new
              {
                  item_id = s.Key,
                  items = s.Select(c => new frequenttlyRequestedItem
                  {
                      // item_id = c.product_id,
                      item_name = c.product_name,
                      totalRequested = db.store_requisition.Where(p => p.product_id == c.product_id && DbFunctions.TruncateTime(p.Approve_dt) >=DbFunctions.TruncateTime(sdate) && DbFunctions.TruncateTime(p.Approve_dt) <=DbFunctions.TruncateTime(edate) && p.request_status == "Approved").Sum(t => t.qty_allocated)
                  }).Distinct()
              }).ToList().Take(10);
              //  int cou = f_items.Count();
                List<frequenttlyRequestedItem> fqItems = new List<frequenttlyRequestedItem>();
                foreach (var fitems in f_items)
                {
                    //List<object> x = new List<object>();

                    fqItems.AddRange(fitems.items.ToList());
                }
                if (fqItems.Count() > 0)
                { return View(fqItems.ToList()); }
                else
                {
                    TempData["error"] = "Chart is empty, try again later.";
                    return RedirectToAction("dashboard", "estore");
                }
            
            }
            return View();
        }
    }
}