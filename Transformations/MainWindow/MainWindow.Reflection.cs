using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Transformations
{
	public partial class MainWindow
	{
		/// Reflection of shapes
		///  - Allow shapes to be reflected.
		///  - Shapes can be reflected in a line y = MX + C
		///	 - Shapes can also be reflected in the Y or X axis.
		///  - Shapes can be update live in real time.
		//Code for reflection
		private void ReflectionExecute(object sender, RoutedEventArgs e)   //When the user starts a reflection
		{
            //Do not allow them to change the properties of a reflection while currently preforming one.
            refX.IsEnabled = false;
			refY.IsEnabled = false;
			refYMXC.IsEnabled = false;

			if (SelectedShape != null)  //If there is a shape selected
			{
				MyCanvas.Children.Add(ReflLine);
				if (ReflectionLine() == true)
				{
					if (refYMXC.IsChecked == true)    //If a Y = MX + C equation is being preformed 
					{
						if (SelectedShape.GetType().ToString() != "System.Windows.Shapes.Ellipse")  //If shape is not a circle
						{
							PointCollection myPointCollection = new PointCollection();  //Finds the points of the newly reflected shape
							foreach (var shapePoint in (SelectedShape as Polygon).Points)
							{
								double x = shapePoint.X;
								double y = -shapePoint.Y; 

								double cc = 0;
								double m = Convert.ToDouble(reflection_m.Text);

								double d1 = (1 + Math.Pow(m, 2));
								double d2 = (y - cc);
								double d3 = (d2 * m) + x;

								double dFinal = d3 / d1;

								double xNew = 2 * dFinal - x;
								double yNew = -((2 * dFinal * m) - y + (2 * cc));

								myPointCollection.Add(new Point(xNew, yNew));
							}
							//Spawns the newly reflected shape
							MyShapes.Add((new FreeForm("dupe_reflection").ReflectionGhost(myPointCollection, 255, 0, 0, MyCanvas, SelectedShape.Name)));				
						}
						else
						{
                            MessageBox.Show( Properties.Strings.NoCircleReflection  + Properties.Strings.UserError,
                                Properties.Strings.EM_InvalidInputTypeError + "302 J", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                            Reflection_Execute.IsChecked = false;
                        }
					}
					else if (refY.IsChecked == true) //Horizontal Line
					{
						MyShapes.Add((new Ghost("dupe_reflection").SpawnGhostShape(255, 0, 0, SelectedShape, MyCanvas, true)));
						MyShapes[MyShapes.Count - 1].PartnerShape = SelectedShape.Name;
						MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleY = -1;
					}
					else if (refX.IsChecked == true) //Vertical Line
					{
						MyShapes.Add((new Ghost("dupe_reflection").SpawnGhostShape(255, 0, 0, SelectedShape, MyCanvas, true)));
						MyShapes[MyShapes.Count - 1].PartnerShape = SelectedShape.Name;
						MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleX = -1;
					}

					ReflectionPolygon();
				}
			}
			else
			{
                MessageBox.Show(Properties.Strings.NoShapeSelected1 + Properties.Strings.UserError,
                    Properties.Strings.EM_FieldEmpty + "300 F", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                Reflection_Execute.IsChecked = false;
            }
		}
       	private void ReflectionPolygon() //While user is moving the mouse and reflection is activated 
		{
			try
			{
				foreach (Shapes c in MyShapes)  //Find the Child shape to the parent shape (the currently selected shape)
				{
					if (c.PartnerShape == SelectedShape.Name)
					{

						if (refYMXC.IsChecked == true)
						{   //Find the new position of the shape based upon the movement of the parent shape 
							double Sx = Canvas.GetLeft(SelectedShape);
							double Sy = -Canvas.GetTop(SelectedShape);

							double Scc = (Convert.ToDouble(reflection_c.Text) * ScaleFactor);
							double Sm = Convert.ToDouble(reflection_m.Text);

							double Sd1 = (1 + Math.Pow(Sm, 2));
							double Sd2 = (Sy - Scc);
							double Sd3 = (Sd2 * Sm) + Sx;

							double SdFinal = Sd3 / Sd1;
							
							double SxNew = 2 * SdFinal - Sx;
							double SyNew = -((2 * SdFinal * Sm) - Sy + (2 * Scc));

							Canvas.SetTop(c.MyShape, SyNew);
							Canvas.SetLeft(c.MyShape, SxNew);
						}
						else if (refY.IsChecked == true) //Horizontal Line
						{
							Canvas.SetTop(c.MyShape, ReflLine.Y1 - (Canvas.GetTop(SelectedShape) - ReflLine.Y1));
							Canvas.SetLeft(c.MyShape, Canvas.GetLeft(SelectedShape));
						}
						else if (refX.IsChecked == true) //Vertical Line
						{
							Canvas.SetLeft(c.MyShape, ReflLine.X1 - (Canvas.GetLeft(SelectedShape) - ReflLine.X1));
							Canvas.SetTop(c.MyShape, Canvas.GetTop(SelectedShape));
						}
					}
				}
			}
			catch (Exception)
			{
				Reflection_Execute.IsChecked = false;
				foreach (Shapes c in MyShapes)
				{
					if (c.MyShape.Name.StartsWith("dupe_reflection"))
					{
						MyCanvas.Children.Remove(c.MyShape);
					}
				}
			}
		}
       	private bool ReflectionLine()  //Draws the reflection line onto the grid
		{
			if (Reflection_Execute.IsChecked == true)
			{
				try
				{
					ReflLine.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
					ReflLine.StrokeThickness = 4;
					Panel.SetZIndex(ReflLine, 1);

					if (refYMXC.IsChecked == true) //Diagonal Line - Y = MX +C
					{
						//Left
						ReflLine.Y1 = ( -(Convert.ToDouble(reflection_c.Text) * (ScaleFactor)) +
						               ((MaxValue) * (Convert.ToDouble(reflection_m.Text))));
						ReflLine.X1 = -MaxValue;

						//Right
						ReflLine.X2 = MaxValue;
						ReflLine.Y2 = (-(Convert.ToDouble(reflection_c.Text) * (ScaleFactor)) -
						               ((MaxValue) * (Convert.ToDouble(reflection_m.Text))));
					}
					else if (refY.IsChecked == true) //Horizontal Line
					{
						ReflLine.X1 = -MaxValue;
						ReflLine.Y1 = -(Convert.ToDouble(reflection_y_axis.Text) * (ScaleFactor));
						ReflLine.X2 = MaxValue;
						ReflLine.Y2 = -(Convert.ToDouble(reflection_y_axis.Text) * (ScaleFactor));
					}
					else if (refX.IsChecked == true) //Vertical Line
					{
						ReflLine.X1 = (Convert.ToDouble(reflection_X_axis.Text) * (ScaleFactor));
						ReflLine.Y1 = -MaxValue;
						ReflLine.X2 = (Convert.ToDouble(reflection_X_axis.Text) * (ScaleFactor));
						ReflLine.Y2 = MaxValue;
					}
				
					return true;
				}
				catch (Exception)
				{
                    MessageBox.Show(
                        Properties.Strings.ReflectNotCorrectFormat + Properties.Strings.UserError,
                        Properties.Strings.EM_InvalidInputTypeError + "302 C", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
                    Reflection_Execute.IsChecked = false;
                    foreach (Shapes c in MyShapes)
					{
						if (c.MyShape.Name.StartsWith("dupe_reflection"))
						{
							MyCanvas.Children.Remove(c.MyShape);
						}
					}
					return false;
				}
			}
			return false;
		}
        private void ReflectionUnexecute(object sender, RoutedEventArgs e) //Reflection is stopped
		{
			try
			{   //Allow the user to start a new reflection and remove the old reflection from the canvas.
				refX.IsEnabled = true;
				refY.IsEnabled = true;
				refYMXC.IsEnabled = true;

				MyCanvas.Children.Remove(ReflLine);
				foreach (Shapes c in MyShapes)
				{
					if (c.PartnerShape != null)
					{
						MyCanvas.Children.Remove(c.MyShape);
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}
}