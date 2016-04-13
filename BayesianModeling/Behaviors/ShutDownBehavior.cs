/* 
    Copyright 2016 Shawn Gilroy

    This file is part of Bayesian Model Selector.

    Bayesian Model Selector is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 2.

    Bayesian Model Selector is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Bayesian Model Selector.  If not, see <http://www.gnu.org/licenses/gpl-2.0.html>.

*/

using System.Windows;
using System.Windows.Interactivity;

namespace BayesianModeling.Behaviors
{
    class ShutDownBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty ShutDownProperty =
            DependencyProperty.Register("ShuttingDownFlag", 
                typeof(bool), typeof(ShutDownBehavior), 
                new PropertyMetadata(false, OnValueChanged));

        public bool ShuttingDownFlag
        {
            get { return (bool)GetValue(ShutDownProperty); }
            set {
                SetValue(ShutDownProperty, value);
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as ShutDownBehavior) != null)
            {
                (d as ShutDownBehavior).OnValueChanged();
            }
        }

        private void OnValueChanged()
        {
            if (ShuttingDownFlag)
            {
                AssociatedObject.Close();
            }
        }
    }
}
