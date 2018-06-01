using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VmTranslator.Helpers;

namespace VmTranslator
{
    public class CodeWriter
    {
        private ArithmeticWriter _arithWriter = new ArithmeticWriter();

        public string CurrentVmFile { get; set; }

        public IEnumerable<string> WriteArithmetic(string command)
        {
            return _arithWriter.GetCommands(command);
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

        private IEnumerable<string> PushConstant(int value)
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
