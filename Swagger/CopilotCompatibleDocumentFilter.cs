using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlightBookingApi.Swagger;

/// <summary>
/// Copilot Studio / Power Platform allow only application/json, text/plain, multipart/form-data,
/// application/x-www-form-urlencoded — Swashbuckle emits text/json and application/*+json which fail validation.
/// </summary>
public sealed class CopilotCompatibleDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var pathItem in swaggerDoc.Paths.Values)
        {
            foreach (var operation in pathItem.Operations.Values)
            {
                CollapseToApplicationJson(operation.RequestBody?.Content);
                foreach (var response in operation.Responses.Values)
                    CollapseToApplicationJson(response.Content);
            }
        }

        if (swaggerDoc.Components?.Schemas is not null)
        {
            foreach (var schema in swaggerDoc.Components.Schemas.Values)
                NormalizeAdditionalProperties(schema);
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

    static void NormalizeAdditionalProperties(OpenApiSchema schema)
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

        foreach (var nested in schema.Properties.Values)
            NormalizeAdditionalProperties(nested);

        if (schema.Items is not null)
            NormalizeAdditionalProperties(schema.Items);

        foreach (var nested in schema.AllOf)
            NormalizeAdditionalProperties(nested);
        foreach (var nested in schema.OneOf)
            NormalizeAdditionalProperties(nested);
        foreach (var nested in schema.AnyOf)
            NormalizeAdditionalProperties(nested);
    }
}
