using System.Windows;
using Memorizer.Logic;
using Memorizer.Utility;
using Microsoft.Win32;

namespace Memorizer.Forms
{
    /// <summary>
    /// Interaction logic for AddWPF.xaml
    /// </summary>
    public partial class AddWpf
    {
        public MainLogic Logic { get; set; }

        public AddWpf(MainLogic logic)
        {
            Logic = logic;

            InitializeComponent();
        }

        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ShowLoading();

            await Logic.Add(TextBoxQuestion.Text, TextBoxAnswer.Text);

            HideLoading();
        }

        private async void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            var importFileDialog = new OpenFileDialog
            {
                Title = "Select the file",
                FileName = "new.txt",
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                //InitialDirectory = @"E:\Документы\Desktop\importData"
            };

            if (importFileDialog.ShowDialog() != true)
                return;

            if (string.IsNullOrEmpty(TextBoxSeparator.Text))
            {
                MessageBox.Show(
                    Extensions.GetLocalizedValue("MessageDefineSeparator"),
                    Extensions.GetLocalizedValue("MessageWarning"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ShowLoading();

            await Logic.ImportFromFile(importFileDialog.FileName, TextBoxSeparator.Text);

            HideLoading();
        }

        private void HideLoading()
        {
            Loading.Visibility = Visibility.Hidden;
        }

        private void ShowLoading()
        {
            Loading.Visibility = Visibility.Visible;
        }
    }
}
