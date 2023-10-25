using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace VlcTest
{
    public partial class Controls : UserControl
    {
        readonly Example1 parent;
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;
        string mediaFile = "demo.mp3";
        public Controls(Example1 Parent)
        {
            parent = Parent;

            InitializeComponent();
            mediaFile = Path.Combine(Environment.CurrentDirectory, mediaFile);
            // we need the VideoView to be fully loaded before setting a MediaPlayer on it.
            parent.VideoView.Loaded += VideoView_Loaded;
            PlayButton.Click += PlayButton_Click;
            StopButton.Click += StopButton_Click;
            Unloaded += Controls_Unloaded;
        }

        private void Controls_Unloaded(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Dispose();
            _libVLC.Dispose();
        }

        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            
            _libVLC = new LibVLC();
            _libVLC.Log += _libVLC_Log;
            _mediaPlayer = new MediaPlayer(_libVLC);
            _mediaPlayer.Paused += _mediaPlayer_Paused;
            _mediaPlayer.EndReached += _mediaPlayer_EndReached;
            _mediaPlayer.Stopped += _mediaPlayer_Stopped;
            _mediaPlayer.LengthChanged += _mediaPlayer_LengthChanged;
            _mediaPlayer.TimeChanged += _mediaPlayer_TimeChanged;

            parent.VideoView.MediaPlayer = _mediaPlayer;
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

        private void _libVLC_Log(object sender, LogEventArgs e)
        {
            Console.WriteLine(e.FormattedLog);
        }

        void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (parent.VideoView.MediaPlayer.IsPlaying)
            {
                parent.VideoView.MediaPlayer.Stop();
            }
        }

        void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!parent.VideoView.MediaPlayer.IsPlaying)
            {
                using (var media = new Media(_libVLC, new Uri(mediaFile)))
                    parent.VideoView.MediaPlayer.Play(media);
            }
        }
    }
}
