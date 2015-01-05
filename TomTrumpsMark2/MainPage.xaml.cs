using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.ApplicationModel;
using Windows.Data.Json;
using TomTrumpsMark2.Common;
using Windows.UI.Xaml.Media.Animation;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.ApplicationSettings;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TomTrumpsMark2
{
     public sealed partial class MainPage : Page
     {
         AnotherPagePayload payload = new AnotherPagePayload();
        
         Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        
         private IMobileServiceTable<Item> item = App.MobileService.GetTable<Item>();
        
        private MediaElement _buzzer;
        
        bool cardOpen = false, leftCardOpen=false, winCheck=false, drawCheck=false, loseCheck=false;
        int drawTwice=0;
        int moves=0;
		
        List<Cards> myListofCardsComp = new List<Cards>();//List of computer's cards
        List<Cards> myListofCards = new List<Cards>();//List of user's cards
        List<Cards> deckCards = new List<Cards>();//Original Deck that will be shuffled and dealt out
        List<Cards> tempList = new List<Cards>();//These lists deal with cards that have been won/lost
        List<Cards> tempList2 = new List<Cards>();//Cards put in here temporarily while they are removed from/added to other lists.
        List<Cards> drawList = new List<Cards>();//When cards draw they are stored here until the winner of next round is determined
        List<Cards> drawList2 = new List<Cards>();//Couldn't reuse templist for this task as there are times when both would be in use resulting in duplicate cards.

        static Random rnd = new Random();

        public class AnotherPagePayload
        {
            public int parameterMoves { get; set; }//the amount of moves taken is passed to final page for later submission to azure.
        }

        public MainPage()
        {      
            this.InitializeComponent();
            //ALl of the buttons are added to event handler
            btnPower.Tapped += myEventHandler;
            btnSpeed.Tapped += myEventHandler;
            btnHeart.Tapped += myEventHandler;
            btnDefence.Tapped += myEventHandler;
            btnChin.Tapped += myEventHandler;
            btnTechnique.Tapped += myEventHandler;
            btnFootwork.Tapped += myEventHandler;
            btnStamina.Tapped += myEventHandler;

            DealCards();//Cards dealt using method below.   
        }

        public async void DealCards()
        {   
            //This method uses the 'Shuffle' method which randomizes the 40 cards in the 'json' list and puts 20 into the user's deck and 20 into the opposition's.          
            await LoadLocalData();//Call in card objects from text file.
            int[] array = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 };//
            Shuffle(array);//40 integers above are shuffled here.

            for (int i = 0; i < 20; i++)
            {
                myListofCards.Add(deckCards[array[i]]);
            }
            for (int i = 20; i < 40; i++)
            {
                myListofCardsComp.Add(deckCards[array[i]]);
            }

            imgMyCard.Source = new BitmapImage(new Uri(myListofCards[0].cardPicture, UriKind.Absolute));//Set left card
            ImageCard.Source = new BitmapImage(new Uri(myListofCardsComp[0].cardPicture, UriKind.Absolute));//Set right card.

            var assets = await Package.Current.InstalledLocation.GetFolderAsync("assets");
            var buzzer = await assets.GetFileAsync("shuffle.wav");
            var stream = await buzzer.OpenAsync(FileAccessMode.Read);

            _buzzer = new MediaElement();
            _buzzer.SetSource(stream, buzzer.ContentType);
        }


        public async Task LoadLocalData()
        {
            //Load card objects using Json.
            var file = await Package.Current.InstalledLocation.GetFileAsync("Data\\boxers.txt");
            var result = await FileIO.ReadTextAsync(file);
            var boxerList = JsonArray.Parse(result);
            CreateBoxerList(boxerList);
        }

        public void CreateBoxerList(JsonArray boxerList)
        {
            //Converting JSON to usable code.
            foreach (var item in boxerList)
            {
                var oneBoxer = item.GetObject();
                Cards nBoxer = new Cards();

                foreach (var key in oneBoxer.Keys)
                {
                    IJsonValue value;
                    if (!oneBoxer.TryGetValue(key, out value))
                        continue;

                    switch (key)
                    {
                        case "cardName":
                            nBoxer.cardName = value.GetString();
                            break;
                        case "cardPicture":
                            nBoxer.cardPicture = value.GetString();
                            break;
                        case "power":
                            nBoxer.power = Convert.ToInt32(value.GetString());
                            break;
                        case "speed":
                            nBoxer.speed = Convert.ToInt32(value.GetString());
                            break;
                        case "heart":
                            nBoxer.heart = Convert.ToInt32(value.GetString());
                            break;
                        case "chin":
                            nBoxer.chin = Convert.ToInt32(value.GetString());
                            break;
                        case "footwork":
                            nBoxer.footwork = Convert.ToInt32(value.GetString());
                            break;
                        case "defence":
                            nBoxer.defence = Convert.ToInt32(value.GetString());
                            break;
                        case "technique":
                            nBoxer.technique = Convert.ToInt32(value.GetString());
                            break;
                        case "stamina":
                            nBoxer.stamina = Convert.ToInt32(value.GetString());
                            break;                       
                    }
                }
                deckCards.Add(nBoxer);
            }
        }

        private async void myEventHandler(object sender, RoutedEventArgs e)
        {
            //Event handler deals with ALL buttons.
            if (leftCardOpen == true)//Buttons can only be clicked while the leftcard is faceup.
            {
               
                if (myListofCardsComp.Count != 0 && myListofCards.Count != 0)
                {
                    rectBlock.Visibility = Visibility.Visible;//ensures that nothing is clickable util you hit continue
                    var assets = await Package.Current.InstalledLocation.GetFolderAsync("assets");
                    var buzzer = await assets.GetFileAsync("cardflip.wav");
                    var stream = await buzzer.OpenAsync(FileAccessMode.Read);

                    _buzzer = new MediaElement();
                    _buzzer.SetSource(stream, buzzer.ContentType);

                    moves++;
                    txtMoves.Text = Convert.ToString(moves);

                    txtNextComp.Visibility = Visibility.Visible;

                    if (cardOpen == false)//If card is face down, user is allowed to overturn it and choose a stat.
                    {
                        if (myListofCards.Count > 0)
                        {
                            imgMyCard.Source = new BitmapImage(new Uri(myListofCards[0].cardPicture, UriKind.Absolute));
                        }
                        else if (myListofCards.Count < 1)
                        {

                        }
                        if (myListofCardsComp.Count > 0)
                        {
                            ImageCard.Source = new BitmapImage(new Uri(myListofCardsComp[0].cardPicture, UriKind.Absolute));

                        }
                        else if (myListofCardsComp.Count <1)
                        {
                        }

                        Button stat = (Button)sender;

                        if (cardOpen == false)
                        {
                            FlipOpen.Begin();
                            cardOpen = true;
                        }


                        if (stat.Name == "btnPower")
                        {                   
                            power();//all stats have their own method. See below.
                        }

                        else if (stat.Name == "btnSpeed")
                        {
                            speed();
                        }

                        else if (stat.Name == "btnHeart")
                        {
                             heart();
                        }

                        else if (stat.Name == "btnDefence")
                        {
                            defence();
                        }

                        else if (stat.Name == "btnChin")
                        {
                            chin();
                        }

                        else if (stat.Name == "btnTechnique")
                        {
                            technique();
                        }

                        else if (stat.Name == "btnFootwork")
                        {
                            footwork();
                        }

                        else if (stat.Name == "btnStamina")
                        {
                            stamina();
                        }
                    }
                    imgBlockCard.Visibility = Visibility.Collapsed;
                }

                //Put this code back in to test!!! It will allow you to see all fighters in both decks.

                //ListLeft.Text = "";
                //ListRight.Text = "";

                //foreach (var myCard in myListofCards)
                //{
                //    ListLeft.Text += myCard.cardName + "\n";
                //}

                //foreach (var myCard2 in myListofCardsComp)
                //{
                //    ListRight.Text += myCard2.cardName + "\n ";
                //}
            }
        }

        private async void InsertTodoItem(Item todoItem)
        {

            await item.InsertAsync(todoItem);

        }

        private void NextPage()
        {
            goToFinalPage();

        }
         
        public static void Shuffle<T>(T[] array)
        {//Shuffle the Deck
            var random = _random;
            for (int i = array.Length; i > 1; i--)
            {
                int j = random.Next(i);
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }

        private void clickHere(object sender, TappedRoutedEventArgs e)
        {   
            //When you draw, you need to click this button before you click continue. 
            tempList.Add(myListofCards[0]);
            myListofCards.RemoveAt(0);
            tempList.RemoveAt(0);
            tempList2.Add(myListofCardsComp[0]);
            myListofCardsComp.RemoveAt(0);
            tempList2.RemoveAt(0);
            btnClickHere.Visibility = Visibility.Collapsed;

            if ((myListofCardsComp.Count > 0) || (myListofCards.Count > 0))
            {
                imgMyCard.Source = new BitmapImage(new Uri(myListofCards[0].cardPicture, UriKind.Absolute));

            }
        }

        private async void Continue_Tapped(object sender, TappedRoutedEventArgs e)
        {
           //Controls whether or not the card animations should start, reverse etc.
            if (cardOpen == true)
            {
                FlipClose.Begin();
                cardOpen = false;
            }
            if ((leftCardOpen == true) && (loseCheck == false))
            {
                FlipCloseLeft.Begin();
                leftCardOpen = false;
                
            }
            else if ((leftCardOpen == true) && (loseCheck == true))
            {
                FlipCloseLeft.Begin();
                leftCardOpen = false;
            }
           
            win.Text = "";
            draw.Text = "";
            lose.Text = "";
            txtNextComp.Text = "";
            
            rectBlock.Visibility = Visibility.Collapsed;//Makes buttons usable
            txtMoveslbl.Visibility = Visibility.Visible;
            txtMoves.Visibility = Visibility.Visible;
			
            if ((winCheck==true)&&(cardOpen == false))//If you won, do this
            {
                imgMyCard.Source = new BitmapImage(new Uri(myListofCards[0].cardPicture, UriKind.Absolute));
                RightCardGoesLeft.Stop();
                RightCardGoesLeft.SetValue(Storyboard.TargetNameProperty, "ImageCoverAnimate");
                RightCardGoesLeft.Begin();
           
            }
            else if (loseCheck == true)//if you lost, do this
            {
                imgMyCard.Source = new BitmapImage(new Uri(myListofCards[0].cardPicture, UriKind.Absolute));
                
                LeftCardGoesRight.Stop();
                LeftCardGoesRight.SetValue(Storyboard.TargetNameProperty, "ImageCoverGoesRight");
                LeftCardGoesRight.Begin();
            }

            winCheck = false;

            var assets = await Package.Current.InstalledLocation.GetFolderAsync("assets");
            var buzzer = await assets.GetFileAsync("cardTurn.wav");
            var stream = await buzzer.OpenAsync(FileAccessMode.Read);

            _buzzer = new MediaElement();
            _buzzer.SetSource(stream, buzzer.ContentType);
            btnContinue.Visibility = Visibility.Collapsed;

            if (myListofCardsComp.Count < 1)
            {
                NextPage();
            }
            else if ((myListofCards.Count < 1))
            {
                this.Frame.Navigate(typeof(Lose));
            }
        }

        static Random _random = new Random();

        
        private async void CardTurns_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //If card is face down, turn it over

            if ((leftCardOpen == false)&& (loseCheck==false))
            {
                FlipOpenLeft.Begin();
                leftCardOpen = true;
            }
            else if ((leftCardOpen == false)&& (loseCheck==true))
            {
                //if you lost the last round, when the card turns over all of the card details are masked.
                imgBlockCard.Visibility = Visibility.Visible;
                FlipOpenLeft.Begin();
                FlipOpenLeftBlock.Begin();
                leftCardOpen = true;
                loseCheck = false;


            }
            var assets = await Package.Current.InstalledLocation.GetFolderAsync("assets");
            var buzzer = await assets.GetFileAsync("cardflip.wav");
            var stream = await buzzer.OpenAsync(FileAccessMode.Read);

            _buzzer = new MediaElement();
            _buzzer.SetSource(stream, buzzer.ContentType);
            
        }
        public void winConditions()
        {
            winCheck = true;

            if (drawCheck == true)
            {
                drawCheck = false;
            }
                if (drawList.Count() != 0)
                {
                    myListofCards.AddRange(drawList);
                    myListofCards.AddRange(drawList2);
                    drawList.Clear();
                    drawList2.Clear();
                    //When you win, take anything that's in the draw pile and then clear the drawpile.
                }
                win.Visibility = Visibility.Visible;
                FlipOpen.Begin();
                cardOpen = true;
                win.Text = "You won the round! \n" + myListofCards[0].cardName.ToString() + " beats " + myListofCardsComp[0].cardName.ToString() + ", \nHit continue to proceed.";
            
                if (myListofCardsComp.Count <1)
                {
                    goToFinalPage();
                   
                }
            
                tempList.Add(myListofCards[0]);
                myListofCards.RemoveAt(0);
                myListofCards.Add(tempList[0]);
                tempList.RemoveAt(0);
                myListofCards.Add(myListofCardsComp[0]);
                myListofCardsComp.RemoveAt(0);          
                btnContinue.Visibility = Visibility.Visible;
                leftCounter.Text = Convert.ToString(myListofCards.Count);
                rightCounter.Text = Convert.ToString(myListofCardsComp.Count);    
        }

        public void goToFinalPage()
        {
              payload.parameterMoves = moves;
              this.Frame.Navigate(typeof(Final), payload);
        }

        public void loseConditions()
        {
            if (drawCheck == true)
            {
                drawCheck = false;
            }

            if (myListofCards.Count == 0)
            {
                this.Frame.Navigate(typeof(Lose));
            }

            else
            {
                if (drawList.Count != 0)
                {
                    myListofCardsComp.AddRange(drawList);
                    myListofCardsComp.AddRange(drawList2);
                    drawList.Clear();
                    drawList2.Clear();
                    
                }

                lose.Visibility = Visibility.Visible;
                lose.Text = "You Lose.\n" + myListofCards[0].cardName.ToString();
                lose.Text += " loses to " + myListofCardsComp[0].cardName.ToString() + ", \nHit continue to proceed.";

                //storing comop card in temp list. Removing comp card from front of list, add it from temp to front of list, remove from temp list
                //adding leftCard to templist, removing from front of left list, adding from templist to back of complist, clear templist.
                tempList.Add(myListofCardsComp[0]);
                ImageCard.Source = new BitmapImage(new Uri(myListofCardsComp[0].cardPicture, UriKind.Absolute));

                myListofCardsComp.RemoveAt(0);
                myListofCardsComp.Add(tempList[0]);
                tempList.RemoveAt(0);
                myListofCardsComp.Add(myListofCards[0]);
                myListofCards.RemoveAt(0);
         
                leftCounter.Text = Convert.ToString(myListofCards.Count);
                rightCounter.Text = Convert.ToString(myListofCardsComp.Count);
                btnContinue.Visibility = Visibility.Visible;
                
            }

            loseCheck = true;

            if (myListofCards.Count < 1)
            {
                this.Frame.Navigate(typeof(Lose));
            }
        }

        public void drawConditions()
        {
            FlipOpen.Begin();
            cardOpen = true;
            drawCheck = true;
            draw.Visibility = Visibility.Visible;
            draw.Text = "Draw! There was nothing to seperate " + myListofCards[0].cardName.ToString();
            draw.Text += " and " + myListofCardsComp[0].cardName.ToString() + ", \nHit continue TWICE to proceed.";

            drawList.Add(myListofCards[0]); 
            drawList2.Add(myListofCardsComp[0]);
                      
            imgMyCard.Source = new BitmapImage(new Uri(drawList[0].cardPicture, UriKind.Absolute));
            ImageCard.Source = new BitmapImage(new Uri(drawList2[0].cardPicture, UriKind.Absolute));

          
            btnContinue.Visibility = Visibility.Visible;
            btnClickHere.Visibility = Visibility.Visible;

            leftCounter.Text = Convert.ToString(myListofCards.Count);
            rightCounter.Text = Convert.ToString(myListofCardsComp.Count);
        }

        //METHODS FOR VARIOUS STATS
        public void power()
        {
            if (myListofCards[0].power != 0)
            {
                if (myListofCards.Count > 0 && myListofCards.Count < 41)//41
                {
                    if (myListofCards[0].power > myListofCardsComp[0].power)
                    {
                        winConditions();
                    }
                    else if (myListofCards[0].power < myListofCardsComp[0].power)
                    {
                        loseConditions();
                    }
                    else drawConditions();                   
                }
            }
        }

        public void speed()
        {
            if (myListofCards.Count > 0 && myListofCards.Count < 41)
            {
                if (myListofCards[0].speed > myListofCardsComp[0].speed)
                {
                    winConditions();
                }
                else if (myListofCards[0].speed < myListofCardsComp[0].speed)
                {
                    loseConditions();
                }

                else 
                    drawConditions();             
            }                    
        }

        public void heart()
        {
            if (myListofCards.Count > 0 && myListofCards.Count < 41)
                {
                    if (myListofCards[0].heart > myListofCardsComp[0].heart)
                    {
                        winConditions();
                    }
                    else if (myListofCards[0].heart < myListofCardsComp[0].heart)
                    {
                        loseConditions();
                    }

                    else    drawConditions();                
                }
            
        
        }

        public void defence()
        {
            if (myListofCards.Count > 0 && myListofCards.Count < 41)
            {
                if (myListofCards[0].defence > myListofCardsComp[0].defence)
                {
                    winConditions();
                }
                else if (myListofCards[0].defence < myListofCardsComp[0].defence)
                {
                    loseConditions();
                }

                else 
                    drawConditions();             
            }
        }

        public void chin()
        {
            if (myListofCards.Count > 0 && myListofCards.Count < 41)
            {
                if (myListofCards[0].chin > myListofCardsComp[0].chin)
                {
                    winConditions();
                }
                else if (myListofCards[0].chin < myListofCardsComp[0].chin)
                {
                    loseConditions();
                }

                else
                    drawConditions();
            }
        }

        public void technique()
        {
            if (myListofCards.Count > 0 && myListofCards.Count < 41)
            {
                if (myListofCards[0].technique > myListofCardsComp[0].technique)
                {
                    winConditions();
                }
                else if (myListofCards[0].technique < myListofCardsComp[0].technique)
                {
                    loseConditions();
                }

                else
                    drawConditions();
            }
        }

        public void footwork()
        {
            if (myListofCards.Count > 0 && myListofCards.Count < 41)
            {
                if (myListofCards[0].footwork > myListofCardsComp[0].footwork)
                {
                    winConditions();
                }
                else if (myListofCards[0].footwork < myListofCardsComp[0].footwork)
                {
                    loseConditions();
                }

                else
                    drawConditions();
            }
        }

        public void stamina()
        {
            if (myListofCards.Count > 0 && myListofCards.Count < 41)
            {
                if (myListofCards[0].stamina > myListofCardsComp[0].stamina)
                {
                    winConditions();
                }
                else if (myListofCards[0].stamina < myListofCardsComp[0].stamina)
                {
                    loseConditions();
                }

                else
                    drawConditions();
            }
        }


        private async void btnBack(object sender, RoutedEventArgs e)
        {
            
            MessageDialog messageDialog = new MessageDialog("If you quit now your progress will be lost. Would you like to quit anyway?", "Warning.");
            messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(CommandHandler)));
            messageDialog.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(CommandHandler)));

            await messageDialog.ShowAsync();
            
        }

         private void CommandHandler(IUICommand command)
        {
            var commandLabel = command.Label;
             switch (commandLabel)
             {
                 case "Yes":
                     this.Frame.Navigate(typeof(StartPage));
                     break;

                 case "Cancel":

                     break;
             }


        }

      
    }
}
