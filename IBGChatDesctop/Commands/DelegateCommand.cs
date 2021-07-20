using System;
using System.Windows.Input;

namespace IBGChatDesctop.Commands
{
    public class DelegateCommand : ICommand
    {
        #region fields
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;
        #endregion

        #region events
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        #endregion

        #region constructor
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        #endregion

        #region public methods
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
        #endregion
    }
}
