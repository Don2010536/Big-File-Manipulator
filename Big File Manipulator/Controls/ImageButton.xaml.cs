using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Big_File_Manipulator.Controls
{
    /// <summary>
    /// Interaction logic for ImageButton.xaml
    /// </summary>
    public partial class ImageButton : UserControl
    {
        string path;
        string assets = $"{Environment.CurrentDirectory}\\Assets";
        string label;
        bool folder;
        WrapPanel panel;
        TextBlock txt;

        public ImageButton(string label, string path, ref WrapPanel panel, ref TextBlock txt, bool folder = false)
        {
            InitializeComponent();

            this.label = label;
            this.path = path;
            this.folder = folder;
            this.panel = panel;
            this.txt = txt;
        }

        public ImageButton Update()
        {
            FileImage.Source = new BitmapImage(new Uri(folder ? $"{assets}\\Folder.png" : $"{assets}\\File.png"));
            Label.Text = label;
            BorderHighlight.ToolTip = label;

            return this;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderHighlight.BorderBrush = Brushes.White;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderHighlight.BorderBrush = Brushes.Transparent;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (folder)
            {
                panel.Children.Clear();

                try
                {
                    string[] dirs = Directory.GetDirectories(path);
                    string[] files = Directory.GetFiles(path);

                    LocationManager.location = path;

                    foreach (string dir in dirs)
                    {
                        _ = panel.Children.Add(new ImageButton(dir.Split('\\')[^1], dir, ref panel, ref txt, true).Update());
                    }

                    foreach (string file in files)
                    {
                        _ = panel.Children.Add(new ImageButton(file.Split('\\')[^1], file, ref panel, ref txt).Update());
                    }

                    txt.Text = LocationManager.location;
                } catch { }
            } else
            {
                Process.Start("explorer.exe", path);
            }
        }
    }
}
