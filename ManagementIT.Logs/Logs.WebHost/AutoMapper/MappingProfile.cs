using AutoMapper;
using Contracts.Logs;
using Logs.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logs.WebHost.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LogMessageEntity, LogMessage>()
                .ForMember(dest => dest.Id, x => x.MapFrom(src => src.Id))
                .ForMember(dest => dest.Iniciator, x => x.MapFrom(src => src.Iniciator))
                .ForMember(dest => dest.Message, x => x.MapFrom(src => src.Message))
                .ForMember(dest => dest.Type, x => x.MapFrom(src => src.Type))
                .ForMember(dest => dest.DateOrTime, x => x.MapFrom(src => src.DateOrTime))
                .ForMember(dest => dest.Address, x => x.MapFrom(src => src.Address));
        }
    }
}
