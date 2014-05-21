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

namespace DragAndDropApp
{
    /// <summary>
    /// Interaction logic for showit.xaml
    /// </summary>
    public partial class showit : UserControl
    {

        DrawingAttributes theDrawingAttributes;

        public showit()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Completes initialization after all XAML member vars have been initialized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            theDrawingAttributes = new DrawingAttributes();
            Click.Click += new RoutedEventHandler(Click_Click);            
        }

        void Click_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("sdfghgfd");
        }
    }
}
