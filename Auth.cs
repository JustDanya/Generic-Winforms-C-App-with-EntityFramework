using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PM01
{
    public partial class Auth : Form
    {
        PM01Entities conn = new PM01Entities();

        public Auth()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (conn.Users.Where(i => i.Login == textBox1.Text.Trim() && i.Password == textBox2.Text.Trim()).Any())
            {
                var u = conn.Users.Where(i => i.Login == textBox1.Text.Trim() && i.Password == textBox2.Text.Trim()).First();
                Session.MakeSession(u);
                Form1 mf = new Form1(this);
                mf.Show();
                this.Hide();
            }
            else
                MessageBox.Show("Пользователь не найден!!!", "Ошибка!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 mf = new Form1(this);
            mf.Show();
            this.Hide();
        }
    }
}
