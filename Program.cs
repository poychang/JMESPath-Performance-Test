using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DevLab.JmesPath;

var summary = BenchmarkRunner.Run<JMESPathBenchmarks>();

[MemoryDiagnoser]
public class JMESPathBenchmarks
{
    private string SampleFile = "sample.json";
    [Benchmark]
    public void UseLINQ()
    {
        var json = File.ReadAllText(SampleFile);
        var doc = System.Text.Json.JsonDocument.Parse(json);
        var data = doc.RootElement
            .GetProperty("locations")
            .EnumerateArray()
            .Where(location => location.GetProperty("state").ToString() == "WA")
            .Select(location => location.GetProperty("name").ToString())
            .OrderBy(name => name)
            .Aggregate((current, next) => $"{current}, {next}");
        var result = new { WashingtonCities = data };
        //Console.WriteLine(result);
    }

    [Benchmark]
    public void UseJMESPath()
    {
        var json = File.ReadAllText(SampleFile);
        const string expression = "locations[?state == 'WA'].name | sort(@) | {WashingtonCities: join(', ', @)}";
        var result = new JmesPath().Transform(json, expression);
        //Console.WriteLine(result);
    }
}