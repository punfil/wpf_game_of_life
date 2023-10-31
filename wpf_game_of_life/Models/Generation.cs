using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace wpf_game_of_life.Models
{
    public struct CellStats
    {
        public int deaths;
        public int births;
    }

    public class Generation : ICloneable
    {
        private readonly Cell[,] cellGrid;
        private readonly int cellGridSize;
        private CellStats cellStats;
        public int generationNumber { get; set; }

        public Generation(int cellGridSize)
        {
            this.cellGridSize = cellGridSize;
            this.cellGrid = new Cell[cellGridSize, cellGridSize];

            var generator = new Random();

            for (int i = 0; i < cellGridSize; i++)
            {
                for (int j = 0; j < cellGridSize; j++)
                {
                    this.cellGrid[i, j] = new Cell(i, j, generator.Next(2) == 0);
                }
            }

            this.generationNumber = 0;
            this.cellStats.births = 0;
            this.cellStats.deaths = 0;
        }

        public Generation(string serializedState)
        {
            string[] lines = serializedState.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            var data = lines[0].Split("#");
            this.cellGridSize = Int32.Parse(data[0]);
            this.generationNumber = Int32.Parse(data[1]);
            this.cellStats.deaths = Int32.Parse(data[2]);
            this.cellStats.births = Int32.Parse(data[3]);

            this.cellGrid = new Cell[cellGridSize, cellGridSize];

            for (int i = 1; i < lines.Length; i++)
            {
                data = lines[i].Split("|");
                this.cellGrid[Int32.Parse(data[0]), Int32.Parse(data[1])] = new Cell(Int32.Parse(data[0]), Int32.Parse(data[1]), Int32.Parse(data[2]) == 1 ? true : false);
            }
        }

        public Generation(int cellGridSize, Cell[,] cellGrid, int generationNumber, int cellDeathsNumber, int cellBirthsNumber)
        {
            this.cellGridSize = cellGridSize;
            this.cellGrid = cellGrid;
            this.generationNumber = generationNumber;
            this.cellStats.births = cellBirthsNumber;
            this.cellStats.deaths = cellDeathsNumber;
        }

        public Cell? GetCell(int row, int col)
        {
            if (row < 0 || row >= this.cellGridSize)
                return null;

            if (col < 0 || col >= this.cellGridSize)
                return null;

            return cellGrid[row, col];
        }

        public void SetCell(int row, int col, bool isAlive)
        {
            Cell? cell = GetCell(row, col);

            if (cell != null)
            {
                cell.IsAlive = isAlive;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Improper row/col combination given");
            }
        }

        public void NegateCellLife(int row, int col)
        {
            Cell? cell = GetCell(row, col);

            if (cell != null )
            {
                SetCell(row, col, !cell.IsAlive);
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{this.cellGridSize}#{this.generationNumber}#{this.cellStats.deaths}#{this.cellStats.births}");
            foreach (Cell cell in this.cellGrid)
            {
                stringBuilder.AppendLine(cell.ToString());
            }

            return stringBuilder.ToString();
        }

        public void fromString(string stringFromToString)
        {

        }

        private int CountCellNeighbors(int row, int col)
        {
            int count = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    int newX = row + i;
                    int newY = col + j;
                    if (newX >= 0 && newX < this.cellGridSize && newY >= 0 && newY < cellGridSize && cellGrid[newX, newY].IsAlive)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public object Clone()
        {
            var newcellGrid = new Cell[cellGridSize, cellGridSize];

            for (int i = 0; i < cellGridSize; i++)
            {
                for (int j = 0; j < cellGridSize; j++)
                {
                    newcellGrid[i, j] = this.cellGrid[i, j].Clone() as Cell;
                }
            }

            return new Generation(cellGridSize, newcellGrid, generationNumber, cellStats.deaths, cellStats.births);
        }

        public IList<Cell> GetEvolveChangeList()
        {
            IList<Cell> cellChangeList = new List<Cell>();

            for (int i = 0; i < cellGridSize; i++)
            {
                for (int j = 0; j < cellGridSize; j++)
                {
                    /* 
                    If a cell is ON and has fewer than two neighbors that are ON, it turns OFF
                    If a cell is ON and has either two or three neighbors that are ON, it remains ON.
                    If a cell is ON and has more than three neighbors that are ON, it turns OFF.
                    If a cell is OFF and has exactly three neighbors that are ON, it turns ON.
                    */
                    int neighbors = CountCellNeighbors(i, j);
                    bool isAlive = (cellGrid[i, j].IsAlive && (neighbors == 2 || neighbors == 3)) || (!cellGrid[i, j].IsAlive && neighbors == 3);
                    if (isAlive != cellGrid[i, j].IsAlive)
                    {
                        cellChangeList.Add(cellGrid[i, j]);
                    }
                }
            }

            return cellChangeList;
        }

        public void ApplyEvolveChangeList(IList<Cell> cellChangeList)
        {
            foreach(Cell cell in cellChangeList)
            {
                if (cell.IsAlive)
                {
                    ++cellStats.deaths;
                }
                else
                {
                    ++cellStats.births;
                }
                NegateCellLife(cell.row, cell.col);
            }
        }

        public void Reset()
        {
            generationNumber = 0;
            cellStats.births = 0;
            cellStats.deaths = 0;
        }

        public int GetCellGridSize()
        {
            return this.cellGridSize;
        }

        public CellStats GetCellStats()
        {
            return cellStats;
        }

        public void DevolveRestoreState(Generation prevGeneration)
        {
            foreach (Cell cell in prevGeneration.cellGrid)
            {
                this.cellGrid[cell.row, cell.col].IsAlive = cell.IsAlive;
            }

            this.generationNumber = prevGeneration.generationNumber;
            this.cellStats.deaths = prevGeneration.cellStats.deaths;
            this.cellStats.births = prevGeneration.cellStats.births;
        }
    }
}
