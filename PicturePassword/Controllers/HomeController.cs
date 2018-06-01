using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using PicturePassword.data;
using System.IO;
using PicturePassword.Models;
namespace PicturePassword.Controllers
{
    public class HomeController : Controller
    {
        DBManager manager = new DBManager(Properties.Settings.Default.ConStr);
        [Authorize]
        public ActionResult Index()
        {
            var user = manager.GetUser(User.Identity.Name);
            return View(user);
        }
        [Authorize]
        public ActionResult MyAccount()
        {
            var vm = new MyAccountVM();
            vm.user = manager.GetUser(User.Identity.Name);
            vm.pictures = manager.UsersPictures(vm.user.id);
            return View(vm);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            manager.delete(id);
            return Redirect("MyAccount");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string password, string email)
        {
            var user = manager.GetUser(email, password);
            if(user == null)
            {
                return Redirect("login");
            }
            FormsAuthentication.SetAuthCookie(user.Email, true);
            return Redirect("Index");
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(User user, string password)
        {
            manager.AddUser(user, password);
            return RedirectToAction("login");
        }
        public ActionResult UploadSuccess(UploadedPicture picture, HttpPostedFileBase image)
        {
            string file = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            image.SaveAs(Server.MapPath("/Pictures/") + file);
            picture.Image = file;
            manager.Add(picture);
            
            return View(picture);
        }

        public ActionResult ViewImage(int id)
        {
            SingleImageVM imageVM = new SingleImageVM();
            var canViewIds = (List<int>)Session["canview"];
            if (TempData["Message"] != null)
            {
                imageVM.message = (string)TempData["Message"];
            }
            if (canViewIds == null || !canViewIds.Contains(id))
            {
                imageVM.canVisit = false;
                imageVM.picture = new UploadedPicture { id = id };
            }
            
            else
            {
                manager.TimesSeen(id);
                imageVM.canVisit = true;
                imageVM.picture = manager.GetPic(id);
                

            }

            return View(imageVM);
        }
        [HttpPost]
        public ActionResult ViewImage(string password, int id)
        {
            var pic = manager.GetPic(id);
            if(pic == null)
            {
                return Redirect("ViewImage");
            }
            if(password != pic.Password)
            {
                TempData["Message"] = "incorrect password";
            }
            else
            {
                List<int> CanView = new List<int>();
                if(Session["canview"]== null)
                {
                    Session["canview"] = CanView;
                }
                else
                {
                    CanView = (List<int>)Session["canview"];
                }
                CanView.Add(id);
            }
            return Redirect($"/home/viewimage?id={id}");
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("index");
        }

    }
}