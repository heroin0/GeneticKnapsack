using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAMultidimKnapsack
{
    class GeneticalAlgorithm
    {
        private static int itemsAmount, dimensions;
        private static double[,] itemsSet;//amount of items*their dimensions
        private static double[] restrictions;
        private double[] itemsCosts;

        private int configsInPoolAmount;
        private int bestConfigsAmount;
        private KnapsackConfig[] configsPool;
        private KnapsackConfig[] bestConfigs;
        private double maximalKnapsackCost;

        private Crossing activeCrossing;
        private Mutation activeMutation;
        private static Random rand;

        public GeneticalAlgorithm(int itemsAm, int dim, double[] rest, double[] costs, double[,] myItemsSet, int confAm,  Crossing myCrs, Mutation myMt)
        {
            itemsAmount = itemsAm;
            restrictions = rest;
            dimensions = dim;
            itemsSet = new double[itemsAm, dim];
            rand = new Random(42);
            
            itemsSet = myItemsSet;
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
            if (GetKnapsackCost(configsPool[0]) == maximalKnapsackCost) return;
            //for (var j = 0; j < configsInPoolAmount/2; j++)//TODO: add customization of amount
            //{
            //    configsPool[j] = activeMutation(configsPool[j], rand);
            //}
            List<int> positions=new List<int>();
            while(positions.Count< 3*configsInPoolAmount / 4)
            {
                positions.Add(rand.Next(configsInPoolAmount));
                positions.Distinct();
            }
            foreach(var pos in positions)
            {
                configsPool[pos]= activeMutation(configsPool[pos], rand);
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

            for (int i = 0; i < bestConfigsAmount; i++)
            {
                if (GetKnapsackCost(bestConfigs[i]) < GetKnapsackCost(configsPool[0]))
                {
                    for (int j = i; i < bestConfigsAmount && j < configsInPoolAmount; j++, i++)
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

        public static KnapsackConfig SinglePointCrossing(KnapsackConfig sack1, KnapsackConfig sack2, bool isLeft)
        {
            int[] crossItems = new int[itemsAmount];
            int crossingPoint = rand.Next(itemsAmount);
            if (isLeft)
            {
                for (var i = 0; i < itemsAmount; i++)
                {
                    if (i < crossingPoint)
                        crossItems[i] = sack2.valueAt(i);
                    else
                        crossItems[i] = sack1.valueAt(i);
                }
            }
            else
            {
                for (var i = 0; i < itemsAmount; i++)
                {
                    if (i < crossingPoint)
                        crossItems[i] = sack1.valueAt(i);
                    else
                        crossItems[i] = sack2.valueAt(i);
                }
            }

            KnapsackConfig crossingResult = new KnapsackConfig(crossItems);
            return crossingResult;
        }

        public static KnapsackConfig BitByBitCrossing(KnapsackConfig sack1, KnapsackConfig sack2, bool isLeft)
        {
            int[] crossItems = new int[itemsAmount];
            if (isLeft)
            {
                for (var i = 0; i < itemsAmount; i++)
                {
                    if (i % 2==0)
                        crossItems[i] = sack2.valueAt(i);
                    else
                        crossItems[i] = sack1.valueAt(i);
                }
            }
            else
            {
                for (var i = 0; i < itemsAmount; i++)
                {
                    if (i % 2 == 0)
                        crossItems[i] = sack1.valueAt(i);
                    else
                        crossItems[i] = sack2.valueAt(i);
                }
            }

            KnapsackConfig crossingResult = new KnapsackConfig(crossItems);

            return crossingResult;
        }

        public delegate KnapsackConfig Mutation(KnapsackConfig sack, Random rand);

        public static KnapsackConfig SinglePointMutation(KnapsackConfig sack, Random rand)
        {
            KnapsackConfig mutatedSack = new KnapsackConfig(sack);//copy constructor
            int mutationPosition = rand.Next(itemsAmount);
            var count = 0;
            while (mutatedSack.Equals(sack) && count < 1000000)//TODO - not mutate empty sack
            {
                mutatedSack.swapValue(mutationPosition);
                if (!IsValid(mutatedSack))
                {
                    mutatedSack.swapValue(mutationPosition);
                    mutationPosition = rand.Next(itemsAmount);
                    count++;
                }   
            }
            while (!IsValid(mutatedSack)&&count == 1000000)//emergensy
            {
                mutatedSack.setValueToPassive(mutationPosition);
                mutationPosition = rand.Next(itemsAmount);
            }
            return mutatedSack;
        }

        public static KnapsackConfig MutateHalf(KnapsackConfig sack, Random rand)
        {
            KnapsackConfig mutatedSack = new KnapsackConfig(sack);
            int mutationPosition=rand.Next(itemsAmount);
            if (rand.Next()%2==0)
            {
                for(var i=0;i< itemsAmount / 2;i++)
                {
                    mutatedSack.swapValue(i);
                }
               // mutationPosition = 0;
            }
            else
            {
                for (var i = itemsAmount/2; i < itemsAmount; i++)
                {
                    mutatedSack.swapValue(i);
                }
                //mutationPosition = itemsAmount / 2;
            }
            while (!IsValid(mutatedSack))
            {
                mutatedSack.setValueToPassive(mutationPosition);
                mutationPosition++;
                if (mutationPosition == itemsAmount)
                    mutationPosition = 0;
            }
            return (mutatedSack);
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
            
            return count;
        }



        public double GetNormalizedMaximalKnapsackCost()
        {
            return GetKnapsackCost(configsPool[0])/maximalKnapsackCost;
        }

        public double GetNormaizedAveragePoolCost()
        {
            if (GetKnapsackCost(configsPool[0]) == maximalKnapsackCost) return GetKnapsackCost(configsPool[0]);
                double averagePoolCost = 0;
            foreach (var config in configsPool)
            {
                averagePoolCost += GetKnapsackCost(config);
            }
            averagePoolCost /= configsInPoolAmount;
            return averagePoolCost/maximalKnapsackCost;
        }

        public double GetAbsoluteMaximalKnapsackCost()
        {
            return GetKnapsackCost(configsPool[0]);
        }
    }
}
