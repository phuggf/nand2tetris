using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmTranslator
{
    public class AssemblyWriter
    {
        public AssemblyWriter(Parser parser, CodeWriter codeWriter)
        {
            _parser = parser;
            _codeWriter = codeWriter;
        }

        private Parser _parser;
        private CodeWriter _codeWriter;

        public string[] GetAssembly()
        {
            var commands = new List<string>();

            while (_parser.HasMoreCommands)
            {
                _parser.Advance();

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
                        commands.AddRange(_codeWriter.WriteLabel(_parser.Arg1()));
                        break;
                    case CommandType.C_GOTO:
                        commands.AddRange(_codeWriter.WriteGoto(_parser.Arg1()));
                        break;
                    case CommandType.C_IF:
                        commands.AddRange(_codeWriter.WriteIf(_parser.Arg1()));
                        break;
                    case CommandType.C_FUNCTION:
                        commands.AddRange(_codeWriter.WriteFunction(_parser.Arg1(), int.Parse(_parser.Arg2())));
                        break;
                    case CommandType.C_RETURN:
                        commands.AddRange(_codeWriter.WriteReturn());
                        break;
                    case CommandType.C_CALL:
                        commands.AddRange(_codeWriter.WriteCall(_parser.Arg1(), int.Parse(_parser.Arg2())));
                        break;
                    default:
                        break;
                }
            }

            return commands.ToArray();
        }
    }
}
