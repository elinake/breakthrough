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

namespace Piece
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Piece : UserControl
    {
        public List<List<int>> allowedMoves = new List<List<int>>();
        public string name;

        public Piece()
        {
            InitializeComponent();
        }


        /// <summary>
        /// click piece
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void P_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClickSource();
        }

        /// <summary>
        /// own event for choosing piece
        /// </summary>
        public static readonly RoutedEvent SourceEvent =
    EventManager.RegisterRoutedEvent("Source", RoutingStrategy.Bubble,
    typeof(RoutedEventHandler), typeof(Piece));

        public event RoutedEventHandler Source
        {
            add { AddHandler(SourceEvent, value); }
            remove { RemoveHandler(SourceEvent, value); }
        }

        void ClickSource()
        {   
            RoutedEventArgs newEventArgs = new RoutedEventArgs(Piece.SourceEvent);
            RaiseEvent(newEventArgs);
        }


        public static readonly DependencyProperty ColourProperty =
         DependencyProperty.Register(
           "Colour",
           typeof(Brush),
           typeof(Piece), 
           new FrameworkPropertyMetadata(Brushes.Red,  
                FrameworkPropertyMetadataOptions.AffectsRender, 
                new PropertyChangedCallback(OnValueChanged), 
                new CoerceValueCallback(ChangeColour)));

        public Brush Colour
        {
            get { return (Brush)GetValue(ColourProperty); }
            set { SetValue(ColourProperty, value); }
        }


        private static object ChangeColour(DependencyObject element, object value)
        {
            Brush colour = (Brush)value;
            return colour;
        }


        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Piece piece = (Piece)obj;
            piece.ellipse.Fill = piece.Colour;
        }


    }
}
