using System;
using System.Collections.Generic;
using VmTranslator.Helpers;

namespace VmTranslator
{
    public class CodeWriter
    {
        private ArithmeticWriter _arithWriter = new ArithmeticWriter();
        private int _returnAddressCounter = 0;

        public string CurrentVmFile { get; set; }

        public IEnumerable<string> WriteInit()
        {
            var ret = new List<string>()
            {
                "@256",
                "D=A",
                "@SP",
                "M=D",
            };

            ret.AddRange(WriteCall("Sys.init", 0));
            return ret;
        }

        public IEnumerable<string> WriteArithmetic(string command)
        {
            return _arithWriter.GetCommands(command, _currentFunction);
        }

        public IEnumerable<string> WritePushPop(CommandType type, string segment, int index)
        {
            if (type == CommandType.C_PUSH)
            {
                switch (segment)
                {
                    case "constant":
                        return PushConstant(index);
                    case "local":
                        return PushSegment("LCL", index);
                    case "argument":
                        return PushSegment("ARG", index);
                    case "this":
                        return PushSegment("THIS", index);
                    case "that":
                        return PushSegment("THAT", index);
                    case "temp":
                        return PushVariable($"R{index + 5}");
                    case "pointer":
                        return PushVariable($"R{index + 3}");
                    case "static":
                        return PushVariable($"{CurrentVmFile}.{index}");
                    default:
                        throw new ArgumentException($"Unknown segment {segment}");
                }
            }
            else if (type == CommandType.C_POP)
            {
                switch (segment)
                {
                    case "local":
                        return PopSegment("LCL", index);
                    case "argument":
                        return PopSegment("ARG", index);
                    case "this":
                        return PopSegment("THIS", index);
                    case "that":
                        return PopSegment("THAT", index);
                    case "temp":
                        return PopVariable($"R{index + 5}");
                    case "pointer":
                        return PopVariable($"R{index + 3}");
                    case "static":
                        return PopVariable($"{CurrentVmFile}.{index}");
                    default:
                        throw new ArgumentException($"Unknown segment {segment}");
                }
            }
            else
            {
                throw new ArgumentException($"{type} not allowed");
            }
        }

        public IEnumerable<string> WriteLabel(string label)
        {
            if (_currentFunction == null)
            {
                return new List<string>() { $"({label})" };
            }
            else
            {
                return new List<string>() { $"({_currentFunction}${label})" };
            }
        }

        public IEnumerable<string> WriteGoto(string label) => new List<string>() { $"@{(_currentFunction == null ? label : $"{_currentFunction}${label}")}", "0;JMP" };

        public IEnumerable<string> WriteIf(string label)
        {
            return new List<string>()
            {
                "@SP",
                "AM=M-1",
                "D=M",
                $"@{(_currentFunction == null ? label : $"{_currentFunction}${label}")}",
                "D;JLT",
            };
        }

        public IEnumerable<string> WriteFunction(string name, int numLocals)
        {
            string label = $"({name})";
            var initialiseLocals = new List<string>() { label };

            for (int i = 0; i < numLocals; i++)
            {
                initialiseLocals.AddRange(WritePushPop(CommandType.C_PUSH, "constant", 0));
            }

            _currentFunction = name;

            return initialiseLocals;
        }

        private string _currentFunction = null;

        public IEnumerable<string> WriteReturn()
        {
            //_currentFunction = null;
            var ret = new List<string>();

            ret.AddRange(new List<string>()
            {
                "@LCL",
                "D=M",
                "@R13",
                "M=D", // FRAME = LCL
                "@5",
                "A=D-A",
                "D=M",
                "@R14",
                "M=D", // RET = *(FRAME-5)
                "@SP",
                "AM=M-1",
                "D=M",
                "@ARG",
                "A=M",
                "M=D", // *ARG = pop()
            });

            ret.AddRange(new List<string>()
            {
                "@ARG",
                "D=M+1",
                "@SP",
                "M=D", // SP = ARG+1
                "@R13",
                "AM=M-1",
                "D=M",
                "@THAT",
                "M=D", // THAT = *(FRAME-1)
                "@R13",
                "AM=M-1",
                "D=M",
                "@THIS",
                "M=D", // THIS = *(FRAME-2)
                "@R13",
                "AM=M-1",
                "D=M",
                "@ARG",
                "M=D", // ARG = *(FRAME-3)
                "@R13",
                "AM=M-1",
                "D=M",
                "@LCL",
                "M=D", // LCL = *(FRAME-4)
                "@R14",
                "A=M",
                "0;JMP", // goto RET
            });

            return ret;
        }

        public IEnumerable<string> WriteCall(string name, int numArgs)
        {
            var ret = new List<string>();
            string returnAddress = $"return-address{_returnAddressCounter}";
            int argSubtractor = numArgs + 5;

            ret.AddRange(PushConstant(returnAddress));
            ret.AddRange(PushVariable("LCL"));
            ret.AddRange(PushVariable("ARG"));
            ret.AddRange(PushVariable("THIS"));
            ret.AddRange(PushVariable("THAT"));
            ret.AddRange(new List<string>()
            {
                $"@{argSubtractor}",
                "D=A",
                "@SP",
                "D=M-D",
                "@ARG",
                "M=D", // ARG = SP-n-5
                "@SP",
                "D=M",
                "@LCL",
                "M=D", // LCL = SP
                $"@{name}",
                "0;JMP", // Goto name
                $"({returnAddress})",
            });
            _returnAddressCounter++;

            return ret;
        }

        private IEnumerable<string> PushConstant(int value)
        {
            return new List<string>() { $"@{value}", "D=A", "@SP", "M=M+1", "A=M-1", "M=D" };
        }

        private IEnumerable<string> PushConstant(string value)
        {
            return new List<string>() { $"@{value}", "D=A", "@SP", "M=M+1", "A=M-1", "M=D" };
        }

        private IEnumerable<string> PushVariable(string variable)
        {
            return new List<string>() { $"@{variable}", "D=M", "@SP", "M=M+1", "A=M-1", "M=D" };
        }

        private IEnumerable<string> PopVariable(string variable)
        {
            return new List<string>() { "@SP", "AM=M-1", "D=M", $"@{variable}", "M=D" };
        }

        private IEnumerable<string> PushSegment(string register, int value)
        {
            return new List<string>() { $"@{register}", "D=M", $"@{value}", "A=D+A", "D=M", "@SP", "M=M+1", "A=M-1", "M=D" };
        }

        private IEnumerable<string> PopSegment(string register, int value)
        {
            return new List<string>() { $"@{register}", "D=M", $"@{value}", "D=D+A", "@R13", "M=D", "@SP", "AM=M-1", "D=M", "@R13", "A=M", "M=D" };
        }
    }
}
