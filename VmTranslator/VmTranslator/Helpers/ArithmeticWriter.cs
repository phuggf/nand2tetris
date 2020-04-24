using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmTranslator.Helpers
{
    public class ArithmeticWriter
    {
        public IEnumerable<string> GetCommands(string command)
        {
            switch (command)
            {
                case "add":
                    return Add();
                case "sub":
                    return Sub();
                case "neg":
                    return Neg();
                case "eq":
                    return Eq();
                case "gt":
                    return Gt();
                case "lt":
                    return Lt();
                case "and":
                    return And();
                case "or":
                    return Or();
                case "not":
                    return Not();
                default:
                    throw new ArgumentException($"Command {command} not allowed");
            }
        }

        public IEnumerable<string> GetCommands(string command, string functionName)
        {
            _currentFuncName = functionName;
            var ret = GetCommands(command);
            _currentFuncName = null;
            return ret;
        }

        private string _currentFuncName;

        private IEnumerable<string> Add() => GetArithmetic("M=D+M");

        private IEnumerable<string> Sub() => GetArithmetic("M=M-D");

        private IEnumerable<string> Neg() => new List<string> { "@SP", "A=M-1", "M=-M" };

        private IEnumerable<string> Eq() => GetCondition("D;JEQ");

        private IEnumerable<string> Gt() => GetCondition("D;JLT");

        private IEnumerable<string> Lt() => GetCondition("D;JGT");

        private IEnumerable<string> And() => GetArithmetic("M=D&M");

        private IEnumerable<string> Or() => GetArithmetic("M=D|M");

        private IEnumerable<string> Not() => new List<string> { "@SP", "A=M-1", "M=!M" };

        private int _addressCounter;

        private IEnumerable<string> GetCondition(string condition)
        {
            List<string> ret;

            if (_currentFuncName != null)
            {
                ret = GetCondition(condition, _currentFuncName).ToList();
            }
            else
            {
                ret = new List<string>
                {
                    "@SP",
                    "AM=M-1",
                    "D=M",
                    "A=A-1",
                    "D=D-M",
                    $"@T{_addressCounter}",
                    $"{condition}",
                    "@SP",
                    "AM=M-1",
                    "M=0",
                    $"@E{_addressCounter}",
                    "0;JMP",
                    $"(T{_addressCounter})",
                    "@SP",
                    "AM=M-1",
                    "M=-1",
                    $"(E{_addressCounter})",
                    "@SP",
                    "M=M+1",
                };
            }

            _addressCounter++;

            return ret;
        }

        private IEnumerable<string> GetCondition(string condition, string functionName)
        {
            var ret = new List<string>
            {
                "@SP",
                "AM=M-1",
                "D=M",
                "A=A-1",
                "D=D-M",
                $"@{functionName}$T{_addressCounter}",
                $"{condition}",
                "@SP",
                "AM=M-1",
                "M=0",
                $"@{functionName}$E{_addressCounter}",
                "0;JMP",
                $"({functionName}$T{_addressCounter})",
                "@SP",
                "AM=M-1",
                "M=-1",
                $"({functionName}$E{_addressCounter})",
                "@SP",
                "M=M+1",
            };

            _addressCounter++;

            return ret;
        }

        private IEnumerable<string> GetArithmetic(string operation)
        {
            return new List<string>
            {
                "@SP",
                "A=M-1",
                "D=M",
                "A=A-1",
                $"{operation}",
                "D=A",
                "@SP",
                "M=D+1",
            };
        }
    }
}
