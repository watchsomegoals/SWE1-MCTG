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
    [TestFixture]
    class MonsterTests
    {
        Monster monster;
        double result;
        [Test]
        [Category("pass")]
        public void DoubleDamageMonsterTest()
        {
            //arrange
            monster = new Monster("1", "BlueEyes", 40.0, ElementType.normal, MonsterType.dragon);
            //act
            result = monster.DoubleDamage();
            //assert
            Assert.AreEqual(80.0, result);
        }
        
        [Test]
        [Category("pass")]
        public void HalveDamageMonsterTest()
        {
            //arrange
            monster = new Monster("1", "BlueEyes", 40.0, ElementType.normal, MonsterType.dragon);
            //act
            result = monster.HalveDamage();
            //assert
            Assert.AreEqual(20.0, result);
        }
    }
}