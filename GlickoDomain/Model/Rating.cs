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
        public double Deviation { get; set; }
        public double RatingValue { get; set; }
        public double Volatility { get; set; }

        internal double WorkingDeviation { get; set; }
        internal double WorkingRatingValue { get; set; }
        internal double WorkingVolatility { get; set; }

        internal Guid PlayerId { get; set; }

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

        public double GetGlicko2Rating(double rating) => RatingValue = ratingGenerator.ConvertRatingToGlicko2Scale(rating);

        public void SetGlicko2Rating(double rating) => RatingValue = ratingGenerator .ConvertRatingToOriginalGlickoScale (rating);

        public double GetGlicko2RatingDeviation() => ratingGenerator.ConvertRatingDeviationToGlicko2Scale(Deviation);

        public void SetGlicko2RatingDeviation(double ratingDeviation) => Deviation = ratingGenerator.ConvertRatingDeviationToOriginalGlickoScale(ratingDeviation);

        public void FinalizeRating()
        {
            RatingValue = WorkingRatingValue;
            Deviation = WorkingDeviation;
            Volatility = WorkingVolatility;

            WorkingRatingValue = 0;
            WorkingDeviation = 0;
            WorkingVolatility = 0;
        }

        public int GetNumberOfResults() => numberOfResults;

        public void IncrementNumberOfResults(int increment) => numberOfResults += increment;

     
    }
}
