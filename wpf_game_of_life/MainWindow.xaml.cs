using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace wpf_game_of_life
{
    public partial class MainWindow : Window
    {
        private int gridSize = 40;
        private bool[,] grid;
        private bool[,] newGrid;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            grid = new bool[gridSize, gridSize];
            newGrid = new bool[gridSize, gridSize];
            InitializeWPFGrid();
            GenerateRandomGrid();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += GameLoop;
            timer.Start();
        }

        /// <summary>
        /// Main loop of the game.
        /// </summary>
        private void GameLoop(object sender, EventArgs e)
        {
            UpdateWPFGrid();
            DrawWPFGrid();
            UpdateGrid();
        }

        /// <summary>
        /// Initializes WPF Grid variables. Generates Columns and Rows.
        /// </summary>
        private void InitializeWPFGrid()
        {
            gameGrid.VerticalAlignment = VerticalAlignment.Top;
            gameGrid.HorizontalAlignment = HorizontalAlignment.Center;
            gameGrid.Background = new SolidColorBrush(Colors.LightBlue);

            // Create Columns
            for (int i = 0; i < gridSize; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                gameGrid.ColumnDefinitions.Add(columnDefinition);
            }

            // Create rows
            for (int i = 0; i < gridSize; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                gameGrid.RowDefinitions.Add(rowDefinition);
            }
        }

        /// <summary>
        /// Generates a random grid. 50% ON 50% OFF
        /// </summary>
        private void GenerateRandomGrid()
        {
            Random rand = new Random();
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    grid[i, j] = rand.Next(3) == 0;
                }
            }
        }

        /// <summary>
        /// Updates View Grid 
        /// </summary>
        private void UpdateWPFGrid()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Rectangle? cell = null;
                    if (gameGrid.Children.Count > i * gridSize + j && gameGrid.Children[i * gridSize + j] is Rectangle existingCell)
                    {
                    } else
                    {
                        cell = new Rectangle
                        {
                            Width = 10,
                            Height = 10,
                            Fill = grid[i, j] ? Brushes.Black : Brushes.White,
                            Stroke = Brushes.Gray,
                            StrokeThickness = 0.5
                        };

                        Grid.SetColumn(cell, j);
                        Grid.SetRow(cell, i);
                        gameGrid.Children.Add(cell);
                    }
                }
            }
        }

        /// <summary>
        /// Implements Logic of the Game. Updates internal grid according to rules of Game Of Life.
        /// </summary>
        private void UpdateGrid()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    /* 
                    If a cell is ON and has fewer than two neighbors that are ON, it turns OFF
                    If a cell is ON and has either two or three neighbors that are ON, it remains ON.
                    If a cell is ON and has more than three neighbors that are ON, it turns OFF.
                    If a cell is OFF and has exactly three neighbors that are ON, it turns ON.
                    */
                    int neighbors = CountNeighbors(i, j);
                    newGrid[i, j] = (grid[i, j] && (neighbors == 2 || neighbors == 3)) || (!grid[i, j] && neighbors == 3);
                }
            }
            Array.Copy(newGrid, grid, gridSize * gridSize);
        }

        /// <summary>
        /// Counts number of neighbors for a given cell.
        /// </summary>
        private int CountNeighbors(int x, int y)
        {
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    int newX = x + i;
                    int newY = y + j;
                    if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize && grid[newX, newY])
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Updates the grid which is displayed on the user screen.
        /// </summary>
        private void DrawWPFGrid()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    var cell = gameGrid.Children[i * gridSize + j] as Rectangle;
                    if (cell != null)
                    {
                        cell.Fill = grid[i, j] ? Brushes.Black : Brushes.White;
                    }
                }
            }

            gameGrid.UpdateLayout();
        }
    }
}
