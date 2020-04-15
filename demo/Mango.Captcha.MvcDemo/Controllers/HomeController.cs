using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Mango.Captcha.Core;

namespace Mango.Captcha.MvcDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Captcha()
        {
            return View();
        }

        public ActionResult GetCaptcha()
        {
            var mangoCaptcha = new MangoCaptcha();
            mangoCaptcha.Init(@"E:\image");
            var image = mangoCaptcha.Generate(out Guid key);

            byte[] imgByte;
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                ms.Position = 0;
                imgByte = ms.ToArray();
            }

            var base64Img = Convert.ToBase64String(imgByte);
            var allowGet = JsonRequestBehavior.AllowGet;
            return Json(new
            {
                key,
                base64Img
            }, allowGet);
        }

        [HttpPost]
        public ActionResult ValidateCaptcha(Guid key, List<int> selectValues)
        {

            var mangoCaptcha = new MangoCaptcha();

            try
            {
                var result = mangoCaptcha.Validate(key, selectValues);
                var message = result ? "success" : "fail";
                return Json(new
                {
                    success = result,
                    message
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    message = e.Message
                });
            }

        }

        public ActionResult GetScripts()
        {
            return Content(new MangoCaptcha().GetScript());
        }
    }
}