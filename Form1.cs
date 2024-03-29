﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Roasting_HWID_Changer
{
    public partial class Form1 : Form
    {
        const String WIN_10_PATH = @"SYSTEM\CurrentControlSet\Control\IDConfigDB\Hardware Profiles\0001";
        string backupHWID = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private bool mouseDown;
        private Point lastLocation;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void takeInputGUI()
        {
            textBox1.Visible = true;
            textBox1.Enabled = true;
            button2.Visible = false;
            button2.Enabled = false;
            button3.Visible = true;
            button3.Enabled = true;
        }

        private void defualtGUI()
        {
            textBox1.Visible = false;
            textBox1.Enabled = false;
            button2.Visible = true;
            button2.Enabled = true;
            button3.Visible = false;
            button3.Enabled = false;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(label2.Text);
        }

        private void randoMize()
        {
            label2.Text = "{" + generateIDs(8) + "-" + generateIDs(4) + "-" + generateIDs(4) + "-" + generateIDs(4) + "-" + generateIDs(12) + "}";
        }

        private static readonly Random getrandom = new Random();
        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }

        private string generateIDs(int length)
        {
            String ret = "";
            for (int x = 0; x < length; x++)
            {
                if (GetRandomNumber(0, 100) < 60)
                {
                    ret += "" + GetRandomNumber(0, 9);
                }
                else
                {
                    ret += ("" + (char)GetRandomNumber(97, 122)).ToLower();
                }
                //int val = 86;
                //do
                //{
                //    val = GetRandomNumber(48,90);
                //} while (val >= 58 && val <=64);
                //ret += ("" + (char)val).ToLower();
            }
            return ret;
        }

        private void setHWIDKey()
        {
            RegistryKey myKey = Registry.LocalMachine.OpenSubKey(WIN_10_PATH, true);
            if (myKey != null)
            {
                myKey.SetValue("HwProfileGuid", label2.Text, RegistryValueKind.String);
                myKey.Close();
            }
            backupHWID = label2.Text;
        }
        private void randomizeGUI()
        {
            textBox1.Visible = false;
            textBox1.Enabled = false;
            button1.Text = "Set";
            button3.Visible = true;
            button3.Enabled = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            randomizeGUI();
            randoMize();
        }

        private void getHWID()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(WIN_10_PATH))
                {
                    if (key != null)
                    {
                        backupHWID = key.GetValue("HwProfileGuid").ToString();
                        label2.Text = backupHWID;

                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error accessing the Registry... Maybe run as admin?\n\n" + ex.ToString(), "HWID_CHNGER", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (!button1.Text.Equals("Set"))
            {
                button1.Text = "Set";
                takeInputGUI();
            }
            else
            {
                if (label2.Text.Equals(backupHWID))
                {
                    label2.Text = textBox1.Text;
                }
                setHWIDKey();
                defualtGUI();
                button1.Text = "Change";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getHWID();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label2.Text = backupHWID;
            defualtGUI();
            button1.Text = "Change";
        }
    }
}