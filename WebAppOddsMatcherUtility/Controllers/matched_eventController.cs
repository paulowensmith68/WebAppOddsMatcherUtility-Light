using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebAppOddsMatcherUtility.Models;
using PagedList;


namespace WebAppOddsMatcherUtility.Controllers
{
    public class matched_eventController : Controller
    {
        private oddsmatchingEntities db = new oddsmatchingEntities();

        // GET: matched_event
        public ActionResult Index(int? page, string sortOrder, string currentFilter, string searchByBookmaker, string searchByMarketType, string searchByBack, string searchBySize, string[] filterSport, string[] filterBookmaker, string[] filterMarket, string[] filterExchange, string filterMinRating, string filterMaxRating, string filterMinOdds, string filterMaxOdds, string filterMinAvail, string filterTimeLimit, string filterSearchText)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.DetailsSortParm = sortOrder == "details" ? "details_desc" : "details";
            ViewBag.RatingSortParm = sortOrder == "rating" ? "rating_desc" : "rating";
            ViewBag.BetSortParm = sortOrder == "bet" ? "bet_desc" : "bet";
            ViewBag.MarketSortParm = sortOrder == "market" ? "market_desc" : "market";
            ViewBag.BookmakerSortParm = sortOrder == "bookmaker" ? "bookmaker_desc" : "bookmaker";
            ViewBag.BackSortParm = sortOrder == "back" ? "back_desc" : "back";
            ViewBag.ExchangeSortParm = sortOrder == "exchange" ? "exchange_desc" : "exchange";
            ViewBag.LaySortParm = sortOrder == "lay" ? "lay_desc" : "lay";
            ViewBag.SizeSortParm = sortOrder == "size" ? "size_desc" : "size";

            ViewBag.CurrentFilter = searchByBookmaker;
            ViewBag.MarketTypeFilter = searchByMarketType;
            ViewBag.BackFilter = searchByBack;
            ViewBag.SizeFilter = searchBySize;
            ViewBag.filterSport = filterSport;
            ViewBag.filterBookmaker = filterBookmaker;
            ViewBag.filterMarket = filterMarket;
            ViewBag.filterExchange = filterExchange;
            ViewBag.filterMinRating = filterMinRating;
            ViewBag.filterMaxRating = filterMaxRating;
            ViewBag.filterMinOdds = filterMinOdds;
            ViewBag.filterMaxOdds = filterMaxOdds;
            ViewBag.filterMinAvail = filterMinAvail;
            ViewBag.filterTimeLimit = filterTimeLimit;
            ViewBag.filterSearchText = filterSearchText;

            if (searchByBookmaker == null)
            {
                searchByBookmaker = currentFilter;
            }

            // Populate filter lists
            SetCollectionForFilter();

            ViewBag.CurrentFilter = searchByBookmaker;
            ViewBag.BookmakerFilter = searchByBookmaker;

            var matched = (from s in db.matched_event
                           orderby s.rating descending
                           select s).Take(3000);

            //
            // Filter
            //
            // Bookmaker filter
            if (!String.IsNullOrEmpty(searchByBookmaker))
            {
                matched = matched.Where(s => s.bookmaker_name.Contains(searchByBookmaker));
            }
            // Market Type filter
            if (!String.IsNullOrEmpty(searchByMarketType))
            {
                matched = matched.Where(s => s.marketName.Contains(searchByMarketType));
            }
            if (!String.IsNullOrEmpty(searchByBack))
            {
                double backFilter = 0;
                switch (searchByBack)
                {
                    case "Back bet 4+":
                        backFilter = 4;
                        break;
                    case "Back bet 3+":
                        backFilter = 3;
                        break;
                    case "Back bet 2+":
                        backFilter = 2;
                        break;
                    case "Back bet 1+":
                        backFilter = 1;
                        break;
                }
                matched = matched.Where(s => s.back >= backFilter);
            }
            if (!String.IsNullOrEmpty(searchBySize))
            {
                double sizeFilter = 0;
                switch (searchBySize)
                {
                    case "Size £2000+":
                        sizeFilter = 2000;
                        break;
                    case "Size £1000+":
                        sizeFilter = 1000;
                        break;
                    case "Size £500+":
                        sizeFilter = 500;
                        break;
                    case "Size £400+":
                        sizeFilter = 400;
                        break;
                    case "Size £300+":
                        sizeFilter = 300;
                        break;
                    case "Size £200+":
                        sizeFilter = 200;
                        break;
                    case "Size £100+":
                        sizeFilter = 100;
                        break;
                    case "Size £50+":
                        sizeFilter = 50;
                        break;
                }
                matched = matched.Where(s => s.size >= sizeFilter);
            }

            // Filter - Sport
            if (filterSport != null)
            {
                foreach(string sport in filterSport)
                {
                    string filterString = HttpUtility.UrlDecode(sport).ToLower();
                    matched = matched.Where(s => s.sport.Contains(filterString));
                }
            }

            //if (!String.IsNullOrEmpty(filterSport))
            //{
            //   if (filterSport.IndexOf(",") > 0)
            //    {
            //        char[] delimiters = new char[] { ',' };
            //        string[] SportsNames = filterSport.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            //        foreach (string sport in SportsNames)
            //        {
            //            matched = matched.Where(s => s.sport.Contains(sport.ToLower()));
            //        }
            //    }
            //    else
            //    {
            //        matched = matched.Where(s => s.sport.Contains(filterSport.ToLower()));
            //    }
            //}

            // Filter - Bookmakers
            if (filterBookmaker != null && filterBookmaker.Length > 0)
            {
                foreach (string bookmaker in filterBookmaker)
                {
                    string filterString = HttpUtility.UrlDecode(bookmaker).ToLower();
                    matched = matched.Where(s => s.bookmaker_name.Contains(filterString));
                }
            }

            //if (!String.IsNullOrEmpty(filterBookmaker))
            //{
            //    if (filterBookmaker.IndexOf(",") > 0)
            //    {
            //        char[] delimiters = new char[] { ',' };
            //        string[] BookmakerNames = filterBookmaker.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            //        foreach (string bookmaker in BookmakerNames)
            //        {
            //            matched = matched.Where(s => s.bookmaker.Contains(bookmaker));
            //        }
            //    }
            //    else
            //    {
            //        matched = matched.Where(s => s.bookmaker.Contains(filterBookmaker));
            //    }
            //}

            // Filter - Market Types
            if (filterMarket != null && filterMarket.Length > 0)
            {
                foreach (string market in filterMarket)
                {
                    string filterString = HttpUtility.UrlDecode(market).ToLower();
                    matched = matched.Where(s => s.marketName.Contains(filterString));
                }
            }
            //if (!String.IsNullOrEmpty(filterMarket))
            //{
            //    if (filterMarket.IndexOf(",") > 0)
            //   {
            //        char[] delimiters = new char[] { ',' };
            //        string[] MarketNames = filterMarket.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            //        foreach (string market in MarketNames)
            //        {
            //            matched = matched.Where(s => s.marketName.Contains(market));
            //        }
            //    }
            //    else
            //    {
            //        matched = matched.Where(s => s.marketName.Contains(filterMarket));
            //    }
            //}


            // Filter - Exchange
            if (filterExchange != null && filterExchange.Length > 0)
            {
                foreach (string exchange in filterExchange)
                {
                    string filterString = HttpUtility.UrlDecode(exchange).ToLower();
                    matched = matched.Where(s => s.marketName.Contains(filterString));
                }
            }

            //if (!String.IsNullOrEmpty(filterExchange))
            //{
            //    if (filterExchange.IndexOf(",") > 0)
            //    {
            //        char[] delimiters = new char[] { ',' };
            //        string[] ExchangeNames = filterExchange.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            //        foreach (string exchange in ExchangeNames)
            //        {
            //            matched = matched.Where(s => s.marketName.Contains(exchange.ToLower()));
            //       }
            //    }
            //    else
            //    {
            //        matched = matched.Where(s => s.exchange.Contains(filterExchange.ToLower()));
            //   }
            //}


            // Filter MinRating
            if (!String.IsNullOrEmpty(filterMinRating))
            {
                string strValue;
                double dblNumber;
                strValue = filterMinRating;
                if (Double.TryParse(strValue, out dblNumber))
                {
                    matched = matched.Where(s => s.rating >= dblNumber);
                }
            }


            // Filter MaxRating
            if (!String.IsNullOrEmpty(filterMaxRating))
            {
                string strValue;
                double dblNumber;
                strValue = filterMaxRating;
                if (Double.TryParse(strValue, out dblNumber))
                {
                    matched = matched.Where(s => s.rating <= dblNumber);
                }
            }


            // Filter MinOdds
            if (!String.IsNullOrEmpty(filterMinOdds))
            {
                string strValue;
                double dblNumber;
                strValue = filterMinOdds;
                if (Double.TryParse(strValue, out dblNumber))
                {
                    matched = matched.Where(s => s.back >= dblNumber);
                }
            }


            // Filter MaxOdds
            if (!String.IsNullOrEmpty(filterMaxOdds))
            {
                string strValue;
                double dblNumber;
                strValue = filterMaxOdds;
                if (Double.TryParse(strValue, out dblNumber))
                {
                    matched = matched.Where(s => s.back <= dblNumber);
                }
            }


            // Filter MinAvail
            if (!String.IsNullOrEmpty(filterMinAvail))
            {
                string strValue;
                double dblNumber;
                strValue = filterMinAvail;
                if (Double.TryParse(strValue, out dblNumber))
                {
                    matched = matched.Where(s => s.size >= dblNumber);
                }
            }


            //
            // Sort
            //
            switch (sortOrder)
            {
                case "Date":
                    matched = matched.OrderBy(s => s.eventDate);
                    break;
                case "date_desc":
                    matched = matched.OrderByDescending(s => s.eventDate);
                    break;
                case "rating":
                    matched = matched.OrderBy(s => s.rating);
                    break;
                case "rating_desc":
                    matched = matched.OrderByDescending(s => s.rating);
                    break;
                case "details":
                    matched = matched.OrderBy(s => s.details);
                    break;
                case "details_desc":
                    matched = matched.OrderByDescending(s => s.details);
                    break;
                case "bet":
                    matched = matched.OrderBy(s => s.betName);
                    break;
                case "bet_desc":
                    matched = matched.OrderByDescending(s => s.betName);
                    break;
                case "market":
                    matched = matched.OrderBy(s => s.marketName);
                    break;
                case "market_desc":
                    matched = matched.OrderByDescending(s => s.marketName);
                    break;
                case "bookmaker":
                    matched = matched.OrderBy(s => s.bookmaker);
                    break;
                case "bookmaker_desc":
                    matched = matched.OrderByDescending(s => s.bookmaker);
                    break;
                case "back":
                    matched = matched.OrderBy(s => s.back);
                    break;
                case "back_desc":
                    matched = matched.OrderByDescending(s => s.back);
                    break;
                case "exchange":
                    matched = matched.OrderBy(s => s.exchange);
                    break;
                case "exchange_desc":
                    matched = matched.OrderByDescending(s => s.exchange);
                    break;
                case "lay":
                    matched = matched.OrderBy(s => s.lay);
                    break;
                case "lay_desc":
                    matched = matched.OrderByDescending(s => s.lay);
                    break;
                case "size":
                    matched = matched.OrderBy(s => s.size);
                    break;
                case "size_desc":
                    matched = matched.OrderByDescending(s => s.size);
                    break;
                default:
                    matched = matched.OrderByDescending(s => s.rating);
                    break;
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(matched.ToPagedList(pageNumber, pageSize));
        }

        // GET: matched_event/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            matched_event matched_event = db.matched_event.Find(id);
            if (matched_event == null)
            {
                return HttpNotFound();
            }
            return View(matched_event);
        }

        // GET: matched_event/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: matched_event/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,eventDate,sport,details,betName,marketName,rating,info,bookmaker,back,exchange,lay,size,betfairEventTypeId,betfairMarketTypeCode,competitionName,countryCode,timezone,n,ut,del")] matched_event matched_event)
        {
            if (ModelState.IsValid)
            {
                db.matched_event.Add(matched_event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(matched_event);
        }

        // GET: matched_event/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            matched_event matched_event = db.matched_event.Find(id);
            if (matched_event == null)
            {
                return HttpNotFound();
            }
            return View(matched_event);
        }

        // POST: matched_event/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,eventDate,sport,details,betName,marketName,rating,info,bookmaker,back,exchange,lay,size,betfairEventTypeId,betfairMarketTypeCode,competitionName,countryCode,timezone,n,ut,del")] matched_event matched_event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(matched_event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(matched_event);
        }

        // GET: matched_event/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            matched_event matched_event = db.matched_event.Find(id);
            if (matched_event == null)
            {
                return HttpNotFound();
            }
            return View(matched_event);
        }

        // POST: matched_event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            matched_event matched_event = db.matched_event.Find(id);
            db.matched_event.Remove(matched_event);
            db.SaveChanges();
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

        private void SetCollectionForFilter()
        {
            // Build list of Bookmakers for filters
            var bookmakers = new List<string>();
            var bookmakerQuery = from s in db.matched_event
                                 orderby s.bookmaker_name
                                 select s.bookmaker_name;
            bookmakers.AddRange(bookmakerQuery.Distinct());
            bookmakers.Sort();

            ViewBag.FilterByBookie = bookmakers;
            ViewBag.SearchByBookmaker = new SelectList(bookmakers);


            // Build list of Back bets for filters
            var backs = new List<string>();
            backs.Add("Back bet 4+");
            backs.Add("Back bet 3+");
            backs.Add("Back bet 2+");
            backs.Add("Back bet 1+");
            ViewBag.SearchByBack = new SelectList(backs);


            // Build list of Size's for filters
            var sizes = new List<string>();
            sizes.Add("Size £2000+");
            sizes.Add("Size £1000+");
            sizes.Add("Size £500+");
            sizes.Add("Size £400+");
            sizes.Add("Size £300+");
            sizes.Add("Size £200+");
            sizes.Add("Size £100+");
            sizes.Add("Size £50+");
            ViewBag.SearchBySize = new SelectList(sizes);


            // Build list of Market Types for filters
            var marketTypes = new List<string>();
            var marketTypeQuery = from s in db.matched_event
                                  orderby s.marketName
                                  select s.marketName;
            marketTypes.AddRange(marketTypeQuery.Distinct());
            marketTypes.Sort();

            ViewBag.FilterByMarketType = marketTypes;
            ViewBag.SearchByMarketType = new SelectList(marketTypes);

        }

    }

}
