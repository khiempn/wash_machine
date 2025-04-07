using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;

namespace WashMachine.Forms.Services.Machine
{
    public class MachineManager
    {
        private static List<SerialPort> machines { get; set; } = new List<SerialPort>();

        private static void Add(SerialPort machine)
        {
            if (!machines.Exists(a => a.PortName.Equals(machine.PortName)))
            {
                machines.Add(machine);
            }
        }

        private static SerialPort GetMachine(string portName)
        {
            return machines.FirstOrDefault(a => a.PortName.Equals(portName));
        }

        public static SerialPort TryGetMachine(string portName, int baudRate, int data, Parity parity = Parity.None, StopBits step = StopBits.One)
        {
            try
            {
                SerialPort serialPort = GetMachine(portName);
                if (serialPort == null)
                {
                    serialPort = new SerialPort(portName, baudRate, parity, data, step);
                    Add(serialPort);
                }

                return serialPort;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return null;
        }

        public static void TryDisconnectAll()
        {
            try
            {
                foreach (SerialPort serialPort in machines)
                {
                    if (serialPort.IsOpen)
                    {
                        serialPort.Close();
                        serialPort.Dispose();
                    }
                }
                machines.Clear();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void Disconnect(string portName, int baudRate = 0)
        {
            try
            {
                SerialPort serialPort = machines.FirstOrDefault(f => f.PortName == portName);
                if (serialPort != null)
                {
                    serialPort.Close();
                    serialPort.Dispose();
                    machines.Remove(serialPort);
                }
                else
                {
                    serialPort = new SerialPort(portName, baudRate);
                    serialPort.Close();
                    serialPort.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void Reset(string portName)
        {
            try
            {
                string query = "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%" + portName + "%'";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                foreach (ManagementObject obj in searcher.Get())
                {
                    obj.InvokeMethod("Disable", null);
                    System.Threading.Thread.Sleep(2000);
                    obj.InvokeMethod("Enable", null);
                    System.Threading.Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
