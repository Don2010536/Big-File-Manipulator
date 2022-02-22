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
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Big_File_Manipulator.Pages
{
    // To do
    // Add the support for the status bar and tracking
    // Add logging for errors
    // Add an about section
    // Add an icon
    // Create a proper window title
    // Modify UI to more clearly convey filetypes at a glance in dirwrap
    // Break up the selectfolder btn click event
    // COMMENT THE CODE

    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        List<string> extensions = new();
        int fileCount = 0;
        int dirCount = 0;

        public Home()
        {
            InitializeComponent();
        }

        private void SelectFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Multiselect = false;
            dialog.Title = "Select a Directory";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                InitDir.Text = dialog.FileName;
                LocationManager.location = dialog.FileName;
            }

            TreeViewItem node = new()
            {
                Header = InitDir.Text.Split('\\')[^1],
                Foreground = Brushes.White
            };

            ExtensionStack.Children.Clear();

            _ = Task.Run(() => {
                GetDirInfo(Dispatcher.Invoke(() => { return InitDir.Text; }), true);

                int count = 0;

                foreach (string ext in extensions)
                {
                    count++;
                    if (count >= 200)
                    {
                        _ = Dispatcher.BeginInvoke((Action)(() =>
                        {
                            _ = ExtensionStack.Children.Add(new Button()
                            {
                                Content = "Only showing 200 extensions\nClick to View all...",
                                VerticalContentAlignment = VerticalAlignment.Center,
                                HorizontalContentAlignment = HorizontalAlignment.Center
                            });
                        }));
                        break;
                    } else
                    {
                        _ = Dispatcher.BeginInvoke((Action)(() =>
                        {
                            _ = ExtensionStack.Children.Add(new TextBlock()
                            {
                                Text = ext
                            });
                        }));
                    }
                }
            });
        }

        private void CopyBtn_Click(object sender, RoutedEventArgs e)
        {
            PerformFileOp((f, t) => { File.Copy(f, t); });
        }

        private void MoveBtn_Click(object sender, RoutedEventArgs e)
        {
            PerformFileOp((f, t) => { File.Move(f, t); });
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            BulkOperator.MassFileOp(LocationManager.location, false, (f, t) => { File.Delete(f); });
        }

        private void GetDirInfo(string loc, bool first = false)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(loc);
                string[] files = Directory.GetFiles(loc);

                if (first)
                    PopulateExplorer(dirs, files);

                fileCount += files.Length;
                dirCount += dirs.Length;

                foreach (string dir in dirs)
                    GetDirInfo(dir);

                PopulateExt(files);
                SetTotals();
            }
            catch { }
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            DirWrap.Children.Clear();

            string[] dirs = Directory.GetDirectories(InitDir.Text);
            string[] files = Directory.GetFiles(InitDir.Text);

            LocationManager.location = InitDir.Text;

            foreach (string dir in dirs)
            {
                _ = DirWrap.Children.Add(new Controls.ImageButton(dir.Split('\\')[^1], dir, ref DirWrap, ref InstructionTxt, true).Update());
            }

            foreach (string file in files)
            {
                _ = DirWrap.Children.Add(new Controls.ImageButton(file.Split('\\')[^1], file, ref DirWrap, ref InstructionTxt).Update());
            }

            InstructionTxt.Text = LocationManager.location;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LocationManager.location.Split('\\').Length > 1)
            {
                DirWrap.Children.Clear();

                string pos = string.Join('\\', LocationManager.location.Split('\\')[..^1]);

                string[] dirs = Directory.GetDirectories($"{pos}\\");
                string[] files = Directory.GetFiles($"{pos}\\");

                LocationManager.location = pos;

                foreach (string dir in dirs)
                {
                    _ = DirWrap.Children.Add(new Controls.ImageButton(dir.Split('\\')[^1], dir, ref DirWrap, ref InstructionTxt, true).Update());
                }

                foreach (string file in files)
                {
                    _ = DirWrap.Children.Add(new Controls.ImageButton(file.Split('\\')[^1], file, ref DirWrap, ref InstructionTxt).Update());
                }

                InstructionTxt.Text = LocationManager.location;
            }
        }

        private void PopulateExt(string[] files)
        {
            foreach (string file in files)
            {
                if (file.Split('.').Length > 1 && !extensions.Contains(file.Split('.')[^1]))
                {
                    extensions.Add(file.Split('.')[^1]);
                }
            }
        }

        private void SetTotals()
        {
            _ = Dispatcher.BeginInvoke((Action)(() =>
            {
                TDTxt.Text = $"Total Directories: {dirCount}";
                TFTxt.Text = $"Total Files: {fileCount}";
            }));
        }

        private void PopulateExplorer(string[] dirs, string[] files)
        {
            _ = Dispatcher.BeginInvoke((Action)(() =>
            {
                DirWrap.Children.Clear();

                DNRTxt.Text = $"Directories in Root: {dirs.Length}";
                FNRTxt.Text = $"Files in Root: {files.Length}";

                foreach (string dir in dirs)
                    _ = DirWrap.Children.Add(new Controls.ImageButton(dir.Split('\\')[^1], dir, ref DirWrap, ref InstructionTxt, true).Update());

                foreach (string file in files)
                    _ = DirWrap.Children.Add(new Controls.ImageButton(file.Split('\\')[^1], file, ref DirWrap, ref InstructionTxt).Update());

                InstructionTxt.Text = LocationManager.location;
            }));
        }

        private void PerformFileOp(Action<string, string> action)
        {
            if ((bool)new Dialogs.SelectDir().ShowDialog())
            {
                StatusGroup.Visibility = Visibility.Visible;
                MessageBoxResult res = MessageBox.Show("Would you like to preserve the directory structure?", "[QUESTION] - Preserve", MessageBoxButton.YesNoCancel);

                if (res != MessageBoxResult.Cancel)
                {
                    BulkOperator.MassFileOp(LocationManager.location, res == MessageBoxResult.Yes, action);
                }
                else
                {
                    StatusGroup.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
