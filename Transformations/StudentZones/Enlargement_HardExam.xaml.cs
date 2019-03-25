using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Transformations
{
	/// <summary>
	/// Enlargement Hard exam- two shapes spawn, the original and the enlarged, the user must then specify the center of enlargement in the text-box, they can draw ray-lines to help aid them.
	/// </summary>
	public partial class Enlargement_HardExam : Window
	{
        readonly Exam Exams;
		List<Shapes> MyShapes = new List<Shapes>();

		public readonly double[] Values = {-2, -1, 0.5, 2};
		public List<int> XAnswer = new List<int>();
		public List<int> YAnswer = new List<int>();

		GridLine GridLine;
		public const int ScaleFactor = 40;

		List<RayLines> MyRayLines = new List<RayLines>();
		public bool IsDrawingRays = false;

		        
		public Enlargement_HardExam()
		{
			InitializeComponent();
            Exams = new Exam(0, -3, "Enlargement Hard Exam", 2, timer);
            border.MouseWheel += new MouseWheelEventHandler((sender, e) => Transformations.Scaling.MouesWheel(sender, e, sliderSf));
			border.MouseUp += new MouseButtonEventHandler(Transformations.Scaling.BorderMouseUp);
			border.MouseMove += new MouseEventHandler((sender, e) => Transformations.Scaling.BorderMouseMove(sender, e, xSlider, ySlider, MyCanvas, Cursor));
			border.MouseDown += new MouseButtonEventHandler((sender, e) => Transformations.Scaling.BorderMouseDown(sender, e, MyCanvas));
			if (Properties.Settings.Default.DarkMode)
				border.Background = new SolidColorBrush(Color.FromArgb(255, 31, 31, 31));
		}
        private void Randomise()
        {
            try
            {
                for (int z = 0; z < 6; z++)
                {
                    switch (Rnd.RandomNumber(1, 8))
                    {
                        case 1:
                            MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Rectangle(ScaleFactor * 5), MyCanvas)));
                            break;
                        case 2:
                            MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Triangle(ScaleFactor * 5), MyCanvas)));
                            break;
                        case 3:
                            MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Parallelogram(ScaleFactor * 5), MyCanvas)));
                            break;
                        case 4:
                            MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Pentogan(ScaleFactor * 5), MyCanvas)));
                            break;
                        case 5:
                            MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Arrow(ScaleFactor * 5), MyCanvas)));
                            break;
                        case 6:
                            MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.LShape(ScaleFactor * 5), MyCanvas)));
                            break;
                        case 7:
                            MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Star(ScaleFactor * 5), MyCanvas)));
                            break;
                     }

                    Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomY(border, ScaleFactor));
                    Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomY(border, ScaleFactor));

					MyShapes.Add((new FreeForm().WrongGhost(233, 255, 0, (Polygon)MyShapes[MyShapes.Count - 1].MyShape, MyCanvas)));
                    				

					if (Rnd.RandomNumber(0, 2) == 0)
                    {
                        MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterX = Rnd.RandomNumber(7, 12) * ScaleFactor;
                    }
                    else
                    {
                        MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterX = Rnd.RandomNumber(1, 6) * -ScaleFactor;
                    }
                    if (Rnd.RandomNumber(0, 2) == 0)
                    {
                        MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterY = Rnd.RandomNumber(7, 12) * ScaleFactor;
                    }
                    else
                    {
                        MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterY = Rnd.RandomNumber(1, 6) * -ScaleFactor;
                    }


                    int RandomScale = Rnd.RandomNumber(0, 4);
                    MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleX = Values[RandomScale];
                    MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleY = Values[RandomScale];

                    XAnswer.Add(Convert.ToInt32((Canvas.GetLeft(MyShapes[MyShapes.Count - 1].MyShape) + MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterX) / ScaleFactor));
                    YAnswer.Add(-Convert.ToInt32((Canvas.GetTop(MyShapes[MyShapes.Count - 1].MyShape) + MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterY) / ScaleFactor));

					MyShapes.Add((new Circle("dupe_enlargement").MakerSpawn((Canvas.GetLeft(MyShapes[MyShapes.Count - 1].MyShape) + MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterX), (Canvas.GetTop(MyShapes[MyShapes.Count - 1].MyShape) + MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterY), MyCanvas)));

				}
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Failed to randomly generate an 'Enlargement Hard' exam. " + LocalizationProvider.GetLocalizedValue<string>("CriticalFailuer"),
                    "Critical Program Failure: 400 C", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SubmitAnswer(object sender, RoutedEventArgs e)
		{
            if (XGuess.Text == XAnswer[Exams.QuestionPos - 1].ToString() && YGuess.Text == YAnswer[Exams.QuestionPos - 1].ToString())
            {
				Exams.AddPoint();
                Show(CorrectAnswer);
                NextQuestion();
            }
            else
            {
               try
                {
                    double XGuess = Convert.ToInt32(this.XGuess.Text) * ScaleFactor;
					double YGuess = -Convert.ToInt32(this.YGuess.Text) * ScaleFactor;

					Exams.AddAttempt();
					MyShapes.Add((new FreeForm().WrongGhost(0, 0, 0, (Polygon)MyShapes[Exams.ArrayPos].MyShape, MyCanvas)));

					MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterX = XGuess - Canvas.GetLeft(MyShapes[Exams.ArrayPos].MyShape);
                    MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterY = YGuess - Canvas.GetTop(MyShapes[Exams.ArrayPos].MyShape);
					MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleX = MyShapes[Exams.ArrayPos + 1].MyScalingTransform.ScaleX;
                    MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleY = MyShapes[Exams.ArrayPos + 1].MyScalingTransform.ScaleY;
					MyShapes.Add((new Circle("dupe_enlargement").MakerSpawn(XGuess, YGuess, MyCanvas)));
					
					if (Exams.Attmepts > 2)
                    {
                        Show(OutofAttempts);
                        NextQuestion();
                    }
                    else
                        Show(WrongAnswer);

                }
                catch (Exception)
                {
                    MessageBox.Show("The coordinates entered were not in the correct format; only numerical values are allowed. " + LocalizationProvider.GetLocalizedValue<string>("UserError"),
                        "Invalid Input Type Error: 302 A", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
           RefreshText();
        }
        private void NextQuestion()
		{
			Exams.AddQuesPos();
			Exams.ArrayPos += 3;
			Exams.ResetAttempts();
            if (Exams.QuestionPos > 6)
            {
                Exams.Timer.Stop();
                BlurEffect myBlurEffect = new BlurEffect { Radius = 10 };
                window.Effect = myBlurEffect;
                this.Topmost = false;

                ExamResults exam = new ExamResults(Exams);
                exam.Show();
                this.Owner = exam;
                this.IsEnabled = false;
            }
            else
            {
                foreach (var raygroup in MyRayLines)
                {
                    foreach (var line in raygroup.RayLinesList)
                    {
                        MyCanvas.Children.Remove(line);
                    }
                }

                foreach (var shape in MyShapes)
                {
                    shape.MyShape.Visibility = Visibility.Hidden;
                }

                MyShapes[Exams.ArrayPos].MyShape.Visibility = Visibility.Visible;        //Original Shape
                MyShapes[Exams.ArrayPos + 1].MyShape.Visibility = Visibility.Visible;    //Ghost Shape
                if (hint.IsChecked == true)
                {
                    MyShapes[Exams.ArrayPos + 2].MyShape.Visibility = Visibility.Visible;    //Marker Point
                }
                RefreshText();

                xSlider.Value = 0;
                ySlider.Value = 0;
                sliderSf.Value = 0;
				Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);

                XGuess.Text = "";
                YGuess.Text = "";
            }
    	}
        private void RefreshText()
		{
			question_no.Content = "Question: " + Exams.QuestionPos.ToString() + "/6";
			score.Content = "Score: " + Exams.ScoreValue.ToString() + "/6";
			attempts.Content = "Attempts: " + Exams.Attmepts.ToString() + "/2";
			question.Content = "What is the coordinate for center of enlargement?";
			ShapeRotAmount.Content = "Hint: This shape has enlarged by: " + MyShapes[Exams.ArrayPos + 1].MyScalingTransform.ScaleX + " ScaleFactor";
		}
        private void CanvasLoaded(object sender, RoutedEventArgs e)
		{
			GridLine = new GridLine().DrawGrid(3500, ScaleFactor, MyCanvas);
			Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);
			Randomise();
			NextQuestion();
			foreach (Label t in GridLine.Labels)
			{
				MyCanvas.Children.Add(t);
			}
		}

        private void Exit(object sender, RoutedEventArgs e)
		{
			MessageBoxResult exit = MessageBox.Show("Are you sure you wish to abandon this exam?", "Are you sure?",
				System.Windows.MessageBoxButton.OKCancel,
				MessageBoxImage.Warning);

			if (exit == MessageBoxResult.OK)
			{
				TakeExam exam = new TakeExam();
				exam.Show();
				this.Close();
			}
		}
        private void Scaling(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);
		}
        private void HintsOff(object sender, RoutedEventArgs e)
		{
			try
			{
				MyShapes[Exams.ArrayPos + 2].MyShape.Visibility = Visibility.Hidden; //Marker Point
			}
			catch (Exception)
			{
			}
		}
        private void HintsOn(object sender, RoutedEventArgs e)
		{
			try
			{
				MyShapes[Exams.ArrayPos + 2].MyShape.Visibility = Visibility.Visible; //Marker Point
			}
			catch (Exception)
			{
			}

		}
        private void KeyPressed(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SubmitAnswer(sender, e);
			}
		}
        private void DrawRays(object sender, RoutedEventArgs e)
		{
			IsDrawingRays = true;
			this.Cursor = Cursors.Cross;

			MyRayLines.Add(new RayLines());
			MyRayLines[MyRayLines.Count - 1].RayLinesList.Add(new Line() {
				Stroke = Brushes.DarkOrange,
				StrokeThickness = 1
			});
			Panel.SetZIndex(MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1], 2);
			MyCanvas.Children.Add(MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1]);
		}
        private void CanvasClick(object sender, MouseButtonEventArgs e)
		{
			if (IsDrawingRays)    //If User is drawing ray lines.
			{
				if ((((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1) < ((MyShapes[Exams.ArrayPos].MyShape as Polygon).Points.Count) - 1)
				{
					MyRayLines[MyRayLines.Count - 1].RayLinesList.Add(new Line()
					{
						Stroke = Brushes.DarkOrange,
						StrokeThickness = 1
					});
					Panel.SetZIndex(MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1], 2);
				
					MyCanvas.Children.Add(MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1]);
				}
				else
				{
					this.Cursor = Cursors.Arrow;
					IsDrawingRays = false;
				}
				CanvasMove(sender, e);
			}
		}
        private void CanvasMove(object sender, MouseEventArgs e)
		{
			if (IsDrawingRays && MyRayLines[MyRayLines.Count - 1].RayLinesList.Count >= 1)
			{
				LineCaculator((Convert.ToDouble(((MyShapes[Exams.ArrayPos].MyShape as Polygon).Points[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].X) + Convert.ToDouble(Canvas.GetLeft(MyShapes[Exams.ArrayPos].MyShape)))), (Convert.ToDouble(((MyShapes[Exams.ArrayPos].MyShape as Polygon).Points[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].Y) + Convert.ToDouble(Canvas.GetTop(MyShapes[Exams.ArrayPos].MyShape)))));
			}
			cords.Content = "( " + (Convert.ToDouble(Mouse.GetPosition(MyCanvas).X) / ScaleFactor).ToString("0.0") + "  ,  " + (-Convert.ToDouble(Mouse.GetPosition(MyCanvas).Y) / ScaleFactor).ToString("0.0") + " )";
		}
        private void LineCaculator(double X, double Y)
		{
			try
			{
				double m = (((Convert.ToDouble(Y) - Convert.ToDouble(Mouse.GetPosition(MyCanvas).Y)) / (Convert.ToDouble(X) - Convert.ToDouble(Mouse.GetPosition(MyCanvas).X))));
				double c = -(Convert.ToDouble(Y) - (m * Convert.ToDouble(X)));

				//Left
				MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].Y1 = (-(c) + ((3500) * (m)));
				MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].X2 = -3500;

				//Right
				MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].X1 = 3500;
				MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].Y2 = (-(c) - ((3500) * (m)));

			}
			catch (Exception)
			{

			}
		}
        private void DeleteRays(object sender, RoutedEventArgs e)
		{
            MyRayLines.ForEach(p => p.RayLinesList.ForEach(o => MyCanvas.Children.Remove(o)));
            MyRayLines.Clear();
        }
        private void Show(Border type)
        {
            type.Visibility = System.Windows.Visibility.Visible;

            var a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(1),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, type);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { type.Visibility = System.Windows.Visibility.Hidden; };
            storyboard.Begin();
        }
    }
}
