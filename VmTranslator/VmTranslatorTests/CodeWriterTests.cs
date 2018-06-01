using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VmTranslator;
using System.Collections.Generic;
using System.Linq;

namespace VmTranslatorTests
{
    [TestClass]
    public class CodeWriterTests
    {
        [TestMethod]
        public void PushConstantTest()
        {
            var writer = new CodeWriter();

            var expected = new List<string>() { "@5", "D=A", "@SP", "M=M+1", "A=M-1", "M=D" };
            var observed = writer.WritePushPop(CommandType.C_PUSH, "constant", 5).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void PushLocalTest()
        {
            var writer = new CodeWriter();

            var expected = new List<string>() { "@LCL", "D=M", "@2", "A=D+A", "D=M", "@SP",
                                                "M=M+1", "A=M-1", "M=D" };
            var observed = writer.WritePushPop(CommandType.C_PUSH, "local", 2).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void PushStaticTest()
        {
            var writer = new CodeWriter();
            writer.CurrentVmFile = "StaticTest";

            var expected = new List<string>() { "@StaticTest.3", "D=M", "@SP", "M=M+1", "A=M-1", "M=D" };
            var observed = writer.WritePushPop(CommandType.C_PUSH, "static", 3).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void PopLocalTest()
        {
            var writer = new CodeWriter();

            var expected = new List<string>() { "@LCL", "D=M", "@0", "D=D+A", "@R13", "M=D",
                                                "@SP", "AM=M-1", "D=M", "@R13", "A=M", "M=D" };
            var observed = writer.WritePushPop(CommandType.C_POP, "local", 0).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void AddTest()
        {
            var command = "add";
            var writer = new CodeWriter();

            var expected = new List<string>() { "@SP", "A=M-1", "D=M", "A=A-1", "M=D+M", "D=A", "@SP", "M=D+1" };
            var observed = writer.WriteArithmetic(command).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void EqTest()
        {
            var command = "eq";
            var writer = new CodeWriter();

            var expected = new List<string>()
            {
                "@SP",
                "AM=M-1",
                "D=M",
                "A=A-1",
                "D=D-M",
                "@T0",
                "D;JEQ",
                "@SP",
                "AM=M-1",
                "M=0",
                "@E0",
                "0;JMP",
                "(T0)",
                "@SP",
                "AM=M-1",
                "M=-1",
                "(E0)",
                "@SP",
                "M=M+1",
            };

            var observed = writer.WriteArithmetic(command).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void LtTest()
        {
            var command = "lt";
            var writer = new CodeWriter();

            var expected = new List<string>()
            {
                "@SP",
                "AM=M-1",
                "D=M",
                "A=A-1",
                "D=D-M",
                "@T0",
                "D;JGT",
                "@SP",
                "AM=M-1",
                "M=0",
                "@E0",
                "0;JMP",
                "(T0)",
                "@SP",
                "AM=M-1",
                "M=-1",
                "(E0)",
                "@SP",
                "M=M+1",
            };

            var observed = writer.WriteArithmetic(command).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void GtTest()
        {
            var command = "gt";
            var writer = new CodeWriter();

            var expected = new List<string>()
            {
                "@SP",
                "AM=M-1",
                "D=M",
                "A=A-1",
                "D=D-M",
                "@T0",
                "D;JLT",
                "@SP",
                "AM=M-1",
                "M=0",
                "@E0",
                "0;JMP",
                "(T0)",
                "@SP",
                "AM=M-1",
                "M=-1",
                "(E0)",
                "@SP",
                "M=M+1",
            };

            var observed = writer.WriteArithmetic(command).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }
    }
}
