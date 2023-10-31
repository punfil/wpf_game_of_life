using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using wpf_game_of_life.Game;
using wpf_game_of_life.Models;

namespace wpf_game_of_life.ViewModels
{
    public class GenerationViewModel : NotifyPropertyChanged
    {
        private readonly GOLGame golgame;
        public event EventHandler RequestClose;

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

        private void OnRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
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
            Setup();
        }

        public GenerationViewModel(string serializedState)
        {
            golgame = new GOLGame(new Generation(serializedState));
            Setup();
        }

        public void Setup()
        {
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
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FilterIndex = 2;
            dlg.RestoreDirectory = true;

            Nullable<bool> result = dlg.ShowDialog();

            string? filecontent = null;

            if (result == true)
            {
                string filePath = dlg.FileName;
                var fileStream = dlg.OpenFile();
                using (StreamReader reader  = new StreamReader(fileStream))
                {
                    filecontent = reader.ReadToEnd();
                }

                var newGame = new MainWindow(filecontent);
                OnRequestClose();
                newGame.Show();
            }
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
