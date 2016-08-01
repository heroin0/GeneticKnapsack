using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAMultidimKnapsack
{

   


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

