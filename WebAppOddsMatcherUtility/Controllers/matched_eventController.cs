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
        public ActionResult Index(int? page, string sortOrder, string betTypeSelector, string currentFilter, string searchByBookmaker, string searchByMarketType, string searchByBack, string searchByLay, string searchBySize, string filterSearchText, string stakeAmount)
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
            ViewBag.SizeFilter = searchByLay;
            ViewBag.SizeFilter = searchBySize; // TODO: confirm this is correct as searchByLay will never take effect because it will always be overwritten by this line!!
            ViewBag.filterSearchText = filterSearchText;

            ViewBag.betTypeSelector = String.IsNullOrEmpty(betTypeSelector) ? "optionQualifier" : betTypeSelector;

            ViewBag.stakeAmount = stakeAmount;

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
                           select s).Take(10000);

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
                    case "Back odds min 20":
                        backFilter = 20;
                        break;
                    case "Back odds min 10":
                        backFilter = 10;
                        break;
                    case "Back odds min 9":
                        backFilter = 9;
                        break;
                    case "Back odds min 8":
                        backFilter = 8;
                        break;
                    case "Back odds min 7":
                        backFilter = 7;
                        break;
                    case "Back odds min 6":
                        backFilter = 6;
                        break;
                    case "Back odds min 5":
                        backFilter = 5;
                        break;
                    case "Back odds min 4":
                        backFilter = 4;
                        break;
                    case "Back odds min 3":
                        backFilter = 3;
                        break;
                    case "Back odds min 2":
                        backFilter = 2;
                        break;
                    case "Back odds min 1":
                        backFilter = 1;
                        break;
                }
                matched = matched.Where(s => s.back >= backFilter);
            }
            if (!String.IsNullOrEmpty(searchByLay))
            {
                double LayFilter = 0;
                switch (searchByLay)
                {
                    case "Lay odds min 20":
                        LayFilter = 20;
                        break;
                    case "Lay odds min 10":
                        LayFilter = 10;
                        break;
                    case "Lay odds min 9":
                        LayFilter = 9;
                        break;
                    case "Lay odds min 8":
                        LayFilter = 8;
                        break;
                    case "Lay odds min 7":
                        LayFilter = 7;
                        break;
                    case "Lay odds min 6":
                        LayFilter = 6;
                        break;
                    case "Lay odds min 5":
                        LayFilter = 5;
                        break;
                    case "Lay odds min 4":
                        LayFilter = 4;
                        break;
                    case "Lay odds min 3":
                        LayFilter = 3;
                        break;
                    case "Lay odds min 2":
                        LayFilter = 2;
                        break;
                    case "Lay odds min 1":
                        LayFilter = 1;
                        break;
                }
                matched = matched.Where(s => s.lay >= LayFilter);
            }
            if (!String.IsNullOrEmpty(searchBySize))
            {
                double sizeFilter = 0;
                switch (searchBySize)
                {
                    case "Avail. min £2000":
                        sizeFilter = 2000;
                        break;
                    case "Avail. min £1000":
                        sizeFilter = 1000;
                        break;
                    case "Avail. min £500":
                        sizeFilter = 500;
                        break;
                    case "Avail. min £400":
                        sizeFilter = 400;
                        break;
                    case "Avail. min £300":
                        sizeFilter = 300;
                        break;
                    case "Avail. min £200":
                        sizeFilter = 200;
                        break;
                    case "Avail. min £100":
                        sizeFilter = 100;
                        break;
                    case "Avail. min £50":
                        sizeFilter = 50;
                        break;
                }
                matched = matched.Where(s => s.size >= sizeFilter);
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

            // Search Text filter
            if (!String.IsNullOrEmpty(filterSearchText))
            {

                matched = matched.Where(s => s.details.ToLower().Contains(filterSearchText.ToLower())
                        || s.competitionName.ToLower().Contains(filterSearchText.ToLower())
                        || s.betName.ToLower().Contains(filterSearchText.ToLower()));

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


            // Build list of Back odds mins for filters
            var backs = new List<string>();
            backs.Add("Back odds min 20");
            backs.Add("Back odds min 10");
            backs.Add("Back odds min 9");
            backs.Add("Back odds min 8");
            backs.Add("Back odds min 7");
            backs.Add("Back odds min 6");
            backs.Add("Back odds min 5");
            backs.Add("Back odds min 4");
            backs.Add("Back odds min 3");
            backs.Add("Back odds min 2");
            backs.Add("Back odds min 1");
            ViewBag.SearchByBack = new SelectList(backs);


            // Build list of Lay odds mins for filters
            var lays = new List<string>();
            lays.Add("Lay odds min 20");
            lays.Add("Lay odds min 10");
            lays.Add("Lay odds min 9");
            lays.Add("Lay odds min 8");
            lays.Add("Lay odds min 7");
            lays.Add("Lay odds min 6");
            lays.Add("Lay odds min 5");
            lays.Add("Lay odds min 4");
            lays.Add("Lay odds min 3");
            lays.Add("Lay odds min 2");
            lays.Add("Lay odds min 1");
            ViewBag.SearchByLay = new SelectList(lays);


            // Build list of Size's for filters
            var sizes = new List<string>();
            sizes.Add("Avail. min £2000");
            sizes.Add("Avail. min £1000");
            sizes.Add("Avail. min £500");
            sizes.Add("Avail. min £400");
            sizes.Add("Avail. min £300");
            sizes.Add("Avail. min £200");
            sizes.Add("Avail. min £100");
            sizes.Add("Avail. min £50");
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
