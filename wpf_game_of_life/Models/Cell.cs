using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpf_game_of_life.Game;

namespace wpf_game_of_life.Models
{
    public class Cell : NotifyPropertyChanged, ICloneable
    {
        public int row { get; set; }
        public int col { get; set; }

        private bool isAlive;
        public bool IsAlive
        {
            get { return isAlive; }
            set
            {
                isAlive = value;
                OnPropertyChanged();
            }
        }
 

        public Cell(int row, int column, bool isAlive)
        {
            this.row = row;
            this.col = column;
            this.isAlive = isAlive;
        }

        public override string ToString()
        {
            var val = isAlive ? 1 : 0;
            return $"{row}|{col}|{val}";
        }

        public object Clone()
        {
            return new Cell(row, col, isAlive);
        }
    }
}
