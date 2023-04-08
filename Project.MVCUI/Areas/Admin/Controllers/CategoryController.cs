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
    public class CategoryController : Controller
    {
        CategoryRepository _cRep;

        public CategoryController()
        {
            _cRep = new CategoryRepository();
        }
        public ActionResult ListCategories(int? id)
        {
            List<AdminCategoryVM> categories;
            if (id==null)
            {
                categories = _cRep.Select(x => new AdminCategoryVM
                {
                    ID = x.ID,
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    DeletedDate = x.DeletedDate,
                    Status = x.Status.ToString()
                }).ToList();
            }
            else
            {
                categories = _cRep.Where(x => x.ID == id).Select(x => new AdminCategoryVM
                {
                    ID = x.ID,
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    DeletedDate = x.DeletedDate,
                    Status = x.Status.ToString()
                }).ToList();
            }
            AdminCategoryListPageVM aclpvm = new AdminCategoryListPageVM
            {
                Categories = categories
            };
            return View(aclpvm);
        }

        public ActionResult AddCategory() 
        {
            return View();
        }   

        [HttpPost]
        public ActionResult AddCategory(AdminCategoryVM category)
        {
            Category c = new Category
            {

                CategoryName = category.CategoryName,
                Description = category.Description

            };
 
            _cRep.Add(c);

            return RedirectToAction("ListCategories");
        }

        public ActionResult UpdateCategory(int id)
        {
            AdminCategoryVM category = _cRep.Where(x => x.ID == id).Select(x => new AdminCategoryVM
            {
                ID = x.ID,
                CategoryName = x.CategoryName,
                Description = x.Description

            }).FirstOrDefault();

            AdminCategoryUpdatePageVM cupvm = new AdminCategoryUpdatePageVM
            {
                Category = category
            };

            return View(cupvm);
        }
        [HttpPost]
        public ActionResult UpdateCategory(AdminCategoryVM category)
        {
            
            Category guncellenecek = _cRep.Find(category.ID);
            guncellenecek.CategoryName = category.CategoryName;
            guncellenecek.Description = category.Description;
            _cRep.Update(guncellenecek);


            return RedirectToAction("ListCategories");

        }

        public ActionResult DeleteCategory(int id)
        {
            _cRep.Delete(_cRep.Find(id));
            return RedirectToAction("ListCategories");
        }
    }
}