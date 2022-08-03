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
    /// Interaction logic for UserAdminTaxi.xaml
    /// </summary>
    public partial class UserAdminTaxi : Window
    {
        public UserAdminTaxi()
        {
            InitializeComponent();
            DataContext = this;
        }
        public double Isok { get; set; } = 0.001;
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Isok = 1;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
           
        }
    }
}
