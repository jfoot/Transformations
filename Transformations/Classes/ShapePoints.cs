using System.Windows;
using System.Windows.Media;
/// <summary>
/// Defines all the points for all of the shapes.
/// Points are given as a ratio to the value of default height.
/// </summary>

namespace Transformations
{
	class ShapePoints
	{
	  public static PointCollection Rectangle(double default_height)    //Rectangle points
		{
            PointCollection myPointCollection = new PointCollection
            {
                new Point(0, default_height),
                new Point(default_height, default_height),
                new Point(default_height, 0),
                new Point(0, 0)
            };

            return myPointCollection;
		}

		public static PointCollection Triangle(double default_height)   //Triangle points
		{
            PointCollection myPointCollection = new PointCollection
            {
                new Point(0, default_height),
                new Point(default_height / 2, 0),
                new Point(default_height, default_height)
            };

            return myPointCollection;
		}

		public static PointCollection Trapzium(double default_height)   //Trapezium points
		{
            PointCollection myPointCollection = new PointCollection
            {
                new Point(0, Round.ToNearest(default_height / 2, 15)),
                new Point(Round.ToNearest((default_height / 4), 15), 0),
                new Point(Round.ToNearest(((default_height / 4) * 3), 15), 0),
                new Point(default_height, Round.ToNearest(default_height / 2, 15))
            };
            return myPointCollection;
		}

		public static PointCollection Pentogan(double default_height)   //Pentagon points
		{
            PointCollection myPointCollection = new PointCollection
            {
                new Point(0, default_height),
                new Point(0, default_height / 2),
                new Point(default_height / 2, 0),
                new Point(default_height, default_height / 2),
                new Point(default_height, default_height)
            };

            return myPointCollection;
		}

		public static PointCollection Arrow(double default_height)  //Arrow Points
		{
            PointCollection myPointCollection = new PointCollection
            {
                new Point(default_height / 4, default_height),
                new Point(default_height / 4, default_height / 8 * 3),
                new Point(0, default_height / 8 * 3),
                new Point(default_height / 2, 0),
                new Point(default_height, default_height / 8 * 3),
                new Point((default_height / 4) * 3, default_height / 8 * 3),
                new Point((default_height / 4) * 3, default_height)
            };

            return myPointCollection;
		}

		public static PointCollection Star(double default_height)   //Star points
		{
            PointCollection myPointCollection = new PointCollection
            {
                new Point((default_height / 2), (0)), //1
                new Point((default_height / 3) * 2, (default_height / 3)), //2
                new Point((default_height), (default_height / 3)), //3
                new Point((default_height / 10) * 8, (default_height / 10) * 6), //4
                new Point((default_height), (default_height)), //5
                new Point((default_height / 2), (default_height / 3) * 2), //6
                new Point((0), (default_height)), //7
                new Point((default_height / 10) * 3, (default_height / 10) * 6), //8
                new Point(0, (default_height / 3)), //9
                new Point((default_height / 3), (default_height / 3)) //10
            };

            return myPointCollection;
		}

		public static PointCollection LShape(double default_height) //L shape points
		{
            PointCollection myPointCollection = new PointCollection
            {
                new Point(0, 0),
                new Point(default_height / 4, 0),
                new Point((default_height / 4), (default_height / 4) * 3),
                new Point((default_height), (default_height / 4) * 3),
                new Point((default_height), (default_height)),
                new Point(0, default_height)
            };

            return myPointCollection;
		}

		public static PointCollection Parallelogram(double default_height)  //Parallelogram points
		{
            PointCollection myPointCollection = new PointCollection
            {
                new Point(default_height / 4, 0),
                new Point(default_height, 0),
                new Point((default_height / 4) * 3, default_height),
                new Point(0, default_height)
            };

            return myPointCollection;
		}
	}
}
