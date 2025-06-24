using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace case1
{
    public class ResourceConstraints
    {
        public double Ore { get; set; }
        public double Nickel { get; set; }
        public double Chrome { get; set; }
        public double Manganese { get; set; }
        public double BlastFurnaceTime { get; set; }
        public double ConverterTime { get; set; }
        public double RollingMillTime { get; set; }
    }

    public class SteelGrade
    {
        public string Name { get; set; }
        public double OrePerTon { get; set; }
        public double NickelPerTon { get; set; }
        public double ChromePerTon { get; set; }
        public double ManganesePerTon { get; set; }
        public double BlastFurnaceTimePerTon { get; set; }
        public double ConverterTimePerTon { get; set; }
        public double RollingMillTimePerTon { get; set; }
        public double PricePerTon { get; set; }
        public double Target { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Steel Production Optimization v2.0");
                string filePath = @"C:\case1\input.txt";

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Файл {filePath} не найден.");
                    Console.WriteLine("Создайте файл input.txt в формате, указанном в задании.");
                    return;
                }

                // Чтение всего файла
                string[] lines = File.ReadAllLines(filePath);
                Console.WriteLine("Содержимое файла input.txt:");
                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }
                Console.WriteLine("\nНачинаем обработку...");

                // Парсинг данных
                var resources = ParseResources(lines);
                var grades = ParseSteelGrades(lines);

                // Проверка данных
                Console.WriteLine("\nПроверка данных:");
                Console.WriteLine($"Ресурсы: Руда={resources.Ore}, Никель={resources.Nickel}, Хром={resources.Chrome}");
                Console.WriteLine($"Марки стали: {grades.Count}");

                foreach (var grade in grades)
                {
                    Console.WriteLine($"  {grade.Name}: Цель={grade.Target}, Цена={grade.PricePerTon}");
                    Console.WriteLine($"    Руда={grade.OrePerTon}, Никель={grade.NickelPerTon}, Хром={grade.ChromePerTon}");
                }

                // Оптимизация
                OptimizeProduction(resources, grades);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            Console.ReadKey();
        }

        private static ResourceConstraints ParseResources(string[] lines)
        {
            var resources = new ResourceConstraints();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.Contains("Ruda") && line.Contains(':'))
                {
                    resources.Ore = ParseNumber(line.Split(':')[1]);
                }
                else if (line.Contains("Nikel") && line.Contains(':'))
                {
                    resources.Nickel = ParseNumber(line.Split(':')[1]);
                }
                else if (line.Contains("Hrom") && line.Contains(':'))
                {
                    resources.Chrome = ParseNumber(line.Split(':')[1]);
                }
                else if (line.Contains("Marganec") && line.Contains(':'))
                {
                    resources.Manganese = ParseNumber(line.Split(':')[1]);
                }
                else if ((line.Contains("domen") || line.Contains("pech")) && line.Contains(':'))
                {
                    resources.BlastFurnaceTime = ParseNumber(line.Split(':')[1]);
                }
                else if (line.Contains("konventor") && line.Contains(':'))
                {
                    resources.ConverterTime = ParseNumber(line.Split(':')[1]);
                }
                else if (line.Contains("prokatn") && line.Contains(':'))
                {
                    resources.RollingMillTime = ParseNumber(line.Split(':')[1]);
                }
            }

            return resources;
        }

        private static List<SteelGrade> ParseSteelGrades(string[] lines)
        {
            var grades = new List<SteelGrade>();
            SteelGrade currentGrade = null;
            int propertiesSet = 0;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("Marka stali"))
                {
                    if (currentGrade != null && propertiesSet >= 8)
                    {
                        grades.Add(currentGrade);
                    }

                    currentGrade = new SteelGrade();
                    propertiesSet = 0;

                    // Извлечение названия
                    var parts = line.Split(':');
                    currentGrade.Name = parts.Length > 1
                        ? parts[1].Trim()
                        : line.Replace("Marka stali", "").Trim();
                }
                else if (currentGrade != null)
                {
                    if (line.Contains("Ruda") && line.Contains(':'))
                    {
                        currentGrade.OrePerTon = ParseNumber(line.Split(':')[1]);
                        propertiesSet++;
                    }
                    else if (line.Contains("Nikel") && line.Contains(':'))
                    {
                        currentGrade.NickelPerTon = ParseNumber(line.Split(':')[1]);
                        propertiesSet++;
                    }
                    else if (line.Contains("Hrom") && line.Contains(':'))
                    {
                        currentGrade.ChromePerTon = ParseNumber(line.Split(':')[1]);
                        propertiesSet++;
                    }
                    else if (line.Contains("Marganec") && line.Contains(':'))
                    {
                        currentGrade.ManganesePerTon = ParseNumber(line.Split(':')[1]);
                        propertiesSet++;
                    }
                    else if ((line.Contains("domen") || line.Contains("pech")) && line.Contains(':'))
                    {
                        currentGrade.BlastFurnaceTimePerTon = ParseNumber(line.Split(':')[1]);
                        propertiesSet++;
                    }
                    else if (line.Contains("konventor") && line.Contains(':'))
                    {
                        currentGrade.ConverterTimePerTon = ParseNumber(line.Split(':')[1]);
                        propertiesSet++;
                    }
                    else if (line.Contains("prokatn") && line.Contains(':'))
                    {
                        currentGrade.RollingMillTimePerTon = ParseNumber(line.Split(':')[1]);
                        propertiesSet++;
                    }
                    else if (line.Contains("Cena") && line.Contains(':'))
                    {
                        currentGrade.PricePerTon = ParseNumber(line.Split(':')[1]);
                        propertiesSet++;
                    }
                    else if ((line.Contains("Celev") || line.Contains("obem")) && line.Contains(':'))
                    {
                        currentGrade.Target = ParseNumber(line.Split(':')[1]);
                        propertiesSet++;
                    }
                }
            }

            // Добавляем последнюю марку
            if (currentGrade != null && propertiesSet >= 8)
            {
                grades.Add(currentGrade);
            }

            return grades;
        }

        private static double ParseNumber(string input)
        {
            // Удаляем все нечисловые символы кроме . , -
            string clean = new string(input
                .Where(c => char.IsDigit(c) || c == '.' || c == ',' || c == '-')
                .ToArray());

            // Заменяем запятые на точки
            clean = clean.Replace(',', '.');

            // Удаляем лишние точки
            if (clean.Count(c => c == '.') > 1)
            {
                int firstDot = clean.IndexOf('.');
                clean = clean.Substring(0, firstDot + 1) +
                        clean.Substring(firstDot + 1).Replace(".", "");
            }

            if (double.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }

            return 0;
        }

        private static void OptimizeProduction(ResourceConstraints resources, List<SteelGrade> grades)
        {
            // Упрощенный алгоритм оптимизации
            double[] production = new double[grades.Count];
            double totalProfit = 0;

            // Пробуем произвести по целевым объемам
            for (int i = 0; i < grades.Count; i++)
            {
                production[i] = grades[i].Target;
            }

            // Рассчитываем использование ресурсов
            double oreUsed = 0, nickelUsed = 0, chromeUsed = 0, manganeseUsed = 0;
            double blastFurnaceUsed = 0, converterUsed = 0, rollingMillUsed = 0;

            for (int i = 0; i < grades.Count; i++)
            {
                var grade = grades[i];
                oreUsed += grade.OrePerTon * production[i];
                nickelUsed += grade.NickelPerTon * production[i];
                chromeUsed += grade.ChromePerTon * production[i];
                manganeseUsed += grade.ManganesePerTon * production[i];
                blastFurnaceUsed += grade.BlastFurnaceTimePerTon * production[i];
                converterUsed += grade.ConverterTimePerTon * production[i];
                rollingMillUsed += grade.RollingMillTimePerTon * production[i];
                totalProfit += production[i] * grade.PricePerTon;
            }

            // Проверяем, не превышаем ли ресурсы
            double reductionRatio = 1.0;

            if (oreUsed > resources.Ore && resources.Ore > 0)
                reductionRatio = Math.Min(reductionRatio, resources.Ore / oreUsed);
            if (nickelUsed > resources.Nickel && resources.Nickel > 0)
                reductionRatio = Math.Min(reductionRatio, resources.Nickel / nickelUsed);
            if (chromeUsed > resources.Chrome && resources.Chrome > 0)
                reductionRatio = Math.Min(reductionRatio, resources.Chrome / chromeUsed);
            if (manganeseUsed > resources.Manganese && resources.Manganese > 0)
                reductionRatio = Math.Min(reductionRatio, resources.Manganese / manganeseUsed);
            if (blastFurnaceUsed > resources.BlastFurnaceTime && resources.BlastFurnaceTime > 0)
                reductionRatio = Math.Min(reductionRatio, resources.BlastFurnaceTime / blastFurnaceUsed);
            if (converterUsed > resources.ConverterTime && resources.ConverterTime > 0)
                reductionRatio = Math.Min(reductionRatio, resources.ConverterTime / converterUsed);
            if (rollingMillUsed > resources.RollingMillTime && resources.RollingMillTime > 0)
                reductionRatio = Math.Min(reductionRatio, resources.RollingMillTime / rollingMillUsed);

            // Если нужно сократить производство
            if (reductionRatio < 1.0)
            {
                oreUsed = nickelUsed = chromeUsed = manganeseUsed = 0;
                blastFurnaceUsed = converterUsed = rollingMillUsed = 0;
                totalProfit = 0;

                for (int i = 0; i < grades.Count; i++)
                {
                    production[i] *= reductionRatio;

                    var grade = grades[i];
                    oreUsed += grade.OrePerTon * production[i];
                    nickelUsed += grade.NickelPerTon * production[i];
                    chromeUsed += grade.ChromePerTon * production[i];
                    manganeseUsed += grade.ManganesePerTon * production[i];
                    blastFurnaceUsed += grade.BlastFurnaceTimePerTon * production[i];
                    converterUsed += grade.ConverterTimePerTon * production[i];
                    rollingMillUsed += grade.RollingMillTimePerTon * production[i];
                    totalProfit += production[i] * grade.PricePerTon;
                }
            }

            // Вывод результатов
            Console.WriteLine("\nОптимальный производственный план:");
            for (int i = 0; i < grades.Count; i++)
            {
                Console.WriteLine($"Марка стали {grades[i].Name}: {Math.Round(production[i], 2)} тонн");
            }

            Console.WriteLine("\nИспользованные ресурсы:");
            Console.WriteLine($"Руда: {Math.Round(oreUsed, 2)} тонн");
            Console.WriteLine($"Никель: {Math.Round(nickelUsed, 2)} кг");
            Console.WriteLine($"Хром: {Math.Round(chromeUsed, 2)} кг");
            Console.WriteLine($"Марганец: {Math.Round(manganeseUsed, 2)} кг");
            Console.WriteLine($"Время доменной печи: {Math.Round(blastFurnaceUsed, 2)} ч");
            Console.WriteLine($"Время конвертера: {Math.Round(converterUsed, 2)} ч");
            Console.WriteLine($"Время прокатного стана: {Math.Round(rollingMillUsed, 2)} ч");
            Console.WriteLine($"\nОбщая прибыль: {Math.Round(totalProfit, 2)} руб.");

            // Проверка на достижение целей
            bool allTargetsMet = true;
            for (int i = 0; i < grades.Count; i++)
            {
                if (production[i] < grades[i].Target)
                {
                    allTargetsMet = false;
                    break;
                }
            }

            if (!allTargetsMet)
            {
                Console.WriteLine("\nВнимание: не удалось достичь целевых объемов для всех марок стали!");
            }
        }
    }
}