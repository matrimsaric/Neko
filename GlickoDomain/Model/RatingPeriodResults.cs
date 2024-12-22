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
        private readonly HashSet<Rating> participants = new HashSet<Rating>();

        public RatingPeriodResults()
        { }

        public RatingPeriodResults(HashSet<Rating> competitors)
        {
            participants = competitors;
        }

        public void AddResult(Rating winner, Rating loser)
        {
            var result = new Result(winner, loser);
            results.Add(result);
        }

        public void AddDraw(Rating player1, Rating player2)
        {
            var result = new Result(player1, player2, true);
            results.Add(result);
        }

        public IList<Result> GetResults(Rating player)
        {
            var filteredResults = new List<Result>();
            foreach(var result in results) if(result.Participated(player)) filteredResults.Add(result);
            return filteredResults;
        }

        public IEnumerable<Rating> GetParticipants()
        {
            // TODO Wont the following add duplicates and is that something we are concerned about?
            foreach(var result in results)
            {
                participants.Add(result.GetWinner());
                participants.Add(result.GetLoser());
            }

            return participants;
        }

        public void AddParticipant(Rating rating) => participants.Add(rating);

        public void Clear() => results.Clear();


    }
}
