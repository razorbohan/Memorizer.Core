using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Memorizer.Logic;
using Memorizer.Utility;
using WPFLocalizeExtension.Engine;

namespace Memorizer.Forms
{
    /// <summary>
    /// Interaction logic for MainWPFForm.xaml
    /// </summary>
    public partial class MainWpfForm
    {
        //TODO Update design
        private MainLogic Logic { get; set; }

        public MainWpfForm()
        {
            LocalizeDictionary.Instance.Culture = new CultureInfo("en");
            InitializeComponent();

            Logger.LogControl = LogBox;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Logic = new MainLogic();
            this.DataContext = Logic;

            await SwitchMode(true);
        }

        private async void MenuItemRepeat_Click(object sender, RoutedEventArgs e)
        {
            ClearView();

            await SwitchMode(true);

            MenuItemRepeat.IsEnabled = false;
            MenuItemLearn.IsEnabled = true;
        }

        private async void MenuItemLearn_Click(object sender, RoutedEventArgs e)
        {
            ClearView();

            await SwitchMode(false);

            MenuItemRepeat.IsEnabled = true;
            MenuItemLearn.IsEnabled = false;
        }

        private void ButtonShowAnswer_Click(object sender, RoutedEventArgs e)
        {
            ShowAnswer();
        }

        private async void ButtonBad_Click(object sender, RoutedEventArgs e) => await SubmitAnswer(MemoAnswerType.Bad);
        private async void ButtonTomorrow_Click(object sender, RoutedEventArgs e) => await SubmitAnswer(MemoAnswerType.Tomorrow);
        private async void ButtonCool_Click(object sender, RoutedEventArgs e) => await SubmitAnswer(MemoAnswerType.Cool);
        private async void ButtonLater_Click(object sender, RoutedEventArgs e) => await SubmitAnswer(MemoAnswerType.Later);

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            new AddWpf(Logic)
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false
            }.ShowDialog();
        }

        private async void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            ShowLoading();

            await Logic.Update(TextBoxQ.Text, TextBoxA.Text);

            HideLoading();
        }

        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            ShowLoading();

            if (await Logic.Delete())
            {
                ShowQuestion();
            }

            HideLoading();
        }

        private void ButtonFind_Click(object sender, RoutedEventArgs e)
        {
            new FindWpf(Logic)
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false
            }.ShowDialog();
        }

        private void MenuItemLanguageEn_Click(object sender, RoutedEventArgs e) => SwitchCulture("en-US");
        private void MenuItemLanguageRu_Click(object sender, RoutedEventArgs e) => SwitchCulture("ru-RU");

        private void LogBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LogBox.ScrollToEnd();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async Task SwitchMode(bool isRepeat)
        {
            ShowLoading();

            if (await Logic.InitializeMemos(isRepeat))
            {
                ShowQuestion();

                ButtonTomorrow.IsEnabled = isRepeat;
                ButtonBad.IsEnabled = isRepeat;
            }

            HideLoading();
        }

        private static void SwitchCulture(string cultureName)
        {
            try
            {
                LocalizeDictionary.Instance.Culture = new CultureInfo(cultureName);
            }
            catch
            {
                LocalizeDictionary.Instance.Culture = CultureInfo.InvariantCulture;
            }
        }

        private void ShowQuestion()
        {
            TextBoxQ.Text = "";
            TextBoxA.Text = "";
            AnswerPanel.IsEnabled = false;

            if (Logic.CurrentMemo == null)
                return;

            TextBoxQ.Text = $"\r\n{Logic.CurrentMemo.Question}";
            LabelLevelValue.Content = Logic.CurrentMemo.PostponeLevel.ToString();
            LabelScoresValue.Content = Logic.CurrentMemo.Scores.ToString();
            ButtonShow.Focus();
        }

        private void ShowAnswer()
        {
            if (Logic.CurrentMemo == null)
                return;

            TextBoxA.Text = Logic.CurrentMemo.Answer;
            AnswerPanel.IsEnabled = true;
            ButtonCool.Focus();
        }

        private async Task SubmitAnswer(MemoAnswerType memoAnswerType)
        {
            ShowLoading();
            await Logic.SubmitAnswer(memoAnswerType);
            HideLoading();

            ShowQuestion();
        }

        private void HideLoading()
        {
            Loading.Visibility = Visibility.Hidden;
        }

        private void ShowLoading()
        {
            Loading.Visibility = Visibility.Visible;
        }

        private void ClearView()
        {
            TextBoxQ.Text = "";
            TextBoxA.Text = "";
            Logic.MemoLeftCount = 0;
            LabelLevelValue.Content = "0";
            LabelScoresValue.Content = "0";
        }
    }
}
