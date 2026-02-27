using LaylaApi.DataAccess;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using LaylaApi.Templates;
using LaylaApi.Models.DtosModels.MainDtos;
using AutoMapper;
using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class ContractService : IContractService
    {
        private readonly LaylaContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public ContractService(LaylaContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ContractDto>> GetAllAsync(CancellationToken ct)
        {
            var contracts = await _context.Contracts
                .AsNoTracking()
                .Include(c => c.Booking)
                .ToListAsync(ct);

            return _mapper.Map<IEnumerable<ContractDto>>(contracts);
        }

        public async Task<ContractDto> GetByIdAsync(int id, int userId, bool isAdmin, CancellationToken ct)
        {
            var contract = await _context.Contracts
               .AsNoTracking()
               .Include(c => c.Booking)
               .ThenInclude(b => b!.Apartment!)
               .FirstOrDefaultAsync(c => c.Id == id, ct);

            if (contract == null)
                throw new KeyNotFoundException("Contract not found");

            if (!HasContractAccess(contract.Booking!, contract.Booking!.Apartment!, userId, isAdmin))
                throw new UnauthorizedAccessException("AccessDenied");

            return _mapper.Map<ContractDto>(contract);
        }

        public async Task<Contract> GetEntityByIdAsync(int id)
        {
            var contract = await _context.Contracts
                 .AsNoTracking()
                 .Include(c => c.Booking)
                 .FirstOrDefaultAsync(c => c.Id == id);
            if (contract == null)
                throw new KeyNotFoundException();
            return contract;
        }

        public async Task<ContractDto> GetByBookingIdAsync(int bookingId, int userId, bool isAdmin, CancellationToken ct)
        {
            var contract = await _context.Contracts
                .AsNoTracking()
                .Include(c => c.Booking).ThenInclude(a => a!.Apartment)
                .FirstOrDefaultAsync(c => c.BookingId == bookingId, ct);

            if (contract == null)
                throw new KeyNotFoundException("Contract not found");

            if (!HasContractAccess(contract.Booking!, contract.Booking!.Apartment!, userId, isAdmin))
                throw new UnauthorizedAccessException("AccessDenied");

            return _mapper.Map<ContractDto>(contract);
        }


        public async Task<Contract> AddEntityAsync(int bookingId, string specialTerms)
        {
            if (await _context.Contracts.AnyAsync(c => c.BookingId == bookingId))
                throw new InvalidOperationException("Contract already exists for this booking.");

            var booking = await _context.Bookings
                .AsNoTracking()
                .Include(a => a.Apartment)
                .SingleAsync(b => b.Id == bookingId);

            if (booking.Status != BookingStatus.Confirmed)
                throw new InvalidOperationException("Contract can only be generated for confirmed bookings.");

            var contract = Contract.Create(bookingId, specialTerms, booking.UserId, booking.Apartment!.OwnerId);

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return contract;
        }

        public async Task<ContractDto> UpdateAsync(int id, CreateContractDto dto, CancellationToken ct)
        {
            var contract = await _context.Contracts.FindAsync(id, ct);
            if (contract == null) 
                throw new KeyNotFoundException();



            contract.Update(dto.SpecialTerms ?? "", dto.ContractUrl);
            await _context.SaveChangesAsync();

            return _mapper.Map<ContractDto>(contract);
        }

        public async Task<ContractDto> UpdateEntityAsync(Contract contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
            return _mapper.Map<ContractDto>(contract);
        }

        public enum ContractSigner
        {
            Owner,
            Renter
        }
        public async Task<ContractDto> SignContractAsync(int id, int userId, bool isAdmin, ContractSigner contractSigner, CancellationToken ct)
        {
            var contract = await _context.Contracts.SingleOrDefaultAsync(c => c.Id == id, ct) ?? throw new KeyNotFoundException("Contract not found.");

            var ownerId = contract.OwnerId;
            var renterId = contract.RenterId;

            switch (contractSigner)
            {
                case ContractSigner.Owner:
                    if (userId != ownerId && !isAdmin)
                        throw new UnauthorizedAccessException();

                    contract.SignByOwner();
                    break;

                case ContractSigner.Renter:
                    if (userId != renterId && !isAdmin)
                        throw new UnauthorizedAccessException();

                    contract.SignByRenter();
                    break;

                default:
                    throw new InvalidOperationException("Invalid signer");
            }

            await _context.SaveChangesAsync();

            return _mapper.Map<ContractDto>(contract);
        }

        public async Task<bool> DeleteAsync(int id, int userId, bool isAdmin, CancellationToken ct)
        {
            var contract = await _context.Contracts
         .Include(c => c.Booking)
            .ThenInclude(b => b!.Apartment)
         .FirstOrDefaultAsync(c => c.Id == id, ct);

            if (contract == null)
                throw new KeyNotFoundException("Contract not found.");

            var ownerId = contract.Booking!.Apartment!.OwnerId;

            if (userId != ownerId && !isAdmin)
                throw new UnauthorizedAccessException("Access denied.");

            var pdfPath = contract.ContractUrl;

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            DeleteContractFile(pdfPath);

            return true;
        }
        private void DeleteContractFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public string GenerateContractPdf(Contract contract, Booking booking, Apartment apartment, User renter, User owner, string specialTerms)
        {
            try
            {
                var document = new contract_template(contract, booking, apartment, owner, renter, specialTerms);

                string folder = Path.Combine(_env.WebRootPath, "contracts");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = $"contract_{contract.Id}_{Guid.NewGuid():N}.pdf";
                string filePath = Path.Combine(folder, fileName);

                document.GeneratePdf(filePath);

                return $"/contracts/{fileName}";
            }
            catch (Exception ex) 
            {
                throw new InvalidOperationException("Failed to generate contract PDF", ex);
            }

            
        }

        private static bool HasContractAccess(Booking booking, Apartment apartment, int userId, bool isAdmin)
        {
            return booking.UserId == userId || apartment.OwnerId == userId || isAdmin;
        }

        public async Task<ContractDto> GenerateContractAsync(int userId, ContractCreateDto model, bool isAdmin, CancellationToken ct)
        {

            var booking = await _context.Bookings
                .Include(a => a.Apartment)
                    .ThenInclude(o => o!.Owner)
                .Include(r => r.User)
                .FirstOrDefaultAsync(i => i.Id == model.BookingId, ct);

            if (booking == null)
                throw new KeyNotFoundException("booking not found");

            if (booking.Apartment == null)
                throw new KeyNotFoundException("Apartment not found");

            if (booking.User == null)
                throw new KeyNotFoundException("Renter not found");

            if (booking.Apartment.Owner == null)
                throw new KeyNotFoundException();

            if (booking.Apartment.OwnerId != userId && !isAdmin)
                throw new UnauthorizedAccessException("Access Denied");

            var contract = await AddEntityAsync(booking.Id, model.SpecialTerms ?? "");

            string pdfUrl = GenerateContractPdf(contract, booking, booking.Apartment, booking.User, booking.Apartment.Owner, model.SpecialTerms ?? "");

            contract.AddPdfUrl(pdfUrl);

            await UpdateEntityAsync(contract);

            return _mapper.Map<ContractDto>(contract);
        }
    }
}
