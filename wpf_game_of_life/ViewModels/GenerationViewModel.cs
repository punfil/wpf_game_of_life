using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using wpf_game_of_life.Game;
using wpf_game_of_life.Models;

namespace wpf_game_of_life.ViewModels
{
    public class GenerationViewModel : NotifyPropertyChanged
    {
        private readonly GOLGame golgame;

        public int cellGridSize { get { return golgame.GetCellGridSize(); } }

        private int generationNumber;
        public int GenerationNumber
        {
            get { return generationNumber; }
            private set
            {
                generationNumber = value;
                OnPropertyChanged();
            }
        }

        public int CellDeathsNumber
        {
            get { return golgame.GetCellStats().deaths;  }
            private set
            {
                OnPropertyChanged();
            }
        }

        public int CellBirthsNumber
        {
            get { return golgame.GetCellStats().births;  }
            private set
            {
                OnPropertyChanged();
            }
        }



        public Command<object> EvolveCommand { get; private set; }
        public Command<object> DeEvolveCommand { get; private set; }
        public Command<object> AutoEvolveCommand { get; private set; }
        public Command<object> LoadStateCommand { get; private set; }
        public Command<object> SaveStateCommand { get; private set; }
        public Command<object> ResetCommand { get; private set; }
        public Command<string> NegateCellLifeCommand { get; private set; }


        public GenerationViewModel(int cellGridSize)
        {
            golgame = new GOLGame(new Generation(cellGridSize));

            EvolveCommand = new Command<object>(
                _ => EvolveGeneration(),
                _ => ReturnTrue()
            );

            DeEvolveCommand = new Command<object>(
                _ => DevolveGeneration(),
                _ => ReturnTrue()
            );

            AutoEvolveCommand = new Command<object>(
                _ => AutoEvolveGeneration(),
                _ => ReturnTrue()
            );

            LoadStateCommand = new Command<object>(
                _ => LoadState(),
                _ => ReturnTrue()
            );

            SaveStateCommand = new Command<object>(
                _ => SaveState(),
                _ => ReturnTrue()
            );

            ResetCommand = new Command<object>(
                _ => ResetGame(),
                _ => ReturnTrue()
            );

            NegateCellLifeCommand = new Command<string>(
                (cellRowColumn) => NegateCellLife(cellRowColumn),
                _ => ReturnTrue()
            );

            GenerationNumber = golgame.GetCurrentGenerationNumber();
        }

        public Cell? GetCell(int row, int column)
        {
            return golgame.GetCell(row, column);
        }

        private void EvolveGeneration()
        {
            EvolveReturn evolveReturn = golgame.Evolve();

            GenerationNumber = evolveReturn.generationNumber;
            CellDeathsNumber = evolveReturn.cellDeathNumber;
            CellBirthsNumber = evolveReturn.cellBirthNumber;
        }

        private void DevolveGeneration()
        {
            EvolveReturn evolveReturn = golgame.Deevolve();

            GenerationNumber = evolveReturn.generationNumber;
            CellDeathsNumber = evolveReturn.cellDeathNumber;
            CellBirthsNumber = evolveReturn.cellBirthNumber;
        }

        private void AutoEvolveGeneration()
        {
            golgame.AutoEvolve();
        }

        private void LoadState()
        {
            golgame.LoadState();
        }

        private void SaveState()
        {
            golgame.SaveState();
        }

        private void ResetGame()
        {
            EvolveReturn evolveReturn = golgame.Reset();

            GenerationNumber = evolveReturn.generationNumber;
            CellDeathsNumber = evolveReturn.cellDeathNumber;
            CellBirthsNumber = evolveReturn.cellBirthNumber;
        }

        private bool ReturnTrue()
        {
            return true;
        }

        private void NegateCellLife(string cellRowColumn)
        {
            string[] cellRowColumnSplit = cellRowColumn.Split('|');

            int row = Int32.Parse(cellRowColumnSplit[0]);
            int column = Int32.Parse(cellRowColumnSplit[1]);

            golgame.NegateCellLife(row, column);
        }
    }
}
