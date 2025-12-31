using LaylaApi.Models.MainModels;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace LaylaApi.Templates
{
    public class contract_template : IDocument
    {
        private readonly Contract _contract;
        private readonly Booking _booking;
        private readonly Apartment _apartment;
        private readonly User _owner;
        private readonly User _renter;
        private readonly string _specialTerms;

        public contract_template(Contract c, Booking b, Apartment a, User owner, User renter, string terms)
        {
            _contract = c;
            _booking = b;
            _apartment = a;
            _owner = owner;
            _renter = renter;
            _specialTerms = terms ?? "لا يوجد / None";
        }

        public DocumentMetadata GetMetadata() => new DocumentMetadata { Title = "Rental Contract" };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(40);

                page.Content().Column(col =>
                {
                    col.Item().Text("Rental Contract")
                        .FontSize(22)
                        .Bold()
                        .AlignCenter();

                    col.Spacing(15);

                    col.Item().Decoration(dec =>
                    {
                        dec.Content().Column(info =>
                        {
                            info.Item().Text($"Contract ID: {_contract.Id}");
                            info.Item().Text($"Date: {DateTime.UtcNow:yyyy-MM-dd}");
                        });
                    });

                    col.Spacing(15);

                    col.Item().Text($"Owner: {_owner.FullName}").FontSize(14);
                    col.Item().Text($"Renter: {_renter.FullName}").FontSize(14);

                    col.Spacing(15);

                    col.Item().Text($"Apartment Details").Bold().FontSize(16);

                    col.Item().Column(a =>
                    {
                        a.Item().Text($"Address: {_apartment.Address}");
                        a.Item().Text($"Apartment ID: {_apartment.Id}");
                    });

                    col.Spacing(15);

                    col.Item().Text($"Rental Duration").Bold().FontSize(16);

                    col.Item().Column(d =>
                    {
                        d.Item().Text($"From: {_booking.StartDate:yyyy-MM-dd}");
                        d.Item().Text($"To: {_booking.EndDate:yyyy-MM-dd}");
                    });

                    col.Spacing(15);

                    col.Item().Text("Special Terms").Bold().FontSize(16);
                    col.Item().Text(_specialTerms).FontSize(13);

                    col.Spacing(30);

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Owner Signature:\n\n______________________");
                        r.RelativeItem().Text("Renter Signature:\n\n______________________");
                    });
                });
            });
        }
    }
}
