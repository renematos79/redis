using Newtonsoft.Json;
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
                if (cm.IsConnected)
                {
                    RefreshContatos();
                }
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

        private void RefreshContatos()
        {
            var values = db.ListRange("contatos");
            var list = new List<Contato>();
            if (values != null)
            {
                values.ToList().ForEach(v =>
                {
                    var contato = JsonConvert.DeserializeObject<Contato>(v);
                    list.Add(contato);
                });
            }

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = list.ToList();
            dataGridView1.Refresh();
        }

        private void btnAddContato_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNome.Text))
            {
                MessageBox.Show("Informe um nome!");
                return;
            }

            var contato = new Contato
            {
                Nome = txtNome.Text,
                Email = txtEmail.Text,
                Telefone = txtTelefone.Text
            };

            var json = JsonConvert.SerializeObject(contato);

            db.ListRightPush("contatos", json);

            RefreshContatos();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cm != null)
            {
                cm.Dispose();
            }
        }

        private void btnDelContato_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var contato = dataGridView1.SelectedRows[0].DataBoundItem as Contato;
                var json = JsonConvert.SerializeObject(contato);
                db.ListRemove("contatos", json);
                RefreshContatos();
            }
        }
    }
}
