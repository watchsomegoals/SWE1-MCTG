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
    class RequestContextTest
    {
        [Test]
        [Category("pass")]
        [TestCase("POST /messages/1 HTTP/1.1\r\n" +
                  "HOST: LOCALHOST:13000\r\n" +
                  "USER-AGENT: CURL/7.55.1\r\n" +
                  "ACCEPT: */*\r\n" +
                  "CONTENT-LENGTH: 23\r\n" +
                  "CONTENT-TYPE: APPLICATION/X-WWW-FORM-URLENCODED\r\n\r\n" +
                  "the payload is here")]
        
        public void ReadContextTestPost(string data)
        {
            //arrange
            RequestContext context = new RequestContext();
            //act
            context.ReadContext(data);
            //assert
            Assert.AreEqual("POST", context.HttpVerb);
            Assert.AreEqual("messages", context.DirName);
            Assert.AreEqual("1", context.ResourceID);
            Assert.AreEqual("HTTP/1.1", context.Protocol);
            Assert.AreEqual("the payload is here", context.Payload);
        }

        [Test]
        [Category("pass")]
        [TestCase("GET /messages/1 HTTP/1.1\r\n" +
                  "HOST: LOCALHOST:13000\r\n" +
                  "USER-AGENT: CURL/7.55.1\r\n" +
                  "ACCEPT: */*\r\n" +
                  "CONTENT-LENGTH: 23\r\n" +
                  "CONTENT-TYPE: APPLICATION/X-WWW-FORM-URLENCODED\r\n\r\n")]
        public void ReadContextTestGet(string data)
        {
            //arrange
            RequestContext context = new RequestContext();
            //act
            context.ReadContext(data);
            //assert
            Assert.AreEqual("GET", context.HttpVerb);
            Assert.AreEqual("messages", context.DirName);
            Assert.AreEqual("1", context.ResourceID);
            Assert.AreEqual("HTTP/1.1", context.Protocol);
            Assert.AreEqual(null, context.Payload);
        }

        [Test]
        [Category("pass")]
        [TestCase("DELETE /messages/1 HTTP/1.1\r\n" +
                  "HOST: LOCALHOST:13000\r\n" +
                  "USER-AGENT: CURL/7.55.1\r\n" +
                  "ACCEPT: */*\r\n" +
                  "CONTENT-LENGTH: 23\r\n" +
                  "CONTENT-TYPE: APPLICATION/X-WWW-FORM-URLENCODED\r\n\r\n")]
        public void ReadContextTestDelete(string data)
        {
            //arrange
            RequestContext context = new RequestContext();
            //act
            context.ReadContext(data);
            //assert
            Assert.AreEqual("DELETE", context.HttpVerb);
            Assert.AreEqual("messages", context.DirName);
            Assert.AreEqual("1", context.ResourceID);
            Assert.AreEqual("HTTP/1.1", context.Protocol);
            Assert.AreEqual(null, context.Payload);
        }

        [Test]
        [Category("pass")]
        public void ComposeResponseTest()
        {
            string responseActual;
            string response = "HTTP/1.1 200 OK\r\n" +
                              "Server: Caraba\r\n" +
                              "Content-Type: text/html\r\n" +
                              "Accept-Ranges: bytes\r\n" +
                              "Content-Length: 2\r\n" +
                              "\r\n" +
                              "\n1" +
                              "\r\n\r\n";
            //arrange
            RequestContext context = new RequestContext
            {
                Protocol = "HTTP/1.1",
                StatusCode = "200",
                ReasonPhrase = "OK",
                ResponseBody = "\n1"
            };
            //act
            responseActual = context.ComposeResponse();
            //assert
            Assert.AreEqual(response, responseActual);
        }
        
    }
}
