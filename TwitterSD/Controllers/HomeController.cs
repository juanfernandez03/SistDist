using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.Serialization;
using Tweetinvi;
using Tweetinvi.Credentials.Models;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using TwitterSD.Models;

namespace TwitterSD.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string endDate)
        {
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToString("MM/dd/yyyy");
            }
            DateTime dt_endDate;
            DateTime.TryParseExact(endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt_endDate);
            DateTime dt_startDate = dt_endDate.AddDays(-7);
            string startDate = dt_startDate.ToString("MM/dd/yyyy");
            // Set up your credentials (https://apps.twitter.com)
            Auth.SetUserCredentials("Wv1B17cYiPwMp3x5cqq8YC9h1", "PdUfX3YAY0fO7wO9wlwdf6ZMZRq6bGfQAIfJDgAo1muqY6KtEL", "1014538885-tWPygR1Cl7UrWAPYe40JRGgjUGxVmVRupXO0x5y", "lWyEwpOcpDuuCfnULxK4naJflmeognhELjz3QsMTJ1XIE");
            // string query = "earthquake 02/27/2017" + dt_endDate.ToString("yyyy-MM-dd");
            string query = "earthquake " +dt_endDate.ToString("yyyy-MM-dd");
            //string query = "quake";
            var tweet = Tweetinvi.Search.SearchTweets(query);
            List<Tweetinvi.Models.ITweet> isCor = new List<Tweetinvi.Models.ITweet>();
            List<Tweetinvi.Models.IUser> listUserInfo = new List<Tweetinvi.Models.IUser>();
            if (tweet != null)
            {
                isCor = tweet.Where(x => x.Coordinates != null).ToList();
                foreach (var item in isCor)
                {
                    listUserInfo.Add(Tweetinvi.User.GetUserFromId(item.CreatedBy.Id));
                }
            }
            else
            {
                ViewData["fecha"] = "No se pudieron cargar los tweets";
            }
            ViewBag.userInfo = listUserInfo;
            ViewBag.kmlQuery = "http://earthquake.usgs.gov/fdsnws/event/1/query?format=kml&starttime=" + dt_startDate.ToString("yyyy-MM-dd") + "&endtime=" + dt_endDate.ToString("yyyy-MM-dd") + "&minmagnitude=5";

            return View(isCor);
        }

        string GET(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (System.IO.Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (System.IO.Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }
        public ActionResult ViewProfile(int id)
        {
            Tweetinvi.Models.IUser user = Tweetinvi.User.GetUserFromId(Convert.ToInt64(id));

            ViewBag.userInfo = user;
            return View();
        }
        private IAuthenticationContext _authenticationContext;
        public ConsumerCredentials appCreds;
        // Step 1 : Redirect user to go on Twitter.com to authenticate
        public ActionResult TwitterAuth()
        {
            appCreds = new ConsumerCredentials("Wv1B17cYiPwMp3x5cqq8YC9h1", "PdUfX3YAY0fO7wO9wlwdf6ZMZRq6bGfQAIfJDgAo1muqY6KtEL");

            // Specify the url you want the user to be redirected to
            var redirectURL = "http://" + Request.Url.Authority + "/Home/ValidateTwitterAuth";
            _authenticationContext = AuthFlow.InitAuthentication(appCreds, redirectURL);

            return new RedirectResult(_authenticationContext.AuthorizationURL);
        }
        public ActionResult ValidateTwitterAuth()
        {
            // Get some information back from the URL
            var verifierCode = Request.Params.Get("oauth_verifier");


            var token = new AuthenticationToken()
            {
                AuthorizationKey = "Wv1B17cYiPwMp3x5cqq8YC9h1",
                AuthorizationSecret = "PdUfX3YAY0fO7wO9wlwdf6ZMZRq6bGfQAIfJDgAo1muqY6KtEL",
                ConsumerCredentials = appCreds

            };

            // And then instead of passing the AuthenticationContext, just pass the AuthenticationToken
            var userCreds = AuthFlow.CreateCredentialsFromVerifierCode(verifierCode, token);
            // Create the user credentials
            //var userCreds = AuthFlow.CreateCredentialsFromVerifierCode(verifierCode, _authenticationContext);

            // Do whatever you want with the user now!
            var user = Tweetinvi.User.GetAuthenticatedUser(userCreds);
            this.Session["ProfileData"] = user;
        
            ViewBag.user = user.ScreenName;
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            try
            {
                if (this.Session["ProfileData"] != null)
                {
                    this.Session["ProfileData"] = null;

                }
               
                return Redirect("/Home/Index");


            }
            catch
            {
                return RedirectToAction("Index");
            }
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
    }
}