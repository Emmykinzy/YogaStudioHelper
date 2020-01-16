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

            Util.EmailSender.sendPurchaseConfirmation(u, pl);
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
            Promotion p = db.getPromotionByPassId(cp.Pass_Id);
            decimal price = cp.Pass_Price;
            decimal dis = 0;
            if(p.Promo_End.Date > DateTime.Today && p.Num_Classes == 0)
            {
                dis = (price * (decimal)p.Discount)* -1;
            }
            double t = ((double)(price+dis) * .15);
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
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
                name = p.Promo_Desc,
                currency = "CAD",
                price = dis.ToString("F"),
                quantity = "1",
                
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = t.ToString("F"),
                subtotal = (price+dis).ToString("F")

            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "CAD",
                total = ((double)(price + dis) + t).ToString("F"), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details,
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Promo",                
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



    }


    }