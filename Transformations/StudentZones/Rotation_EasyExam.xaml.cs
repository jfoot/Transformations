using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MessageBox = System.Windows.MessageBox;
using MouseEventHandler = System.Windows.Input.MouseEventHandler;
using System.Windows.Shapes;

namespace Transformations
{
	/// <summary>
	/// Rotation exam easy, the user must specify both the clockwise and anti-clockwise amount to map the original shape onto the ghost shape.
	/// </summary>
	public partial class Rotation_EasyExam : Window
	{
        Exam Exams;
		List<Shapes> MyShapes = new List<Shapes>();
		readonly int[] Values =  { 45, 90, 135, 180, 255, 270, 315 };
		readonly int[] InverseValues = {  315, 270, 255, 180, 135, 90, 45 };
		List<int> Answers = new List<int>();
		GridLine GridLines;
		const int ScaleFactor = 30;
	
		public Rotation_EasyExam()
		{
			InitializeComponent();
			Exams = new Exam(0, -2, Properties.Strings.RotEasyE, 7, timer);
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

					MyShapes.Add((new FreeForm().WrongGhost(0, 255, 0, (Polygon)MyShapes[MyShapes.Count - 1].MyShape, MyCanvas)));
					Answers.Add(Rnd.RandomNumber(0, 7));
                    MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterX = -((Canvas.GetLeft(MyShapes[MyShapes.Count - 1].MyShape)));
                    MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterY = (-(Canvas.GetTop(MyShapes[MyShapes.Count - 1].MyShape)));
                    MyShapes[MyShapes.Count - 1].MyRotateTransform.Angle = Values[Answers[Answers.Count - 1]];
                }
            }
            catch (Exception)
            {
                MessageBox.Show(
                    Properties.Strings.FailedToMakeExam + Properties.Strings.CriticalFailuer,
                    Properties.Strings.EM_CriticalFailure + "400 J", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SubmitAnswer(object sender, RoutedEventArgs e)
		{
			try
			{
				if (Answers[Exams.QuestionPos - 1] == clockwise_rot.SelectedIndex && (6 - Answers[Exams.QuestionPos - 1]) == anticlock_rot.SelectedIndex)
				{
                    Show(CorrectAnswer);
					Exams.AddPoint();
					NextQuestion();
				}
				else
				{
					Exams.AddAttempt();

					if (Exams.Attmepts > 2)
                    {
                        Show(OutofAttempts);
                        NextQuestion();
                    }
                    else
                    {
                        Show(WrongAnswer);

						//ClockWise Guess
						MyShapes.Add((new FreeForm().WrongGhost(0, 0, 0, (Polygon)MyShapes[Exams.ArrayPos].MyShape, MyCanvas)));

						MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterX = -((Canvas.GetLeft(MyShapes[Exams.ArrayPos].MyShape)));
                        MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterY = -(Canvas.GetTop(MyShapes[Exams.ArrayPos].MyShape));
                        MyShapes[MyShapes.Count - 1].MyRotateTransform.Angle = Values[clockwise_rot.SelectedIndex];

						//Anti-ClockWise Guess
						MyShapes.Add((new FreeForm().WrongGhost(0, 0, 0, (Polygon)MyShapes[Exams.ArrayPos].MyShape, MyCanvas)));

						MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterX = -((Canvas.GetLeft(MyShapes[Exams.ArrayPos].MyShape)));
                        MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterY = -(Canvas.GetTop(MyShapes[Exams.ArrayPos].MyShape));
                        MyShapes[MyShapes.Count - 1].MyRotateTransform.Angle = InverseValues[anticlock_rot.SelectedIndex];
                    }
                }
                RefreshText();
            }
			catch (Exception)
			{
                MessageBox.Show(
                    Properties.Strings.NoClockGuess + Properties.Strings.UserError,
                    Properties.Strings.EM_FieldEmpty + "300 I", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
            }
		}
		private void NextQuestion()
		{
			Exams.AddQuesPos();
			Exams.ArrayPos += 2;
			Exams.ResetAttempts();

            //makes all shapes invisible 
            MyShapes.ForEach(p => p.MyShape.Visibility = Visibility.Hidden);

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

			MyShapes.Add((new Circle("dupe_rotation").MakerSpawn(0, 0, MyCanvas)));
			MyShapes[Exams.ArrayPos].MyShape.Visibility = Visibility.Visible;
			MyShapes[Exams.ArrayPos + 1].MyShape.Visibility = Visibility.Visible;
			RefreshText();

			xSlider.Value = 0;
			ySlider.Value = 0;
			sliderSf.Value = 0;
			Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);
		}
		private void RefreshText()
		{
            Exams.Refresh(question_no, score, attempts);
            question.Content = Properties.Strings.RotEasyEText;
        }
        private void CanvasLoaded(object sender, RoutedEventArgs e)
		{
			GridLines = new GridLine().DrawGrid(3500, ScaleFactor, MyCanvas);
			Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);
			Randomise();            
			NextQuestion();
            GridLines.Labels.ForEach(p => MyCanvas.Children.Add(p));
        }	
	
		private void Scaling(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);
		}
        private void KeyPressed(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SubmitAnswer(sender, e);
			}
		}
        private void MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			cords.Content = "( " + (Convert.ToDouble(Mouse.GetPosition(MyCanvas).X) / ScaleFactor).ToString("0.0") + "  ,  " + (-Convert.ToDouble(Mouse.GetPosition(MyCanvas).Y) / ScaleFactor).ToString("0.0") + " )";
        }
        private void Show(Border type) //Shows the correct, incorrect or skip answer method.
        {
            Exams.Show(type);
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            Exams.Exit(this);
        }
    }
}
