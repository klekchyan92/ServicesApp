using AutoMapper;
using ServicesApp.Models;
using System.Collections.ObjectModel;
using System.Management;
using System.ServiceProcess;

namespace ServicesApp
{
    public static class CustomMapper
    {
        private static bool _isInitialized;
        private static ObservableCollection<ServiceModel> _serviceModels { get; set; }
        public static ServiceController[] Services { get; private set; }
        public static ObservableCollection<ServiceModel> Initialize()
        {
            if (!_isInitialized)
            {
                Services = GetServices();
                var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<ServiceController, ServiceModel>()
                            .ForMember(name => name.Name,
                                map => map.MapFrom(service => service.ServiceName)
                            )
                            .ForMember(displayName => displayName.DisplayName,
                                map => map.MapFrom(service => service.DisplayName)
                            )
                            .ForMember(status => status.Status,
                                map => map.MapFrom(service => service.Status)
                            )
                            .ForMember(account => account.Account,
                                map => map.MapFrom(service => new ManagementObject("Win32_Service.Name='" + service.ServiceName + "'")["StartName"].ToString())
                            );
                        });

                IMapper mapper = config.CreateMapper();
                _serviceModels = mapper.Map<ObservableCollection<ServiceModel>>(Services);

                _isInitialized = true;
            }

            return _serviceModels;
        }
        private static ServiceController[] GetServices()
        {
            return ServiceController.GetServices();
        }
    }
}