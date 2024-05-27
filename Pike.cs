using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace virtual_aquarium
{
    public class Pike : Canvas
    {
        Random rand = new Random();

        public int status;

        /// <summary>
        /// Создание изображения щуки
        /// </summary>
        public Image AddFish()
        {
            Image image = new Image();

            image.Width = 500;
            image.Stretch = Stretch.Fill;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(@"images\fishes\щука.png", UriKind.Relative);
            bitmap.EndInit();
            image.Source = bitmap;
            return image;
        }

        /// <summary>
        /// Создание прямоугольника и прикрепление к нему изоюражения щуки
        /// </summary>
        public Rectangle AddFishRectangle(Image fish)
        {
            Rectangle rectangle = new Rectangle();

            rectangle.Width = 500;
            rectangle.Height = 200;

            rectangle.Fill = new ImageBrush(fish.Source);

            return rectangle;
        }

        /// <summary>
        /// Передвижение щуки
        /// </summary>
        public void StartPikeAnimation(List<Rectangle> pikes, Canvas MyCanvas)
        {
            if (pikes.Count > 0)
            {
                double newLeft = rand.Next(0, (int)(MyCanvas.ActualWidth - pikes[0].ActualWidth));
                double newTop = rand.Next(0, (int)(MyCanvas.ActualHeight - pikes[0].ActualHeight));

                for (int i = 0; i < pikes.Count; i++)
                {
                    Rectangle pike = pikes[i];
                    DoubleAnimation da = new DoubleAnimation();
                    double oldLeft = Canvas.GetLeft(pike);
                    da.From = oldLeft;
                    da.To = newLeft;
                    da.Duration = new Duration(TimeSpan.FromSeconds(rand.Next(3, 6) + i));
                    da.Completed += (s, e) => StartPikeAnimation(pikes, MyCanvas);

                    DoubleAnimation da2 = new DoubleAnimation();
                    da2.From = Canvas.GetTop(pike);
                    da2.To = newTop;
                    da2.Duration = new Duration(TimeSpan.FromSeconds(rand.Next(3, 6) + i));
                    da2.Completed += (s, e) => StartPikeAnimation(pikes, MyCanvas);

                    pike.BeginAnimation(Canvas.LeftProperty, da);
                    pike.BeginAnimation(Canvas.TopProperty, da2);

                    ScaleTransform flipTrans = new ScaleTransform();
                    pike.RenderTransformOrigin = new Point(0.5, 0.5);
                    pike.RenderTransform = flipTrans;

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

        /// <summary>
        /// Охота щуки на карпов
        /// </summary>
        public void StartPikeChaseAnimation(List<Rectangle> carps, List<Rectangle> pikes, Canvas MyCanvas)
        {
            if (carps.Count > 0 && pikes.Count > 0)
            {
                status = 1;
                Rectangle targetCarp = carps[rand.Next(carps.Count)];

                for (int i = 0; i < pikes.Count; i++)
                {
                    Rectangle pike = pikes[i];
                    DoubleAnimation da = new DoubleAnimation();
                    double oldLeft = Canvas.GetLeft(pike);
                    double newLeft = Canvas.GetLeft(targetCarp);
                    da.From = oldLeft;
                    da.To = newLeft;
                    da.Duration = new Duration(TimeSpan.FromSeconds(rand.Next(2, 4) + i));
                    da.Completed += (s, e) => StartPikeChaseAnimation(carps, pikes, MyCanvas);

                    DoubleAnimation da2 = new DoubleAnimation();
                    da2.From = Canvas.GetTop(pike);
                    da2.To = Canvas.GetTop(targetCarp);
                    da2.Duration = new Duration(TimeSpan.FromSeconds(rand.Next(2, 4) + i));
                    da2.Completed += (s, e) => StartPikeChaseAnimation(carps, pikes, MyCanvas);

                    pike.BeginAnimation(Canvas.LeftProperty, da);
                    pike.BeginAnimation(Canvas.TopProperty, da2);

                    ScaleTransform flipTrans = new ScaleTransform();
                    pike.RenderTransformOrigin = new Point(0.5, 0.5);
                    pike.RenderTransform = flipTrans;

                    if (newLeft < oldLeft)
                    {
                        flipTrans.ScaleX = 1;
                    }
                    else
                    {
                        flipTrans.ScaleX = -1;
                    }

                    if (Math.Abs(newLeft - oldLeft) <= pike.ActualWidth && Math.Abs(da2.To.Value - da2.From.Value) <= pike.ActualHeight)
                    {
                        if (carps.Contains(targetCarp))
                        {
                            Canvas.SetLeft(targetCarp, Canvas.GetLeft(targetCarp));
                            Canvas.SetTop(targetCarp, Canvas.GetTop(targetCarp));
                            targetCarp.BeginAnimation(Canvas.LeftProperty, null);
                            targetCarp.BeginAnimation(Canvas.TopProperty, null);

                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(0.5);
                            timer.Tick += (s, e) =>
                            {
                                MyCanvas.Children.Remove(targetCarp);
                                carps.Remove(targetCarp);
                                timer.Stop();
                                StartPikeChaseAnimation(carps, pikes, MyCanvas);
                            };
                            timer.Start();
                        }
                    }
                }
            }
            else
            {
                status = 0;
                StartPikeAnimation(pikes, MyCanvas);
            }
        }
    }
}
