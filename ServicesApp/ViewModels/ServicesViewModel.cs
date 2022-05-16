using Prism.Commands;
using Prism.Mvvm;
using ServicesApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Windows.Input;

namespace ServicesApp.ViewModels
{
    public class ServicesViewModel : BindableBase
    {
        public ServicesViewModel()
        {
            Services = CustomMapper.Initialize();
            StartCommand = new DelegateCommand(StartCommandExecute,CanStart);
            StopCommand = new DelegateCommand(StopCommandExecute, CanStop);
        }

        public bool CanStart()
        {
            return SelectedRow?.Status != ServiceControllerStatus.Running;
        }

        public bool CanStop()
        {
            return SelectedRow?.Status == ServiceControllerStatus.Running;
        }
        public void StartCommandExecute()
        {
            ServiceController sc = CustomMapper.Services.FirstOrDefault(x => x.ServiceName == SelectedRow.Name);

            try
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 5));
                
            }
            catch(InvalidOperationException)
            {
                //will write log,if started from my program and stopped from windows services then catched this exception
            }
            catch(System.ServiceProcess.TimeoutException)
            {
                //will write log,if WaitForStatus time longer then timespan seconds
            }
            finally
            {
                SelectedRow.Status = sc.Status;
                StartCommand.RaiseCanExecuteChanged();
                StopCommand.RaiseCanExecuteChanged();
            }

        }

        public void StopCommandExecute()
        {
            ServiceController sc = CustomMapper.Services.FirstOrDefault(x => x.ServiceName == SelectedRow.Name);
            try
            {
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 5));
            }

            catch (InvalidOperationException)
            {
                //some
            }

            catch (System.ServiceProcess.TimeoutException)
            {
                //some
            }
            finally
            {
                SelectedRow.Status = sc.Status;
                StopCommand.RaiseCanExecuteChanged();
                StartCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand RowChangedCommand { get; set; }
        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }

        private ObservableCollection<ServiceModel> _services;
        public ObservableCollection<ServiceModel> Services
        {
            get { return _services; }
            set { _services = value; }
        }

        private bool _isStopped;
        public bool IsStopped
        {
            get { return _isStopped; }
            set { _isStopped = value; }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set { _isRunning = value; }
        }
        protected void SetProperty(ref ServiceModel field, ServiceModel newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                RaisePropertyChanged();
            }

        }

        private ServiceModel _selectedRow;
        public ServiceModel SelectedRow
        {
            get => _selectedRow;
            set
            {
                SetProperty(ref _selectedRow, value);
                StartCommand.RaiseCanExecuteChanged();
                StopCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
