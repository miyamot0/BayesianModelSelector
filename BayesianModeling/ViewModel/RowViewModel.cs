//----------------------------------------------------------------------------------------------
// <copyright file="RowViewModel.cs" 
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

using BayesianModeling.Utilities;
using System.ComponentModel;

namespace BayesianModeling.ViewModel
{
    public class RowViewModel : ViewModelBase
    {
        public string[] values = new string[100];

        /// <summary>
        /// Instantiate each row with n columns
        /// </summary>
        public RowViewModel()
        {
            for (int i=0; i<values.Length; i++)
            {
                values[i] = string.Empty;
            }
        }

        /// <summary>
        /// Force property changed calls on parameters (i.e., call columns directly)
        /// </summary>
        public void ForcePropertyUpdate(int col)
        {
            OnPropertyChanged(DataGridTools.GetColumnName(col));
        }

        public string A
        {
            get { return values[0]; }
            set
            {
                values[0] = value;
                OnPropertyChanged("A");
            }
        }

        public string B
        {
            get { return values[1]; }
            set
            {
                values[1] = value;
                OnPropertyChanged("B");
            }
        }

        public string C
        {
            get { return values[2]; }
            set
            {
                values[2] = value;
                OnPropertyChanged("C");
            }
        }

        public string D
        {
            get { return values[3]; }
            set
            {
                values[3] = value;
                OnPropertyChanged("D");
            }
        }

        public string E
        {
            get { return values[4]; }
            set
            {
                values[4] = value;
                OnPropertyChanged("E");
            }
        }

        public string F
        {
            get { return values[5]; }
            set
            {
                values[5] = value;
                OnPropertyChanged("F");
            }
        }

        public string G
        {
            get { return values[6]; }
            set
            {
                values[6] = value;
                OnPropertyChanged("G");
            }
        }

        public string H
        {
            get { return values[7]; }
            set
            {
                values[7] = value;
                OnPropertyChanged("H");
            }
        }

        public string I
        {
            get { return values[8]; }
            set
            {
                values[8] = value;
                OnPropertyChanged("I");
            }
        }

        public string J
        {
            get { return values[9]; }
            set
            {
                values[9] = value;
                OnPropertyChanged("J");
            }
        }

        public string K
        {
            get { return values[10]; }
            set
            {
                values[10] = value;
                OnPropertyChanged("K");
            }
        }

        public string L
        {
            get { return values[11]; }
            set
            {
                values[11] = value;
                OnPropertyChanged("L");
            }
        }
        
        public string M
        {
            get { return values[12]; }
            set
            {
                values[12] = value;
                OnPropertyChanged("M");
            }
        }

        public string N
        {
            get { return values[13]; }
            set
            {
                values[13] = value;
                OnPropertyChanged("N");
            }
        }

        public string O
        {
            get { return values[14]; }
            set
            {
                values[14] = value;
                OnPropertyChanged("O");
            }
        }

        public string P
        {
            get { return values[15]; }
            set
            {
                values[15] = value;
                OnPropertyChanged("P");
            }
        }

        public string Q
        {
            get { return values[16]; }
            set
            {
                values[16] = value;
                OnPropertyChanged("Q");
            }
        }

        public string R
        {
            get { return values[17]; }
            set
            {
                values[17] = value;
                OnPropertyChanged("R");
            }
        }

        public string S
        {
            get { return values[18]; }
            set
            {
                values[18] = value;
                OnPropertyChanged("S");
            }
        }

        public string T
        {
            get { return values[19]; }
            set
            {
                values[19] = value;
                OnPropertyChanged("T");
            }
        }

        public string U
        {
            get { return values[20]; }
            set
            {
                values[20] = value;
                OnPropertyChanged("U");
            }
        }

        public string V
        {
            get { return values[21]; }
            set
            {
                values[21] = value;
                OnPropertyChanged("V");
            }
        }

        public string W
        {
            get { return values[22]; }
            set
            {
                values[22] = value;
                OnPropertyChanged("W");
            }
        }

        public string X
        {
            get { return values[23]; }
            set
            {
                values[23] = value;
                OnPropertyChanged("X");
            }
        }

        public string Y
        {
            get { return values[24]; }
            set
            {
                values[24] = value;
                OnPropertyChanged("Y");
            }
        }

        public string Z
        {
            get { return values[25]; }
            set
            {
                values[25] = value;
                OnPropertyChanged("Z");
            }
        }

        public string AA
        {
            get { return values[26]; }
            set
            {
                values[26] = value;
                OnPropertyChanged("AA");
            }
        }

        public string AB
        {
            get { return values[27]; }
            set
            {
                values[27] = value;
                OnPropertyChanged("AB");
            }
        }

        public string AC
        {
            get { return values[28]; }
            set
            {
                values[28] = value;
                OnPropertyChanged("AC");
            }
        }

        public string AD
        {
            get { return values[29]; }
            set
            {
                values[29] = value;
                OnPropertyChanged("AD");
            }
        }

        public string AE
        {
            get { return values[30]; }
            set
            {
                values[30] = value;
                OnPropertyChanged("AE");
            }
        }

        public string AF
        {
            get { return values[31]; }
            set
            {
                values[31] = value;
                OnPropertyChanged("AF");
            }
        }

        public string AG
        {
            get { return values[32]; }
            set
            {
                values[32] = value;
                OnPropertyChanged("AG");
            }
        }

        public string AH
        {
            get { return values[33]; }
            set
            {
                values[33] = value;
                OnPropertyChanged("AH");
            }
        }

        public string AI
        {
            get { return values[34]; }
            set
            {
                values[34] = value;
                OnPropertyChanged("AI");
            }
        }

        public string AJ
        {
            get { return values[35]; }
            set
            {
                values[35] = value;
                OnPropertyChanged("AJ");
            }
        }

        public string AK
        {
            get { return values[36]; }
            set
            {
                values[36] = value;
                OnPropertyChanged("AK");
            }
        }

        public string AL
        {
            get { return values[37]; }
            set
            {
                values[37] = value;
                OnPropertyChanged("AL");
            }
        }

        public string AM
        {
            get { return values[38]; }
            set
            {
                values[38] = value;
                OnPropertyChanged("AM");
            }
        }

        public string AN
        {
            get { return values[39]; }
            set
            {
                values[39] = value;
                OnPropertyChanged("AN");
            }
        }

        public string AO
        {
            get { return values[40]; }
            set
            {
                values[40] = value;
                OnPropertyChanged("AO");
            }
        }

        public string AP
        {
            get { return values[41]; }
            set
            {
                values[41] = value;
                OnPropertyChanged("AP");
            }
        }

        public string AQ
        {
            get { return values[42]; }
            set
            {
                values[42] = value;
                OnPropertyChanged("AQ");
            }
        }

        public string AR
        {
            get { return values[43]; }
            set
            {
                values[43] = value;
                OnPropertyChanged("AR");
            }
        }

        public string AS
        {
            get { return values[44]; }
            set
            {
                values[44] = value;
                OnPropertyChanged("AS");
            }
        }

        public string AT
        {
            get { return values[45]; }
            set
            {
                values[45] = value;
                OnPropertyChanged("AT");
            }
        }

        public string AU
        {
            get { return values[46]; }
            set
            {
                values[46] = value;
                OnPropertyChanged("AU");
            }
        }

        public string AV
        {
            get { return values[47]; }
            set
            {
                values[47] = value;
                OnPropertyChanged("AV");
            }
        }

        public string AW
        {
            get { return values[48]; }
            set
            {
                values[48] = value;
                OnPropertyChanged("AW");
            }
        }

        public string AX
        {
            get { return values[49]; }
            set
            {
                values[49] = value;
                OnPropertyChanged("AX");
            }
        }

        public string AY
        {
            get { return values[50]; }
            set
            {
                values[50] = value;
                OnPropertyChanged("AY");
            }
        }

        public string AZ
        {
            get { return values[51]; }
            set
            {
                values[51] = value;
                OnPropertyChanged("AZ");
            }
        }

        public string BA
        {
            get { return values[52]; }
            set
            {
                values[52] = value;
                OnPropertyChanged("BA");
            }
        }

        public string BB
        {
            get { return values[53]; }
            set
            {
                values[53] = value;
                OnPropertyChanged("BB");
            }
        }

        public string BC
        {
            get { return values[54]; }
            set
            {
                values[54] = value;
                OnPropertyChanged("BC");
            }
        }

        public string BD
        {
            get { return values[55]; }
            set
            {
                values[55] = value;
                OnPropertyChanged("BD");
            }
        }

        public string BE
        {
            get { return values[56]; }
            set
            {
                values[56] = value;
                OnPropertyChanged("BE");
            }
        }

        public string BF
        {
            get { return values[57]; }
            set
            {
                values[57] = value;
                OnPropertyChanged("BF");
            }
        }

        public string BG
        {
            get { return values[58]; }
            set
            {
                values[58] = value;
                OnPropertyChanged("BG");
            }
        }

        public string BH
        {
            get { return values[59]; }
            set
            {
                values[59] = value;
                OnPropertyChanged("BH");
            }
        }

        public string BI
        {
            get { return values[60]; }
            set
            {
                values[60] = value;
                OnPropertyChanged("BI");
            }
        }

        public string BJ
        {
            get { return values[61]; }
            set
            {
                values[61] = value;
                OnPropertyChanged("BJ");
            }
        }

        public string BK
        {
            get { return values[62]; }
            set
            {
                values[62] = value;
                OnPropertyChanged("BK");
            }
        }

        public string BL
        {
            get { return values[63]; }
            set
            {
                values[63] = value;
                OnPropertyChanged("BL");
            }
        }

        public string BM
        {
            get { return values[64]; }
            set
            {
                values[64] = value;
                OnPropertyChanged("BM");
            }
        }

        public string BN
        {
            get { return values[65]; }
            set
            {
                values[65] = value;
                OnPropertyChanged("BN");
            }
        }

        public string BO
        {
            get { return values[66]; }
            set
            {
                values[66] = value;
                OnPropertyChanged("BO");
            }
        }

        public string BP
        {
            get { return values[67]; }
            set
            {
                values[67] = value;
                OnPropertyChanged("BP");
            }
        }

        public string BQ
        {
            get { return values[68]; }
            set
            {
                values[68] = value;
                OnPropertyChanged("BQ");
            }
        }

        public string BR
        {
            get { return values[69]; }
            set
            {
                values[69] = value;
                OnPropertyChanged("BR");
            }
        }

        public string BS
        {
            get { return values[70]; }
            set
            {
                values[70] = value;
                OnPropertyChanged("BS");
            }
        }

        public string BT
        {
            get { return values[71]; }
            set
            {
                values[71] = value;
                OnPropertyChanged("BT");
            }
        }

        public string BU
        {
            get { return values[72]; }
            set
            {
                values[72] = value;
                OnPropertyChanged("BU");
            }
        }

        public string BV
        {
            get { return values[73]; }
            set
            {
                values[73] = value;
                OnPropertyChanged("BV");
            }
        }

        public string BW
        {
            get { return values[74]; }
            set
            {
                values[74] = value;
                OnPropertyChanged("BW");
            }
        }

        public string BX
        {
            get { return values[75]; }
            set
            {
                values[75] = value;
                OnPropertyChanged("BX");
            }
        }

        public string BY
        {
            get { return values[76]; }
            set
            {
                values[76] = value;
                OnPropertyChanged("BY");
            }
        }

        public string BZ
        {
            get { return values[77]; }
            set
            {
                values[77] = value;
                OnPropertyChanged("BZ");
            }
        }

        public string CA
        {
            get { return values[78]; }
            set
            {
                values[78] = value;
                OnPropertyChanged("CA");
            }
        }

        public string CB
        {
            get { return values[79]; }
            set
            {
                values[79] = value;
                OnPropertyChanged("CB");
            }
        }

        public string CC
        {
            get { return values[80]; }
            set
            {
                values[80] = value;
                OnPropertyChanged("CC");
            }
        }

        public string CD
        {
            get { return values[81]; }
            set
            {
                values[81] = value;
                OnPropertyChanged("CD");
            }
        }

        public string CE
        {
            get { return values[82]; }
            set
            {
                values[82] = value;
                OnPropertyChanged("CE");
            }
        }

        public string CF
        {
            get { return values[83]; }
            set
            {
                values[83] = value;
                OnPropertyChanged("CF");
            }
        }

        public string CG
        {
            get { return values[84]; }
            set
            {
                values[84] = value;
                OnPropertyChanged("CG");
            }
        }

        public string CH
        {
            get { return values[85]; }
            set
            {
                values[85] = value;
                OnPropertyChanged("CH");
            }
        }

        public string CI
        {
            get { return values[86]; }
            set
            {
                values[86] = value;
                OnPropertyChanged("CI");
            }
        }

        public string CJ
        {
            get { return values[87]; }
            set
            {
                values[87] = value;
                OnPropertyChanged("CJ");
            }
        }

        public string CK
        {
            get { return values[88]; }
            set
            {
                values[88] = value;
                OnPropertyChanged("CK");
            }
        }

        public string CL
        {
            get { return values[89]; }
            set
            {
                values[89] = value;
                OnPropertyChanged("CL");
            }
        }

        public string CM
        {
            get { return values[90]; }
            set
            {
                values[90] = value;
                OnPropertyChanged("CM");
            }
        }

        public string CN
        {
            get { return values[91]; }
            set
            {
                values[91] = value;
                OnPropertyChanged("CN");
            }
        }

        public string CO
        {
            get { return values[92]; }
            set
            {
                values[92] = value;
                OnPropertyChanged("CO");
            }
        }

        public string CP
        {
            get { return values[93]; }
            set
            {
                values[93] = value;
                OnPropertyChanged("CP");
            }
        }

        public string CQ
        {
            get { return values[94]; }
            set
            {
                values[94] = value;
                OnPropertyChanged("CQ");
            }
        }

        public string CR
        {
            get { return values[95]; }
            set
            {
                values[95] = value;
                OnPropertyChanged("CR");
            }
        }

        public string CS
        {
            get { return values[96]; }
            set
            {
                values[96] = value;
                OnPropertyChanged("CS");
            }
        }

        public string CT
        {
            get { return values[97]; }
            set
            {
                values[97] = value;
                OnPropertyChanged("CT");
            }
        }

        public string CU
        {
            get { return values[98]; }
            set
            {
                values[98] = value;
                OnPropertyChanged("CU");
            }
        }

        public string CV
        {
            get { return values[99]; }
            set
            {
                values[99] = value;
                OnPropertyChanged("CV");
            }
        }


    }
}
