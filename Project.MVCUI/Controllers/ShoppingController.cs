using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using Project.BLL.Repositories.ConcRep;
using Project.COMMON.Tools;
using Project.ENTITIES.Models;
using Project.MVCUI.Models.PageVMs;
using Project.MVCUI.Models.ShoppingTools;
using Project.VM.PureVMs;

namespace Project.MVCUI.Controllers
{
    public class ShoppingController : Controller
    {
        ProductRepository _pRep;
        OrderRepository _oRep;
        OrderDetailRepository _odRep;
        CategoryRepository _cRep;

        public ShoppingController()
        {
            _odRep = new OrderDetailRepository();
            _cRep = new CategoryRepository();
            _pRep = new ProductRepository();
            _oRep = new OrderRepository();
        }
        public ActionResult ShoppingList(int? page, int? categoryID)
        {
            IPagedList<ProductVM> products = categoryID == null ? _pRep.Where(x => x.Status != ENTITIES.Enums.DataStatus.Deleted).Select(x => new ProductVM
            {
                ID = x.ID,
                CategoryID = x.CategoryID,
                ProductName = x.ProductName,
                Status = x.Status.ToString(),
                UnitsInStock = x.UnitsInStock,
                UnitPrice = x.UnitPrice,
                ImagePath = x.ImagePath
            }).ToPagedList(page ?? 1, 9) : _pRep.Where(x => x.CategoryID == categoryID && x.Status != ENTITIES.Enums.DataStatus.Deleted).Select(x => new ProductVM
            {
                ID = x.ID,
                CategoryID = x.CategoryID,
                ProductName = x.ProductName,
                Status = x.Status.ToString(),
                UnitsInStock = x.UnitsInStock,
                UnitPrice = x.UnitPrice,
                ImagePath = x.ImagePath,
                CategoryName = x.Category.CategoryName
            }).ToPagedList(page ?? 1, 9);

            PaginationVM pvm = new PaginationVM
            {
                PagedProducts = products,
                Categories = _cRep.Where(x => x.Status != ENTITIES.Enums.DataStatus.Deleted).Select(x => new CategoryVM
                {
                    ID = x.ID,
                    CategoryName = x.CategoryName,
                    Description = x.Description
                }).ToList()

            };



            if (categoryID == null) TempData["catID"] = categoryID;

            return View(pvm);

        }
        public ActionResult AddToCart(int id)
        {
            Cart c = Session["scart"] == null ? new Cart() : Session["scart"] as Cart;

            Product eklenecekUrun = _pRep.Find(id);

            CartItem ci = new CartItem
            {
                ID = eklenecekUrun.ID,
                Name = eklenecekUrun.ProductName,
                Price = eklenecekUrun.UnitPrice,
                ImagePath = eklenecekUrun.ImagePath
            };

            c.SepeteEkle(ci);
            Session["scart"] = c;
            return RedirectToAction("ShoppingList");




        }
        public ActionResult CartPage()
        {
            if (Session["scart"] != null)
            {
                Cart c = Session["scart"] as Cart;
                CartPageVM cpvm = new CartPageVM
                {
                    Cart = c
                };
                return View(cpvm);
            }
            TempData["bos"] = "Sepetinizde ürün bulunmamaktadır";
            return RedirectToAction("ShoppingList");
        }
        public ActionResult DeleteFromCart(int id)
        {
            if (Session["scart"] != null)
            {
                Cart c = Session["scart"] as Cart;
                c.SepettenCikar(id);
                if (c.Sepetim.Count == 0)
                {
                    Session.Remove("scart");
                    TempData["sepetbos"] = "Sepetinizdeki tüm ürünler çıkartılmıştır";
                    return RedirectToAction("ShoppingList");
                }

            }
            return RedirectToAction("CartPage");

        }
        public ActionResult ConfirmOrder()
        {
            AppUser currentUser;
            if (Session["member"] != null)
            {
                currentUser = Session["member"] as AppUser;
            }
            return View();
        }
        //http://localhost:55665/api/Payment/ReceivePayment
        //PaymentRequestModel
        //Microsoft.AspNet.WebApi.Client indir
        //WebApiRestService.webapiclient indir

        [HttpPost]

        public ActionResult ConfirmOrder(OrderPageVM ovm)
        {
            bool sonuc;
            Cart sepet = Session["scart"] as Cart;
            ovm.Order.TotalPrice = ovm.PaymentRM.ShoppingPrice = sepet.TotalPrice;

            // APISection
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55665/api/");
                Task<HttpResponseMessage> postTask = client.PostAsJsonAsync("Payment/ReceivePayment", ovm.PaymentRM);

                HttpResponseMessage result;
                try
                {
                    result = postTask.Result;
                }
                catch (Exception)
                {
                    TempData["baglantiRed"] = "Banka baglantıyı reddetti";
                    return RedirectToAction("ShoppingList");
                }

                if (result.IsSuccessStatusCode) sonuc = true;
                else sonuc = false;

                if (sonuc)
                {
                    if (Session["member"] != null)
                    {
                        AppUser kullanici = Session["member"] as AppUser;
                        ovm.Order.AppUserID = kullanici.ID;
                    }

                    _oRep.Add(ovm.Order);

                    foreach (CartItem item in sepet.Sepetim)
                    {
                        OrderDetail od = new OrderDetail();
                        od.OrderID = ovm.Order.ID;
                        od.ProductID = item.ID;
                        od.TotalPrice = item.SubTotal;
                        od.Quantity = item.Amount;
                        _odRep.Add(od);

                        //Stoktan da düsürelim
                        Product stoktanDusurulecek = _pRep.Find(item.ID);
                        stoktanDusurulecek.UnitsInStock -= item.Amount;
                        _pRep.Update(stoktanDusurulecek);

                        //Algoritma  Ödevi : Eger stoktan düsürüldügünde stokta kalmayacak bir şekilde item varsa onun Amount'ı Sepette asılamayacak bir hale gelsin
                    }


                    TempData["odeme"] = "Siparişiniz bize ulasmıstır...Tesekkür ederiz";

                    if (Session["member"] != null)
                        MailService.Send(ovm.Order.AppUser.Email, body: $"Siparişiniz basarıyla alındı{ovm.Order.TotalPrice}"); //Kullanıcıya aldıgı ürünleri de Mail yoluyla gönderin...
                    else MailService.Send(ovm.Order.NonMemberEmail, body: $"Siparişiniz basarıyla alındı{ovm.Order.TotalPrice}");

                    Session.Remove("scart");
                    return RedirectToAction("ShoppingList");
                }

                else
                {
                    Task<string> s = result.Content.ReadAsStringAsync();
                    TempData["sorun"] = s;
                    return RedirectToAction("ShoppingList");
                }




            }


        }
    } }