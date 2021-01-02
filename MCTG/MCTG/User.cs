using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG
{
    public class User
    {
        private static Random rng = new Random();
        private string username;
        private string password;
        private string name;
        private string bio;
        private string image;
        private List<ICard> deck;
        public User()
        {
            deck = new List<ICard>();
        }
        public User(User usercopy)
        {
            this.username = usercopy.Username;
            this.password = usercopy.Password;
            this.name = usercopy.Name;
            this.bio = usercopy.Bio;
            this.image = usercopy.Image;
            this.deck = new List<ICard>(usercopy.Deck);
        }
        public string Image
        {
            get { return image; }
            set { image = value; }
        }
        public string Bio
        {
            get { return bio; }
            set { bio = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public void AddCardToDeckOfUser(string id, string name, double damage, string elementType, string monsterType)
        {
            if(monsterType == "spell")
            {
                if(elementType == "normal")
                {
                    deck.Add(new Spell(id, name, damage, ElementType.normal));
                }
                else if(elementType == "water")
                {
                    deck.Add(new Spell(id, name, damage, ElementType.water));
                }
                else if(elementType == "fire")
                {
                    deck.Add(new Spell(id, name, damage, ElementType.fire));
                }
            }
            else
            {
                if(monsterType == "dragon")
                {
                    if(elementType == "normal")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.normal, MonsterType.dragon));
                    }
                    else if(elementType == "water")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.water, MonsterType.dragon));
                    }
                    else if(elementType == "fire")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.fire, MonsterType.dragon));
                    }
                }
                else if(monsterType == "elf")
                {
                    if (elementType == "normal")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.normal, MonsterType.elf));
                    }
                    else if (elementType == "water")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.water, MonsterType.elf));
                    }
                    else if (elementType == "fire")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.fire, MonsterType.elf));
                    }
                }
                else if (monsterType == "goblin")
                {
                    if (elementType == "normal")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.normal, MonsterType.goblin));
                    }
                    else if (elementType == "water")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.water, MonsterType.goblin));
                    }
                    else if (elementType == "fire")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.fire, MonsterType.goblin));
                    }
                }
                else if (monsterType == "ork")
                {
                    if (elementType == "normal")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.normal, MonsterType.ork));
                    }
                    else if (elementType == "water")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.water, MonsterType.ork));
                    }
                    else if (elementType == "fire")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.fire, MonsterType.ork));
                    }
                }
                else if (monsterType == "wizard")
                {
                    if (elementType == "normal")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.normal, MonsterType.wizard));
                    }
                    else if (elementType == "water")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.water, MonsterType.wizard));
                    }
                    else if (elementType == "fire")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.fire, MonsterType.wizard));
                    }
                }
                else if (monsterType == "knight")
                {
                    if (elementType == "normal")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.normal, MonsterType.knight));
                    }
                    else if (elementType == "water")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.water, MonsterType.knight));
                    }
                    else if (elementType == "fire")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.fire, MonsterType.knight));
                    }
                }
                else if (monsterType == "kraken")
                {
                    if (elementType == "normal")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.normal, MonsterType.kraken));
                    }
                    else if (elementType == "water")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.water, MonsterType.kraken));
                    }
                    else if (elementType == "fire")
                    {
                        deck.Add(new Monster(id, name, damage, ElementType.fire, MonsterType.kraken));
                    }
                }
            }
        }
        public List<ICard> Deck
        {
            set { deck = value; }
            get { return deck; }
        }
        public void ShuffleDeck()
        {
            int n = Deck.Count();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                ICard value = Deck[k];
                Deck[k] = Deck[n];
                Deck[n] = value;
            }
        }
    }
}
