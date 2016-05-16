/* 
    Copyright 2016 Shawn Gilroy

    This file is part of Small N Stats.

    Small N Stats is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 2.

    Small N Stats is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Small N Stats.  If not, see <http://www.gnu.org/licenses/gpl-2.0.html>.

*/

using System.Collections.Generic;
using System.Windows.Controls;
using static BayesianModeling.Messaging.MessagesType;

namespace BayesianModeling.Messaging
{
    class RespondCellsMessage
    {
        public IList<DataGridCellInfo> SelectedCells { get; set; }
        public string RangeString { get; set; }
        public MessagingFormat Format { get; set; }
        public int LowCol { get; set; }
        public int HighCol { get; set; }
        public int LowRow { get; set; }
        public int HighRow { get; set; }
        public List<string> CellValues { get; set; }
    }
}
