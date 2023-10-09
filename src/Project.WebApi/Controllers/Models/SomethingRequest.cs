using Architecture;
using Architecture.Core;
using FluentValidation;
using Project.Domain.SomethingContext.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Project.WebApi.Controllers.Models;

public class SomethingRequest
{
    public class Body
    {
        public Body(string @string, int number, bool boolean, DateTime dateTime)
        {
            String = @string;
            Number = number;
            Boolean = boolean;
            DateTime = dateTime;
        }

        public string String { get; }
        public int Number { get; }
        public bool Boolean { get; }
        public DateTime DateTime { get; }
    }

    public class BodyValidator : AbstractValidator<Body>
    {
        public BodyValidator()
        {
            var specification = SomethingValueObject.Specification<Body>.Create(
                Selector<Body>.Set(x => x.String),
                Selector<Body>.Set(x => x.Number),
                Selector<Body>.Set(x => x.Boolean),
                Selector<Body>.Set(x => x.DateTime)
            );

            this.UseSpecification(specification);
        }
    }

    public class BodyExample : IMultipleExamplesProvider<Body>
    {
        public IEnumerable<SwaggerExample<Body>> GetExamples()
        {
            yield return SwaggerExample.Create<Body>("可用的輸入", new Body("不為空字串", 1, true, SystemDateTime.UtcNow.AddHours(1)));
            yield return SwaggerExample.Create<Body>("不可用的輸入", new Body("", 0, false, SystemDateTime.UtcNow.AddHours(-1)));
        }
    }

    public class Response
    {
        public Response(SomethingValueObject valueObject)
        {
            ValueObject = valueObject;
        }

        public SomethingValueObject ValueObject { get; }
    }

    public class ResponseExample : IMultipleExamplesProvider<Response>
    {
        public IEnumerable<SwaggerExample<Response>> GetExamples()
        {
            yield return SwaggerExample.Create<Response>("正常的輸出",
                new Response(SomethingValueObject.Create("不為空字串", 1, true, SystemDateTime.UtcNow.AddHours(1)).Value));
        }
    }
}
