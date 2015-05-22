using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.ComponentModel;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Represents an object that can help wire and unwire event handlers later in time.
    /// </summary>
    /// <typeparam name="T1">Additional info passed to the actions.</typeparam>
    /// <typeparam name="T2">The type of event handler.</typeparam>
    public sealed class HandlerReference<T1, T2>
    {
        /// <summary>
        /// Creates a new instance of HandlerReference
        /// </summary>
        /// <param name="removeHandler">The action to call when the event handler should be removed.</param>
        /// <param name="addHandler">The action to call when the event handler should be added.</param>
        public HandlerReference(Action<T1, T2> removeHandler, Action<T1, T2> addHandler)
        {
            this.AddHandler = addHandler;
            this.RemoveHandler = removeHandler;
        }

        /// <summary>
        /// Gets the action to call when the event handler is removed.
        /// </summary>
        public Action<T1, T2> RemoveHandler { get; private set; }

        /// <summary>
        /// Gets the action to call when the event handler is added.
        /// </summary>
        public Action<T1, T2> AddHandler { get; private set; }
    }

    /// <summary>
    /// Represents a command associated with a view model.
    /// </summary>
    public class ViewModelCommand : ICommand
    {
        private IInteractiveViewModel viewModel;

        /// <summary>
        /// Supports an opportunity to cancel or intercept a command that is about to execute.
        /// </summary>
        public event EventHandler<CancelEventArgs> Executing;

        /// <summary>
        /// The action to invoke when the Execute method is called.
        /// </summary>
        protected Action<IInteractiveViewModel> ExecuteMethod { get; set; }

        /// <summary>
        /// The action to invoke when the CanExecute method is called.
        /// </summary>
        protected Func<IInteractiveViewModel, bool> CanExecuteMethod { get; set; }

        /// <summary>
        /// The collection of event handlers delegates that will invoke CanExecuteChanged.
        /// </summary>
        protected IList<HandlerReference<IInteractiveViewModel, RoutedEventHandler>> ChangeHandlers { get; set; }

        /// <inheritdoc /> 
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Instantiates a new instance of the DelegateCommand class.
        /// </summary>
        /// <param name="executeMethod">A delegate to handle the Execute method</param>
        /// <param name="canExecuteMethod">A delegate to handle the CanExecute method</param>
        /// <param name="changeHandlers">A param array of delegates to call to wire and unwire notification for the CanExecuteChanged event.</param>
        public ViewModelCommand(Action<IInteractiveViewModel> executeMethod, Func<IInteractiveViewModel, bool> canExecuteMethod, params HandlerReference<IInteractiveViewModel, RoutedEventHandler>[] changeHandlers)
        {
            this.ExecuteMethod = executeMethod;
            this.CanExecuteMethod = canExecuteMethod;
            this.ChangeHandlers = changeHandlers;
        }

        /// <summary>
        /// Instantiates a new instance of the DelegateCommand class.
        /// Always returns true for CanExecute
        /// </summary>
        /// <param name="executeMethod">A delegate to handle the Execute method</param>
        public ViewModelCommand(Action<IInteractiveViewModel> executeMethod)
        {
            this.ExecuteMethod = executeMethod;
        }

        /// <summary>
        /// Instantiates a new instance of the DelegateCommand class.
        /// Delegates must be set separately.
        /// </summary>
        protected ViewModelCommand()
        { }

        /// <summary>
        /// Indicates whether or not the command can execute without a parameter
        /// </summary>
        /// <returns>boolean indicating whether the command can execute.</returns>
        public virtual bool CanExecute()
        {
            if (ViewModel == null) return false;
            if (CanExecuteMethod == null) return true;
            return CanExecuteMethod(ViewModel);
        }

        /// <summary>
        /// Executes the command without a parameter
        /// </summary>
        public virtual void Execute()
        {
            if (OnExecuting())
            {
                if (ExecuteMethod != null)
                {
                    ExecuteMethod(ViewModel);
                }
            }
        }

        /// <summary>
        /// Invokes the Executing event
        /// </summary>
        /// <returns>true if the event was canceled by the consumer.</returns>
        protected bool OnExecuting()
        {
            if (Executing != null)
            {
                var args = new CancelEventArgs();
                Executing(this, args);
                return !args.Cancel;
            }
            return true;
        }

        /// <inheritdoc /> 
        public virtual bool CanExecute(object parameter)
        {
            return CanExecute();
        }

        /// <inheritdoc /> 
        public virtual void Execute(object parameter)
        {
            Execute();
        }

        /// <summary>
        /// Invokes the CanExecuteChanged event.
        /// </summary>
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Invokes the CanExecuteChanged event.
        /// Useful to assign as an event handler.
        /// </summary>
        public void OnCanExecuteChanged(object sender, object eventArgs)
        {
            OnCanExecuteChanged();
        }

        /// <summary>
        /// Gets or sets the ViewModel associated with the command.
        /// </summary>
        public IInteractiveViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                if (viewModel != null && ChangeHandlers != null)
                {
                    foreach (var handlerRef in ChangeHandlers)
                    {
                        handlerRef.RemoveHandler(viewModel, OnCanExecuteChanged);
                    }
                }
                viewModel = value;
                if (viewModel != null && ChangeHandlers != null)
                {
                    foreach (var handlerRef in ChangeHandlers)
                    {
                        handlerRef.AddHandler(viewModel, OnCanExecuteChanged);
                    }
                }
                OnCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Represents a strongly typed command associated with a view model.
    /// </summary>
    internal class ViewModelCommand<T> : ViewModelCommand
    {
        /// <summary>
        /// The strongly typed delegate to handle the Execute method with the parameter cast.
        /// </summary>
        protected Action<IInteractiveViewModel, T> ExecuteParameterMethod { get; set; }

        /// <summary>
        /// The strongly typed delegate to handle the CanExecute method with the parameter cast.
        /// </summary>
        protected Func<IInteractiveViewModel, T, bool> CanExecuteParameterMethod { get; set; }

        /// <summary>
        /// Instantiates a new instance of the DelegateCommand class.
        /// </summary>
        /// <param name="executeMethod">A delegate to handle the Execute method</param>
        /// <param name="canExecuteMethod">A delegate to handle the CanExecute method</param>
        /// <param name="changeHandlers">A param array of delegates to call to wire and unwire notification for the CanExecuteChanged event.</param>
        public ViewModelCommand(Action<IInteractiveViewModel, T> executeMethod, Func<IInteractiveViewModel, T, bool> canExecuteMethod, params HandlerReference<IInteractiveViewModel, RoutedEventHandler>[] changeHandlers)
        {
            this.ExecuteParameterMethod = executeMethod;
            this.CanExecuteParameterMethod = canExecuteMethod;
            base.ChangeHandlers = changeHandlers;
        }

        /// <summary>
        /// Instantiates a new instance of the DelegateCommand class.
        /// </summary>
        /// <param name="executeMethod">A delegate to handle the Execute method</param>
        /// <param name="canExecuteMethod">A delegate to handle the CanExecute method</param>
        public ViewModelCommand(Action<IInteractiveViewModel, T> executeMethod, Func<IInteractiveViewModel, bool> canExecuteMethod)
        {
            this.ExecuteParameterMethod = executeMethod;
            base.CanExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Instantiates a new instance of the DelegateCommand class.
        /// </summary>
        /// <param name="executeMethod">A delegate to handle the Execute method</param>
        public ViewModelCommand(Action<IInteractiveViewModel, T> executeMethod)
        {
            this.ExecuteParameterMethod = executeMethod;
        }

        /// <inheritdoc /> 
        protected ViewModelCommand()
        { }

        /// <inheritdoc /> 
        public override bool CanExecute()
        {
            if (ViewModel == null) return false;
            if (CanExecuteParameterMethod == null) return true;
            return CanExecuteParameterMethod(ViewModel, default(T));
        }

        /// <inheritdoc /> 
        public override void Execute()
        {
            if (OnExecuting())
            {
                if (ExecuteParameterMethod != null)
                {
                    ExecuteParameterMethod(ViewModel, default(T));
                }
            }
        }

        /// <inheritdoc /> 
        public override bool CanExecute(object parameter)
        {
            if (CanExecuteParameterMethod == null)
            {
                return base.CanExecute();
            }
            else
            {
                if (ViewModel == null) return false;
                if (parameter is ValueType || parameter != null)
                {
                    return CanExecuteParameterMethod(ViewModel, (T)parameter);
                }
                else
                {
                    return CanExecuteParameterMethod(ViewModel, default(T));
                }
            }
        }

        /// <inheritdoc /> 
        public override void Execute(object parameter)
        {
            if (OnExecuting())
            {
                if (ExecuteParameterMethod != null)
                {
                    if (parameter is ValueType || parameter != null)
                    {
                        ExecuteParameterMethod(ViewModel, (T)parameter);
                    }
                    else
                    {
                        ExecuteParameterMethod(ViewModel, default(T));
                    }
                }
            }
        }
    }
}
