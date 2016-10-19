using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tweetinvi;

namespace TwitterSD.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            //string aaaa = GetTweets(30);
            //var x = HomeTimelineAsync().Result;
            // Set up your credentials (https://apps.twitter.com)
            Auth.SetUserCredentials("Wv1B17cYiPwMp3x5cqq8YC9h1", "PdUfX3YAY0fO7wO9wlwdf6ZMZRq6bGfQAIfJDgAo1muqY6KtEL", "1014538885-tWPygR1Cl7UrWAPYe40JRGgjUGxVmVRupXO0x5y", "lWyEwpOcpDuuCfnULxK4naJflmeognhELjz3QsMTJ1XIE");

            // Publish the Tweet "Hello World" on your Timeline
            var tweet = Tweetinvi.Search.SearchTweets("quake&locale=california");
            List<Tweetinvi.Models.ITweet> isCor = new List<Tweetinvi.Models.ITweet>();
            if (tweet != null)
            {
                isCor = tweet.Where(x => x.Coordinates != null).ToList();

            }
            else
            {
                ViewData["fecha"] = "No se pudieron cargar los tweets";

            }
            ViewData["fecha"] = DateTime.Now.Date;

            return View(isCor);
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