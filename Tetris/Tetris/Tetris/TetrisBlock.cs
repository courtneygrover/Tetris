using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tetris
{
    internal class TetrisBlock
    {
        /* Attributes */
        private int[,] gameBoard;
        private Brush[] rgb = { Brushes.Black, Brushes.Coral, Brushes.LightSeaGreen, Brushes.DeepSkyBlue, Brushes.RoyalBlue, Brushes.Aquamarine, Brushes.Wheat, Brushes.PaleGreen };
        private Canvas gameBoardView;
        private int shapeIndex;
        private int rotationIndex;
        private Rectangle[] prevRects;
        private int[,,] offsets = {{{0, 1, 0, 2},{2, 0, 1, 0},{0, 2, 0, 1},{1, 0, 2, 0}},   // line
                                   {{0, 0, 1, 2},{1, 0, 1, 1},{0, 1, 1, 1},{0, 0, 2, 1}},   // L left
                                   {{0, 0, 1, 2},{1, 0, 1, 1},{0, 1, 1, 1},{0, 0, 2, 1}},   // L right
                                   {{0, 0, 1, 2},{1, 0, 1, 1},{0, 1, 1, 1},{0, 0, 2, 1}},   // zag top right
                                   {{0, 0, 1, 2},{1, 0, 1, 1},{0, 1, 1, 1},{0, 0, 2, 1}},   // zag top left
                                   {{1, 1, 1, 1},{1, 1, 1, 1},{1, 1, 1, 1},{1, 1, 1, 1}},   // square
                                   {{0, 0, 1, 2},{1, 0, 1, 1},{0, 1, 1, 1},{0, 0, 2, 1}}};  // T
        private int[,,,] blocks = {{{{0, 0, 0, 0},{1, 1, 1, 1},{0, 0, 0, 0},{0, 0, 0, 0}},{{0, 0, 1, 0},{0, 0, 1, 0},{0, 0, 1, 0},{0, 0, 1, 0}},{{0, 0, 0, 0},{0, 0, 0, 0},{1, 1, 1, 1},{0, 0, 0, 0}},{{0, 1, 0, 0},{0, 1, 0, 0},{0, 1, 0, 0},{0, 1, 0, 0}}},	// line
						           {{{2, 0, 0, 0},{2, 2, 2, 0},{0, 0, 0, 0},{0, 0, 0, 0}},{{0, 2, 2, 0},{0, 2, 0, 0},{0, 2, 0, 0},{0, 0, 0, 0}},{{0, 0, 0, 0},{2, 2, 2, 0},{0, 0, 2, 0},{0, 0, 0, 0}},{{0, 2, 0, 0},{0, 2, 0, 0},{2, 2, 0, 0},{0, 0, 0, 0}}},	// L left
						           {{{0, 0, 3, 0},{3, 3, 3, 0},{0, 0, 0, 0},{0, 0, 0, 0}},{{0, 3, 0, 0},{0, 3, 0, 0},{0, 3, 3, 0},{0, 0, 0, 0}},{{0, 0, 0, 0},{3, 3, 3, 0},{3, 0, 0, 0},{0, 0, 0, 0}},{{3, 3, 0, 0},{0, 3, 0, 0},{0, 3, 0, 0},{0, 0, 0, 0}}},	// L right
						           {{{0, 4, 4, 0},{4, 4, 0, 0},{0, 0, 0, 0},{0, 0, 0, 0}},{{0, 4, 0, 0},{0, 4, 4, 0},{0, 0, 4, 0},{0, 0, 0, 0}},{{0, 0, 0, 0},{0, 4, 4, 0},{4, 4, 0, 0},{0, 0, 0, 0}},{{4, 0, 0, 0},{4, 4, 0, 0},{0, 4, 0, 0},{0, 0, 0, 0}}}, 	// zag top right
						           {{{5, 5, 0, 0},{0, 5, 5, 0},{0, 0, 0, 0},{0, 0, 0, 0}},{{0, 0, 5, 0},{0, 5, 5, 0},{0, 5, 0, 0},{0, 0, 0, 0}},{{0, 0, 0, 0},{5, 5, 0, 0},{0, 5, 5, 0},{0, 0, 0, 0}},{{0, 5, 0, 0},{5, 5, 0, 0},{5, 0, 0, 0},{0, 0, 0, 0}}},	// zag top left
						           {{{0, 0, 0, 0},{0, 6, 6, 0},{0, 6, 6, 0},{0, 0, 0, 0}},{{0, 0, 0, 0},{0, 6, 6, 0},{0, 6, 6, 0},{0, 0, 0, 0}},{{0, 0, 0, 0},{0, 6, 6, 0},{0, 6, 6, 0},{0, 0, 0, 0}},{{0, 0, 0, 0},{0, 6, 6, 0},{0, 6, 6, 0},{0, 0, 0, 0}}},	// square
						           {{{0, 7, 0, 0},{7, 7, 7, 0},{0, 0, 0, 0},{0, 0, 0, 0}},{{0, 7, 0, 0},{0, 7, 7, 0},{0, 7, 0, 0},{0, 0, 0, 0}},{{0, 0, 0, 0},{7, 7, 7, 0},{0, 7, 0, 0},{0, 0, 0, 0}},{{0, 7, 0, 0},{7, 7, 0, 0},{0, 7, 0, 0},{0, 0, 0, 0}}}};	// T

        /* Properties */
        public Canvas SetCanvas { set { gameBoardView = value; } }

        /* Constructors */
        public TetrisBlock(int[,] gameBoard, Canvas gameBoardView, int shapeIndex, int rotationIndex)
        {
            this.gameBoard = gameBoard;
            this.gameBoardView = gameBoardView;
            this.shapeIndex = shapeIndex;
            this.rotationIndex = rotationIndex;

            prevRects = new Rectangle[4];
        }

        /* Methods */
        // Draws a block at the given row
        public void DrawBlock(int rowIndex, int colIndex)
        {
            RemovePreviousBlock();

            int count = 0;

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (blocks[shapeIndex, rotationIndex, y, x] != 0)
                    {
                        Rectangle r = new Rectangle();
                        r.Width = 35;
                        r.Height = 35;

                        r.Fill = rgb[blocks[shapeIndex, rotationIndex, y, x]];
                        r.StrokeThickness = 2;
                        r.Stroke = Brushes.Black;

                        Canvas.SetTop(r, (rowIndex * 35) + (y * 35));
                        Canvas.SetLeft(r, (colIndex * 35) + (x * 35));

                        gameBoardView.Children.Add(r);
                        prevRects[count] = r;
                        count++;
                    }
                }
            }

            
        }

        private void RemovePreviousBlock()
        {
            foreach(Rectangle r in prevRects)
            {
                gameBoardView.Children.Remove(r);
            }
        }

        public int Occupied(int y, int x)
        {
            return blocks[shapeIndex, rotationIndex, y, x];
        }

        public bool ShiftBlockRightLeft(int rowIndex, int colIndex)
        {
            int leftOffset = offsets[shapeIndex, rotationIndex, 0];
            int rightOffset = offsets[shapeIndex, rotationIndex, 2];

            // check left boundary
            if (colIndex < 0)
            {
                if (leftOffset == 1)
                {
                    if (colIndex < -1)
                        return false;
                }

                else if (leftOffset == 2)
                {
                    if (colIndex < -2)
                        return false;
                }

                else
                    return false;
            }

            // check right boundary
            if (colIndex > 6)
            {
                if(leftOffset == 0)
                {
                    if (rightOffset == 1)
                    {
                        if (colIndex > 7)
                            return false;
                    }
                    else if(rightOffset == 2)
                    {
                        if (colIndex > 8)
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }

                else if(leftOffset == 1)
                {
                    if (rightOffset == 1)
                    {
                        if (colIndex > 7)
                            return false;
                    }
                    else if(rightOffset == 2)
                    {
                        if (colIndex > 8)
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }

                else
                {
                    if (rightOffset == 1)
                    {
                        if (colIndex > 7)
                            return false;
                    }
                    else if (rightOffset == 2)
                    {
                        if (colIndex > 8)
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // check for other blocks
            for(int y = 0; y < 4; y++)
            {
                for(int x = 0; x < 4; x++)
                {
                    try
                    {
                        if (blocks[shapeIndex, rotationIndex, y, x] != 0)
                            if (gameBoard[rowIndex + y, colIndex + x] != 0)
                                return false;
                    }
                    catch(IndexOutOfRangeException e)
                    {
                        // EMPTY CATCH I'M SO SORRY
                    }
                }
            }

            DrawBlock(rowIndex, colIndex);
            return true;
        }

        public bool ShiftBlockDown(int rowIndex, int colIndex)
        {
            int buttOffset = offsets[shapeIndex, rotationIndex, 3];

            // check if at bottom of board
            if(buttOffset == 0)
            {
                if (rowIndex > 14)
                    return false;
            }
            else if(buttOffset == 1)
            {
                if (rowIndex > 15)
                    return false;
            }
            else
            {
                if (rowIndex > 16)
                    return false;
            }


            // check if running into blocks
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    try
                    {
                        if (blocks[shapeIndex, rotationIndex, y, x] != 0)
                            if (gameBoard[rowIndex + y, colIndex + x] != 0)
                                return false;
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        // EMPTY CATCH I'M SO SORRY
                    }
                }
            }

            DrawBlock(rowIndex, colIndex);
            return true;
        }

        public bool RotateBlock(int rotationIndex, int rowIndex, int colIndex)
        {
            // check block radius
            int radius = (shapeIndex == 0 || shapeIndex == 5) ? 4 : 3;

            for(int i = 0; i < radius; i++)
            {
                for(int k = 0; k < radius; k++)
                {
                    try
                    {
                        if (gameBoard[i + rowIndex, k + colIndex] != 0)
                            return false;
                    }
                    catch(IndexOutOfRangeException e)
                    {
                        return false;
                    }
                }
            }

            this.rotationIndex = rotationIndex;
            DrawBlock(rowIndex, colIndex);
            return true;
        }
    }
}