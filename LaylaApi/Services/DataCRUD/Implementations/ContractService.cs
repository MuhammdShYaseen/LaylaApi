using LaylaApi.DataAccess;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using LaylaApi.Templates;
using LaylaApi.Models.DtosModels.MainDtos;
using AutoMapper;

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

        public async Task<IEnumerable<ContractDto>> GetAllAsync()
        {
            var contracts = await _context.Contracts
                .AsNoTracking()
                .Include(c => c.Booking)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ContractDto>>(contracts);
        }

        public async Task<ContractDto> GetByIdAsync(int id, int userId, bool isAdmin)
        {
            var contract = await _context.Contracts
               .AsNoTracking()
               .Include(c => c.Booking)
               .ThenInclude(b => b!.Apartment!)
               .FirstOrDefaultAsync(c => c.Id == id);

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

        public async Task<ContractDto> GetByBookingIdAsync(int bookingId, int userId, bool isAdmin)
        {
            var contract = await _context.Contracts
                .AsNoTracking()
                .Include(c => c.Booking).ThenInclude(a => a!.Apartment)
                .FirstOrDefaultAsync(c => c.BookingId == bookingId);

            if (contract == null)
                throw new KeyNotFoundException("Contract not found");

            if (!HasContractAccess(contract.Booking!, contract.Booking!.Apartment!, userId, isAdmin))
                throw new UnauthorizedAccessException("AccessDenied");

            return _mapper.Map<ContractDto>(contract);
        }

        public async Task<ContractDto> AddAsync(CreateContractDto dto)
        {
            var contract = _mapper.Map<Contract>(dto);
            contract.CreatedAt = DateTime.UtcNow;

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return _mapper.Map<ContractDto>(contract);
        }

        public async Task<Contract> AddEntityAsync(int bookingId, string specialTerms)
        {

            var booking = await _context.Bookings
                .Include(b => b.Apartment)
                .ThenInclude(a => a!.Owner)
                .Include(b => b.User)
                .FirstAsync(b => b.Id == bookingId);

            var contract = Contract.Create(booking, specialTerms);
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return contract;
        }

        public async Task<ContractDto> UpdateAsync(int id, CreateContractDto dto)
        {
            var existing = await _context.Contracts.FindAsync(id);
            if (existing == null) 
                throw new KeyNotFoundException();

            _mapper.Map(dto, existing);

            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return _mapper.Map<ContractDto>(existing);
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
        public async Task<ContractDto> SignContractAsync(int id, int userId, bool isAdmin, ContractSigner contractSigner)
        {
            var contract = await _context.Contracts
                .Include(c => c.Booking)
                    .ThenInclude(b => b!.User) // المستأجر
                .Include(c => c.Booking)
                    .ThenInclude(b => b!.Apartment)
                    .ThenInclude(a => a!.Owner) // المالك
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null)
                throw new KeyNotFoundException("Contract not found.");

            var ownerId = contract.Booking!.Apartment!.OwnerId;
            var renterId = contract.Booking.UserId;

            switch (contractSigner)
            {
                case ContractSigner.Owner:
                    if (userId != ownerId && !isAdmin)
                        throw new UnauthorizedAccessException();

                    contract.SignByOwner(contract);
                    break;

                case ContractSigner.Renter:
                    if (userId != renterId && !isAdmin)
                        throw new UnauthorizedAccessException();

                    contract.SignByRenter(contract);
                    break;

                default:
                    throw new InvalidOperationException("Invalid signer");
            }
            await _context.SaveChangesAsync();
            return _mapper.Map<ContractDto>(contract);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Contracts.FindAsync(id);
            if (existing == null) return false;

            _context.Contracts.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
        public string GenerateContractPdf(Contract contract, Booking booking, Apartment apartment, User renter, User owner, string specialTerms)
        {
            var document = new contract_template(contract, booking, apartment, owner, renter, specialTerms);

            string folder = Path.Combine(_env.WebRootPath, "contracts");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = $"contract_{contract.Id}_{DateTime.UtcNow.Ticks}.pdf";
            string filePath = Path.Combine(folder, fileName);

            document.GeneratePdf(filePath);

            return $"/contracts/{fileName}";
        }

        private static bool HasContractAccess(Booking booking, Apartment apartment, int userId, bool isAdmin)
        {
            return booking.UserId == userId || apartment.OwnerId == userId || isAdmin;
        }

        
    }
}
