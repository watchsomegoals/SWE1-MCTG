using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG
{
    class Spell : ICard
    {
        private string _id;
        private string _name;
        private float _damage;
        private ElementType _elementType;
        private CardType _cardType;
        private bool _isDead;
        private MonsterType _monsterType;

        public Spell(string id, string name, float damage, ElementType elementType)
        {
            _id = id;
            _name = name;
            _damage = damage;
            _elementType = elementType;
            _cardType = CardType.spell;
            _isDead = false;
            _monsterType = MonsterType.def;
        }
        public float DoubleDamage() => 2 * Damage;
        public float HalveDamage() => Damage / 2;
        public int Attack(ICard defender)
        {
            float damageValueAttacker = 0;
            float damageValueDefender = defender.Damage;

            if (defender.MonsterType == MonsterType.knight && ElementType == ElementType.water)
            {
                return 1;
            }
            else if (defender.MonsterType == MonsterType.kraken)
            {
                return 0;
            }
            else if (defender.ElementType == ElementType.fire)
            {
                if (ElementType == ElementType.fire)
                {
                    damageValueAttacker = Damage;
                }
                else if (ElementType == ElementType.water)
                {
                    damageValueAttacker = DoubleDamage();
                }
                else if (ElementType == ElementType.normal)
                {
                    damageValueAttacker = HalveDamage();
                }
            }
            else if (defender.ElementType == ElementType.water)
            {
                if (ElementType == ElementType.fire)
                {
                    damageValueAttacker = HalveDamage();
                }
                else if (ElementType == ElementType.water)
                {
                    damageValueAttacker = Damage;
                }
                else if (ElementType == ElementType.normal)
                {
                    damageValueAttacker = DoubleDamage();
                }
            }
            else if (defender.ElementType == ElementType.normal)
            {
                if (ElementType == ElementType.fire)
                {
                    damageValueAttacker = DoubleDamage();
                }
                else if (ElementType == ElementType.water)
                {
                    damageValueAttacker = HalveDamage();
                }
                else if (ElementType == ElementType.normal)
                {
                    damageValueAttacker = Damage;
                }
            }

            if (damageValueAttacker > damageValueDefender)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public string Id
        {
            get => _id;
            set => _id = value;
        }
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        public float Damage
        {
            get => _damage;
            set => _damage = value;
        }
        public ElementType ElementType
        {
            get => _elementType;
            set => _elementType = value;
        }
        public CardType CardType
        {
            get => _cardType;
            set => _cardType = value;
        }
        public bool IsDead
        {
            get => _isDead;
            set => _isDead = value;
        }
        public MonsterType MonsterType
        {
            get => _monsterType;
            set => _monsterType = value;
        }
    }
}
