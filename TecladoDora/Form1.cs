using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;


namespace TecladoDora
{
    public partial class Form1 : Form
    {
        private bool btn1;
        private bool btn2;
        private bool btn3;
        private bool btn4;
        private bool btn5;
        private bool btn6;
        private bool btn7;
        private bool btn8;
        private bool btnEnter;
        private string record;

        private Thread buttonReader;
        private Thread readSerial;
        private DateTime millis = DateTime.Now;

        public Form1()
        {
            InitializeComponent();

            btn1 = false;
            btn2 = false;
            btn3 = false;
            btn4 = false;
            btn5 = false;
            btn6 = false;
            btn7 = false;
            btn8 = false;
            btnEnter = false;

            readSerial = new Thread(new ThreadStart(Serial));
            buttonReader = new Thread(readButtons);
            buttonReader.Start();
        }

        void Serial()
        {
            while (true)
            {
                lblMostra.Text += SPort.ReadLine();
                lblMostra.Text += "\r\n";
            }
        }

        private void sendData(string data)
        {
            transformingDataToString(data);
            SPort.WriteLine(data);
        }

        private string transformingDataToString(string data)
        {
            if (btn1) { data = "btn1On"; }
            return data;
        }

        private void readButtons()
        {
            for (;;)
            {
                if (InvokeRequired) Invoke(new Action(() => updateLabel(buttonsPressed())));
            }
        }

        private void updateLabel(string buttons)
        {
            lblMostra.Text = buttons;

            if (((TimeSpan)(DateTime.Now - millis)).TotalMilliseconds > 24)
            {
                millis = DateTime.Now;

                if (buttons.Contains("Dó - ")) { c1.Height -= 3; t1.BorderStyle = BorderStyle.Fixed3D; } else { c1.Height += 5; t1.BorderStyle = BorderStyle.FixedSingle; }
                if (buttons.Contains("Ré - ")) { c2.Height -= 3; t2.BorderStyle = BorderStyle.Fixed3D; } else { c2.Height += 5; t2.BorderStyle = BorderStyle.FixedSingle; }
                if (buttons.Contains("Mi - ")) { c3.Height -= 3; t3.BorderStyle = BorderStyle.Fixed3D; } else { c3.Height += 5; t3.BorderStyle = BorderStyle.FixedSingle; }
                if (buttons.Contains("Fá - ")) { c4.Height -= 3; t4.BorderStyle = BorderStyle.Fixed3D; } else { c4.Height += 5; t4.BorderStyle = BorderStyle.FixedSingle; }
                if (buttons.Contains("Sol - ")) { c5.Height -= 3; t5.BorderStyle = BorderStyle.Fixed3D; } else { c5.Height += 5; t5.BorderStyle = BorderStyle.FixedSingle; }
                if (buttons.Contains("Lá - ")) { c6.Height -= 3; t6.BorderStyle = BorderStyle.Fixed3D; } else { c6.Height += 5; t6.BorderStyle = BorderStyle.FixedSingle; }
                if (buttons.Contains("Si - ")) { c7.Height -= 3; t7.BorderStyle = BorderStyle.Fixed3D; } else { c7.Height += 5; t7.BorderStyle = BorderStyle.FixedSingle; }
                if (buttons.Contains("Dó 2 - ")) { c8.Height -= 3; t8.BorderStyle = BorderStyle.Fixed3D; } else { c8.Height += 5; t8.BorderStyle = BorderStyle.FixedSingle; }

            }
        }

        private string buttonsPressed()
        {
            string buttonsPressed = string.Empty;
            string arduinoData = string.Empty;

            if (btn1) { buttonsPressed += "Dó - "; arduinoData += 1; }
            if (btn2) { buttonsPressed += "Ré - "; arduinoData += 2; }
            if (btn3) { buttonsPressed += "Mi - "; arduinoData += 3; }
            if (btn4) { buttonsPressed += "Fá - "; arduinoData += 4; }
            if (btn5) { buttonsPressed += "Sol - "; arduinoData += 5; }
            if (btn6) { buttonsPressed += "Lá - "; arduinoData += 6; }
            if (btn7) { buttonsPressed += "Si - "; arduinoData += 7; }
            if (btn8) { buttonsPressed += "Dó 2 - "; arduinoData += 8; }
            if (btnEnter) { lblStatusTeclado.Text = "Record"; recording(); lblRecording.Text = record; }
            else { lblStatusTeclado.Text = "Stop"; }

            sendData(arduinoData);
            return buttonsPressed;
        }

        private void keyUp(Keys key)
        {
            if (key == Keys.Q) btn1 = false;
            else if (key == Keys.W) btn2 = false;
            else if (key == Keys.E) btn3 = false;
            else if (key == Keys.R) btn4 = false;
            else if (key == Keys.T) btn5 = false;
            else if (key == Keys.Y) btn6 = false;
            else if (key == Keys.U) btn7 = false;
            else if (key == Keys.I) btn8 = false;
        }

        private void keyDown(Keys key)
        {
            if (key == Keys.Q) btn1 = true;
            else if (key == Keys.W) btn2 = true;
            else if (key == Keys.E) btn3 = true;
            else if (key == Keys.R) btn4 = true;
            else if (key == Keys.T) btn5 = true;
            else if (key == Keys.Y) btn6 = true;
            else if (key == Keys.U) btn7 = true;
            else if (key == Keys.I) btn8 = true;
            else if (key == Keys.Enter) btnEnter = !btnEnter;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            keyUp(e.KeyCode);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            keyDown(e.KeyCode);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SPort.Close();
            readSerial.Abort();
            buttonReader.Abort();
        }

        private void findPort()
        {
            for (int i = 0; i <= 20; i++)
            {
                try
                {
                    SPort.PortName = "COM" + i;
                    SPort.Open();
                    ComboPorta.Text = "COM" + i;
                    if (SPort.IsOpen)
                    {
                        break;
                    }
                }
                catch (System.IO.IOException) { }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            findPort();            
            if (SPort.IsOpen) {
                lblStatus.Text = "Conectado";
                SPort.BaudRate = Convert.ToInt32(ComboBaud.Text);
            }
            else lblStatus.Text = "Desconectado";
        }

        private void recording(){

                record += lblMostra.Text;
                lblRecording.Text += record;
            
        }
    }
}
