using System;
using Xamarin.Forms;
using Android.Media;
using Android.Content.Res;
using ReceptAlarm.Droid;
using ReceptAlarm.Services;

[assembly: Dependency(typeof(AudioService))]
namespace ReceptAlarm.Droid
{
    public class AudioService : IAudio
    {
        public AudioService()
        {
        }

        public void PlayAudioFile(string fileName)
        {
          
                var player = new MediaPlayer();

                var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
                player.Prepared += (s, e) =>
                {
                    player.Start();
                };
                player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
                player.Prepare();
            
          
        }
       
    }
}