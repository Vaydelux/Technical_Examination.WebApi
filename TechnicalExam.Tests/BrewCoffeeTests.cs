using Microsoft.AspNetCore.Mvc;
using Technical_Examination.WebApi.Controllers;
using Technical_Examination.WebApi.DataContext;
using Xunit;

namespace TechnicalExam.Tests;

public class BrewCoffeeTests
{

    [Fact]
    public void Return503_After5Endpoints()
    {
        var controller = new BrewCoffeeController();
        int counter = 1;

        foreach (var caseData in BrewCasesData())
        {
            var date = caseData.Date;
            var status_code = caseData.ExpectedStatusCode;

            var new_coffee = new BrewCoffee
            {
                Prepared = DateTime.Parse(date)
            };
            var result = controller.Get(new_coffee);

            if (result.Result is ObjectResult objresult)
            {
                Assert.Equal(status_code, objresult.StatusCode);

                if (status_code == 200 && counter < 5)
                {
                    var coffee = objresult.Value as BrewCoffee;
                    Assert.NotNull(coffee);
                    Assert.Equal("Your piping hot coffee is ready", coffee.Message);
                    Assert.Equal(DateTime.Parse(date), coffee.Prepared);
                }
                else if (status_code == 503 && counter == 5)
                {
                    Assert.Equal("503 Service Unavailable.", objresult.Value?.ToString());
                }
            }
            else if (result.Result is StatusCodeResult statusResult)
            {
                Assert.Equal(status_code, statusResult.StatusCode);
            }
            counter++;
        }
    }

    public static IEnumerable<CoffeeTestCase> BrewCasesData()
    {
        return
        [
            new CoffeeTestCase { Date = "2025-12-17T12:46:00Z", ExpectedStatusCode = 200 },
            new CoffeeTestCase { Date = "2021-11-10T20:43:00+08:00", ExpectedStatusCode = 200 },
            new CoffeeTestCase { Date = "2022-02-21T14:33:00+02:00", ExpectedStatusCode = 200 },
            new CoffeeTestCase { Date = "2024-07-23T08:36:00-04:00", ExpectedStatusCode = 200 },
            new CoffeeTestCase { Date = "2026-06-06T12:06:00Z", ExpectedStatusCode = 503 }
        ];
    }

    public class CoffeeTestCase
    {
        public string Date { get; set; }
        public int ExpectedStatusCode { get; set; }
    }
}
