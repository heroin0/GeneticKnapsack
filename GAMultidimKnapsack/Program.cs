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
        int[] CurrentConfiguration;

        public KnapsackConfig(int elementsAmount)
        {
            CurrentConfiguration = new int[elementsAmount];
        }

        public KnapsackConfig(int[] initConfig)
        {
            CurrentConfiguration = initConfig;
        }

        public void setValueToActive(int position)
        {
            CurrentConfiguration[position] = 1;
        }

        public void setValueToPassive(int position)
        {
            CurrentConfiguration[position] = -1;
        }

        public void swapValue(int position)
        {
            CurrentConfiguration[position] = -CurrentConfiguration[position];
        }

        public bool isValueActive(int position)
        {
            return (CurrentConfiguration[position] > 0);
        }

        public int valueAt(int position)
        {
            return (CurrentConfiguration[position]);
        }
    }

    
    class GeneticalAlgorithm
    {
        private static int itemsAmount, dimensions;
        private double[,] itemsSet;//amount of items*their dimensions
        private double[] restrictions;

        private uint configsInPoolAmount;
        private KnapsackConfig[] bestConfigs;

        private Crossing activeCrossing;

        public GeneticalAlgorithm(int itemsAm, int dim, double[] rest, uint confAm, Crossing myCrs)
        {
            itemsAmount = itemsAm;
            restrictions = rest;
            dimensions = dim;
            itemsSet = new double[itemsAm, dim];
            configsInPoolAmount = confAm;

            activeCrossing = myCrs;

            var bestConfigsAmount = 10;
            bestConfigs = new KnapsackConfig[bestConfigsAmount];
         
        }


        KnapsackConfig FirstApproachGenerate()
        {
            KnapsackConfig result = new KnapsackConfig(itemsAmount);

            for (var i = 0; i < itemsAmount; i++)
            {
                result.setValueToActive(i);
            }
            Random rand = new Random();
            while (!IsValid(result))
            {
                int positionNumber = rand.Next(itemsAmount);
                while (!result.isValueActive(positionNumber))
                {
                    positionNumber = rand.Next(itemsAmount);
                }
                result.setValueToPassive(positionNumber);
            }
            return result;
        }

        public delegate KnapsackConfig Crossing(KnapsackConfig sack1, KnapsackConfig sack2);

        public static KnapsackConfig Crossing1(KnapsackConfig sack1, KnapsackConfig sack2)
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

        KnapsackConfig Mutate(KnapsackConfig sack)
        {
            Random rand = new Random();
            KnapsackConfig mutatedSack = sack;//copy constructor
            int mutationPosition = rand.Next(itemsAmount);
            while (KnapsackConfig.Equals(mutatedSack, sack))
            {
                mutatedSack.swapValue(mutationPosition);
                if (!IsValid(mutatedSack))
                {
                    mutatedSack.swapValue(mutationPosition);
                    mutationPosition = rand.Next(itemsAmount);
                }
            }
            return mutatedSack;
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
            GeneticalAlgorithm GA = new GeneticalAlgorithm(4, 3, a, 8, GeneticalAlgorithm.Crossing1 );
            //TODO - переделать так, чтобы было актуально использование делегатов.
        }
    }

}

