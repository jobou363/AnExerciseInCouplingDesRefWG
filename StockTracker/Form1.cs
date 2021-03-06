﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
// ReSharper disable SpecifyACultureInStringConversionExplicitly
// ReSharper disable StringIndexOfIsCultureSpecific.1

namespace StockTracker
{
    public partial class Form1 : Form, E_IStockDisplayTable, M_IStockDisplayTable
    {
        private readonly StocksStore _stocksRepository;
        private readonly StockCollection _stockCollection;
        private readonly GainModel _gainModel;
        private readonly V_GetStockPriceDelegate _getStockPrice;
        private StocksStore _stocksStore;

        public Form1(StocksStore stocksStore, GainModel gainModel, V_GetStockPriceDelegate getStockPrice)
        {
            _stocksStore = stocksStore;
            InitializeComponent();

            _stocksRepository = stocksStore;
            _gainModel = gainModel;
            _getStockPrice = getStockPrice;
            _stockCollection = stocksStore.LoadStocks();
            _stockCollection.Changed += (sender, e) => V_StockProcessor.RefreshTable(_stockCollection, _getStockPrice, _listViewStocks);
            _stockCollection.Changed += (sender, e) => SaveStocks();

            V_StockProcessor.RefreshTable(_stockCollection, _getStockPrice, _listViewStocks);
        }

        private void RefreshValues(object sender, EventArgs e)
        {
            E_StockProcessor.RefreshTable(_stocksStore, _gainModel, this);

            V_StockProcessor.RefreshTable(_stockCollection, _getStockPrice, _listViewStocks);

            M_StockProcessor.RefreshTable(_stocksStore, this);

        }

        private void AddTicker(object sender, EventArgs e)
        {
            string ticker = _textBoxTicker.Text.TrimEnd('\n').ToUpper();
            double shares = Double.Parse(_textBoxShares.Text);
            double purchasePrice = Double.Parse(_textBoxPurchasePrice.Text);
            string purchaseDate = _textBoxPurchaseDate.Text;

            _stockCollection.Add(ticker, shares, purchasePrice, purchaseDate);
            _textBoxTicker.Text = String.Empty;
            _textBoxShares.Text = String.Empty;
            _textBoxPurchaseDate.Text = String.Empty;
            _textBoxPurchasePrice.Text = String.Empty;
        }

        private void SaveStocks()
        {
            _stocksRepository.SaveStocks(_stockCollection.EnumerateStocks());
        }

        private void DeleteStock(object sender, EventArgs e)
        {
            int index = _listViewStocks.SelectedIndices[0];
            if (index != -1)
            {
                _stockCollection.RemoveAt(index);
            }
        }

        private void ClearAllData(object sender, EventArgs e)
        {
            _stockCollection.RemoveAll();
        }

        public void AddItemToList(params object[] parameters)
        {
            var listViewItem = new ListViewItem(parameters[0].ToString());

            for (int i = 1; i < new[] {parameters}.Length; i++)
            {
                listViewItem.SubItems.Add(new[] {parameters}[i].ToString());
            }
            _listViewStocks.Items.Add(listViewItem);
        }

        public void ClearList()
        {
            _listViewStocks.Items.Clear();
        }

        public void Render(IEnumerable<M_StockDisplayData> stockDisplayDatas)
        {
            _listViewStocks.Items.Clear();

            foreach (M_StockDisplayData stockDisplayData in stockDisplayDatas)
            {
                var listViewItem = new ListViewItem(stockDisplayData.Items[0]);

                for (int i = 1; i < stockDisplayData.Items.Count; i++)
                {
                    listViewItem.SubItems.Add(stockDisplayData.Items[i].ToString());
                }
                _listViewStocks.Items.Add(listViewItem);
            }
        }
    }
}
