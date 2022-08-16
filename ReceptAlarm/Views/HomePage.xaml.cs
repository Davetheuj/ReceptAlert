
using ReceptAlarm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ReceptAlarm.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        private double threshold = 1;
        public double currentSpeed = 0;
        private bool isRunning = true;
        private DateTime oldTime = DateTime.UtcNow;
        private double checkTimeInterval = 1;
        private Task receptionToggleTask;
        private double receptionThreshold = 0;
        private bool hasNoReception = false;
        private Switch gainToggle;
        private Switch lossToggle;


        public HomePage()
        {
            InitializeComponent();

            receptionToggleTask = Task.Factory.StartNew(() => {
                while (isRunning)
                {


                    if (DateTime.UtcNow.Subtract(oldTime).TotalSeconds >= checkTimeInterval)
                    {
                        oldTime = DateTime.UtcNow;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            speedLabel.Text = $"Current Connection Speed: ~{currentSpeed} kB/s";

                            if (currentSpeed > receptionThreshold && hasNoReception)
                            {
                                hasNoReception = false;
                                //play audio here
                                if (notifyReceptionGainToggle.IsToggled)
                                {
                                   DependencyService.Get<IAudio>().PlayAudioFile("Alarm.mp3");
                                }
                            }

                            else if(currentSpeed < receptionThreshold && !hasNoReception)
                            {
                                if (notifyReceptionLossToggle.IsToggled)
                                {
                                    DependencyService.Get<IAudio>().PlayAudioFile("Alarm.mp3");
                                }
                            }

                        });
                        CheckInternetSpeed();
                        if(currentSpeed >= receptionThreshold)
                        {
                            hasNoReception = false;
                        }
                        else
                        {
                            hasNoReception = true;
                        }

                    }
                }
            });

            Task.Factory.StartNew(() => {
                while (isRunning)
                {
                    if (DateTime.UtcNow.Subtract(oldTime).TotalSeconds >= checkTimeInterval)
                    {
                        //Check Geolocation Data
                       Geolocation.GetLocationAsync();

                        oldTime = DateTime.UtcNow;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            //Update UI
                        });
                    }
                }
            });


        }

        private void NotifyReceptionGainToggle_Toggled(object sender, ToggledEventArgs e)
        {
            gainToggle = (Switch)sender;
            if (gainToggle.IsToggled)
            {
                header.Text = "Sound Notifications On!";
                statusLabel.Text = "On";
                if (notifyReceptionLossToggle.IsToggled)
                {
                    header.BackgroundColor = Color.LightSeaGreen;
                }
            }
            else
            {
                header.Text = "Sound Notifications Off";
                statusLabel.Text = "Off";
                header.BackgroundColor = Color.DarkSalmon;

            }
        }

        private void NotifyReceptionLossToggle_Toggled(object sender, ToggledEventArgs e)
        {
            lossToggle = (Switch)sender;
            if (lossToggle.IsToggled)
            {
                header.Text = "Sound Notifications On!";
                statusLossLabel.Text = "On";
                if (notifyReceptionGainToggle.IsToggled)
                {
                    header.BackgroundColor = Color.LightSeaGreen;
                }

            }
            else
            {
                header.Text = "Sound Notifications Off";
                statusLossLabel.Text = "Off";
                header.BackgroundColor = Color.DarkSalmon;

            }
        }


        public async Task<double> CheckInternetSpeed()
        {
            //DateTime Variable To Store Download Start Time.
            DateTime dt1 = DateTime.Now;
            double internetSpeed = 0;
            try
            {
                // Create Object Of WebClient
                var client = new HttpClient();
                //Number Of Bytes Downloaded Are Stored In ‘data’
                byte[] data = await client.GetByteArrayAsync("https://www.thirdpartition.com");
                //DateTime Variable To Store Download End Time.
                DateTime dt2 = DateTime.Now;
                //To Calculate Speed in Kb Divide Value Of data by 1024 And Then by End Time Subtract Start Time To Know Download Per Second.
                //Console.WriteLine("ConnectionSpeed: DataSize (kb) " + data.Length / 1024);
                //Console.WriteLine("ConnectionSpeed: ElapsedTime (secs) " + (dt2 - dt1).TotalSeconds);
                internetSpeed = Math.Round((data.Length / 1024) / (dt2 - dt1).TotalSeconds, 2);

                currentSpeed = internetSpeed;
                //speedLabel.TextColor = Color.LightSeaGreen;

            }
            catch (Exception ex)
            {
                currentSpeed = 0;
                //speedLabel.Text = $"Current Connection Speed: 0 kB/s";
                //Console.WriteLine("ConnectionSpeed:Unknown Exception-" + ex.Message);
                hasNoReception = true;
            }
            //Console.WriteLine(internetSpeed);
            //speedLabel.Text = $"Current Connection Speed: ~{internetSpeed} kB/s";
            return internetSpeed;
        }

        private void thresholdSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Slider thresholdSlider = (Slider)sender;
            receptionThreshold = (thresholdSlider.Value + .1) + (thresholdSlider.Value * thresholdSlider.Value * thresholdSlider.Value * 2000);
            if (receptionThreshold > 1000)
            {
                thresholdLabel.Text = Math.Round(receptionThreshold / 1000, 2).ToString() + " MB/s";
            }
            else
            {
                thresholdLabel.Text = Math.Round(receptionThreshold, 2).ToString() + " kB/s";
            }
            if (currentSpeed <= receptionThreshold)
            {
                speedLabel.TextColor = Color.DarkSalmon;
                
            }
            else
            {
                speedLabel.TextColor = Color.LightSeaGreen;
               
            }
        }

        private void speedLabel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (currentSpeed <= receptionThreshold)
            {
                speedLabel.TextColor = Color.DarkSalmon;
               
            }
            else
            {
                speedLabel.TextColor = Color.LightSeaGreen;
                
            }

        }
    }
}