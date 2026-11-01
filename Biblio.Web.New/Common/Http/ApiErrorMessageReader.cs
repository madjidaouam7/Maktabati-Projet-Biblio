using System.Text.Json;

namespace Biblio.Web.New.Common.Http;

public static class ApiErrorMessageReader
{

    public static async Task<string> ReadAsync(HttpResponseMessage response, string fallback)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(content))
        {
            return fallback;
        }

        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            // 1. Chercher d'abord les erreurs de validation
            if (TryGetPropertyIgnoreCase(root, "errors", out var errors))
            {
                var validationMessages = new List<string>();

                if (errors.ValueKind == JsonValueKind.Object)
                {
                    foreach (var error in errors.EnumerateObject())
                    {
                        if (error.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var value in error.Value.EnumerateArray())
                            {
                                if (value.ValueKind == JsonValueKind.String
                                    && !string.IsNullOrWhiteSpace(value.GetString()))
                                {
                                    validationMessages.Add(value.GetString()!);
                                }
                            }
                        }
                        else if (error.Value.ValueKind == JsonValueKind.String
                            && !string.IsNullOrWhiteSpace(error.Value.GetString()))
                        {
                            validationMessages.Add(error.Value.GetString()!);
                        }
                    }
                }

                if (validationMessages.Count > 0)
                {
                    return string.Join(" ", validationMessages);
                }
            }

            // 2. Si aucune erreur détaillée n'existe, chercher le message général
            foreach (var propertyName in new[] { "message", "error", "detail", "title" })
            {
                if (TryGetPropertyIgnoreCase(root, propertyName, out var property)
                    && property.ValueKind == JsonValueKind.String
                    && !string.IsNullOrWhiteSpace(property.GetString()))
                {
                    return property.GetString()!;
                }
            }

            return fallback;
        }
        catch (JsonException)
        {
            return content.Trim();
        }

        static bool TryGetPropertyIgnoreCase(
            JsonElement element,
            string name,
            out JsonElement property)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var candidate in element.EnumerateObject())
                {
                    if (string.Equals(
                        candidate.Name,
                        name,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        property = candidate.Value;
                        return true;
                    }
                }
            }

            property = default;
            return false;
        }
    }

}
