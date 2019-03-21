using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Xml;
using MessageBox = System.Windows.MessageBox;
/// <summary>
/// Shape Properties contains a variety of different functions preformed onto shapes including:
/// - Selecting a shape
/// - Highlighting the shape
/// - Deleting a shape
/// - Un-selecting a shape
/// - Deleting all ghost shapes
/// - Deleting ghosts based on their transformation group
/// - Hiding and showing ghosts based upon their transformation type.
/// </summary>


namespace Transformations
{
    public partial class MainWindow
	{
		public void MyPolygonMouseDown(object sender, MouseButtonEventArgs e) //If the mouse is down on a polygon
		{
			Dragging = true;
			SelectedShape = sender as Shape;			 //make it the selected shape
			HighlightDetials();							//Make the shape highlighted
			ClickV = e.GetPosition(SelectedShape);		//Get the position of the cursor relative to the shape
			if (Reflection_Execute.IsChecked == true)   //If reflection is enabled, then allow reflection 
			{
				foreach (Shapes c in MyShapes)
				{
					if (c.MyShape.Name.StartsWith("dupe_reflection"))
					{
						MyCanvas.Children.Remove(c.MyShape);
					}
				}
				MyCanvas.Children.Remove(ReflLine);
				ReflectionExecute(sender, e);
			}
		}
        private void HighlightDetials() //Highlights the currently selected shape for the user.
		{
			try
			{
                unselectShape.IsEnabled = true;
				deleteShape.IsEnabled = true;
				selectedShapeLabel.Content = SelectedShape.Name.ToString();
		
				string Accuracy = GridSnap.IsChecked == true ? "0" : "0.00";
				//Updates the UI for the details of the updated shape
				if (SelectedShape.Name.StartsWith("Trapezium"))
				{
					selected_shape_dim.Content = "Width: " + (Properties.Settings.Default.DefaultHeight / ScaleFactor).ToString() + "     Height: " +
					                             (Properties.Settings.Default.DefaultHeight / ScaleFactor).ToString();
					selected_shape_cord.Content =
						"( " + (-(- Canvas.GetLeft(SelectedShape)) / ScaleFactor).ToString(Accuracy) + " , "
						+
						(((-Canvas.GetTop(SelectedShape)) / ScaleFactor) - (Round.ToNearest(Properties.Settings.Default.DefaultHeight / 2, 15)) / (ScaleFactor))
						.ToString(Accuracy) + " )";
				}
				else if (SelectedShape.Name.StartsWith("FreeForm"))
				{
					selected_shape_dim.Content = "UNKNOWN CUSTOM SHAPE";
					selected_shape_cord.Content = "X: " + ((Canvas.GetLeft(SelectedShape) + (SelectedShape as Polygon).Points[0].X) / ScaleFactor).ToString(Accuracy) + "  Y: " + (-(Canvas.GetTop(SelectedShape) + +(SelectedShape as Polygon).Points[0].Y) / ScaleFactor).ToString(Accuracy);
				}
				else
				{
					selected_shape_dim.Content = "Width: " + (Properties.Settings.Default.DefaultHeight / ScaleFactor).ToString() + "     Height: " +
					                             (Properties.Settings.Default.DefaultHeight / ScaleFactor).ToString();
					selected_shape_cord.Content =
						"( " + ( Canvas.GetLeft(SelectedShape) / ScaleFactor).ToString(Accuracy) + " , "
						+
						(((-Canvas.GetTop(SelectedShape)) / ScaleFactor) - Properties.Settings.Default.DefaultHeight / (ScaleFactor))
						.ToString(Accuracy) + " )";
				}

				SelectedShape.StrokeThickness = 3;
				SelectedShape.Stroke = Brushes.Black;


				SelectedShape.Fill = new SolidColorBrush(Color.FromArgb((ControlPaint.Light(Properties.Settings.Default.DefaultColour, 1).A),
					(ControlPaint.Light(Properties.Settings.Default.DefaultColour, 1)).R, (ControlPaint.Light(Properties.Settings.Default.DefaultColour, 1)).G,
					(ControlPaint.Light(Properties.Settings.Default.DefaultColour, 1)).B));
				if (Properties.Settings.Default.DefaultPerformance == true)    //if high performance add extra effects to the shape
				{
					DropShadowEffect myDropShadowEffect = new DropShadowEffect();
					Color myShadowColor = new Color {ScA = 1};
					myDropShadowEffect.Color = myShadowColor;
					myDropShadowEffect.BlurRadius = 25;
					myDropShadowEffect.Opacity = 1;
					SelectedShape.Effect = myDropShadowEffect;
				}

				foreach (Shapes t in MyShapes)  //If the shape is not selected then make it a non-selected shape.
				{
					if (!t.MyShape.Name.StartsWith("dupe"))
					{
						if (t.MyShape != SelectedShape)
						{
							t.MyShape.Fill = new SolidColorBrush(Color.FromArgb(Properties.Settings.Default.DefaultColour.A, Properties.Settings.Default.DefaultColour.R, Properties.Settings.Default.DefaultColour.G,
								Properties.Settings.Default.DefaultColour.B));
							t.MyShape.Stroke = Brushes.Black;
							t.MyShape.StrokeThickness = 2;
							t.MyShape.Effect = null;
						}
                    }
				}
			}
			catch (Exception)
			{
				
			}
		}
        private void DeleteShapeClick(object sender, RoutedEventArgs e) //Allows the user to delete a shape upon pressing the button
		{
			MessageBoxResult messageBoxResult = MessageBox.Show(
				"Are you sure you want to delete " + SelectedShape.Name.ToString() + " ?", "Delete Confirmation",
				System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (messageBoxResult == MessageBoxResult.Yes)
			{
				for (int x = 0; x < MyShapes.Count; x++)
				{
					if (MyShapes[x].MyShape == SelectedShape)
					{
						MyCanvas.Children.Remove(MyShapes[x].MyShape);
						MyShapes.Remove(MyShapes[x]);
					}
				}

				SelectedShape = null;

				unselectShape.IsEnabled = false;
				deleteShape.IsEnabled = false;
				selectedShapeLabel.Content = "No Shape Currently Selected";
				selected_shape_dim.Content = "Width: 0     Height: 0";
				selected_shape_cord.Content = "( 0.00 , 0.00 )";
			}
		}
        private void UnselectShapeClick(object sender, RoutedEventArgs e) //Allows the user to unselect a shape upon pressing the button
		{
			SelectedShape = null;

			unselectShape.IsEnabled = false;
			deleteShape.IsEnabled = false;
			selectedShapeLabel.Content = "No Shape Currently Selected";
			selected_shape_dim.Content = "Width: 0     Height: 0";
			selected_shape_cord.Content = "( 0.00 , 0.00 )";

			foreach (Shapes polygon in MyShapes) //Need to run through the circle and also the my polygon free form.
			{
				if (!polygon.MyShape.Name.StartsWith("dupe"))
				{
					if (polygon.MyShape != SelectedShape)
					{
						polygon.MyShape.Fill = new SolidColorBrush(Color.FromArgb(Properties.Settings.Default.DefaultColour.A, Properties.Settings.Default.DefaultColour.R,
							Properties.Settings.Default.DefaultColour.G, Properties.Settings.Default.DefaultColour.B));
						polygon.MyShape.Stroke = Brushes.Black;
						polygon.MyShape.StrokeThickness = 2;
						polygon.MyShape.Effect = null;
					}
				}
			}
		}
		private void DeleteAllGhosts(object sender, RoutedEventArgs e) //Allows the user to delete all ghost shapes upon pressing the button
		{
			foreach (Shapes t in MyShapes)
			{
				if (t.MyShape.Name.StartsWith("dupe"))
				{
					MyCanvas.Children.Remove(t.MyShape);
				}
			}
            DeleteRays(new object(), new RoutedEventArgs());
            MyShapes.RemoveAll(item => item.MyShape.Name.StartsWith("dupe"));
			Reflection_Execute.IsChecked = false;
		}
        private void DeleteGhosts(string transformation) //Allows the user to delete ghosts shapes based upon their transformation type.
		{
			foreach (Shapes t in MyShapes)
			{
				if (t.MyShape.Name == "dupe_" + transformation)
				{
					MyCanvas.Children.Remove(t.MyShape);
				}
			}
            MyShapes.RemoveAll(item => item.MyShape.Name.Contains("dupe_" + transformation));
			Reflection_Execute.IsChecked = false;
		}
        public void HideGhosts(string transformation)   //A generalised function, used to hide ghosts based upon their type of transformation.
		{
            foreach (Shapes t in MyShapes)
            {
                if (t.MyShape.Name == "dupe_" + transformation)
                {
                    t.MyShape.Visibility = Visibility.Hidden;
                }
            }
        }
        private void ShowGhosts(string transformation)      //A generalised function, used to show ghosts based upon their type of transformation.
		{
			foreach (Shapes t in MyShapes)
			{
				if (t.MyShape.Name == "dupe_" + transformation)
				{
					t.MyShape.Visibility = Visibility.Visible;
				}
			}
		}
	}
}