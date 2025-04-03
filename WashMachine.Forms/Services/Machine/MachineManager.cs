using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

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
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
