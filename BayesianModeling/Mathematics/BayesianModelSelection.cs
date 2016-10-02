//----------------------------------------------------------------------------------------------
// <copyright file="BayesianModelSelection.cs" 
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

namespace BayesianModeling.Mathematics
{
    class BayesianModelSelection
    {
        /// <summary>
        /// Chris Franck's Discounting Model Selection script is called from resources here (as string), for use in R function calls
        /// </summary> 
        public static string GetFranckFunction()
        {
            return Properties.Resources.FranckComputation;
        }

        /// <summary>
        /// Charting methods are called from this script (as string), to visualize results
        /// </summary>
        public static string GetLogChartFunction()
        {
            return Properties.Resources.ChartingFunctions;
        }
    }
}
