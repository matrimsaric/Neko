using GlickoDomain.RatingGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlickoDomain.Model
{
    internal enum RESULT
    {
        WIN = 0,
        LOSS = 1,
        UNKNOWN = 2,
    }
    public class Rating
    {
        private readonly RatingGenerator ratingGenerator;

        private RESULT Result;
        internal double Deviation { get; set; }
        internal double RatingValue { get; set; }
        internal double Volatility { get; set; }

        private Guid PlayerId { get; set; }

        internal bool NewPlayer { get; set; }


        private int numberOfResults;

        public Rating(RatingGenerator ratingSystem)
        {
            ratingGenerator = ratingSystem;
            RatingValue = RATING_DEFAULTS.RATING;
            Deviation = RATING_DEFAULTS.DEVIATION;
            Volatility = RATING_DEFAULTS.VOLATILITY;
            PlayerId = Guid.NewGuid();// new player, generate a tracking id
            NewPlayer = true;
        }

        public Rating(RatingGenerator ratingSystem, double initRating, double initDeviation, double initVolatility, Guid playerId)
        {
            ratingGenerator = ratingSystem;
            RatingValue = initRating;
            Deviation = initDeviation;
            Volatility = initVolatility;
            PlayerId = playerId;

            NewPlayer = false;
        }

        public void GetGlicko2Rating(double rating) => RatingValue = ratingGenerator.ConvertRatingToGlicko2Scale(rating);

        public void SetGlicko2Rating(double rating) => RatingValue = ratingGenerator .ConvertRatingToOriginalGlickoScale (rating);

        public double GetGlicko2RatingDeviation() => ratingGenerator.ConvertRatingDeviationToGlicko2Scale(Deviation);

        public void SetGlicko2RatingDeviation(double ratingDeviation) => Deviation = ratingGenerator.ConvertRatingDeviationToOriginalGlickoScale(ratingDeviation);
    }
}
