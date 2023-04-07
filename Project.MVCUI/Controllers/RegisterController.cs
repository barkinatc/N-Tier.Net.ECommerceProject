using Project.BLL.Repositories.ConcRep;
using Project.COMMON.Tools;
using Project.ENTITIES.Models;
using Project.VM.PureVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVCUI.Controllers
{
    public class RegisterController : Controller
    {
        AppUserRepository _auRep;
        AppUserProfileRepository _aupRep;
        public RegisterController()
        {
            _aupRep = new AppUserProfileRepository();
            _auRep = new AppUserRepository();

        }
        public ActionResult RegisterNow()
        {
            return View();

        }

        [HttpPost]

        public ActionResult RegisterNow(AppUserVM appUser, ProfileVM profile)
        {
            if (_auRep.Any(x => x.UserName == appUser.UserName))
            {
                ViewBag.ZatenVar = "Kullanıcı ismi daha önce alınmış.";
                return View();
            }
            else if (_auRep.Any(x => x.Email == appUser.Email))
            {
                ViewBag.ZatenVar = "Bu email zaten kayıtlı.";
                return View();
            }

            appUser.Password = DantexCrypt.Crypt(appUser.Password); // Şifreyi kriptopladık.
            AppUser domainUser = new AppUser
            {
                UserName = appUser.UserName,
                Password = appUser.Password,
                Email = appUser.Email
            };
            _auRep.Add(domainUser);
            string gonderilecekMail = "Tebrikler .. Hesabınız oluşturulmuştur..Hesabınızı aktive etmek için https://localhost:44328/Register/Activation/" + domainUser.ActivationCode + " linkine tıklayabilirsiniz.";

            MailService.Send(appUser.Email, body: gonderilecekMail, subject: "Hesap aktivasyon!!");


            if (!string.IsNullOrEmpty(profile.FirstName.Trim()) || !string.IsNullOrEmpty(profile.LastName.Trim()))
            {
                AppUserProfile domainProfile = new AppUserProfile
                {
                    ID = domainUser.ID,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName
                };
            }



            return View("RegisterOK");
        }

        public ActionResult RegisterOK()
        {
            return View();
        }
        public ActionResult Activation(Guid id)
        {
            AppUser aktifEdilecek = _auRep.FirstOrDefault(x => x.ActivationCode == id);

            if (aktifEdilecek!=null)
            {
                aktifEdilecek.Active = true;
                _auRep.Update(aktifEdilecek);
                TempData["HesapAktifMi"] = "Hesabınız aktif hale getirildi";

                return RedirectToAction("Login","Home");
            }
            //şüpheli girişi

            TempData["HesapAktifMi"] = "Hesabınız bulunamadı";

            return RedirectToAction("Login","Home");
        }
    }
}