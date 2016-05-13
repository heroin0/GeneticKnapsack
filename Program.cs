using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;



namespace GAMultidimKnapsack
{

    class KnapsackConfig
    {
        int[] ActiveElements;

        public KnapsackConfig(int elementsAmount)
        {
            ActiveElements = new int[elementsAmount];
        }

        public KnapsackConfig(int[] initConfig)
        {
            ActiveElements = initConfig;
        }

        public void setValue(int position, int value)
        {
            ActiveElements[position] = value;
        }

        public void swapValue(int position)
        {
            ActiveElements[position] = -ActiveElements[position];
        }

        public int valueAt(int position)
        {
            return ActiveElements[position];
        }
    }

    class GeneticalAlgorithm
    {
        uint itemsAmount;
        double[,] itemsSet;//amount of items*their dimensions
        double[] restrictions;

        uint configsAmount;
        KnapsackConfig[] bestConfigs;


      public GeneticalAlgorithm(uint itemsAm, uint dimensions, double[] rest, uint confAm)
        {
            itemsAmount = itemsAm;
            restrictions = rest;
            itemsSet = new double[itemsAm, dimensions];

            configsAmount = confAm;
            //for (var i = 0; i < configsAmount; i++)
        }

        KnapsackConfig FirstApproachGenerate()//TODO - Узнать, как это делается
        {

        }

        KnapsackConfig Crossing(KnapsackConfig sack1, KnapsackConfig sack2)
        {
            int[] crossItems = new int[itemsAmount];
            for (var i = 0; i < itemsAmount; i++)
            {
                if (i < (itemsAmount / 2))
                crossItems[i] = sack1.valueAt(i);
                else
                    crossItems[i] = sack2.valueAt(i);
            }
            KnapsackConfig crossingResult = new KnapsackConfig(crossItems);
            //valid check here or not?
            return crossingResult;
        }

    }

    

    class Program
    {
        static void Main(string[] args)
        {
            double[] a =new double[]{ 1.2, 2.5, 2.7 };
            GeneticalAlgorithm GA = new GeneticalAlgorithm(4, 3,a,8);
        }
    }

}

namespace WindowsFormsApplication1
    {
        public partial class Form1 : Form
        {
            public Form1()
            {
                InitializeComponent();
            }

            private void Form1_Load(object sender, EventArgs e)
            {
                // Data arrays.
                string[] seriesArray = { "Cats", "Dogs" };
                int[] pointsArray = { 1, 2 };

                // Set palette.
                this.chart1.Palette = ChartColorPalette.SeaGreen;

                // Set title.
                this.chart1.Titles.Add("Pets");

                // Add series.
                for (int i = 0; i < seriesArray.Length; i++)
                {
                    // Add series.
                    Series series = this.chart1.Series.Add(seriesArray[i]);

                    // Add point.
                    series.Points.Add(pointsArray[i]);
                }
            }
        }
    }