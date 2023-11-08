using Rand = System.Random;
using UnityEngine;

namespace CustomDataTypes
{
    public sealed class ArithmeticProblemData
    {
        public ArithmeticProblemType Type { get; private set; }
        public int Term1 { get; private set; }
        public int Term2 { get; private set; }
        public int Answer { get; private set; }

        private readonly static Rand rand = new Rand(); 

        // min/max of problem terms are optional. min is inclusive while max is exclusive 
        public ArithmeticProblemData(int incMin = 10, int excMax = 100)
        {
            Type = RandomlyGenerateType();
            Term1 = RandomlyGeneratePositiveInt(incMin, excMax);
            Term2 = RandomlyGeneratePositiveInt(incMin, excMax);
            Answer = GetAnswer();

        }

        // this is for specific problems to be instanced. Useful for testing...
        public ArithmeticProblemData(ArithmeticProblemType type, int term1, int term2)
        {
            Type = type;
            Term1 = term1;
            Term2 = term2;
            Answer = GetAnswer();
        }

        private ArithmeticProblemType RandomlyGenerateType() => rand.Next(0, 2) == 0 ? ArithmeticProblemType.Addition : ArithmeticProblemType.Subtraction;
        
        private int RandomlyGeneratePositiveInt(int incMin, int excMax) => rand.Next(incMin, excMax);
        
        private int GetAnswer() => (Type == ArithmeticProblemType.Addition) ? Term1 + Term2 : Term1 - Term2;
       
        public override string ToString() => $"{Term1} {(Type == ArithmeticProblemType.Addition ? "+" : "-")} {Term2}";
    }
}