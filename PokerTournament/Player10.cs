using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerTournament
{
    /// <summary>
    /// Team 10 Poker Player
    /// Kevin Idzik, Aiden Melendez, Joel Shuart
    /// </summary>
    class Player10 : Player
    {
        /// <summary>
        /// Player10 Constructor
        /// </summary>
        /// <param name="idNum">Player ID number</param>
        /// <param name="nm">Player name</param>
        /// <param name="mny">Amount of money the player has</param>
        public Player10(int idNum, string nm, int mny) : base(idNum, nm, mny)
        {
        }

        public override PlayerAction BettingRound1(List<PlayerAction> actions, Card[] hand)
        {
            throw new NotImplementedException();
        }

        public override PlayerAction BettingRound2(List<PlayerAction> actions, Card[] hand)
        {
            throw new NotImplementedException();
        }

        public override PlayerAction Draw(Card[] hand)
        {
            throw new NotImplementedException();
        }
    }
}
