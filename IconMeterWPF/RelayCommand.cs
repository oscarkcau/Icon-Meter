using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IconMeterWPF
{
	class RelayCommand : ICommand
	{
		// Common relay command class for instantiating ICommand object

		private readonly Action<object> _action;
		private readonly Func<object, bool> _canExecute;

		public RelayCommand(Action<object> action, Func<object, bool> canExecute = null)
		{
			_action = action;
			_canExecute = canExecute;
		}

		public void Execute(object parameter)
		{
			_action(parameter);
		}
		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute(parameter);
		}

		public event EventHandler CanExecuteChanged {
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}

}
