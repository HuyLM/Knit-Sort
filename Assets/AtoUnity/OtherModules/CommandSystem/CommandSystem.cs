using System.Collections.Generic;




namespace OtherModules.CommandSystem
{
    public class CommandSystem
    {
        public char splitKey = ' ';
        public bool matchCase = false;


        private List<BaseCommand> commands = new List<BaseCommand>();

        public BaseCommand GetCommand(int index)
        {
            if (index < 0 || index >= commands.Count)
            {
                return null;
            }
            return commands[index];
        }

        public List<BaseCommand> GetCommands(string id)
        {
            List<BaseCommand> results = new List<BaseCommand>();
            foreach (var cmd in commands)
            {
                if (CompareId(cmd.Id, id))
                {
                    results.Add(cmd);
                }
            }
            return results;
        }

        public List<BaseCommand> GetAllCommand()
        {
            return commands;
        }

        public bool AddCommand(BaseCommand cmd)
        {
            if (commands == null)
            {
                commands = new List<BaseCommand>();
            }
            foreach (var c in commands)
            {
                if (CompareId(cmd.Id, c.Id)) // same id
                {
                    if (cmd.GetType().Equals(c.GetType()))
                    {
                        return false;
                    }
                }
            }
            commands.Add(cmd);
            return true;
        }

        public bool DoCommand(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            string[] paramsData = input.Split(splitKey);
            string idCmd = paramsData[0];
            int paramNumber = paramsData.Length - 1;


            foreach (var cmd in commands)
            {
                if (CompareId(idCmd, cmd.Id) && paramNumber == cmd.GetParamNumber())
                {
                    bool validate = ValidateParams(cmd, paramsData);
                    if (validate)
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        private bool CompareId(string input, string id)
        {
            if (!matchCase)
            {
                input = input.ToLower();
                id = id.ToLower();
            }
            return id.Equals(input);
        }

        private bool ValidateParams(BaseCommand cmd, string[] param)
        {
            int paramNumber = param.Length - 1;
            if (paramNumber == 0)
            {
                return ValidateParams_0(cmd, param);
            }
            else if (paramNumber == 1)
            {
                return ValidateParams_1(cmd, param);
            }
            else if (paramNumber == 2)
            {
                return ValidateParams_2(cmd, param);
            }
            else if (paramNumber == 3)
            {
                return ValidateParam_3(cmd, param);
            }
            return false;
        }

        private bool ValidateParams_0(BaseCommand cmd, string[] param)
        {
            if (cmd is Command _cmd)
            {
                _cmd.Invoke();
                return true;
            }
            return false;
        }

        private bool ValidateParams_1(BaseCommand cmd, string[] param)
        {
            if (cmd is Command<int> i_cmd)
            {
                int data;
                if (int.TryParse(param[1], out data))
                {
                    i_cmd.Invoke(data);
                    return true;
                }
            }
            else if (cmd is Command<float> f_cmd)
            {
                float data;
                if (float.TryParse(param[1], out data))
                {
                    f_cmd.Invoke(data);
                    return true;
                }
            }
            else if (cmd is Command<string> s_cmd)
            {
                s_cmd.Invoke(param[1]);
                return true;
            }
            return false;
        }

        private bool ValidateParams_2(BaseCommand cmd, string[] param)
        {
            // int
            if (cmd is Command<int, int> i_i_cmd) // int, int
            {
                int data1;
                int data2;
                if (int.TryParse(param[1], out data1) && int.TryParse(param[2], out data2))
                {
                    i_i_cmd.Invoke(data1, data2);
                    return true;
                }
            }
            else if (cmd is Command<int, float> i_f_cmd) // int, float
            {
                int data1;
                float data2;
                if (int.TryParse(param[1], out data1) && float.TryParse(param[2], out data2))
                {
                    i_f_cmd.Invoke(data1, data2);
                    return true;
                }
            }
            else if (cmd is Command<int, string> i_s_cmd) // int, string
            {
                int data1;
                string data2 = param[2];
                if (int.TryParse(param[1], out data1))
                {
                    i_s_cmd.Invoke(data1, data2);
                    return true;
                }
            }

            // float 
            else if (cmd is Command<float, int> f_i_cmd) // float, int
            {
                float data1;
                int data2;
                if (float.TryParse(param[1], out data1) && int.TryParse(param[2], out data2))
                {
                    f_i_cmd.Invoke(data1, data2);
                    return true;
                }
            }
            else if (cmd is Command<float, float> f_f_cmd) // float, float
            {
                float data1;
                float data2;
                if (float.TryParse(param[1], out data1) && float.TryParse(param[2], out data2))
                {
                    f_f_cmd.Invoke(data1, data2);
                    return true;
                }
            }
            else if (cmd is Command<float, string> f_s_cmd) // float, string
            {
                float data1;
                string data2 = param[2];
                if (float.TryParse(param[1], out data1))
                {
                    f_s_cmd.Invoke(data1, data2);
                    return true;
                }
            }

            //string
            else if (cmd is Command<string, int> s_i_cmd) // string, int
            {
                string data1 = param[1];
                int data2;
                if (int.TryParse(param[2], out data2))
                {
                    s_i_cmd.Invoke(data1, data2);
                    return true;
                }
            }
            else if (cmd is Command<string, float> s_f_cmd) // string, float
            {
                string data1 = param[1]; ;
                float data2;
                if (float.TryParse(param[2], out data2))
                {
                    s_f_cmd.Invoke(data1, data2);
                    return true;
                }
            }
            else if (cmd is Command<string, string> s_s_cmd) // string, string
            {
                string data1 = param[1]; ;
                string data2 = param[2];
                s_s_cmd.Invoke(data1, data2);
                return true;
            }
            return false;
        }

        private bool ValidateParam_3(BaseCommand cmd, string[] param)
        {
            // int
            if (cmd is Command<int, int, int> i_i_i_cmd) // int, int, int
            {
                int data1;
                int data2;
                int data3;
                if (int.TryParse(param[1], out data1) && int.TryParse(param[2], out data2) && int.TryParse(param[3], out data3))
                {
                    i_i_i_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<int, int, float> i_i_f_cmd) // int, int, float
            {
                int data1;
                int data2;
                float data3;
                if (int.TryParse(param[1], out data1) && int.TryParse(param[2], out data2) && float.TryParse(param[3], out data3))
                {
                    i_i_f_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<int, int, string> i_i_s_cmd) // int, int, string
            {
                int data1;
                int data2;
                string data3 = param[3];
                if (int.TryParse(param[1], out data1) && int.TryParse(param[2], out data2))
                {
                    i_i_s_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }

            else if (cmd is Command<int, float, int> i_f_i_cmd) // int, float, int
            {
                int data1;
                float data2;
                int data3;
                if (int.TryParse(param[1], out data1) && float.TryParse(param[2], out data2) && int.TryParse(param[3], out data3))
                {
                    i_f_i_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<int, float, float> i_f_f_cmd) // int, float, float
            {
                int data1;
                float data2;
                float data3;
                if (int.TryParse(param[1], out data1) && float.TryParse(param[2], out data2) && float.TryParse(param[3], out data3))
                {
                    i_f_f_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<int, float, string> i_f_s_cmd) // int, float, string
            {
                int data1;
                float data2;
                string data3 = param[3];
                if (int.TryParse(param[1], out data1) && float.TryParse(param[2], out data2))
                {
                    i_f_s_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }

            else if (cmd is Command<int, string, int> i_s_i_cmd) // int, string, int
            {
                int data1;
                string data2 = param[2];
                int data3;
                if (int.TryParse(param[1], out data1) && int.TryParse(param[3], out data3))
                {
                    i_s_i_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<int, string, float> i_s_f_cmd) // int, string, float
            {
                int data1;
                string data2 = param[2];
                float data3;
                if (int.TryParse(param[1], out data1) && float.TryParse(param[3], out data3))
                {
                    i_s_f_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<int, string, string> i_s_s_cmd) // int, string, string
            {
                int data1;
                string data2 = param[2];
                string data3 = param[3];
                if (int.TryParse(param[1], out data1))
                {
                    i_s_s_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }

            // float
            else if (cmd is Command<float, int, int> f_i_i_cmd) // float, int, int
            {
                float data1;
                int data2;
                int data3;
                if (float.TryParse(param[1], out data1) && int.TryParse(param[2], out data2) && int.TryParse(param[3], out data3))
                {
                    f_i_i_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<float, int, float> f_i_f_cmd) // float, int, float
            {
                float data1;
                int data2;
                float data3;
                if (float.TryParse(param[1], out data1) && int.TryParse(param[2], out data2) && float.TryParse(param[3], out data3))
                {
                    f_i_f_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<float, int, string> f_i_s_cmd) // float, int, string
            {
                float data1;
                int data2;
                string data3 = param[3];
                if (float.TryParse(param[1], out data1) && int.TryParse(param[2], out data2))
                {
                    f_i_s_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }

            else if (cmd is Command<float, float, int> f_f_i_cmd) // float, float, int
            {
                float data1;
                float data2;
                int data3;
                if (float.TryParse(param[1], out data1) && float.TryParse(param[2], out data2) && int.TryParse(param[3], out data3))
                {
                    f_f_i_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<float, float, float> f_f_f_cmd) // float, float, float
            {
                float data1;
                float data2;
                float data3;
                if (float.TryParse(param[1], out data1) && float.TryParse(param[2], out data2) && float.TryParse(param[3], out data3))
                {
                    f_f_f_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<float, float, string> f_f_s_cmd) // float, float, string
            {
                float data1;
                float data2;
                string data3 = param[3];
                if (float.TryParse(param[1], out data1) && float.TryParse(param[2], out data2))
                {
                    f_f_s_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }

            else if (cmd is Command<float, string, int> f_s_i_cmd) // float, string, int
            {
                float data1;
                string data2 = param[2];
                int data3;
                if (float.TryParse(param[1], out data1) && int.TryParse(param[3], out data3))
                {
                    f_s_i_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<float, string, float> f_s_f_cmd) // float, string, float
            {
                float data1;
                string data2 = param[2];
                float data3;
                if (float.TryParse(param[1], out data1) && float.TryParse(param[3], out data3))
                {
                    f_s_f_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<float, string, string> f_s_s_cmd) // float, string, string
            {
                float data1;
                string data2 = param[2];
                string data3 = param[3];
                if (float.TryParse(param[1], out data1))
                {
                    f_s_s_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }

            // string
            else if (cmd is Command<string, int, int> s_i_i_cmd) // string, int, int
            {
                string data1 = param[1];
                int data2;
                int data3;
                if (int.TryParse(param[2], out data2) && int.TryParse(param[3], out data3))
                {
                    s_i_i_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<string, int, float> s_i_f_cmd) // string, int, float
            {
                string data1 = param[1];
                int data2;
                float data3;
                if (int.TryParse(param[2], out data2) && float.TryParse(param[3], out data3))
                {
                    s_i_f_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<string, int, string> s_i_s_cmd) // string, int, string
            {
                string data1 = param[1];
                int data2;
                string data3 = param[3];
                if (int.TryParse(param[2], out data2))
                {
                    s_i_s_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }

            else if (cmd is Command<string, float, int> s_f_i_cmd) // string, float, int
            {
                string data1 = param[1];
                float data2;
                int data3;
                if (float.TryParse(param[2], out data2) && int.TryParse(param[3], out data3))
                {
                    s_f_i_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<string, float, float> s_f_f_cmd) // string, float, float
            {
                string data1 = param[1];
                float data2;
                float data3;
                if (float.TryParse(param[2], out data2) && float.TryParse(param[3], out data3))
                {
                    s_f_f_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<string, float, string> s_f_s_cmd) // string, float, string
            {
                string data1 = param[1];
                float data2;
                string data3 = param[3];
                if (float.TryParse(param[2], out data2))
                {
                    s_f_s_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }

            else if (cmd is Command<string, string, int> s_s_i_cmd) // string, string, int
            {
                string data1 = param[1];
                string data2 = param[2];
                int data3;
                if (int.TryParse(param[3], out data3))
                {
                    s_s_i_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<string, string, float> s_s_f_cmd) // string, string, float
            {
                string data1 = param[1];
                string data2 = param[2];
                float data3;
                if (float.TryParse(param[3], out data3))
                {
                    s_s_f_cmd.Invoke(data1, data2, data3);
                    return true;
                }
            }
            else if (cmd is Command<string, string, string> s_s_s_cmd) // string, string, string
            {
                string data1 = param[1];
                string data2 = param[2];
                string data3 = param[3];
                s_s_s_cmd.Invoke(data1, data2, data3);
                return true;
            }
            return false;
        }
    }
}