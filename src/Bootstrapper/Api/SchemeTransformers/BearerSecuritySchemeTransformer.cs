using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Api.SchemeTransformers;

internal sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider
) : IOpenApiDocumentTransformer
{
    private const string SchemeId = "Bearer";
    
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var schemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        if (!schemes.Any(s => s.Name == "Bearer"))
            return;

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeId] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization"
        };

        document.Security ??= new List<OpenApiSecurityRequirement>();

        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SchemeId)] = []
        });
    }
}