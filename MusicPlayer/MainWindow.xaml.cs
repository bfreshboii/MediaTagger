using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Create new media player
        private MediaPlayer mediaPlayer = new MediaPlayer();

        //Create new tag lib file
        private TagLib.File currentFile;

        //Create new file dialouge
        OpenFileDialog fileDlg = new OpenFileDialog();

        //Create new edit tag object
        EditTag edit = new EditTag();
        public MainWindow()
        {
            InitializeComponent();
        }
        
        //File open button click event
        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            
            //Create a file filter
            fileDlg.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            
            //if the dialog shows then populate the control with the tags
            if (fileDlg.ShowDialog() == true)
            {
                try
                {
                    //open the media player object      
                    mediaPlayer.Open(new Uri(fileDlg.FileName));
                    NowPlaying playing = new NowPlaying();
                    UC_Panel.Children.Add(playing);

                    //Set the file but to be enabled
                    EditFile.IsEnabled = true;

                    //if the file is not null then populate the fields
                    if (currentFile != null)
                    {
                        playing.Title.Text = currentFile.Tag.Title.ToString();
                        playing.Album.Text = currentFile.Tag.Album.ToString();
                        playing.Artist.Text = currentFile.Tag.AlbumArtists.ToString();
                        playing.Year.Text = currentFile.Tag.Year.ToString();
                    }
                }
                catch 
                {
                    NowPlaying playing = new NowPlaying();
                    UC_Panel.Children.Add(playing);

                    if (currentFile != null)
                    {
                        playing.Title.Text = "Title";
                        playing.Album.Text = "Album";
                        playing.Artist.Text = "Artist";
                        playing.Year.Text = "Year";
                    }

                }

                //new timer object
                //cited from https://learn.microsoft.com/en-us/dotnet/api/system.windows.threading.dispatchertimer?view=windowsdesktop-7.0
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1); //set the interval to count by 1
                timer.Tick += timer_Tick; // add it to the function for each tick
                timer.Start(); //start the timer

            }
            
        }

        //timer tick function
        void timer_Tick(object sender, EventArgs e) 
        {
            //checks if the player source is null 
            if (mediaPlayer.Source != null)
            {
                //sets the timer status to the label
                lblStatus.Content = String.Format("{0} / {1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            else
            {
                lblStatus.Content = "No file....";
            }
        }
        
        //edit file click event
        private void EditFile_Click_1(object sender, RoutedEventArgs e)
        {
            //add the user control to the panel
            UCE_Panel.Children.Add(edit);
            try
            {
                if (currentFile != null)
                {
                    edit.Title.Text = currentFile.Tag.Title.ToString();
                    edit.Album.Text = currentFile.Tag.Album.ToString();
                    edit.Artist.Text = currentFile.Tag.AlbumArtists.ToString();
                    edit.Year.Text = currentFile.Tag.Year.ToString();
                }
            }
            catch (Exception ex)
            {
                
                    

                    if (currentFile != null)
                    {
                       edit.Title.Text = "Title";
                       edit.Album.Text = "Album";
                       edit.Artist.Text = "Artist";
                       edit.Year.Text = "Year";
                    }
                
            }
            //set the save button to be enabled
            SaveFile.IsEnabled= true;
            
            
        }

        //save button clicked event
        public void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            //close the media player object
            mediaPlayer.Close();

            //set the current file to the path of the file
            currentFile = TagLib.File.Create(fileDlg.FileName);

            //set the tags to the each field
            currentFile.Tag.Album = edit.Album.Text;
            currentFile.Tag.Title = edit.Title.Text;

            //create Year variable
            uint Year = 0;
            //if the user enters a value that is not an int the set the year to a default value
            try
            {
                Year = (uint)int.Parse(edit.Year.Text.ToString());
                Year.ToString();
                currentFile.Tag.Year = Year;
            }
            catch 
            {
                Year = 2000;
                Year.ToString();
                currentFile.Tag.Year = Year;
            }
         
            //convert the string to an array 
            String Artists = edit.Artist.Text;
            currentFile.Tag.AlbumArtists = Artists.Split(' ');

            //save the current file
            currentFile.Save();

            //set the button to not be enabled
            SaveFile.IsEnabled= false;

            //re-open the file
            mediaPlayer.Open(new Uri(fileDlg.FileName));

            //remove the panel
            UCE_Panel.Children.Remove(edit);
        }

        private void PlayCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mediaPlayer.Source != null);
        }

        private void PlayCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Play();

            
        }

        private void PauseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mediaPlayer.Source != null);
        }

        private void PauseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void StopCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mediaPlayer.Source != null);
        }

        private void StopCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        
    }
}
