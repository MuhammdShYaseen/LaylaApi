using LaylaApi.Services.DataCRUD.Implementations;
using LaylaApi.Services.DataCRUD.Interfaces;

namespace LaylaApi.Services.DataCRUD.ServiceCollectionExtensions
{
    public static class DataCRUDServiceCollectionExtensions
    {
        public static IServiceCollection AddDataCRUDServices (this IServiceCollection services)
        {
            services.AddScoped<IApartmentService, ApartmentService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<IMediaFileService, MediaFileService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDeviceTokenService, DeviceTokenService>();
            return services;
        }
    }
}
