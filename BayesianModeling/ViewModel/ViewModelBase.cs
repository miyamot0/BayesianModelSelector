//----------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" 
// Copyright 2016 Shawn Gilroy
//
// This file is part of Discounting Model Selector.
//
// Discounting Model Selector is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 2.
//
// Discounting Model Selector is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Discounting Model Selector.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The Discounting Model Selector is a tool to assist researchers in behavior economics.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// </summary>
//----------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace BayesianModeling.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Base view model.  
        /// Essentially a template for inheriting INotifyPropertyChanged and IDisposable
        /// </summary>
        protected ViewModelBase() {}

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var evnt = new PropertyChangedEventArgs(propertyName);
                handler(this, evnt);
            }
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose() {}
    }
}
