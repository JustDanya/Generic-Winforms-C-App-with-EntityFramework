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
    public partial class NUTech : Form
    {
        PM01Entities conn = new PM01Entities();

        bool isInsert = true;

        Tech currentItem;

        public NUTech()
        {
            InitializeComponent();
            var empls = conn.Employees.ToList();
            foreach(Employees e in empls)
            {
                comboBox1.Items.Add(e);
            }
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }

        public NUTech(Tech item, PM01Entities cn)
        {
            InitializeComponent();
            isInsert = false;
            var empls = conn.Employees.ToList();
            int si = 0, count = 0;
            foreach (Employees e in empls)
            {
                comboBox1.Items.Add(e);
                if (e.ID == item.Employee_ID)
                    si = count;
                count++;
            }
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = si;
            currentItem = item;
            textBox1.Text = currentItem.Serial.Trim();
            textBox2.Text = currentItem.Name.Trim();
            richTextBox1.Text = currentItem.Description.Trim();
            conn = cn;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sn = textBox1.Text.Trim(), name = textBox2.Text.Trim(), desc = richTextBox1.Text.Trim();
            int empID = (comboBox1.SelectedItem as Employees).ID;
            if (String.IsNullOrEmpty(sn) || String.IsNullOrEmpty(name) || String.IsNullOrEmpty(desc))
            {
                MessageBox.Show("Все поля должны быть заполненны!", "Ошибка!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (conn.Tech.Where(i => i.Serial == sn).Any() && isInsert)
            {
                MessageBox.Show("Серийный номер должен быть уникальным!", "Ошибка!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!isInsert && conn.Tech.Where(i => i.Serial == sn).Any() && sn != currentItem.Serial)
            {
                MessageBox.Show("Серийный номер должен быть уникальным!", "Ошибка!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (isInsert)
                {
                    conn.Tech.Add(new Tech() { Serial = sn, Name = name, Description = desc, Employee_ID = empID});
                    conn.SaveChanges();
                }
                else
                {
                    currentItem.Serial = sn;
                    currentItem.Name = name;
                    currentItem.Description = desc;
                    currentItem.Employee_ID = empID;
                    conn.SaveChanges();
                }
                this.Close();
            }
        }
    }
}
