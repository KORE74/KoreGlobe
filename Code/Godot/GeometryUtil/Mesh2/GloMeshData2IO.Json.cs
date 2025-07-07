using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

public static partial class GloMeshData2IO
{
    // --------------------------------------------------------------------------------------------
    // MARK: ToJson
    // --------------------------------------------------------------------------------------------

    // Save GloMeshData2 to JSON (triangles as 3 points, lines as native structure)
    public static string ToJson(GloMeshData2 mesh)
    {
        // Remove any quotes, brackets or potentially problematic characters from the name, so it can serialise correctly
        // Define a whitelist of characters, and remove any that are not in the whitelist
        char[] whitelist = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-".ToCharArray();
        mesh.Name = new string(mesh.Name.Where(c => whitelist.Contains(c)).ToArray());

        var obj = new
        {
            name = mesh.Name,
            vertices = mesh.Vertices,
            lines = mesh.Lines, // (int, int, Color, Color)
            triangles = mesh.Triangles.ConvertAll(t => new[] {
                mesh.Vertices[t.Item1],
                mesh.Vertices[t.Item2],
                mesh.Vertices[t.Item3]
            }),
            normals = mesh.Normals,
            uvs = mesh.UVs,
            vertexColors = mesh.VertexColors
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new Vector3Converter(), new Vector2Converter(), new ColorConverter(), new LineTupleConverter() }
        };
        return JsonSerializer.Serialize(obj, options);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: FromJson
    // --------------------------------------------------------------------------------------------

    // Load GloMeshData2 from JSON (optimistic: ignore unknowns, default missing)
    public static GloMeshData2 FromJson(string json)
    {
        var mesh = new GloMeshData2();
        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        mesh.Name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" : "";

        // Vertices
        var vertexList = new List<Vector3>();
        if (root.TryGetProperty("vertices", out var vertsProp) && vertsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var v in vertsProp.EnumerateArray())
                vertexList.Add(Vector3Converter.ReadVector3(v));
        }
        mesh.Vertices = vertexList;

        // Lines
        mesh.Lines = new List<(int, int, Color, Color)>();
        if (root.TryGetProperty("lines", out var linesProp) && linesProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var l in linesProp.EnumerateArray())
                mesh.Lines.Add(LineTupleConverter.ReadLineTuple(l));
        }

        // Triangles (as sets of 3 points)
        mesh.Triangles = new List<(int, int, int)>();
        if (root.TryGetProperty("triangles", out var trisProp) && trisProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var tri in trisProp.EnumerateArray())
            {
                if (tri.ValueKind == JsonValueKind.Array && tri.GetArrayLength() == 3)
                {
                    var a = Vector3Converter.ReadVector3(tri[0]);
                    var b = Vector3Converter.ReadVector3(tri[1]);
                    var c = Vector3Converter.ReadVector3(tri[2]);
                    int idxA = mesh.Vertices.FindIndex(v => v == a);
                    int idxB = mesh.Vertices.FindIndex(v => v == b);
                    int idxC = mesh.Vertices.FindIndex(v => v == c);
                    // If not found, add and use new index
                    if (idxA == -1) { mesh.Vertices.Add(a); idxA = mesh.Vertices.Count - 1; }
                    if (idxB == -1) { mesh.Vertices.Add(b); idxB = mesh.Vertices.Count - 1; }
                    if (idxC == -1) { mesh.Vertices.Add(c); idxC = mesh.Vertices.Count - 1; }
                    mesh.Triangles.Add((idxA, idxB, idxC));
                }
            }
        }

        // Normals
        mesh.Normals = new List<Vector3>();
        if (root.TryGetProperty("normals", out var normalsProp) && normalsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var n in normalsProp.EnumerateArray())
                mesh.Normals.Add(Vector3Converter.ReadVector3(n));
        }

        // UVs
        mesh.UVs = new List<Vector2>();
        if (root.TryGetProperty("uvs", out var uvsProp) && uvsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var uv in uvsProp.EnumerateArray())
                mesh.UVs.Add(Vector2Converter.ReadVector2(uv));
        }

        // VertexColors
        mesh.VertexColors = new List<Color>();
        if (root.TryGetProperty("vertexColors", out var colorsProp) && colorsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var c in colorsProp.EnumerateArray())
                mesh.VertexColors.Add(ColorConverter.ReadColor(c));
        }

        return mesh;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Converters
    // --------------------------------------------------------------------------------------------

    private class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadVector3(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.X);
            writer.WriteNumberValue(value.Y);
            writer.WriteNumberValue(value.Z);
            writer.WriteEndArray();
        }
        public static Vector3 ReadVector3(JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Array && el.GetArrayLength() == 3)
                return new Vector3(el[0].GetSingle(), el[1].GetSingle(), el[2].GetSingle());
            return Vector3.Zero;
        }
    }

    // --------------------------------------------------------------------------------------------

    private class Vector2Converter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadVector2(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.X);
            writer.WriteNumberValue(value.Y);
            writer.WriteEndArray();
        }
        public static Vector2 ReadVector2(JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Array && el.GetArrayLength() == 2)
                return new Vector2(el[0].GetSingle(), el[1].GetSingle());
            return Vector2.Zero;
        }
    }

    // --------------------------------------------------------------------------------------------

    private class ColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadColor(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.R);
            writer.WriteNumberValue(value.G);
            writer.WriteNumberValue(value.B);
            writer.WriteNumberValue(value.A);
            writer.WriteEndArray();
        }
        public static Color ReadColor(JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Array && el.GetArrayLength() >= 3)
            {
                float r = el[0].GetSingle();
                float g = el[1].GetSingle();
                float b = el[2].GetSingle();
                float a = el.GetArrayLength() > 3 ? el[3].GetSingle() : 1f;
                return new Color(r, g, b, a);
            }
            return new Color(1, 1, 1, 1);
        }
    }

    // --------------------------------------------------------------------------------------------

    private class LineTupleConverter : JsonConverter<(int, int, Color, Color)>
    {
        public override (int, int, Color, Color) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadLineTuple(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, (int, int, Color, Color) value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.Item1);
            writer.WriteNumberValue(value.Item2);
            ColorConverter cc = new ColorConverter();
            cc.Write(writer, value.Item3, options);
            cc.Write(writer, value.Item4, options);
            writer.WriteEndArray();
        }
        public static (int, int, Color, Color) ReadLineTuple(JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Array && el.GetArrayLength() == 4)
            {
                int a = el[0].GetInt32();
                int b = el[1].GetInt32();
                Color cA = ColorConverter.ReadColor(el[2]);
                Color cB = ColorConverter.ReadColor(el[3]);
                return (a, b, cA, cB);
            }
            return (0, 0, new Color(1, 1, 1, 1), new Color(1, 1, 1, 1));
        }
    }
}