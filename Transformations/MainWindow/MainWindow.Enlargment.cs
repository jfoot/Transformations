using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Transformations
{
	public partial class MainWindow
	{
		///Enlargement of shapes
		///  - Allow a user to change the size of a shape by set scale factors
		///  - Allow the center of origin to specified 
		///	 - Draw ray lines of the enlargement.
		///	 - Allow the user to change the animation speed 
		///	 - Allow the user to show/ hide and delete ghosts
		/// 
		//Code for enlargement	
	
		private void EnlargementExecute(object sender, RoutedEventArgs e)
		{
			double[] enlAmounts = { -5, -4, -3, -2, -1, 0.25, 0.5, 0.75, 1, 2, 3, 4, 5 };
			if (SelectedShape != null)  //If the user has a shape selected
			{
				try
				{
					//Converts the user input into doubles
					double xCord = Convert.ToInt32(EnlargementXCenter.Text) * (ScaleFactor);
					double yCord = -Convert.ToInt32(EnlargementYCenter.Text) * (ScaleFactor);

					//Spawns the ghost and CofE point
					MyShapes.Add((new Circle("dupe_enlargement").MakerSpawn( xCord, yCord, MyCanvas)));
                    MyShapes.Add((new Ghost("dupe_enlargement").SpawnGhostShape(233, 255, 0, SelectedShape, MyCanvas, (bool)EnlargementGhostVisibality.IsChecked)));

				
                    //Create an animation
					DoubleAnimation myanimation = new DoubleAnimation(1, enlAmounts[enlargementAmount.SelectedIndex], new Duration(TimeSpan.FromSeconds(Times[enlargement_speed.SelectedIndex])));

					//Set the center of origin for enlargement
					MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterX = xCord - Canvas.GetLeft(MyShapes[MyShapes.Count - 1].MyShape);
					MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterY = yCord - Canvas.GetTop(MyShapes[MyShapes.Count - 1].MyShape);

					//set the scale for the enlargement
					MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleX = enlAmounts[enlargementAmount.SelectedIndex];
					MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleY = enlAmounts[enlargementAmount.SelectedIndex];

					//Start the animation
					MyShapes[MyShapes.Count - 1].MyScalingTransform.BeginAnimation(ScaleTransform.ScaleXProperty, myanimation);
					MyShapes[MyShapes.Count - 1].MyScalingTransform.BeginAnimation(ScaleTransform.ScaleYProperty, myanimation);
				}
				catch (Exception)   //Coordinates not in a numerical value 
				{
					MessageBox.Show("The coordinates entered were not in the correct format; only numerical values are allowed. " + Properties.Resources.UserError,
						"Invalid Input Type Error: 302 B", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
			else //No shape has been selected 
			{
				MessageBox.Show("You have not selected any shape.  " + Properties.Resources.UserError,
					"Field Empty Error: 300 D", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

        private void DrawRaysClick(object sender, RoutedEventArgs e)   //The user has clicked on the ray-lines button
		{
			if (SelectedShape != null && SelectedShape.GetType().ToString() == "System.Windows.Shapes.Polygon") //If a shape is selected and the shape is not a circle.
			{
				IsDrawingRays = true;         
				this.Cursor = Cursors.Cross;    //Change cursor
				
				MyRayLines.Add(new RayLines()); //Create a new ray-lines object
				MyRayLines[MyRayLines.Count - 1].RayLinesList.Add(new Line() {
					Stroke = Brushes.DarkOrange,
					StrokeThickness = 1
				});  //Add a line to the new ray-line object.
				Panel.SetZIndex(MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1], 2); //Make this new line on top of the canvas	
				MyCanvas.Children.Add(MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1]);
			}
			else
			{
				MessageBox.Show("You have not selected any shape or the selected shape is invalid.  " + Properties.Resources.UserError,
					"Field Empty Error: 300 E", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
        private void LineCaculator(double X, double Y)  //Used to calculate to position of the ray-lines so that they stretch to infinity
		{
			try
			{
                //The variables M and C are declared, so that they can be applied to the Y = MX + C equation, which is already used in the reflection.
				double m = (((Convert.ToDouble(Y) - Convert.ToDouble(Mouse.GetPosition(MyCanvas).Y)) / (Convert.ToDouble(X) - Convert.ToDouble(Mouse.GetPosition(MyCanvas).X))));
				double c = -(Convert.ToDouble(Y) - (m * Convert.ToDouble(X)));
				
				//Left
				MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].Y1 = (-(c) + ((MaxValue) * (m)));
				MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].X2 = -MaxValue;
				
				//Right
				MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].X1 = MaxValue;
				MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].Y2 = (-(c) - ((MaxValue) * (m)));

			}
			catch (Exception )
			{
				
			}
		}
        
		private void HideGhostEnlargement(object sender, RoutedEventArgs e)   //Hides all of the ghost enlargement
		{
			HideGhosts("enlargement");
            MyRayLines.ForEach(p => p.RayLinesList.ForEach(o => o.Visibility = Visibility.Hidden));
        }
		private void ShowGhostEnlargement(object sender, RoutedEventArgs e)   //Shows all of the ghost enlargement
		{
			ShowGhosts("enlargement");
            MyRayLines.ForEach(p => p.RayLinesList.ForEach(o => o.Visibility = Visibility.Visible));
		}
        private void DeleteRays(object sender, RoutedEventArgs e)      //Deletes all of the ray-lines from the grid
		{
            MyRayLines.ForEach(p => p.RayLinesList.ForEach(o => MyCanvas.Children.Remove(o)));
			MyRayLines.Clear();
		}
        private void DeleteEnlargementGhosts(object sender, RoutedEventArgs e)  //Deletes all the enlargement ghosts
		{
			DeleteGhosts("enlargement");
		}
	}
}