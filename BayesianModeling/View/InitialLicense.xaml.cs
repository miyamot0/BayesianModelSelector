﻿//----------------------------------------------------------------------------------------------
// <copyright file="InitialLicense.cs" 
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

using System.Windows;

namespace BayesianModeling.View
{
    /// <summary>
    /// Interaction logic for InitialLicense.xaml
    /// </summary>
    public partial class InitialLicense : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InitialLicense()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Affirm action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void agreeButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Decline action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void disagreeButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
