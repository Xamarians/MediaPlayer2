using System;

namespace Xamarians.MediaPlayers
{
    public class PlayerErrorEventArgs : EventArgs
    {
        public string Message { get; set; }

        public PlayerErrorEventArgs(string message)
        {
            Message = message;
        }
    }
}
