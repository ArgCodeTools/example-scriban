using ExampleScriban.Tests.Models;
using Scriban;
using Scriban.Runtime;
using System.Text.Json;

namespace ExampleScriban.Tests;

public class ScribanUnitTests
{
    private Template _template;

    public ScribanUnitTests()
    {
        _template = Template.Parse("Nuevo evento: {{data.name}} (DNI: {{data.document_number}})");
    }

    [Fact]
    public void Render_WithDynamicObject_ReturnCorrectString()
    {
        // Arrange
        var model = new
        {
            Data = new
            {
                Name = "Juan",
                DocumentNumber = "12345678"
            }
        };

        // Act
        var result = _template.Render(model);

        // Assert
        Assert.Equal("Nuevo evento: Juan (DNI: 12345678)", result);
    }

    [Fact]
    public void Render_WithDeserializeOnPocoObject_ReturnCorrectString()
    {
        // Arrange
        string dataJson = @"{
            ""Data"": {
                ""Name"": ""Juan"",
                ""DocumentNumber"": ""12345678""
            }
        }";

        var model = JsonSerializer.Deserialize<Model>(dataJson);

        // Act
        var result = _template.Render(model);

        // Assert
        Assert.Equal("Nuevo evento: Juan (DNI: 12345678)", result);
    }

    [Fact]
    public void Render_WithScriptObject_ReturnCorrectString()
    {
        // Arrange
        var template = Template.Parse("Nombre: {{Name}} | Documento: {{DocumentNumber}}"); // No sirve para datos anidados.

        string dataJson = @"{
            ""Name"": ""Juan"",
            ""DocumentNumber"": ""12345678""
        }";

        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(dataJson);

        var scriptObject = new ScriptObject();
        scriptObject.Import(dict);

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);

        // Act
        var result = template.Render(context);

        // Assert
        Assert.Equal("Nombre: Juan | Documento: 12345678", result);
    }

    [Fact]
    public void Render_WithNewLineCharacters_PreservesNewLines()
    {
        // Arrange
        var template = Template.Parse("Línea 4\nLínea 2\nLínea 3");

        // Act
        var result = template.Render();

        // Assert
        Assert.Equal("Línea 4\nLínea 2\nLínea 3", result);

        File.WriteAllBytes("ScribanNewLinesTest.txt", System.Text.Encoding.UTF8.GetBytes(result));
    }
}
