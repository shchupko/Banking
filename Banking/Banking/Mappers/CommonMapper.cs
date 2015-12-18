using AutoMapper;
using Banking.Domain;
using Banking.Domain.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Banking.Mappers
{
    public class CommonMapper : IMapper
    {
        static CommonMapper()
        {
            Mapper.CreateMap<User, UserRegisterView>()
                    .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
                    .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Lastname))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
            Mapper.CreateMap<UserRegisterView, User>()
                   .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
                    .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                    .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.Address))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        }

        public object Map(object source, Type sourceType, Type destinationType)
        {
            return Mapper.Map(source, sourceType, destinationType);
        }
    }
}