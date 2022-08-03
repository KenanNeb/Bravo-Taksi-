using Bravo_Taksi.Command;
using Bravo_Taksi.View;
using System.Windows;

namespace Bravo_Taksi.ViewModel
{
    public class AdminPanelViewModel
    {
        private AdminPanelView APV { get; set; }
        private bool boolen { get; set; } = false;
        public RelayCommand1 Show { get; set; }
        private void Add(object parameter)
        {
            TOGG_btn_Click();
        }
        private bool CanAdd(object parameter)
        {
            return true;
        }
        public void TOGG_btn_Click()
        {
            boolen = !boolen;
            if (!boolen) { APV.TAB_CNR.Visibility = Visibility.Visible; APV.FirstTAB_CNR.Visibility = Visibility.Hidden; return; }
            APV.TAB_CNR.Visibility = Visibility.Hidden; APV.FirstTAB_CNR.Visibility = Visibility.Visible; return;

        }
        public AdminPanelViewModel(AdminPanelView aPV)
        {
            APV = aPV;
            Show = new RelayCommand1(Add, CanAdd);

        }
    }
}
