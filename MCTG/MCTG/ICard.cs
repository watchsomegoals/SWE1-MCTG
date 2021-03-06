﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ElementType
{
    fire = 1,
    water,
    normal
}
public enum CardType
{
    monster = 1,
    spell
}
public enum MonsterType
{
    def = 0,
    dragon,
    elf,
    goblin,
    ork,
    wizard,
    knight,
    kraken
}

namespace MCTG
{
    public interface ICard
    {
        string Id { get; set; }
        string Name { get; set; }
        double Damage { get; set; }
        ElementType ElementType { get; set; }
        CardType CardType { get; set; }
        MonsterType MonsterType { get; set; }
        double DoubleDamage();
        double HalveDamage();
    }
}
