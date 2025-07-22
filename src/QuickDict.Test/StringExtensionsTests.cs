// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickDict.Test
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringExtensions_EscapeForXml_NullTest()
        {
            StringExtensions.EscapeForXml(null);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("test")]
        [DataRow("hello world")]
        [DataRow("hello world ")]
        [DataRow(" hello world")]
        [DataRow(" hello world ")]
        [DataRow(" ")]
        [DataRow("\t")]
        public void StringExtensions_EscapeForXml_NoChangeTests(string input)
        {
            Assert.AreEqual(input, input.EscapeForXml());
        }

        [TestMethod]
        [DataRow("<", "&lt;")]
        [DataRow(">", "&gt;")]
        [DataRow("&", "&amp;")]
        [DataRow("<&>", "&lt;&amp;&gt;")]
        [DataRow("<test>", "&lt;test&gt;")]
        [DataRow("<hello world>", "&lt;hello world&gt;")]
        [DataRow(">&<", "&gt;&amp;&lt;")]
        [DataRow(">test<", "&gt;test&lt;")]
        [DataRow(">hello world<", "&gt;hello world&lt;")]
        public void StringExtensions_EscapeForXml_ChangeTests(string input, string expectedOutput)
        {
            Assert.AreEqual(expectedOutput, input.EscapeForXml());
        }
    }
}