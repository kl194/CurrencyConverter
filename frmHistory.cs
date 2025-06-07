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

namespace Converter
{
    public partial class frmHistory : Form
    {
        public frmHistory()
        {
            InitializeComponent();
        }

        private void LoadHistoryToGrid(string filePath)
        {
            dgvHistory.Rows.Clear();
            dgvHistory.Columns.Clear();

            dgvHistory.Columns.Add("date", "Date and time");
            dgvHistory.Columns.Add("from", "From");
            dgvHistory.Columns.Add("amount", "Value");
            dgvHistory.Columns.Add("to", "To");
            dgvHistory.Columns.Add("result", "Result");

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    string[] parts = line.Split(';');
                    if (parts.Length == 5)
                    {
                        dgvHistory.Rows.Add(parts[0], parts[1], parts[2], parts[3], parts[4]);
                    }
                }
            }
        }

        private void ClearHistory(string filePath)
        {
            File.WriteAllText(filePath, string.Empty);
            LoadHistoryToGrid(filePath);
            SetGrid();
        }

        public void SetGrid() 
        {
            dgvHistory.Columns[0].Width = 120;
            dgvHistory.Columns[1].Width = 40;
            dgvHistory.Columns[2].Width = 60;
            dgvHistory.Columns[3].Width = 40;
            dgvHistory.Columns[4].Width = 60;
        }

        private void frmHistory_Load(object sender, EventArgs e)
        {
            string filePath = "history.txt";
            
            LoadHistoryToGrid(filePath);
            SetGrid();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            string filePath = "history.txt";

            ClearHistory(filePath);
        }
    }
}
