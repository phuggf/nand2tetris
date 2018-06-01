using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmTranslator
{
    public class Parser
    {
        public Parser(string[] commands)
        {
            _commands = commands;
            _currentLine = 0;
            _numCommands = commands.Count();
        }

        public bool HasMoreCommands => _currentLine < _numCommands;
        public CommandType CommandType { get; private set; }

        public void Advance()
        {
            bool isValid = false;

            while (isValid == false)
            {
                _currentCommand = _commands[_currentLine++];

                if (TryParseCommand(_currentCommand, out CommandType type))
                {
                    isValid = true;
                    CommandType = type;
                }
                else
                {
                    isValid = false;
                }
            }
        }

        private bool TryParseCommand(string command, out CommandType type)
        {
            string com = command?.Split(' ')?.FirstOrDefault();

            if (string.IsNullOrEmpty(com))
            {
                type = default(CommandType);
                return false;
            }

            if (_commandTypes.ContainsKey(com))
            {
                type = _commandTypes[com];
                return true;
            }
            else
            {
                type = default(CommandType);
                return false;
            }
        }

        public string Arg1()
        {
            if (CommandType == CommandType.C_ARITHMETIC)
            {
                return _currentCommand;
            }
            else
            {
                return _currentCommand.Split(' ')?[1];
            }
        }

        public string Arg2()
        {
            return _currentCommand.Split(' ')?[2];
        }

        public void Reset()
        {
            _currentLine = 0;
        }

        private string[] _commands;
        private string _currentCommand;
        private int _currentLine;
        private int _numCommands;
        private Dictionary<string, CommandType> _commandTypes = new Dictionary<string, CommandType>()
        {
            ["push"] = CommandType.C_PUSH,
            ["pop"] = CommandType.C_POP,

            ["add"] = CommandType.C_ARITHMETIC,
            ["sub"] = CommandType.C_ARITHMETIC,
            ["neg"] = CommandType.C_ARITHMETIC,
            ["eq"] = CommandType.C_ARITHMETIC,
            ["gt"] = CommandType.C_ARITHMETIC,
            ["lt"] = CommandType.C_ARITHMETIC,
            ["and"] = CommandType.C_ARITHMETIC,
            ["or"] = CommandType.C_ARITHMETIC,
            ["not"] = CommandType.C_ARITHMETIC,

            ["label"] = CommandType.C_LABEL,
            ["goto"] = CommandType.C_GOTO,
            ["if-goto"] = CommandType.C_IF,

            ["function"] = CommandType.C_FUNCTION,
            ["call"] = CommandType.C_CALL,
            ["return"] = CommandType.C_RETURN
        };
    }

    public enum CommandType
    {
        C_ARITHMETIC,
        C_PUSH,
        C_POP,
        C_LABEL,
        C_GOTO,
        C_IF,
        C_FUNCTION,
        C_RETURN,
        C_CALL
    }
}
