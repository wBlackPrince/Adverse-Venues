using AdverseVenues;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.UseSwaggerUI(
    options => options.SwaggerEndpoint("/openapi/v1.json", "Adverse-Venues v1"));

app.MapControllers();

Dictionary<string, HashSet<string>> Venues = new Dictionary<string, HashSet<string>>();


app.MapGet("/", (string location) =>
{
    if (!Venues.ContainsKey(location))
        return Results.BadRequest("location not found (use the following syntax /ru/svrd/...");
    
    return Results.Ok(Venues[location]);
});

app.MapPost("/upload-venues", (IFormFile file) =>
{
    Venues.Clear();
    
    string extension = Path.GetExtension(file.FileName);

    if (extension != ".txt")
        return Results.BadRequest("file's extensions should be txt");


    StreamReader reader = new StreamReader(file.OpenReadStream());

    // var text = reader
    //     .ReadToEnd()
    //     .TrimEnd()
    //     .Replace("\n", "")
    //     .Split("\r");
    
    var text = new List<string>();
    string? line;
    while ((line = reader.ReadLine()) != null)
    {
        text.Add(line.TrimEnd());
    }

    for (int i = 0; i < text.Count; i++)
    {
        var item = text[i].Split(':');
        
        string venueName = item[0];
        string[] places = item[1].Split(',');
        
        for (int j = 0; j < places.Length; j++)
        {
            if (!Venues.ContainsKey(places[j]))
            {
                Venues[places[j]] = new HashSet<string>();
            }
            
            Venues[places[j]].Add(venueName);
            
            string[] otherPlaces = places[j].Split('/').Take(places[j].Length - 1).ToArray();

            string current_place = $"/{otherPlaces[1]}";

            for (int k = 2; k < otherPlaces.Length; k++)
            {
                if (!Venues.ContainsKey(current_place))
                {
                    Venues[current_place] = new HashSet<string>();
                }
                Venues[current_place].Add(venueName);
                current_place += $"/{otherPlaces[k]}";
            }
        }
    }

    foreach (var key in Venues.Keys)
    {
        Console.WriteLine($"{key}: ");
        foreach (string venue in Venues[key])
        {
            Console.Write("-    ");
            Console.WriteLine($"{venue}");
        }
    }

    return TypedResults.Ok("ok");
}).DisableAntiforgery();


app.Run();