using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PM01
{
    public partial class TechCon : UserControl
    {
        PM01Entities conn = new PM01Entities();

        public TechCon()
        {
            InitializeComponent();
            dataGridView1.DataSource = conn.Tech.ToList();
        }

        void StyleDataGrid()
        {
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Серийный номер";
            dataGridView1.Columns[2].HeaderText = "Название";
            dataGridView1.Columns[3].HeaderText = "Описание";
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[6].HeaderText = "Работник";

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var item = row.DataBoundItem as Tech;
                if (item.Employees.Status.Trim() == "DSCH")
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                }
            }
        }

        void StyleDataAttachGrid()
        {
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[1].HeaderText = "Файл";
            dataGridView2.Columns[2].Visible = false;
            dataGridView2.Columns[3].Visible = false;

            dataGridView1.Columns[3].HeaderText = "Техника";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //+
            NUTech f = new NUTech();
            f.ShowDialog();
            dataGridView1.DataSource = conn.Tech.ToList();
            StyleDataGrid();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                Tech item = dataGridView1.SelectedRows[dataGridView1.SelectedRows.Count-1].DataBoundItem as Tech;
                NUTech f = new NUTech(item, conn);
                f.ShowDialog();
                dataGridView1.DataSource = conn.Tech.ToList();
                StyleDataGrid();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //-
            foreach(DataGridViewRow row in dataGridView1.SelectedRows)
            {
                var item = row.DataBoundItem as Tech;
                if (item.Attachments.Count > 0)
                    MessageBox.Show("У техники имеются прикрепленные документы!!!", "Ошибка!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    conn.Tech.Remove(item);
            }
            conn.SaveChanges();
            dataGridView1.DataSource = conn.Tech.ToList();
            StyleDataGrid();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //+ attach
            if (dataGridView1.SelectedRows.Count == 0) return;

            Tech item = dataGridView1.SelectedRows[dataGridView1.SelectedRows.Count - 1].DataBoundItem as Tech;

            var filePath = string.Empty;
            var targetDir = @"C:\Users\Public\Documents\Attachments\" + item.Serial;
            var onlyFname = string.Empty;

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "doc files (*.doc)|*.doc|docx files (*.docx)|*.docx|pdf files (*.pdf)|*.pdf";
                openFileDialog.FilterIndex = 3;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    onlyFname = openFileDialog.SafeFileName;
                    targetDir += @"\" + onlyFname;
                    //MessageBox.Show(onlyFname);
                    File.Copy(filePath, targetDir, true);

                    conn.Attachments.Add(new Attachments() { File_name = targetDir, Tech_ID = item.ID});
                    conn.SaveChanges();
                    dataGridView2.DataSource = item.Attachments.ToList();
                    StyleDataAttachGrid();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //- attach
            foreach (DataGridViewRow row in dataGridView2.SelectedRows)
            {
                var item = row.DataBoundItem as Attachments;
                if (File.Exists(item.File_name))
                    File.Delete(item.File_name);
                conn.Attachments.Remove(item);
            }
            conn.SaveChanges();
            Tech tch = dataGridView1.SelectedRows[dataGridView1.SelectedRows.Count - 1].DataBoundItem as Tech;
            dataGridView2.DataSource = tch.Attachments.ToList();
            StyleDataAttachGrid();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //open attach
            if (dataGridView2.SelectedRows.Count > 0)
            {
                var item = dataGridView2.SelectedRows[dataGridView2.SelectedRows.Count - 1].DataBoundItem as Attachments;
                System.Diagnostics.Process.Start(item.File_name);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
            Tech item = dataGridView1.SelectedRows[dataGridView1.SelectedRows.Count - 1].DataBoundItem as Tech;
            dataGridView2.DataSource = item.Attachments.ToList();
            StyleDataAttachGrid();
        }

        private void TechCon_Load(object sender, EventArgs e)
        {
            StyleDataGrid();
            if (!Session.authority)
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
            }
        }
    }
}
