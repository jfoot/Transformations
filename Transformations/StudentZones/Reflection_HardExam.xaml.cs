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
	/// Reflection hard exam, the user must specify the gradient and interception point of the reflection line.
	/// </summary>
	public partial class Reflection_HardExam : Window
	{
        readonly Exam Exams;
		List<Shapes> MyShapes = new List<Shapes>();

		List<Line> RefLine = new List<Line>();
		List<int> MList = new List<int>();
		List<int> CList = new List<int>();

		GridLine GridLine;
		public const int ScaleFactor = 25;
		
        
		public Reflection_HardExam()
		{
			InitializeComponent();
            Exams = new Exam(0, -2, "Reflection Hard Exam", 6, timer);
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
                    int shape_type = Rnd.RandomNumber(1, 7);
                    switch (shape_type)
                    {
                        case 1:
                            MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Star(ScaleFactor * 5), MyCanvas)));
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
                    }
                    int shape_X = Rnd.RandomNumber(0, Convert.ToInt32((border.ActualWidth / 2) - ScaleFactor * 5));
                    double newleft = (Round.ToNearest((shape_X), (ScaleFactor)));
                    Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, (newleft - Round.ToNearest(((border.ActualWidth / 4)), (ScaleFactor))));

                    int shape_y = Rnd.RandomNumber(0, Convert.ToInt32((border.ActualHeight / 2) - ScaleFactor * 5));
                    double newtop = (Round.ToNearest((shape_y), (ScaleFactor)));
                    Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, (newtop - Round.ToNearest(((border.ActualHeight / 4)), (ScaleFactor))));


                    if (Rnd.RandomNumber(0, 2) == 0)
                    {
                        MList.Add(Rnd.RandomNumber(-5, 0));
                    }
                    else
                    {
                        MList.Add(Rnd.RandomNumber(1, 6));
                    }

                    CList.Add(Rnd.RandomNumber(-10, 10));


                    RefLine.Add(new Line() {
						Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
						StrokeThickness = 2,
						Y1 = (-(CList[CList.Count - 1] * (ScaleFactor)) + ((3500) * (Convert.ToDouble(MList[MList.Count - 1])))),
						X1 = -3500,
						X2 = 3500,
						Y2 = (-(Convert.ToDouble(CList[CList.Count - 1]) * (ScaleFactor)) - ((3500) * (Convert.ToDouble(MList[MList.Count - 1]))))
					});

                    MyCanvas.Children.Add(RefLine[RefLine.Count - 1]);
					
                    PointCollection myPointCollection = new PointCollection();
                    foreach (var shapePoint in (MyShapes[MyShapes.Count - 1].MyShape as Polygon).Points)
                    {
                        double Sx = shapePoint.X;
                        double Sy = -shapePoint.Y; //Minus might not be needed

                        double Scc = 0;
                        double Sm = MList[MList.Count - 1];

                        double Sd1 = (1 + Math.Pow(Sm, 2));
                        double Sd2 = (Sy - Scc);
                        double Sd3 = (Sd2 * Sm) + Sx;

                        double Sd_final = Sd3 / Sd1;

                        double SxNew = 2 * Sd_final - Sx;
                        double SyNew = -((2 * Sd_final * Sm) - Sy + (2 * Scc));

                        myPointCollection.Add(new Point(SxNew, SyNew));
                    }

					MyShapes.Add((new FreeForm("dupe_reflection").ReflectionGhost(myPointCollection, 255, 0, 0, MyCanvas, "")));
									

                    double x = Canvas.GetLeft(MyShapes[MyShapes.Count - 2].MyShape);
                    double y = -Canvas.GetTop(MyShapes[MyShapes.Count - 2].MyShape);

                    double cc = (Convert.ToDouble(CList[CList.Count - 1]) * ScaleFactor);

                    double d1 = (1 + Math.Pow(MList[MList.Count - 1], 2));
                    double d2 = (y - cc);
                    double d3 = (d2 * MList[MList.Count - 1]) + x;

                    double dFinal = d3 / d1;


                    double xNew = 2 * dFinal - x;
                    double yNew = -((2 * dFinal * MList[MList.Count - 1]) - y + (2 * cc));


                    Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, yNew);
                    Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, xNew);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Failed to randomly generate an 'Reflection Hard' exam. " + Properties.Resources.CriticalFailuer,
                    "Critical Program Failure: 400 I", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
        private void SubmitAnswer(object sender, RoutedEventArgs e)
		{
			try
			{
				if (M.Text == MList[Exams.QuestionPos - 1].ToString() && C.Text == CList[Exams.QuestionPos - 1].ToString())
				{
                    Show(CorrectAnswer);
					Exams.AddPoint();
					NextQuestion();
				}
				else
				{
                    double Sm = Convert.ToDouble(M.Text);
                    double cc = (Convert.ToDouble(C.Text) * ScaleFactor);

					Exams.AddAttempt();

					if (Exams.Attmepts > 2)
                    {
                        Show(OutofAttempts);
                        NextQuestion();
                    }
                    else
                    {
                        Show(WrongAnswer);
                    
                        RefLine.Add(new Line() {
							Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
							StrokeThickness = 2,
							Y1 = (-(Convert.ToDouble(C.Text) * (ScaleFactor))) + ((3500) * (Convert.ToDouble(M.Text))),
							X1 = -3500,
							X2 = 3500,
							Y2 = (-(Convert.ToDouble(C.Text) * (ScaleFactor)) - ((3500) * (Convert.ToDouble(M.Text))))
						});
                        MyCanvas.Children.Add(RefLine[RefLine.Count - 1]);


                        PointCollection myPointCollection = new PointCollection();
                        foreach (var shapePoint in (MyShapes[Exams.ArrayPos].MyShape as Polygon).Points)
                        {
                            double Sx = shapePoint.X;
                            double Sy = -shapePoint.Y; 

                            double Scc = 0;
                          
                            double Sd1 = (1 + Math.Pow(Sm, 2));
                            double Sd2 = (Sy - Scc);
                            double Sd3 = (Sd2 * Sm) + Sx;

                            double SdFinal = Sd3 / Sd1;

                            double SxNew = 2 * SdFinal - Sx;
                            double SyNew = -((2 * SdFinal * Sm) - Sy + (2 * Scc));

                            myPointCollection.Add(new Point(SxNew, SyNew));
                        }

						MyShapes.Add((new FreeForm("dupe_reflection").ReflectionGhost(myPointCollection, 0, 0, 0, MyCanvas, "")));
						
                        double x = Canvas.GetLeft(MyShapes[Exams.ArrayPos].MyShape);
                        double y = -Canvas.GetTop(MyShapes[Exams.ArrayPos].MyShape);

                        double d1 = (1 + Math.Pow(Convert.ToDouble(M.Text), 2));
                        double d2 = (y - cc);
                        double d3 = (d2 * Convert.ToDouble(M.Text)) + x;

                        double d_final = d3 / d1;
                        
                        double x_new = 2 * d_final - x;
                        double y_new = -((2 * d_final * Convert.ToDouble(M.Text)) - y + (2 * cc));
                        
                        Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, y_new);
                        Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, x_new);
                    }
				}
				RefreshText();
			}
			catch (Exception)
			{
				MessageBox.Show(
					"The line equation entered is not in the correct format; only numerical values are allowed. " + Properties.Resources.UserError,
					"Invalid Input Type Error: 302 G", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
		private void NextQuestion()
		{
			Exams.AddQuesPos();
			Exams.ArrayPos += 2;
			Exams.ResetAttempts();

			foreach (var shape in MyShapes)
			{
				shape.MyShape.Visibility = Visibility.Hidden;
			}
			foreach (var shape in RefLine)
			{
				shape.Visibility = Visibility.Hidden;
			}

			if (Exams.QuestionPos > 6)
			{
				Exams.Timer.Stop();
				BlurEffect myBlurEffect = new BlurEffect {Radius = 10};
				window.Effect = myBlurEffect;
				this.Topmost = false;

				ExamResults exam = new ExamResults(Exams);
				exam.Show();
				this.Owner = exam;
				this.IsEnabled = false;
			}
			else
			{
				MyShapes[Exams.ArrayPos].MyShape.Visibility = Visibility.Visible;
				MyShapes[Exams.ArrayPos + 1].MyShape.Visibility = Visibility.Visible;
				if (hint.IsChecked == true)
				{
					RefLine[Exams.QuestionPos - 1].Visibility = Visibility.Visible;
				}
				RefreshText();

				xSlider.Value = 0;
				ySlider.Value = 0;
				sliderSf.Value = 0;
				Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);
				M.Text = "";
				C.Text = "";
			}
		}
	    private void RefreshText()
		{
			question_no.Content = "Question: " + Exams.QuestionPos.ToString() + "/6";
			score.Content = "Score: " + Exams.ScoreValue.ToString() + "/6";
			attempts.Content = "Attempts: " + Exams.Attmepts.ToString() + "/2";
			question.Content = "What line reflects the original to the ghost shape?";
		}
		private void HintsOn(object sender, RoutedEventArgs e)
		{
			try
			{
				RefLine[Exams.QuestionPos - 1].Visibility = Visibility.Visible;
			}
			catch (Exception) { }
			
		}
		private void HintsOff(object sender, RoutedEventArgs e)
		{
			try
			{
				RefLine[Exams.QuestionPos - 1].Visibility = Visibility.Hidden;
			}
			catch (Exception)
			{
			}
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
		private void KeyPressed(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SubmitAnswer(sender, e);
			}
		}
		private void MouseMove(object sender, MouseEventArgs e)
		{
			cords.Content = "( " + (Convert.ToDouble(Mouse.GetPosition(MyCanvas).X) / ScaleFactor).ToString("0.0") + "  ,  " + (-Convert.ToDouble(Mouse.GetPosition(MyCanvas).Y) / ScaleFactor).ToString("0.0") + " )";
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
