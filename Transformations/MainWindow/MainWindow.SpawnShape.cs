using Microsoft.AppCenter.Analytics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Transformations
{
    public partial class MainWindow
	{
		/// <summary>
		///                     Spawning
		/// Here the program can spawn in shapes.
		/// - Spawn pre-defined shapes.
		/// - Spawn Polygons (free-form shapes)
		///  </summary>

		//Pre-defined shapes
		private void SpawnCircleClick(object sender, RoutedEventArgs e)		//Spawn Circle
		{
            Analytics.TrackEvent("Spawn Circle");
            Counter.myEllipse++;
			MyShapes.Add((new Circle((Properties.Strings.CircleString + "_" + Counter.myEllipse.ToString())).SpawnCircle(MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}

		private void SpawnRectangleClick(object sender, RoutedEventArgs e)   //Spawn rectangle
        {
            Analytics.TrackEvent("Spawn Rectangle");
            Counter.myRect++;
			MyShapes.Add((new FreeForm((Properties.Strings.SquareString + "_" + (Counter.myRect).ToString())).SpawnCustomShape(ShapePoints.Rectangle(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
		private void SpawnTriangleClick(object sender, RoutedEventArgs e)	//Spawn triangle
		{
            Analytics.TrackEvent("Spawn Triangle");
            Counter.myTriangle++;
			MyShapes.Add((new FreeForm((Properties.Strings.TriangleString + "_" + (Counter.myTriangle).ToString())).SpawnCustomShape(ShapePoints.Triangle(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawmTrapeziumClick(object sender, RoutedEventArgs e) //Spawn trapezium 
		{
            Analytics.TrackEvent("Spawn Trapezium");
            Counter.myTrapzium++;
			MyShapes.Add((new FreeForm((Properties.Strings.TrapeziumString + "_" + (Counter.myTrapzium).ToString())).SpawnCustomShape(ShapePoints.Trapzium(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, -(Round.ToNearest(Properties.Settings.Default.DefaultHeight / 2, 15)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawnPentoganClick(object sender, RoutedEventArgs e) //Spawn pentagon
		{
            Analytics.TrackEvent("Spawn Pentogan");
            Counter.myPentagon++;
			MyShapes.Add((new FreeForm((Properties.Strings.PentagonString + "_" + (Counter.myPentagon).ToString())).SpawnCustomShape(ShapePoints.Pentogan(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawnArrowClick(object sender, RoutedEventArgs e)   //Spawn arrow
		{
            Analytics.TrackEvent("Spawn Arrow");
            Counter.myArrow++;
			MyShapes.Add((new FreeForm((Properties.Strings.ArrowString + "_" + (Counter.myArrow).ToString())).SpawnCustomShape(ShapePoints.Arrow(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawnStarClick(object sender, RoutedEventArgs e)	//Spawn Star
		{
            Analytics.TrackEvent("Spawn Star");
            Counter.myStar++;
			MyShapes.Add((new FreeForm((Properties.Strings.StarString + "_" + (Counter.myStar).ToString())).SpawnCustomShape(ShapePoints.Star(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawnLShapeClick(object sender, RoutedEventArgs e)  //Spawn L shape
		{
            Analytics.TrackEvent("Spawn L Shape");
            Counter.myLshape++;
			MyShapes.Add((new FreeForm((Properties.Strings.LShapeString + "_" + (Counter.myLshape).ToString())).SpawnCustomShape(ShapePoints.LShape(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawnParaClick(object sender, RoutedEventArgs e)	//Spawn parallelogram
		{
            Analytics.TrackEvent("Spawn Parallelogram");
            Counter.myPara++;
			MyShapes.Add((new FreeForm((Properties.Strings.ParallelogramString + "_" + (Counter.myPara).ToString())).SpawnCustomShape(ShapePoints.Parallelogram(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawnFreeFormClick(object sender, RoutedEventArgs e) //User Clicks Free-Form Button
		{
            Analytics.TrackEvent("Spawn Free Form");
            IsDrawing = true;
			MyLines.Add(new Lines());
			this.Cursor = Cursors.Cross;
		}
	}
}