using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;

namespace pizza
{
    public partial class Form1 : Form
    {
        static List<Pizza> pizzaList = new List<Pizza>();
        static BindingSource bindingSource = new BindingSource();
        static List<OrderedPizza> orderList = new List<OrderedPizza>();
        static List<OrderedPizza> AllOrderedList = new List<OrderedPizza>();
        static BindingSource toBeDeliveredList = new BindingSource();
        static string ordersFilePath = "orders.txt";


        public Form1()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string filePath = "pizza.txt";

                if (File.Exists(filePath))
                {
                    StreamReader file = new StreamReader(filePath);
                    while (!file.EndOfStream)
                    {
                        string line = file.ReadLine().Trim();

                        var match = Regex.Match(line, @"^(.*?)(\d+)\sFt\s(\d+)\sFt$");

                        if (match.Success)
                        {
                            string name = match.Groups[1].Value.Trim();
                            int smallerPrice = int.Parse(match.Groups[2].Value);
                            int biggerPrice = int.Parse(match.Groups[3].Value);

                            Pizza pizza = new Pizza(name, smallerPrice, biggerPrice);
                            pizzaList.Add(pizza);
                        }
                    }
                    file.Close();

                    
                    bindingSource.DataSource = pizzaList;
                    pizzaDataGrid.DataSource = bindingSource;

                    
                    pizzaDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    FillComboBoxes();
                    setAllToDefault();
                }
                else
                {
                    MessageBox.Show("Pizza adatok nem találva");
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error reading file: " + ex.Message);
            }

            loadToBeDelivered();
        }

        private void loadToBeDelivered()
        {
            toBeDeliveredList.DataSource = AllOrderedList;
            toBeDeliveredDGRID.DataSource = toBeDeliveredList;
            toBeDeliveredDGRID.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void setAllToDefault()
        {
            pizzaNameCBOX.SelectedIndex = -1;
            toppingsCBOX.SelectedIndex = -1;
            costumerNameTBOX.Clear();
            costumerAddressTBOX.Clear();
            deliverDTP.Value = DateTime.Now;
        }

        
        private void FillComboBoxes()
        {
            pizzaNameCBOX.Items.Clear();
            toppingsCBOX.Items.Clear();

            foreach (var pizza in pizzaList)
            {
                pizzaNameCBOX.Items.Add(pizza.Name);
            }

            
            toppingsCBOX.Items.Add("Sajt");
            toppingsCBOX.Items.Add("Gomba");
            toppingsCBOX.Items.Add("Pepperoni");
            toppingsCBOX.Items.Add("Chilli");
            toppingsCBOX.Items.Add("Tejföl");
            toppingsCBOX.Items.Add("Olivabogyó");
            toppingsCBOX.Items.Add("Áronka");
        }


        private void addBTN_Click(object sender, EventArgs e)
        {
            if (pizzaNameCBOX.SelectedIndex == -1 || toppingsCBOX.SelectedIndex == -1)
            {
                MessageBox.Show("Kérlek válassz pizzát és feltétet.");
                return;
            }

            string selectedPizza = pizzaNameCBOX.SelectedItem.ToString();
            string selectedSize = (smallerRBTN.Checked) ? "Smaller" : "Bigger";
            string selectedToppings = toppingsCBOX.SelectedItem.ToString();
            string customerName = costumerNameTBOX.Text;
            string customerAddress = costumerAddressTBOX.Text;
            DateTime deliverDate = deliverDTP.Value;

            Pizza pizza = pizzaList.FirstOrDefault(p => p.Name == selectedPizza);
            if (pizza != null)
            {
                int price = (selectedSize == "Smaller") ? pizza.Smaller : pizza.Bigger;

                
                if (!customerAddress.Contains("Baja") && !customerAddress.Contains("baja"))
                {
                    price += 500;
                }

                OrderedPizza order = new OrderedPizza(
                    pizza.Name,
                    selectedSize,
                    customerName,
                    customerAddress,
                    selectedToppings,
                    customerName,
                    deliverDate,
                    price
                );

                if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(customerAddress))
                {
                    MessageBox.Show("Nem adtál meg minden adatot.");
                }
                else
                {
                    orderList.Add(order);

                    costOrdersCBOX.Items.Add($"{order.PizzaName} - {order.Size} - {order.Toppings} ({order.CostumerName}) - {order.Price} Ft");

                    MessageBox.Show($"Rendelés: {order.PizzaName}, {order.Size} plusz {order.Toppings}, Ár: {order.Price} Ft");

                    setAllToDefault();
                }
            }
            else
            {
                MessageBox.Show("Nincs ilyen pizza.");
            }
        }


        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void orderBTN_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(ordersFilePath, true)) 
                {
                    foreach (var order in orderList)
                    {
                        AllOrderedList.Add(order);

                        string orderData = $"{order.PizzaName};{order.Size};{order.Toppings};{order.CostumerName};{order.CostumerAddress};{order.DeliverDate};{order.Price}";
                        writer.WriteLine(orderData); 
                    }
                }

                MessageBox.Show("Rendelését sikeresen felvettük!");
                orderList.Clear();
                setAllToDefault();

                LoadEveryOrderGrid(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a rendelés mentése közben: " + ex.Message);
            }
        }
        private void LoadEveryOrderGrid()
        {
            if (File.Exists(ordersFilePath))
            {
                List<OrderedPizza> fileOrders = new List<OrderedPizza>();
                string[] lines = File.ReadAllLines(ordersFilePath);

                foreach (var line in lines)
                {
                    string[] data = line.Split(';');
                    if (data.Length == 7)
                    {
                        OrderedPizza order = new OrderedPizza(
                            data[0], 
                            data[1], 
                            data[3], 
                            data[4], 
                            data[2], 
                            data[3],
                            DateTime.Parse(data[5]), 
                            int.Parse(data[6]) 
                        );
                        fileOrders.Add(order);
                    }
                }

                everyOrderDGRID.DataSource = new BindingSource { DataSource = fileOrders };
                everyOrderDGRID.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void deliveredBTN_Click(object sender, EventArgs e)
        {
            if (toBeDeliveredDGRID.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in toBeDeliveredDGRID.SelectedRows)
                {
                    OrderedPizza selectedOrder = row.DataBoundItem as OrderedPizza;
                    if (selectedOrder != null)
                    {
                        AllOrderedList.Remove(selectedOrder); 
                    }
                }
                MessageBox.Show("Kiszállítva!");
                
                toBeDeliveredList.ResetBindings(false);
            }
            else
            {
                MessageBox.Show("Válasszon ki rendelést!");
            }
        }

    }


    internal class Pizza
    {
        public string Name { get; set; }
        public int Smaller { get; set; }
        public int Bigger { get; set; }

        public Pizza(string name, int smaller, int bigger)
        {
            Name = name;
            Smaller = smaller;
            Bigger = bigger;
        }
    }

    
    internal class OrderedPizza
    {
        public string PizzaName { get; set; }
        public string Size { get; set; }
        public string Toppings { get; set; }
        public string CostumerName { get; set; }
        public string CostumerAddress { get; set; }
        public DateTime DeliverDate { get; set; }
        public int Price { get; set; }

        public OrderedPizza(string name, string size, string costName, string costumerAddress, string toppings, string costumerName, DateTime deliverDate, int price)
        {
            PizzaName = name;
            Size = size;
            CostumerAddress = costumerAddress;
            Toppings = toppings;
            CostumerName = costumerName;
            DeliverDate = deliverDate;
            Price = price;
        }
    }
}
