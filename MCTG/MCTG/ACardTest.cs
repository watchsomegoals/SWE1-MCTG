using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG
{
    [TestFixture]
    class ACardTest
    {
        //arrange
        ElementType testType;

        [Test]
        [Category("pass")]
        [TestCase(1)]
        public void GetRandomElementTypeFire(int a)
        {
            //act
            testType = ACard.GetRandomElementType(a);

            //verify
            Assert.AreEqual(ElementType.fire, testType);
        }

        [Test]
        [Category("pass")]
        [TestCase(2)]
        public void GetRandomElementTypeWater(int a)
        {
            //act
            testType = ACard.GetRandomElementType(a);

            //verify
            Assert.AreEqual(ElementType.water, testType);
        }

        [Test]
        [Category("pass")]
        [TestCase(3)]
        public void GetRandomElementTypeNormal(int a)
        {
            //act
            testType = ACard.GetRandomElementType(a);

            //verify
            Assert.AreEqual(ElementType.normal, testType);
        }

        [Test]
        [Category("fail")]
        [TestCase(4)]
        public void GetRandomElementTypeFail(int a)
        {
            //act
            //testType = ACard.GetRandomElementType(a);

            //verify
            var ex = Assert.Throws<ArgumentException>(() => ACard.GetRandomElementType(a));
            Assert.That(ex.Message, Is.EqualTo("Argument is not between 1 and 3!"));
        }
    }
}
