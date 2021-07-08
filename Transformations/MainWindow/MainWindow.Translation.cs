using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Transformations
{
    public partial class MainWindow
	{
		///Translation of shapes
		/// - Allow the user to translate shapes
        /// - Allow the user to define the speed of translation
		/// - Allow the user to hide/ show ghosts and delete them
		// Code for translation
		private void TranslationExecute(object sender, RoutedEventArgs e)  //When the execute button is pressed
		{
			if (SelectedShape != null)  //If there is a shape selected
			{
				try
				{
					//Turns the user input into its pixel values as a double.
					double xVector = Convert.ToDouble(transX.Text) * ScaleFactor;
					double yVector = -Convert.ToDouble(transY.Text) * ScaleFactor;
				
					//Spawns a new ghost shape
					MyShapes.Add((new Ghost("dupe_translation").SpawnGhostShape(142, 0, 217, SelectedShape, MyCanvas, (bool)translationGhostVisibality.IsChecked)));
                   
					//Create a new animation for the X direction
                    DoubleAnimation xAnimation = new DoubleAnimation(0, xVector, TimeSpan.FromSeconds(Times[transSpeed.SelectedIndex]));
					MyShapes[MyShapes.Count - 1].MyTranslateTransform.BeginAnimation(TranslateTransform.XProperty, xAnimation);
                   
					//Create a new animation for the Y direction
                    DoubleAnimation yAnimation = new DoubleAnimation(0 , yVector, TimeSpan.FromSeconds(Times[transSpeed.SelectedIndex]));
					MyShapes[MyShapes.Count - 1].MyTranslateTransform.BeginAnimation(TranslateTransform.YProperty, yAnimation);
                    Analytics.TrackEvent("Translation Executed");
                }
                catch (Exception ex)	//If user input could not been turned into a double.
				{
                    Crashes.TrackError(ex);
                    MessageBox.Show(
						Properties.Strings.VectorNotInFormat,
						Properties.Strings.EM_InvalidInputTypeError + "302 E", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
			else	//If User did not select any shape
			{
                Analytics.TrackEvent("Translation No Shape Selected");
                MessageBox.Show(Properties.Strings.NoShapeSelected1 + Properties.Strings.UserError,
                    Properties.Strings.EM_FieldEmpty + "300 H", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void HideTranslationGhosts(object sender, RoutedEventArgs e)      //Hide translation ghosts
        {
            Analytics.TrackEvent("Hide Translation Ghost");
            HideGhosts("translation");
		}
		private void ShowTranslationGhosts(object sender, RoutedEventArgs e)       //Show translation ghosts
		{
            ShowGhosts("translation");
		}
		private void DeleteTranslationGhosts(object sender, RoutedEventArgs e)     //Delete translation ghosts
        {
            Analytics.TrackEvent("Delete Translation Ghost");
            DeleteGhosts("translation");
		}
    }
}