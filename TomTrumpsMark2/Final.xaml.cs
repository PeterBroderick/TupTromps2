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
using System.Diagnostics;
using Newtonsoft.Json;
using Windows.UI.Popups;
using Windows.ApplicationModel;
using Windows.Storage;
using TomTrumpsMark2.Common;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TomTrumpsMark2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Final : Page
    {
        private IMobileServiceTable<Item> todoTable = App.MobileService.GetTable<Item>();
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private NavigationHelper navigationHelper;
        
        public Final()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TomTrumpsMark2.MainPage.AnotherPagePayload passedParameter = e.Parameter as TomTrumpsMark2.MainPage.AnotherPagePayload;     
            int parameterMovesToAzure = passedParameter.parameterMoves;
            txtOutput.Text = parameterMovesToAzure.ToString();
        }
        public async void submit()
        {
            Item m = new Item();

            m.Moves = Convert.ToInt32(txtOutput.Text);
            var todoItem = new Item
            {
                Text = txtNameInput.Text,
                Moves = m.Moves
            };
            await App.MobileService.GetTable<Item>().InsertAsync(todoItem);  
        }


        
        private void btnYes(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void btnNo(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StartPage));
        }
        public void submit(object sender, TappedRoutedEventArgs e)
        {
            submit();
            btnSubmit.Visibility = Visibility.Collapsed;
        }

        private void Button_Tapped(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HiScores));
        }

       
    }
}
