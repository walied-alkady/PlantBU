// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-23-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="EquipmentsPage.xaml.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.ViewModels;
using Syncfusion.DataSource.Extensions;
using Syncfusion.ListView.XForms;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EquipmentsPage : ContentPage
    {
        GroupResult expandedGroup;
        ToolbarItem Add;
        public EquipmentsPage()
        {
            InitializeComponent();
            try
            {
                (BindingContext as EquipmentsViewModel).Navigation = Navigation;
                (BindingContext as EquipmentsViewModel).page = this;
                (BindingContext as EquipmentsViewModel).sflist = listView;
                if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                    (BindingContext as EquipmentsViewModel).IsEnabled = true;
                else
                    (BindingContext as EquipmentsViewModel).IsEnabled = false;
                listView.CollapseAll();

                searchBar.Text = "";
                if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                {

                    Add = new ToolbarItem()
                    {
                        Text = Properties.Resources.Add,
                        Order = ToolbarItemOrder.Secondary,
                        Priority = 2,
                        StyleId = "Add"
                    };
                    Add.Clicked += ToolbarItem_Clicked;
                    this.ToolbarItems.Add(Add);

                }

            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            
            searchBar = (sender as SearchBar);
            if (searchBar.Text.StartsWith("m:"))
                await Navigation.PushAsync(new MotorsPage());
            if (searchBar.Text.StartsWith("s:"))
                await Navigation.PushAsync(new SensorsPage());
            if (searchBar.Text.StartsWith("o:"))
                await Navigation.PushAsync(new OtherComponentsPage());
            else if (listView.DataSource != null)
            {
                this.listView.DataSource.Filter = FilterEquipments;
                this.listView.DataSource.RefreshFilter();
            }
        }
        private bool FilterEquipments(object obj)
        {
            if (searchBar == null || searchBar.Text == null)
                return true;

            var contacts = obj as Equipment;
            if ((contacts.Code != null ? contacts.Code.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.Description != null ? contacts.Description.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.Shop != null ? contacts.Shop.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.Type != null ? contacts.Type.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 )
                return true;
            else
                return false;
        }
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                string action = "";
                if (sender is ToolbarItem)
                {
                    var proLines = DBManager.realm.All<ProductionLine>().ToList().Select(x => x.Code).ToArray();
                    action = await DisplayActionSheet(Properties.Resources.Select, Properties.Resources.Cancel, null, proLines);
                    if (string.IsNullOrEmpty(action))
                        return;
                    var proLine = DBManager.realm.All<ProductionLine>().Where(x => x.Code == action).First();

                    var newstring = DBManager.realm.All<Plant>().ToList()
                        .Select(x => x.Code).ToList()
                        .Where(y => y.Length > 9)
                        .Select(z => z.Substring(9, 2))
                        .Select(int.Parse).ToList();
                    string newstringName;
                    if (newstring.Any())
                        newstringName = "000AA00AA" + (newstring.Max() > 9 ? (newstring.Max() + 1).ToString() : "0" + (newstring.Max() + 1).ToString());
                    else
                        newstringName = "000AA00AA00";
                    switch ((sender as ToolbarItem).StyleId)
                    {
                        case "Add":
                            Equipment newEq = new Equipment()
                            {
                                Code = newstringName,
                                Description = "New Equipment"
                            };
                            proLine.EquipmentAdd(newEq);
                            await Navigation.PushAsync(new EquipmentPage(newEq, true));

                            break;

                    }

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private void listView_GroupExpanding(object sender, GroupExpandCollapseChangingEventArgs e)
        {
            if (e.Groups.Count > 0)
            {
                var group = e.Groups[0];
                if (expandedGroup == null || group.Key != expandedGroup.Key)
                {
                    foreach (var otherGroup in listView.DataSource.Groups)
                    {
                        if (group.Key != otherGroup.Key)
                        {
                            listView.CollapseGroup(otherGroup);
                        }
                    }
                    expandedGroup = group;
                    listView.ExpandGroup(expandedGroup);
                }
            }
        }
        private void listView_Loaded(object sender, ListViewLoadedEventArgs e)
        {
            listView.CollapseAll();
        }
    }
}