﻿using System;
using Xamarin.Forms;
using Samples.ViewModels;


namespace Samples.Views {

    public partial class SignatureListView : ContentPage {
    
        public SignatureListView() {
            InitializeComponent();
			this.ListView.ItemSelected += (sender, e) => {
				if (e.SelectedItem == null)
					return;

				((SignatureListViewModel)this.BindingContext).Select.Execute(e.SelectedItem);
			};
        }
    }
}
