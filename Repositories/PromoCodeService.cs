
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MVCBookingFinal_YARAB_.Repositories
{
    public class PromoCodeService(ApplicationDbContext context) : IPromoCodeService
    {        
        public List<PromoCode> GetAll()
        {
            
            return context.PromoCodes.Include(p=>p.AddingUser).ToList();
        }
        public List<PromoCode> GetAllAddedBySingleUser(string id)
        {                        
                        
            return context.PromoCodes.Include(pc=>pc.AddingUser).Where(p => p.AddingUserID == id).ToList();
        }
        public PromoCode GetById(int id)
        {
            return context.PromoCodes.FirstOrDefault(pc=>pc.Id==id);
        }

        public void Create(PromoCodeViewModel pc,string userId)
        {
            PromoCode code = new PromoCode();
            code.ExpiryDate = pc.ExpiryDate;
            code.AddingUserID = userId;
            code.Code = pc.Code;
            context.PromoCodes.Add(code);
            context.SaveChanges();
        }
        public void Update(PromoCodeViewModel pc)
        {
            var selectedpromocode = context.PromoCodes.FirstOrDefault(pc => pc.Id == pc.Id);
            selectedpromocode.ExpiryDate = pc.ExpiryDate;
            selectedpromocode.Code = pc.Code;
            selectedpromocode.AddingUserID = pc.AddingUserID;

			context.PromoCodes.Update(selectedpromocode);
        }

        public void Delete(PromoCode pc)
        {
            var selectedpromocode= context.PromoCodes.FirstOrDefault(p => p.Id == pc.Id);
            selectedpromocode.IsActive = false;
            context.PromoCodes.Update(pc);
            context.SaveChanges();
        }        
    }
}
