using NFun.TypeInference.Solving;
using NUnit.Framework;

namespace NFun.HmTests
{
    public class IncTests
    {
        private HmHumanizerSolver solver;

        [SetUp]
        public void Init()
        {
            solver = new HmHumanizerSolver();
        }

    }
}