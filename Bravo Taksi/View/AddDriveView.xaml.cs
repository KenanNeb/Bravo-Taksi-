﻿using Bravo_Taksi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bravo_Taksi.View
{
    /// <summary>
    /// Interaction logic for AddDriveView.xaml
    /// </summary>
    public partial class AddDriveView : Window
    {
        private AddDriversVM ADVM {get;set;}
        public AddDriveView()
        {
            InitializeComponent();
            ADVM = new AddDriversVM(this);
            DataContext = this;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }
    }
}
