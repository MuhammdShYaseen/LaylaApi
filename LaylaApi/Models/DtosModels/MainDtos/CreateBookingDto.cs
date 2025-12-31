namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class CreateBookingDto
    {
        public int ApartmentId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }
}
