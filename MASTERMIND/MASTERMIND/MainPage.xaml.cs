
// I used Micosoft.com , github and google search  for some documentations. 
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MASTERMIND
{
    public partial class MainPage : ContentPage
    {
        private const int Rowround = 8;
        private int ColorChoice;
        private readonly List<Grid> RGrid = new List<Grid>();
        private saveGame gameSaved = new saveGame();
        private readonly List<Image> Gameboard = new List<Image>();
        private static readonly string[] Colorsuggest = { "chocolate", "Pink", "Blue", "Yellow", "Green", "Red" };
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            BoardForGame();
            GameChoice();
            freshGame();
        }

        private void GameDisplay()
        {
            for(int i = 0; i< RGrid.Count; i++)
            {
                int num = 0;
                foreach(Image mindGame in RGrid[Rowround -1 -i].Children)
                {
                    if(num <gameSaved.mindsFrame[i, 1])
                    {
                        mindGame.Source = "Black.png";
                    }
                    else if (num < gameSaved.mindsFrame[i, 0])
                    {
                        mindGame.Source = "White.png";
                    }
                    else
                    {
                        mindGame.Source = "";
                    }
                    num++;
                }
            }
        }

        /// <summary>
        /// Check button
        /// </summary>
        
        private void CheckBtnResult()
        {
            bool isRight = true;
            for(int i = 0; i < 4; i++)
            {
                if(gameSaved.boardFrame[gameSaved.rowround, i]== -1)
                {
                    isRight = false;
                }
            }

            BtnResult.IsEnabled = isRight;
        }
        
        //Load Gamedaved

        private void GameRetrieve()
        {
            for(int i = 0; i <Gameboard.Count; i++ )
            {
                int y = int.Parse(Gameboard[i].StyleId);
                int x = int.Parse(Gameboard[i].Parent.StyleId);
                int partThree = gameSaved.boardFrame[x, y];

                if(partThree == -1)
                {
                    Gameboard[i].Source = "White.png";
                }
                else
                {
                    Gameboard[i].Source = Colorsuggest[partThree]+ ".png";
                }
            }

            presentRow();
            GameDisplay();
        }

        private void presentRow()
        {
            foreach (Image mindGame in Gameboard )

            {
                if(mindGame.Parent.StyleId == gameSaved.rowround.ToString())
                {
                    mindGame.Opacity = 2f;
                    mindGame.IsEnabled = true;
                }
                else
                {
                    mindGame.Opacity = 0.7f;
                    mindGame.IsEnabled = false;
                }
            }
        }


        // Display loosing Message
        private async void GameLost()
        {
            await DisplayAlert("You lost the game ", $"Never Give Up ", "Re-try a new one");
            freshGame();
            
        }
         private async void  Gamewon()
        {
            await DisplayAlert("You Won the Game ", $"{gameSaved.rowround + 1}", "Start New Game");
        }

        private void freshGame()
        {
            gameSaved = new saveGame();
            Random mds = new Random();
            List<int> duplicates = new List<int>();
            
            for(int i = 0; i <Gameboard.Count; i++)
            {
                Gameboard[i].Source = "White.png";
            }

            for(int i = 0; i < RGrid.Count; i++)
            {
                foreach (Image item in RGrid[i].Children)
                {
                    item.Source = "";
                }
            }

            presentRow();
            for(int i = 0; i < 4; i++)
            {
                int Numr;
                //stop duplication
                do
                {
                    Numr = mds.Next(0, Colorsuggest.Length);
                } while (duplicates.Contains(Numr));
                duplicates.Add(Numr);
                gameSaved.goalFrame[i] = Numr;
            }

        }

         
        private async void Button_Clicked(object sender, EventArgs e)
        {
            //Start New Game
            bool resultAns = await DisplayAlert("Do you really want to re-start", "are you happy to start a new game", "Choose (Yes)", "(No)");
            if (resultAns == false)
            {
                return;
            }
            // Start a new game
            freshGame();

            //Show Update
            CheckBtnResult();
            GameRetrieve();


        }

        //Verify the correct number
        private void Button_Clicked_3(object sender, EventArgs e)
        {
            //Result Button
            int whiteMind = 0;
            int blackMind = 0;

            //Verify color
            for( int i = 0; i< 4; i++)
            {
                bool whiteSame = false;
                for(int m = 0; m < 4; m++)
                {
                    if(gameSaved.goalFrame[i] == gameSaved.boardFrame[gameSaved.rowround, m])
                    {
                      if(whiteSame == false)
                        {
                            whiteMind++;
                            whiteSame = true;
                        }
                      if( i == m)
                        {
                            blackMind++;
                        }
                    }
                }
            }

            gameSaved.mindsFrame[gameSaved.rowround, 0] = whiteMind;
            gameSaved.mindsFrame[gameSaved.rowround, 1] = blackMind;

            // The game was won
            if(blackMind == 4)
            {
                Gamewon();
                return;
            }
            GameDisplay();

            // move player
            gameSaved.rowround++;

            if(gameSaved.rowround >= Rowround)
            {
                GameLost();
                return;
            }

            presentRow();
            CheckBtnResult();

        }


        //Generate
        private void GameChoice()
        {
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += ChoiceMind_Tapped;

            for(int i = 0; i < 6; i++)
            {
                Image mindGame = new Image();
                mindGame.StyleId = i.ToString();
                mindGame.Source = Colorsuggest[i] + ".png";

                if(i == 0)
                {
                    mindGame.BackgroundColor = new Color(2f, 2f, 30f);
                }

                mindGame.GestureRecognizers.Add(tapGestureRecognizer);
                MindsContainer.Children.Add(mindGame);

            }
        }

        //set board
       private void  BoardForGame()
        {
          for(int row = 0; row < Rowround; row++)
            {
                
                StackLayout RoundLayout = new StackLayout();
                RoundLayout.Orientation = StackOrientation.Horizontal;

                GenerateName(row, RoundLayout);
                GenerateMind(RoundLayout);

                GenerateRow(row, RoundLayout);

                RowsContainer.Children.Add(RoundLayout);
            }
        }

        //Generate Names
        private static void  GenerateName(int row, StackLayout RoundLayout)
        {
            Label numName = new Label();
            numName.Margin = new Thickness(7, 0);
            numName.VerticalTextAlignment = TextAlignment.Center;
            numName.Text = (Rowround - row).ToString();
            numName.FontSize = 32;
            numName.FontAttributes = FontAttributes.Italic;
            numName.TextColor = Color.Black;
            RoundLayout.Children.Add(numName);

        }

        //Generate Rows
       private void  GenerateRow(int row, StackLayout RoundLayout)
        {
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += mind_Tapped;

            Frame mindSelectFrame = new Frame();
            mindSelectFrame.Margin = new Thickness(5, 0);
            mindSelectFrame.CornerRadius = 10;
            mindSelectFrame.BackgroundColor = Color.FromHex("D3D3D3");
            mindSelectFrame.Padding = new Thickness(0);

            StackLayout mindsLayout = new StackLayout();
            mindsLayout.Padding = new Thickness(Rowround, 0);
            mindsLayout.Orientation = StackOrientation.Horizontal;

            mindsLayout.StyleId = (Rowround - 1 - row).ToString();

            for( int m = 0; m < 4; m++)
            {
                Image mindGame = new Image();
                mindGame.Source = " White.png";
                if(row != 7)
                {
                    mindGame.Opacity = .7f;
                    mindGame.IsEnabled = false;
                }

                mindGame.GestureRecognizers.Add(tapGestureRecognizer);
                mindGame.StyleId = m.ToString();

                Gameboard.Add(mindGame);
                mindsLayout.Children.Add(mindGame);

            }

            mindSelectFrame.Content = mindsLayout;
            mindSelectFrame.CornerRadius = 20;
            RoundLayout.Children.Add(mindSelectFrame);


        }

        //Generate Games
        private void GenerateMind(StackLayout RoundLayout)
        {
            Frame mindGFrame = new Frame();
            mindGFrame.BackgroundColor = Color.FromHex("#FFFFFF");
            mindGFrame.Padding = new Thickness(2);

            Grid gChoice = new Grid();
            RGrid.Add(gChoice);

            gChoice.RowDefinitions.Add(new RowDefinition());
            gChoice.RowDefinitions.Add(new RowDefinition());
            gChoice.ColumnDefinitions.Add(new ColumnDefinition());
            gChoice.ColumnDefinitions.Add(new ColumnDefinition());

            for(int x = 0; x < 2; x++)
            {
                for(int y = 0; y < 2; y++)
                {
                    Image mindGame = new Image();

                    mindGame.WidthRequest = 26;
                    mindGame.HeightRequest = 26;

                    mindGame.HorizontalOptions = LayoutOptions.Center;
                    mindGame.VerticalOptions = LayoutOptions.Center;

                    mindGame.SetValue(Grid.RowProperty, x);
                    mindGame.SetValue(Grid.ColumnProperty, y);

                    gChoice.Children.Add(mindGame);
                }
            }

            mindGFrame.Content = gChoice;
            RoundLayout.Children.Add(mindGFrame);
        }


        //change color of mind player
        private void mind_Tapped(object sender, EventArgs e)
        {
            Image mindImg = (Image)sender;
            mindImg.Source = Colorsuggest[ColorChoice] + ".png";
            gameSaved.boardFrame[gameSaved.rowround, int.Parse(mindImg.StyleId)] = ColorChoice;
            CheckBtnResult();
        }

        //Choice of player on board

        private void ChoiceMind_Tapped(object sender, EventArgs e)
        {
            Image pic = (Image)sender;

            foreach(Image child in MindsContainer.Children)
            {
                child.BackgroundColor = Color.Transparent;
            }

            pic.BackgroundColor = new Color(2f, 2f, 30f);
            ColorChoice = int.Parse(pic.StyleId);
        }


    }
}
