using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading;

namespace CodeSolveTool
{
    class Program
    {
        /// <summary>
        /// Yurtdışı yazılım geliştirici işe alımlarında sıklıkla kullanılan ve 
        /// her geçen gün yurt içi işe alımlarında da kullanılan HackerRank, LeetCode
        /// algoritma geliştirme ve yazılım problemi çözme ortamıdır.
        /// Şirketler kendi içlerinde bu problem çözme ve çözüm geçerleme uygulamaları geliştirmektedir.
        /// Ödevde bu yapının bir benzeri kodlanmıştır.
        /// Verilen olan problemlere ait test case'ler XUnit aracı ile birim testi üzerinden test edilmektedir.
        /// Inputs üzerinden verilen test case'lere göre Sorulara cevap verilmektedir.
        /// Verilen cevaplar ise Outputs klasöründeki dosyalara yazılmakta ve 
        /// bu dosyalara yazılan sonuçlar birim testine tabi tutulmaktadır.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("     CodeSolveTool     ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("------------------------");

            // Verilen soru sayısına göre cevaplar kaydedilir.
            AnswersCheck(3);

            // Kaydedilen cevaplar birim testine tabi tutulur. 
            AnswersPass(3);

            // Cevaplara ait performans testi gerçekleştirilir.
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n------------------------");
            if (Confirm("Would you like to perform performance tests?"))
                AnswerPerformanceTest();

            Console.Read();
        }

        static string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));

        /// <summary>
        /// Her soruya ait verilen cevaplar işleme koyularak elde edilen sonuçlar output klasörüne yazılır.
        /// CodeSolveTool.Test içerisinde ise sorulara ait input ve outputlara göre
        /// olması gereken doğru cevaplara göre unit test işlemi gerçekleştirilir.
        /// </summary>
        /// <param name="totalQuestions">Toplam soru sayısı bilgisi</param>
        static void AnswersCheck(int totalQuestions)
        {
            for (int i = 1; i < totalQuestions + 1; i++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\nSaving question {i} answers...");
                //Her test-case için cevap çıktıları alınarak output klasörüne yazılır
                //Bu sonuçlar birim testleri için kıyaslamada kullanılacaktır.
                for (int k = 1; k <= 5; k++)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"Saving test case {k} result.");
                    string input = File.ReadAllText($"{path}/Inputs/Question{i}_Input{k}.txt");

                    Answers QuestionClass = Activator.CreateInstance<Answers>();
                    MethodInfo Question = typeof(Answers).GetMethod("Question" + i);
                    int result = (int)Question.Invoke(QuestionClass, new object[] { input });

                    //Cevaplar Outputs klasörüne Question1_Output1 şeklinde yazılır. 
                    File.WriteAllText($"{path}/Outputs/Question{i}_Output{k}.txt", result + "");
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// CodeSolveTool.Test uygulaması içerisinde XUnit ile yazılmış olan birim testleri çalıştırılır.
        /// Verilen cevaplara göre elde edilen sonuçlar ekranda gösterilir.
        /// </summary>
        /// <param name="totalQuestions">Toplam soru sayısı bilgisi</param>
        static void AnswersPass(int totalQuestions)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n------------------------");
            Console.WriteLine("Unit test operations started...");
            //CodeSolveTool.Test uygulaması powershell üzerinden dotnet run komutu ile tetiklenir.
            //Test case'ler çalıştırıldıktan sonra alınan sonuçlar console uygulamasından gösterilir.
            //Testlerin geçerlenmesi için bir diğer yöntem doğrudan
            //CodeSolveTool.Test projesine sağ tıklayarak "Run Tests" komutunun çalıştırılmasıdır.
            using (PowerShell powershell = PowerShell.Create())
            {
                // Script ile CodeSolveTool.Test tetiklenerek unit test işlemleri gerçekleştirilir.
                powershell.AddScript("Set-ExecutionPolicy -Scope Process -ExecutionPolicy Unrestricted");
                powershell.AddScript($"&'{path}\\test-run.ps1'");

                // Script çalıştırılır.
                //Powershell üzerinden dönen sonuç XUnit tarafından gerçekleştirilmiş test sonuçlarıdır.
                var sonuc = powershell.Invoke();
                powershell.Streams.ClearStreams();
                powershell.Commands.Clear();

                List<string> listResult = new List<string>();
                foreach (PSObject obj in sonuc)
                {
                    listResult.Add(obj.ToString());
                }

                // Gerçekleştirilen test işlemlerinden dönen ham sonuç verileri renklendirilerek gösterilir.
                listResult = listResult.FindAll(x => x.Contains("questionNo"));

                for (int i = 1; i <= totalQuestions; i++)
                {
                    List<string> listResultItem = listResult.FindAll(y => y.Substring(y.IndexOf("questionNo: ") + 12, y.IndexOf(",") - (y.IndexOf("questionNo: ") + 12)) == i + "");
                    listResultItem.Sort((a, b) => a.Substring(a.IndexOf("caseNo: ") + 8, a.IndexOf(", input") - (a.IndexOf("questionNo: ") + 12)).CompareTo(b.Substring(b.IndexOf("caseNo: ") + 8, b.IndexOf(", input") - (b.IndexOf("caseNo: ") + 8))));

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\n> Question {i} test case processes started.");
                    int passed = 0, failed = 0;

                    //İlgili soruya ait test case sonuçları ekrana yazdırılır
                    for (int k = 0; k < listResultItem.Count; k++)
                    {
                        if (listResultItem[k].IndexOf("Passed") > -1 || listResultItem[k].IndexOf("Başarılı") > -1)
                        {
                            //Başarılı
                            passed++;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"+ Test case #{k + 1} is successful.");
                        }
                        else if (listResultItem[k].IndexOf("Failed") > -1 || listResultItem[k].IndexOf("Başarısız") > -1)
                        {
                            //Başarısız
                            failed++;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"X Test case #{k + 1} failed!");
                        }
                        Thread.Sleep(250);
                    }

                    //Sonuç bilgisi                
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    //Sonuçlar çıktı halinde gösterilir.
                    Console.WriteLine("|{0,-12}|{1,-12}|{2,-12}|{3,-12}|", "Question No", "Total Test", "Passed", "Failed");
                    Console.WriteLine("|{0,-12}|{1,-12}|{2,-12}|{3,-12}|", "------------", "------------", "------------", "------------");
                    Console.WriteLine("|{0,-12}|{1,-12}|{2,-12}|{3,-12}|", i, passed + failed, passed, failed);

                    if (failed == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"*All tests for question {i} passed successfully.");
                    }
                    else if (passed == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"*All tests fail for question {i}");
                    }
                    Thread.Sleep(500);
                }
            }
        }

        /// <summary>
        /// BenchmarkDotNet kütüphanesi aracılığıyla verilmiş olan cevaplara ait performans testi yapılır.
        /// Performans testleri sonucunda çözüme ait "işlem süresi" ve "RAM" kullanım bilgileri ekranda gösterilir. 
        /// </summary>
        static void AnswerPerformanceTest()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Performance tests are in progress, please wait...");
            var config = new ManualConfig();
            config.AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray());
            config.AddLogger(DefaultConfig.Instance.GetLoggers().ToArray());
            //Sonuçlar bin altındaki BenchmarkDotNet.Artifacts\results klasörüne html ve csv formatında kaydedilir.
            config.AddExporter(DefaultExporters.Html, DefaultExporters.Csv, DefaultExporters.Json);

            BenchmarkRunner.Run<Answers>(config);
        }


        public static bool Confirm(string title)
        {
            ConsoleKey response;
            do
            {
                Console.Write($"{ title } [y/n] ");
                response = Console.ReadKey(false).Key;
                if (response != ConsoleKey.Enter)
                {
                    Console.WriteLine();
                }
            } while (response != ConsoleKey.Y && response != ConsoleKey.N);

            return (response == ConsoleKey.Y);
        }
    }
}
