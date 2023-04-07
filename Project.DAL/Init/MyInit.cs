using Bogus.DataSets;
using Project.COMMON.Tools;
using Project.DAL.Context;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.Init
{
    public class MyInit:CreateDatabaseIfNotExists<MyContext>
    {
        protected override void Seed(MyContext context)
        {
            //admin
            AppUser au = new AppUser();
            au.UserName = "barkin";
            au.Password = DantexCrypt.Crypt("123");
            au.Email = "barkin@hotmail.com";
            au.Role = ENTITIES.Enums.UserRole.Admin;
            au.Active = true;
            context.AppUsers.Add(au);
            context.SaveChanges();


            //Kullanıcı 
            for (int i = 0; i < 10; i++)
            {
                AppUser ap = new AppUser();

                ap.UserName = new Internet("tr").UserName();
                ap.Password = new Internet("tr").Password();
                ap.Email = new Internet("tr").Email();
                context.AppUsers.Add(ap);
            }
            context.SaveChanges();

            //Kullanıcı profili

            //i 2 den başlar çünkü 1 admin ekledik.
            for (int i = 2; i < 12; i++)
            {
                AppUserProfile apu = new AppUserProfile();
                apu.ID = i;
                apu.FirstName = new Name().FirstName();
                apu.LastName = new Name().LastName();
                context.AppUserProfiles.Add(apu);


            }
            context.SaveChanges();

            //CAtegory ekleme

            for (int i = 0; i < 10; i++)
            {
                Category c = new Category();
                c.CategoryName = new Commerce("tr").Categories(1)[0];
                c.Description = new Lorem("tr").Sentence(10);


                //Categorinin içine product ekleme
                for (int j = 0; j < 30; j++)
                {
                    Product p = new Product();
                    p.ProductName = new Commerce("tr").ProductName();
                    p.UnitPrice = Convert.ToDecimal(new Commerce("tr").Price());
                    p.UnitsInStock = 100;
                    c.Products.Add(p);
                }
                context.Categories.Add(c);
                context.SaveChanges();
            }

        }
    }
}
