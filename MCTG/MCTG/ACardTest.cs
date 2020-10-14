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
        ElementType testType;

        [Test]
        [Category("pass")]
        public void getRandomElementType()
        {
            testType = ACard.getRandomElementType(1);

            Assert.AreEqual(ElementType.fire, testType);
        }
    }
}
