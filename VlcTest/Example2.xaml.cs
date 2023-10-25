using System;
using LibVLCSharp.Shared;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;
using System.IO;

namespace VlcTest
{
    public partial class Example2 : Window
    {
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;
        string mediaFile = "demo.mp3";

        public Example2()
        {
            InitializeComponent();
            mediaFile = Path.Combine(Environment.CurrentDirectory, mediaFile); 
            var label = new Label
            {
                Content = "TEST",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Foreground = new SolidColorBrush(Colors.Red)
            };
            test.Children.Add(label);
            // this will work fine
            _libVLC = new LibVLC("--file-caching=0", "--network-caching=0");
            //_libVLC = new LibVLC();
            _libVLC.Log += _libVLC_Log;
            _mediaPlayer = new MediaPlayer(_libVLC);
            _mediaPlayer.LengthChanged += _mediaPlayer_LengthChanged;
            _mediaPlayer.TimeChanged += _mediaPlayer_TimeChanged;
            _mediaPlayer.Paused += _mediaPlayer_Paused;
            _mediaPlayer.EndReached += _mediaPlayer_EndReached;
            _mediaPlayer.Stopped += _mediaPlayer_Stopped;
            // we need the VideoView to be fully loaded before setting a MediaPlayer on it.
            VideoView.Loaded += (sender, e) => VideoView.MediaPlayer = _mediaPlayer;
            Unloaded += Example2_Unloaded;
        }
        private void _mediaPlayer_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                TimeText.Text = TimeSpan.FromMilliseconds(e.Time).ToString(@"hh\:mm\:ss\.ff");

            }
            ));
        }

        private void _mediaPlayer_LengthChanged(object sender, MediaPlayerLengthChangedEventArgs e)
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                DurationText.Text = TimeSpan.FromMilliseconds(e.Length).ToString(@"hh\:mm\:ss\.ff");

            }
            ));
        }
        private void _libVLC_Log(object sender, LogEventArgs e)
        {
            Console.WriteLine(e.FormattedLog);
        }
        private void _mediaPlayer_Stopped(object sender, EventArgs e)
        {
            Console.WriteLine("_mediaPlayer_Stopped");
        }

        private void _mediaPlayer_EndReached(object sender, EventArgs e)
        {
            Console.WriteLine("_mediaPlayer_EndReached");
        }

        private void _mediaPlayer_Paused(object sender, EventArgs e)
        {
            Console.WriteLine("_mediaPlayer_Paused");
        }

        private void Example2_Unloaded(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Dispose();
            _libVLC.Dispose();
        }

        void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideoView.MediaPlayer.IsPlaying)
            {
                VideoView.MediaPlayer.Stop();
            }
        }

        void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!VideoView.MediaPlayer.IsPlaying)
            {
                using(var media = new Media(_libVLC, new Uri(mediaFile)))
                    VideoView.MediaPlayer.Play(media);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            VideoView.Dispose();
        }
    }
}
