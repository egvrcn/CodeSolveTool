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
    public class Question3UnitTest
    {
        static string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\"));
        private readonly ITestOutputHelper _testOutputHelper;
        public Question3UnitTest(ITestOutputHelper testOutputHelper)
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
            int expectedAnswer;

            //Act - Olması Gereken Davranış
            Dictionary<int, int> dictNumbers = new Dictionary<int, int>();
            foreach (var num in input.Split(' '))
            {
                int number = 0;
                int.TryParse(num, out number);
                if (dictNumbers.ContainsKey(number))
                {
                    dictNumbers[number]++;
                }
                else
                {
                    dictNumbers[number] = 1;
                }
            }

            //Eşleniği çift olmayan sayı yalnız sayıdır.
            expectedAnswer = dictNumbers.Where(p => p.Value % 2 == 1).First().Key;

            //Assert - Sonuç Kontrol
            Assert.Equal(expectedAnswer, answer);
        }

        public static IEnumerable<object[]> GetData(int numTests)
        {
            var listInputOutput = new List<object[]>();

            for (int k = 1; k <= 5; k++)
            {
                string input = File.ReadAllText($"{path}/CodeSolveTool/Inputs/Question3_Input{k}.txt");
                string output = File.ReadAllText($"{path}/CodeSolveTool/Outputs/Question3_Output{k}.txt");
                listInputOutput.Add(new object[] { 3, k, input, output });
            }

            return listInputOutput.Take(numTests);
        }
    }
}
