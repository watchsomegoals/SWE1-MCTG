using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG
{
    public class Monster : ICard
    {
        private string _id;
        private string _name;
        private double _damage;
        private ElementType _elementType;
        private CardType _cardType;
        private bool _isDead;
        private MonsterType _monsterType;

        public Monster(string id, string name, double damage, ElementType elementType, MonsterType monsterType)
        {
            _id = id;
            _name = name;
            _damage = damage;
            _elementType = elementType;
            _monsterType = monsterType;
            _cardType = CardType.monster;
            _isDead = false;
        }
        public double DoubleDamage() => 2 * Damage;
        public double HalveDamage() => Damage / 2;
        public int Attack (ICard defender)
        {
            double damageValueAttacker = 0;
            double damageValueDefender = defender.Damage;

            if (defender.CardType == CardType.spell)
            {
                if (MonsterType == MonsterType.kraken)
                {
                    damageValueAttacker = Damage;
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
                    if (MonsterType == MonsterType.knight)
                    {
                        return -1;
                    }
                    else if (ElementType == ElementType.water)
                    {
                        damageValueAttacker = Damage;
                    }
                    else if (ElementType == ElementType.fire)
                    {
                        damageValueAttacker = HalveDamage();
                    }
                    else if (ElementType == ElementType.normal)
                    {
                        damageValueAttacker = DoubleDamage();
                    }
                }
                else if (defender.ElementType == ElementType.normal)
                {
                    if (ElementType == ElementType.normal)
                    {
                        damageValueAttacker = Damage;
                    }
                    else if (ElementType == ElementType.water)
                    {
                        damageValueAttacker = HalveDamage();
                    }
                    else if (ElementType == ElementType.fire)
                    {
                        damageValueAttacker = DoubleDamage();
                    }
                }
            }
            else if (defender.CardType == CardType.monster)
            {
                if (defender.MonsterType == MonsterType.wizard && MonsterType == MonsterType.ork)
                {
                    return 0;
                }
                else if (defender.MonsterType == MonsterType.dragon && MonsterType == MonsterType.goblin)
                {
                    return 0;
                }
                else if (defender.MonsterType == MonsterType.elf && MonsterType == MonsterType.dragon)
                {
                    return 0;
                }
                else
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
        public double Damage
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
