using System;
using System.Collections.Generic;
using System.Globalization;
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

    public class GOLGame
    {
        private Generation currentGeneration {  get; set; }
        private IList<Generation> generationList { get; set; }

        private DispatcherTimer timer;

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

        public void LoadState()
        {
            
        }

        public void SaveState()
        {

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
