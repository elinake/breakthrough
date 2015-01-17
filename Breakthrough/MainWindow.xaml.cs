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

namespace Breakthrough
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        /// <summary>
        /// new game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
               MenuItem m = (MenuItem)sender;
               grid.Children.Clear();
               Board.Board board1 = new Board.Board();

                string teksti = m.Header.ToString();

                board1.Size = (int)Char.GetNumericValue(teksti[0]);
                grid.Children.Add(board1);
                board1.createPieces();
        }

        /// <summary>
        /// about dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }

        /// <summary>
        /// information about breakthrough
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Info_Click(object sender, RoutedEventArgs e)
        {
            Info info = new Info();
            info.ShowDialog();
        }

    }
}
