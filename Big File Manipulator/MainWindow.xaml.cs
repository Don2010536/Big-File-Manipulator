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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Big_File_Manipulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _ = ContentFrame.Navigate(new Pages.Home());
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            _ = ContentFrame.Navigate(new Pages.Home());
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = null;
        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
