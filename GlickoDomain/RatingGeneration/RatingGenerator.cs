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

        public void CalculateNewRating(GlickoRating player, IList<Result> results)
        {
            var phi = player.GetGlicko2RatingDeviation();
            var sigma = player.Volatility;
            var a = Math.Log(Math.Pow(sigma, 2));
            var delta = Delta(player, results);
            var v = V(player, results);

            // step 5.2 - set the initial value of the iterative algorithm to come in step 5.4
            var A = a;
            double B;

            if (Math.Pow(delta, 2) > Math.Pow(phi, 2) + v) B = Math.Log(Math.Pow(delta, 2) - Math.Pow(phi, 2) - v);
            else
            {
                double k = 1;
                B = a - (k * Math.Abs(tau));

                while(F(B, delta, phi, v, a, tau) < 0)
                {
                    k++;
                    B = a - (k * Math.Abs(tau));
                }

            }

            // step 5.3
            var fA = F(A, delta, phi, v, a, tau);
            var fB = F(B, delta, phi, v, a, tau);

            // step 5.4
            while(Math.Abs(B- A) > RATING_DEFAULTS.CONVERGENCE_TOLERANCE)
            {
                double C = A + ((A - B) * fA / (fB - fA));
                try
                {
                    double fC = F(C, delta, phi, v, a, tau);

                    if (fC * fB < 0)
                    {
                        A = B;
                        fA = fB;
                    }
                    else fA /= 2.0;

                    B = C;
                    fB = fC;
                }
                catch (Exception ex)
                {
                    string t = ex.Message; 
                }
               
            }

            var newSigma = Math.Exp(A / 2.0);

            player.WorkingVolatility = newSigma;

            // Step 6
            var phiStar = CalculateNewRatingDeviation(phi, newSigma);

            // Step 7
            var newPhi = 1.0 / Math.Sqrt((1.0 / Math.Pow(phiStar, 2)) + (1.0 / v));


            // Store newly calculated rating in working area of object so we dont calculate subsequent calculations against
            // a changing rating
            player.WorkingRatingValue = (player.GetGlicko2Rating() + Math.Pow(newPhi, 2) * OutcomeBasedRating(player, results));
            player.WorkingDeviation = newPhi;
            player.IncrementNumberOfResults(results.Count);
        }


        #region Glicko Elo Calculation Static Methods
        /// <summary>
        /// Formula for step 4 of Glickmans paper
        /// </summary>
        /// <param name="player"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        private double Delta(GlickoRating player, IList<Result> results) => V(player, results) * OutcomeBasedRating(player, results);

        private static double F(double x, double delta, double phi, double v, double a,double tau)
        {
            return (Math.Exp(x) * (Math.Pow(delta,2) - Math.Pow(phi,2) - v - Math.Exp(x)) /
                (2.0 * Math.Pow(Math.Pow(phi,2) + v + Math.Exp(x), 2))) -
                ((x - a) / Math.Pow(tau , 2));
        }

        /// <summary>
        /// This is the primary function in step 3 of Glickman's paper
        /// </summary>
        /// <param name="player"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        private static double V(GlickoRating player, IEnumerable<Result> results)
        {
            var v = 0.0;

            foreach (Result result in results)
            {
                v += (
                    (
                        Math.Pow(G(result.GetOpponent(player).GetGlicko2RatingDeviation()), 2))
                        * E(player.GetGlicko2Rating(),
                        result.GetOpponent(player).GetGlicko2Rating(),
                        result.GetOpponent(player).GetGlicko2RatingDeviation())
                        * (1.0 - E(player.GetGlicko2Rating(),
                        result.GetOpponent(player).GetGlicko2Rating(),
                        result.GetOpponent(player).GetGlicko2RatingDeviation())
                    ));
            }

            return Math.Pow(v, -1);
        }

        /// <summary>
        /// First sub function of step 3 of Glickman's paper
        /// </summary>
        /// <param name="deviation"></param>
        /// <returns></returns>
        private static double G(double deviation) => 1.0 / (Math.Sqrt(1.0 + (3.0 * Math.Pow(deviation, 2) / Math.Pow(Math.PI, 2))));

        /// <summary>
        /// Second sub function of step 3 of Glickman's paper
        /// </summary>
        /// <param name="playerRating"></param>
        /// <param name="opponentRating"></param>
        /// <param name="opponentDeviation"></param>
        /// <returns></returns>
        private static double E(double playerRating, double opponentRating, double opponentDeviation) => 1.0 / (1.0 + Math.Exp(-1.0 * G(opponentDeviation) * (playerRating - opponentRating)));

        private static double OutcomeBasedRating(GlickoRating player, IEnumerable<Result> results)
        {
            double outcomeBasedRating = 0;

            foreach (var result in results)
            { 
                outcomeBasedRating += (G(result.GetOpponent(player).GetGlicko2RatingDeviation())
                    * (result.GetScore(player) - E(
                        player.GetGlicko2Rating(),
                        result.GetOpponent(player).GetGlicko2Rating(),
                        result.GetOpponent(player).GetGlicko2RatingDeviation())));  
            }

            return outcomeBasedRating;
        }

        private static double CalculateNewRatingDeviation(double phi, double sigma) => Math.Sqrt(Math.Pow(phi, 2) + Math.Pow(sigma, 2));
        #endregion Glicko Elo Calculation Static Methods

        #region Adjustment Methods
        public double ConvertRatingToOriginalGlickoScale(double rating) => ((rating * RATING_DEFAULTS.MULTIPLIER) + RATING_DEFAULTS.RATING);
        public double ConvertRatingToGlicko2Scale(double rating) => ((rating - RATING_DEFAULTS.RATING) /  RATING_DEFAULTS.MULTIPLIER);
        public double ConvertRatingDeviationToOriginalGlickoScale(double ratingDeviation) => (ratingDeviation * RATING_DEFAULTS.MULTIPLIER);
        public double ConvertRatingDeviationToGlicko2Scale(double ratingDeviation) => (ratingDeviation  / RATING_DEFAULTS.MULTIPLIER);
        #endregion Adjustment Methods
    }
}
