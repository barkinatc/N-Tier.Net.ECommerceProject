using Project.BLL.Repositories.ConcRep;
using Project.ENTITIES.Models;
using Project.MVCUI.Areas.Admin.Data.AdminPageVMs;
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

            List<AdminProductVM> products = _pRep.Select(x => new AdminProductVM
            {
                ID = x.ID,
                ProductName = x.ProductName,
                CategoryName = x.Category.CategoryName,
                UnitPrice = x.UnitPrice,
                UnitsInStock = x.UnitsInStock,
                ImagePath = x.ImagePath
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
        public ActionResult AddProduct(AdminProductVM product)
        {
            Product p = new Product
            {
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                ImagePath = product.ImagePath,
                CategoryID = product.CategoryID,


            };
            _pRep.Add(p); 

            return RedirectToAction("ListProducts");
        }
    }
}