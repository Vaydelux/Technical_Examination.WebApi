using Microsoft.AspNetCore.Mvc;
using Technical_Examination.WebApi.Controllers;
using Technical_Examination.WebApi.DataContext;

namespace TechnicalExam.Tests;

public class BrewCoffeeTests
{

    [Theory]
    [MemberData(nameof(BrewCasesData))]
    public void Return503_After5Endpoints(string date, int status_code)
    {
        var new_coffee = new BrewCoffee
        {
            Prepared = DateTime.Parse(date)
        };
        var controller = new BrewCoffeeController();
        var result = controller.Get(new_coffee);

        if (result.Result is ObjectResult objresult)
        {
            Assert.Equal(status_code, objresult.StatusCode);

            if (status_code == 200)
            {
                var coffee = objresult.Value as BrewCoffee;
                Assert.NotNull(coffee);
                Assert.Equal("Your piping hot coffee is ready", coffee.Message);
                Assert.Equal(DateTime.Parse(date), coffee.Prepared);
            }
            else if (status_code == 503)
            {
                Assert.Equal("503 Service Unavailable.", objresult.Value?.ToString());
            }
        }
        else if (result.Result is StatusCodeResult statusResult)
        {
            Assert.Equal(status_code, statusResult.StatusCode);
        }
    }

    public static IEnumerable<object[]> BrewCasesData()
    {
        yield return new object[] { "2025-12-17T12:46:00Z", 200};
        yield return new object[] { "2021-11-10T20:43:00+08:00", 200 };
        yield return new object[] { "2022-02-21T14:33:00+02:00", 200 };
        yield return new object[] { "2024-07-23T08:36:00-04:00", 200 };
        yield return new object[] { "2026-06-06T12:06:00Z", 503};
    }

}
