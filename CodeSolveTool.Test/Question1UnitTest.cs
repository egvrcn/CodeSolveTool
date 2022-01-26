using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TestYazilimi.Test
{
    public class Question1UnitTest
    {
        static string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\"));
        private readonly ITestOutputHelper _testOutputHelper;
        public Question1UnitTest(ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
        }

        [Theory]
        [MemberData(nameof(GetData), parameters: 5)]
        public void TestCases(int questionNo, int caseNo, string input, string output)
        {
            _testOutputHelper.WriteLine($"{questionNo}. question, {caseNo} test case process");
            //Arrange - Veri Girişleri
            int answer = 0;
            int.TryParse(output, out answer);

            //Act - Olması Gereken Davranış
            HashSet<char> set = new HashSet<char>();
            int currentMax = 0,
                i = 0,
                j = 0;

            while (j < input.Length)
                if (!set.Contains(input[j]))
                {
                    set.Add(input[j++]);
                    currentMax = Math.Max(currentMax, j - i);
                }
                else
                    set.Remove(input[i++]);

            //Assert - Sonuç Kontrol
            Assert.Equal(currentMax, answer);
        }

        public static IEnumerable<object[]> GetData(int numTests)
        {
            var listInputOutput = new List<object[]>();

            for (int k = 1; k <= 5; k++)
            {
                string input = File.ReadAllText($"{path}/CodeSolveTool/Inputs/Question1_Input{k}.txt");
                string output = File.ReadAllText($"{path}/CodeSolveTool/Outputs/Question1_Output{k}.txt");
                listInputOutput.Add(new object[] { 1, k, input, output });
            }

            return listInputOutput.Take(numTests);
        }
    }
}
