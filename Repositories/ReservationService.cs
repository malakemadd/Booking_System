
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVCBookingFinal_YARAB_.Models;

namespace MVCBookingFinal_YARAB_.Repositories
{
	public class ReservationService(ApplicationDbContext context,UserManager<AppUser> usermanager) : IReservationService
	{
		public async Task savedraft(DraftReservation reservation,string userid, List<Room> rooms)
		{

	
			DraftReservation newone = new();
			var userdraft = context.DraftReservations.Include(r => r.Reserved).FirstOrDefault(d => d.UserId == userid);
			if (userdraft is null)
			{
				newone.UserId = userid;
				newone.amenity = reservation.amenity ?? 0;
				newone.mealPlan = reservation.mealPlan ?? 0;
				newone.CheckInDate = reservation.CheckInDate ?? null;
				newone.CheckOutDate = reservation.CheckOutDate ?? null;
				newone.UsedPromoCodeId = reservation.UsedPromoCodeId ?? null;
				context.Add(newone);
				context.SaveChanges();
			}
			else
			{
				userdraft.amenity = reservation.amenity ?? 0;
				userdraft.mealPlan = reservation.mealPlan ?? 0;
				userdraft.CheckInDate = reservation.CheckInDate ?? null;
				userdraft.CheckOutDate = reservation.CheckOutDate ?? null;
				userdraft.UsedPromoCodeId = reservation.UsedPromoCodeId ?? null;
				newone = userdraft;
				context.Update(userdraft);
				context.SaveChanges();
			}

			context.DraftReservationRoom.RemoveRange(context.DraftReservationRoom.Where(r => r.DraftReservationId == newone.Id));
			newone.Reserved = new();
			foreach (var room in rooms)
			{
				var draftReservationRoom = new DraftReservationRoom
				{
					DraftReservationId = newone.Id,
					ReservedId = room.Id
				};
				newone.Reserved.Add(draftReservationRoom);
			}
			//context.Update(newone);
			context.SaveChanges();
		}
			
		
	
		public PromoCode GETCODE(string text)
		{
			return context.PromoCodes.FirstOrDefault(r => r.Code == text);
		}
		public string GETCODEText(int id)
		{
			var code = context.PromoCodes.FirstOrDefault(r => r.Id == id);
			return code==null? "": code.Code;
		}

		public void Delete(string userid)
		{
			var draft = context.DraftReservations.FirstOrDefault(d => d.UserId == userid);
			context.DraftReservationRoom.RemoveRange(context.DraftReservationRoom.Where(r => r.DraftReservationId == draft.Id));
			//if(draft is not null)
			context.Remove(draft);
			context.SaveChanges();
		}
		public DraftReservation getUserReservation(string userid)
		{
			var hhh=context.DraftReservations.Include(r => r.Reserved).ThenInclude(R => R.Reserved).ThenInclude(R => R.Hotel).ThenInclude(H => H.Ameneties).Include(r=>r.UsedPromoCode).FirstOrDefault(d => d.UserId == userid);
			return hhh;
		
		}

	}
}
