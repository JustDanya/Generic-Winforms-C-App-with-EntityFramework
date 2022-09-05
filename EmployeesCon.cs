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
    public partial class EmployeesCon : UserControl
    {
        PM01Entities conn = new PM01Entities();

        List<string> searchModes = new List<string>() { "Все", "Только работающие", "Только уволеные", "Руковолители отделов" };

        public EmployeesCon()
        {
            InitializeComponent();
            dataGridView1.DataSource = conn.Employees.ToList();
        }

        void StyleDataGrid()
        {
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "табельный номер";
            dataGridView1.Columns[2].HeaderText = "ФИО";
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].HeaderText = "Статус Работника";
            dataGridView1.Columns[6].HeaderText = "Подразделение";
            dataGridView1.Columns[7].Visible = false;
            dataGridView1.Columns[8].Visible = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var item = row.DataBoundItem as Employees;
                if (item.Status.Trim() == "DSCH")
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                }
                //MessageBox.Show(item.UnitLeaders.Count.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //+
            NUEmployee f = new NUEmployee();
            f.ShowDialog();
            dataGridView1.DataSource = conn.Employees.ToList();
            StyleDataGrid();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ch
            if (dataGridView1.SelectedRows.Count > 0)
            {
                Employees item = dataGridView1.SelectedRows[dataGridView1.SelectedRows.Count - 1].DataBoundItem as Employees;
                NUEmployee f = new NUEmployee(conn, item);
                f.ShowDialog();
                dataGridView1.DataSource = conn.Employees.ToList();
                StyleDataGrid();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //-
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                var item = row.DataBoundItem as Employees;
                if (item.Tech.Count > 0)
                    MessageBox.Show("У пользователя имеется техника!!!", "Ошибка!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (item.Unit_ID != null || item.UnitLeaders.Count > 0)
                    MessageBox.Show("Пользователь состоит в подразделении или является лидером подразделения!!!", "Ошибка!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    if (item.Unit_ID != null)
                        item.Units.EmployeesCount--;
                    conn.Employees.Remove(item);
                }

            }
            conn.SaveChanges();
            dataGridView1.DataSource = conn.Employees.ToList();
            StyleDataGrid();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //ch status
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                var item = row.DataBoundItem as Employees;
                if (item.Status.Trim() == "WRK")
                    item.Status = "DSCH";
                else
                    item.Status = "WRK";
            }
            conn.SaveChanges();
            dataGridView1.DataSource = conn.Employees.ToList();
            StyleDataGrid();
        }

        private void EmployeesCon_Load(object sender, EventArgs e)
        {
            StyleDataGrid();
            foreach(string sm in searchModes)
                comboBox1.Items.Add(sm);
            comboBox1.SelectedIndex = 0;
            var units = conn.Units.ToList();
            foreach(Units u in units)
                comboBox2.Items.Add(u);
            comboBox2.Items.Insert(0, "Все");
            comboBox2.SelectedIndex = 0;
            if (!Session.authority)
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSearch();
        }

        void UpdateSearch()
        {
            string fio = textBox1.Text.Trim(), tn = textBox2.Text.Trim();
            var items = conn.Employees.ToList();
            if (comboBox2.SelectedIndex > 0)
            {
                var unit = comboBox2.SelectedItem as Units;
                items = unit.Employees.ToList();
            }
            switch(comboBox1.SelectedIndex)
            {
                case 0:
                    //do nothing
                    break;
                case 1:
                    items = items.Where(i => i.Status == "WRK").ToList();
                    break;
                case 2:
                    items = items.Where(i => i.Status == "DSCH").ToList();
                    break;
                case 3:
                    items = items.Where(i => i.UnitLeaders.Count == 1).ToList();
                    break;
            }
            items = items.Where(i => i.FIO.Contains(fio)).ToList();
            items = items.Where(i => i.Tabel_number.Contains(tn)).ToList();

            dataGridView1.DataSource = items;
            StyleDataGrid();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            UpdateSearch();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateSearch();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSearch();
        }
    }
}
