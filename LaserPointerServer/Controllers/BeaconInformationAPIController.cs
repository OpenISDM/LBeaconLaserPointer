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

        public string Get()
        {
            List<BeaconInformation> beacons = db.BeaconInformations.ToList();

            return JsonConvert.SerializeObject(beacons);
        }

        public string Post([FromBody]string value)
        {
            try
            {
                dynamic Json = JsonConvert.DeserializeObject(value);
                List<BeaconInformation> BeaconData;

                var UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                string UserId = Json["UserId"].ToString();
                string Token = Json["Token"].ToString();

                if (!UserManager.VerifyUserToken(UserId, "API", Token))
                {
                    return "Authentication failed";
                }

                BeaconData = JsonConvert.DeserializeObject<List<BeaconInformation>>(Json["Add"].ToString());
                if (BeaconData.Count > 0)
                    db.BeaconInformations.AddRange(BeaconData);

                BeaconData = JsonConvert.DeserializeObject<List<BeaconInformation>>(Json["Update"].ToString());
                if (BeaconData.Count > 0)
                    foreach(var q in BeaconData)
                        db.Entry(q).State = EntityState.Modified;

                BeaconData = JsonConvert.DeserializeObject<List<BeaconInformation>>(Json["Delete"].ToString());
                if (BeaconData.Count > 0)
                    db.BeaconInformations.RemoveRange(BeaconData);

                db.SaveChanges();
                return true.ToString();
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
