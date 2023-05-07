using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private IEventAggregator _events; //Bunlar shellview calistigi surece ayaktalar, loginview her shell degistiginde sifirlaniyor
        private SalesViewModel _salesVM;
        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM)
        {
            _events = events;
            _salesVM = salesVM;

            _events.Subscribe(this); //Subscribe to events which is declared through IHandle

            ActivateItem(IoC.Get<LoginViewModel>()); //Conductor'dan geldi, LoginViewModel'i gorunce LoginView'in gelme sebebi caliburn.micro
        }

        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM);
        }
    }
}
