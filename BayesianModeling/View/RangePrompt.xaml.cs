﻿using System.Windows;
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

namespace BayesianModeling.View
{
    /// <summary>
    /// Interaction logic for RangePrompt.xaml
    /// </summary>
    public partial class RangePrompt : Window
    {
        public RangePrompt()
        {
            InitializeComponent();
        }

        public string ResponseText
        {
            get { return RangeText.Text; }
            set { RangeText.Text = value; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
