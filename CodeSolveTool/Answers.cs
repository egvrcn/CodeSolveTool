using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Order;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeSolveTool
{
    /// <summary>
    /// Parametre ile sırası ile Inputs klasörü içerisinde bulunan test case inputları ile işlemler yapılmaktadır.
    /// Kullanıcı parametre ile gelen input'a göre işlemini yapmalıdır. 
    /// Kullanıcının cevapları program çalıştırıldığı anda input'lara göre çalıştırılıp sonuçları outputs klasörüne text olarak yazılır.
    /// Outputs klasöründeki sonuçlar CodeSolveTool.Test içerisinde XUnit aracı ile birim testlerine tabi tutulur.
    /// Birim testleri sonucunda elde edilen test geçerleme sonuçları ekranda gösterilir.
    /// Kullanıcı bu sonuçlara göre hangi Questionnun hangi test case'lerinin geçip geçmediğini görebilir.
    /// Verilen cevaplara ayrıca performans testi yapılmaktadır. 
    /// Questionların doğru cevaplanmasının yanı sıra çözümün ne kadar performans olduğu da önemlidir.
    /// Performans testleri ile işlem süresi ve RAM kullanım bilgileri hesaplanmaktadır.
    /// </summary>
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [SimpleJob(RunStrategy.ColdStart, id: "Benchmarks"),
        GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [MemoryDiagnoser]
    public class Answers
    {
        /// <summary>
        /// Verilen string ifadesindeki karakterlerden birbirini tekrar etmeyen en uzun alt karakter dizisi bulunmalıdır.
        /// Örnek-1: birimtestleri ifadesindeki en uzun tekrar etmeyen karakter dizisi mtestl'dir ve cevap 3'tür.
        /// Örnek-2: rrrr ifadesindeki tüm karakterler tekrarlıdır ve cevap 1'dir.
        /// Örnek-3: ankara ifadesindeki en uzun tekrar etmeyen karakter dizisi nkar'dır ve cevap 4'tür.
        /// </summary>
        /// <param name="input">Inputs içerisinden gelen veriler</param>
        /// <returns>Elde edilen sonuç</returns>
        [Benchmark, BenchmarkCategory("Question1")]
        [ArgumentsSource(nameof(GetDataQuestion1))]
        public int Question1(string input)
        {
            //Soru 1 cevabını buraya yazınız!
            if (input == null || input == String.Empty)
                return 0;

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

            return currentMax;
        }


        /// <summary>
        /// Rastgele bir şekilde verilen parantezler kapanışlarına ve ağırlıklı puanlarına göre sayılmalıdır.
        /// () => 1 puan, [] => 2 puan, {} => 3 puan
        /// Parantezler [{}]([)] gibi verilebilir. Hiyerarşik bir sıralama yoktur.
        /// Verilen örnekte açıp kapanan çiftler puanlandırılır ve sonucu 8'dir.
        /// Eğer herhangi bir parantezin karşılığı yoksa -1 değeri döndürülmelidir.
        /// </summary>
        /// <param name="input">Inputs içerisinden gelen veriler</param>
        /// <returns>Elde edilen sonuç</returns>
        [Benchmark, BenchmarkCategory("Question2")]
        [ArgumentsSource(nameof(GetDataQuestion2))]
        public int Question2(string input)
        {
            //Soru 2 cevabını buraya yazınız!
            char[] arrInput = input.ToCharArray();
            char[] arrCheck = { '(', '[', '{' };
            List<int> checkedOut = new List<int>();
            int score = 0;

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
                            score += numClose;
                            break;
                        case '[':
                            numClose = arrInput.Where(y => y == ']').Count();
                            score += 2 * numClose;
                            break;
                        case '{':
                            numClose = arrInput.Where(y => y == '}').Count();
                            score += 3 * numClose;
                            break;
                        default:
                            break;
                    }

                    if (numOpen != numClose)
                    {
                        //Eşitsizlik vardır
                        return -2; //olması gereken cevap -1. 
                    }
                }
            }
            return score;
        }


        /// <summary>
        /// Girdi olarak verilen tam sayılardan eşi olmayan sayı(yalnız sayı) bulunmalıdır.
        /// Bir sayıya ait birden çok sayı çifti bulunabilir. 
        /// Örneğin 1 3 3 1 2 3 3 burada eşleniği olmayan sayı 2'dir. 
        /// 4 5 4 1 4 1 5 burada eşleniği olmayan sayı ise 4'tür.
        /// Tam sayılar aralarında birer boşluk bırakılarak verilmektedir.
        /// </summary>
        /// <param name="input">Inputs içerisinden gelen veriler</param>
        /// <returns>Elde edilen sonuç</returns>
        [Benchmark, BenchmarkCategory("Question3")]
        [ArgumentsSource(nameof(GetDataQuestion3))]
        public int Question3(string input)
        {
            //Soru 3 cevabını buraya yazınız!
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
            return dictNumbers.Where(p => p.Value % 2 == 1).First().Key;
        }

        #region TestInputs
        static string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));

        public static IEnumerable<object[]> GetDataQuestion1()
        {
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question1_Input1.txt") };
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question1_Input2.txt") };
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question1_Input3.txt") };
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question1_Input4.txt") };
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question1_Input5.txt") };
        }

        public static IEnumerable<object[]> GetDataQuestion2()
        {
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question2_Input1.txt") };
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question2_Input2.txt") };
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question2_Input3.txt") };
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question2_Input4.txt") };
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question2_Input5.txt") };
        }
        public static IEnumerable<object[]> GetDataQuestion3()
        {
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question3_Input1.txt") };
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question3_Input2.txt") };
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question3_Input3.txt") };
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question3_Input4.txt") };
            yield return new object[] { File.ReadAllText(@$"{path}/Inputs/Question3_Input5.txt") };
        }
        #endregion
    }
}
