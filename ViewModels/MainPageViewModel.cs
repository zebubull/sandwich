﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sandwich.Manager;
using Sandwich.Models;
using Sandwich.Views;

namespace Sandwich.ViewModels
{
    internal class MainPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Pack> Packs { get; private set; }
        private static int _packIndex = 0;
        private static MainPageViewModel instance;
        public MainPageViewModel(IEnumerable<Pack> packs)
        {
            Singleton();
            Packs = new ObservableCollection<Pack>(packs);
            OnPropertyChanged(nameof(Packs));
        }

        private void Singleton()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                throw new Exception("MainPageViewModel Singleton failed...");
            }
        }

        public ICommand SidebarCommand => new Command(ToggleSidebar);
        public ICommand SettingsCommand => new Command<Pack>(OpenSettingsPage);
        public ICommand LaunchCommand => new Command<Pack>(LaunchPack);
        public ICommand NewPackCommand => new Command(AddPack);

        private bool SidebarActive = false;
        async void ToggleSidebar()
        {
            SidebarActive = !SidebarActive;
            MainPage.instance.SetSidebarButtonText(SidebarActive ? "<" : ">");
            await MainPage.instance.MoveSidebar(SidebarActive ? 0 : -224);
        }

        async void OpenSettingsPage(Pack pack)
        {
            _packIndex = Packs.IndexOf(pack);
            await MainPage.instance.NavigateTo(new SettingsPage(pack));
        }

        public static void UpdateCurrentPack(Pack pack)
        {
            instance.Packs[_packIndex] = pack;
        }

        public static void RemoveCurrentPack()
        {
            instance.Packs.RemoveAt(_packIndex);
        }

        void LaunchPack(Pack pack)
        {
            FileManager.InjectProfile(pack);
            Process.Start($"{DataStore.store.GameDirectory}\\minecraft.exe");
        }

        void AddPack()
        {
            Pack p = new Pack(new Manifest(), "");
            Packs.Add(p);
            OnPropertyChanged(nameof(Packs));
            OpenSettingsPage(p);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
