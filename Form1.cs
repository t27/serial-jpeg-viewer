using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.IO;
namespace SimpleSerial
{
    public partial class Form1 : Form
    {
        // Add this variable 
        string RxString;
        int read_next_data;
        int fps;
        public Form1()
        {
            read_next_data = 0;
            fps = 0;
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            serialPort1.PortName = "COM3";
            serialPort1.BaudRate = 2000000;//1048576;//

            serialPort1.Open();
            if (serialPort1.IsOpen)
            {
                buttonStart.Enabled = false;
                buttonStop.Enabled = true;
                textBox1.ReadOnly = false;
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
                textBox1.ReadOnly = true;
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If the port is closed, don't try to send a character.
            if (!serialPort1.IsOpen) return;

            // If the port is Open, declare a char[] array with one element.
            char[] buff = new char[1];

            // Load element 0 with the key character.
            buff[0] = e.KeyChar;

            // Send the one character buffer.
            serialPort1.Write(buff, 0, 1);

            // Set the KeyPress event as handled so the character won't
            // display locally. If you want it to display, omit the next line.
            e.Handled = true;
        }

        private void DisplayText(object sender, EventArgs e)
        {
            textBox1.AppendText(RxString+"\n");
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (read_next_data != 1)
            {
                RxString = serialPort1.ReadLine();
                if (RxString.Contains("jpg"))
                {
                    read_next_data = 1;
                }
                this.Invoke(new EventHandler(DisplayText));
            }
            else
            {//Now we will read the jpeg data

                RxString = serialPort1.ReadLine();
                //Console.Write(RxString);
                string[] strbytes = RxString.Split(',');
                List<byte> bytes1 = new List<byte>();
                //byte[] bytes = strbytes.Select(s => Convert.ToByte(s, 16)).ToArray();
                for (int i = 0; i < strbytes.Length; i++)
                {
                    if (strbytes[i] == "")
                    {
                        bytes1.Add(0);
                    }
                    else
                    {
                        try
                        {
                            bytes1.Add(Convert.ToByte(strbytes[i], 16));
                        }
                        catch (Exception ex)
                        {
                            bytes1.Add(0);
                        }
                    }

                }

                var ms = new MemoryStream(bytes1.ToArray());
                pictureBox1.Image = Image.FromStream(ms);

                read_next_data = 0;
                fps++;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = fps.ToString();
            fps = 0;
        }
    }
}