using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Community.PowerToys.Run.Plugin.SVGL
{
    public class CategoryBaseConverter : JsonConverter<CategoryBase>
    {
        public override CategoryBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string category = reader.GetString();
                return new CategoryString(category);
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                var categories = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                return new CategoryArray(categories);
            }
            throw new JsonException("Invalid JSON for CategoryBase");
        }

        public override void Write(Utf8JsonWriter writer, CategoryBase value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case CategoryString categoryString:
                    writer.WriteStringValue(categoryString.Category);
                    break;

                case CategoryArray categoryArray:
                    JsonSerializer.Serialize(writer, categoryArray.Categories, options);
                    break;

                default:
                    throw new InvalidOperationException("Unknown CategoryBase type");
            }
        }
    }

    // ThemeBaseConverter: Used in order to make sure, the Json Deserialization deserialize Route Type/Property Properly, since Route is Discrimanating/Union Type (of string and object).
    public class ThemeBaseConverter : JsonConverter<ThemeBase>
    {
        public override ThemeBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                // Deserialize as ThemeString
                string themeString = reader.GetString();
                return new ThemeString(themeString);
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Deserialize (Reading) as ThemeObject
                SVGThemes themeObject = JsonSerializer.Deserialize<SVGThemes>(ref reader, options);
                return new ThemeObject(themeObject);
            }

            throw new JsonException("Invalid JSON for RouteBase");
        }

        public override void Write(Utf8JsonWriter writer, ThemeBase value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case ThemeString routeString:
                    writer.WriteStringValue(routeString.Route);
                    break;

                case ThemeObject routeObject:
                    JsonSerializer.Serialize(writer, routeObject.Route, options);
                    break;

                default:
                    throw new InvalidOperationException("Unknown RouteBase type");
            }
        }
    }
}
