using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net.Http;
using System.Text.Json;
using System.Globalization;
using System.IO;


namespace Converter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string[] currencies = { "USD", "EUR", "UAH", "PLN", "CZK", "GBP" };
            cbFrom.Items.AddRange(currencies);
            cbTo.Items.AddRange(currencies);
            cbFrom.SelectedIndex = 0;
            cbTo.SelectedIndex = 1;
        }

        private async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency, decimal amount)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiKey = "9321af51d8b203b8a00c532ac783e0e6";
                string url = $"https://api.exchangerate.host/convert?from={fromCurrency}&to={toCurrency}&amount={amount.ToString(CultureInfo.InvariantCulture)}&access_key={apiKey}";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        JsonElement root = doc.RootElement;
                        if (root.TryGetProperty("result", out JsonElement rateElement))
                        {
                            return rateElement.GetDecimal();
                        }
                    }
                }
            }

            throw new Exception("Unable to retrieve exchange rate.");
        }


        private void btnSwap_Click(object sender, EventArgs e)
        {
            var t1 = cbFrom.SelectedItem;
            cbFrom.SelectedItem = cbTo.SelectedItem;
            cbTo.SelectedItem = t1;

            string t2 = tbTo.Text;
            tbTo.Text = tbFrom.Text;
            tbFrom.Text = t2;
        }

        private void tbFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
            {
                e.Handled = true;
            }
        }

        private async void btnConvert_Click(object sender, EventArgs e)
        {
            try
            {
                string input = tbFrom.Text.Replace('.', ',');
                string fromCurrency = cbFrom.SelectedItem.ToString();
                string toCurrency = cbTo.SelectedItem.ToString();

                if (decimal.TryParse(input, NumberStyles.Any, new CultureInfo("uk-UA"), out decimal amount))
                {
                    decimal result = await GetExchangeRateAsync(fromCurrency, toCurrency, amount);
                    tbTo.Text = result.ToString("0.00");
                    string historyLine = $"{DateTime.Now:G};{fromCurrency};{amount};{toCurrency};{result.ToString("0.00")}";
                    string filePath = "history.txt";

                    try
                    {
                        File.AppendAllText(filePath, historyLine + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("History error: " + ex.Message);
                    }

                }
                else
                {
                    MessageBox.Show("Please, enter valid value.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            frmHistory history= new frmHistory();
            history.ShowDialog();
        }
    }
}
