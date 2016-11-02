using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedisWin
{
    public partial class Form1 : Form
    {
        private ConnectionMultiplexer cm = null;
        private IDatabase db;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.Enabled = false;
            try
            {
                var options = new ConfigurationOptions
                {
                    EndPoints =
                {
                    { txtServer.Text, (int)txtPort.Value }
                },
                    Password = txtPwd.Text,
                    AllowAdmin = true,
                    ClientName = txtClientName.Text
                };

                cm = ConnectionMultiplexer.Connect(options);
                db = cm.GetDatabase();
            }
            finally
            {
                tabControl1.Enabled = cm != null && cm.IsConnected;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            txtValue.Text = db.StringGet(txtKey.Text);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            db.StringSet(txtKey.Text, txtValue.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            txtValue.Text = db.KeyTimeToLive(txtKey.Text).ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var when = new TimeSpan(0, 0, int.Parse(txtValue.Text));
            db.KeyExpire(txtKey.Text, when);
        }
    }
}
