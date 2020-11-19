using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG
{
    abstract class ACard
    {
        private static readonly Random _random = new Random();
        protected CardType cardType;
        protected ElementType elemType;
        protected MonsterType monsterType;
        protected int damageValue;
        protected bool dead;
  
        public ACard(CardType _cardType, MonsterType _monsterType)
        {
            this.elemType = GetRandomElementType(_random.Next(1, 4));
            this.damageValue = _random.Next(0, 101);
            this.dead = false;
            this.cardType = _cardType;
            this.monsterType = _monsterType;
        }

        public static ElementType GetRandomElementType(int number)
        {
            if (number == 1)
            {
                return ElementType.fire;
            }
            else if (number == 2)
            {
                return ElementType.water;
            }
            else if (number == 3)
            {
                return ElementType.normal;
            }
            else
            {
                throw new ArgumentException("Argument is not between 1 and 3!");
            }
        }
        public int DamageValue
        {
            get { return damageValue; }
            set { damageValue = value; }
        }
        public ElementType ElemType
        {
            get { return elemType; }
        }
        public bool Dead
        {
            get { return dead; }
            set { dead = value; }
        }
        public CardType CType
        {
            get { return cardType; }
        }
        public MonsterType MType
        {
            get { return monsterType; }
        }
        public abstract int Attack(ACard other);
        //return 1 if attack was successful
        //return 0 if attack wasn't successful
        public int NormalAttack(int att, int def)
        {
            if(att > def)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
