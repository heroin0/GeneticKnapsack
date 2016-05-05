using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace GAMultidimKnapsack
{

    class KnapsackConfig
    {
        bool[] ActiveElements;

        KnapsackConfig(int elementsAmount)
        {
            ActiveElements = new bool[elementsAmount];
        }

        KnapsackConfig(bool[] initConfig)
        {
            ActiveElements = initConfig;
        }


        void setValue(int position, bool value)
        {
            ActiveElements[position] = value;
        }

        void swapValue(int position)
        {
            ActiveElements[position] = !ActiveElements[position];
        }
    }

    class Knapsack
    {
        double[,] itemsSet;
        double[] restrictions;
        public Knapsack(uint dimensions, uint amount, double[] rest)
        {
            restrictions = rest;  
            itemsSet = new double[dimensions, amount];
            Random rand = new Random(42);
            for (var i = 0; i < dimensions; i++)
                for (var j = 0; j < amount; j++)
                {
                    itemsSet[i, j] = rand.NextDouble();
                }
        }
    }

    class GeneticalAlgorithm
    {

    }


    class Program
    {
        static void Main(string[] args)
        {
            KnapsackItems GA = new KnapsackItems(4, 3);
        }
    }

}
