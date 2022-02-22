using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.Windows.Input;

namespace Big_File_Manipulator.Dialogs
{
    /// <summary>
    /// Interaction logic for SelectDir.xaml
    /// </summary>
    public partial class SelectDir : Window
    {
        public SelectDir()
        {
            InitializeComponent();
        }

        private void SelectFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Multiselect = false;
            dialog.Title = "To Directory";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                InitDir.Text = dialog.FileName;
                LocationManager.selectedDir = dialog.FileName;
            }
        }

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LocationManager.selectedDir == "")
            {
                _ = MessageBox.Show("You must have a directory selected", "[ERROR] - No Directory Selected", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } else
            {
                DialogResult = true;
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void InitDir_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LocationManager.selectedDir = InitDir.Text;
            }
        }
    }
}
