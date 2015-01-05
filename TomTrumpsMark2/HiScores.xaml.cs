using TomTrumpsMark2.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Windows.UI.Popups;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace TomTrumpsMark2
{

    public sealed partial class HiScores : Page
    {
        private MobileServiceCollection<Item, Item> items;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private IMobileServiceTable<Item> todoTable = App.MobileService.GetTable<Item>();
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        List<Item> historyWinners;
        List<Item> all;


        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public HiScores()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;


        }

        private async void RefreshTodoItems(){

            btnView.Visibility = Visibility.Collapsed;
            txtLoad.Visibility = Visibility.Visible;

            MobileServiceInvalidOperationException exception = null;
            try
            {
                historyWinners = await todoTable
                    .Where(todoItem => todoItem.Text != null)
                    .ToListAsync();

            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }
            if (exception != null)
            {
                var dialog = new MessageDialog("error");
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            else
            {
                items = await todoTable.OrderBy(todoItem => todoItem.Moves).ToCollectionAsync();
                ListItems.ItemsSource = items;
                ListItems2.ItemsSource = items;

                imgBronze.Visibility = Visibility.Visible;
                imgSilver.Visibility = Visibility.Visible;
                imgGold.Visibility = Visibility.Visible;
                txtLoad.Visibility = Visibility.Collapsed;
                txtName.Visibility = Visibility.Visible;
                txtRounds.Visibility = Visibility.Visible;
  
            }

         }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }


        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void back_Tapped(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StartPage));
        }
        private void View_Tapped(object sender, RoutedEventArgs e)
        {
            RefreshTodoItems();
        }

    }
}
