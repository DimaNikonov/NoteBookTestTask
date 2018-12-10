using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NoteBook.Core
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action action;
        private ICommand readFromDbText;

        public Command(Action action)
        {
            this.action = action;
        }

        public Command(ICommand readFromDbText)
        {
            this.readFromDbText = readFromDbText;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.action();
        }
    }

    public class Command<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<T> action;

        public Command(Action<T> action)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.action((T)parameter);
        }
    }
}
