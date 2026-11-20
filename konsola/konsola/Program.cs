using System;

namespace SalaryCalculatorApp
{
    /// <summary>
    /// Określa formę zatrudnienia pracownika.
    /// </summary>
    public enum EmploymentType
    {
        EmploymentContract, // Umowa o pracę
        MandateContract     // Umowa zlecenie
    }

    /********************************************************
    * nazwa klasy:EmployeeSalaryCalculator
    * opis klasy:wylicza podatki od wynagrodzenia(ZUS, PIT) oraz wynagrodzenie po wyliczeniu podatków
    * pola: -  grossAmount(wynagrodzenie), EmploymentType(rodzaj zatrudnienia), IsUnder26(wiek<26)
    * autor: Kacprzak Szymon
    ********************************************************/
    public class EmployeeSalaryCalculator
    {
        private const decimal SocialZusRate = 0.1371m; // 13,71%
        private const decimal HealthZusRate = 0.09m;   // 9,00%
        private const decimal PitTaxRate = 0.12m;      // 12,00%

        public decimal GrossAmount { get; }
        public EmploymentType EmploymentType { get; }
        public bool IsUnder26 { get; }

        public EmployeeSalaryCalculator(decimal grossAmount, EmploymentType employmentType, bool isUnder26)
        {
            if (!IsValidGrossAmount(grossAmount))
            {
                throw new ArgumentException("Kwota wynagrodzenia brutto musi być większa od zera.", nameof(grossAmount));
            }

            GrossAmount = grossAmount;
            EmploymentType = employmentType;
            IsUnder26 = isUnder26;
        }

        /// <summary>
        /// Waliduje czy wprowadzona kwota brutto jest dodatnia.
        /// </summary>
        public static bool IsValidGrossAmount(decimal grossAmount)
        {
            return grossAmount > 0;
        }

        /// <summary>
        /// Oblicza kwotę składek społecznych ZUS (13,71% kwoty brutto).
        /// </summary>
        public decimal CalculateSocialZus()
        {
            return GrossAmount * SocialZusRate;
        }

        /// <summary>
        /// Oblicza składkę zdrowotną (9,00% podstawy wymiaru: brutto - ZUS społeczny).
        /// </summary>
        public decimal CalculateHealthZus()
        {
            decimal baseForHealthZus = GrossAmount - CalculateSocialZus();
            return baseForHealthZus * HealthZusRate;
        }

        /// <summary>
        /// Oblicza zaliczkę na podatek PIT (uwzględniając ulgę dla młodych < 26 lat).
        /// </summary>
        public decimal CalculatePitTax()
        {
            if (IsUnder26)
            {
                return 0.00m;
            }

            decimal baseForPit = GrossAmount - CalculateSocialZus();
            return baseForPit * PitTaxRate;
        }

        /// <summary>
        /// Oblicza ostateczne wynagrodzenie netto (na rękę).
        /// </summary>
        public decimal CalculateNetSalary()
        {
            decimal socialZus = CalculateSocialZus();
            decimal healthZus = CalculateHealthZus();
            decimal pitTax = CalculatePitTax();

            return GrossAmount - socialZus - healthZus - pitTax;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Ustawienie kodowania UTF-8 dla poprawnego wyświetlania polskich znaków w konsoli
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== SYSTEM KALKULACJI WYNAGRODZEŃ PRACOWNIKÓW ===\n");

            // Tworzenie wariantu 1: Umowa o pracę bez ulgi dla młodych (wiek > 26 lat)
            var standardEmployee = new EmployeeSalaryCalculator(
                grossAmount: 6000.00m,
                employmentType: EmploymentType.EmploymentContract,
                isUnder26: false
            );

            DisplaySalarySummary("Wariant 1: Umowa o pracę (wiek > 26 lat, brak ulgi)", standardEmployee);

            // Tworzenie wariantu 2: Umowa zlecenie z ulgą dla młodych (wiek <= 26 lat)
            var youngContractor = new EmployeeSalaryCalculator(
                grossAmount: 6000.00m,
                employmentType: EmploymentType.MandateContract,
                isUnder26: true
            );

            DisplaySalarySummary("Wariant 2: Umowa zlecenie (wiek <= 26 lat, z ulgą PIT)", youngContractor);
        }

        /// <summary>
        /// Pomocnicza metoda wyświetlająca czytelne podsumowanie wyliczeń dla danego kalkulatora.
        /// </summary>
        private static void DisplaySalarySummary(string title, EmployeeSalaryCalculator calculator)
        {
            decimal socialZus = calculator.CalculateSocialZus();
            decimal healthZus = calculator.CalculateHealthZus();
            decimal pitTax = calculator.CalculatePitTax();
            decimal netSalary = calculator.CalculateNetSalary();

            Console.WriteLine(title);
            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"Forma zatrudnienia      : {GetEmploymentTypeName(calculator.EmploymentType)}");
            Console.WriteLine($"Ulga dla młodych (<26)  : {(calculator.IsUnder26 ? "Tak" : "Nie")}");
            Console.WriteLine($"Kwota brutto            : {calculator.GrossAmount,10:C2}");
            Console.WriteLine($"Składki ZUS społeczne   : {socialZus,10:C2}");
            Console.WriteLine($"Składka zdrowotna       : {healthZus,10:C2}");
            Console.WriteLine($"Zaliczka na podatek PIT : {pitTax,10:C2}");
            Console.WriteLine($"Wynagrodzenie netto     : {netSalary,10:C2}");
            Console.WriteLine(new string('=', 60) + "\n");
        }

        private static string GetEmploymentTypeName(EmploymentType type)
        {
            return type switch
            {
                EmploymentType.EmploymentContract => "Umowa o pracę",
                EmploymentType.MandateContract => "Umowa zlecenie",
                _ => "Nieznana"
            };
        }
    }
}
