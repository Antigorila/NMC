using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NMC.App.MVVM.Models
{
    public partial class UserModel : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        public UserModel(int id, string username, string password)
        {
            this.id = id;
            this.username = username;
            this.password = password;
        }
    }
}
