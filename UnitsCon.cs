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
    public partial class UnitsCon : UserControl
    {
        PM01Entities conn = new PM01Entities();

        public UnitsCon()
        {
            InitializeComponent();
            dataGridView1.DataSource = conn.Units.ToList();
            StyleDataGrid();
        }

        void StyleDataGrid()
        {
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Название";
            dataGridView1.Columns[2].HeaderText = "Город";
            dataGridView1.Columns[3].HeaderText = "Количество Сотрудников";
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].Visible = false;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var item = row.DataBoundItem as Units;
                if (item.UnitLeaders.Count > 0)
                {
                    var leader = item.UnitLeaders.First().Employees;
                    if (leader.Status.Trim() == "DSCH")
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //+
            NUUnit f = new NUUnit();
            f.ShowDialog();
            dataGridView1.DataSource = conn.Units.ToList();
            StyleDataGrid();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ch
            if (dataGridView1.SelectedRows.Count > 0)
            {
                Units item = dataGridView1.SelectedRows[dataGridView1.SelectedRows.Count - 1].DataBoundItem as Units;
                NUUnit f = new NUUnit(conn, item);
                f.ShowDialog();
                dataGridView1.DataSource = conn.Units.ToList();
                StyleDataGrid();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //-
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                var item = row.DataBoundItem as Units;
                if (item.Employees.Count <= 0 && item.UnitLeaders.Count <= 0)
                    conn.Units.Remove(item);
                else
                {
                    string message = "Подразделение " + item.Unit_name + " имеет сотрудников или лидера!!!";
                    MessageBox.Show(message, "Ошибка!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            conn.SaveChanges();
            dataGridView1.DataSource = conn.Units.ToList();
            StyleDataGrid();
        }

        void UpdateSearch()
        {
            string name = textBox1.Text.Trim(), city = textBox2.Text.Trim();
            int count = Convert.ToInt32(numericUpDown1.Value);
            var items = conn.Units.ToList();
            items = items.Where(i => i.Unit_name.Contains(name)).ToList();
            items = items.Where(i => i.City.Contains(city)).ToList();
            if (count > 0)
                items = items.Where(i => i.EmployeesCount == count).ToList();

            dataGridView1.DataSource = items;
            StyleDataGrid();
        }

        private void UnitsCon_Load(object sender, EventArgs e)
        {
            StyleDataGrid();
            if (!Session.authority)
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateSearch();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            UpdateSearch();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            UpdateSearch();
        }
    }
}
