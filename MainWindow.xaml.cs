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
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using WMPLib;


namespace MediaViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool IsMediaPlaying;
        List<string> fileQueue = new List<string>();
        int queuePosition = 0;
        double videoLength = 0.0;
        double currentVideoPos = 0.0;
        System.Threading.Thread slider;

        public MainWindow()
        {
            InitializeComponent();
            MediaView.LoadedBehavior = MediaState.Manual;
            IsMediaPlaying = false;
            //slider = new System.Threading.Thread(updateSliderTime);
            //slider.IsBackground = true;
            //slider.Start();
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
                //nextFile = data[0];
                //DevLabel.Content = nextFile;
                for(int i = 0; i < data.Length; ++i)
                {
                    fileQueue.Add(data[i]);
                    Console.WriteLine($"Added {data[i]} to queue!");
                }
                MediaView.Source = new Uri(fileQueue[0]);
                MediaView.Play();
                IsMediaPlaying = true;
                SetSliderTime();
                DevLabel.Content = MediaView.DataContext.ToString();
            }
        }

        private void MediaStateButton_Click(object sender, RoutedEventArgs e)
        {
            if(fileQueue.Count == 0)
            {
                return;
            }

            if (currentVideoPos == videoLength)
            {
                MediaView.Source = new Uri(fileQueue[queuePosition]);
            }
            else if (!IsMediaPlaying)
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
            VolumeLabel.Content = Math.Round(VolumeSlider.Value, 1).ToString();
            MediaView.Volume = Math.Round(VolumeSlider.Value, 1);
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            if(fileQueue.Count == 0)
                return;

            if (queuePosition == fileQueue.Count -1)
            {
                return;
            }
            else
            {
                queuePosition += 1;
                MediaView.Source = new Uri(fileQueue[queuePosition]);
                SetSliderTime();
            }
            /*
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
            */

        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (fileQueue.Count == 0)
                return;
            if (queuePosition == 0)
                return;

            queuePosition -= 1;
            MediaView.Source = new Uri(fileQueue[queuePosition]);
            SetSliderTime();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            MediaView.Source = null;
            fileQueue.Clear();
            queuePosition = 0;
            Console.WriteLine("Cleared File Queue!");
        }

        private void SetSliderTime()
        {
            MediaView.MediaOpened += MediaView_MediaOpened;

            //DevLabel.Content = VideoDurationSlider.Maximum;

            //while(IsMediaPlaying)
            //{
              //  VideoDurationSlider.Value = currentVideoPos;
            //}
        }

        private void MediaView_MediaOpened(object sender, RoutedEventArgs e)
        {
            videoLength = 0.0;
            currentVideoPos = 0.0;

            videoLength = MediaView.NaturalDuration.TimeSpan.TotalSeconds;
            
            VideoDurationSlider.Maximum = videoLength;
        }

        private void updateSliderTime()
        {
            while(true)
            {
                this.Dispatcher.Invoke(() => {
                    currentVideoPos = MediaView.Position.TotalSeconds;
                    VideoDurationSlider.Value = currentVideoPos;
                });
            }
        }
    }
}
