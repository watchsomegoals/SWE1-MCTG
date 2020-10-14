using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ElementType 
{   
    fire = 1,
    water = 2,
    normal = 3
}

namespace MCTG
{
    abstract class ACard
    {
        private static readonly Random _random = new Random();
        protected int damageValue;
        protected ElementType type;

        public ACard()
        {
            this.type = getRandomElementType(_random.Next(1, 4));
            this.damageValue = _random.Next(0, 101);
        }

        public static ElementType getRandomElementType(int number)
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
        }

        public ElementType Type
        {
            get { return type; }
        }
    }
}
