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
			Counter.myEllipse++;
			MyShapes.Add((new Circle(("Circle_" + Counter.myEllipse.ToString())).SpawnCircle(MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}

		private void SpawnRectangleClick(object sender, RoutedEventArgs e)   //Spawn rectangle
		{
			Counter.myRect++;
			MyShapes.Add((new FreeForm(("Rectangle_" + (Counter.myRect).ToString())).SpawnCustomShape(ShapePoints.Rectangle(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
		private void SpawnTriangleClick(object sender, RoutedEventArgs e)	//Spawn triangle
		{
			Counter.myTriangle++;
			MyShapes.Add((new FreeForm(("Triangle_" + (Counter.myTriangle).ToString())).SpawnCustomShape(ShapePoints.Triangle(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawmTrapeziumClick(object sender, RoutedEventArgs e) //Spawn trapezium 
		{
			Counter.myTrapzium++;
			MyShapes.Add((new FreeForm(("Trapaizum_" + (Counter.myTrapzium).ToString())).SpawnCustomShape(ShapePoints.Trapzium(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, -(Round.ToNearest(Properties.Settings.Default.DefaultHeight / 2, 15)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawnPentoganClick(object sender, RoutedEventArgs e) //Spawn pentagon
		{
			Counter.myPentagon++;
			MyShapes.Add((new FreeForm(("Pentogan_" + (Counter.myPentagon).ToString())).SpawnCustomShape(ShapePoints.Pentogan(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawnArrowClick(object sender, RoutedEventArgs e)   //Spawn arrow
		{
			Counter.myArrow++;
			MyShapes.Add((new FreeForm(("Arrow_" + (Counter.myArrow).ToString())).SpawnCustomShape(ShapePoints.Arrow(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawnStarClick(object sender, RoutedEventArgs e)	//Spawn Star
		{
			Counter.myStar++;
			MyShapes.Add((new FreeForm(("Star_" + (Counter.myStar).ToString())).SpawnCustomShape(ShapePoints.Star(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawnLShapeClick(object sender, RoutedEventArgs e)  //Spawn L shape
		{
			Counter.myLshape++;
			MyShapes.Add((new FreeForm(("LShape_" + (Counter.myLshape).ToString())).SpawnCustomShape(ShapePoints.LShape(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawnParaClick(object sender, RoutedEventArgs e)	//Spawn parallelogram
		{
			Counter.myPara++;
			MyShapes.Add((new FreeForm(("Parallelogram_" + (Counter.myPara).ToString())).SpawnCustomShape(ShapePoints.Parallelogram(Properties.Settings.Default.DefaultHeight), MyCanvas)));
			MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
		}
        private void SpawnFreeFormClick(object sender, RoutedEventArgs e) //User Clicks Free-Form Button
		{
			IsDrawing = true;
			MyLines.Add(new Lines());
			this.Cursor = Cursors.Cross;
		}
	}
}