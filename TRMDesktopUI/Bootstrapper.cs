using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TRMDesktopUI.Helpers;
using TRMDesktopUI.Library.API;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.ViewModels;

namespace TRMDesktopUI
{
    public class Bootstrapper : BootstrapperBase
    {
        //Dependency Injection(DI) here
        private SimpleContainer _container = new SimpleContainer();
        public Bootstrapper()
        {
            Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(
            PasswordBoxHelper.BoundPasswordProperty,
            "Password",
            "PasswordChanged");
        }

        //Instance burada olusuyor, meta bir olay var, container kendi icinde kendi instance'ini olusturuyor ve sonra dagitiyor
        //Handling of bringing windows - windowmanager()
        //Event handler, raising events and listening events. We let caliburn.micro handle them eventaggregator()
        //Singleton: One instance in scope of an application start and stop,
        //singleton means others views or other things will demand the same eventaggregator or window manager etc. instead of creating a new one
        //Singleton'i kendin de olusturabilirsin, temel mantigi design pattern olusturmak. User nesne uretemiyor, singleton'dan talep ediyor.
        //Tim Corey: Don't use it unless you can't find a better way, not great on memory usage
        protected override void Configure()
        {
            _container.Instance(_container)
                .PerRequest<IProductEndpoint, ProductEndpoint>();

            _container
                .Singleton<IWindowManager, WindowManager>() 
                .Singleton<IEventAggregator, EventAggregator>() 
                .Singleton<ILoggedInUserModel, LoggedInUserModel>()
                .Singleton<IConfigHelper, ConfigHelper>()
                .Singleton<IAPIHelper, APIHelper>(); //Open on one entire lifespan of HttpClient

            //Reflection, get type's assembly, get all the types that's running in the entire application
            //where it is a class and ends with ViewModel just like ShellViewModel
            //Then for every viewModel type register that everytime requested from the container
            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        //When wpf starts, App.xml'den bootstraper ctor ile initialize yapiyor, onstartup tetikleniyor ve shellviewmodel aciliyor ilk acilisda
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        //Bootstrapper'in instance almasini override yaptik, container ile DI yapinca container'in getinstance'i instant atiyor
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        //construct olayini yapiyor bootstrapper yerine
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
