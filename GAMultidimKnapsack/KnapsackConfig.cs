﻿using System;
using System.Collections.Generic;
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
            catch (NullReferenceException ex)
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
}
