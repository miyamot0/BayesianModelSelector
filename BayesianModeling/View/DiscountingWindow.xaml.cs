//----------------------------------------------------------------------------------------------
// <copyright file="DiscountingWindow.cs" 
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

using BayesianModeling.ViewModel;
using System.Windows;

namespace BayesianModeling.View
{
    /// <summary>
    /// Interaction logic for DiscountingWindow.xaml
    /// </summary>
    public partial class DiscountingWindow : Window
    {
        public DiscountingWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (ViewModelDiscounting)DataContext;

            if (viewModel.ViewLoadedCommand.CanExecute(null))
                viewModel.ViewLoadedCommand.Execute(null);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = (ViewModelDiscounting)DataContext;

            if (viewModel.ViewClosingCommand.CanExecute(null))
                viewModel.ViewClosingCommand.Execute(null);
        }
    }
}
