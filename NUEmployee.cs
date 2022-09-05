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
    public partial class NUEmployee : Form
    {
        PM01Entities conn = new PM01Entities();

        bool isInsert = true;
        Employees currentItem;

        public NUEmployee()
        {
            InitializeComponent();
        }
        public NUEmployee(PM01Entities cn, Employees emp)
        {
            InitializeComponent();
            isInsert = false;
            currentItem = emp;
            textBox1.Text = emp.FIO.Trim();
            textBox2.Text = emp.Tabel_number.Trim();
            conn = cn;
        }

        private void NUEmployee_Load(object sender, EventArgs e)
        {
            int index = 0, count = 0;
            var units = conn.Units.ToList();
            foreach(Units un in units)
            {
                comboBox1.Items.Add(un);
                if (!isInsert)
                {
                    if (un.ID == currentItem.Unit_ID)
                        index = count;
                }
                count++;
            }
            if (units.Count > 0)
                comboBox1.SelectedIndex = index;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //
            string fio = textBox1.Text.Trim(), serial = textBox2.Text.Trim(); 
            int unId = (comboBox1.SelectedItem as Units).ID;
            var newUnit = conn.Units.Where(i => i.ID == unId).First();
            if (String.IsNullOrEmpty(fio) || String.IsNullOrEmpty(serial))
            {
                MessageBox.Show("Все поля должны быть заполненны!", "Ошибка!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (conn.Employees.Where(i => i.Tabel_number == serial).Any() && isInsert)
            {
                MessageBox.Show("Табельный номер должен быть уникальным!", "Ошибка!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!isInsert && conn.Employees.Where(i => i.Tabel_number == serial).Any() && serial != currentItem.Tabel_number)
            {
                MessageBox.Show("Табельный номер должен быть уникальным!", "Ошибка!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (isInsert)
                {
                    conn.Employees.Add(new Employees() { Tabel_number = serial, FIO = fio, Unit_ID = unId, Status = "WRK"});
                    newUnit.EmployeesCount++;
                }
                else
                {
                    var oldUnit = currentItem.Units;
                    currentItem.Tabel_number = serial;
                    currentItem.FIO = fio;
                    currentItem.Unit_ID = unId;
                    oldUnit.EmployeesCount--;
                    newUnit.EmployeesCount++;
                }
                conn.SaveChanges();
                this.Close();
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
