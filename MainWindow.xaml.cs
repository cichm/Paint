using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;

using System.IO;
using System.Reflection;

namespace DragAndDropApp
{
    public partial class DragAndDropControls : Window
    {
        double FirstXPos, FirstYPos, FirstArrowXPos, FirstArrowYPos;
        object MovingObject;
        Line Path1, Path2, Path3, Path4;
        Rectangle FirstPosition, CurrentPosition;

        private showit _showit;

        public DragAndDropControls()
        {
            InitializeComponent();

            #region *** 'Latajace obiekty ***

            /*
             * Assigning PreviewMouseLeftButtonDown, PreviewMouseMove and PreviewMouseLeftButtonUp
             * events to each controls on our canvas control.
             * Some controls events like TextBox's MouseLeftButtonDown doesn't fire, because of that
             * we use Preview events.
             */

            foreach (Control control in DesigningCanvas.Children)
            {
                control.PreviewMouseLeftButtonDown += this.MouseLeftButtonDown;
                control.PreviewMouseMove += this.MouseMove;
                control.PreviewMouseLeftButtonUp += this.PreviewMouseLeftButtonUp;
                control.Cursor = Cursors.Hand;
            }

            // Setting up the Lines that we want to show the path of movement
            List<Double> Dots = new List<double>();
            Dots.Add(1);
            Dots.Add(2);
            Path1 = new Line();
            Path1.Width = DesigningCanvas.Width;
            Path1.Height = DesigningCanvas.Height;
            Path1.Stroke = Brushes.DarkGray;
            Path1.StrokeDashArray = new DoubleCollection(Dots);

            Path2 = new Line();
            Path2.Width = DesigningCanvas.Width;
            Path2.Height = DesigningCanvas.Height;
            Path2.Stroke = Brushes.DarkGray;
            Path2.StrokeDashArray = new DoubleCollection(Dots);

            Path3 = new Line();
            Path3.Width = DesigningCanvas.Width;
            Path3.Height = DesigningCanvas.Height;
            Path3.Stroke = Brushes.DarkGray;
            Path3.StrokeDashArray = new DoubleCollection(Dots);

            Path4 = new Line();
            Path4.Width = DesigningCanvas.Width;
            Path4.Height = DesigningCanvas.Height;
            Path4.Stroke = Brushes.DarkGray;
            Path4.StrokeDashArray = new DoubleCollection(Dots);

            FirstPosition = new Rectangle();
            FirstPosition.Stroke = Brushes.DarkGray;
            FirstPosition.StrokeDashArray = new DoubleCollection(Dots);

            CurrentPosition = new Rectangle();
            CurrentPosition.Stroke = Brushes.DarkGray;
            CurrentPosition.StrokeDashArray = new DoubleCollection(Dots);

            // Adding Lines to main designing panel(Canvas)
            DesigningCanvas.Children.Add(Path1);
            DesigningCanvas.Children.Add(Path2);
            DesigningCanvas.Children.Add(Path3);
            DesigningCanvas.Children.Add(Path4);
            DesigningCanvas.Children.Add(FirstPosition);
            DesigningCanvas.Children.Add(CurrentPosition);
        }

        void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // In this event, we should set the lines visibility to Hidden
            MovingObject = null;
            Path1.Visibility = System.Windows.Visibility.Hidden;
            Path2.Visibility = System.Windows.Visibility.Hidden;
            Path3.Visibility = System.Windows.Visibility.Hidden;
            Path4.Visibility = System.Windows.Visibility.Hidden;
            FirstPosition.Visibility = System.Windows.Visibility.Hidden;
            CurrentPosition.Visibility = System.Windows.Visibility.Hidden;
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            /*
             * In this event, at first we check the mouse left button state. If it is pressed and 
             * event sender object is similar with our moving object, we can move our control with
             * some effects.
             */
            if (e.LeftButton == MouseButtonState.Pressed &&
                sender == MovingObject)
            {
                // We start to moving objects with setting the lines positions.
                Path1.X1 = FirstArrowXPos;
                Path1.Y1 = FirstArrowYPos;
                Path1.X2 = e.GetPosition((sender as Control).Parent as Control).X - FirstXPos - 20;
                Path1.Y2 = e.GetPosition((sender as Control).Parent as Control).Y - FirstYPos - 20;

                Path2.X1 = Path1.X1 + (MovingObject as Control).ActualWidth;
                Path2.Y1 = Path1.Y1;
                Path2.X2 = Path1.X2 + (MovingObject as Control).ActualWidth;
                Path2.Y2 = Path1.Y2;

                Path3.X1 = Path1.X1;
                Path3.Y1 = Path1.Y1 + (MovingObject as Control).ActualHeight;
                Path3.X2 = Path1.X2;
                Path3.Y2 = Path1.Y2 + (MovingObject as Control).ActualHeight;

                Path4.X1 = Path1.X1 + (MovingObject as Control).ActualWidth;
                Path4.Y1 = Path1.Y1 + (MovingObject as Control).ActualHeight;
                Path4.X2 = Path1.X2 + (MovingObject as Control).ActualWidth;
                Path4.Y2 = Path1.Y2 + (MovingObject as Control).ActualHeight;

                FirstPosition.Width = (MovingObject as Control).ActualWidth;
                FirstPosition.Height = (MovingObject as Control).ActualHeight;
                FirstPosition.SetValue(Canvas.LeftProperty, FirstArrowXPos);
                FirstPosition.SetValue(Canvas.TopProperty, FirstArrowYPos);

                CurrentPosition.Width = (MovingObject as Control).ActualWidth;
                CurrentPosition.Height = (MovingObject as Control).ActualHeight;
                CurrentPosition.SetValue(Canvas.LeftProperty, Path1.X2);
                CurrentPosition.SetValue(Canvas.TopProperty, Path1.Y2);

                Path1.Visibility = System.Windows.Visibility.Visible;
                Path2.Visibility = System.Windows.Visibility.Visible;
                Path3.Visibility = System.Windows.Visibility.Visible;
                Path4.Visibility = System.Windows.Visibility.Visible;
                FirstPosition.Visibility = System.Windows.Visibility.Visible;
                CurrentPosition.Visibility = System.Windows.Visibility.Visible;

                /*
                 * For changing the position of a control, we should use the SetValue method to setting
                 * the Canvas.LeftProperty and Canvas.TopProperty dependencies.
                 * 
                 * For calculating the currect position of the control, we should do :
                 *      Current position of the mouse cursor on the object parent - 
                 *      Mouse position on the control at the start of moving -
                 *      position of the control's parent.
                 */
                (sender as Control).SetValue(Canvas.LeftProperty,
                    e.GetPosition((sender as Control).Parent as Control).X - FirstXPos - 20);

                (sender as Control).SetValue(Canvas.TopProperty,
                    e.GetPosition((sender as Control).Parent as Control).Y - FirstYPos - 20);
            }
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //In this event, we get current mouse position on the control to use it in the MouseMove event.
            FirstXPos = e.GetPosition(sender as Control).X;
            FirstYPos = e.GetPosition(sender as Control).Y;
            FirstArrowXPos = e.GetPosition((sender as Control).Parent as Control).X - FirstXPos - 20;
            FirstArrowYPos = e.GetPosition((sender as Control).Parent as Control).Y - FirstYPos - 20;
            MovingObject = sender;

            #endregion


            #region *** Boczny panel - znikające przyciski i znikający panel /MainWindow.xaml ***
            _showit = new showit();
            MrPanel.Children.Add(_showit);
            #endregion 

            Point pt = e.GetPosition(null);
            X_position.Content = string.Format("Cursor position: X = {0}, Y = {1}", pt.X, pt.Y);
        }

        #region *** przyciski i funkcje do nich nalezace ***

        /// <summary>
        /// Poniżej znajdują się zaprogramowane przyciski.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            fSelection();
        }

        private void fSelection()
        {
            BigCanvas.EditingMode = InkCanvasEditingMode.Select;
        }

        /// <summary>
        /// Przycisk odpowiedzialny za wygladzanie zaznaczonych elementow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Smooth_Click(object sender, RoutedEventArgs e)
        {
            fSmoot();
        }

        private void fSmoot()
        {
            StrokeCollection strokeCol = BigCanvas.GetSelectedStrokes();
            if (strokeCol.Count > 0)
            {
                foreach (System.Windows.Ink.Stroke oStroke in strokeCol)
                {
                    oStroke.DrawingAttributes.FitToCurve = true;
                }
            }
        }

        /// <summary>
        /// Dostępna w programie paleta kolorow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //po to aby rysować po BigCanvas-ie (po wymazywaniu)
            BigCanvas.EditingMode = InkCanvasEditingMode.Ink;
            DrawingAttributes inkk = new DrawingAttributes();

            if (this.cboColor.SelectedIndex == 1)
            {
                BigCanvas.DefaultDrawingAttributes.Color = Colors.Black;
            }
            else if (this.cboColor.SelectedIndex == 2)
            {
                BigCanvas.DefaultDrawingAttributes.Color = Colors.Blue;
            }
            else if (this.cboColor.SelectedIndex == 3)
            {
                BigCanvas.DefaultDrawingAttributes.Color = Colors.Green;
            }
            else if (this.cboColor.SelectedIndex == 4)
            {
                BigCanvas.DefaultDrawingAttributes.Color = Colors.Red;
            }
            else if (this.cboColor.SelectedIndex == 5)
            {
                BigCanvas.DefaultDrawingAttributes.Color = Colors.Yellow;
            }
        }

        private void lbHeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //po to aby rysować po BigCanvas-ie (po wymazywaniu)
            BigCanvas.EditingMode = InkCanvasEditingMode.Ink;
            DrawingAttributes inkk = new DrawingAttributes();

            if (this.lbHeight.SelectedIndex == 1)
            {
                inkk.Height = 2;
                inkk.Width = 2;
            }
            else if (this.lbHeight.SelectedIndex == 2)
            {
                inkk.Height = 4;
                inkk.Width = 4;

            }
            else if (this.lbHeight.SelectedIndex == 3)
            {
                inkk.Height = 6;
                inkk.Width = 6;
            }
            else if (this.lbHeight.SelectedIndex == 4)
            {
                inkk.Height = 8;
                inkk.Width = 8;
            }

            BigCanvas.DefaultDrawingAttributes = inkk;
        }

        /// <summary>
        /// Gumka i jej rodzaje.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbErase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BigCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
            if (this.lbErase.SelectedIndex == 1)
            {
                BigCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
            }
            else if (this.lbErase.SelectedIndex == 2)
            {
                EllipseStylusShape ellipseStylus = new EllipseStylusShape(4, 4, 0);
                BigCanvas.EraserShape = ellipseStylus;
            }
            else if (this.lbErase.SelectedIndex == 3)
            {
                EllipseStylusShape ellipseStylus = new EllipseStylusShape(6, 6, 0);
                BigCanvas.EraserShape = ellipseStylus;
            }
            else if (this.lbErase.SelectedIndex == 4)
            {
                EllipseStylusShape ellipseStylus = new EllipseStylusShape(8, 8, 0);
                BigCanvas.EraserShape = ellipseStylus;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            fSave();
        }

        public void fSave()
        {
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";
            if (saveDialog.ShowDialog().Value == true)
            {
                RenderTargetBitmap targetBitmap =
                    new RenderTargetBitmap((int)BigCanvas.ActualWidth,
                                           (int)BigCanvas.ActualHeight,
                                           96d, 96d,
                                           PixelFormats.Default);
                targetBitmap.Render(BigCanvas);

                BitmapEncoder encoder = new BmpBitmapEncoder();
                string extension = saveDialog.FileName.Substring(saveDialog.FileName.LastIndexOf('.'));
                switch (extension.ToLower())
                {
                    case ".jpg":
                        encoder = new JpegBitmapEncoder();
                        break;
                    case ".bmp":
                        encoder = new BmpBitmapEncoder();
                        break;
                    case ".gif":
                        encoder = new GifBitmapEncoder();
                        break;
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;
                }
                encoder.Frames.Add(BitmapFrame.Create(targetBitmap));
                using (FileStream fs = File.Open(saveDialog.FileName, FileMode.OpenOrCreate))
                {
                    encoder.Save(fs);
                }
            }
        }

        private void Shapes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Pędzel
            if (this.lbShapes.SelectedIndex == 1)
            {
                BigCanvas.EditingMode = InkCanvasEditingMode.Ink;
            }
            // Kolo
            else if (this.lbShapes.SelectedIndex == 2)
            {
                Stroke oS = NewCircle(200.0, 300.0, 50.0);

                DrawingAttributes attribs = new DrawingAttributes();
                attribs.Color = Colors.LimeGreen;
                attribs.Height = 1.0;
                attribs.Width = 1.0;
                attribs.StylusTip = StylusTip.Ellipse;
                attribs.FitToCurve = false;

                oS.DrawingAttributes = attribs;
                BigCanvas.Strokes.Add(oS);
            }
            // Kwadrat
            else if (this.lbShapes.SelectedIndex == 3)
            {
                Shape r = new Rectangle() { Fill = Brushes.Blue, Height = 45, Width = 45, RadiusX = 0, RadiusY = 0 };

                // Ustawianie rozmiarow
                r.Width = 50;
                r.Height = 50;

                // Kolor wypelnienia
                r.Fill = Brushes.White;

                // Grubosc obramowania (domyslnie 0)
                r.StrokeThickness = 2;
                r.Stroke = Brushes.Black;

                // Pozycja
                Canvas.SetTop(r, 100);
                Canvas.SetLeft(r, 100);

                // Rysowanie obiektu w Canvas
                BigCanvas.Children.Add(r);
            }
        }

        /// <summary>
        /// Funcja odpowiedzialna za wyrysowywanie kola na ekranie.
        /// </summary>
        /// <param name="dTop"></param>
        /// <param name="dLeft"></param>
        /// <param name="dRadius"></param>
        /// <returns></returns>
        private Stroke NewCircle(double dTop, double dLeft, double dRadius)
        {
            double T = dTop;
            double L = dLeft;
            double R = dRadius;
            StylusPoint point;

            StylusPointCollection strokePoints = new StylusPointCollection();
            for (int i = 0; i < 360; i++)
            {
                point = new StylusPoint(L + R * Math.Sin(i), T + R * Math.Cos(i));
                strokePoints.Add(point);
            }
            Stroke newStroke = new Stroke(strokePoints);
            return newStroke;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            fOpen();
        }

        public void fOpen()
        {
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
            if (openDialog.ShowDialog().Value == true)
            {
                picture.Source = new BitmapImage(new Uri(openDialog.FileName));
                BigCanvas.Width = picture.Width;
                BigCanvas.Height = picture.Height;
            }

            fClear();
        }

        /// <summary>
        /// Wyczysc okno
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clean_Click(object sender, RoutedEventArgs e)
        {
            fClear();
        }

        /// <summary>
        /// Bo w programie mysimy wymazak okno 'z kilku miejsc'.
        /// </summary>
        public void fClear()
        {
            BigCanvas.Strokes.Clear();
        }

        /// <summary>
        /// Przycisk odpowiedzialny za wyswietlenie wszystkich punktow rysunku.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Point_Click(object sender, RoutedEventArgs e)
        {
            fPoint();
        }

        private void fPoint()
        {
            // Pokaż wszystkie punkty
            string s = GetAllPoints();

            // Wyswietl wszystko w textboxie.
            TextBox oT = new TextBox();
            oT.AcceptsReturn = true;
            oT.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            oT.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            oT.FontFamily = new FontFamily("Lucida Console");
            oT.FontSize = 12.0;
            oT.Text = s;
            oT.SelectionLength = 0;
            oT.SelectionStart = oT.Text.Length;

            // Otworz nowe okno i 'wrzuc tam' wszystko to co potrzebne
            Window oF = new Window();
            oF.MaxWidth = 300;
            oF.Width = 300;
            oF.Height = 300;
            oF.Title = "    StylusPoints";
            oF.Content = oT;
            oF.ShowDialog();
        }

        private string GetAllPoints()
        {
            int iP = -1;        // Numeracja punktow.
            string s = "Point" + "\t\t" + " X" + "\t\t" + " Y" + "\r\n";
            StylusPointCollection colStylusPoints;

            // Czy zaznaczony element?
            StrokeCollection strokeCol = BigCanvas.GetSelectedStrokes();
            if (strokeCol.Count < 1)
            {
                // Kompletny rysunek.
                strokeCol = BigCanvas.Strokes;
            }

            // Dosań punkt z zaznaczonego i kompletnego rysunku.
            foreach (Stroke oS in strokeCol)
            {
                colStylusPoints = oS.StylusPoints;
                foreach (StylusPoint oPoint in colStylusPoints)
                {
                    iP += 1;
                    s += iP.ToString().PadLeft(4, '0') + "\t\t" +
                        ((int)oPoint.X).ToString() + "\t\t" +
                        ((int)oPoint.Y).ToString() + "\r\n";
                }
            }
            return s;
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            fCopy();
        }

        private void fCopy()
        {
            //jeśli zaznaczony jest przynajmniej jeden element
            if (BigCanvas.GetSelectedStrokes().Count > 0)
            {
                BigCanvas.CopySelection();
            }
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            fPaste();
        }

        private void fPaste()
        {
            BigCanvas.Paste();
        }

        void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(null);
            X_position.Content = string.Format("Cursor position: X = {0}, Y = {1}", pt.X, pt.Y);
        }

        private void SP_Click(object sender, RoutedEventArgs e)
        {
            BigCanvas.Height = 5000;
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            fPrint();
        }

        private void fPrint()
        {
            PrintDialog pd = new PrintDialog();
            if ((pd.ShowDialog() == true))
            {
                pd.PrintVisual(BigCanvas as Visual, "printing as visual");
            }
        }

        private void Tool_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Save
            if (this.lbTool.SelectedIndex == 2)
            {
                fSave();
            }
            // Otworz
            else if (this.lbTool.SelectedIndex == 3)
            {
                fOpen();
            }
            // Coppy
            else if (this.lbTool.SelectedIndex == 4)
            {
                fCopy();
            }
            // Paste
            else if (this.lbTool.SelectedIndex == 5)
            {
                fPaste();    
            }
            // Drukuj
            else if (this.lbTool.SelectedIndex == 6)
            {
                fPrint();
            }
            // Czyść
            else if (this.lbTool.SelectedIndex == 8)
            {
                fClear();
            }
            // Punkty
            else if (this.lbTool.SelectedIndex == 10)
            {
                fPoint();
            }
            // Zaznacz
            else if (this.lbTool.SelectedIndex == 11)
            {
                fSelection();
            }
            // Wygładź
            else if (this.lbTool.SelectedIndex == 12)
            {
                fSmoot();
            }
        }

        #endregion
    }
}