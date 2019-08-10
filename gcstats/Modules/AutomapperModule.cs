using Autofac;
using AutoMapper;
using gcstats.Queries;

namespace gcstats.Modules
{
    public class AutoMapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //var config = new MapperConfiguration(cfg => cfg.CreateMap<ParsePlayerDataFromHtml.Result, >)
            //builder
            //    .RegisterType<Mapper>()
            //    .As<IMapper>();
        }
    }
}
