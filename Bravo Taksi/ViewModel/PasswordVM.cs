using Bravo_Taksi.Auxiliary;
using Bravo_Taksi.Command;
using Bravo_Taksi.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Bravo_Taksi.ViewModel
{
    public class PasswordVM
    {
        private int _randomCode { get; set; }
        private string ConcatenatedCode { get; set; }
        private ForgetPassword FP { get; set; }
        public RelayCommand1 Save_BTN { get; set; }
        public RelayCommand1 ExitCommand { get; set; }
        public RelayCommand1 BackCommand { get; set; }
        public RelayCommand1 SendCode { get; set; }
        public PasswordVM(ForgetPassword fP)
        {
            Save_BTN = new RelayCommand1(Add, Verify);
            FP = fP;
            ExitCommand = new RelayCommand1(Exit, CanExit);
            BackCommand = new RelayCommand1(Back, CanExit);
            Random _random = new Random();
            _randomCode = _random.Next(100000, 1000000);
            SendCode = new RelayCommand1(Sends, CanSend);
        }
        private void Exit(object parameter)
        {
            FP.Close();
        }
        private bool CanExit(object parameter)
        {
            return true;
        }
        private string Concatenate()
        {
            ConcatenatedCode = FP.tboxCode1.Text + FP.tboxCode2.Text + FP.tboxCode3.Text + FP.tboxCode4.Text + FP.tboxCode5.Text + FP.tboxCode6.Text;
            return ConcatenatedCode;
        }
        private bool EqualityCheck()
        {
            if (FP.Passsword_TXT.Password.Length >= 6 && FP.Passsword_TXT.Password == FP.Passsword_TXT2.Password)
                return true;
            return false;
        }
        private bool Verify(object parameter) => Concatenate() == _randomCode.ToString() && EqualityCheck() && (FileFolder.BinEmailRead(FP.Email_TXT.Text, 1));

        private void Back(object parameter)
        {
            LoadingPanel LP = new LoadingPanel(1, 400, 500);
            LP.window = new Login();
            LP.Show(); FP.Close();
        }

        private void Sends(object parametr)
        {
            if (FileFolder.BinEmailRead(FP.Email_TXT.Text, 1))
            {

                Send();
                FP.lbl_ReceivedCode.Content = "ENTER CODE YOU RECEIVED";
                FP.lbl_ReceivedCode.Foreground = new SolidColorBrush(Colors.Black);

            }
            else
            {
                FP.lbl_ReceivedCode.Content = "This email doesn't exist in server !";
                FP.Foreground = new SolidColorBrush(Colors.Red);
                FP.lbl_ReceivedCode.Foreground = new SolidColorBrush(Colors.Red);
            }
        }
        private bool CanSend(object parameter)
        {
            if (Regex.IsMatch(FP.Email_TXT.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                return true;

            return false;
        }
        private void Send()
        {
            try
            {
                SmtpClient client = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = "sendcsharpemail@gmail.com",
                        Password = "stnizrmeyrdmenot"
                    }
                };
                MailAddress from = new MailAddress("sendcsharpemail@gmail.com", "Bravo Taksi Service");
                MailAddress to = new MailAddress(FP.Email_TXT.Text, "Someone");
                MailMessage Message = new MailMessage()
                {
                    From = from,
                    Subject = "Bravo Taksi  Password Reset code",
                    Body = "Your reset code for updating password: " + _randomCode.ToString()
                };
                Message.To.Add(to);
                client.Send(Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void Add(object parameter)
        {
            FileFolder.NewPassword(FP.Email_TXT.Text, FP.Passsword_TXT.Password);
            LoadingPanel LP = new LoadingPanel(1, 400, 500);
            LP.window = new Login();
            LP.Show(); FP.Close();
        }
    }
}
