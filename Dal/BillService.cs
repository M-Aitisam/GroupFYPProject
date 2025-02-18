using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Model;

namespace Dal
{
    public class BillService
    {
        private readonly string rateItemsFilePath;

        public List<RateItem> RateItems { get; private set; } = new();
        public List<RateItem> SelectedItems { get; private set; } = new();

        public decimal TotalAmount => SelectedItems.Sum(item => item.Price);

        public event Action? OnChange;

        public BillService()
        {
            rateItemsFilePath = Path.Combine(AppContext.BaseDirectory, "Dal", "RateItems.json");
            EnsureDirectoryExists();
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
                item.Quantity = 1;
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
            SaveRateItemsToFile();
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
                SaveRateItemsToFile();
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

        private void NotifyStateChanged() => OnChange?.Invoke();

        private void SaveRateItemsToFile()
        {
            EnsureDirectoryExists();

            var options = new JsonSerializerOptions { WriteIndented = true };
            var rateItemsJson = JsonSerializer.Serialize(RateItems, options);
            File.WriteAllText(rateItemsFilePath, rateItemsJson);
        }

        private void LoadRateItemsFromFile()
        {
            if (File.Exists(rateItemsFilePath))
            {
                var rateItemsJson = File.ReadAllText(rateItemsFilePath);
                RateItems = JsonSerializer.Deserialize<List<RateItem>>(rateItemsJson) ?? new List<RateItem>();
            }
        }

        private void EnsureDirectoryExists()
        {
            string? directoryPath = Path.GetDirectoryName(rateItemsFilePath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
