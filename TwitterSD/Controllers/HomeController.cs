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
            //string aaaa = GetTweets(30);
            //var x = HomeTimelineAsync().Result;
            // Set up your credentials (https://apps.twitter.com)
            Auth.SetUserCredentials("Wv1B17cYiPwMp3x5cqq8YC9h1", "PdUfX3YAY0fO7wO9wlwdf6ZMZRq6bGfQAIfJDgAo1muqY6KtEL", "1014538885-tWPygR1Cl7UrWAPYe40JRGgjUGxVmVRupXO0x5y", "lWyEwpOcpDuuCfnULxK4naJflmeognhELjz3QsMTJ1XIE");
            //string query = "quake OR Tremblement de terre OR terremoto OR sismo OR earthquake &until=" + dt_startDate.ToString("yyyy-MM-dd");
            string query = "quake";
            // Publish the Tweet "Hello World" on your Timeline
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
            ViewBag.user = user.ScreenName;
            return RedirectToAction("Index");
        }
        // public async Task<ActionResult> BeginAsync()
        //{
        //    //var auth = new MvcSignInAuthorizer
        //    var auth = new MvcAuthorizer
        //    {
        //        CredentialStore = new SessionStateCredentialStore
        //        {
        //            ConsumerKey = ConfigurationManager.AppSettings["Wv1B17cYiPwMp3x5cqq8YC9h1"],
        //            ConsumerSecret = ConfigurationManager.AppSettings["PdUfX3YAY0fO7wO9wlwdf6ZMZRq6bGfQAIfJDgAo1muqY6KtEL"]
        //        }
        //    };
        //    string twitterCallbackUrl = Request.Url.ToString().Replace("Begin", "Complete");
        //    return await auth.BeginAuthorizationAsync(new Uri(twitterCallbackUrl));
        //}
        // public async Task<ActionResult> CompleteAsync()
        // {
        //var auth = new MvcAuthorizer
        //{
        //    CredentialStore = new SessionStateCredentialStore()
        //};

        //await auth.CompleteAuthorizeAsync(Request.Url);

        // This is how you access credentials after authorization.
        // The oauthToken and oauthTokenSecret do not expire.
        // You can use the userID to associate the credentials with the user.
        // You can save credentials any way you want - database, 
        //   isolated storage, etc. - it's up to you.
        // You can retrieve and load all 4 credentials on subsequent 
        //   queries to avoid the need to re-authorize.
        // When you've loaded all 4 credentials, LINQ to Twitter will let 
        //   you make queries without re-authorizing.
        //
        //    var credentials = auth.CredentialStore;
        //    string oauthToken = credentials.OAuthToken;
        //    string oauthTokenSecret = credentials.OAuthTokenSecret;
        //    string screenName = credentials.ScreenName;
        //    ulong userID = credentials.UserID;
        //    //

        //    return RedirectToAction("Index", "Home");
        //}
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //public async Task<ActionResult> HomeTimelineAsync()
        //{
        //    var auth = new MvcAuthorizer
        //    {
        //        CredentialStore = new SessionStateCredentialStore()
        //    };
        //    var ctx = new TwitterContext(auth);

        //    var tweets = ctx.Status.Where(x=>x.Type == StatusType.Home).FirstOrDefault();



        //    return View(tweets);
        //}

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //public class Tweet
        //{
        //    public string Username { get; set; }
        //    public string FullName { get; set; }
        //    public string Text { get; set; }
        //    public string FormattedText { get; set; }
        //}

        //public static string GetTweets(int max)
        //{
        //    // get the auth
        //    var auth = new SingleUserAuthorizer
        //    {
        //        CredentialStore = new SingleUserInMemoryCredentialStore
        //        {
        //            ConsumerKey = ConfigurationManager.AppSettings["Wv1B17cYiPwMp3x5cqq8YC9h1"],
        //            ConsumerSecret = ConfigurationManager.AppSettings["PdUfX3YAY0fO7wO9wlwdf6ZMZRq6bGfQAIfJDgAo1muqY6KtEL"],
        //            AccessToken = ConfigurationManager.AppSettings["1014538885-tWPygR1Cl7UrWAPYe40JRGgjUGxVmVRupXO0x5y"],
        //            AccessTokenSecret = ConfigurationManager.AppSettings["lWyEwpOcpDuuCfnULxK4naJflmeognhELjz3QsMTJ1XIE"]
        //        }
        //    };
        //    // get the context, query the last status
        //    var context = new TwitterContext(auth);
        //    var tweets =
        //        from tw in context.Status
        //        where
        //            tw.Type == StatusType.User &&
        //            tw.ScreenName == "JuanFernandez03"
        //        select tw;
        //    // handle exceptions, twitter service might be down
        //    try
        //    {
        //        return tweets
        //    .Take(max)
        //    .Select(t =>
        //        new Tweet
        //        {
        //            Username = t.ScreenName,
        //            FullName = t.User.Name,
        //            Text = t.Text,
        //            FormattedText = ParseTweet(t.Text)
        //        })
        //    .ToString();

        //    }
        //    catch (Exception) { }
        //    return tweets.ToString();
        //}
        //public static string ParseTweet(string rawTweet)
        //{
        //    Regex link = new Regex(@"http(s)?://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&amp;\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?");
        //    Regex screenName = new Regex(@"@\w+");
        //    Regex hashTag = new Regex(@"#\w+");

        //    string formattedTweet = link.Replace(rawTweet, delegate(Match m)
        //    {
        //        string val = m.Value;
        //        return "<a href='" + val + "'>" + val + "</a>";
        //    });

        //    formattedTweet = screenName.Replace(formattedTweet, delegate(Match m)
        //    {
        //        string val = m.Value.Trim('@');
        //        return string.Format("@<a href='http://twitter.com/{0}'>{1}</a>", val, val);
        //    });

        //    formattedTweet = hashTag.Replace(formattedTweet, delegate(Match m)
        //    {
        //        string val = m.Value;
        //        return string.Format("<a href='http://twitter.com/#search?q=%23{0}'>{1}</a>", val, val);
        //    });

        //    return formattedTweet;
        //}
        //async Task<string> AccessTheWebAsync()
        //{
        //    var auth = new SingleUserAuthorizer
        //    {
        //        CredentialStore = new SingleUserInMemoryCredentialStore
        //        {
        //            ConsumerKey = ConfigurationManager.AppSettings["Wv1B17cYiPwMp3x5cqq8YC9h1"],
        //            ConsumerSecret = ConfigurationManager.AppSettings["PdUfX3YAY0fO7wO9wlwdf6ZMZRq6bGfQAIfJDgAo1muqY6KtEL"],
        //            AccessToken = ConfigurationManager.AppSettings["1014538885-tWPygR1Cl7UrWAPYe40JRGgjUGxVmVRupXO0x5y"],
        //            AccessTokenSecret = ConfigurationManager.AppSettings["lWyEwpOcpDuuCfnULxK4naJflmeognhELjz3QsMTJ1XIE"]
        //        }
        //    };
        //    var twitterCtx = new TwitterContext(auth);
        //    var tweets =
        //       from tweet in twitterCtx.Status
        //       where tweet.Type == StatusType.Home
        //       select tweet;

        //    Console.WriteLine("\nTweets for " + twitterCtx.User.FirstOrDefault().ScreenNameResponse + "\n");
        //    List<string> s = new List<string>();

        //    var tweets3 =

        //       (from tweet in twitterCtx.Status
        //        where tweet.Type == StatusType.Home
        //        select tweet)
        //       .ToList();
        //    var searchResponse =
        //           await
        //           (from search in twitterCtx.Search
        //            where search.Type == SearchType.Search &&
        //                  search.Query == "mendoza"
        //            select search)
        //           .SingleOrDefaultAsync();

        //    if (searchResponse != null && searchResponse.Statuses != null)
        //        searchResponse.Statuses.ForEach(tweet =>
        //            Console.WriteLine(
        //                "User: {0}, Tweet: {1}",
        //                tweet.User.ScreenNameResponse,
        //                tweet.Text));
        //    var res =  searchResponse.Statuses.All(tweet => tweet.Coordinates != null);

        //    return res.ToString();
        //}
    }
}