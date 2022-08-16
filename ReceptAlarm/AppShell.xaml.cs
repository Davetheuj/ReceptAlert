using System;
using System.Collections.Generic;
using ReceptAlarm.ViewModels;
using ReceptAlarm.Views;
using Xamarin.Forms;

namespace ReceptAlarm
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        }

    }
}
