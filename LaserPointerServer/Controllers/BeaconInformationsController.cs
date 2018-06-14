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
using System.Drawing;
using LBeacon.Class;

namespace LBeacon.Controllers
{
    public class BeaconInformationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BeaconInformations
        public async Task<ActionResult> Index()
        {
            return View(await db.BeaconInformations.ToListAsync());
        }

        // GET: BeaconInformations/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BeaconInformation beaconInformation = await db.BeaconInformations.FindAsync(id);
            if (beaconInformation == null)
            {
                return HttpNotFound();
            }
            return View(beaconInformation);
        }

        // GET: BeaconInformations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BeaconInformations/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Position,Latitude,Longitude,Floor,LaserPointerInformationId")] BeaconInformation beaconInformation)
        {
            if (ModelState.IsValid)
            {
                beaconInformation.Id = Guid.NewGuid();
                db.BeaconInformations.Add(beaconInformation);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(beaconInformation);
        }

        // GET: BeaconInformations/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BeaconInformation beaconInformation = await db.BeaconInformations.FindAsync(id);
            if (beaconInformation == null)
            {
                return HttpNotFound();
            }
            return View(beaconInformation);
        }

        // POST: BeaconInformations/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Position,Latitude,Longitude,Floor,LaserPointerInformationId")] BeaconInformation beaconInformation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beaconInformation).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(beaconInformation);
        }

        // GET: BeaconInformations/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BeaconInformation beaconInformation = await db.BeaconInformations.FindAsync(id);
            if (beaconInformation == null)
            {
                return HttpNotFound();
            }
            return View(beaconInformation);
        }

        // POST: BeaconInformations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            BeaconInformation beaconInformation = await db.BeaconInformations.FindAsync(id);
            db.BeaconInformations.Remove(beaconInformation);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult Image(Guid id)
        {
            Bitmap Image = Barcode.QRcode(id.ToString());
            var bitmapBytes = Barcode.BitmapToBytes(Image);
            return File(bitmapBytes, "image/jpeg");
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
