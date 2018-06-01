using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmTranslator
{
    public class AssemblyWriter
    {
        public AssemblyWriter( Parser parser, CodeWriter codeWriter )
        {
            _parser = parser;
            _codeWriter = codeWriter;
        }

        private Parser _parser;
        private CodeWriter _codeWriter;

        public string[] GetAssembly()
        {
            var commands = new List<string>();
            int lineNum = 0;

            while (_parser.HasMoreCommands)
            {
                _parser.Advance();
                lineNum = commands.Count;

                switch (_parser.CommandType)
                {
                    case CommandType.C_ARITHMETIC:
                        commands.AddRange(_codeWriter.WriteArithmetic(_parser.Arg1()));
                        break;
                    case CommandType.C_PUSH:
                    case CommandType.C_POP:
                        commands.AddRange(_codeWriter.WritePushPop(_parser.CommandType, _parser.Arg1(), int.Parse(_parser.Arg2())));
                        break;
                    case CommandType.C_LABEL:
                        break;
                    case CommandType.C_GOTO:
                        break;
                    case CommandType.C_IF:
                        break;
                    case CommandType.C_FUNCTION:
                        break;
                    case CommandType.C_RETURN:
                        break;
                    case CommandType.C_CALL:
                        break;
                    default:
                        break;
                }
            }

            return commands.ToArray();
        }
    }
}
