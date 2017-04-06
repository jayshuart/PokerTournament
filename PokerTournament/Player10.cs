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
            

            return new PlayerAction(Name, "Bet1", "bet", 1);
        }

        public override PlayerAction BettingRound2(List<PlayerAction> actions, Card[] hand)
        {
            throw new NotImplementedException();
            //review actions of other player
                //how much did they bet in round 1?

                //draw? - how many?
                    //1
                    //2
                    //3
                    //4+
                        //they dont have shit

                //call?

                //bet/raise?
                    //how much?

                    //how many times has this happened?
                        //twice means

            //evaluate hand
                //fold if really weak

                //bluff for 2+

                //bet
        }

        public override PlayerAction Draw(Card[] hand)
        {
            throw new NotImplementedException();
        }

        #region Helper Methods
        private void ReviewActions(List<PlayerAction> actions)
        {
            //review other players actions
            foreach (PlayerAction act in actions)
            {
                //only check other players acts
                if (act.Name != this.Name)
                {
                    //divide phases by bet or draw
                    if (act.ActionPhase == "draw") //draw
                    {
                        //check action name (stand pat, or draw)
                        if (act.ActionName == "stand pat") //not taking any cards
                        {
                            //prolly has solid hand if they dont want to change out cards
                        }
                        else //draw
                        {
                            //switch based on how many cards are to be drawn
                            switch (act.Amount)
                            {
                                case 1:  //1, 2, 3 cards measn they prolly have somthing- average
                                    break;
                                case 2:
                                    break;
                                case 3: //most likely atleast have a pair
                                    break;
                                case 4: //4+ cards mean they dont have anything
                                case 5:
                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                    else //bet 1 or 2, should be same checks
                    {
                        //how much was bet?

                        //how many times in a row have they bet? if its 2+ might mean good hand

                    }

                }
            }
        }
        #endregion
    }
}
