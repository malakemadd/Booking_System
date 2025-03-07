namespace MVCBookingFinal_YARAB_.ViewModels
{
	public class RoomFilterationViewModel
	{
		public int minPrice { set; get; }
		public int maxPrice { set; get; }
		//public AmenityEnum amenity { set; get; }
		public sortBy Sorting { set; get; }
	}
	public enum sortBy
	{
		OurTopPicks,
		LowPrice,
		HighPrice

	}
}
