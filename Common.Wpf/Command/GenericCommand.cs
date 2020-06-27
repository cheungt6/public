using System;

namespace Common.Wpf.Command
{
    public class GenericCommand : CommandBase
    {
        private readonly Action _commandAction;
        private readonly Func<object, bool> _canExecute;

        public GenericCommand(Action commandAction)
        {
            _commandAction = commandAction;
        }

        public GenericCommand(Action commandAction, Func<object, bool> canExecute) : this(commandAction)
        {
            _canExecute = canExecute;
        }

        public override void Execute(object parameter)
        {
            _commandAction.Invoke();
        }

        public override bool CanExecute(object obj)
        {
            var result = ((_canExecute == null) ||
                (_canExecute != null && _canExecute(obj))) &&

                base.CanExecute(obj);

            return result;
        }
    }
}
