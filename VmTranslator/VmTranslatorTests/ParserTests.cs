using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using VmTranslator;

namespace VmTranslatorTests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void CommandTypeTest()
        {
            var commands = new List<string>() { "push constant 7", "push constant 8", "add" }.ToArray();
            var parser = new Parser(commands);

            Assert.IsTrue(parser.HasMoreCommands);

            parser.Advance();
            Assert.AreEqual(CommandType.C_PUSH, parser.CommandType);

            parser.Advance();
            Assert.AreEqual(CommandType.C_PUSH, parser.CommandType);

            parser.Advance();
            Assert.AreEqual(CommandType.C_ARITHMETIC, parser.CommandType);

            Assert.IsFalse(parser.HasMoreCommands);
        }

        [TestMethod]
        public void Arg1Test()
        {
            var commands = new List<string>() { "push constant 7", "push constant 8", "add", "push local 0" }.ToArray();
            var parser = new Parser(commands);

            Assert.IsTrue(parser.HasMoreCommands);

            parser.Advance();
            Assert.AreEqual("constant", parser.Arg1());

            parser.Advance();
            Assert.AreEqual("constant", parser.Arg1());

            parser.Advance();
            Assert.AreEqual("add", parser.Arg1());

            parser.Advance();
            Assert.AreEqual("local", parser.Arg1());

            Assert.IsFalse(parser.HasMoreCommands);
        }

        [TestMethod]
        public void Arg2Test()
        {
            var commands = new List<string>() { "push constant 7", "function mult 2", "push local 0" }.ToArray();
            var parser = new Parser(commands);

            Assert.IsTrue(parser.HasMoreCommands);

            parser.Advance();
            Assert.AreEqual("7", parser.Arg2());

            parser.Advance();
            Assert.AreEqual("2", parser.Arg2());

            parser.Advance();
            Assert.AreEqual("0", parser.Arg2());

            Assert.IsFalse(parser.HasMoreCommands);
        }
    }
}
