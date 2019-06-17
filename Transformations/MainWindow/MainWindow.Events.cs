using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

//This Class contains all the events for the main window of the program
//An event is triggered when a user preforms an action, such as moving a mouse or clicking on an object.

namespace Transformations
{
	public partial class MainWindow
	{
		//Mouse Wheel function - Used to increase or decrease the zoom
		private void MouesWheel(object sender, MouseWheelEventArgs e)
		{
			sliderSf.Value += (e.Delta / 120);
		}
        //Mouse Up Function - Triggered when the mouse is released
		private void MouseUp(object sender, MouseButtonEventArgs e)
		{
			GridSnapState(sender, e);
			Ctrldragging = false;
			Dragging = false;
		}
		//Mouse Down - Triggered when the mouse is clicked on the canvas
		private void MouseDown(object sender, MouseButtonEventArgs e) 
		{
			ClickX = e.GetPosition(MyCanvas);     //Gets the mouse position relativite to the canvas
			Ctrldragging = true;				
			
			if (IsDrawing)	//If user is currently drawing. Free Form Shape
			{
				//Create a new Line where the user first clicked
				MyLines[MyLines.Count - 1].LinesList.Add(new Line() {
					Stroke = Brushes.Black,
					StrokeThickness = 2,
					X1 = Convert.ToDouble(Mouse.GetPosition(MyCanvas).X),
					Y1 = Convert.ToDouble(Mouse.GetPosition(MyCanvas).Y)
				});   
				//Add this line to the canvas
				MyCanvas.Children.Add(MyLines[MyLines.Count - 1].LinesList[((MyLines[MyLines.Count - 1].LinesList).Count) - 1]);
				IsDrawing = true;
			}
			if (IsDrawingRays)    //If User is drawing ray lines.
			{
				//Keep creating new lines till there is the same number of lines as there is points.
				if ((((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1) < ((SelectedShape as Polygon).Points.Count) - 1)  
				{
					MyRayLines[MyRayLines.Count - 1].RayLinesList.Add(new Line() {
						Stroke = Brushes.DarkOrange,
						StrokeThickness = 1
					});
					Panel.SetZIndex(MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1], 2);
					MyCanvas.Children.Add(MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1]);
				}
				else
				{   //Reset the mouse cursor back to an arrow 
					this.Cursor = Cursors.Arrow;
					IsDrawingRays = false;
				}
			}
		
			if (CtrlDown)
			{
				this.Cursor = GrabbingCursor;
			}
			MouseMove(sender, e);
		}
        //Mouse Moving - Triggered when the user moves their mouse
		private void MouseMove(object sender, MouseEventArgs e)
		{
			if (IsDrawing && MyLines[MyLines.Count - 1].LinesList.Count >= 1) //If user is drawing a free-form shape they have at least one point
			{   //Line should follow the position of the mouse cursor
				MyLines[MyLines.Count - 1].LinesList[((MyLines[MyLines.Count - 1].LinesList).Count) - 1].X2 = Convert.ToDouble(Mouse.GetPosition(MyCanvas).X);
				MyLines[MyLines.Count - 1].LinesList[((MyLines[MyLines.Count - 1].LinesList).Count) - 1].Y2 = Convert.ToDouble(Mouse.GetPosition(MyCanvas).Y);
			}
			if (IsDrawingRays && MyRayLines[MyRayLines.Count - 1].RayLinesList.Count >= 1)
			{   //If user is drawing a ray-line and there is at least one ray-line already
				LineCaculator((Convert.ToDouble((SelectedShape as Polygon).Points[(((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1)].X)  + Convert.ToDouble(Canvas.GetLeft(SelectedShape))), (Convert.ToDouble((SelectedShape as Polygon).Points[(((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1)].Y) + Convert.ToDouble(Canvas.GetTop(SelectedShape))));
			}
			if (Reflection_Execute.IsChecked.GetValueOrDefault() == true)
			{
				ReflectionPolygon();
			}
			if (Dragging)   //If user is dragging/ moving a shape
			{
				if (GridSnap.IsChecked == true)    //If snap to grid is turned on
				{   //Move the shapes only by scale factor values
					double newleft = (Round.ToNearest((e.GetPosition(MyCanvas).X), (ScaleFactor)));
					Canvas.SetLeft(SelectedShape, (newleft) - (ClickV.X));

					double newtop = (Round.ToNearest((e.GetPosition(MyCanvas).Y), (ScaleFactor)));
					Canvas.SetTop(SelectedShape, (newtop) - (ClickV.Y));
				}
				else
				{   //Else allow the shape to be moved around freely
					Canvas.SetLeft(SelectedShape, ((e.GetPosition(MyCanvas).X) - (ClickV.X)));
					Canvas.SetTop(SelectedShape, ((e.GetPosition(MyCanvas).Y) - (ClickV.Y)));
				}
				HighlightDetials();
			}
			if (CtrlDown && Ctrldragging)
			{
				XSlider.Value += ((e.GetPosition(MyCanvas).X) - (ClickX.X));
				YSlider.Value -= ((e.GetPosition(MyCanvas).Y) - (ClickX.Y));
				this.Cursor = GrabbingCursor;
			}
		}
		//Key Down - triggered when a user presses a key on their keyboard
		private void KeyDownMethod(object sender, KeyEventArgs e) 
		{
			if (e.Key == Key.Escape && IsDrawing && MyLines[MyLines.Count - 1].LinesList.Count > 1)    //If escape key is pressed and the user has at least one line
			{
				this.Cursor = Cursors.Arrow;
				//Create a new line and connect it up with the first drawn point
				MyLines[MyLines.Count - 1].LinesList.Add(new Line() {
					X1 = MyLines[MyLines.Count - 1].LinesList[((MyLines[MyLines.Count - 1].LinesList).Count) - 1].X1,
					Y1 = MyLines[MyLines.Count - 1].LinesList[((MyLines[MyLines.Count - 1].LinesList).Count) - 1].Y1,
					X2 = MyLines[MyLines.Count - 1].LinesList[0].X1,
					Y2 = MyLines[MyLines.Count - 1].LinesList[0].Y1
				});  		
				MyCanvas.Children.Add(MyLines[MyLines.Count - 1].LinesList[((MyLines[MyLines.Count - 1].LinesList).Count) - 1]);
				IsDrawing = false;

                //Extract all the user points from the shape
				foreach (Line t in MyLines[MyLines.Count - 1].LinesList)
				{
					MyLines[MyLines.Count - 1].MyPoints.Add(new Point(t.X1, t.Y1));
				}
				//Create a new free form shape out of the points created
				Counter.myPolygon++;
				MyShapes.Add((new FreeForm((Properties.Strings.FreeFormString + "_" + (Counter.myPolygon).ToString())).SpawnCustomShape(MyLines[MyLines.Count - 1].MyPoints, MyCanvas)));
				MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
				Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, 0); 
				Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, 0);

                MyLines[MyLines.Count - 1].LinesList.ForEach(o => MyCanvas.Children.Remove(o));
			}
			else if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)   //If ctrl is being held down
			{
				CtrlDown = true;
				if (Ctrldragging == false)
				{
					this.Cursor = GrabCursor;   //Change mouse cursor
				}
			}
			else
			{
				this.Cursor = Cursors.Arrow;
			}
			
			if (e.Key == Key.F1) // If F1 is pressed open settings panel
			{
				Settings ownedWindow = new Settings {Owner = this};
				ownedWindow.Show();
			}
			if (e.Key == Key.Delete && SelectedShape != null)   //if delete is pressed delete the shape
			{
				DeleteShapeClick(sender, e);
			}
		}
        //Key Up - triggered when a key on the keyboard is released
		private void KeyUpMethod(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
			{
				CtrlDown = false;
				this.Cursor = Cursors.Arrow;
			}
		}
	}
}