namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class ApartmentMapPointDto
    {
    public int ApartmentId { get; init; }

    public double Latitude { get; init; }

    public double Longitude { get; init; }

    public decimal PricePerDay { get; init; }

    public bool IsAvailable { get; init; }
    }
}
