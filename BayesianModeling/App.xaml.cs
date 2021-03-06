﻿//----------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" 
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

using BayesianModeling.ViewModel;
using System.Windows;
using unvell.ReoGrid;

namespace BayesianModeling
{
    /// <summary>
    /// Interaction logic for App.xaml.  Overrides startup to launch core View/ViewModel
    /// </summary>
    public partial class App : Application
    {
        private static MainWindow window;

        private static ReoGridControl workbook;
        public static ReoGridControl Workbook
        {
            get
            {
                return window.reoGridControl;
            }
            set
            {
                workbook = value;
            }
        }

        public static bool IsSearchingForPick = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            window = new MainWindow();
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.DataContext = new ViewModelMainWindow
            {
                MainWindow = window
            };

            window.Show();
        }
    }
}
