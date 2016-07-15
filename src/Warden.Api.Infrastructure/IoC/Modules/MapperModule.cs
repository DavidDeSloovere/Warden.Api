﻿using Autofac;
using AutoMapper;

namespace Warden.Api.Infrastructure.IoC.Modules
{
    public class MapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx => Mappers.Mapper.Resolve())
                .As<IMapper>()
                .SingleInstance();
        }
    }
}