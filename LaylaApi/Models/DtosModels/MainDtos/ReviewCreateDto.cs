namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class ReviewCreateDto
    {
        public int ApartmentId { get; set; }   // الشقة التي يتم تقييمها

        public int Rating { get; set; }        // من 1 إلى 5

        public string? Comment { get; set; }   // تعليق اختياري، بحد 1000 حرف
    }
}
