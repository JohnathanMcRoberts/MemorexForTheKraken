using System;
using System.Windows.Input;

namespace MongoDbBooks.ViewModels.Utilities
{
    public class CommandHandler : ICommand
    {
        private Action _action;
        private readonly bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;

            var a = CanExecuteChanged;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
