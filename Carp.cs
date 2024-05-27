using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace virtual_aquarium
{
    public class Carp : Canvas
    {
        Random rand = new Random();

        /// <summary>
        /// Создание изображения карпа
        /// </summary>
        public Image AddFish()
        {
            Image image = new Image();

            image.Width = 200;
            image.Stretch = Stretch.Fill;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(@"images\fishes\карп.png", UriKind.Relative);
            bitmap.EndInit();
            image.Source = bitmap;
            return image;
        }

        /// <summary>
        /// Создание прямоугольника и прикрепление к нему изображения карпа
        /// </summary>
        public Rectangle AddFishRectangle(Image fish)
        {
            Rectangle rectangle = new Rectangle();

            rectangle.Width = 200;
            rectangle.Height = 120;

            rectangle.Fill = new ImageBrush(fish.Source);

            return rectangle;
        }

        /// <summary>
        /// Передвижение карпа
        /// </summary>
        public void StartCarpAnimation(List<Rectangle> carps, Canvas MyCanvas)
        {
            if (carps.Count > 0)
            {
                double newLeft = rand.Next(0, (int)(MyCanvas.ActualWidth - carps[0].ActualWidth));
                double newTop = rand.Next(0, (int)(MyCanvas.ActualHeight - carps[0].ActualHeight));

                for (int i = 0; i < carps.Count; i++)
                {
                    Rectangle carp = carps[i];
                    DoubleAnimation da = new DoubleAnimation();
                    double oldLeft = Canvas.GetLeft(carp);
                    da.From = oldLeft;
                    da.To = newLeft;
                    da.Duration = new Duration(TimeSpan.FromSeconds(rand.Next(3, 6) + i));
                    da.Completed += (s, e) => StartCarpAnimation(carps, MyCanvas);

                    DoubleAnimation da2 = new DoubleAnimation();
                    da2.From = Canvas.GetTop(carp);
                    da2.To = newTop;
                    da2.Duration = new Duration(TimeSpan.FromSeconds(rand.Next(3, 6) + i));
                    da2.Completed += (s, e) => StartCarpAnimation(carps, MyCanvas);

                    carp.BeginAnimation(Canvas.LeftProperty, da);
                    carp.BeginAnimation(Canvas.TopProperty, da2);

                    ScaleTransform flipTrans = new ScaleTransform();
                    carp.RenderTransformOrigin = new Point(0.5, 0.5);
                    carp.RenderTransform = flipTrans;

                    if (newLeft < oldLeft)
                    {
                        flipTrans.ScaleX = 1;
                    }
                    else
                    {
                        flipTrans.ScaleX = -1;
                    }
                }
            }
        }
    }
}
