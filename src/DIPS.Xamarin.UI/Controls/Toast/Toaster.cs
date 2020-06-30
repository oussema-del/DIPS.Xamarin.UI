using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DIPS.Xamarin.UI.Controls.Toast
{
    public enum ToastState
    {
        Closed = 0,
        Closing,
        Opened
    }

    public class Toaster : BindableObject
    {
        private Toaster()
        {
        }

        private static ToastState ToastState { get; set; }
        private static Guid? Id { get; set; }
        private CancellationTokenSource CancellationSource { get; set; } = new CancellationTokenSource();

        public static Toaster Current { get; } = new Toaster();

        private Toast GetToast()
        {
            var toast = new Toast();

            toast.SetBinding(Toast.TextProperty, new Binding(nameof(Text), source: this));
            toast.SetBinding(Toast.FontSizeProperty, new Binding(nameof(FontSize), source: this));
            toast.SetBinding(Toast.FontFamilyProperty, new Binding(nameof(FontFamily), source: this));
            toast.SetBinding(Toast.TextColorProperty, new Binding(nameof(TextColor), source: this));
            toast.SetBinding(Toast.BackgroundColorProperty, new Binding(nameof(BackgroundColor), source: this));
            toast.SetBinding(Toast.CornerRadiusProperty, new Binding(nameof(CornerRadius), source: this));
            toast.SetBinding(Toast.PaddingProperty, new Binding(nameof(Padding), source: this));
            toast.SetBinding(Toast.PositionYProperty, new Binding(nameof(PositionY), source: this));

            return toast;
        }

        private async Task HideToasterIn(int timeInSeconds)
        {
            await Task.Delay(timeInSeconds * 1000, CancellationSource.Token);

            await HideToaster();
        }

        public async Task ShowToaster(View toaster = null)
        {
            // check state closed
            if (ToastState != ToastState.Closed)
            {
                CancellationSource.Cancel();
                CancellationSource = new CancellationTokenSource();
                await HideToasterIn(HideToastIn);
                return;
            }

            ToastState = ToastState.Opened;
            CancellationSource = new CancellationTokenSource();

            // get current page
            var currentPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
            if (!(currentPage is ContentPage))
            {
                return;
            }

            // old content
            var oldContent = ((ContentPage)currentPage).Content;

            // arrange toast view
            var toastView = toaster == null ? GetToast() : toaster;
            toastView.Opacity = 0;

            // tap command
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) =>
            {
                _ = HideToasterIn(0);
            };
            toastView.GestureRecognizers.Add(tapGesture);

            // arrange new content
            var newContent = new Grid {Children = {oldContent, toastView}};
            Id = newContent.Id;

            // set new content
            ((ContentPage)currentPage).Content = newContent;

            // animate toast
            await toastView.FadeTo(1, (uint)AnimateFor, Easing.Linear);

            // hide toast
            if (HideToastIn > 0)
            {
                await HideToasterIn(HideToastIn);
            }
        }

        public async Task HideToaster()
        {
            // check state closed
            if (ToastState != ToastState.Opened)
            {
                return;
            }

            ToastState = ToastState.Closing;
            CancellationSource.Cancel();

            // get current page
            var currentPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
            if (!(currentPage is ContentPage))
            {
                return;
            }

            // get toast container grid
            var toastContent = ((ContentPage)currentPage).Content;
            if (toastContent.Id != Id || !(toastContent is Grid))
            {
                return;
            }

            // get toast view
            var toastGrid = (Grid)toastContent;
            var toastView = toastGrid.Children.Last();

            // animate toast
            await toastView.FadeTo(0, (uint)AnimateFor, Easing.Linear);

            // remove toast
            toastGrid.Children.Remove(toastView);
            ((ContentPage)currentPage).Content = toastGrid.Children.First();

            // set toast state
            ToastState = ToastState.Closed;
        }

        #region Public Properties

        /// <summary>
        ///     Animate the appearing and disappearing of the toaster for the given milliseconds
        /// </summary>
        public int AnimateFor { get; set; } = 250;

        /// <summary>
        ///     Hide the toaster automatically after the given seconds
        ///     <remarks> If value is 0, toaster won't be hide automatically </remarks>
        /// </summary>
        public int HideToastIn { get; set; } = 5;

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public float CornerRadius
        {
            get => (float)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        public double PositionY
        {
            get => (double)GetValue(PositionYProperty);
            set => SetValue(PositionYProperty, value);
        }

        #endregion

        #region Bindable Properties

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(Toaster), Label.TextProperty.DefaultValue);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(Toaster),
                Label.FontSizeProperty.DefaultValue,
                defaultValueCreator: FontSizeDefaultValueCreator);

        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(Toaster),
                Label.FontFamilyProperty.DefaultValue);

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(Toaster),
                Label.TextColorProperty.DefaultValue);

        public static readonly BindableProperty BackgroundColorProperty =
            BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(Toaster), Color.Default);

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius),
            typeof(float), typeof(Toaster), -1f,
            validateValue: OnCornerRadiusValidate);

        public static readonly BindableProperty PaddingProperty =
            BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(Toaster), new Thickness(5, 5, 5, 5));

        public static readonly BindableProperty PositionYProperty =
            BindableProperty.Create(nameof(PositionY), typeof(double), typeof(Toaster), 10d);

        private static object FontSizeDefaultValueCreator(BindableObject bindable)
        {
            return Device.GetNamedSize(NamedSize.Default, typeof(Toast));
        }

        private static bool OnCornerRadiusValidate(BindableObject bindable, object value)
        {
            if (value is float f)
            {
                return (int)f == -1 || f >= 0f;
            }

            return false;
        }

        #endregion
    }
}