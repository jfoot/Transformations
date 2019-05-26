using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Media.Animation;


namespace Transformations
{
	/// <summary>
	/// Reflection easy exam- the user must specify the type of reflection, either X or Y and then specify the coordinate for the reflection line.
	/// </summary>
	public partial class Reflection_EasyExam : Window
	{
        readonly Exam Exams;
		List<Shapes> MyShapes = new List<Shapes>();

		List<string> RefType = new List<string>();
		List<Line> RefLine = new List<Line>();

		GridLine GridLine;
		public const int ScaleFactor = 30;


		public Reflection_EasyExam()
		{
			InitializeComponent();
			Exams = new Exam(0, -2, "Reflection Easy Exam", 5, timer);
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
				for (int x = 0; x < 6; x++)
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

                    Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomX(border, ScaleFactor));
					Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomY(border, ScaleFactor));

					MyShapes.Add((new FreeForm().WrongGhost(255, 0, 0, (Polygon)MyShapes[MyShapes.Count - 1].MyShape, MyCanvas)));
									
					bool validPos = false;
					int type = Rnd.RandomNumber(0, 2);
					if (type == 0)
					{
						RefType.Add("X");
						MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleX = -1;

						while (validPos == false)
						{
							Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomX(border, ScaleFactor));
							Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, Canvas.GetTop(MyShapes[MyShapes.Count - 2].MyShape));

							if (Canvas.GetLeft(MyShapes[MyShapes.Count - 1].MyShape) > (Canvas.GetLeft(MyShapes[MyShapes.Count - 2].MyShape) + (ScaleFactor*10)) || Canvas.GetLeft(MyShapes[MyShapes.Count - 1].MyShape) < Canvas.GetLeft(MyShapes[MyShapes.Count - 2].MyShape))
							{
								validPos = true;
							}
						}
					}
					else
					{
						RefType.Add("Y");
						MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleY = -1;

						while (validPos == false)
						{
							Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, Canvas.GetLeft(MyShapes[MyShapes.Count - 2].MyShape));
							Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomY(border, ScaleFactor));

							if (Canvas.GetTop(MyShapes[MyShapes.Count - 1].MyShape) > (Canvas.GetTop(MyShapes[MyShapes.Count - 2].MyShape) + ScaleFactor * 10) || Canvas.GetTop(MyShapes[MyShapes.Count - 1].MyShape) < Canvas.GetTop(MyShapes[MyShapes.Count - 2].MyShape))
							{
								validPos = true;
							}
						}
					}
				}
			}
			catch (Exception)
			{
				//MessageBox.Show(
				//	"Failed to randomly generate an 'Reflection Easy' exam. " + Properties.Strings.CriticalFailuer,
				//	Properties.Strings.EM_CriticalFailure + "400 H", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}
		private void SubmitAnswer(object sender, RoutedEventArgs e)
		{
			try
			{
				if (RefType[(Exams.QuestionPos - 1)] == "X" && XButton.IsChecked == true && XAnswer().ToString() == User_answer.Text.ToString() || RefType[(Exams.QuestionPos - 1)] == "Y" && YButton.IsChecked == true && YAnswer().ToString() == User_answer.Text.ToString())
				{
					Exams.AddPoint();
                    Show(CorrectAnswer);
                     NextQuestion();
				}
				else
				{
                    double Guess = (Convert.ToDouble(User_answer.Text) * (ScaleFactor));

					Exams.AddAttempt();
					if (Exams.Attmepts > 2)
                    {
                        Show(OutofAttempts);
                        NextQuestion();
                    }
                    else
                    {
                          Show(WrongAnswer);

                        if (XButton.IsChecked == true)
                        {
                            RefLine.Add(new Line() {
								Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
								StrokeThickness = 2,
								X1 = Guess,
								Y1 = -3500,
								X2 = Guess,
								Y2 = 3500
							}); 
                            MyCanvas.Children.Add(RefLine[RefLine.Count - 1]);

							MyShapes.Add((new FreeForm().WrongGhost(0, 0, 0, (Polygon)MyShapes[Exams.ArrayPos].MyShape, MyCanvas)));
							MyShapes[MyShapes.Count - 1].MyShape.Visibility = Visibility.Visible;
                            MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleX = -1;

                            Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape,
                                RefLine[RefLine.Count - 1].X1 - (Canvas.GetLeft(MyShapes[Exams.ArrayPos].MyShape) -
                                                                   RefLine[RefLine.Count - 1].X1));
                            Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, Canvas.GetTop(MyShapes[Exams.ArrayPos].MyShape));

                        }
                        else if (YButton.IsChecked == true)
                        {
                            RefLine.Add(new Line() {
								Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
								StrokeThickness = 2,
								X1 = -3500,
								Y1 = -Guess,
								X2 = 3500,
								Y2 = -Guess
							});
                            MyCanvas.Children.Add(RefLine[RefLine.Count - 1]);

							MyShapes.Add((new FreeForm().WrongGhost(0, 0, 0, (Polygon)MyShapes[Exams.ArrayPos].MyShape, MyCanvas)));
							MyShapes[MyShapes.Count - 1].MyShape.Visibility = Visibility.Visible;
                            MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleY = -1;

                            Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape,
                                RefLine[RefLine.Count - 1].Y1 - (Canvas.GetTop(MyShapes[Exams.ArrayPos].MyShape) -
                                                                   RefLine[RefLine.Count - 1].Y1));
                            Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, Canvas.GetLeft(MyShapes[Exams.ArrayPos].MyShape));
                        }
                    }
                    
                }
                RefreshText();
			}
			catch (Exception)
			{
				//MessageBox.Show(
				//	"The line equation entered is not in the correct format; only numerical values are allowed. " + Properties.Strings.UserError,
				//	Properties.Strings.EM_InvalidInputTypeError + "302 F", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
		private double XAnswer()
		{
			double x_answer;
			double Orginal_shape = Canvas.GetLeft(MyShapes[Exams.ArrayPos].MyShape);
			double Ghost_shape = Canvas.GetLeft(MyShapes[Exams.ArrayPos + 1].MyShape);
			double difference = ((Ghost_shape + Orginal_shape) / 2);
			x_answer = (difference) / ScaleFactor;
			x_answer = Round.ToNearest(x_answer, 0.5);
			return x_answer;
		}
		private double YAnswer()
		{
			double y_answer;
			double Orginal_shape = Canvas.GetTop(MyShapes[Exams.ArrayPos].MyShape);
			double Ghost_shape = Canvas.GetTop(MyShapes[Exams.ArrayPos + 1].MyShape);
			double difference = ((Ghost_shape + Orginal_shape) / 2);
			y_answer = (difference) / ScaleFactor;
			y_answer = Round.ToNearest(y_answer, 0.5);
			return -y_answer;
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
				BlurEffect myBlurEffect = new BlurEffect { Radius = 10 };
				window.Effect = myBlurEffect;
				this.Topmost = false;

				ExamResults exam = new ExamResults(Exams);
				exam.Show();
				this.Owner = exam;
				this.IsEnabled = false;
			}


			MyShapes[Exams.ArrayPos].MyShape.Visibility = Visibility.Visible;
			MyShapes[Exams.ArrayPos + 1].MyShape.Visibility = Visibility.Visible;
			RefreshText();

			xSlider.Value = 0;
			ySlider.Value = 0;
			sliderSf.Value = 0;
			Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);

			User_answer.Text = "";
		}
		private void RefreshText()
		{
			question_no.Content = "Question: " + Exams.QuestionPos.ToString() + "/6";
			score.Content = "Score: " + Exams.ScoreValue.ToString() + "/6";
			attempts.Content = "Attempts: " + Exams.Attmepts.ToString() + "/2";
			question.Content = "What line reflects the original to the ghost shape?";
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
			MessageBoxResult exit = MessageBox.Show("Are you sure you wish to abandon this exam?", Properties.Strings.AreYouSure,
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
