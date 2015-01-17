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
using System.Windows.Shapes;

namespace Board
{
    /// <summary>
    /// Interaction logic for Win.xaml
    /// </summary>
    public partial class Win : Window
    {
        public Win()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static readonly DependencyProperty TextProperty =
 DependencyProperty.Register(
   "Text",
   typeof(string),
   typeof(Win), 
   new FrameworkPropertyMetadata("text",  
        FrameworkPropertyMetadataOptions.AffectsRender, 
        new PropertyChangedCallback(OnValueChanged),  
        new CoerceValueCallback(Muuta))); 

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static object Muuta(DependencyObject element, object value)
        {
            string luku = (string)value;

            return luku;
        }

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Win win = (Win)obj;
            win.label.Content = win.Text;
        }
    }
}
