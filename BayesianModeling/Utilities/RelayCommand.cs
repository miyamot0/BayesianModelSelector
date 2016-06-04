//----------------------------------------------------------------------------------------------
// <copyright file="RelayCommand.cs" 
// Copyright 2016 Shawn Gilroy
//
// This file is part of Bayesian Model Selector.
//
// Bayesian Model Selector is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 2.
//
// Bayesian Model Selector is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Bayesian Model Selector.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The Bayesian Model Selector is a tool to assist researchers in behavior economics.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// </summary>
//----------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows.Input;

namespace BayesianModeling.Utilities
{
    class RelayCommand : ICommand
    {
        readonly Action<object> _executeAction;
        readonly Predicate<object> _executePredicate;

        /// <summary>
        /// Base command class
        /// </summary>
        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> predicate)
        {
            if (execute == null)
                throw new ArgumentNullException("Execute Action");

            _executeAction = execute;
            _executePredicate = predicate;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _executePredicate == null ? true : _executePredicate(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }
    }
}
