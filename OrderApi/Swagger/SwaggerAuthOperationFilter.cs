using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Orders.Api.Swagger
{
    public class SwaggerAuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Adiciona o esquema de segurança apenas para endpoints autorizados
            var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();

            if (authAttributes.Any())
            {
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "JWT"
                                }
                            }
                        ] = new List<string>()
                    }
                };

                // Adiciona metadata para customizar o Swagger UI
                operation.Extensions.Add(
                    "x-tokenInput",
                    new Microsoft.OpenApi.Any.OpenApiBoolean(true)
                );
            }
        }
    }
}
