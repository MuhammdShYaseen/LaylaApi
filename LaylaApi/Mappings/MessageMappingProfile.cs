using AutoMapper;
using LaylaApi.Models.DtosModels.MessageDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Mappings
{
    public sealed class MessageMappingProfile : Profile
    {
        public MessageMappingProfile()
        {
            CreateMap<Message, MessageDto>()
                .ForMember(
                    dest => dest.VoiceUrl,
                    opt => opt.MapFrom(src =>
                        src.VoiceFilePath == null
                            ? null
                            : $"/api/messages/voice/{src.Id}"
                    )
                );
        }
    }
}
