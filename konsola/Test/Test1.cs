using SalaryCalculatorApp;

namespace Test
{
    [TestClass]
    public sealed class TestyKalkulatora
    {
        [TestMethod]
        public void TestPodatkuPit()
        {
            EmployeeSalaryCalculator kalkulator = new EmployeeSalaryCalculator(5500, EmploymentType.EmploymentContract, true);

            Assert.AreEqual(0, kalkulator.CalculatePitTax());
        }
    }
}
