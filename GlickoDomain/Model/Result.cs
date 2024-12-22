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
        private readonly Rating winner;
        private readonly Rating loser;

        public Result(Rating newWinner, Rating newLoser, bool newIsDraw = false)
        {
            if (newWinner == null || newLoser == null) throw new ArgumentNullException("Players records are blank!");
#pragma warning disable CS8604 // Possible null reference argument. Covered by previous null check
            if (!ValidPlayer(winner, loser)) throw new ArgumentException("Players winner and loser are the same");
#pragma warning restore CS8604 // Possible null reference argument.
            winner = newWinner;
            loser = newLoser;
            isDraw = newIsDraw;
        }

        private static bool ValidPlayer(Rating player1, Rating player2) => player1 != player2;// TODO no id in rating. Think this may be needed

        public bool Participated(Rating player) => player == winner || player == loser;

        public double GetScore(Rating player)
        {
            double score;

            if (winner == player) score = PointsForWin;
            else if (loser == player) score = PointsForLoss;
            else throw new ArgumentException("Player did not participate in match", "player");
            if(isDraw) score = PointsForDraw;

            return score;
        }

        public Rating GetOpponent(Rating player)
        {
            Rating opponent;

            if (winner == player) opponent = loser;
            else if (loser == player) opponent = winner;
            else throw new ArgumentException("Player did not participate in match", "player");
            return opponent;
        }

        public Rating GetWinner() => winner;
        public Rating GetLoser() => loser;
    }
}
