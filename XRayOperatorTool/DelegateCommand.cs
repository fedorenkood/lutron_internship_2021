//using LoggingFramework;
using System;
using System.Text;
using System.Windows.Input;

namespace XRayOperatorTool
{
    /// <summary>
    /// Class for create commands
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region private members
        private readonly Predicate<object> canExecute;
        private readonly Action<object> execute;
        #endregion

        #region Constructor
        /// <summary>
        /// Create instance of delegate command with action to invoke
        /// </summary>
        /// <param name="execute">Action that will invoke when execute this command</param>
        public DelegateCommand(Action<object> execute) : this(execute, null) { }

        /// <summary>
        /// Create instance of command with execute and canExecute actions
        /// </summary>
        /// <param name="execute">Action to invoke when execute the command.</param>
        /// <param name="canExecute">Action to invode to validate whether command can execute in current state.</param>
        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException("Execute method can not be null."); ;
            this.canExecute = canExecute;
        }
        #endregion

        #region ICommand implement
        /// <summary>
        /// Method used to determine whether the command should able to execute in its current state.
        /// </summary>
        /// <param name="parameter">data used by the command. parameter can be null if no data required to pass.</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            try
            {
                bool canExecuteCommand = true;
                if (canExecute != null)
                {
                    canExecuteCommand = canExecute(parameter);
                    //Log.Instance.WriteDetailLog($"Can Execute {this.ToString()} with parameter {parameter} : {canExecuteCommand}");
                }
                return canExecuteCommand;
            }
            catch (Exception ex)
            {
                //log exception and throw.
                //Log.Instance.WriteExceptionEntry(ex, $"An error occured while can executing {this.ToString()} with parameters: {parameter}");
                throw;
            }
        }

        /// <summary>
        /// Event that occured to indicate whether or not this command can execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Method call to invoke the command.
        /// </summary>
        /// <param name="parameter">Data used by the command. parameter object can be null when no data required to pass.</param>
        public void Execute(object parameter)
        {
            try
            {
                if (CanExecute(parameter))
                {
                    //Log before executing command.
                    //Log.Instance.WriteDetailLog($"Executing {this.ToString()} with parameters: {parameter?.ToString()}");

                    //Execute command.
                    execute(parameter);
                }
            }
            catch (Exception ex)
            {
                //log exception and throw.
                //Log.Instance.WriteExceptionEntry(ex, $"An error occured while executing {this.ToString()} with parameters: {parameter}");
                throw;
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Trigger the can execture changed event for this command.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Return string that represent this object
        /// </summary>
        /// <returns>String that represent this object</returns>
        public override string ToString()
        {
            base.ToString();
            string target = execute != null ? (execute.Target != null ? execute.Target.ToString() : string.Empty) : string.Empty;
            string method = execute != null ? execute.Method.Name : string.Empty;

            return $"Command [{target}.{method}]";
        }

        #endregion
    }
}