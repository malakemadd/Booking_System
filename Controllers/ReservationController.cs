﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using MVCBookingFinal_YARAB_.IRepositories;
using MVCBookingFinal_YARAB_.Models;
using MVCBookingFinal_YARAB_.Repositories;
using Newtonsoft.Json;
using Stripe;
using System.Security.Claims;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MVCBookingFinal_YARAB_.Controllers
{
    public class ReservationController(IRoomService roomService,IHotelService hotelservice,UserManager<AppUser> usermanager,IReservationService reservationservice) : Controller
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
			ViewBag.Hotel = result.FirstOrDefault().FirstOrDefault().Hotel;
			//ViewBag.CanMatchAmenties = true;
			TempData["myviewmodel"] = JsonConvert.SerializeObject(myvm);
            if(!checkfilteration)
            {

				ViewBag.HasCombinations = true;
				return View(result);

            }
			else
			{
                var query=result.Where(combination => (combination.Sum(r => r.PricePerNight) >= (int)vm.minPrice) && combination.Sum(r => r.PricePerNight) <= (int)vm.maxPrice);
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
		public async Task<ActionResult> Reserve(string RoomsCombination)
        {
		
			List<Room>rooms=JsonConvert.DeserializeObject<List<Room>>(RoomsCombination);
            
			SearchViewModel myvm = JsonConvert.DeserializeObject<SearchViewModel>(TempData["myviewmodel"].ToString());
            ViewBag.MyHotel = rooms.FirstOrDefault().Hotel;
			//List<ReservationRoom> reservationRooms = new List<ReservationRoom>();
   //         foreach(var room in rooms)
   //         {
   //             reservationRooms.Add(new()
   //             {
   //                 RoomId = room.Id,
   //                 UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
   //                 IsDeleted = true
   //             });
   //         }
            //var user = await usermanager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            DraftReservation draft = new()
            {
                //Reserved = rooms
            };
			
            if(myvm.CheckInDate!=default)
            {
				draft.CheckInDate = myvm.CheckInDate;
			}     
            if(myvm.CheckOutDate!=default)
            {
				draft.CheckOutDate = myvm.CheckOutDate;
			}

            await reservationservice.savedraft(draft, User.FindFirstValue(ClaimTypes.NameIdentifier),rooms);
            string promoCode;
			if (draft.UsedPromoCode==null)
            {
                promoCode = "";
			}
            else
            {
                promoCode = reservationservice.GETCODEText(draft.UsedPromoCode.Id);

			}
                ReservationViewModel vm = new()
                {
                    CheckInDate = draft.CheckInDate,
                    CheckOutDate = draft.CheckOutDate,
                    amenity = draft.amenity == null ? 0 : draft.amenity,
                    Plan = draft.mealPlan == null ? 0 : draft.mealPlan,
                    PromoCode = promoCode,
					rooms = RoomsCombination

                };

			return View(vm);
        }

        [HttpPost]
		[Authorize]
		public async Task<ActionResult> Reserve(ReservationViewModel vm)
		{
			List<Room> rooms = JsonConvert.DeserializeObject<List<Room>>(vm.rooms);
			ViewBag.MyHotel = rooms.FirstOrDefault().Hotel;

			if (!ModelState.IsValid)
            {
                return View(vm);
            }
            if(vm.CheckInDate is null || vm.CheckOutDate is null)
            {
                ModelState.AddModelError("","Dates has to be added");
                return View(vm);
            }
			var user = await usermanager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            reservationservice.Delete(User.FindFirstValue(ClaimTypes.NameIdentifier));
			return View(vm);
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> SaveinDraft(ReservationViewModel vm)
		{
			List<Room> rooms = JsonConvert.DeserializeObject<List<Room>>(vm.rooms);
			ViewBag.MyHotel = rooms.FirstOrDefault().Hotel;
			DraftReservation draft = new()
			{
				//Reserved = rooms
			};

			if (vm.CheckInDate != default)
			{
				draft.CheckInDate = vm.CheckInDate;
			}
			if (vm.CheckOutDate != default)
			{
				draft.CheckOutDate = vm.CheckOutDate;
			}
			string promoCode;
			if (draft.UsedPromoCode == null)
			{
				promoCode = "";
			}
			else
			{
				promoCode = reservationservice.GETCODEText(draft.UsedPromoCode.Id);

			}
			draft.amenity = vm.amenity;
            draft.mealPlan = vm.Plan;

            draft.UsedPromoCodeId = reservationservice.GETCODE(vm.PromoCode)?.Id;
			await reservationservice.savedraft(draft, User.FindFirstValue(ClaimTypes.NameIdentifier), rooms);
			return View("Reserve",vm);
		}
		[HttpGet]
        [Authorize]
		public async Task<ActionResult> ViewMyDraft()
		{
            //var user=await usermanager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var draft=reservationservice.getUserReservation(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ReservationViewModel model = new();

			if (draft is not null)
            {
                var hotel= draft.Reserved.FirstOrDefault().Reserved.Hotel; ;
				ViewBag.MyHotel = hotel;
                
					var options = new JsonSerializerOptions
					{
						ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
					};

                
				model.rooms = JsonSerializer.Serialize(draft.Reserved.Select(r => r.Reserved), options);
				model.CheckInDate = draft.CheckInDate;
                model.CheckOutDate = draft.CheckOutDate;
                model.amenity = draft.amenity == null ? 0 : draft.amenity;
                model.Plan = draft.mealPlan == null ? 0 : draft.mealPlan;
                string text;
                if (draft.UsedPromoCodeId != null)
                {
                     text = reservationservice.GETCODEText((int)draft.UsedPromoCodeId);
				}
                else
                {
                    text = "";

				}
                    model.PromoCode = text == "" ? "" : text;
			}
            else
            {
                RedirectToAction("Index", "Home");
            }
                
            return View("Reserve", model);
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
