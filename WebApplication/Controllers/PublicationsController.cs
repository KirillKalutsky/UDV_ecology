using DBLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Controllers
{
    public class PublicationsController : Controller
    {
        private readonly DBLayerContext context;

        public PublicationsController(DBLayerContext context)
        {
            this.context = context;
        }
        // GET: PublicationsController
        public async Task<IActionResult> Index()
        {
            return View(await context.Publications.Include(x=>x.Source).ToListAsync());
        }

        // GET: PublicationsController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            return View(await context.GetPublicationById(id));
        }

        // GET: PublicationsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PublicationsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PublicationsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PublicationsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PublicationsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PublicationsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
