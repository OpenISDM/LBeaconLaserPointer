using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using LBeacon.Models;

namespace LBeacon.Controllers
{
    [Authorize]
    public class LaserPointerInformationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LaserPointerInformations
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            return View(await db.LaserPointerInformations.ToListAsync());
        }

        // GET: LaserPointerInformations/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LaserPointerInformation laserPointerInformation = await db.LaserPointerInformations.FindAsync(id);
            if (laserPointerInformation == null)
            {
                return HttpNotFound();
            }
            return View(laserPointerInformation);
        }

        // GET: LaserPointerInformations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LaserPointerInformations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Longitude,Latitude,FaceLongitude,FaceLatitude,Floor")] LaserPointerInformation laserPointerInformation)
        {
            if (ModelState.IsValid)
            {
                laserPointerInformation.Id = Guid.NewGuid();
                db.LaserPointerInformations.Add(laserPointerInformation);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(laserPointerInformation);
        }

        // GET: LaserPointerInformations/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LaserPointerInformation laserPointerInformation = await db.LaserPointerInformations.FindAsync(id);
            if (laserPointerInformation == null)
            {
                return HttpNotFound();
            }
            return View(laserPointerInformation);
        }

        // POST: LaserPointerInformations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Longitude,Latitude,FaceLongitude,FaceLatitude,Floor")] LaserPointerInformation laserPointerInformation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(laserPointerInformation).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(laserPointerInformation);
        }

        // GET: LaserPointerInformations/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LaserPointerInformation laserPointerInformation = await db.LaserPointerInformations.FindAsync(id);
            if (laserPointerInformation == null)
            {
                return HttpNotFound();
            }
            return View(laserPointerInformation);
        }

        // POST: LaserPointerInformations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            LaserPointerInformation laserPointerInformation = await db.LaserPointerInformations.FindAsync(id);
            db.LaserPointerInformations.Remove(laserPointerInformation);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
