using System.Threading.Tasks;
using System.Windows.Input;
using DIPS.Xamarin.UI.Controls.Toast;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Color = System.Drawing.Color;

namespace DIPS.Xamarin.UI.Samples.Controls.Toast
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ToastPage : ContentPage
    {
        private string m_pageTitle;
        private ICommand m_showToastCommand;
        private ICommand m_showToastCommand2;
        private ICommand m_showToastCommand3;

        public ToastPage()
        {
            InitializeComponent();

            ConfigureToast();

            PageTitle = "Hello, World!";

            ShowToastCommand = new Command(parameter =>
            {
                Toaster.Current.HideToastIn = 5;
                Toaster.Current.ToastAction = async () =>
                {
                    PageTitle = "Hello, Mercury!";
                    await Task.Delay(1500);
                    PageTitle = "Hello, World!";
                };
                Toaster.Current.Text = parameter.ToString();

                Toaster.Current.DisplayToast();
            });

            ShowToastCommand2 = new Command(parameter =>
            {
                Toaster.Current.HideToastIn = 0;
                Toaster.Current.ToastAction = null;
                Toaster.Current.Text = parameter.ToString();

                Toaster.Current.DisplayToast();
            });

            ShowToastCommand3 = new Command(parameter =>
            {
                Toaster.Current.HideToastIn = 5;
                Toaster.Current.ToastAction = async () =>
                {
                    await Toaster.Current.CancelToast();
                };
                Toaster.Current.Text = parameter.ToString();

                Toaster.Current.DisplayToast();
            });
        }

        public string PageTitle
        {
            get => m_pageTitle;
            set
            {
                m_pageTitle = value;
                OnPropertyChanged(nameof(PageTitle));
            }
        }

        public ICommand ShowToastCommand
        {
            get => m_showToastCommand;
            set
            {
                m_showToastCommand = value;
                OnPropertyChanged(nameof(ShowToastCommand));
            }
        }

        public ICommand ShowToastCommand2
        {
            get => m_showToastCommand2;
            set
            {
                m_showToastCommand2 = value;
                OnPropertyChanged(nameof(ShowToastCommand2));
            }
        }

        public ICommand ShowToastCommand3
        {
            get => m_showToastCommand3;
            set
            {
                m_showToastCommand3 = value;
                OnPropertyChanged(nameof(ShowToastCommand3));
            }
        }

        private static void ConfigureToast()
        {
            Toaster.Current.FontSize = 13;
            Toaster.Current.TextColor = Color.White;
            Toaster.Current.BackgroundColor = Color.DodgerBlue;
            Toaster.Current.CornerRadius = 8;
            Toaster.Current.Padding = new Thickness(20, 10);
            Toaster.Current.PositionY = 30;
            Toaster.Current.AnimateFor = 500;
            Toaster.Current.HideToastIn = 5;
            Toaster.Current.LineBreakMode = LineBreakMode.TailTruncation;
            Toaster.Current.MaxLines = 1;
        }
    }
}