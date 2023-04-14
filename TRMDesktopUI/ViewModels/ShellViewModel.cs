using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private LoginViewModel _loginVM; // In ctor, we need to ask for our login form, we ask for it. Declare a holder
        public ShellViewModel(LoginViewModel loginVM)
        {
            _loginVM = loginVM;
            ActivateItem(_loginVM); //Conductor'dan geldi, LoginViewModel'i gorunce LoginView'in gelme sebebi caliburn.micro
        }
    }
}
