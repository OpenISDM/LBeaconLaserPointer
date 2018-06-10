using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IdentitySample.Models;
using LBeacon.Models;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using System.Web;

namespace LBeacon.Controllers
{
    public class BeaconInformationAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Get: /api/BeaconInformationAPI
        public string Get()
        {
            List<BeaconInformation> beacons = db.BeaconInformations.ToList();
            List<LaserPointerInformation> pointerDatas = db.LaserPointerInformations.ToList();

            return JsonConvert.SerializeObject(new {
                BeaconInformation = beacons,
                LaserPointerInformation = pointerDatas
            });
        }

        public string Post([FromBody]string value)
        {
            try
            {
                dynamic Json = JsonConvert.DeserializeObject(value);
                List<BeaconInformation> BeaconData;
                List<LaserPointerInformation> LaserPointerData;

                var UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                string UserId = Json["UserId"].ToString();
                string Token = Json["UserToken"].ToString();

                if (!UserManager.VerifyUserToken(UserId, "API", Token))
                {
                    return "Authentication failed";
                }



                if (Json["Table"].ToString() == "BeaconInformation")
                {
                    switch (Json["Action"].ToString())
                    {
                        case "Add":
                            BeaconData = JsonConvert.DeserializeObject<List<BeaconInformation>>(Json["Data"].ToString());
                            if (BeaconData.Count > 0)
                                db.BeaconInformations.AddRange(BeaconData);

                            db.SaveChanges();
                            return true.ToString();
                        case "Update":
                            BeaconData = JsonConvert.DeserializeObject<List<BeaconInformation>>(Json["Data"].ToString());
                            if (BeaconData.Count > 0)
                                foreach (var q in BeaconData)
                                    db.Entry(q).State = EntityState.Modified;

                            db.SaveChanges();
                            return true.ToString();
                        case "Delete":
                            BeaconData = JsonConvert.DeserializeObject<List<BeaconInformation>>(Json["Delete"].ToString());
                            if (BeaconData.Count > 0)
                                db.BeaconInformations.RemoveRange(BeaconData);


                            db.SaveChanges();
                            return true.ToString();
                    }
                }

                if (Json["Table"].ToString() == "LaserPointerInformation")
                {
                    switch (Json["Action"].ToString())
                    {
                        case "Add":
                            LaserPointerData = JsonConvert.DeserializeObject<List<LaserPointerInformation>>(Json["Data"].ToString());
                            if (LaserPointerData.Count > 0)
                                db.LaserPointerInformations.AddRange(LaserPointerData);

                            db.SaveChanges();
                            return true.ToString();
                        case "Update":
                            LaserPointerData = JsonConvert.DeserializeObject<List<LaserPointerInformation>>(Json["Data"].ToString());
                            if (LaserPointerData.Count > 0)
                                foreach (var q in LaserPointerData)
                                    db.Entry(q).State = EntityState.Modified;

                            db.SaveChanges();
                            return true.ToString();
                        case "Delete":
                            LaserPointerData = JsonConvert.DeserializeObject<List<LaserPointerInformation>>(Json["Delete"].ToString());
                            if (LaserPointerData.Count > 0)
                                db.LaserPointerInformations.RemoveRange(LaserPointerData);


                            db.SaveChanges();
                            return true.ToString();
                    }
                }

                return false.ToString();
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
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
