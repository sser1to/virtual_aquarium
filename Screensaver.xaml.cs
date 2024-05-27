using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace virtual_aquarium
{
    /// <summary>
    /// Логика взаимодействия для Screensaver.xaml
    /// </summary>
    public partial class Screensaver : Window
    {

        public Screensaver()
        {
            InitializeComponent();

            PreviewKeyDown += new KeyEventHandler(EnterClick);

            // Запуск таймера для воспроизведения звука и появления логотипа
            DispatcherTimer timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromSeconds(1);
            timer1.Tick += (s, e) =>
            {
                timer1.Stop();

                DoubleAnimation animation1 = new DoubleAnimation();

                animation1.From = 0;
                animation1.To = 1;

                animation1.Duration = new Duration(TimeSpan.FromSeconds(2));

                img.Opacity = 0;
                img.BeginAnimation(Image.OpacityProperty, animation1);

                SoundPlayer sp = new SoundPlayer(@"sounds\appearance.wav");
                sp.Play();
            };
            timer1.Start();

            // Запуск таймера для появления текста
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3.5);
            timer.Tick += (s, e) =>
            {
                timer.Stop();

                ColorAnimation animation = new ColorAnimation();

                animation.From = Colors.White;
                animation.To = Colors.Black;

                animation.Duration = new Duration(TimeSpan.FromSeconds(2));

                lbl.Foreground = new SolidColorBrush(Colors.White);
                lbl.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            };
            timer.Start();
        }

        private void EnterClick(object sender, KeyEventArgs e)
        {
            // Переход на другое окно по нажатию Enter
            if (e.Key == Key.Enter)
            {
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
        }
    }
}
