using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAMultidimKnapsack
{
    public partial class MyExperimentalForm : Form
    {
        public HistoryChart valuationsChart { get; set; }
        public MyExperimentalForm()
        {
            var table = new TableLayoutPanel() { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
            valuationsChart = new HistoryChart
            {
                Lines =
                {
                    new HistoryChartValueLine { DataFunction = { Color = Color.Green, BorderWidth=2 }
},
                    new HistoryChartValueLine { DataFunction = { Color = Color.Orange, BorderWidth=2 }},
                },
                Max = 1,
                Dock = DockStyle.Fill
            };
            table.Controls.Add(valuationsChart, 0, 0);
            table.RowStyles.Add(new RowStyle { SizeType = SizeType.Percent, Height = 50 });

            InitializeComponent();
        }
    }
}
