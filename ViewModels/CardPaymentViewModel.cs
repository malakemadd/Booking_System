namespace MVCBookingFinal_YARAB_.ViewModels
{
    public class CardPaymentViewModel
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public string CVV { get; set; }
    }
}
