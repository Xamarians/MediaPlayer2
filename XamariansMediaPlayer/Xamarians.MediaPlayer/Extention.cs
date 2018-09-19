using System;
using Xamarin.Forms;

namespace Xamarians.MediaPlayers
{
    public static class Extention
    {
        public static void OnTapped(this View view, Action action)
        {
            view.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(action)
            });
        }
    }
}
