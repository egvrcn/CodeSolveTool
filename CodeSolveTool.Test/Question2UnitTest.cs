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
    public class Question2UnitTest
    {
        static string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\"));
        private readonly ITestOutputHelper _testOutputHelper;
        public Question2UnitTest(ITestOutputHelper testOutputHelper)
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
            char[] arrInput = input.ToCharArray();
            char[] arrCheck = { '(', '[', '{' };
            List<int> checkedOut = new List<int>();
            int puan = 0;

            foreach (var item in arrInput)
            {
                if (arrCheck.Contains(item) && !checkedOut.Contains(item))
                {
                    checkedOut.Add(item);
                    int numOpen = arrInput.Where(x => x == item).Count();
                    int numClose = 0;
                    //(, {, [ açılış parantezleri değerlendirilir ve kapanış sayısıyla karşılaştırılır.
                    switch (item)
                    {
                        case '(':
                            numClose = arrInput.Where(y => y == ')').Count();
                            puan += numClose;
                            break;
                        case '[':
                            numClose = arrInput.Where(y => y == ']').Count();
                            puan += 2 * numClose;
                            break;
                        case '{':
                            numClose = arrInput.Where(y => y == '}').Count();
                            puan += 3 * numClose;
                            break;
                        default:
                            break;
                    }

                    if (numOpen != numClose)
                    {
                        //Eşitsizlik vardır
                        puan = -1;
                        break;
                    }
                }
            }

            //Assert - Sonuç Kontrol
            Assert.Equal(puan, answer);
        }

        public static IEnumerable<object[]> GetData(int numTests)
        {
            var listInputOutput = new List<object[]>();

            for (int k = 1; k <= 5; k++)
            {
                string input = File.ReadAllText($"{path}/CodeSolveTool/Inputs/Question2_Input{k}.txt");
                string output = File.ReadAllText($"{path}/CodeSolveTool/Outputs/Question2_Output{k}.txt");
                listInputOutput.Add(new object[] { 2, k, input, output });
            }

            return listInputOutput.Take(numTests);
        }
    }
}
