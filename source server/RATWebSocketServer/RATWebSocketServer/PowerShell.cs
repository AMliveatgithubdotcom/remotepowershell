using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Net;

namespace PowerShellCommands
{
    public class PowerShellSession
    {
        InitialSessionState iss;
        Runspace rs;
        PowerShell ps;
        public PowerShellSession()
        {
            iss = InitialSessionState.CreateDefault();
            rs = RunspaceFactory.CreateRunspace(iss);
            ps = PowerShell.Create();
            ps.Runspace = rs;
            rs.Open();
        }
        public List<string> SanitizeInput(string s)
        {
            List<string> subs = new List<string>();
            Console.Write("Input: ");
            foreach (string sub in s.Split('/'))
            {
                Console.Write(sub + ", ");
                subs.Add(sub);
            }
            subs.RemoveAt(subs.Count - 1);
            return subs;
        }
        public string InvokeClean()
        {
            string result = "";
            try
            {
                foreach (PSObject obj in ps.Invoke())
                {
                    result += obj.ToString() + "\n";
                }
                Console.WriteLine("Command has been invoked.");
            }
            catch (Exception ex)
            { throw; }
            ps.Commands.Clear();
            return result;
        }
        public string PSGetUser()
        {   
            ps.AddCommand("Get-LocalUser");
            return InvokeClean();
        }
        /* public void PSCommand(List<string> commands)
         {

             foreach (string command in commands)
             {
                 try
                 {
                     Console.WriteLine("Adding: " + command);
                     ps.AddCommand(command);
                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine("Error! :\n" + ex.ToString());
                 }
             }
             Console.WriteLine("Commands added successfully. Now invoking.");
             InvokeClean();
         }*/
        public void PSCreateUser(List<string> parameters)
        {
            ps.AddCommand("New-LocalUser");
            for (int i = 0; i < parameters.Count(); i++)
            {
                if (parameters[i].ToString() != "")
                {
                    try
                    {
                        switch (i)
                        {
                            case 0:
                                Console.WriteLine("Name: " + parameters[i]);
                                ps.AddParameter("Name", parameters[i]);
                                break;
                            case 1:
                                Console.WriteLine("Password: " + parameters[i]);
                                SecureString ss = new NetworkCredential("", parameters[i].ToString()).SecurePassword;
                                ps.AddParameter("Password", ss);
                                break;
                            case 2:
                                Console.WriteLine("Description: " + parameters[i]);
                                ps.AddParameter("Description", parameters[i]);
                                break;
                            case 3:
                                Console.WriteLine("FullName: " + parameters[i]);
                                ps.AddParameter("FullName", parameters[i]);
                                break;
                            case 4:
                                Console.WriteLine("-NoPassword");
                                ps.AddParameter("-NoPassword");
                                break;
                            case 5:
                                Console.WriteLine("-Disabled");
                                ps.AddParameter($"-Disabled");
                                break;
                            default: throw new ArgumentException();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed on adding current parameter " + parameters[i].ToString() + " to pipeline! \n Error: \n{1}" + ex);
                    }

                }
                else
                {
                    Console.WriteLine("Parameter " + i + " is empty!");
                }
            }
            try
            {
                InvokeClean();
                Console.WriteLine("Command has gone through.");
            }
            catch (Exception ex)
            { Console.WriteLine("Command failed on invoke!: \n" + ex.ToString()); }
        }
        public void PSEditUser(List<string> parameters)
        {
            int blankcount = 0;
            if (parameters.Count() == 2)
            {
                try
                {
                    ps.AddCommand("Rename-LocalUser");
                    Console.WriteLine("Changing Name of: " + parameters[0] + "to " + parameters[1]);
                    ps.AddParameter("Name", parameters[0]);
                    ps.AddParameter("NewName", parameters[1]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to add to pipeline!\n Error: \n" + ex.ToString());
                }

            }
            else
            {
                for (int i = 0; i < parameters.Count(); i++)
                {
                    if (parameters[i].ToString() != "")
                    {
                        try
                        {
                            switch (i)
                            {
                                case 0:
                                    if (parameters[1] != "")
                                    {
                                        ps.AddCommand("Rename-LocalUser");
                                        Console.WriteLine("Changing Name of: " + parameters[0] + "to " + parameters[1]);
                                        ps.AddParameter("Name", parameters[0]);
                                        ps.AddParameter("NewName", parameters[1]);
                                        InvokeClean();
                                    }
                                    ps.AddCommand("Get-LocalUser");
                                    Console.WriteLine("Editing: " + parameters[i]);
                                    ps.AddParameter("Name", parameters[i]);
                                    ps.AddCommand("Set-LocalUser");
                                    break;
                                case 1:
                                    break;
                                case 2:
                                    Console.WriteLine("Password: " + parameters[i]);
                                    SecureString ss = new NetworkCredential("", parameters[i].ToString()).SecurePassword;
                                    ps.AddParameter("Password", ss);
                                    break;
                                case 3:
                                    Console.WriteLine("Description: " + parameters[i]);
                                    ps.AddParameter("Description", parameters[i]);
                                    break;
                                case 4:
                                    Console.WriteLine("FullName: " + parameters[i]);
                                    ps.AddParameter("FullName", parameters[i]);
                                    break;
                                case 5:
                                    Console.WriteLine("-NoPassword");
                                    ps.AddParameter("NoPassword");
                                    break;
                                case 6:
                                    Console.WriteLine("-Disabled");
                                    ps.AddParameter($"Disabled");
                                    break;
                                default: throw new ArgumentException();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed on adding current parameter " + parameters[i].ToString() + " to pipeline! \n Error: \n{1}" + ex);
                        }

                    }
                    else
                    {
                        Console.WriteLine("Parameter " + i + " is empty!");
                        blankcount++;

                    }
                }

            }
            try
            {
                InvokeClean();
            }
            catch (Exception ex)
            { Console.WriteLine("Command failed on invoke!: \n" + ex.ToString()); }
            Console.WriteLine("Command has gone through.");
        }
        public void PSDeleteUser(string name)
        {
            ps.AddCommand("Remove-LocalUser");
            ps.AddParameter("Name", name);
            InvokeClean();
            Console.WriteLine("Deleted User: " + name);
        }
    }
}

