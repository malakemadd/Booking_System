using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace MVCBookingFinal_YARAB_.Controllers
{
    public class ReservationController(IRoomService roomService,IHotelService hotelservice) : Controller
    {
        
        // GET: ReservationController
        public ActionResult Index()
        {
            return View();
        }
        public IActionResult ViewHotels(SearchViewModel vm,int pagenum=0)
        {
            if(vm.CheckInDate>vm.CheckOutDate)
            {
                return RedirectToAction("index", "home");
            }
            ViewBag.PageNum = pagenum;
            TempData["myviewmodel"] = JsonConvert.SerializeObject(vm);
			var hotelsquery = hotelservice.GetAllFilteredPaginated(PerPage: 10,
                pagenum: pagenum,
                city: vm.CountryOrCity,
                country: vm.CountryOrCity,
                vm:vm
                );


			return View(hotelsquery);
        }
        public IActionResult nextPage(int pagenum)
        {
            SearchViewModel myvm= JsonConvert.DeserializeObject<SearchViewModel>(TempData["myviewmodel"].ToString());
			ViewBag.PageNum = pagenum;
			var hotelsquery = hotelservice.GetAllFilteredPaginated(PerPage: 10,
				pagenum: pagenum,
				city: myvm.CountryOrCity,
				country: myvm.CountryOrCity,
				vm: myvm
				);
			
			TempData["myviewmodel"] = JsonConvert.SerializeObject(myvm);
			return View("ViewHotels", hotelsquery);		
        }
		public IActionResult PicturePress(string countryorcity)
        {
			
			var hotelsquery=hotelservice.GetAllFilteredPaginated(new SearchViewModel()
            {
                AdultsNumber=0,
                ChildrenNumber=0,
                CheckInDate=DateTime.Now.AddYears(99),
                CheckOutDate=DateTime.Now.AddYears(199),
                CountryOrCity=null,
                NumberOfRooms=0,

            },
                PerPage:10,
				pagenum: 0,
				city: countryorcity,
				country: countryorcity
				);
            return View("ViewRooms", hotelsquery.Take(5));
		}
        public IActionResult GoToHotel(int id,RoomFilterationViewModel vm=null,bool checkfilteration=false)
        {
                

			SearchViewModel myvm = JsonConvert.DeserializeObject<SearchViewModel>(TempData["myviewmodel"].ToString());
            var result = roomService.GetCombinationsByHotelId(id, myvm);
            ViewBag.AllHotels = result;
			//ViewBag.CanMatchAmenties = true;
			TempData["myviewmodel"] = JsonConvert.SerializeObject(myvm);
            if(!checkfilteration)
            {

			    //var serializedCombinations = JsonConvert.SerializeObject(result);

	
			    //ViewBag.SerializedCombinations = serializedCombinations;
				ViewBag.HasCombinations = true;
				return View(result);

            }
			else
			{
                var query=result.Where(combination => (combination.Sum(r => r.PricePerNight) >= vm.minPrice) && combination.Sum(r => r.PricePerNight) <= vm.maxPrice);
                IEnumerable<List<Room>> query2;
				switch (vm.Sorting)
                {
                    case sortBy.LowPrice:
                        query2 = query.OrderBy(combination => combination.Sum(r => r.PricePerNight));
                        break;
					case sortBy.HighPrice:
						query2 = query.OrderByDescending(combination => combination.Sum(r => r.PricePerNight));
						break;
                    case sortBy.OurTopPicks:
                        query2 = query;
                        break;
                    default:
						query2 = query;
                        break;

				}

				//var serializedCombinations = JsonConvert.SerializeObject(query2);


				//ViewBag.SerializedCombinations = serializedCombinations;
				if (query2.Count()==0)
                {
                    ViewBag.HasCombinations = false;
                }
                else
                {
                    ViewBag.HasCombinations = true;
                }
				return View(query2);

            }
        }

        // GET: ReservationController/Details/5
        [Authorize]
		public ActionResult Reserve(string RoomsCombination)
        {

			List<Room>rooms=JsonConvert.DeserializeObject<List<Room>>(RoomsCombination);

			return View();
        }

        // GET: ReservationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReservationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReservationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReservationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReservationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReservationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
