using Microsoft.AppCenter.Analytics;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace Transformations
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Load(object sender, RoutedEventArgs e) //Called when the window loads
        {
            Version.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();



            //Shape Colour
            if (Properties.Settings.Default.DefaultColour == Color.Blue)
            {
                blue_shape.IsChecked = true;
                shape_example.Fill = Brushes.Blue;
            }
            else if (Properties.Settings.Default.DefaultColour == Color.Red)
            {
                red_shape.IsChecked = true;
                shape_example.Fill = Brushes.Red;
            }
            else if (Properties.Settings.Default.DefaultColour == Color.Green)
            {
                green_shape.IsChecked = true;
                shape_example.Fill = Brushes.Green;
            }
            else if (Properties.Settings.Default.DefaultColour == Color.Black)
            {
                black_shape.IsChecked = true;
                shape_example.Fill = Brushes.Black;
            }

            //Grid Colour
            if (Properties.Settings.Default.DefaultGridColour == Color.LavenderBlush)
            {
                light_gray.IsChecked = true;
            }
            else if (Properties.Settings.Default.DefaultGridColour == Color.LightPink)
            {
                red.IsChecked = true;
            }
            else if (Properties.Settings.Default.DefaultGridColour == Color.LightGray)
            {
                dark_gray.IsChecked = true;
            }
            else if (Properties.Settings.Default.DefaultGridColour == Color.DarkSlateGray)
            {
                black_dark_gray.IsChecked = true;
            }
            else if (Properties.Settings.Default.DefaultGridColour == Color.Gray)
            {
                black_light_gray.IsChecked = true;
            }
            else if (Properties.Settings.Default.DefaultGridColour == Color.GhostWhite)
            {
                black_white.IsChecked = true;
            }



            //Resolution Setting
            if (Properties.Settings.Default.DefaultResolution == false)    //Low res
            {
                low_res.IsChecked = true;
                high_res.IsChecked = false;
            }
            else if (Properties.Settings.Default.DefaultResolution == true)
            {
                high_res.IsChecked = true;
                low_res.IsChecked = false;
            }

            //Performance Setting
            if (Properties.Settings.Default.DefaultPerformance == true)    //High performance
            {
                high_prof.IsChecked = true;
                low_prof.IsChecked = false;
            }
            else if (Properties.Settings.Default.DefaultPerformance == false)
            {
                high_prof.IsChecked = false;
                low_prof.IsChecked = true;
            }

           
            if (Properties.Settings.Default.UserTel)
                UserTel.IsChecked = true;
            else
                NoUserTel.IsChecked = true;

            if (Properties.Settings.Default.CrashTel)
                CrashTel.IsChecked = true;
            else
                NoCrashTel.IsChecked = true;



            size_slider.Value = (Properties.Settings.Default.DefaultHeight) / (75 / 5);
            size_label.Content = Properties.Strings.Size + ": " + size_slider.Value.ToString();
                       

            LangDrop.SelectedItem = LangDrop.Items.Cast<object>().Any(p => ((ComboBoxItem)p).Tag.Equals(Properties.Settings.Default.Language)) ? LangDrop.Items.Cast<object>().Single(p => ((ComboBoxItem)p).Tag.Equals(Properties.Settings.Default.Language)) : LangDrop.Items.Cast<object>().Single(p => ((ComboBoxItem)p).Tag.Equals("EN"));
        }
        private void BlueShapeChecked(object sender, RoutedEventArgs e)   //Blue colour checked
        {
            shape_example.Fill = Brushes.Blue;
        }
        private void RedShapeChecked(object sender, RoutedEventArgs e) //Red colour checked
        {
            shape_example.Fill = Brushes.Red;
        }
        private void GreenShapeChecked(object sender, RoutedEventArgs e) //Green colour checked
        {
            shape_example.Fill = Brushes.Green;
        }
        private void BlackShapeChecked(object sender, RoutedEventArgs e)//Black colour checked
        {
            shape_example.Fill = Brushes.Black;
        }
        private void ValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)   //Called upon when the size of shape slider is changed 
        {
            if (size_slider.IsLoaded)
            {
                size_label.Content = Properties.Strings.Size + ": " + size_slider.Value.ToString();
            }
        }
        private void ResetChanges(object sender, RoutedEventArgs e) //Reset settings to default button.
        {
            Analytics.TrackEvent("Reset Settings Changes");
            Properties.Settings.Default.Reset();
            this.Close();
            MessageBox.Show(Properties.Strings.SettingsReset);

            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
        private void ApplyChanges(object sender, RoutedEventArgs e) //Apply setting changes 
        {
            bool valid = true;

            //Shape Colours
            if (blue_shape.IsChecked == true)
            {
                Properties.Settings.Default.DefaultColour = Color.Blue;
            }
            else if (red_shape.IsChecked == true)
            {
                Properties.Settings.Default.DefaultColour = Color.Red;
            }
            else if (green_shape.IsChecked == true)
            {
                Properties.Settings.Default.DefaultColour = Color.Green;
            }
            else if (black_shape.IsChecked == true)
            {
                Properties.Settings.Default.DefaultColour = Color.Black;
            }

            //Grid Colour
            if (light_gray.IsChecked == true)
            {
                Properties.Settings.Default.DefaultGridColour = Color.LavenderBlush;
                Properties.Settings.Default.DarkMode = false;
            }
            else if (dark_gray.IsChecked == true)
            {
                Properties.Settings.Default.DefaultGridColour = Color.LightGray;
                Properties.Settings.Default.DarkMode = false;
            }
            else if (red.IsChecked == true)
            {
                Properties.Settings.Default.DefaultGridColour = Color.LightPink;
                Properties.Settings.Default.DarkMode = false;
            }
            else if (black_dark_gray.IsChecked == true)
            {
                Properties.Settings.Default.DefaultGridColour = Color.DarkSlateGray;
                Properties.Settings.Default.DarkMode = true;
            }
            else if (black_light_gray.IsChecked == true)
            {
                Properties.Settings.Default.DefaultGridColour = Color.Gray;
                Properties.Settings.Default.DarkMode = true;
            }
            else if (black_white.IsChecked == true)
            {
                Properties.Settings.Default.DefaultGridColour = Color.GhostWhite;
                Properties.Settings.Default.DarkMode = true;
            }



            //Resolution Setting
            if (low_res.IsChecked == true)
            {
                Properties.Settings.Default.DefaultResolution = false;
            }
            else if (high_res.IsChecked == true)
            {
                Properties.Settings.Default.DefaultResolution = true;
            }

            //Performance Setting
            if (high_prof.IsChecked == true)
            {
                Properties.Settings.Default.DefaultPerformance = true;
            }
            else if (low_prof.IsChecked == true)
            {
                Properties.Settings.Default.DefaultPerformance = false;
            }
            

            if(CrashTel.IsChecked == true)
                Properties.Settings.Default.CrashTel = true;
            else
                Properties.Settings.Default.CrashTel = false;

            if (UserTel.IsChecked == true)
                Properties.Settings.Default.UserTel = true;
            else
                Properties.Settings.Default.UserTel = false;

            Properties.Settings.Default.DefaultHeight = Convert.ToInt32(size_slider.Value * (75 / 5));

            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo(((ComboBoxItem)LangDrop.SelectedItem).Tag.ToString());


           

            Properties.Settings.Default.Language = ((ComboBoxItem)LangDrop.SelectedItem).Tag.ToString();

            if (valid)
            {
                Analytics.TrackEvent("Saved Settings", new System.Collections.Generic.Dictionary<string, string> {
                    { "Height",  Properties.Settings.Default.DefaultHeight.ToString() },
                    { "Shape Colour",   Properties.Settings.Default.DefaultColour.ToString()},
                    { "Grid Colour",  Properties.Settings.Default.DefaultGridColour.ToString() },
                    { "Resoultion",  Properties.Settings.Default.DefaultResolution.ToString() },
                    { "Performance",  Properties.Settings.Default.DefaultPerformance.ToString() },
                    { "Dark Mode",  Properties.Settings.Default.DarkMode.ToString() },
                    { "Language",  Properties.Settings.Default.Language.ToString() },
                    { "UserTel",  Properties.Settings.Default.UserTel.ToString() },
                    { "CrashTel",  Properties.Settings.Default.CrashTel.ToString() }
                });
                

                Properties.Settings.Default.Save();
                this.Close();
                MessageBox.Show(Properties.Strings.SettingsSaved);

                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

      
       

        private void PrivacyPolicyView(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Analytics.TrackEvent("View Privacy Policy");
            System.Diagnostics.Process.Start(Properties.Strings.PPolicyLink);
        }
    }
}
