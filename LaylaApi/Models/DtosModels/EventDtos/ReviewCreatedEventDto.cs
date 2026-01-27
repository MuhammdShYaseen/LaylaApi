namespace LaylaApi.Models.DtosModels.EventDtos
{
    public class ReviewCreatedEventDto
    {
        public int OwnerId { get; init; }
        public string OwnerLang { get; init; } = "en";
        public int Rating { get; init; }
    }
}
