using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using SON_eStore.Models;
using System.Net;
using SON_eStore.Customclasses;
using System.Web.Routing;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
namespace SON_eStore.Controllers
{

    public class estoreController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        UserslogActivities ulog = new UserslogActivities();
        string dbName = System.Configuration.ConfigurationManager.AppSettings["DBName"]; 
        //public ActionResult Index()
        //{
        //    ViewBag.Title = "Home Page";
        //    return View();
        //}
        [AllowAnonymous]
        public ActionResult landingpage()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult login()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult Dashboard()
        {
            return View();
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult categoryList()
        {
            return View(db.category.ToList());
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult items_In_Store()
        {
            return View(db.product.ToList());
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult in_stock_items()
        {
            var items = db.product.Where(p => p.opening_stock_qty > 0);
            ViewBag.itemdata = items;
            return View(items.Select(p => new itemsViewModel
            {
                id = p.id,
                category_name = p.category.category_name,
                qtyAvailable = p.opening_stock_qty,
                product_name = p.product_name,
                item_base_unit = p.item_base_unit
            }).ToList());
        }
        public ActionResult ViewAllItemsInStore()
        {
            //var items = db.product.ToList();
            ////ViewBag.itemdata = items;
            //return View(items.Select(p => new itemsViewModel
            //{
            //    id = p.id,
            //    category_name = p.category.category_name,
            //    qtyAvailable = p.opening_stock_qty,
            //    product_name = p.product_name,
            //    item_base_unit = p.item_base_unit,
            //    desc = p.p_descripition

            //}).ToList());
            return View();
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult out_of_stock()
        {
            return View(db.product.Where(p => p.opening_stock_qty <= 0).Select(p => new itemsViewModel
            {
                id = p.id,
                category_name = p.category.category_name,
                qtyAvailable = p.opening_stock_qty,
                product_name = p.product_name,
                item_base_unit = p.item_base_unit
            }).ToList());
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult suppliers()
        {
            return View(db.supplier.ToList());
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult stockSupplies()
        {
            return View();
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult depts()
        {
            return View();
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult dept_units()
        {
            return View();
        }
        #region //Account Management for users
        [Authorize(Roles = "StoreAdmin")]
        public ActionResult CreateUser()
        {
            return View();
        }
        public ActionResult Reg_StoreStaff()
        {
            return View();
        }
        public ActionResult view_eStoreUsers()
        {
            return View();
        }
        [Authorize(Roles = "StoreAdmin")]
        public ActionResult roles()
        {
            return View();
        }
        [Authorize(Roles = "StoreAdmin")]
        public ActionResult viewUsers()
        {
            return View();
        }
        [Authorize]//this allow user to change password
        public ActionResult Change_password()
        {
            return View();
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult resetPassword()
        {
            return View();
        }
       
        [Authorize(Roles = "StoreAdmin")]
        [HttpPost]
        public JsonResult resetpassword(ChangePasswordBindingModel model)
        {
            if (model != null)
            {
                if(model.NewPassword == model.ConfirmPassword)
                {
                PasswordHasher pw = new PasswordHasher();
                var getUser = db.Users.FirstOrDefault(u => u.UserName == model.Username);
                getUser.PasswordHash = pw.HashPassword(model.ConfirmPassword);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("new password doesnot match confirm password", JsonRequestBehavior.AllowGet);
                }             
               
            }
            return Json(false,JsonRequestBehavior.AllowGet);
            
        }
        #endregion
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult create_item_request()
        {
            return View();
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult fresh_request()
        {
            return View();
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult pending_approval_request()
        {
            return View();
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult get_Items_order()
        {
            return View();
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult get_Items_orderf()
        {
            return View();
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        [HttpGet]
        public ActionResult storeRequestInvoice(string orderid)
        {
            ViewBag.OrderID = orderid;
            var rq_order = db.store_requisition.Where(o => o.R_order_no == orderid && (o.request_status.ToLower() == "fresh" || o.request_status.ToLower() == "pending")).Select(o => new storeRequestViewModel
            {
                rid = o.R_ID,
                item_id = o.product_id,
                item_name = o.product_name,
                qty_requested = o.qty_requested,
                qty_allocated = o.qty_allocated,
                dept_name = o.department,
                unit_name = o.unit,
                state_office = o.state_office,
                request_type = o.request_type,
                rq_orderno = o.R_order_no,
                Rq_staff_name = (!string.IsNullOrEmpty(o.reqst_staff_name))?o.reqst_staff_name:null,
                request_date = o.Request_dt,
                request_status = o.request_status,
                created_date = o.Created_Date,
                s_r_v_no = o.S_R_V_No,
                Requested_qty_unit_value = o.Requested_qty_unit_value,
                Requested_qty_unit = o.Requested_qty_unit,
                item_base_unit = o.item_base_unit
            });
            if (rq_order.Count() > 0)
            {
                foreach (var i in rq_order)
                {
                    ViewBag.OrderID = i.s_r_v_no;
                    if (i.request_status.ToLower() == "fresh")
                    {
                        var getitem = db.store_requisition.Find(i.rid);
                        getitem.request_status = "pending";
                     
                    }
                }
                db.SaveChanges();
                return View(rq_order.ToList());
            }
            else
            {
                return View("dashboard");
            }
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        [HttpGet]
        public ActionResult FreshRqReceiptForAll()
        {
            var items = db.store_requisition.Where(s=>s.request_status.ToLower() == "fresh")
                .GroupBy(k => k.R_order_no)
                .Select(g => new
                {
                    orderid = g.Key,
                    request_pending = g.Select(c => new storeRequestListByORDERID_vm
                    {
                        orderid = c.R_order_no,
                        request_status = c.request_status,
                        request_type = c.request_type,
                        requesting_state = c.state_office,
                        requesting_dept = c.department,
                        unit = c.unit,
                        requesting_region = c.regional_office,
                        reqst_staff_name = c.reqst_staff_name,
                        srv = c.S_R_V_No,
                        request_date = db.store_requisition.FirstOrDefault(d => d.R_order_no == c.R_order_no).Request_dt,
                        created_date = db.store_requisition.FirstOrDefault(d => d.R_order_no == c.R_order_no).Created_Date,
                        total_item_requested = db.store_requisition.Where(t => t.R_order_no == c.R_order_no).Count(),
                    }).Distinct().OrderBy(d => d.request_type)
                }).ToList();

            List<storeRequestListByORDERID_vm> strq = new List<storeRequestListByORDERID_vm>();
            foreach (var item in items)
            {

                strq.AddRange(item.request_pending);
                //strq.Add(app.ToList());
            }
            var updateItemStatus = db.store_requisition.Where(i => i.request_status == "fresh").ToList();
            if (updateItemStatus.Count() > 0)
            {
                foreach (var t in updateItemStatus)
                {
                    var getItem = db.store_requisition.Find(t.R_ID);
                    getItem.request_status = "pending";
                    db.SaveChanges();
                }
            }
            return View(strq);
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        [HttpGet]
        public ActionResult PendingRqReceiptForAll()
        {
            var items = db.store_requisition.Where(s => s.request_status.ToLower() == "pending")
                .GroupBy(k => k.R_order_no)
                .Select(g => new
                {
                    orderid = g.Key,
                    request_pending = g.Select(c => new storeRequestListByORDERID_vm
                    {
                        orderid = c.R_order_no,
                        request_status = c.request_status,
                        request_type = c.request_type,
                        requesting_state = c.state_office,
                        requesting_dept = c.department,
                        requesting_region = c.regional_office,
                        reqst_staff_name = c.reqst_staff_name,
                        request_date = db.store_requisition.FirstOrDefault(d => d.R_order_no == c.R_order_no).Request_dt,
                        created_date = db.store_requisition.FirstOrDefault(d => d.R_order_no == c.R_order_no).Created_Date,
                        total_item_requested = db.store_requisition.Where(t => t.R_order_no == c.R_order_no).Count(),
                    }).Distinct().OrderBy(d => d.request_type)
                }).ToList();

            List<storeRequestListByORDERID_vm> strq = new List<storeRequestListByORDERID_vm>();
            foreach (var item in items)
            {

                strq.AddRange(item.request_pending);
                //strq.Add(app.ToList());
            }
            return View(strq);
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult approval_receipt(string orderid)
        {
            ViewBag.OrderID = orderid;
            var rq_order = db.store_requisition.Where(o => o.R_order_no == orderid && o.request_status.ToLower() == "approved").Select(o => new storeRequestViewModel
            {
                item_name = o.product_name,
                qty_requested = o.qty_requested,
                qty_allocated = o.qty_allocated,
                dept_name = o.department,
                unit_name = o.unit,
                state_office = o.state_office,
                request_type = o.request_type,
                rq_orderno = o.R_order_no,
                Rq_staff_name = o.reqst_staff_name,
                request_date = o.Request_dt,
                request_status = o.request_status,
                approved_by = o.approve_by,
                approved_date = o.Approve_dt,
                srv = o.S_R_V_No,
                Requested_qty_unit_value = o.Requested_qty_unit_value,
                Requested_qty_unit = o.Requested_qty_unit,
                item_base_unit = o.item_base_unit,
            });
            if (rq_order.Count() > 0)
            {
                return View(rq_order.ToList());
            }
            else
            {
                return View("dashboard");
            }
        }
        [Authorize]
        public JsonResult authUsername(string token, string userName)
        {
            Session["token"] = token;
            ulog.loguserActivities(userName, "Login Successfully!");
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult approved_request()
        {
            //var logInUserName = 
            var approved_RQlist = db.store_requisition.Where(s => s.request_status.ToLower() == "approved")
                .GroupBy(k => k.R_order_no)
                .Select(g => new
                {
                    orderid = g.Key,
                    request_pending = g.Select(c => new storeRequestListByORDERID_vm
                    {
                        orderid = c.R_order_no,
                        request_status = c.request_status,
                        request_type = c.request_type,
                        requesting_state = c.state_office,
                        requesting_dept = c.department,
                        requesting_region = c.regional_office,
                        unit = c.unit,
                        srv = c.S_R_V_No,
                        approved_date = db.store_requisition.FirstOrDefault(d => d.R_order_no == c.R_order_no && d.request_status.ToLower() == "approved").Approve_dt,
                        request_date = db.store_requisition.FirstOrDefault(d => d.R_order_no == c.R_order_no && d.request_status.ToLower() == "approved").Request_dt,
                        created_date=db.store_requisition.FirstOrDefault(d => d.R_order_no == c.R_order_no && d.request_status.ToLower() == "approved").Created_Date,
                        total_item_requested = db.store_requisition.Where(t => t.R_order_no == c.R_order_no && t.request_status.ToLower() == "approved").Count(),

                    }).Distinct(),
                }).ToList();
            //  ulog.loguserActivities(logInUserName, "View list of pending request");
            List<storeRequestListByORDERID_vm> strq = new List<storeRequestListByORDERID_vm>();
            foreach (var item in approved_RQlist)
            {
                strq.AddRange(item.request_pending);
                //strq.Add(app.ToList());
            }
            return View(strq.ToList());
        }
        public ActionResult approvedItemByOrderid(string o)
        {
            var appItems = db.store_requisition.Where(i => i.R_order_no == o && i.request_status.ToLower() == "approved").ToList();
            var dept = db.store_requisition.First(i => i.R_order_no == o);
            var state_office = db.store_requisition.First(i => i.R_order_no == o);
            TempData["Rdept"] = (dept.department != null) ? dept.department.ToString().ToUpper() : null;
            TempData["Rstate"] = (state_office.state_office != null) ? state_office.state_office.ToString().ToUpper() : null;
            if (appItems.Count() > 0)
            {
                foreach(var i in appItems)
                {
                    ViewBag.srv = i.S_R_V_No;
                }
                return View(appItems.ToList());
            }
            else
            {
                return View("approved_request");
            }
        }
        public JsonResult LogOut(string userName)
        {
            ulog.loguserActivities(userName, "Logout Successfully!");
            Session.Clear();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #region //report generation

        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult item_supplies_rpt()
        {
            var items_supplied = db.stock_in_items.GroupBy(key => key.s_r_v_no)
                .Select(s => new
                {
                    srn = s.Key,
                    items = s.Select(e => new storeSuppliesViewModel
                    {
                        srv = e.s_r_v_no,
                       // product_name = db.stock_in_items.fi,
                        supplier_name = db.stock_in_items.FirstOrDefault(sd => sd.s_r_v_no == e.s_r_v_no).supplier_name,
                        supplied_date = db.stock_in_items.FirstOrDefault(sd => sd.s_r_v_no == e.s_r_v_no).supplied_date,
                        Created_date = db.stock_in_items.FirstOrDefault(sd => sd.s_r_v_no == e.s_r_v_no).Created_date,
                        total_item_supplied = db.stock_in_items.Where(t => t.s_r_v_no == e.s_r_v_no).Count(),
                    }).Distinct()
                });
            List<storeSuppliesViewModel> sp = new List<storeSuppliesViewModel>();
            if (items_supplied.Count() > 0)
            {
                foreach (var i in items_supplied)
                {
                    sp.AddRange(i.items);
                }
            }
            return View(sp.ToList());     
        }
        public ActionResult itemsuppliesbysrv (string srv)
        {
            ViewBag.srv = srv;
            var data = db.stock_in_items.Where(d => d.s_r_v_no == srv).ToList();
            foreach (var item in data)
            {
                ViewBag.spname = item.supplier_name;
               
            }
            return View(data);
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult item_supplies_today()
        {
            return View(db.stock_in_items.Where(s=>DbFunctions.TruncateTime(s.supplied_date)== DateTime.UtcNow).ToList());
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult Request_Item_RPT()
        {
            List<state> state = new List<state>()
            {
               new state(){ id= 1, name= "Abia" },
new state() { id= 2, name= "Adamawa" },
 new state(){ id= 3, name= "Anambra" },
 new state(){ id= 4, name= "Akwa Ibom" },
 new state(){ id= 5, name= "Bauchi" },
 new state(){ id= 6, name= "Bayelsa" },
 new state(){ id= 7, name= "Benue" },
 new state(){ id= 8, name= "Borno" },
  new state(){ id= 9, name= "Cross River" },
  new state(){ id= 10, name= "Delta" },
  new state(){ id= 11, name= "Ebonyi" },
  new state(){ id= 12, name= "Enugu" },
  new state(){ id= 13, name= "Edo" },
  new state(){ id= 14, name= "Ekiti" },
  new state(){ id= 15, name= "FCT - Abuja" },
  new state(){ id= 16, name= "Gombe" },
  new state(){ id= 17, name= "Imo" },
    new state(){ id= 18, name= "Jigawa" },
    new state(){ id= 19, name= "Kaduna" },
    new state(){ id= 20, name= "Kano" },
  new state(){ id= 21, name= "Katsina" },
  new state(){ id= 22, name= "Kebbi" },
  new state(){ id= 23, name= "Kogi" },
  new state(){ id= 24, name= "Kwara" },
  new state(){ id= 25, name= "Lagos" },
  new state(){ id= 26, name= "Nasarawa" },
  new state(){ id= 27, name= "Niger" },
  new state(){ id= 28, name= "Ogun" },
  new state(){ id= 29, name= "Ondo" },
  new state(){ id= 30, name= "Osun" },
    new state(){ id= 31, name= "Oyo" },
    new state(){ id= 32, name= "Plateau" },
    new state(){ id= 33, name= "Rivers" },
    new state(){ id= 34, name= "Sokoto" },
    new state(){ id= 35, name= "Taraba" },
    new state(){ id= 36, name= "Yobe" },
    new state(){ id= 30, name= "Zamfara" }
            };
            List<Region> region = new List<Region>()
            {
               new Region(){ id= "North Central", name= "North Central" },
                new Region(){ id= "North East", name= "North East" },
                 new Region(){ id= "North West", name= "North West" },
                  new Region(){ id= "South East", name= "South East" },
                   new Region(){ id= "South West", name= "South West" },
                    new Region(){ id= "South South", name= "South South" },
            };
            List<RequestType> rtype = new List<RequestType>()
            {
               new RequestType(){ id= "internalRequest", name= "Internal Request" },
                new RequestType(){ id= "regionalRequest", name= "Regional Request" },
                 new RequestType(){ id= "stateRequest", name= "State Request" },
                 
            };
            ViewBag.state = state;
            ViewBag.region = region;
            ViewBag.rtype = rtype;
            return View(db.store_requisition.Where(s => s.request_status.ToLower() == "approved").ToList().OrderByDescending(d => d.Approve_dt).ThenByDescending(d => d.Request_dt));
        }

        public ActionResult recentRequest() {
            //var logInUserName = 
            var approved_RQlist = db.store_requisition
                .GroupBy(k => k.R_order_no)
                .Select(g => new
                {
                    orderid = g.Key,
                    request_pending = g.Select(c => new storeRequestListByORDERID_vm
                    {
                        orderid = c.R_order_no,
                        request_status = c.request_status,
                        request_type = c.request_type,
                        requesting_state = c.state_office,
                        requesting_dept = c.department,
                        requesting_region = c.regional_office,
                        approved_date = db.store_requisition.FirstOrDefault(d => d.R_order_no == c.R_order_no).Approve_dt,
                        request_date = db.store_requisition.FirstOrDefault(d => d.R_order_no == c.R_order_no).Request_dt,
                        created_date = db.store_requisition.FirstOrDefault(d => d.R_order_no == c.R_order_no).Created_Date,
                        total_item_requested = db.store_requisition.Where(t => t.R_order_no == c.R_order_no).Count(),
                    }).OrderBy(d => d.created_date).Distinct()
                }).ToList();
           // int coo= approved_RQlist.Count();
            //  ulog.loguserActivities(logInUserName, "View list of pending request");
            List<storeRequestListByORDERID_vm> strq = new List<storeRequestListByORDERID_vm>();
            foreach (var item in approved_RQlist)
            {
                strq.AddRange(item.request_pending);
                //strq.Add(app.ToList());
            }
            return View(strq);
        }

        public ActionResult recentItembyOrderid(string o)
        {
            var Items = db.store_requisition.Where(i => i.R_order_no == o).ToList();
            if (Items.Count() > 0)
            {
                return View(Items.ToList());
            }
            return View();
        }
        #endregion
        //Runing out of stocks
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]        
        public ActionResult ros()
        {
            var ros = db.product.Where(p => p.opening_stock_qty <= p.stock_reorder_alert_qty && p.opening_stock_qty>0).ToList().OrderByDescending(s=>s.opening_stock_qty);
            return View(ros.Select(p => new itemsViewModel
            {
                id = p.id,
                category_name = p.category.category_name,
                qtyAvailable = p.opening_stock_qty,
                product_name = p.product_name,
                item_base_unit = p.item_base_unit
            }).ToList());
        }
        #region // Activities logs and dbBackup
        [HttpPost]
        [Authorize(Roles = "StoreAdmin")]
        public JsonResult deleteLog()
        {
           int i = db.Database.ExecuteSqlCommand("Delete UsersActivitiesLogs");
           if (i > 0)
           {
               TempData["success"] = "User activities log has been successfully deleted.";
               return Json(true,JsonRequestBehavior.AllowGet);
           }
           else
           {
               TempData["error"] = "error deleting user activities log";
               return Json(false, JsonRequestBehavior.AllowGet);
           }
            

        }

        public ActionResult log()
        {
            var log = db.usersActivitiesLog.ToList().OrderByDescending(d => d.date);
            return View(log);
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult dbBackup_Restore()
        {
            string backupDIR = @"C:\eStore_BackupDB\";
            if (!System.IO.Directory.Exists(backupDIR))
            {
                System.IO.Directory.CreateDirectory(backupDIR);
            }
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                //   string folderpath = "C:\\SQLBackup";

                BackupService bs = new BackupService(connectionString, backupDIR);
                bs.BackupDatabase(dbName);
                TempData["success"] = "Database backup is successfully.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Database backup fail, Reason: " + ex.Message;
            }
            return View("dashboard");
        }
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult restoreDB()
        {
            //var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //string folderpath = "C:\\SQLBackup";
            //string dbName = "SON_eStoreDB";
            //BackupService bs = new BackupService(connectionString,folderpath);
            //bs.BackupDatabase(dbName);            
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public ActionResult restorDB(HttpPostedFileBase postedfile)
        {
            try
            {
                if (postedfile != null)
                {
                    //string dbName = "SON_eStoreDB";
                    string backupDIR = @"C:\eStore_BackupDB\";
                    if (!System.IO.Directory.Exists(backupDIR))
                    {
                        System.IO.Directory.CreateDirectory(backupDIR);
                    }
                    string files = backupDIR + Path.GetFileName(postedfile.FileName);
                    var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    //SqlConnection sqlConnection = new SqlConnection();
                    //sqlConnection.ConnectionString = connectionString;
                    //sqlConnection.Open();
                    //string sqlQuery = "RESTORE DATABASE " + dbName + " FROM DISK ='" + files + "'";
                    //SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                    //sqlCommand.CommandType = CommandType.Text;
                    //int iRows = sqlCommand.ExecuteNonQuery();
                    //sqlConnection.Close();
                    //TempData["success"] = "Database has been restore successfully";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string UseMaster = "USE master";
                        SqlCommand UseMasterCommand = new SqlCommand(UseMaster, conn);
                        UseMasterCommand.ExecuteNonQuery();

                        string Alter1 = @"ALTER DATABASE [" + dbName + "] SET Single_User WITH Rollback Immediate";
                        SqlCommand Alter1Cmd = new SqlCommand(Alter1, conn);
                        Alter1Cmd.ExecuteNonQuery();

                        string Restore = string.Format("Restore database " + dbName + " from disk='{0}'", files);
                        SqlCommand RestoreCmd = new SqlCommand(Restore, conn);
                        RestoreCmd.ExecuteNonQuery();

                        string Alter2 = @"ALTER DATABASE [" + dbName + "] SET Multi_User";
                        SqlCommand Alter2Cmd = new SqlCommand(Alter2, conn);
                        Alter2Cmd.ExecuteNonQuery();
                        conn.Close();
                        TempData["success"] = "Database has been restore successfully";

                    }
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Fail to restore database, Reason: " + ex.Message;
            }


            return View("dashboard");
        }
        #endregion
        #region //managing borrowing
        public ActionResult createBorowRequest()
        {
            return View();
        }
        public ActionResult borrowRequestList()
        {
            return View();
        }
        public ActionResult borrowed_items_by_order()
        {
            return View();
        }
        #endregion

        #region//Item conversion table
        public ActionResult converstionTable()
        {
            return View();
        }
        #endregion
    }
}
