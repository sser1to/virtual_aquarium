using System.Windows;

namespace virtual_aquarium
{
    /// <summary>
    /// Логика взаимодействия для Parameters1.xaml
    /// </summary>
    public partial class Parameters1 : Window
    {

        public int Type { get; set; }

        public Parameters1()
        {
            InitializeComponent();

            // Запрет на изменение размера окна
            this.ResizeMode = ResizeMode.NoResize;
            this.MaxHeight = this.Height;
            this.MaxWidth = this.Width;
        }

        private void BtnOK(object sender, RoutedEventArgs e)
        {
            Type = cbx.SelectedIndex;
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
