using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MCTG
{
    public static class Arena
    {
        public static List<User> players = new List<User>();
        public static string winneruser;
        public static string loseruser;
        public static int rounds = 1;
        public static string log = null;
        public static ManualResetEvent Restart { get; } = new ManualResetEvent(false);

        public static void PrepareFight(User user)
        {
            players.Add(user);
            if(players.Count == 2)
            {
                battle(players[0], players[1]);
                players.RemoveAt(1);
                players.RemoveAt(0);
                DatabaseManager mycon = new DatabaseManager();
                mycon.ChangeScoreAndStatsIfLost(Arena.LoserUser);
                mycon.ChangeScoreAndStatsIfWon(Arena.WinnerUser);
            }
        }
        public static void battle(User user1, User user2)
        {
            double damage1;
            double damage2;
            RequestContext req = new RequestContext();
            string element;
            string card = null;

            while (rounds <= 100)
            {
                if (user1.Deck.Count == 0)
                {
                    break;
                }
                else if (user2.Deck.Count == 0)
                {
                    break;
                }

                log += rounds + ".Round\n";
                user1.ShuffleDeck();
                user2.ShuffleDeck();
                log += user1.Username + ": " + user1.Deck[0].Name + " (" + user1.Deck[0].Damage + ") vs " + user2.Username + ": " + user2.Deck[0].Name + " (" + user2.Deck[0].Damage + ") => " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage;
       
                if (user1.Deck[0] is Monster && user2.Deck[0] is Monster)
                {
                    if(user1.Deck[0].MonsterType == MonsterType.wizard && user2.Deck[0].MonsterType == MonsterType.ork)
                    {
                        if (user1.Deck[0].Damage > user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0); 
                        }
                        else
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => draw\n";
                        }
                    }
                    else if(user2.Deck[0].MonsterType == MonsterType.wizard && user1.Deck[0].MonsterType == MonsterType.ork)
                    {
                        if (user2.Deck[0].Damage > user1.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => draw\n";
                        }
                    }
                    else if(user1.Deck[0].MonsterType == MonsterType.goblin && user2.Deck[0].MonsterType == MonsterType.dragon)
                    {
                        if (user2.Deck[0].Damage > user1.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => draw\n";
                        }
                    }
                    else if (user2.Deck[0].MonsterType == MonsterType.goblin && user1.Deck[0].MonsterType == MonsterType.dragon)
                    {
                        if (user1.Deck[0].Damage > user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => draw\n";
                        }
                    }
                    else if (user1.Deck[0].MonsterType == MonsterType.elf && user2.Deck[0].MonsterType == MonsterType.dragon)
                    {
                        if(user1.Deck[0].ElementType == ElementType.fire)
                        {
                            if (user1.Deck[0].Damage > user2.Deck[0].Damage)
                            {
                                log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user1.Deck[0].Name + " wins\n";
                                element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                                card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                                user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                                user2.Deck.RemoveAt(0);
                            }
                            else
                            {
                                log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => draw\n";
                            }
                        }
                    }
                    else if (user2.Deck[0].MonsterType == MonsterType.elf && user1.Deck[0].MonsterType == MonsterType.dragon)
                    {
                        if (user2.Deck[0].ElementType == ElementType.fire)
                        {
                            if (user2.Deck[0].Damage > user1.Deck[0].Damage)
                            {
                                log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user2.Deck[0].Name + " wins\n";
                                element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                                card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                                user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                                user1.Deck.RemoveAt(0);
                            }
                            else
                            {
                                log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => draw\n";
                            }
                        }
                    }
                    else
                    {
                        if (user1.Deck[0].Damage > user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (user1.Deck[0].Damage < user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (user1.Deck[0].Damage == user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => draw\n";
                        }
                    }
                }
 //***************************************************************************************************************************************
                else if(user1.Deck[0] is Spell && user2.Deck[0] is Spell)
                {
                    if(user1.Deck[0].ElementType == ElementType.fire && user2.Deck[0].ElementType == ElementType.water)
                    {
                        damage1 = user1.Deck[0].HalveDamage();
                        damage2 = user2.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if(user2.Deck[0].ElementType == ElementType.fire && user1.Deck[0].ElementType == ElementType.water)
                    {
                        damage1 = user2.Deck[0].HalveDamage();
                        damage2 = user1.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user1.Deck[0].ElementType == ElementType.water && user2.Deck[0].ElementType == ElementType.normal)
                    {
                        damage1 = user1.Deck[0].HalveDamage();
                        damage2 = user2.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user2.Deck[0].ElementType == ElementType.water && user1.Deck[0].ElementType == ElementType.normal)
                    {
                        damage1 = user2.Deck[0].HalveDamage();
                        damage2 = user1.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user1.Deck[0].ElementType == ElementType.normal && user2.Deck[0].ElementType == ElementType.fire)
                    {
                        damage1 = user1.Deck[0].HalveDamage();
                        damage2 = user2.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user2.Deck[0].ElementType == ElementType.normal && user1.Deck[0].ElementType == ElementType.fire)
                    {
                        damage1 = user2.Deck[0].HalveDamage();
                        damage2 = user1.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user2.Deck[0].ElementType == user1.Deck[0].ElementType)
                    {
                        if (user1.Deck[0].Damage > user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (user1.Deck[0].Damage < user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (user1.Deck[0].Damage == user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => draw\n";
                        }
                    }
                }
 //***************************************************************************************************************************************
                else if(user1.Deck[0] is Monster && user2.Deck[0] is Spell)
                {
                    if(user1.Deck[0].MonsterType == MonsterType.knight && user2.Deck[0].ElementType == ElementType.water)
                    {
                        log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user2.Deck[0].Name + " wins\n";
                        element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                        card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                        user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                        user1.Deck.RemoveAt(0);
                    }
                    else if (user1.Deck[0].MonsterType == MonsterType.kraken)
                    {
                        log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user1.Deck[0].Name + " wins\n";
                        element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                        card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                        user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                        user2.Deck.RemoveAt(0);
                    }
                    else if (user1.Deck[0].ElementType == ElementType.fire && user1.Deck[0].MonsterType != MonsterType.knight && user2.Deck[0].ElementType == ElementType.water && user1.Deck[0].MonsterType != MonsterType.kraken)
                    {
                        damage1 = user1.Deck[0].HalveDamage();
                        damage2 = user2.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user2.Deck[0].ElementType == ElementType.fire && user1.Deck[0].ElementType == ElementType.water && user1.Deck[0].MonsterType != MonsterType.kraken)
                    {
                        damage1 = user2.Deck[0].HalveDamage();
                        damage2 = user1.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user1.Deck[0].ElementType == ElementType.water && user2.Deck[0].ElementType == ElementType.normal && user1.Deck[0].MonsterType != MonsterType.kraken)
                    {
                        damage1 = user1.Deck[0].HalveDamage();
                        damage2 = user2.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user2.Deck[0].ElementType == ElementType.water && user1.Deck[0].ElementType == ElementType.normal && user1.Deck[0].MonsterType != MonsterType.kraken && user1.Deck[0].MonsterType != MonsterType.knight)
                    {
                        damage1 = user2.Deck[0].HalveDamage();
                        damage2 = user1.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user1.Deck[0].ElementType == ElementType.normal && user2.Deck[0].ElementType == ElementType.fire && user1.Deck[0].MonsterType != MonsterType.kraken)
                    {
                        damage1 = user1.Deck[0].HalveDamage();
                        damage2 = user2.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user2.Deck[0].ElementType == ElementType.normal && user1.Deck[0].ElementType == ElementType.fire && user1.Deck[0].MonsterType != MonsterType.kraken)
                    {
                        damage1 = user2.Deck[0].HalveDamage();
                        damage2 = user1.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user2.Deck[0].ElementType == user1.Deck[0].ElementType && user1.Deck[0].MonsterType != MonsterType.kraken)
                    {
                        if (user1.Deck[0].Damage > user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (user1.Deck[0].Damage < user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (user1.Deck[0].Damage == user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => draw\n";
                        }
                    }
                }
//****************************************************************************************************************************************
                else if (user2.Deck[0] is Monster && user1.Deck[0] is Spell)
                {
                    if (user2.Deck[0].MonsterType == MonsterType.knight && user1.Deck[0].ElementType == ElementType.water)
                    {
                        log += " -> " + user1.Deck[0].Damage + " VS " + user1.Deck[0].Damage + " => " + user1.Deck[0].Name + " wins\n";
                        element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                        card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                        user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                        user2.Deck.RemoveAt(0);
                    }
                    else if (user2.Deck[0].MonsterType == MonsterType.kraken)
                    {
                        log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user2.Deck[0].Name + " wins\n";
                        element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                        card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                        user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                        user1.Deck.RemoveAt(0);
                    }
                    else if (user2.Deck[0].ElementType == ElementType.fire && user2.Deck[0].MonsterType != MonsterType.knight && user1.Deck[0].ElementType == ElementType.water && user2.Deck[0].MonsterType != MonsterType.kraken)
                    {
                        damage1 = user2.Deck[0].HalveDamage();
                        damage2 = user1.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user1.Deck[0].ElementType == ElementType.fire && user2.Deck[0].ElementType == ElementType.water && user2.Deck[0].MonsterType != MonsterType.kraken)
                    {
                        damage1 = user1.Deck[0].HalveDamage();
                        damage2 = user2.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user2.Deck[0].ElementType == ElementType.water && user1.Deck[0].ElementType == ElementType.normal && user2.Deck[0].MonsterType != MonsterType.kraken)
                    {
                        damage1 = user2.Deck[0].HalveDamage();
                        damage2 = user1.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user1.Deck[0].ElementType == ElementType.water && user2.Deck[0].ElementType == ElementType.normal && user2.Deck[0].MonsterType != MonsterType.kraken && user2.Deck[0].MonsterType != MonsterType.knight)
                    {
                        damage1 = user1.Deck[0].HalveDamage();
                        damage2 = user2.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user2.Deck[0].ElementType == ElementType.normal && user1.Deck[0].ElementType == ElementType.fire && user2.Deck[0].MonsterType != MonsterType.kraken)
                    {
                        damage1 = user2.Deck[0].HalveDamage();
                        damage2 = user1.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user1.Deck[0].ElementType == ElementType.normal && user2.Deck[0].ElementType == ElementType.fire && user2.Deck[0].MonsterType != MonsterType.kraken)
                    {
                        damage1 = user1.Deck[0].HalveDamage();
                        damage2 = user2.Deck[0].DoubleDamage();
                        if (damage1 > damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (damage1 < damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (damage1 == damage2)
                        {
                            log += " -> " + damage1 + " VS " + damage2 + " => draw\n";
                        }
                    }
                    else if (user2.Deck[0].ElementType == user1.Deck[0].ElementType && user1.Deck[0].MonsterType != MonsterType.kraken)
                    {
                        if (user1.Deck[0].Damage > user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user1.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user2.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user2.Deck[0].MonsterType);
                            user1.AddCardToDeckOfUser(user2.Deck[0].Id, user2.Deck[0].Name, user2.Deck[0].Damage, element, card);
                            user2.Deck.RemoveAt(0);
                        }
                        else if (user1.Deck[0].Damage < user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => " + user2.Deck[0].Name + " wins\n";
                            element = req.GetElementTypeFromEnum(user1.Deck[0].ElementType);
                            card = req.GetMonsterTypeFromEnum(user1.Deck[0].MonsterType);
                            user2.AddCardToDeckOfUser(user1.Deck[0].Id, user1.Deck[0].Name, user1.Deck[0].Damage, element, card);
                            user1.Deck.RemoveAt(0);
                        }
                        else if (user1.Deck[0].Damage == user2.Deck[0].Damage)
                        {
                            log += " -> " + user1.Deck[0].Damage + " VS " + user2.Deck[0].Damage + " => draw\n";
                        }
                    }
                }

                rounds++;
            }
            if (user1.Deck.Count == 0)
            {
                winneruser = user2.Username;
                loseruser = user1.Username;
            }
            else if (user2.Deck.Count == 0)
            {
                winneruser = user1.Username;
                loseruser = user2.Username;
            }

            if (rounds == 101)
            {
                log += "Match ended in a draw\n";
            }
            
            log += winneruser + " wins!\n";
            Restart.Set();
        }
        public static string WinnerUser
        {
            get { return winneruser; }
            set { winneruser = value; }
        }
        public static string LoserUser
        {
            get { return loseruser; }
            set { loseruser = value; }
        }
    }
}
