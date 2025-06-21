using ASP.Authentication.DTOs;
using AutoMapper;
using Core;
using Core.Enteties;
using PlanetDiseaaseDR.DTO;

namespace ASP.Authentication;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserForRegisterationDto, AppUser>()
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.Email));
        CreateMap<ReportProblem, ReportProblemToReturnDTO>().ForMember(P => P.AppUser, U => U.MapFrom(N => N.AppUser.Name)).ForMember(P => P.PlantImage, U => U.MapFrom(N => N.AppUser.ImageName)).ReverseMap();
        CreateMap<CreateProblemDTO, ReportProblem>().ReverseMap();


    }
}
