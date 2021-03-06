﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG
{
    public class Spell : ICard
    {
        private string _id;
        private string _name;
        private double _damage;
        private ElementType _elementType;
        private CardType _cardType;
        private MonsterType _monsterType;

        public Spell(string id, string name, double damage, ElementType elementType)
        {
            _id = id;
            _name = name;
            _damage = damage;
            _elementType = elementType;
            _cardType = CardType.spell;
            _monsterType = MonsterType.def;
        }
        public double DoubleDamage() => 2 * Damage;
        public double HalveDamage() => Damage / 2;
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
        public MonsterType MonsterType
        {
            get => _monsterType;
            set => _monsterType = value;
        }
    }
}
