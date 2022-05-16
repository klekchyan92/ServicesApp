using Prism.Mvvm;
using System.ServiceProcess;

namespace ServicesApp.Models
{
    public class ServiceModel : BindableBase
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        private ServiceControllerStatus _status;
        public ServiceControllerStatus Status 
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged(); } 
        }
        public string Account { get; set; }

    }
}
