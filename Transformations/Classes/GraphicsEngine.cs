using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Cursor = System.Windows.Input.Cursor;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Transformations
{
	class Rnd   //A shared class for a variety of randomisation functions.
	{
		private static readonly Random random = new Random();
		private static readonly object syncLock = new object();
		public static int RandomNumber(int min, int max)    //Generates a random number between a range, it will create a different random number each time
		{
			lock (syncLock)
			{ // synchronize
				return random.Next(min, max);
			}
		}


		public static double RandomX(Border border, int scaleFactor)   //Creates a random X position for a shape inside of the canvas.
		{
			double x = 0;

			int shape_X = Rnd.RandomNumber(0, Convert.ToInt32(border.ActualWidth - scaleFactor * 5));
			double newleft = (Round.ToNearest((shape_X), (scaleFactor)));
			x = (newleft - Round.ToNearest(((border.ActualWidth / 2)), (scaleFactor)));

			return x;
		}

		public static double RandomY(Border border, int scaleFactor) //Creates a random Y position for a shape inside of the canvas.
		{
			double y = 0;

			int shape_y = Rnd.RandomNumber(0, Convert.ToInt32(border.ActualHeight - scaleFactor * 5));
			double newtop = (Round.ToNearest((shape_y), (scaleFactor)));
			y = (newtop - Round.ToNearest(((border.ActualHeight / 2)), (scaleFactor)));

			return y;
		}
	}

	class Round     //Declares a class for rounding
	{
		public static double ToNearest(double n, double x)  //Allows to numbers to be rounded to their nearest value.
		{
			return Math.Round(n / x) * x;
		}
	}

	class Counter   //Creates a counter, counting integers for every type of shape for their naming.
	{
		public int myRect, myTriangle, myTrapzium, myPentagon, myArrow, myStar, myPara, myLshape, myEllipse, myPolygon;
	}

	class Scaling
	{
		public static bool CtrlDown = false;        //If the ctrl button is being pressed down
		public static bool Ctrldragging = false;    //If user is dragging while holding down ctrl
		public static Point ClickX;     //A point on the canvas.
										//The main scaling function for the program, it accepts the variables from different windows and then can be used in multiple situations.
		public static void Main(TranslateTransform TranslationTransformCanvas, ScaleTransform scaleTransformCanvas, Slider x_slider, Slider y_slider, Slider slider_sf, Border border) //Generic Scaling Function- used to trigger the individual functions in order.
		{
			try
			{   //Movement of the canvas up or down and left or right
				TranslationTransformCanvas.X = x_slider.Value + border.ActualWidth / 2;
				TranslationTransformCanvas.Y = -y_slider.Value + border.ActualHeight / 2;
				//Zooming the canvas in and out 
				scaleTransformCanvas.ScaleX = slider_sf.Value;
				scaleTransformCanvas.ScaleY = slider_sf.Value;
				scaleTransformCanvas.CenterX = border.ActualWidth / 2 - TranslationTransformCanvas.X;
				scaleTransformCanvas.CenterY = border.ActualHeight / 2 - TranslationTransformCanvas.Y;
			}
			catch (Exception)
			{
			}
		}

		public static void MouesWheel(object sender, MouseWheelEventArgs e, Slider slider_sf)   //Mouse wheel being strolled
		{
			slider_sf.Value += (e.Delta / 120); //Change slider value (zoom) with the mouse wheel
		}

		public static void BorderMouseUp(object sender, MouseButtonEventArgs e)    //Mouse being pressed on the canvas
		{
			Ctrldragging = false;   //If not dragging make variable false
		}

		public static void BorderMouseMove(object sender, MouseEventArgs e, Slider x_slider, Slider y_slider, Canvas canvas, Cursor CursorWin) //Mouse moving on the canvas
		{
			if (Ctrldragging)
			{
				x_slider.Value += ((e.GetPosition(canvas).X) - (ClickX.X)); //Get position on the canvas and minus it from the original position to allow canvas panning
				y_slider.Value -= ((e.GetPosition(canvas).Y) - (ClickX.Y));
				Mouse.SetCursor(new Cursor(new System.IO.MemoryStream(Transformations.Properties.Resources.grabbing)));    //Change mouse cursor 
			}
		}

		public static void BorderMouseDown(object sender, MouseButtonEventArgs e, Canvas canvas) // mouse being pressed on the canvas
		{
			Ctrldragging = true;
			ClickX = e.GetPosition(canvas);
		}
	}
}
