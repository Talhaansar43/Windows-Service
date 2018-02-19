using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing;

namespace K142115_IPT_Assignment_3
{
    class Get_Data
    {
        public string name;
        public string type;
        public string startTime;
        public string endTime;
        public string repeat;

        public string filename;
        public string destination;
        public string source;

        public void Set_Data(string name, string type, string startTime, string endTime, string repeat, string filename, string destination)
        {
            this.name = name;
            this.type = type;
            this.startTime = startTime;
            this.endTime = endTime;
            this.repeat = repeat;

            this.filename = filename;
            this.destination = destination;
        }
        public void getData(string Type)
        {
            XElement xelement = XElement.Load("TaskFile.xml");
            IEnumerable<XElement> Task = xelement.Elements();

            var tasks = from task in Task
                        where task.Attribute("type").Value == Type
                        select task;

            foreach (var task in tasks)
            {
                string name = task.Attribute("name").Value;
                string type = task.Attribute("type").Value;
                string startTime = task.Element("Occurrence").Attribute("starttime").Value;
                string endTime = task.Element("Occurrence").Attribute("endtime").Value;
                string repeat = task.Element("Occurrence").Attribute("repeat").Value;
                if (type == "SecureBackup")
                {
                    filename = task.Element("SecureBackup").Attribute("src").Value;
                    destination = task.Element("SecureBackup").Attribute("dest").Value;
                }
                else
                {
                    if (type == "SecureRestore")
                    {
                        source = task.Element("SecureBackup").Attribute("src").Value;
                        destination = task.Element("SecureBackup").Attribute("dest").Value;
                    }
                }
                Set_Data(name, type, startTime, endTime, repeat, filename, destination);
            }
        }
        
    }
    class SecureBackup
    {
        public string name;
        public string type;
        public string startTime;
        public string endTime;
        public string repeat;

        public string filename;
        public string destination;



        public SecureBackup()
        {
            Get_Data data = new Get_Data();
            data.getData("SecureBackup");

            this.name = data.name;
            this.type = data.type;
            this.startTime = data.startTime;
            this.endTime = data.endTime;
            this.repeat = data.repeat;

            this.filename = data.filename;
            this.destination = data.destination;
        }

        public void Secure_Backup_Task()
        {
            var CurrentTime = DateTime.Now;
            string currentTime = CurrentTime.ToString("yyyy/MM/dd hh:mm");

            while (currentTime != startTime)
            {
                Thread.Sleep(60000);   // Sleep thread for 1 minute
                CurrentTime = DateTime.Now;
                currentTime = CurrentTime.ToString("yyyy/MM/dd hh:mm");
            }

            XDocument doc = XDocument.Load("Output.xml");
            int count = 0;
        while(currentTime != endTime)    
        {
            string fileData = File.ReadAllText(filename);
            string evenBytes = "";
            string oddBytes = "";
            for (int index = 0; index < fileData.Length; index++)
            {
                evenBytes = evenBytes + fileData[index];
                if (index + 1 < fileData.Length)
                {
                    index++;
                    oddBytes = oddBytes + fileData[index];
                }
            }
            File.WriteAllText(destination + "EvenBytes.txt", evenBytes);
            File.WriteAllText(destination + "OddBytes.txt", oddBytes);
                
           // File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "Output.txt", File.ReadAllText("Output.txt") + count++ + " Thank you \n");
            
            if (repeat.Contains("d"))
            {
                repeat = repeat.Replace("d", "");
                Thread.Sleep(Int32.Parse(repeat) * 24 * 60 * 60 * 1000);
            }
            if (repeat.Contains("h"))
            {
                repeat = repeat.Replace("h", "");
                Thread.Sleep(Int32.Parse(repeat) * 60 * 60 * 1000);
            }
            if (repeat.Contains("m"))
            {
                repeat = repeat.Replace("m", "");
                Thread.Sleep(Int32.Parse(repeat) * 60 * 1000);
            }
            doc.Element("Results").Add(new XElement("TaskRun", new XAttribute("TaskName", name), new XAttribute("Type", type), new XAttribute("Time", currentTime), new XElement("TaskResult", new XAttribute("Result", "Task Completed"))));
            doc.Save("Output.xml");
            CurrentTime = DateTime.Now;
            currentTime = CurrentTime.ToString("yyyy/MM/dd hh:mm");
        }
            
            
        }
        
        

    }

    class SecureRestore
    {   
        public string name;
        public string type;
        public string startTime;
        public string endTime;
        public string repeat;

        public string source;
        public string destination;

        public SecureRestore()
        {
            Get_Data data = new Get_Data();
            data.getData("SecureRestore");

            this.name = data.name;
            this.type = data.type;
            this.startTime = data.startTime;
            this.endTime = data.endTime;
            this.repeat = data.repeat;

            this.source = data.source;
            this.destination = data.destination;

        }

        public void Secure_Restore_Task()
        {

            var CurrentTime = DateTime.Now;
            string currentTime = CurrentTime.ToString("yyyy/MM/dd hh:mm");

            while (currentTime != startTime)
            {
                Thread.Sleep(60000);   // Sleep thread for 1 minute
                CurrentTime = DateTime.Now;
                currentTime = CurrentTime.ToString("yyyy/MM/dd hh:mm");
            }
            
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "Output.txt",  startTime + " " );

            while (currentTime != endTime)
            {
                string evenbytes = File.ReadAllText(source + "EvenBytes.txt");
                string oddbytes = File.ReadAllText(source + "oddBytes.txt");

                string fileData = "";
                int count = 0;
                for (int index = 0; index < (evenbytes.Length + oddbytes.Length); index++)
                {
                    if (index % 2 == 0)
                    {
                        fileData += evenbytes[count];
                    }
                    else
                    {
                        fileData += oddbytes[count];
                        count++;
                    }
                }
                File.WriteAllText(source + "fileData.txt", fileData);

                if (repeat.Contains("d"))
                {
                    repeat = repeat.Replace("d", "");
                    Thread.Sleep(Int32.Parse(repeat) * 24 * 60 * 60 * 1000);
                }
                if (repeat.Contains("h"))
                {
                    repeat = repeat.Replace("h", "");
                    Thread.Sleep(Int32.Parse(repeat) * 60 * 60 * 1000);
                }
                if (repeat.Contains("m"))
                {
                    repeat = repeat.Replace("m", "");
                    Thread.Sleep(Int32.Parse(repeat) * 60 * 1000);
                }
                
                CurrentTime = DateTime.Now;
                currentTime = CurrentTime.ToString("yyyy/MM/dd hh:mm");
            
            }

            /*XDocument doc = XDocument.Load("Output.xml");
            doc.Element("Results").Add(new XElement("TaskRun", new XAttribute("TaskName", name), new XAttribute("Type", type), new XAttribute("Time", currentTime), new XElement("TaskResult", new XAttribute("Result", "Task Completed"))));
            doc.Save("Output.xml");
            */
        }

    }
    class ProcessesRunning
    {
        public string name;
        public string type;
        public string startTime;
        public string endTime;
        public string repeat;

        public ProcessesRunning()
        {
            Get_Data data = new Get_Data();
            data.getData("ProcessesRunning");

            this.name = data.name;
            this.type = data.type;
            this.startTime = data.startTime;
            this.endTime = data.endTime;
            this.repeat = data.repeat;           
        }

        public void ProcessesRunning_Task()
        {   
            var CurrentTime = DateTime.Now;
            string currentTime = CurrentTime.ToString("yyyy/MM/dd hh:mm");

            while (currentTime != startTime)
            {
                Thread.Sleep(60000);   // Sleep thread for 1 minute
                CurrentTime = DateTime.Now;
                currentTime = CurrentTime.ToString("yyyy/MM/dd hh:mm");
            }

            while (currentTime != endTime)
            {
                Process[] processlist = Process.GetProcesses();
                StreamWriter sw = File.AppendText("ProcessRunning.txt");
                foreach (Process theprocess in processlist)
                {
                    sw.WriteLine(theprocess.Id + " " + theprocess.ProcessName + " " + theprocess.WorkingSet64 + " " + theprocess.TotalProcessorTime);
                }

                if (repeat.Contains("d"))
                {
                    repeat = repeat.Replace("d", "");
                    Thread.Sleep(Int32.Parse(repeat) * 24 * 60 * 60 * 1000);
                }
                if (repeat.Contains("h"))
                {
                    repeat = repeat.Replace("h", "");
                    Thread.Sleep(Int32.Parse(repeat) * 60 * 60 * 1000);
                }
                if (repeat.Contains("m"))
                {
                    repeat = repeat.Replace("m", "");
                    Thread.Sleep(Int32.Parse(repeat) * 60 * 1000);
                }
                CurrentTime = DateTime.Now;
                currentTime = CurrentTime.ToString("yyyy/MM/dd hh:mm");
            }
            
        }
    }
    
    public partial class Service1 : ServiceBase
    {
        
        public Service1()
        {
            InitializeComponent();
        }
        
        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            SecureBackup createBackup = new SecureBackup();
            SecureRestore createRestore = new SecureRestore();
            ProcessesRunning processesRunning = new ProcessesRunning();
            
            
            Thread SecureBackupThread = new Thread(createBackup.Secure_Backup_Task);
            SecureBackupThread.Start();
            
            Thread SecureRestoreThread = new Thread(createRestore.Secure_Restore_Task);
            SecureRestoreThread.Start();
            
            Thread RunningProcessThread = new Thread(processesRunning.ProcessesRunning_Task);
            RunningProcessThread.Start();

            //File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "Output.txt", formattedTime + " " + taskList[0]);
        }

        protected override void OnStop()
        {
        }
    }
}
