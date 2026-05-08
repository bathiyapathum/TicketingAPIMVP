using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlightBookingApi.Swagger;

/// <summary>
/// Copilot Studio / Power Platform allow only application/json (plus a few others) — Swashbuckle emits
/// text/json and application/*+json which fail validation. Foundry agents also commonly reject mixed-case
/// paths and strict additionalProperties rules.
/// </summary>
public sealed class CopilotCompatibleDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (swaggerDoc.Servers is null || swaggerDoc.Servers.Count == 0)
        {
            swaggerDoc.Servers =
            [
                new OpenApiServer
                {
                    Url = "/",
                    Description = "Paths are resolved relative to your API host (Foundry connection base URL)."
                }
            ];
        }

        foreach (var pathItem in swaggerDoc.Paths.Values)
        {
            foreach (var operation in pathItem.Operations.Values)
            {
                CollapseToApplicationJson(operation.RequestBody?.Content);
                VisitSchemasInContent(operation.RequestBody?.Content, SanitizeSchemaNode);

                foreach (var response in operation.Responses.Values)
                {
                    CollapseToApplicationJson(response.Content);
                    VisitSchemasInContent(response.Content, SanitizeSchemaNode);
                }
            }
        }

        if (swaggerDoc.Components?.Schemas is not null)
        {
            foreach (var schema in swaggerDoc.Components.Schemas.Values)
                VisitSchemaRecursive(schema, SanitizeSchemaNode);
        }
    }

    static void CollapseToApplicationJson(IDictionary<string, OpenApiMediaType>? content)
    {
        if (content is null || content.Count == 0)
            return;

        if (content.TryGetValue("application/json", out var json))
        {
            content.Clear();
            content["application/json"] = json;
            return;
        }

        var preferred = content.Keys.FirstOrDefault(k =>
            k.StartsWith("application/", StringComparison.OrdinalIgnoreCase) &&
            k.Contains("json", StringComparison.OrdinalIgnoreCase));

        var pickKey = preferred ?? content.Keys.First();
        var media = content[pickKey];
        content.Clear();
        content["application/json"] = media;
    }

    static void VisitSchemasInContent(IDictionary<string, OpenApiMediaType>? content, Action<OpenApiSchema> visit)
    {
        if (content is null)
            return;

        foreach (var media in content.Values)
        {
            if (media.Schema is not null)
                VisitSchemaRecursive(media.Schema, visit);
        }
    }

    static void VisitSchemaRecursive(OpenApiSchema schema, Action<OpenApiSchema> visit)
    {
        visit(schema);

        foreach (var nested in schema.Properties.Values)
            VisitSchemaRecursive(nested, visit);

        if (schema.Items is not null)
            VisitSchemaRecursive(schema.Items, visit);

        foreach (var nested in schema.AllOf)
            VisitSchemaRecursive(nested, visit);
        foreach (var nested in schema.OneOf)
            VisitSchemaRecursive(nested, visit);
        foreach (var nested in schema.AnyOf)
            VisitSchemaRecursive(nested, visit);

        if (schema.AdditionalProperties is not null &&
            schema.AdditionalProperties.Reference is null)
            VisitSchemaRecursive(schema.AdditionalProperties, visit);
    }

    /// <summary>
    /// Normalizes ProblemDetails-style empty additionalProperties objects and drops strict
    /// <c>additionalProperties: false</c>, which some Foundry / connector parsers reject.
    /// </summary>
    static void SanitizeSchemaNode(OpenApiSchema schema)
    {
        if (schema.AdditionalProperties is { } ap &&
            ap.Properties.Count == 0 &&
            ap.Reference is null &&
            ap.Type is null &&
            !ap.Nullable &&
            ap.OneOf.Count == 0 &&
            ap.AllOf.Count == 0 &&
            ap.AnyOf.Count == 0)
        {
            schema.AdditionalProperties = null;
            schema.AdditionalPropertiesAllowed = true;
        }

        if (schema.AdditionalPropertiesAllowed == false)
        {
            schema.AdditionalPropertiesAllowed = null;
            schema.AdditionalProperties = null;
        }
    }
}
