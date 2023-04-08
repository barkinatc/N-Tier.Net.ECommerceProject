using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Project.MVCUI.Models.CustomTools
{
    public static class ImageUpLoader
    {
        //Geriye string değer döndüren metodumz resmin yolunu döndürecek veya resim yükleme ile ilgili bir sorun varsa onun kodunu döndürecek.

        //HttpPostedFileBase 

        public static string UploadImage(string serverPath,HttpPostedFileBase file,string name)
        {
            if (file!=null)
            {
                Guid uniqueName = Guid.NewGuid();

                string[] fileArray = file.FileName.Split('.'); //ilgili yapıyı noktalardan parçalayarak bir string arrayıne attık.

                string extension = fileArray[fileArray.Length - 1].ToLower();// file arrayin son üyesini seçip extensiona atıyoruz..

                string fileName = $"{uniqueName}.{name}.{extension} "; // guid kullandıımız için asla bir dosya ismi aynı olmayacak

                if (extension =="jpg" || extension =="gif" || extension == "png")
                {
                    //Eğer guid kullanmasaydık dosya ismi zaten varise

                    if (File.Exists(HttpContext.Current.Server.MapPath(serverPath + fileName)))
                    {
                        return "1";//Dosya zaten var kodu..
                    }
                    else
                    {
                        string filePath = HttpContext.Current.Server.MapPath(serverPath + fileName);
                        file.SaveAs(filePath);
                        return $"{serverPath}{fileName}";
                    }
                }
                else
                {
                    return "2"; //Seçilen dosya bir resim değildir kodu
                }
            }
            else
            {
                return "3"; //Dosya boş kodu
            }
        }
    }
}