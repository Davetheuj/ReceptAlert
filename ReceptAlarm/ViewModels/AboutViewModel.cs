using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ReceptAlarm.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://www.paypal.com/donate/?hosted_button_id=7SW6FLQ9QEJA2"));
        }

        public ICommand OpenWebCommand { get; }
    }
}