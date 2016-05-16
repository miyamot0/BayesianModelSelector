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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace BayesianModeling.View
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
        }

        private void outputBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = sender as Selector;
            if (selector is ListBox)
            {
                if (VisualTreeHelper.GetChildrenCount((selector as ListBox)) > 0)
                {
                    Border border = (Border)VisualTreeHelper.GetChild((selector as ListBox), 0);
                    ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                    scrollViewer.ScrollToBottom();
                }
            }
        }
    }
}
