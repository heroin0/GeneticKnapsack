using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public KnapsackConfig(KnapsackConfig conf)
        {
            try
            {
                this.CurrentConfiguration = new int[conf.Length()];
                for (int i = 0; i < conf.Length(); i++)
                    this.CurrentConfiguration[i] = conf.valueAt(i);
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Empty configuration", "Null Reference Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
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

        public int Length()
        {
            try
            {
                    return CurrentConfiguration.Length;
            }
            catch(NullReferenceException ex)
            {
                MessageBox.Show("Empty configuration", "Null Reference Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return 0;
            }
        }

        public bool Equals(KnapsackConfig sack)
        {
            if (this.Length() != sack.Length()) return false;
            for (int i = 0; i < this.Length(); i++)
            {
                if (this.valueAt(i) != sack.valueAt(i))
                    return false;
            }
            return true;
        }
    }

    class GeneticalAlgorithm
    {
        private static int itemsAmount, dimensions;
        private static double[,] itemsSet;//amount of items*their dimensions
        private static double[] restrictions;
        private double[] itemsCosts;

        private uint configsInPoolAmount;
        private int bestConfigsAmount;
        private KnapsackConfig[] configsPool;
        private KnapsackConfig[] bestConfigs;
        private double maximalKnapsackCost;

        private Crossing activeCrossing;
        private Mutation activeMutation;
        private Random rand;

        public GeneticalAlgorithm(int itemsAm, int dim, double[] rest, double[] costs, uint confAm, Crossing myCrs, Mutation myMt)
        {
            itemsAmount = itemsAm;
            restrictions = rest;
            dimensions = dim;
            itemsSet = new double[itemsAm, dim];
            rand = new Random(42);
            for (int i = 0; i < itemsAmount; i++)
                for (int j = 0; j < dimensions; j++)
                    itemsSet[i, j] = rand.NextDouble();

            itemsCosts = costs;
            configsInPoolAmount = confAm;

            activeCrossing = myCrs;
            activeMutation = myMt;

            bestConfigsAmount = 10;
            int[] emptyConfig = (new int[itemsAmount]).Select(x => 0).ToArray();
            bestConfigs = (new KnapsackConfig[bestConfigsAmount]).Select(x => new KnapsackConfig(emptyConfig)).ToArray();//HACK
            configsPool = new KnapsackConfig[configsInPoolAmount];
            maximalKnapsackCost = itemsCosts.Sum();

            StartCycling();
        }

        private void StartCycling()
        {
            try
            {
                
                configsPool[0] = FirstApproachGenerate();

                int active = 0, passive = 0;
                for (int i = 0; i < itemsAmount; i++)
                {
                    if (configsPool[0].isValueActive(i))
                        active++;
                    else passive++;
                }
                //TODO:Do sth in that case. Somehow stop the algo. Also, need to return the best of our active configs to the output
                if (active == itemsAmount || passive == itemsAmount)
                    return;


                for (int i = 1; i < configsInPoolAmount; i++)
                {
                    configsPool[i] = activeMutation(configsPool[0], rand);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bugs in initialization", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        public void MakeIteration()
        {
            for (var j = 0; j < configsInPoolAmount / 2; j++)//TODO: add customization of amount
            {
                configsPool[j] = activeMutation(configsPool[j], rand);
            }

            KnapsackConfig[] crossingPool = new KnapsackConfig[configsInPoolAmount * 2 - 2];//not very well, if i want to customize crossing ,but works
            for (var j = 0; j < (configsInPoolAmount - 1); j++)
            {
                crossingPool[j] = activeCrossing(configsPool[j], configsPool[j + 1], true);
                crossingPool[(crossingPool.Length - 1) - j] = activeCrossing(configsPool[j], configsPool[j + 1], false);
            }
            var tempConfigs = crossingPool.OrderByDescending(config => GetKnapsackCost(config))
                .Distinct()
                .Take(Convert.ToInt32(configsInPoolAmount))
                .ToArray();
            configsPool = tempConfigs;

            for(int i=0;i<bestConfigsAmount;i++)
            {
                if (GetKnapsackCost(bestConfigs[i])< GetKnapsackCost(configsPool[0]))
                {
                    for (int j = i; i < bestConfigsAmount && j<configsInPoolAmount; j++, i++)
                        bestConfigs[i] = configsPool[j];
                    break;
                }
            }
           
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

        public delegate KnapsackConfig Crossing(KnapsackConfig sack1, KnapsackConfig sack2, bool isLeft);

        public static KnapsackConfig Crossing1(KnapsackConfig sack1, KnapsackConfig sack2, bool isLeft)
        {
            int[] crossItems = new int[itemsAmount];
            if (isLeft)
            {
                for (var i = 0; i < itemsAmount; i++)
                {
                    if (i < (itemsAmount / 2))
                        crossItems[i] = sack2.valueAt(i);
                    else
                        crossItems[i] = sack1.valueAt(i);
                }
            }
            else
            {
                for (var i = 0; i < itemsAmount; i++)
                {
                    if (i < (itemsAmount / 2))
                        crossItems[i] = sack1.valueAt(i);
                    else
                        crossItems[i] = sack2.valueAt(i);
                }
            }

            KnapsackConfig crossingResult = new KnapsackConfig(crossItems);

            return crossingResult;
        }

        public delegate KnapsackConfig Mutation(KnapsackConfig sack, Random rand);

        public static KnapsackConfig Mutate1(KnapsackConfig sack, Random rand)
        {
            KnapsackConfig mutatedSack = new KnapsackConfig(sack);//copy constructor
            int mutationPosition = rand.Next(itemsAmount);
            var count = 0;
            while (mutatedSack.Equals(sack) && count < 100)
            {
                mutatedSack.swapValue(mutationPosition);
                if (!IsValid(mutatedSack))
                {
                    mutatedSack.swapValue(mutationPosition);
                    mutationPosition = rand.Next(itemsAmount);
                }
                else
                {
                    count++;
                }
            }
            return mutatedSack;
        }

        private static bool IsValid(KnapsackConfig config)
        {
            double[] summ = new double[dimensions];//якобы он забит нулями
            for (var i = 0; i < itemsAmount; i++)
            {
                if (config.isValueActive(i))
                {
                    for (var j = 0; j < dimensions; j++)
                    {
                        summ[j] += itemsSet[i, j];
                        if (summ[j] > restrictions[j]) return false;
                    }
                }
                //Amount of items is much bigger than number of dimensions, so we can do checks on every turn. 
            }
            return true;
        }

        private double GetKnapsackCost(KnapsackConfig sack)
        {
            double count = 0;
            for (int i = 0; i < itemsAmount; i++)
                if (sack.isValueActive(i))
                    count += itemsCosts[i];
            double result = count / maximalKnapsackCost;
            return result;
        }

 

        public double GetMaximalKnapsackInPoolCost()
        {
            return GetKnapsackCost(configsPool[0]);
        }

        public double GetAveragePoolCost()
        {
            double averagePoolCost = 0;
            foreach (var config in configsPool)
            {
                averagePoolCost += GetKnapsackCost(config);
            }
            averagePoolCost /= configsInPoolAmount;
            return averagePoolCost;
        }
    }

    class Program
    {
        static void Main2(string[] args)
        {
            double[] restrictions = new double[] { 1.5, 1.2, 2.7 }, costs = { 0.5, 0.4, 0.7, 1.1 };

            GeneticalAlgorithm GA = new GeneticalAlgorithm(4, 3, restrictions, costs, 8, GeneticalAlgorithm.Crossing1, GeneticalAlgorithm.Mutate1);
            Console.WriteLine("//TODO - переделать так, чтобы было актуально использование делегатов.");
            
        }
    }

}

