using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.Messaging.Messages;
using NMC.Mobile.Models;

namespace NMC.Mobile.Helper
{
    public class PlaylistCreatedMessage : ValueChangedMessage<PlaylistModel>
    {
        public PlaylistCreatedMessage(PlaylistModel playlist) : base(playlist)
        {
        }
    }
}
