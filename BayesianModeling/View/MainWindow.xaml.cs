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

using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using static BayesianModeling.Events.PublishSubscribe;

namespace BayesianModeling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///  Subscribe - Attaches events for Pub-Sub until proper MVVM tools and binding are added
        /// </summary>
        public void Subscribe()
        {
            PubSub<object>.RegisterEvent("OutputEventHandler", OutputEvents);
            PubSub<object>.RegisterEvent("SaveLogsEventHandler", SaveLogsEvent);
            PubSub<object>.RegisterEvent("ClearLogsEventHandler", ClearLogsEvent);
        }

        /// <summary>
        ///  OutputEvents - Is passed a stirng value, subsequently passed to RichTextBox
        /// </summary>
        public void OutputEvents(object sender, PubSubEventArgs<object> args)
        {
            Paragraph para = new Paragraph();
            para.Inlines.Add((string)args.Item);
            outputWindow2.Document.Blocks.Add(para);
            outputWindow2.ScrollToEnd();
            Scroller2.ScrollToEnd();
        }

        /// <summary>
        ///  SaveLogsEvent - Save contents of RichTextBox to .txt file
        /// </summary>
        public void SaveLogsEvent(object sender, PubSubEventArgs<object> args)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.FileName = "Logs";
            sd.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*";

            if (sd.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(sd.FileName))
                {
                    TextRange textRange = new TextRange(outputWindow2.Document.ContentStart, outputWindow2.Document.ContentEnd);
                    sw.Write(textRange.Text);
                }
            }
        }

        /// <summary>
        ///  ClearLogsEvent - Clear contents of RichTextBox
        /// </summary>
        public void ClearLogsEvent(object sender, PubSubEventArgs<object> args)
        {
            outputWindow2.Document.Blocks.Clear();
        }

    }
}
