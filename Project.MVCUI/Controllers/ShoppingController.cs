using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using Project.BLL.Repositories.ConcRep;
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
                CategoryID = x.CategoryID,
                ProductName = x.ProductName,
                Status = x.Status.ToString(),
                UnitsInStock = x.UnitsInStock,
                UnitPrice = x.UnitPrice,
                ImagePath = x.ImagePath
            }).ToPagedList(page ?? 1, 9) : _pRep.Where(x => x.CategoryID == categoryID && x.Status != ENTITIES.Enums.DataStatus.Deleted).Select(x => new ProductVM
            {
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
    }
}