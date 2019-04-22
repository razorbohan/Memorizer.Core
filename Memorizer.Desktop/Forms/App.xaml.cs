using System;
using System.Windows;

namespace Memorizer.Forms
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    
    public partial class App
    {
        [STAThread]
        static void Main()
        {
            var app = new Application();
            var window = new MainWpfForm();
            app.Run(window);
        }
    }
}
