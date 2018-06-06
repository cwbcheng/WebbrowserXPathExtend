using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebbrowserXPathExtend;

namespace WebbrowserXPathUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestStaticXPath()
        {
            WebBrowser webBrowser = new WebBrowser();
            webBrowser.DocumentCompleted += delegate
            {
                var elements = webBrowser.Document.Body
                    .FindElements("//*[@id='su']")
                    .ToArray();
                Assert.AreEqual(elements.Count(), 1);
                var soutu_btnEle = webBrowser.Document.Body
                    .FindElements("//*[className='soutu-btn']")
                    .ToArray();
                Assert.AreEqual(soutu_btnEle.Count(), 1);
            };
            webBrowser.Navigate(@"https://www.baidu.com");
        }

        [TestMethod]
        public void TestFramesXPath()
        {
            WebBrowser webBrowser = new WebBrowser();
            string pathStr = Path.Combine(Environment.CurrentDirectory, "FramesTestPage.html");
            webBrowser.Navigate(pathStr);

            while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            var elements = webBrowser.Document.Body
                    .FindElements("//div")
                    .ToArray();
            Assert.AreEqual(elements.Count(), 11);
            Assert.AreEqual(elements[4].Name, "4.1");
            Assert.AreEqual(elements[5].Name, "4.2");
            Assert.AreEqual(elements[6].Name, "5");

            var div4 = webBrowser.Document.Body
                .FindElements("//div[4]")
                .ToArray();
            var div4_divs = div4[0].FindElements("//div").ToArray();
            Assert.AreEqual(div4_divs.Count(), 2);
            Assert.AreEqual(div4_divs[0].Name, "4.1");
            Assert.AreEqual(div4_divs[1].Name, "4.2");

            var div4_1 = webBrowser.Document.Body
                .FindElements("/div[4]/div[1]")
                .ToArray();
            Assert.AreEqual(div4_1[0].Name, "4.1");

            var frameDivs = webBrowser.Document.Window.Frames[0].Document.Body
                .FindElements("//div")
                .ToArray();
            Assert.AreEqual(frameDivs.Count(), 3);

            var frameDiv2 = webBrowser.Document.Window.Frames[0].Document.Body
                .FindElements("//div[2]")
                .ToArray();
            Assert.AreEqual(frameDiv2[0].InnerText, "15");

            var inputs = frameDiv2[0].Document.Body
                .FindElements("//input")
                .ToArray();
            Assert.AreEqual(inputs.Count(), 1);

            var userNameELe = webBrowser.Document.Window.Frames[0].Document.Body
                .FindElements("/div[2]/input[1]")
                .FirstOrDefault();
            Assert.AreEqual(userNameELe.Name, "username");
        }
    }
}
