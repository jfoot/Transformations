using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Transformations
{
	public partial class MainWindow
	{
		/// Rotation of shapes
		/// - Allow the user to rotate a shape
		/// - Allow the user to specify the speed of rotation
        /// - Allow the user to specify the center or rotation
        /// - Allow the user to specify the direction and the amount.
		//Code for rotation
		private void RotationExecute(object sender, RoutedEventArgs e) //The user has started a rotation
		{
			int[] rotAmounts = { 45, 90, 180, 270, 360 };	//An array containing the rotation amounts from the drop down menu
			if (SelectedShape != null)  //if a shape is selected
			{
				try
				{
					//Converts the user input into doubles 
					double xCord = Convert.ToInt32(rotation_x_center.Text) * ScaleFactor;
					double yCord = -Convert.ToInt32(rotation_y_center.Text) * ScaleFactor;
					int rotAmount = rotationDirection.SelectedIndex == 0 ? rotAmounts[rotationAmount.SelectedIndex] : -rotAmounts[rotationAmount.SelectedIndex];


					//Spawn a marker onto the grid- to show the center of rotation
					MyShapes.Add((new Circle("dupe_rotation").MakerSpawn(xCord, yCord , MyCanvas)));
					//Spawn a new ghost shape
                    MyShapes.Add((new Ghost("dupe_rotation").SpawnGhostShape(0, 255, 0, SelectedShape, MyCanvas, (bool)rotationGhostVisibality.IsChecked)));

                    //Create  a new animation
					DoubleAnimation myanimation = new DoubleAnimation(0, rotAmount, new Duration(TimeSpan.FromSeconds(Convert.ToInt32(Times[rotationSpeed.SelectedIndex]))));

					//set the rotations center of origin
					MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterX = xCord - Canvas.GetLeft(MyShapes[MyShapes.Count - 1].MyShape);
					MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterY = yCord - Canvas.GetTop(MyShapes[MyShapes.Count - 1].MyShape);


					//Set the rotations angle and then execute it.
					MyShapes[MyShapes.Count - 1].MyRotateTransform.Angle = rotAmount;
					MyShapes[MyShapes.Count - 1].MyRotateTransform.BeginAnimation(RotateTransform.AngleProperty, myanimation);
				}
				catch (Exception)
				{
					MessageBox.Show(
						"The coordinates entered were not in the correct format; only numerical values are allowed. " + Properties.Resources.UserError,
						"Invalid Input Type Error: 302 D", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
			else
			{
				MessageBox.Show("You have not selected any shape. " + Properties.Resources.UserError,
					"Field Empty Error: 300 G", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
        private void ShowRotationGhosts(object sender, RoutedEventArgs e) //Show rotation ghosts
		{
			ShowGhosts("rotation");
		}
        private void HideRotationGhosts(object sender, RoutedEventArgs e) //Hide rotation ghosts
		{
			HideGhosts("rotation");
		}
        private void DeleteRotationGhosts(object sender, RoutedEventArgs e)//Delete rotation ghosts
		{
			DeleteGhosts("rotation");
		}
	}
}