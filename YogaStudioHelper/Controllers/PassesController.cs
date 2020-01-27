using Database;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using YogaStudioHelper.Util;

namespace YogaStudioHelper.Controllers
{
    public class PassesController : Controller
    {
        private Payment payment;
        DBMaster db = new DBMaster();


        // GET: Passes

        //View hardcoded 
        public ActionResult OnlineStore()
        {

            //String message = Util.EmailSender.sendEmail();
            //Response.Write(message);

            IEnumerable<Promotion> promoList = db.getPromotions();
            ViewBag.PromoList = promoList; 

           
            IEnumerable<Class_Passes> class_Pass_List = db.getClassPasses();
            return View(class_Pass_List);


        }

        public ActionResult Store(int id)
        {

            //String message = Util.EmailSender.sendEmail();
            //Response.Write(message);
            ViewBag.uId = id;
            IEnumerable<Promotion> promoList = db.getPromotions();
            ViewBag.PromoList = promoList;


            IEnumerable<Class_Passes> class_Pass_List = db.getClassPasses();
            return View(class_Pass_List);


        }

        public ActionResult StorePurchase(int passId, int userId)
        {
            Pass_Log pl = db.processPurchase(db.getClassPasse(passId), userId);

            Yoga_User u = db.getUserById(userId);

            EmailSender.sendPurchaseConfirmation(u, pl);

            return RedirectToAction("Store");
        }


        public ActionResult Purchase(int passId, string Cancel = null)
        {
            var pass = db.getClassPasse(passId);

            // veryfy paypal successfull before 
            //getting the apiContext  
            APIContext apiContext = Paypal.GetAPIContext();
            try
            {

                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {

                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Passes/Purchases?passId="+pass.Pass_Id+"&";

                    var guid = Convert.ToString((new Random()).Next(100000));

                    var createdPayment = CreatePayment(apiContext, baseURI + "guid=" + guid, pass);

                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return View("FailureView");
            }

            return View("FailureView");
        }

        public ActionResult Purchases(string Cancel = null)
        {
            int passId = Int32.Parse(Request.QueryString["passId"]);
            var pass = db.getClassPasse(passId);

            // veryfy paypal successfull before 
            //getting the apiContext  
            APIContext apiContext = Paypal.GetAPIContext();
            try
            {

                string payerId = Request.Params["PayerID"];
                
                var guid = Request.Params["guid"];
                var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                if (executedPayment.state.ToLower() != "approved")
                {
                    return View("FailureView");
                }
                
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return View("FailureView");
            }

            int userId = Int32.Parse(Session["Uid"].ToString());

            //todo update all field correctly later on 

            // create purchase log 
            Pass_Log pl = db.processPurchase(pass, userId);

            Yoga_User u = db.getUserById(userId);

            EmailSender.sendPurchaseConfirmation(u, pl);
            // todo success message with receipt etc. 

            return View("SuccessView");
        }

            public ActionResult FailureView()
        {
            return View();
        }

        public ActionResult SuccessView()
        {
            return View();
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            payment = new Payment()
            {
                id = paymentId
            };
            return payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl, Class_Passes cp)
        {
            Promotion p;

            p = db.getPromotionByPassId(cp.Pass_Id);
            

            decimal price = decimal.Round(cp.Pass_Price, 2);
            decimal dis = 0;
            decimal t;
            var amount = new Amount();
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            var details = new Details();
            string desc = "";
            try
            {
                if (p.Promo_End.Date > DateTime.Today && p.Num_Classes == 0)
                {
                    decimal discount = (decimal)p.Discount;
                    dis = (price * discount * -1);
                    dis = decimal.Round(dis, 2);

                    t = ((price + dis) * (decimal).15);
                    t = decimal.Round(t, 2);
                    //create itemlist and add item objects to it  
                    //Adding Item Details like name, currency, price etc  
                    itemList.items.Add(new Item()
                    {
                        name = cp.Pass_Name,
                        currency = "CAD",
                        price = price.ToString("F"),
                        quantity = "1"
                    });
                    itemList.items.Add(new Item()
                    {
                        name = p.Promo_Desc + " " + (int)(discount * 100) + "% Off",
                        currency = "CAD",
                        price = dis.ToString("F"),
                        quantity = "1",

                    });
                    // Adding Tax, shipping and Subtotal details  


                    details.tax = t.ToString("F");
                    details.subtotal = (price + dis).ToString("F");

                    
                    //Final amount with details  
                    amount.currency = "CAD";
                    amount.total = ((price + dis) + t).ToString("F");
                    amount.details = details;
                }
                else
                {
                    
                    t = ((price) * (decimal).15);
                    //create itemlist and add item objects to it  

                    //Adding Item Details like name, currency, price etc  
                    itemList.items.Add(new Item()
                    {
                        name = cp.Pass_Name+" + "+p.Num_Classes+" Classes",
                        currency = "CAD",
                        price = price.ToString("F"),
                        quantity = "1"
                    });

                    details.tax = t.ToString("F");
                    details.subtotal = (price).ToString("F");


                    //Final amount with details  
                    amount.currency = "CAD";
                    amount.total = ((price) + t).ToString("F");
                    amount.details = details;
                }
            }
            catch
            {
                t = ((price) * (decimal).15);
                //create itemlist and add item objects to it  

                //Adding Item Details like name, currency, price etc  
                itemList.items.Add(new Item()
                {
                    name = cp.Pass_Name,
                    currency = "CAD",
                    price = price.ToString("F"),
                    quantity = "1"
                });
                details.tax = t.ToString("F");
                details.subtotal = (price).ToString("F");


                //Final amount with details  
                amount.currency = "CAD";
                amount.total = ((price) + t).ToString("F");
                amount.details = details;
            }

            // Configure Redirect Urls here with RedirectUrls object  

            // Adding Tax, shipping and Subtotal details  

            


            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = desc,
                amount = amount,
                item_list = itemList
            });
            payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return payment.Create(apiContext);

        }

        [HttpGet]
        public ActionResult PurchaseFindUser()
        {

            return View();
        }

        [HttpPost]
        public ActionResult PurchaseFindUser(FormCollection collection)
        {

            string email = collection["Email"];
            IEnumerable<Yoga_User> userList = db.getUserByEmail(email);

            if (userList.Count() == 0)
            {
                ViewBag.FindClassMessage = "No users with an email containing " + email + " was found";
                return View();
            }
            else
            {

                    IEnumerable<Yoga_User> l = userList.Where(x => x.Active == true);
                    TempData["userList"] = l;
                    return RedirectToAction("UserList");
  
            }

        }

        public ActionResult UserList()
        {
            if (TempData["userList"] != null)
            {
                IEnumerable<Yoga_User> c = TempData["userList"] as IEnumerable<Yoga_User>;
                return View(c);
            }
            else
            {
                IEnumerable<Yoga_User> c = TempData["List"] as IEnumerable<Yoga_User>;

                return View(c);
            }
        }
    }

   

}

 
        






    