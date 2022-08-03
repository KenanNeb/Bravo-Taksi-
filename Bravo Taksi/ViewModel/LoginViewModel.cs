using Bravo_Taksi.Auxiliary;
using Bravo_Taksi.Command;
using Bravo_Taksi.View;
using System.Windows;
using System.Windows.Media;

namespace Bravo_Taksi.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        public RelayCommand1 AddCommand { get; set; }
        public RelayCommand1 Sign { get; set; }
        public RelayCommand1 Forgot_PSD { get; set; }
        public RelayCommand1 Exit { get; set; }
        public Brush Coloruser { get; set; } = new SolidColorBrush(Colors.Green);
        private Login lgn;

        private bool isok { get; set; }
        public string UserName { get; set; } = "";
        private LoginViewModel() { }
        public LoginViewModel(Login obj)
        {
            lgn = obj;
            AddCommand = new RelayCommand1(Add, CanAdd);
            Sign = new RelayCommand1(SingUP, AlwaysTrue);
            Exit = new RelayCommand1(ExitBtn, AlwaysTrue);
            Forgot_PSD = new RelayCommand1(Forgot_Password, AlwaysTrue);
        }
        private void Add(object parameter)
        {
            if (FileFolder.BinRead(lgn.TXT.Text, lgn.PSP.Password, 1))
            {
                lgn.LGN_LBL.Content = "Login";
                lgn.LGN_LBL.Foreground = new SolidColorBrush(Color.FromRgb(14, 111, 143));
                LoadingPanel LP = new LoadingPanel(1, 400, 500);
                LP.window = new RouteMap();
                LP.Show(); lgn.Close();
                return;
            }
            lgn.LGN_LBL.Content = "No User";
            lgn.LGN_LBL.Foreground = new SolidColorBrush(Colors.Red);


        }
        private bool CheckBool()
        {
            bool boolen = true;
            if (UserName != null && UserName.Length <= 2)
            {
                boolen = false;
                lgn.TXT.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else { lgn.TXT.BorderBrush = new SolidColorBrush(Color.FromRgb(35, 151, 151)); boolen = true; }
            bool boole1 = true;
            if (lgn.PSP.Password != null && lgn.PSP.Password.Length <= 5)
            {
                boole1 = false;
                lgn.PSP.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else { lgn.PSP.BorderBrush = new SolidColorBrush(Color.FromRgb(35, 151, 151)); boole1 = true; }

            return boolen && boole1;
        }
       
        private void ExitBtn(object parametr) => lgn.Close();
        private void SingUP(object parameter)
        {
            LoadingPanel LP = new LoadingPanel(1, 400, 500);
            LP.window = new SignUp();
            LP.Show(); lgn.Close();
        }
        private void Forgot_Password(object parameter)
        {
            LoadingPanel LP = new LoadingPanel(1, 400, 500);
            LP.window = new ForgetPassword();
            LP.Show(); lgn.Close();
        }
        private bool AlwaysTrue(object parametr) => true;
        private bool CanAdd(object parametr)
        {
            isok = CheckBool();
            return isok;
        }
    }

}