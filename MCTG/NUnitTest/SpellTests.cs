using NUnit.Framework;
using MCTG;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTest
{
    class SpellTests
    {
        Spell spell;
        double result;
        [Test]
        [Category("pass")]
        public void DoubleDamageSpellTest()
        {
            //arrange
            spell = new Spell("1", "BlueEyes", 40.0, ElementType.normal);
            //act
            result = spell.DoubleDamage();
            //assert
            Assert.AreEqual(80.0, result);
        }

        [Test]
        [Category("pass")]
        public void HalveDamageSpellTest()
        {
            //arrange
            spell = new Spell("1", "BlueEyes", 40.0, ElementType.normal);
            //act
            result = spell.HalveDamage();
            //assert
            Assert.AreEqual(20.0, result);
        }
    }
}
