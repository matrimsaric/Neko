// See https://aka.ms/new-console-template for more information
using GlickoDomain.Model;
using GlickoDomain.RatingGeneration;

Console.WriteLine("Hello, World!");

// lets create a couple of players and see what result comes back
RatingGenerator ratingGenerator = new RatingGenerator();
Rating play1 = new Rating(ratingGenerator, 1200, 100, 6.5, Guid.NewGuid());
Rating play2 = new Rating(ratingGenerator, 1500,300, 6.5, Guid.NewGuid());

// generate a game
Result newResult = new Result(play1, play2);

// see what the ratings do
List<Result> allResults = new List<Result>();
allResults.Add(newResult);
ratingGenerator.CalculateNewRating(play1, allResults);
ratingGenerator.CalculateNewRating(play2, allResults);

play1.FinalizeRating();
play2.FinalizeRating();
Console.WriteLine($"Player 1 New Rating: {play1.RatingValue} New Deviation {play1.Deviation}");
Console.WriteLine($"Player 2 New Rating: {play2.RatingValue} New Deviation {play2.Deviation}");
Console.ReadLine();

