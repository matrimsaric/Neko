using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlickoDomain.Model
{

    //https://github.com/RobinVangampelaere/Glicko2.NET/blob/master/src/RatingCalculator.cs
    public class RatingPeriodResults
    {
        private readonly List<Result> results = new List<Result>();
        private readonly HashSet<GlickoRating> participants = new HashSet<GlickoRating>();

        public RatingPeriodResults()
        { }

        public RatingPeriodResults(HashSet<GlickoRating> competitors)
        {
            participants = competitors;
        }

        public void AddResult(GlickoRating winner, GlickoRating loser)
        {
            var result = new Result(winner, loser);
            results.Add(result);
        }

        public void AddDraw(GlickoRating player1, GlickoRating player2)
        {
            var result = new Result(player1, player2, true);
            results.Add(result);
        }

        public IList<Result> GetResults(GlickoRating player)
        {
            var filteredResults = new List<Result>();
            foreach(var result in results) if(result.Participated(player)) filteredResults.Add(result);
            return filteredResults;
        }

        public IEnumerable<GlickoRating> GetParticipants()
        {
            // TODO Wont the following add duplicates and is that something we are concerned about?
            foreach(var result in results)
            {
                participants.Add(result.GetWinner());
                participants.Add(result.GetLoser());
            }

            return participants;
        }

        public void AddParticipant(GlickoRating rating) => participants.Add(rating);

        public void Clear() => results.Clear();


    }
}
