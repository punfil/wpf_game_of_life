﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using wpf_game_of_life.Models;

namespace wpf_game_of_life.Game
{
    public struct EvolveReturn
    {
        public int generationNumber;
        public int cellDeathNumber;
        public int cellBirthNumber;
    }

    public class AutoEvolveReturn : EventArgs
    {
        public EvolveReturn evolveReturn { get; set; }

        public AutoEvolveReturn(EvolveReturn evolveReturn)
        {
            this.evolveReturn = evolveReturn;
        }
    }

    public class GOLGame
    {
        private Generation currentGeneration {  get; set; }
        private IList<Generation> generationList { get; set; }

        private DispatcherTimer timer;
        public event EventHandler RequestTickData;

        public GOLGame(Generation generation)
        {
            this.currentGeneration = generation;
            this.generationList = new List<Generation>();
            this.timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += AutoEvolve;
        }

        private void AutoEvolve(object sender, EventArgs e)
        {
            this.Evolve();
        }

        private void OnRequestTickData(int generationNumber, int births, int deaths)
        {
            var val = new EvolveReturn();
            val.generationNumber = generationNumber;
            val.cellBirthNumber = births;
            val.cellDeathNumber = deaths;
            RequestTickData?.Invoke(this, new AutoEvolveReturn(val));
        }

        public EvolveReturn Evolve()
        {
            var copy = this.currentGeneration.Clone() as Generation;
            this.generationList.Add(copy);

            ++this.currentGeneration.generationNumber;

            if (currentGeneration == null)
            {
                throw new OutOfMemoryException();
            }

            var evolveChangeList = currentGeneration.GetEvolveChangeList();
            currentGeneration.ApplyEvolveChangeList(evolveChangeList);

            if (this.timer.IsEnabled)
            {
                OnRequestTickData(this.currentGeneration.generationNumber, this.currentGeneration.GetCellStats().births, this.currentGeneration.GetCellStats().deaths);
            }

            return new EvolveReturn
            {
                generationNumber = this.currentGeneration.generationNumber,
                cellBirthNumber = this.currentGeneration.GetCellStats().births,
                cellDeathNumber = this.currentGeneration.GetCellStats().deaths
            };
        }

        public EvolveReturn Deevolve()
        {
            if (generationList.Count > 0)
            { 
                var previousGeneration = generationList.Last();
                generationList.Remove(previousGeneration);
                this.currentGeneration.DevolveRestoreState(previousGeneration);
            }

            return new EvolveReturn
            {
                generationNumber = this.currentGeneration.generationNumber,
                cellBirthNumber = this.currentGeneration.GetCellStats().births,
                cellDeathNumber = this.currentGeneration.GetCellStats().deaths
            };
        }

        public void AutoEvolve()
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
            }
            else
            {
                timer.Start();
            }
        }

        public string? LoadState()
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
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    filecontent = reader.ReadToEnd();
                }
            }
            return filecontent;
        }

        public void SaveState()
        {
            string? filename = null;

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "game_of_life_state";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                filename = dlg.FileName;
                System.IO.File.WriteAllText(filename, this.currentGeneration.ToString());
            }
        }

        public EvolveReturn Reset()
        {
            generationList.Clear();
            currentGeneration.Reset();

            return new EvolveReturn
            {
                generationNumber = this.currentGeneration.generationNumber,
                cellBirthNumber = this.currentGeneration.GetCellStats().births,
                cellDeathNumber = this.currentGeneration.GetCellStats().deaths
            };
        }

        public int GetCellGridSize()
        {
            return this.currentGeneration.GetCellGridSize();
        }

        public int GetCurrentGenerationNumber()
        {
            return this.currentGeneration.generationNumber;
        }

        public Cell? GetCell(int row, int col)
        {
            return this.currentGeneration.GetCell(row, col);
        }

        public void NegateCellLife(int row, int col)
        {
            this.currentGeneration.NegateCellLife(row, col);
        }

        public CellStats GetCellStats()
        {
            return this.currentGeneration.GetCellStats();
        }
    }
}
