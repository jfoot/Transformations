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
            return new PointCollection
            {
                new Point(0, default_height),
                new Point(default_height, default_height),
                new Point(default_height, 0),
                new Point(0, 0)
            };
		}

		public static PointCollection Triangle(double default_height)   //Triangle points
		{
            return new PointCollection
            {
                new Point(0, default_height),
                new Point(default_height / 2, 0),
                new Point(default_height, default_height)
            };
		}

		public static PointCollection Trapzium(double default_height)   //Trapezium points
		{
            return new PointCollection
            {
                new Point(0, Round.ToNearest(default_height / 2, 15)),
                new Point(Round.ToNearest((default_height / 4), 15), 0),
                new Point(Round.ToNearest(((default_height / 4) * 3), 15), 0),
                new Point(default_height, Round.ToNearest(default_height / 2, 15))
            };
		}

		public static PointCollection Pentogan(double default_height)   //Pentagon points
		{
            return new PointCollection
            {
                new Point(0, default_height),
                new Point(0, default_height / 2),
                new Point(default_height / 2, 0),
                new Point(default_height, default_height / 2),
                new Point(default_height, default_height)
            };
		}

		public static PointCollection Arrow(double default_height)  //Arrow Points
		{
            return new PointCollection
            {
                new Point(default_height / 4, default_height),
                new Point(default_height / 4, default_height / 8 * 3),
                new Point(0, default_height / 8 * 3),
                new Point(default_height / 2, 0),
                new Point(default_height, default_height / 8 * 3),
                new Point((default_height / 4) * 3, default_height / 8 * 3),
                new Point((default_height / 4) * 3, default_height)
            };
		}

		public static PointCollection Star(double default_height)   //Star points
		{
            return new PointCollection
            {
                new Point((default_height / 2), (0)),
                new Point((default_height / 8) * 5, (default_height/3)),
                new Point((default_height), (default_height/3)),
                new Point((default_height/8) * 6, (default_height/2)), //4
                new Point((default_height/8)*7, (default_height/10)*9),
                new Point((default_height/2), (default_height/3)*2),
                new Point(default_height/8, (default_height/10)*9),
                new Point((default_height / 8) * 2, (default_height/2)), //8
                new Point(0, (default_height/3)),
                new Point((default_height/8)*3, (default_height/3))
            };
		}

		public static PointCollection LShape(double default_height) //L shape points
		{
            return new PointCollection
            {
                new Point(0, 0),
                new Point(default_height / 4, 0),
                new Point((default_height / 4), (default_height / 4) * 3),
                new Point((default_height), (default_height / 4) * 3),
                new Point((default_height), (default_height)),
                new Point(0, default_height)
            };
		}

		public static PointCollection Parallelogram(double default_height)  //Parallelogram points
		{
            return new PointCollection
            {
                new Point(default_height / 4, 0),
                new Point(default_height, 0),
                new Point((default_height / 4) * 3, default_height),
                new Point(0, default_height)
            };
		}
	}
}
