using System.Windows;

namespace Transformations
{
	public partial class MainWindow
	{
		/// <summary>
		///                     Scaling
		/// Here the program recalculates any scaling needed.
		/// - Changes in the screen size.
		/// - Changes in the slider value.
		/// - Changes in Axis Location.
		/// </summary>
	
		private void SilderValueChanged(object sender,	RoutedPropertyChangedEventArgs<double> e) //Zoom Slider Bar - trigged upon changing.
		{
			XSlider.Maximum = MaxValue - ((border.ActualWidth / 2) / sliderSf.Value);
			XSlider.Minimum = -MaxValue + ((border.ActualWidth / 2) / sliderSf.Value);
			YSlider.Maximum = MaxValue - ((border.ActualHeight / 2) / sliderSf.Value);
			YSlider.Minimum = -MaxValue + ((border.ActualHeight / 2) / sliderSf.Value);
			Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, XSlider, YSlider,sliderSf ,border);
		}
    	private void SizeChange(object sender, SizeChangedEventArgs e) //Windows Size - trigged upon changing the size of the window.
		{
			XSlider.Maximum = MaxValue - ((border.ActualWidth / 2) / sliderSf.Value);
			XSlider.Minimum = -MaxValue + ((border.ActualWidth / 2) / sliderSf.Value);
			YSlider.Maximum = MaxValue - ((border.ActualHeight / 2) / sliderSf.Value);
			YSlider.Minimum = -MaxValue + ((border.ActualHeight / 2) / sliderSf.Value);
			Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, XSlider, YSlider, sliderSf, border);
		}
	}
}