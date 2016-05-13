//using AIRLab.GeneticAlgorithms;
using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetPartition
{
    internal static class ConcurrentQueueExtensions
    {
        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            T item;
            while (queue.TryDequeue(out item))
            {
                // do nothing
            }
        }
    }//вынести в отдельную либу

    static class SetPartition //в этом классе мы создаём экземпляр алгоритма и крутим его.
    {
        static Random rnd = new Random(42);
        static Form form;
        static ConcurrentQueue<double> averageValuations = new ConcurrentQueue<double>();
        static ConcurrentQueue<double> maxValuations = new ConcurrentQueue<double>();
        static ConcurrentQueue<double> ages = new ConcurrentQueue<double>();

        static HistoryChart valuationsChart = new HistoryChart();
        static HistoryChart agesChart = new HistoryChart();


        static void Algorithm()
        {
            while (true)
            {
                var watch = new Stopwatch();
                watch.Start();
                while (watch.ElapsedMilliseconds < 200)
                {
                   // ga.MakeIteration();
                    averageValuations.Enqueue(watch.ElapsedMilliseconds/100);//здесь мы добавляем свои значения.
                    maxValuations.Enqueue(watch.ElapsedTicks/100000);
                   // ages.Enqueue(rnd.NextDouble()*100);

                }
                watch.Stop();
                if (!form.IsDisposed)
                    form.BeginInvoke(new Action(UpdateCharts));
            }
        }

        static void UpdateCharts()
        {
            valuationsChart.AddRange(maxValuations, averageValuations);
          
            maxValuations.Clear();
            averageValuations.Clear();
           // agesChart.AddRange(ages);
          //  ages.Clear();
        }

        

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main2()
        {
            form = new Form();
            var table = new TableLayoutPanel() { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };

            valuationsChart = new HistoryChart
            {
                Lines = 
                {
                    new HistoryChartValueLine { DataFunction = { Color = Color.Green, BorderWidth=2 }},
                    new HistoryChartValueLine { DataFunction = { Color = Color.Orange, BorderWidth=2 }},
                },
                Max = 1,
                Dock = DockStyle.Fill
            };
            //agesChart = new HistoryChart
            //{
            //    Lines = { new HistoryChartValueLine { DataFunction = { Color = Color.Blue, BorderWidth=2 } } },
            //    Dock = DockStyle.Fill,
            //    Max=100
            //};

            table.Controls.Add(valuationsChart,0,0);
           // table.Controls.Add(agesChart,0,1);

            for (int i = 0; i < 2; i++)
            {
                table.RowStyles.Add(new RowStyle { SizeType = SizeType.Percent, Height = 50 });
            }
            
            form.Controls.Add(table);
            form.WindowState = FormWindowState.Maximized;

            new Thread(Algorithm) { IsBackground = true }.Start();
            Application.Run(form);

        }
    }
}
