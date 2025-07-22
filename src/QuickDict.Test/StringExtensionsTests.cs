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
        public void StringExtensions_EscapeForXml_NoChangeTest(string input)
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
        [DataRow("<&> ", "&lt;&amp;&gt; ")]
        [DataRow("<test> ", "&lt;test&gt; ")]
        [DataRow("<hello world> ", "&lt;hello world&gt; ")]
        [DataRow(" <&>", " &lt;&amp;&gt;")]
        [DataRow(" <test>", " &lt;test&gt;")]
        [DataRow(" <hello world>", " &lt;hello world&gt;")]
        public void StringExtensions_EscapeForXml_ChangeTest(string input, string expectedOutput)
        {
            Assert.AreEqual(expectedOutput, input.EscapeForXml());
        }

        [TestMethod]
        [DataRow(null, "tag")]
        [DataRow("test", null)]
        [DataRow("test", "")]
        [DataRow("test", " ")]
        [DataRow("test", "\t")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringExtensions_WrapInTag_NullTest(string? s, string? tag)
        {
            StringExtensions.WrapInTag(s, tag);
        }

        [TestMethod]
        [DataRow("", "tag", "<tag></tag>")]
        [DataRow("", "tag ", "<tag></tag>")]
        [DataRow("", " tag ", "<tag></tag>")]
        [DataRow(" ", "tag", "<tag> </tag>")]
        [DataRow(" ", "tag ", "<tag> </tag>")]
        [DataRow(" ", " tag ", "<tag> </tag>")]
        [DataRow("test", "tag", "<tag>test</tag>")]
        [DataRow("test", "tag ", "<tag>test</tag>")]
        [DataRow("test", " tag ", "<tag>test</tag>")]
        [DataRow("hello world", "tag", "<tag>hello world</tag>")]
        [DataRow("hello world", "tag ", "<tag>hello world</tag>")]
        [DataRow("hello world", " tag ", "<tag>hello world</tag>")]
        [DataRow("test ", "tag", "<tag>test </tag>")]
        [DataRow(" test", "tag", "<tag> test</tag>")]
        [DataRow(" test ", " tag", "<tag> test </tag>")]
        public void StringExtensions_WrapInTag_ChangeTest(string s, string tag, string expectedOutput)
        {
            Assert.AreEqual(expectedOutput, s.WrapInTag(tag));
        }

        [TestMethod]
        [DataRow("", "target", "tag", StringWrapInTagOptions.None, "")]
        [DataRow("target test", "target", "tag", StringWrapInTagOptions.None, "<tag>target</tag> test")]
        [DataRow("test target", "target", "tag", StringWrapInTagOptions.None, "test <tag>target</tag>")]
        [DataRow("test target test", "target", "tag", StringWrapInTagOptions.None, "test <tag>target</tag> test")]
        [DataRow("target test", "target", "tag", StringWrapInTagOptions.WrapWholeWordsOnly, "<tag>target</tag> test")]
        [DataRow("test target", "target", "tag", StringWrapInTagOptions.WrapWholeWordsOnly, "test <tag>target</tag>")]
        [DataRow("test target test", "target", "tag", StringWrapInTagOptions.WrapWholeWordsOnly, "test <tag>target</tag> test")]
        [DataRow("targettest", "target", "tag", StringWrapInTagOptions.None, "<tag>target</tag>test")]
        [DataRow("testtarget", "target", "tag", StringWrapInTagOptions.None, "test<tag>target</tag>")]
        [DataRow("testtargettest", "target", "tag", StringWrapInTagOptions.None, "test<tag>target</tag>test")]
        [DataRow("targettest", "target", "tag", StringWrapInTagOptions.WrapWholeWordsOnly, "targettest")]
        [DataRow("testtarget", "target", "tag", StringWrapInTagOptions.WrapWholeWordsOnly, "testtarget")]
        [DataRow("testtargettest", "target", "tag", StringWrapInTagOptions.WrapWholeWordsOnly, "testtargettest")]
        public void StringExtensions_WrapInTag_WithTarget_ChangeTest(string s, string target, string tag, StringWrapInTagOptions options, string expectedOutput)
        {
            Assert.AreEqual(expectedOutput, s.WrapInTag(target, tag, options));
        }
    }
}