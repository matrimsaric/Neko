// See https://aka.ms/new-console-template for more information
using GlickoDomain.Model;
using GlickoDomain.RatingGeneration;

Console.WriteLine("Hello, World!");

// lets create a couple of players and see what result comes back
RatingGenerator ratingGenerator = new RatingGenerator();

GlickoRating play1 = new GlickoRating(ratingGenerator, 1004, 61, 0.68, Guid.NewGuid());
GlickoRating play2 = new GlickoRating(ratingGenerator, 1148, 61, 0.68, Guid.NewGuid());


// generate a game
Result newResult = new Result(play1, play2);

// see what the ratings do
string p1oldRating = play1.ToString();
string p2oldRating = play2.ToString();

List<Result> allResults = new List<Result>();
allResults.Add(newResult);
ratingGenerator.CalculateNewRating(play1, allResults);
ratingGenerator.CalculateNewRating(play2, allResults);

play1.FinalizeRating();
play2.FinalizeRating();
Console.WriteLine($"{play1.ToString()} ::  {p1oldRating}");
Console.WriteLine($"{play2.ToString()} ::  {p2oldRating}");
Console.ReadLine();

