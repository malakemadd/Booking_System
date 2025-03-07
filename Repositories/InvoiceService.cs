using Microsoft.EntityFrameworkCore;

namespace MVCBookingFinal_YARAB_.Repositories
{
	public class InvoiceService(ApplicationDbContext _context):IInvoiceService
	{
		public List<Invoice> GetAll()
		{
			return _context.Invoices
				.Include(i => i.Reservation).ThenInclude(R => R.UsedPromoCodes).ThenInclude(u => u.User)
				.Include(i => i.Payment).ThenInclude(p => p.payment).Include(i => i.Reservation)
				.ThenInclude(R => R.Reserved).ThenInclude(r => r.Room).ThenInclude(r => r.Hotel).
				Include(i => i.Reservation).ThenInclude(R => R.UsedPromoCodes).ThenInclude(u => u.PromoCode)

				.Include(i => i.Reservation)
				.ThenInclude(R => R.mealPlan)

				.Include(i => i.Reservation)
				.ThenInclude(R => R.amenity).
				Include(i => i.Reservation)
				.ThenInclude(R => R.Reserved).ThenInclude(r => r.Room).ThenInclude(r => r.Images).ToList();
		}
		public List<Invoice> GetByUserId(string id)
		{
			return _context.Invoices
				.Include(i => i.Reservation).ThenInclude(R => R.UsedPromoCodes).ThenInclude(u => u.User)
				.Include(i => i.Payment).ThenInclude(p => p.payment).Include(i => i.Reservation)
				.ThenInclude(R => R.Reserved).ThenInclude(r => r.Room).ThenInclude(r => r.Hotel).
				Include(i => i.Reservation).ThenInclude(R => R.UsedPromoCodes).ThenInclude(u => u.PromoCode)

				.Include(i => i.Reservation)
				.ThenInclude(R => R.mealPlan)

				.Include(i => i.Reservation)
				.ThenInclude(R => R.amenity).
				Include(i => i.Reservation)
				.ThenInclude(R => R.Reserved).ThenInclude(r => r.Room).ThenInclude(r => r.Images).Where(i=>i.Reservation.UsedPromoCodes.All(i=>i.UserId==id)).ToList();
		}
		public Invoice GetById(int id)
		{
			return _context.Invoices
				.Include(i => i.Reservation).ThenInclude(R => R.UsedPromoCodes).ThenInclude(u => u.User)
				.Include(i => i.Payment).ThenInclude(p => p.payment).Include(i => i.Reservation)
				.ThenInclude(R => R.Reserved).ThenInclude(r => r.Room).ThenInclude(r => r.Hotel).
				Include(i => i.Reservation).ThenInclude(R => R.UsedPromoCodes).ThenInclude(u => u.PromoCode)

				.Include(i => i.Reservation)
				.ThenInclude(R => R.mealPlan)

				.Include(i => i.Reservation)
				.ThenInclude(R => R.amenity).
				Include(i => i.Reservation)
				.ThenInclude(R => R.Reserved).ThenInclude(r => r.Room).ThenInclude(r => r.Images).FirstOrDefault(i=>i.Id==id);
		}
	}
}
