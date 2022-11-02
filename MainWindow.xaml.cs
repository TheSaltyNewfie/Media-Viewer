using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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

namespace MediaViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool IsMediaPlaying;
        string previousFile;
        string currentPlayingFile;
        string nextFile;

        public MainWindow()
        {
            InitializeComponent();
            MediaView.LoadedBehavior = MediaState.Manual;
            IsMediaPlaying = false;
        }

        private void MediaView_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void MediaView_Drop(object sender, DragEventArgs e)
        {
            if (null != e.Data && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var data = e.Data.GetData(DataFormats.FileDrop) as string[];
                e.Handled = true;
                nextFile = data[0];
                DevLabel.Content = nextFile;
            }
        }

        private void MediaStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlayingFile == null)
            {
                currentPlayingFile = nextFile;
                MediaView.Source = new Uri(currentPlayingFile);
                //DevLabel.Content = currentPlayingFile;
            }

            if (!IsMediaPlaying)
            {
                MediaView.Play();
                IsMediaPlaying = true;
            }
            else
            {
                MediaView.Pause();
                IsMediaPlaying = false;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VolumeLabel.Content = VolumeSlider.Value.ToString();
            MediaView.Volume = VolumeSlider.Value;
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlayingFile == null || nextFile == null)
            {
                return;
            }
            else if(currentPlayingFile != nextFile)
            {
                previousFile = currentPlayingFile;
                currentPlayingFile = nextFile;
                MediaView.Source = MediaView.Source = new Uri(currentPlayingFile);
            }

        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlayingFile == null || nextFile == null)
                return;
            MediaView.Source = MediaView.Source = new Uri(previousFile);
        }
    }
}
