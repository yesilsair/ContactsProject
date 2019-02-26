using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using Rehber.Models;

namespace Rehber.Controllers
{
    public class HomeController : Controller
    {


        public IActionResult Index()
        {
            return View();
        }

        private readonly IHostingEnvironment hostingEnvironment;
        public HomeController(IHostingEnvironment environment)
        {
            hostingEnvironment = environment;
        }
        [HttpPost]
        public async Task<IActionResult> SendData(List<IFormFile> files)
        {
            var model = new List<Recorded>();

            try
            {
                var uploads = Path.Combine(hostingEnvironment.WebRootPath);
                var adres = Path.Combine(uploads, files.First().FileName);

                foreach (var file in files)
                {
                    var filePath = Path.Combine(uploads, file.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
                var RehberList = System.IO.File.ReadAllLines(adres, Encoding.Default).ToList(); //Dosya Rehberlistesine alınır
                var OptionalArea = "";
                var ContactsName = "";
                int Counter = 1;
                foreach (var row in RehberList.Skip(1)) //Data dönerken kolon isimlerini atlamak için 
                {
                    var ContactColumns = row.Split(','); //Kolonlar virgül ile ayrılır.
                    ContactsName = ContactColumns[0];
                    OptionalArea = ContactColumns[1];
                    try
                    {
                        var Number = OptionalArea.Substring(OptionalArea.Length - 1);
                        Int32 yeni = Convert.ToInt32(Number) * 3; //2 isimli kaydedilen kişiler için 2.kolonun telefon no olup olmadığını kontrol ediyorum. çarpma işlemi son karakterde başarılı olursa o bir telefon numarası değil se bu döngüden çıkacaktır.
                        ContactsName = ContactColumns[0];
                        OptionalArea = ContactColumns[1].Replace("(", "").Replace(")", "").Replace("+", "").Replace(" ", string.Empty); //Parantez işaretinden ve + işaretinden kurtulmak için 
                    }
                    catch
                    {
                        ContactsName = ContactColumns[0] + " " + ContactColumns[1];
                        OptionalArea = ContactColumns[2].Replace("(", "").Replace(")", "").Replace("+", "");
                    }
                    model.Add(new Recorded { Name = ContactsName, Number = OptionalArea, ID = Counter.ToString() });
                    Counter++;
                }
                return View(model);
            }
            catch
            {
                model.Add(new Recorded { Name = "Bir Hata Oluştu", Number = "", ID = "" });
                return View(model);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
