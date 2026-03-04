using System.ComponentModel.DataAnnotations;
using URLShortener.Domain.DTOs;

namespace URLShortener.Tests.Validation;

public class RequestValidationTests
{
    [Fact]
    public void AuthenticationRequest_WhenInvalid_FailsValidation()
    {
        var request = new AuthenticationRequest
        {
            Username = "ab",
            Password = "123"
        };

        var isValid = TryValidate(request, out var results);

        Assert.False(isValid);
        Assert.NotEmpty(results);
    }

    [Fact]
    public void CreateShortUrlRequest_WhenLongUrlInvalid_FailsValidation()
    {
        var request = new CreateShortUrlRequest
        {
            LongUrl = "not-a-url"
        };

        var isValid = TryValidate(request, out var results);

        Assert.False(isValid);
        Assert.NotEmpty(results);
    }

    [Fact]
    public void AboutContentUpdateRequest_WhenContentMissing_FailsValidation()
    {
        var request = new AboutContentUpdateRequest();

        var isValid = TryValidate(request, out var results);

        Assert.False(isValid);
        Assert.NotEmpty(results);
    }

    private static bool TryValidate(object model, out List<ValidationResult> validationResults)
    {
        var context = new ValidationContext(model);
        validationResults = new List<ValidationResult>();

        return Validator.TryValidateObject(model, context, validationResults, true);
    }
}
