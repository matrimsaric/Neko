using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlickoDomain.Model
{
    public class Result
    {
        private const double PointsForWin = 1.0;
        private const double PointsForLoss = 0.0;
        private const double PointsForDraw = 0.5;

        private readonly bool isDraw;
        private readonly GlickoRating winner;
        private readonly GlickoRating loser;

        public Result(GlickoRating newWinner, GlickoRating newLoser, bool newIsDraw = false)
        {
            if (newWinner == null || newLoser == null) throw new ArgumentNullException("Players records are blank!");
#pragma warning disable CS8604 // Possible null reference argument. Covered by previous null check
            if (!ValidPlayer(newWinner, newLoser)) throw new ArgumentException("Players winner and loser are the same");
#pragma warning restore CS8604 // Possible null reference argument.
            winner = newWinner;
            loser = newLoser;
            isDraw = newIsDraw;
        }

        private static bool ValidPlayer(GlickoRating player1, GlickoRating player2) => player1.PlayerId != player2.PlayerId;// TODO no id in rating. Think this may be needed

        public bool Participated(GlickoRating player) => player == winner || player == loser;

        public double GetScore(GlickoRating player)
        {
            double score;

            if (winner == player) score = PointsForWin;
            else if (loser == player) score = PointsForLoss;
            else throw new ArgumentException("Player did not participate in match", "player");
            if(isDraw) score = PointsForDraw;

            return score;
        }

        public GlickoRating GetOpponent(GlickoRating player)
        {
            GlickoRating opponent;

            if (winner == player) opponent = loser;
            else if (loser == player) opponent = winner;
            else throw new ArgumentException("Player did not participate in match", "player");
            return opponent;
        }

        public GlickoRating GetWinner() => winner;
        public GlickoRating GetLoser() => loser;
    }
}
