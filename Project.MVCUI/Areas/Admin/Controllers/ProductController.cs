using Project.BLL.Repositories.ConcRep;
using Project.ENTITIES.Models;
using Project.MVCUI.Areas.Admin.Data.AdminPageVMs;
using Project.MVCUI.Models.CustomTools;
using Project.VM.PureVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVCUI.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        ProductRepository _pRep;
        CategoryRepository _cRep;

        public ProductController()
        {
            _pRep = new ProductRepository();
            _cRep = new CategoryRepository();
        }


        //Product id si değildir category id sidir.


        public ActionResult ListProducts(int? id)
        {

            List<AdminCategoryVM> categories = _cRep.Select(x => new AdminCategoryVM
            {
                ID = x.ID,
                CategoryName = x.CategoryName,
                Description = x.Description
            }).ToList();

            List<AdminProductVM> products = _pRep.Where(x => x.Status != ENTITIES.Enums.DataStatus.Deleted).Select(x => new AdminProductVM
            {
                ID = x.ID,
                ProductName = x.ProductName,
                CategoryName = x.Category.CategoryName,
                UnitPrice = x.UnitPrice,
                UnitsInStock = x.UnitsInStock,
                ImagePath = x.ImagePath,
                Status = x.Status.ToString()
                
            }).ToList();

            AdminProductListPageVM apvm = new AdminProductListPageVM
            {
 
                Categories = categories,
                Products   = products

               
            };
            return View(apvm);
        }
        public ActionResult AddProduct()
        {
            AdminProductAddUpdatePageVM apvm = new AdminProductAddUpdatePageVM
            {
                Categories = _cRep.Select(x => new AdminCategoryVM
                {
                    ID = x.ID,
                    CategoryName = x.CategoryName,
                    Description = x.Description
                }).ToList()
            };

            return View(apvm);
        }

        [HttpPost]
        public ActionResult AddProduct(AdminProductVM product,HttpPostedFileBase image, string fileName)
        {
            //product.ImagePath = ImageUpLoader.UploadImage("/Pictures/",image,fileName);

            Product p = new Product
            {
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                ImagePath = product.ImagePath = ImageUpLoader.UploadImage("/Pictures/", image, fileName),
                CategoryID = product.CategoryID,


            };
            _pRep.Add(p); 

            return RedirectToAction("ListProducts");
        }
        public ActionResult UpdateProduct(int id)
        {
            List<AdminCategoryVM> categories = _cRep.Select(x=>new AdminCategoryVM
            {
                ID=x.ID,
                CategoryName = x.CategoryName,
                Description = x.Description
            }).ToList();

            AdminProductAddUpdatePageVM aupvm = new AdminProductAddUpdatePageVM
            {
                Categories = categories,
                Product = _pRep.Where(x => x.ID == id).Select(x => new AdminProductVM
                {
                    ID = x.ID,
                    ProductName = x.ProductName,
                    CategoryID = x.CategoryID,
                    UnitPrice = x.UnitPrice,
                    UnitsInStock = x.UnitsInStock,  
                    ImagePath = x.ImagePath
                    

                }).FirstOrDefault()
            };

            return View( aupvm);
        }
        [HttpPost]

        public ActionResult UpdateProduct(AdminProductVM product, HttpPostedFileBase image, string fileName)
        {
            Product guncellenecek = _pRep.Find(product.ID);
            guncellenecek.ID = product.ID;
            guncellenecek.ProductName = product.ProductName;
            guncellenecek.CategoryID = product.CategoryID;
            guncellenecek.UnitPrice = product.UnitPrice;
            guncellenecek.UnitsInStock = product.UnitsInStock;

            if (guncellenecek.ImagePath ==null)
            {
                guncellenecek.ImagePath = product.ImagePath = ImageUpLoader.UploadImage("/Pictures/", image, fileName);
            }



            _pRep.Update(guncellenecek);

            return RedirectToAction("ListProducts");
        }

        public ActionResult DeleteProduct(int id)
        {
            _pRep.Delete(_pRep.Find(id));

            return RedirectToAction("ListProducts");
        }
    }
}