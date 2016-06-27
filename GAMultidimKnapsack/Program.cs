using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




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
        uint itemsAmount, dimensions;
        double[,] itemsSet;//amount of items*their dimensions
        double[] restrictions;

        uint configsAmount;
        KnapsackConfig[] bestConfigs;


        public GeneticalAlgorithm(uint itemsAm, uint dim, double[] rest, uint confAm)
        {
            itemsAmount = itemsAm;
            restrictions = rest;
            dimensions = dim;
            itemsSet = new double[itemsAm, dim];

            configsAmount = confAm;
            //for (var i = 0; i < configsAmount; i++)
        }

        //KnapsackConfig FirstApproachGenerate()//TODO - Узнать, как это делается
        //{

        //}

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

            return crossingResult;
        }

        public bool IsValid(KnapsackConfig config)
        {
            double[] summ = new double[dimensions];//якобы он забит нулями
            for (var i = 0; i < itemsAmount; i++)
            {
                if (config.valueAt(i) == 1)
                {
                    for (var j = 0; j < dimensions; j++)
                    {
                        summ[j] = itemsSet[i, j];
                        if (summ[j] > restrictions[j]) return false;
                    }
                }
                //Amount of items is much bigger than number of dimensions, so we can do checks on every turn. 
            }
            return true;
        }

    }



    class Program
    {
        static void Main(string[] args)
        {
            double[] a = new double[] { 1.2, 2.5, 2.7 };
            GeneticalAlgorithm GA = new GeneticalAlgorithm(4, 3, a, 8);
        }
    }

}

