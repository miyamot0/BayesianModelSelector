/*
 * Shawn Gilroy, 2016
 * Bayesian Model Selection Application
 * Base view model class from which to inherit property update methods
 * 
 *  */

using System;
using System.ComponentModel;

namespace BayesianModeling.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        protected ViewModelBase() {}

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose() {}
    }
}
