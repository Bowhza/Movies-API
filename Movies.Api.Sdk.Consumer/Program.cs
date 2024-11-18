
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer;
using Movies.Contracts.Requests;
using Refit;

// var moviesApi = RestService.For<IMoviesApi>("https://localhost:44328");

// var movie = await moviesApi.GetMovieAsync("inception-2010");
var services = new ServiceCollection();


services
    .AddHttpClient()
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApi>(s => new RefitSettings
    {
        AuthorizationHeaderValueGetter = async (message, token) => await s.GetRequiredService<AuthTokenProvider>().GetTokenAsync() 
    })
    .ConfigureHttpClient(c => 
        c.BaseAddress = new Uri("https://localhost:44328"));

var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();

var newMovie = await moviesApi.CreateMovieAsync(new CreateMovieRequest
{
    Title = "Star Wars: The Force Awakens",
    YearOfRelease = 2015,
    Genres = new[] { "Action" }
});

var request = new GetAllMoviesRequest
{
    Title = null,
    Year = null,
    SortBy = null,
    Page = 1,
    PageSize = 5
};

var movies = await moviesApi.GetAllMoviesAsync(request);

foreach (var movieResponse in movies.Items)
{
    Console.WriteLine(JsonSerializer.Serialize(movieResponse));
}