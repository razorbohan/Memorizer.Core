using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Memorizer.Logic;
using Memorizer.Model;
using Memorizer.Utility;

namespace Memorizer.Forms
{
    /// <summary>
    /// Interaction logic for FindWPF.xaml
    /// </summary>
    public partial class FindWpf
    {
        //TODO delete & edit from DataGridView
        public MainLogic Logic { get; set; }

        public FindWpf(MainLogic logic)
        {
            Logic = logic;

            InitializeComponent();
        }

        private async void ButtonFind_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowLoading();
                var searchMemoList = (await Logic.Find(TextBoxFind.Text)).ToList();
                HideLoading();

                DataGrid.ItemsSource = searchMemoList;
                LabelTotal.Content = searchMemoList.Count;

                Logger.Log($"{Extensions.GetLocalizedValue("LoggerFound")}: {searchMemoList.Count}");
            }
            catch (Exception ex)
            {
                Logger.ErrorLog(ex);
            }
        }

        private async void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataGrid.SelectedItem is Memo currentMemo)) return;

            ShowLoading();
            await Logic.Update(currentMemo);
            HideLoading();
        }

        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataGrid.SelectedItem is Memo currentMemo)) return;

            ShowLoading();
            await Logic.Delete(currentMemo.Id);
            HideLoading();
        }

        private void TextBoxFind_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                    ButtonFind.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.Escape:
                    this.Close();
                    break;
            }
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
