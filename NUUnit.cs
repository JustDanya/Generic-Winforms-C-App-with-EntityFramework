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
    public partial class NUUnit : Form
    {
        PM01Entities conn = new PM01Entities();

        bool isInsert = true;
        Units currentItem;

        List<Employees> UnitEmploees = new List<Employees>();
        List<Employees> DischargedEmploees = new List<Employees>();

        public NUUnit()
        {
            InitializeComponent();
        }
        public NUUnit(PM01Entities cn, Units item)
        {
            InitializeComponent();
            conn = cn;
            currentItem = item;
            isInsert = false;
            textBox2.Text = item.City;
            textBox1.Text = item.Unit_name;
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
            }
        }

        private void NUUnit_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = UnitEmploees;
            var allEmps = conn.Employees.ToList();
            foreach (Employees em in allEmps)
                comboBox2.Items.Add(em);
            if (allEmps.Count > 0) comboBox2.SelectedIndex = 0;
            if (!isInsert)
            {
                var emps = currentItem.Employees.ToList();
                UnitEmploees = emps;
                dataGridView1.DataSource = UnitEmploees;
                StyleDataGrid();
                if (emps.Count > 0)
                {
                    int index = 0, count = 0;
                    foreach (Employees em in emps)
                    {
                        comboBox1.Items.Add(em);
                        if (!isInsert)
                        {
                            if (em.UnitLeaders.Count > 0 && em.UnitLeaders.First().Unit == currentItem.ID)
                                index = count;
                        }
                        count++;
                    }
                    comboBox1.SelectedIndex = index;
                }
                if (comboBox1.Items.Count <= 0) comboBox1.Enabled = false;
            }
            else
                comboBox1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim(), city = textBox2.Text.Trim();
            int leaderID = -1;
            if (comboBox1.Enabled)
                leaderID = (comboBox1.SelectedItem as Employees).ID;
            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(city))
            {
                MessageBox.Show("Все поля должны быть заполненны!", "Ошибка!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (isInsert)
                {
                    var newItem = new Units() { Unit_name = name, City = city, EmployeesCount = 0 };
                    conn.Units.Add(newItem);
                    conn.SaveChanges();
                    foreach (Employees emp in UnitEmploees)
                    {
                        emp.Unit_ID = newItem.ID;
                    }
                    conn.SaveChanges();
                    newItem.EmployeesCount = newItem.Employees.Count;
                    conn.UnitLeaders.Add(new UnitLeaders() { Leader = leaderID, Unit = newItem.ID});
                }
                else
                {
                    currentItem.City = city;
                    currentItem.Unit_name = name;
                    foreach (Employees emp in UnitEmploees)
                    {
                        emp.Unit_ID = currentItem.ID;
                    }
                    foreach (Employees emp in DischargedEmploees)
                        emp.Unit_ID = null;
                    conn.SaveChanges();
                    currentItem.EmployeesCount = currentItem.Employees.Count;
                    if (currentItem.EmployeesCount > 0 )
                    {
                        if (currentItem.UnitLeaders.Count > 0)
                        {
                            var ul = currentItem.UnitLeaders.First();
                            ul.Leader = leaderID;
                        }
                        else
                        {
                            conn.UnitLeaders.Add(new UnitLeaders() { Leader = leaderID, Unit = currentItem.ID });
                        }
                    }
                    else if (currentItem.UnitLeaders.Count > 0)
                    {
                        conn.UnitLeaders.Remove(currentItem.UnitLeaders.First());
                    }
                }
                conn.SaveChanges();
                Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //+
            Employees item = comboBox2.SelectedItem as Employees;
            if (!UnitEmploees.Where(i => i.ID == item.ID).Any())
            {
                UnitEmploees.Add(item);
                if (!isInsert && currentItem.Employees.Where(i => i.ID == item.ID).Any())
                    DischargedEmploees.Remove(item);
                dataGridView1.DataSource = UnitEmploees.ToList();
                StyleDataGrid();
                comboBox1.Items.Add(item);
                if (!comboBox1.Enabled)
                { 
                    comboBox1.Enabled = true;
                    comboBox1.SelectedIndex = 0;
                } 
            }
            else
                MessageBox.Show("Сотрудник уже добавлен");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //-
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                var item = row.DataBoundItem as Employees;
                UnitEmploees.Remove(item);
                if (!isInsert && currentItem.Employees.Where(i => i.ID == item.ID).Any())
                    DischargedEmploees.Add(item);
                comboBox1.Items.Remove(item);
                if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;
                else
                    comboBox1.Enabled = false;
            }
            dataGridView1.DataSource = UnitEmploees.ToList();
            StyleDataGrid();
        }
    }
}
