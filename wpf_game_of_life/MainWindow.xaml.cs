using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using wpf_game_of_life.Models;
using wpf_game_of_life.ViewModels;
using wpf_game_of_life.Game;

namespace wpf_game_of_life
{
    public partial class MainWindow : Window
    {
       private GenerationViewModel generationViewModel;

        public MainWindow(int cellgridSize)
        {
            InitializeComponent();

            generationViewModel = new GenerationViewModel(cellgridSize);

            DrawUI(generationViewModel);

            DataContext = generationViewModel;
        }

        private void DrawUI(GenerationViewModel generationViewModel)
        {
            gameGrid.VerticalAlignment = VerticalAlignment.Top;
            gameGrid.HorizontalAlignment = HorizontalAlignment.Center;
            gameGrid.Background = new SolidColorBrush(Colors.LightBlue);

            // Create Columns
            for (int i = 0; i < generationViewModel.cellGridSize; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                gameGrid.ColumnDefinitions.Add(columnDefinition);
            }

            // Create rows
            for (int i = 0; i < generationViewModel.cellGridSize; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                gameGrid.RowDefinitions.Add(rowDefinition);
            }

            for (int i = 0;i < generationViewModel.cellGridSize; i++)
            {
                for (int j = 0;j < generationViewModel.cellGridSize;j++)
                {
                    var rectangleWithBinding = CreateRectangleWithBinding(generationViewModel.GetCell(i, j));

                    Grid.SetColumn(rectangleWithBinding, j);
                    Grid.SetRow(rectangleWithBinding, i);

                    gameGrid.Children.Add(rectangleWithBinding);
                }
            }

            gameGrid.UpdateLayout();
        }

        private Rectangle CreateRectangleWithBinding(Cell cell)
        {
            Rectangle cellRectangle = new Rectangle();
            cellRectangle.DataContext = cell;
            cellRectangle.InputBindings.Add(CreateMouseClickInputBinding(cell));
            cellRectangle.Stroke = Brushes.Gray;
            cellRectangle.StrokeThickness = 0.5;
            cellRectangle.Width = 10;
            cellRectangle.Height = 10;
            cellRectangle.SetBinding(Rectangle.FillProperty, CreateCellAliveBinding());

            return cellRectangle;
        }

        private InputBinding CreateMouseClickInputBinding(Cell cell)
        {
            InputBinding cellTextBlockInputBinding = new InputBinding(
                generationViewModel.NegateCellLifeCommand,
                new MouseGesture(MouseAction.LeftClick)
            );
            cellTextBlockInputBinding.CommandParameter = $"{cell.row}|{cell.col}";

            return cellTextBlockInputBinding;
        }

        private Binding CreateCellAliveBinding()
        {
            return new Binding
            {
                Path = new PropertyPath("IsAlive"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = new CellHealthColorConventer(
                    aliveColour: Brushes.Black,
                    deadColour: Brushes.White
                )
            };
        }
    }
}
