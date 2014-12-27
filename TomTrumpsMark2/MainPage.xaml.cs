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
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;
using Windows.Storage;
using Windows.ApplicationModel;
using Windows.Data.Json;
using TomTrumpsMark2.Common;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TomTrumpsMark2
{


     public sealed partial class MainPage : Page
     {
       
        private MediaElement _buzzer;
        
        bool cardOpen = false, leftCardOpen=false, winCheck=false, drawCheck=false, loseCheck=false;
        int drawTwice=0;
        int moves=0;
		
        List<Cards> myListofCardsComp = new List<Cards>();
        List<Cards> myListofCards = new List<Cards>(); 
        List<Cards> deckCards = new List<Cards>();
        List<Cards> tempList = new List<Cards>();
        List<Cards> tempList2 = new List<Cards>();
        List<Cards> drawList = new List<Cards>();
        List<Cards> drawList2 = new List<Cards>();
        


        static Random rnd = new Random();
       
		
		
        public MainPage()
        {      
            this.InitializeComponent();

            btnPower.Click += myEventHandler;
            btnSpeed.Click += myEventHandler;
            btnHeart.Click += myEventHandler;
            btnDefence.Click += myEventHandler;
            btnChin.Click += myEventHandler;
            btnTechnique.Click += myEventHandler;
            btnFootwork.Click += myEventHandler;
            btnStamina.Click += myEventHandler;

            DealCards();        
        }

        public async void DealCards()
        {   //This method uses the 'Shuffle' method which randomizes the 40 cards in the 'json' list and puts 20 into the user's deck and 20 into the opposition's.
            
            await LoadLocalData();
            int[] array = { 0, 1, 2, 3, 4, 5, 6, 7};//, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39
            Shuffle(array);

            for (int i = 0; i < 4; i++)
            {
                myListofCards.Add(deckCards[array[i]]);
            }
            //for (int i = 20; i < 40; i++)
            for (int i = 4; i < 8; i++)
            {

                myListofCardsComp.Add(deckCards[array[i]]);
            }
            imgMyCard.Source = new BitmapImage(new Uri(myListofCards[0].cardPicture, UriKind.Absolute));
            ImageCard.Source = new BitmapImage(new Uri(myListofCardsComp[0].cardPicture, UriKind.Absolute));

            var assets = await Package.Current.InstalledLocation.GetFolderAsync("assets");
            var buzzer = await assets.GetFileAsync("shuffle.wav");
            var stream = await buzzer.OpenAsync(FileAccessMode.Read);

            _buzzer = new MediaElement();
            _buzzer.SetSource(stream, buzzer.ContentType);
        }


        public async Task LoadLocalData()
        {
            var file = await Package.Current.InstalledLocation.GetFileAsync("Data\\boxers.txt");
            var result = await FileIO.ReadTextAsync(file);
            var boxerList = JsonArray.Parse(result);
            CreateDogsList(boxerList);
        }

        public void CreateDogsList(JsonArray boxerList)
        {
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
            
            if (leftCardOpen == true)
            {
               
                if (myListofCardsComp.Count != 0 && myListofCards.Count != 0)
                {
                    rectBlock.Visibility = Visibility.Visible;
                    var assets = await Package.Current.InstalledLocation.GetFolderAsync("assets");
                    var buzzer = await assets.GetFileAsync("cardflip.wav");
                    var stream = await buzzer.OpenAsync(FileAccessMode.Read);

                    _buzzer = new MediaElement();
                    _buzzer.SetSource(stream, buzzer.ContentType);

                    moves++;
                    txtMoves.Text = Convert.ToString(moves);

                    txtNextComp.Visibility = Visibility.Visible;
                    //imgBlockCard.Visibility = Visibility.Collapsed;

                    if (cardOpen == false)
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
                            //if (countLeft < 1)
                            //{
                            //    win.Text = ("You have no cards left, You Lose");
                            //}
                            //else if (countRight < 1)
                            //{
                            //    win.Text = ("Congratulations, you won");
                            //}
                            power();
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
                    
                }

                ListLeft.Text = "";
                ListRight.Text = "";

                foreach (var myCard in myListofCards)
                {
                    ListLeft.Text += myCard.cardName + "\n";
                }

                foreach (var myCard2 in myListofCardsComp)
                {
                    ListRight.Text += myCard2.cardName + "\n ";
                }
            }
        }

        public static void Shuffle<T>(T[] array)
        {
            var random = _random;
            for (int i = array.Length; i > 1; i--)
            {
                int j = random.Next(i);
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }//Shuffle the Deck

        private void clickHere(object sender, TappedRoutedEventArgs e)
        {          
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
            
            rectBlock.Visibility = Visibility.Collapsed;
            txtMoveslbl.Visibility = Visibility.Visible;
            txtMoves.Visibility = Visibility.Visible;
			
            if ((winCheck==true)&&(cardOpen == false))
            {
                imgMyCard.Source = new BitmapImage(new Uri(myListofCards[0].cardPicture, UriKind.Absolute));
                RightCardGoesLeft.Stop();
                RightCardGoesLeft.SetValue(Storyboard.TargetNameProperty, "ImageCoverAnimate");
                RightCardGoesLeft.Begin();
           
            }
            else if (loseCheck == true)
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
                this.Frame.Navigate(typeof(Final));
            }
            else if ((myListofCards.Count < 1))
            {
                this.Frame.Navigate(typeof(Lose));
            }
        }

        static Random _random = new Random();

        
        private async void CardTurns_Tapped(object sender, TappedRoutedEventArgs e)
        {
            

            if ((leftCardOpen == false)&& (loseCheck==false))
            {
                FlipOpenLeft.Begin();
                leftCardOpen = true;
            }
            else if ((leftCardOpen == false)&& (loseCheck==true))
            {
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

            imgBlockCard.Visibility = Visibility.Collapsed;

            
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
                }
                win.Visibility = Visibility.Visible;
                //imgBlockCard.Visibility = Visibility.Collapsed;
                FlipOpen.Begin();
                cardOpen = true;
                win.Text = "You won the round! \n" + myListofCards[0].cardName.ToString() + " beats " + myListofCardsComp[0].cardName.ToString() + ", \nHit continue to proceed.";
            
                if (myListofCardsComp.Count <1)
                {
                    this.Frame.Navigate(typeof(Final));
                }

            
                tempList.Add(myListofCards[0]);
                myListofCards.RemoveAt(0);
                myListofCards.Add(tempList[0]);
                tempList.RemoveAt(0);
                myListofCards.Add(myListofCardsComp[0]);
                myListofCardsComp.RemoveAt(0);
            
                if (myListofCardsComp.Count > 1)
                {
                    txtNextComp.Text = ("The next challenger is " + myListofCardsComp[0].cardName.ToString());
                }
                else if (myListofCardsComp.Count == 1)
                {
                    txtNextComp.Text = myListofCardsComp[0].cardName.ToString()+" is the opponent's last card!";
                }

                btnContinue.Visibility = Visibility.Visible;
                imgBlockCard.Visibility = Visibility.Collapsed;

                leftCounter.Text = Convert.ToString(myListofCards.Count);
                rightCounter.Text = Convert.ToString(myListofCardsComp.Count);
            
           
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

                tempList.Add(myListofCardsComp[0]);
                ImageCard.Source = new BitmapImage(new Uri(myListofCardsComp[0].cardPicture, UriKind.Absolute));

                myListofCardsComp.RemoveAt(0);
                myListofCardsComp.Add(tempList[0]);
                tempList.RemoveAt(0);
                myListofCardsComp.Add(myListofCards[0]);
                myListofCards.RemoveAt(0);
                if (myListofCardsComp.Count > 1)
                {
                    txtNextComp.Text = ("The next challenger is " + myListofCardsComp[0].cardName.ToString());
                }
                else if (myListofCardsComp.Count < 2)
                {
                    txtNextComp.Text = "Opponent is on his last card!";
                }
                
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
                if (myListofCards.Count > 0 && myListofCards.Count < 9)//41
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
            if (myListofCards.Count > 0 && myListofCards.Count < 9)
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
            if (myListofCards.Count > 0 && myListofCards.Count < 9)
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
            if (myListofCards.Count > 0 && myListofCards.Count < 9)
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
            if (myListofCards.Count > 0 && myListofCards.Count < 9)
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
            if (myListofCards.Count > 0 && myListofCards.Count < 9)
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
            if (myListofCards.Count > 0 && myListofCards.Count < 9)
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
            if (myListofCards.Count > 0 && myListofCards.Count < 9)
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


        private void btnBack(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StartPage));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
