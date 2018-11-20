using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using Lab28.Models;

namespace Lab28.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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
        public ActionResult CreateDeck()
        {
            string deck_id;
            HttpWebRequest request = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1");
            request.UserAgent = "Mozilla / 5.0(Windows NT 6.1; WOW64; rv: 64.0) Gecko / 20100101 Firefox / 64.0";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader rd = new StreamReader(response.GetResponseStream());
                string data = rd.ReadToEnd();
                rd.Close();
                response.Close();
                JObject shuffleJson = JObject.Parse(data);

                if (TempData["deck_id"] == null)
                {
                    TempData["deck_id"] = shuffleJson["deck_id"];
                    deck_id = shuffleJson["deck_id"].ToString();
                }
                else
                {
                    deck_id = TempData["deck_id"].ToString();
                }
                ViewBag.Deck = deck_id;
                return View("Index");
            }

            return View("Index");
        }
        public ActionResult DrawCards(string deck_id)
        {
            HttpWebRequest request = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/"+deck_id+"/draw/?count=5");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader rd = new StreamReader(response.GetResponseStream());
            string data = rd.ReadToEnd();
            rd.Close();
            response.Close();

            JObject shuffleJson = JObject.Parse(data);

            List<JToken> deck = shuffleJson["cards"].ToList();

            List<Card> myHand = new List<Card>();
            for (int i = 0; i < 5; i++)
            {
                if (i == deck.Count)
                {
                    break;
                }
                Card card = new Card();
                card.Image = deck[i]["image"].ToString();
                card.Value = deck[i]["value"].ToString();
                card.Suit = deck[i]["suit"].ToString();

                myHand.Add(card);
            }
            ViewBag.Deck = deck_id;
            return View("Index", myHand);
        }
    }
}