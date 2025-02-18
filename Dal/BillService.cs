
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Model;

namespace Dal

{
    public class BillService
    {
        private readonly string rateItemsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data/RateItems.json");

        public List<RateItem> RateItems { get; set; } = new List<RateItem>();
        public List<RateItem> SelectedItems { get; private set; } = new List<RateItem>();

        public decimal TotalAmount => SelectedItems.Sum(item => item.Price);

        public BillService()
        {
            LoadRateItemsFromFile();
        }

        public void AddItem(RateItem item)
        {
            var existingItem = SelectedItems.FirstOrDefault(i => i.Name == item.Name);
            if (existingItem != null)
            {
                existingItem.Quantity++;
                existingItem.Price = existingItem.BasePrice * existingItem.Quantity;
            }
            else
            {
                item.Quantity = 1; // Set the initial quantity to 1
                item.Price = item.BasePrice;
                SelectedItems.Add(item);
            }
            NotifyStateChanged();
        }

        public void RemoveItem(RateItem item)
        {
            SelectedItems.Remove(item);
            NotifyStateChanged();
        }

        public void AddRateItem(RateItem item)
        {
            RateItems.Add(item);
            SaveRateItemsToFile(); // Save to file after adding
            NotifyStateChanged();
        }

        public void ClearAllItems()
        {
            SelectedItems.Clear();
            NotifyStateChanged();
        }

        public void RemoveRateItem(string productName)
        {
            var rateItem = RateItems.FirstOrDefault(x => x.Name == productName);
            if (rateItem != null && !rateItem.IsActive)
            {
                RateItems.Remove(rateItem);
                SaveRateItemsToFile(); // Save to file after removing
                NotifyStateChanged();
            }
        }

        public void ClearTotalAmount()
        {
            foreach (var item in SelectedItems)
            {
                item.Price = 0;
                item.Quantity = 0;
            }
            NotifyStateChanged();
        }

        public void UpdateItem(RateItem updatedItem)
        {
            var item = SelectedItems.FirstOrDefault(i => i.Name == updatedItem.Name);
            if (item != null)
            {
                item.Quantity = updatedItem.Quantity;
                item.Price = item.BasePrice * item.Quantity;
                NotifyStateChanged();
            }
        }

        public event Action? OnChange;

        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }

        // Method to save RateItems to file
        private void SaveRateItemsToFile()
        {
            var rateItemsJson = JsonSerializer.Serialize(RateItems);
            File.WriteAllText(rateItemsFilePath, rateItemsJson);
        }

        // Method to load RateItems from file
        private void LoadRateItemsFromFile()
        {
            if (File.Exists(rateItemsFilePath))
            {
                var rateItemsJson = File.ReadAllText(rateItemsFilePath);
                RateItems = JsonSerializer.Deserialize<List<RateItem>>(rateItemsJson) ?? new List<RateItem>();
            }
        }
    }

}