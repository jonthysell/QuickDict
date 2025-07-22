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
        public void StringExtensions_EscapeForXml_EmptyTest()
        {
            Assert.AreEqual("", "".EscapeForXml());
        }

        [TestMethod]
        public void StringExtensions_EscapeForXml_NoChangeTests()
        {
            TestUtils.LoadAndExecuteTestCases<EscapeForXmlTestCase>("StringExtensions_EscapeForXml_NoChangeTests.txt");
        }

        [TestMethod]
        public void StringExtensions_EscapeForXml_ChangeTests()
        {
            TestUtils.LoadAndExecuteTestCases<EscapeForXmlTestCase>("StringExtensions_EscapeForXml_ChangeTests.txt");
        }

        public class EscapeForXmlTestCase : StringExtensionsTestCase
        {
            public EscapeForXmlTestCase() : base(StringExtensions.EscapeForXml) { }
        }

        public abstract class StringExtensionsTestCase : ITestCase
        {
            public string Input { get; private set; } = "";
            public string ExpectedOutput { get; private set; } = "";

            public readonly Func<string, string> FunctionToTest;

            public string ActualOutput { get; private set; } = "";

            public StringExtensionsTestCase(Func<string, string> functionToTest)
            {
                FunctionToTest = functionToTest;
            }

            public void Execute()
            {
                ActualOutput = FunctionToTest(Input);
                Assert.AreEqual(ExpectedOutput, ActualOutput);
            }

            public void Parse(string s)
            {
                var split = s.Split('\t', 2, StringSplitOptions.None);
                Input = split[0];
                ExpectedOutput = split[1];
            }
        }
    }
}