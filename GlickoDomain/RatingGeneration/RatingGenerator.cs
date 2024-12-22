using GlickoDomain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlickoDomain.RatingGeneration
{
    public class RatingGenerator
    {
        private readonly double tau;
        private readonly double defaultVolatility;

        public RatingGenerator()
        {
            tau = RATING_DEFAULTS.TAU ;
            defaultVolatility = RATING_DEFAULTS.VOLATILITY;
        }

        public RatingGenerator(double newVolatility, double newTau)
        {
            tau = newTau;
            defaultVolatility = newVolatility;
        }

        private Tuple<Rating, Rating> CalculateSingleRating(Rating player1, Rating player2)
        {
            Tuple<Rating, Rating> result;

            var phi = player1.g


            return null;
        }




        public double ConvertRatingToOriginalGlickoScale(double rating) => ((rating * RATING_DEFAULTS.MULTIPLIER) + RATING_DEFAULTS.RATING);
        public double ConvertRatingToGlicko2Scale(double rating) => ((rating - RATING_DEFAULTS.RATING) /  RATING_DEFAULTS.MULTIPLIER);
        public double ConvertRatingDeviationToOriginalGlickoScale(double ratingDeviation) => (ratingDeviation * RATING_DEFAULTS.MULTIPLIER);
        public double ConvertRatingDeviationToGlicko2Scale(double ratingDeviation) => (ratingDeviation  / RATING_DEFAULTS.MULTIPLIER);
    }
}
