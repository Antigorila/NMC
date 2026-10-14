using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.App.MVVM.Models
{
    public partial class GenreModel : ObservableObject
    {
        [ObservableProperty]
        private int id;
        [ObservableProperty]
        private string name;
        public GenreModel(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
