using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace wpf_game_of_life.Game
{
    public class Command<T> : ICommand
    {
        private readonly Action<T> execute = null;
        private readonly Predicate<T> canExecute = null;
        public event EventHandler? CanExecuteChanged = null;

        public Command(Action<T> execute, Predicate<T> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute == null ? true : canExecute((T)parameter);
        }

        public void Execute(object? parameter)
        {
            execute((T)parameter);
        }
    }
}
