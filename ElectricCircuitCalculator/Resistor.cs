using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricCircuitCalculator
{
    public class Resistor
    {
        public double Resistance { get; set; }
        public bool IsSeries { get; set; }
        public List<Resistor> ParallelResistors { get; set; }

        public Resistor(double resistance, bool isSeries)
        {
            Resistance = resistance;
            IsSeries = isSeries;
            ParallelResistors = new List<Resistor>();
        }
    }
}
