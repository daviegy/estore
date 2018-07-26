using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using SON_eStore.Models;
using System.Globalization;

namespace SON_eStore.Controllers
{
    [EnableCors("*", "*", "*")]
    [Authorize]
    public class CartController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public List<cartViewModel> cart { get; private set; }
        public List<item_supplied_cart> item_supplied_cart { get; private set; }
        [HttpGet]
        public IHttpActionResult getCart()
        {
            var cart = db.cart.Where(c => c.request_status.ToLower() == "fresh".ToLower()).ToList();
            if (cart.Count() > 0)
            {
                return Ok(cart);
            }
            else
            {
                return Content(HttpStatusCode.NoContent, cart);
            }

        }
        [Route("api/cart/getborrowCart")]
        public IHttpActionResult getborrowCart()
        {
            var cart = db.cart.Where(c => c.request_status.ToLower() == "borrow".ToLower()).ToList();
            if (cart.Count() > 0)
            {
                return Ok(cart);
            }
            else
            {
                return Content(HttpStatusCode.NoContent, cart);
            }

        }
        [Route("api/cart/getItemRequestCart/{id}")]
        public IHttpActionResult getItemRequestCart(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int Id = Convert.ToInt32(id);
                var items = db.cart.Find(Id);
                return Ok(items);
            }
            return NotFound();
        }
        [HttpPost]
        public IHttpActionResult addToCart([FromBody] cartViewModel cartmodel)
        {
            try
            {
                if (cartmodel != null)
                {
                    #region fresh request
                    if (cartmodel.request_status.ToLower() == "fresh".ToLower())
                    {
                        var user = (cartmodel.staff_id != null) ? db.Users.Find(cartmodel.staff_id) : null;
                        var unit = (cartmodel.unit_id != null) ? db.dept_unit.Find(cartmodel.unit_id) : null;
                        var dept = (cartmodel.dept_id != null) ? db.department.Find(cartmodel.dept_id) : null;
                        var item = (cartmodel.item_id != null) ? db.product.Find(cartmodel.item_id) : null;
                        cartViewModel model = new cartViewModel();
                        if (cartmodel.Request_type == "internalRequest")
                        {
                            model.cart_id = string.Concat("c-", cartmodel.dept_id);
                        }
                        else if (cartmodel.Request_type == "regionalRequest")
                        {
                            model.cart_id = string.Concat("c-", cartmodel.Region);
                        }
                        else if (cartmodel.Request_type == "stateRequest")
                        {
                            model.cart_id = string.Concat("c-", cartmodel.State);
                        }
                        model.dept_id = cartmodel.dept_id;
                        model.dept_name = (dept != null) ? dept.dept_name : null;
                        model.unit_id = cartmodel.unit_id;
                        model.unit_name = (unit != null) ? unit.unit_name : null;
                        model.staff_id = cartmodel.staff_id;
                        model.staff_name = (user != null) ? user.Name : null;
                        model.State = (!string.IsNullOrEmpty(cartmodel.State)) ? cartmodel.State : null;
                        model.isLabRequest = cartmodel.isLabRequest;
                        model.Requested_qty_unit = cartmodel.Requested_qty_unit;
                        model.Requested_qty_unit_value = cartmodel.Requested_qty_unit_value;
                        model.conversion_value = (cartmodel.conversion_value == 0) ? 1 : cartmodel.conversion_value;
                        model.Qty_Allocated = cartmodel.Qty_Allocated;
                        model.Qty_Requested = cartmodel.Qty_Requested;
                        model.item_base_unit = cartmodel.item_base_unit;
                        model.item_id = cartmodel.item_id;
                        model.item_name = item.product_name;
                        model.Request_type = cartmodel.Request_type;
                        model.s_r_v_no = cartmodel.s_r_v_no;
                        model.Region = cartmodel.Region;
                        model.request_status = cartmodel.request_status;
                        var dt = cartmodel.requested_date;
                        model.requested_date = cartmodel.requested_date;
                        db.cart.Add(model);
                        //check if cart is not null 
                        //and check if new cart_id matches the one earlier created
                        //if yes, proceed to add item to cart, other reject item from cart
                        var ct = db.cart.Where(c => c.request_status.ToLower() == "fresh".ToLower()).ToList();
                        if (ct.Count > 0)
                        {
                            #region
                            var cart_id_in_cart = db.cart.Where(m => m.cart_id == model.cart_id && m.request_status.ToLower() == "fresh".ToLower());
                            if (cart_id_in_cart.Count() > 0)
                            {
                                foreach (var b_item in cart_id_in_cart)
                                {
                                    if (b_item.Request_type == "stateRequest")
                                    {
                                        if (b_item.State == cartmodel.State)
                                        {
                                            var item_in_cart = db.cart.Where(i => i.item_id == model.item_id && i.request_status.ToLower() == "fresh".ToLower() && i.State == b_item.State);
                                            if (item_in_cart.Count() > 0)
                                            {
                                                return Content(HttpStatusCode.BadRequest, " You cannot add '" + model.item_name + "' to cart more than once, Kindly delete and re-add in-order to make changes");
                                            }


                                        }
                                        else
                                        {
                                            return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the department, state or region selected is different from the existing one in cart, kindly proceed to checkout the existing cart item or delete items in cart.");
                                        }

                                    }
                                    else if (b_item.Request_type == "regionalRequest")
                                    {
                                        if (b_item.Region == cartmodel.Region)
                                        {
                                            var item_in_cart = db.cart.Where(i => i.item_id == model.item_id && i.request_status.ToLower() == "fresh".ToLower() && i.Region == b_item.Region);
                                            if (item_in_cart.Count() > 0)
                                            {
                                                return Content(HttpStatusCode.BadRequest, " You cannot add '" + model.item_name + "' to cart more than once, Kindly delete and re-add in-order to make changes");
                                            }

                                        }
                                        else
                                        {
                                            return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the department, state or region selected is different from the existing one in cart, kindly proceed to checkout the existing cart item or delete items in cart.");
                                        }
                                    }
                                    else if (b_item.Request_type == "internalRequest")
                                    {
                                        if (b_item.dept_id == cartmodel.dept_id)
                                        {
                                            if (b_item.staff_id == cartmodel.staff_id)
                                            {
                                                if (!string.IsNullOrEmpty(b_item.unit_id))

                                                {
                                                    if (b_item.unit_id == cartmodel.unit_id)
                                                    {
                                                        var item_in_cart = db.cart.Where(i => i.item_id == model.item_id && i.request_status.ToLower() == "fresh".ToLower() && i.dept_id == b_item.dept_id);
                                                        if (item_in_cart.Count() > 0)
                                                        {
                                                            return Content(HttpStatusCode.BadRequest, " You cannot add '" + model.item_name + "' to cart more than once, Kindly delete and re-add in-order to make changes");
                                                        }

                                                    }
                                                    else
                                                    {
                                                        return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the UNIT selected is different from '" + b_item.unit_name.ToString().ToUpper() + "', kindly proceed to checkout the existing cart item or delete items in cart.");
                                                    }

                                                }
                                                else
                                                {
                                                    var item_in_cart = db.cart.Where(i => i.item_id == model.item_id && i.request_status.ToLower() == "fresh".ToLower() && i.dept_id == b_item.dept_id);
                                                    if (item_in_cart.Count() > 0)
                                                    {
                                                        return Content(HttpStatusCode.BadRequest, " You cannot add '" + model.item_name + "' to cart more than once, Kindly delete and re-add in-order to make changes");
                                                    }

                                                }


                                            }
                                            else
                                            {
                                                return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the STAFF selected is different from the one whose request is been process, kindly proceed to checkout the existing cart item or delete items in cart.");
                                            }
                                        }
                                        else
                                        {
                                            return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the department, state or region selected is different from the existing one in cart, kindly proceed to checkout the existing cart item or delete items in cart.");
                                        }
                                    }
                                    else
                                    {
                                        return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the department, state or region selected is different from the existing one in cart, kindly proceed to checkout the existing cart item or delete items in cart.");
                                    }
                                }

                            }
                            else
                            {
                                return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the department, state or region selected is different from the existing one in cart, kindly proceed to checkout the existing cart item or delete items in cart.");
                            }
                            #endregion
                        }                       
                        db.SaveChanges();
                        return Ok(cart);
                       

                    }
                    #endregion
                    #region borrow request
                    if (cartmodel.request_status.ToLower() == "borrow")
                    {
                        var user = (cartmodel.staff_id != null) ? db.Users.Find(cartmodel.staff_id) : null;
                        var unit = (cartmodel.unit_id != null) ? db.dept_unit.Find(cartmodel.unit_id) : null;
                        var dept = (cartmodel.dept_id != null) ? db.department.Find(cartmodel.dept_id) : null;
                        var item = (cartmodel.item_id != null) ? db.product.Find(cartmodel.item_id) : null;
                        cartViewModel model = new cartViewModel();
                        if (cartmodel.Request_type == "internalRequest")
                        {
                            model.cart_id = string.Concat("c-", cartmodel.dept_id);
                        }
                        else if (cartmodel.Request_type == "regionalRequest")
                        {
                            model.cart_id = string.Concat("c-", cartmodel.Region);
                        }
                        else if (cartmodel.Request_type == "stateRequest")
                        {
                            model.cart_id = string.Concat("c-", cartmodel.State);
                        }
                        model.dept_id = cartmodel.dept_id;
                        model.dept_name = (dept != null) ? dept.dept_name : null;
                        model.unit_id = cartmodel.unit_id;
                        model.unit_name = (unit != null) ? unit.unit_name : null;
                        model.staff_id = cartmodel.staff_id;
                        model.staff_name = (user != null) ? user.Name : null;
                        model.State = (!string.IsNullOrEmpty(cartmodel.State)) ? cartmodel.State: null;
                        model.isLabRequest = cartmodel.isLabRequest;
                        model.Requested_qty_unit = cartmodel.Requested_qty_unit;
                        model.Requested_qty_unit_value = cartmodel.Requested_qty_unit_value;
                        model.conversion_value = (cartmodel.conversion_value == 0) ? 1 : cartmodel.conversion_value;
                        model.Qty_Allocated = cartmodel.Qty_Allocated;
                        model.Qty_Requested = cartmodel.Qty_Requested;
                        model.item_base_unit = cartmodel.item_base_unit;
                        model.item_id = cartmodel.item_id;
                        model.item_name = item.product_name;
                        model.Request_type = cartmodel.Request_type;
                        model.s_r_v_no = cartmodel.s_r_v_no;
                        model.Region = cartmodel.Region;
                        model.request_status = cartmodel.request_status;
                        var dt = cartmodel.requested_date;
                        model.requested_date = cartmodel.requested_date;
                        db.cart.Add(model);
                        //check if cart is not null 
                        //and check if new cart_id matches the one earlier created
                        //if yes, proceed to add item to cart, other reject item from cart
                        var ct = db.cart.Where(c => c.request_status.ToLower() == "borrow".ToLower()).ToList();
                        if (ct.Count > 0)
                        {
                            #region
                            var cart_id_in_cart = db.cart.Where(m => m.cart_id == model.cart_id && m.request_status.ToLower() == "borrow".ToLower());
                            if (cart_id_in_cart.Count() > 0)
                            {
                                foreach (var b_item in cart_id_in_cart)
                                {
                                    if (b_item.Request_type == "stateRequest")
                                    {
                                        if (b_item.State == cartmodel.State)
                                        {
                                            var item_in_cart = db.cart.Where(i => i.item_id == model.item_id && i.request_status.ToLower() == "borrow".ToLower() && i.State == b_item.State);
                                            if (item_in_cart.Count() > 0)
                                            {
                                                return Content(HttpStatusCode.BadRequest, " You cannot add '" + model.item_name + "' to cart more than once, Kindly delete and re-add in-order to make changes");
                                            }
                                           

                                        }
                                        else
                                        {
                                            return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the department, state or region selected is different from the existing one in cart, kindly proceed to checkout the existing cart item or delete items in cart.");
                                        }

                                    }
                                    else if (b_item.Request_type == "regionalRequest")
                                    {
                                        if (b_item.Region == cartmodel.Region)
                                        {
                                            var item_in_cart = db.cart.Where(i => i.item_id == model.item_id && i.request_status.ToLower() == "borrow".ToLower() && i.Region == b_item.Region);
                                            if (item_in_cart.Count() > 0)
                                            {
                                                return Content(HttpStatusCode.BadRequest, " You cannot add '" + model.item_name + "' to cart more than once, Kindly delete and re-add in-order to make changes");
                                            }
                                            
                                        }
                                        else
                                        {
                                            return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the department, state or region selected is different from the existing one in cart, kindly proceed to checkout the existing cart item or delete items in cart.");
                                        }
                                    }
                                    else if (b_item.Request_type == "internalRequest")
                                    {
                                        if (b_item.dept_id == cartmodel.dept_id)
                                        {
                                            if (b_item.staff_id == cartmodel.staff_id)
                                            {
                                                if (!string.IsNullOrEmpty(b_item.unit_id))

                                                {
                                                    if (b_item.unit_id == cartmodel.unit_id)
                                                    {
                                                        var item_in_cart = db.cart.Where(i => i.item_id == model.item_id && i.request_status.ToLower() == "borrow".ToLower() && i.dept_id == b_item.dept_id);
                                                        if (item_in_cart.Count() > 0)
                                                        {
                                                            return Content(HttpStatusCode.BadRequest, " You cannot add '" + model.item_name + "' to cart more than once, Kindly delete and re-add in-order to make changes");
                                                        }
                                                        
                                                    }
                                                    else
                                                    {
                                                        return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the UNIT selected is different from '" + b_item.unit_name.ToString().ToUpper() + "', kindly proceed to checkout the existing cart item or delete items in cart.");
                                                    }

                                                }
                                                else
                                                {
                                                    var item_in_cart = db.cart.Where(i => i.item_id == model.item_id && i.request_status.ToLower() == "borrow".ToLower() && i.dept_id == b_item.dept_id);
                                                    if (item_in_cart.Count() > 0)
                                                    {
                                                        return Content(HttpStatusCode.BadRequest, " You cannot add '" + model.item_name + "' to cart more than once, Kindly delete and re-add in-order to make changes");
                                                    }
                                                    
                                                }


                                            }
                                            else
                                            {
                                                return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the STAFF selected is different from the one whose request is been process, kindly proceed to checkout the existing cart item or delete items in cart.");
                                            }
                                        }
                                        else
                                        {
                                            return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the department, state or region selected is different from the existing one in cart, kindly proceed to checkout the existing cart item or delete items in cart.");
                                        }
                                    }
                                    else
                                    {
                                        return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the department, state or region selected is different from the existing one in cart, kindly proceed to checkout the existing cart item or delete items in cart.");
                                    }
                                }

                            }
                            else
                            {
                                return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the department, state or region selected is different from the existing one in cart, kindly proceed to checkout the existing cart item or delete items in cart.");
                            }
                            #endregion
                        }

                        db.SaveChanges();
                            return Ok(cart);
                        

                    }
                    #endregion
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }


        }
        [HttpPut]
        [Route("api/cart/updatecart")]
        public IHttpActionResult updateCart([FromBody] cartViewModel cartmodel)
        {
            try
            {
                if (cartmodel != null)
                {
                    var user = (cartmodel.staff_id != null) ? db.Users.Find(cartmodel.staff_id) : null;
                    var unit = (cartmodel.unit_id != null) ? db.dept_unit.Find(cartmodel.unit_id) : null;
                    var dept = (cartmodel.dept_id != null) ? db.department.Find(cartmodel.dept_id) : null;
                    var item = (cartmodel.item_id != null) ? db.product.Find(cartmodel.item_id) : null;
                    var model = db.cart.Find(cartmodel.id);
                    if (cartmodel.Request_type == "internalRequest")
                    {
                        model.cart_id = string.Concat("c-", cartmodel.dept_id);
                    }
                    else if (cartmodel.Request_type == "regionalRequest")
                    {
                        model.cart_id = string.Concat("c-", cartmodel.Region);
                    }
                    else if (cartmodel.Request_type == "stateRequest")
                    {
                        model.cart_id = string.Concat("c-", cartmodel.State);
                    }
                    model.dept_id = cartmodel.dept_id;
                    model.dept_name = (dept != null) ? dept.dept_name : null;
                    model.unit_id = cartmodel.unit_id;
                    model.unit_name = (unit != null) ? unit.unit_name : null;
                    model.staff_id = cartmodel.staff_id;
                    model.staff_name = (user != null) ? user.Name : null;
                    model.State = (!string.IsNullOrEmpty(cartmodel.State)) ? cartmodel.State: null;
                    model.isLabRequest = cartmodel.isLabRequest;
                    model.Requested_qty_unit = cartmodel.Requested_qty_unit;
                    model.Requested_qty_unit_value = cartmodel.Requested_qty_unit_value;
                    model.conversion_value = (cartmodel.conversion_value == 0) ? 1 : cartmodel.conversion_value;
                    model.Qty_Allocated = cartmodel.Qty_Allocated;
                    model.Qty_Requested = cartmodel.Qty_Requested;
                    model.item_base_unit = cartmodel.item_base_unit;
                    model.item_id = cartmodel.item_id;
                    model.item_name = item.product_name;
                    model.Request_type = cartmodel.Request_type;
                    model.s_r_v_no = cartmodel.s_r_v_no;
                    model.Region = cartmodel.Region;
                    var dt = cartmodel.requested_date;
                    model.requested_date = cartmodel.requested_date;
                    db.SaveChanges();
                    return Ok(cart);
                    //db.cart.Add(model);
                    //check if cart is not null 
                    //and check if new cart_id matches the one earlier created
                    //if yes, proceed to add item to cart, other reject item from cart

                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }
        [HttpDelete]
        public IHttpActionResult deleteCart(string id)
        {
            if (id != null)
            {
                int Id = Convert.ToInt32(id);
                var cart = db.cart.Find(Id);
                db.cart.Remove(cart);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        [Route("api/cart/item_Supplied_to_Cart")]
        public IHttpActionResult item_Supplied_to_Cart([FromBody]item_supplied_cart item_supplied_model)
        {
            try
            {
                if (item_supplied_model != null)
                {
                    var supplier = (item_supplied_model.supplier_id != null) ? db.supplier.Find(item_supplied_model.supplier_id) : null;
                    var item = (item_supplied_model.item_id != null) ? db.product.Find(item_supplied_model.item_id) : null;
                    item_supplied_cart model = new item_supplied_cart();
                    model.supplier_id = item_supplied_model.supplier_id;
                    model.supplier = (supplier != null) ? supplier.supplier_name : null;
                    model.item_id = item_supplied_model.item_id;
                    model.item_name = (item != null) ? item.product_name : null;
                    model.item_base_unit = item_supplied_model.item_base_unit;
                    model.qtySupplied = item_supplied_model.qtySupplied;
                    model.qty_denomination = item_supplied_model.qty_denomination;
                    model.qty_supplied_in_base_unit = item_supplied_model.qty_supplied_in_base_unit;
                    model.s_r_v_no = item_supplied_model.s_r_v_no;
                    model.unit_price = item_supplied_model.unit_price;
                    model.total_amount_per_item = item_supplied_model.total_amount_per_item;
                    model.supplied_date = item_supplied_model.supplied_date;
                    db.item_supplied_cart.Add(model);
                    //check if cart is not null 
                    //and check if new cart_id matches the one earlier created
                    //if yes, proceed to add item to cart, other reject item from cart
                    var ct = db.item_supplied_cart.ToList();
                    if (ct.Count > 0)
                    {
                        var s_r_v_no_in_cart = db.item_supplied_cart.FirstOrDefault(m => m.s_r_v_no == model.s_r_v_no);
                        if (s_r_v_no_in_cart != null)
                        {
                            var item_in_cart = db.item_supplied_cart.FirstOrDefault(i => i.item_id == model.item_id);
                            var supplier_in_cart = db.item_supplied_cart.FirstOrDefault(s => s.supplier_id == model.supplier_id);
                            if (item_in_cart != null)
                            {
                                return Content(HttpStatusCode.BadRequest, " You cannot add '" + model.item_name + "' to cart more than once, Kindly delete and re-add in-order to make changes");
                            }
                            if (supplier_in_cart == null)
                            {
                                return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the supplier you selected is the different from the one currently been processed, kindly proceed to checkout the existing cart item or delete item(s) in cart");
                            }
                            db.SaveChanges();
                            return Ok(item_supplied_cart);
                        }
                        else
                        {
                            return Content(HttpStatusCode.BadRequest, "New item cannot be added to cart, because the store requistion number is the different from the one currently been processed, kindly proceed to checkout the existing cart item or delete item(s) in cart.");
                        }

                    }
                    db.SaveChanges();
                    return Ok(item_supplied_cart);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }
        [HttpGet]
        [Route("api/cart/getItem_Supplied_to_Cart")]
        public IHttpActionResult getItem_Supplied_to_Cart()
        {
            var cart = db.item_supplied_cart.ToList();
            if (cart.Count() > 0)
            {
                return Ok(cart);
            }
            else
            {
                return Content(HttpStatusCode.NoContent, cart);
            }

        }
        [HttpDelete]
        [Route("api/cart/{id}/deleteItem_Supplied_to_Cart")]
        public IHttpActionResult deleteItem_Supplied_to_Cart(string id)
        {
            if (id != null)
            {
                int Id = Convert.ToInt32(id);
                var cart = db.item_supplied_cart.Find(Id);
                db.item_supplied_cart.Remove(cart);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        [System.Web.Http.HttpGet]
        [Route("api/cart/item_Supplied_to_Cart/{id}")]
        public IHttpActionResult getItem_Supplies_in_cart(string id)
        {
            // var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (id != null)
                {
                    int ctid = Convert.ToInt32(id);
                    var ct = db.item_supplied_cart.Find(ctid);
                    if (ct != null)
                    {
                        // ulog.loguserActivities(logInUserName, "Requested for store supplied item: '" + ct.product_name + "'");
                        return Ok(ct);
                    }
                    else
                    {
                        // ulog.loguserActivities(logInUserName, "Supplier '" + id + "' not found");
                        return Content(HttpStatusCode.NotFound, "Supplies not found");
                    }
                }
                else
                {
                    //  ulog.loguserActivities(logInUserName, "Supplies '" + id + " 'not found");
                    return Content(HttpStatusCode.NotFound, "Supplies not found");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Route("api/cart/updateSupplies_in_Cart")]
        [HttpPut]
        public HttpResponseMessage updateSupplies_in_Cart([FromBody]item_supplied_cart model)
        {
            // var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {

                if (!string.IsNullOrEmpty(model.s_r_v_no) && !string.IsNullOrEmpty(model.supplier_id) && !string.IsNullOrEmpty(model.item_id) && model.qty_supplied_in_base_unit > 0)
                {
                    //IDictionary<string, string> values = JsonConvert.DeserializeObject<IDictionary<string, string>>(data);
                    string supplier_name = db.supplier.Find(model.supplier_id).supplier_name;
                    var p = db.product.Find(model.item_id);
                    var ct = db.item_supplied_cart.Find(model.id);
                    if (ct != null)
                    {
                        //  ulog.loguserActivities(logInUserName, "Update the details of Store supply item : '" + ct.product_name + "'");
                        ct.supplier = supplier_name;
                        ct.supplier_id = model.supplier_id;
                        ct.item_id = model.item_id;
                        ct.item_name = p.product_name;
                        ct.qtySupplied = model.qtySupplied;
                        ct.qty_denomination = model.qty_denomination;
                        ct.qty_supplied_in_base_unit = model.qty_supplied_in_base_unit;
                        ct.item_base_unit = model.item_base_unit;
                        ct.unit_price = model.unit_price;
                        ct.total_amount_per_item = model.total_amount_per_item;
                        ct.supplied_date = model.supplied_date;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "cart has been updated successfully!");
                    }
                    else
                    {
                        // ulog.loguserActivities(logInUserName, "supplier details update fail");
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "cart update fail");
                    }
                }
                //   ulog.loguserActivities(logInUserName, "supplier details update fail");
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "All Fields must be filled!");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }
        //#region
        //[HttpGet]
        //[Route("api/cart/getCartItem_dept/{dept_id}")]
        //public IHttpActionResult getCartItem_dept (string dept_id)
        //{
        //    try
        //    {
        //        var dept = db.cart.Where(d => d.dept_id == dept_id);
        //        return Ok(dept);
        //    }
        //    catch (Exception ex) {
        //        return Content(HttpStatusCode.BadRequest, ex.Message);
        //    }
          
        //}
        //[HttpGet]
        //[Route("api/cart/getCartItem_unit/{unit_id}")]
        //public IHttpActionResult getCartItem_unit(string unit_id)
        //{
        //    try
        //    {
        //        var unit = db.cart.Where(d => d.unit_id == unit_id);
        //        return Ok(unit);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(HttpStatusCode.BadRequest, ex.Message);
        //    }

        //}
        //[HttpGet]
        //[Route("api/cart/getCartItem_staff/{staff_id}")]
        //public IHttpActionResult getCartItem_staff(string staff_id)
        //{
        //    try
        //    {
        //        var staff = db.cart.Where(d => d.staff_id == staff_id);
        //        return Ok(staff);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(HttpStatusCode.BadRequest, ex.Message);
        //    }

        //}
        //#endregion
    }
}
