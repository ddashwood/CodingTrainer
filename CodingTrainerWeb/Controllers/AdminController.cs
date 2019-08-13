using CodingTrainer.CodingTrainerModels.Security;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CodingTrainer.CodingTrainerWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        ICodingTrainerRepository rep;
        IUserServices userServices;

        public AdminController(ICodingTrainerRepository repository, IUserServices userServices)
        {
            rep = repository;
            this.userServices = userServices;
        }

        // GET: Admin
        public async Task<ActionResult> UserList()
        {
            var users = await rep.GetUsersAsync();
            return View(users);
        }

        public ActionResult Emulate(string id)
        {
            userServices.Emulate(id);
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Edit(string id)
        {
            var user = await userServices.GetUserByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id, ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await userServices.UpdateUser(user);
                    return RedirectToAction("UserList");
                }
                catch (Exception e)
                {
                    ViewBag.Error = "Error: " + e.GetBaseException().Message;
                    return View(user);
                }
            }
            else
            {
                return View(user);
            }
        }
    }
}