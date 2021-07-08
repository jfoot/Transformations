using System.Windows;
using System.Windows.Input;

namespace Transformations
{
    /// <summary>
    /// Interaction logic for WhatsNew.xaml
    /// </summary>
    public partial class WhatsNew : Window
    {
        public WhatsNew()
        {
            InitializeComponent();
        }

        private void PPolicy(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Strings.PPolicyLink);
        }

        private void TDOff(object sender, MouseButtonEventArgs e)
        {
            this.Close();
            Settings s = new Settings();
            s.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
