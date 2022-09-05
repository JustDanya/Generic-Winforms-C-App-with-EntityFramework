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


    public partial class Form1 : Form
    {
        Auth authForm;

        public Form1(Auth a)
        {
            InitializeComponent();
            TechCon HC = new TechCon();
            HC.Dock = DockStyle.Fill;
            panel1.Controls.Add(HC);
            authForm = a;
        }

        private void DestroyView()
        {
            if (panel1.Controls.Count > 0)
            {
                UserControl oldView = panel1.Controls[0] as UserControl;
                panel1.Controls.Remove(oldView);
                oldView.Dispose();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DestroyView();
            TechCon HC = new TechCon();
            HC.Dock = DockStyle.Fill;
            panel1.Controls.Add(HC);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DestroyView();
            UnitsCon HC = new UnitsCon();
            HC.Dock = DockStyle.Fill;
            panel1.Controls.Add(HC);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DestroyView();
            EmployeesCon HC = new EmployeesCon();
            HC.Dock = DockStyle.Fill;
            panel1.Controls.Add(HC);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Session.DestroySession();
            authForm.Show();
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            authForm.Show();
        }
    }
}
