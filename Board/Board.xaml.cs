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

namespace Board
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Board : UserControl
    {

        int piecesWhite = 0;
        int piecesBlack = 0;
        Piece.Piece selectedPiece = null;

        Piece.Piece turn = new Piece.Piece();


        /// <summary>
        /// initializing
        /// </summary>
        public Board()
        {
            InitializeComponent();
            createPieces();
            turn.Colour = Brushes.Black;
        }

        /// <summary>
        /// check if player is allowed to eat the selected enemy and if it is, eat it
        /// </summary>
        /// <param name="selectedEnemy"></param>
        private void checkEatable(Piece.Piece selectedEnemy)
        {              
            if (selectedPiece.Colour != selectedEnemy.Colour)
            {
                bool allowed = false;

                for (int i = 0; i < selectedPiece.allowedMoves.Count; i++)
                {
                    if (selectedPiece.allowedMoves[i][0] == Grid.GetRow((UIElement)selectedEnemy) && selectedPiece.allowedMoves[i][1] == Grid.GetColumn((UIElement)selectedEnemy))
                        allowed = true;
                }
                if (allowed)
                {
                    eatIt(selectedPiece, selectedEnemy);
                    move(new List<int> { Grid.GetRow((UIElement)selectedEnemy), Grid.GetColumn((UIElement)selectedEnemy) });
                }

                else //move not allowed
                {
                    selectedPiece.Background = Brushes.Transparent;
                    highlight(selectedPiece.allowedMoves, false);
                    selectedPiece = null;
                }

            }
            else //target piece was not enemy
            {
                selectedPiece.Background = Brushes.Transparent;
                highlight(selectedPiece.allowedMoves, false);
                selectedPiece = null;
            }
        }

        /// <summary>
        /// select a piece for moving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectPiece(object sender, RoutedEventArgs e)
        {
            if (selectedPiece != null) checkEatable((Piece.Piece)e.Source); //eating someone

            else
            {
                Piece.Piece selected = (Piece.Piece)e.Source;
                if (selected.Colour != turn.Colour) return; //cannot choose wrong team

                selectedPiece = selected;

                List<int> selectedLocation = new List<int> { Grid.GetRow((UIElement)e.Source), Grid.GetColumn((UIElement)e.Source) };

                int up = -1;
                int down = 1;

                //mustat menee ylöspäin
                if (selectedPiece.Colour == Brushes.Black) selectedPiece.allowedMoves = checkAllowed(selectedLocation, e, up);
                else selectedPiece.allowedMoves = checkAllowed(selectedLocation, e, down);

                if (selected.allowedMoves.Count == 0) //if no moves, remove selection
                {
                    selected.Background = Brushes.Transparent;
                    selectedPiece = null;
                } 

                else if (selected.allowedMoves.Count == 1) 
                {
                    Piece.Piece enemy = checkTarget(selected.allowedMoves[0]);
                    if (enemy != null) checkEatable(enemy);

                    else move(selected.allowedMoves[0]);
                }
                else //wait for target click
                {
                    selected.Background = Brushes.Yellow;              
                    highlight(selected.allowedMoves, true);
                }
            }
        }

        /// <summary>
        /// set new row and column for moving piece, check won
        /// switch the turn
        /// disable highlight & selection
        /// </summary>
        /// <param name="list"></param>
        private void move(List<int> list)
        {
            selectedPiece.SetValue(Grid.RowProperty, list[0]);
            selectedPiece.SetValue(Grid.ColumnProperty, list[1]);

            if (turn.Colour == Brushes.Black) turn.Colour = Brushes.White; //switch the turn
            else turn.Colour = Brushes.Black;

            highlight(selectedPiece.allowedMoves, false);
            selectedPiece.Background = Brushes.Transparent;

            if (selectedPiece.Colour == Brushes.Black && Grid.GetRow(selectedPiece) == 0) win(selectedPiece);
            else if (selectedPiece.Colour == Brushes.White && Grid.GetRow(selectedPiece) == Size - 1) win(selectedPiece);

            selectedPiece = null;
 
        }

        /// <summary>
        /// target cells highlighting
        /// </summary>
        /// <param name="list">targets for highlight</param>
        /// <param name="showAllowed">enable or disable hightlighting</param>
        private void highlight(List<List<int>> list, bool showAllowed)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var element = Board2.Children.Cast<UIElement>().FirstOrDefault(e => Grid.GetColumn(e) == list[i][1] && Grid.GetRow(e) == list[i][0]);
                var item = (Border)element;
                
                if (showAllowed == true) item.Background = Brushes.Yellow;
                else item.Background = Brushes.Transparent;
            }            
            
        }


        /// <summary>
        /// winning & disabling mouse clicks
        /// </summary>
        /// <param name="p">piece which won</param>
        private void win(Piece.Piece p)
        {
            Board2.IsHitTestVisible = false;
            Board2.Background = Brushes.Gray;
            
            string text;
            if (p.Colour == Brushes.Black) text = "Black player won!";
            else text = "White player won!";

            Win win = new Win();
            win.Text = text;
            win.ShowDialog();
 
        }

        /// <summary>
        /// check which moves are allowed
        /// </summary>
        /// <param name="location">where piece is now</param>
        /// <param name="e"></param>
        /// <param name="direction">up=-1 or down=1</param>
        /// <returns>all allowed moves</returns>
        private List<List<int>> checkAllowed(List<int> location, RoutedEventArgs e, int direction)
        {
            List<List<int>> allowedMoves = new List<List<int>>();
            
            //up and down, no need check winning because they are checked at the end of every move
            List<int> straight = new List<int> { location[0] + direction, location[1] };
            if (checkTarget(straight) == null) allowedMoves.Add(straight); //cannot eat direct
            
            //vasen
            if (location[1] <= 0) ; //if left edge, go to check right move
            else {
                List<int> left = new List<int>{location[0] + direction, location[1] - 1};
                
                //onko varattu
                if (checkTarget(left) == null) allowedMoves.Add(left);
                else
                {//onko syötävä
                    Piece.Piece player = checkTarget(location); 
                    Piece.Piece enemy = checkTarget(left);
                    
                    if (player.Colour != enemy.Colour) allowedMoves.Add(left);
                }
            }

            //right
            if (location[1] >= Size - 1) ;
            else
            {
                List<int> right = new List<int> { location[0] + direction, location[1] + 1 };
                
                if (checkTarget(right) == null)
                    allowedMoves.Add(right);
                else
                {
                    Piece.Piece player = checkTarget(location);
                    Piece.Piece enemy = checkTarget(right);

                    if (player.Colour != enemy.Colour) allowedMoves.Add(right);
                }
            }
            return allowedMoves;
        }

        /// <summary>
        /// choose a target cell for selected piece
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedPiece == null) return;

            else
            {
                var cell = sender as Border; 
                List<int> location = new List<int>() { Grid.GetRow((UIElement)cell), Grid.GetColumn((UIElement)cell) };
                cell.Background = Brushes.Transparent;

                bool cellIsAllowed = false;

                for (int i = 0; i < selectedPiece.allowedMoves.Count; i++)
                {
                    if (selectedPiece.allowedMoves[i][0] == location[0] && selectedPiece.allowedMoves[i][1] == location[1])
                        cellIsAllowed = true;
                }
   
                if (cellIsAllowed) move(location);
                else
                {
                    selectedPiece.Background = Brushes.Transparent;
                    highlight(selectedPiece.allowedMoves, false);
                    selectedPiece = null;
                }
            }
        }

        /// <summary>
        /// player eats enemy, check if all enemy pieces are gone
        /// </summary>
        /// <param name="player">selected piece</param>
        /// <param name="enemy">piece in a target cell</param>
        private void eatIt(Piece.Piece player, Piece.Piece enemy)
        {
            Board2.Children.Remove(enemy);
            if (enemy.Colour == Brushes.Black)
            {
                piecesBlack--;
                if (piecesBlack == 0) win(player);
            }
            else
            {
                piecesWhite--;
                if (piecesWhite == 0) win(player);
            }
        }

        /// <summary>
        /// if there is a piece in a target cell, if null, player knows there is a free cell
        /// </summary>
        /// <param name="location">location of target</param>
        /// <returns>piece in a target cell or null</returns>
        private Piece.Piece checkTarget(List<int> location)
        {
            //there is both border and piece in the cell
            var elements = Board2.Children.Cast<UIElement>().Where(e => Grid.GetColumn(e) == location[1] && Grid.GetRow(e) == location[0]);
            foreach (var item in elements)
            {
                if (item.GetType() == typeof(Piece.Piece)) return (Piece.Piece)item;
            }
            return null;

        }

   
         /// <summary>
        /// creating pieces and cells
        /// </summary>
        public void createPieces()
        {
            Board2.Children.Clear();
            piecesBlack = 0;
            piecesWhite = 0;

            //have to create border to every cell to make them clickable
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    var cell = new Border();
                    cell.MouseDown += new MouseButtonEventHandler(Cell_MouseDown);
                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, j);
                    cell.Background = Brushes.Transparent;

                    cell.BorderBrush = Brushes.Black;
                    cell.BorderThickness = new Thickness(1);
                    Board2.Children.Add(cell);
                }
            }

            for (int i = Size-1; i >= 0; i--)
            {
                Piece.Piece p1 = new Piece.Piece();
                p1.Colour = Brushes.Black;
                p1.SetValue(Grid.RowProperty, Size - 1);
                p1.SetValue(Grid.ColumnProperty, i);
                p1.name = "Black";
                piecesBlack++;
                Board2.Children.Add(p1);

                Piece.Piece p2 = new Piece.Piece();
                p2.Colour = Brushes.Black;
                p2.SetValue(Grid.RowProperty, Size - 2);
                p2.SetValue(Grid.ColumnProperty, i);
                p1.name = "Black";
                piecesBlack++;
                Board2.Children.Add(p2);

                Piece.Piece p3 = new Piece.Piece();
                p3.Colour = Brushes.White;
                p3.SetValue(Grid.RowProperty, Size - Size + 1);
                p3.SetValue(Grid.ColumnProperty, i);
                p1.name = "White";
                piecesWhite++;
                Board2.Children.Add(p3);

                Piece.Piece p4 = new Piece.Piece();
                p4.Colour = Brushes.White;
                p4.SetValue(Grid.RowProperty, Size - Size);
                p4.SetValue(Grid.ColumnProperty, i);
                p1.name = "White";
                piecesWhite++;
                Board2.Children.Add(p4);
                
            }
        }
            
        public static readonly DependencyProperty SizeProperty =
       DependencyProperty.Register(
         "Size",
         typeof(int), 
         typeof(Board), 
         new FrameworkPropertyMetadata(0,  
              FrameworkPropertyMetadataOptions.AffectsRender,
              new PropertyChangedCallback(OnValueChanged),  
              new CoerceValueCallback(ChangeSize))); 

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        private static object ChangeSize(DependencyObject element, object value)
        {
            int nr = (int)value;
            return nr;
        }

        /// <summary>
        /// create board
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Board b = (Board)obj;
            
            for (int i = 0; i < b.Size; i++)
            {
                ColumnDefinition gridCol1 = new ColumnDefinition();
                b.Board2.ColumnDefinitions.Add(gridCol1);
                RowDefinition gridRow1 = new RowDefinition();
                b.Board2.RowDefinitions.Add(gridRow1);

             
            }
   
        }
       
       
    }
}
