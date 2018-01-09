using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Windows.Input;


namespace Squareio
{
    



    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        #region Useful things

        int FoesLeft;
        int minFoesLimit = 20;
        int maxFoesLimit = 40;
        Point startPoint = new Point(20, 20);
        double boostSpeed = 1; //value of acceleration
        double reduceBoostSpeed = 1; //value of negative acceleration
        int boostTimerInterval = 10;
        int reduceSpeedTimerInterval = 45;
        double bounceCoefficient = -0.75;
        int foeSpeed = 2;

        #endregion

        #region Variables
        
        SoundPlayer backgroundSound = new SoundPlayer();
        double charSpeedX = 0, charSpeedY = 0;
        PictureBox picCharacter;
        PictureBox picBackground;
        int foesCount;
        List<Square> enemyList;
        
        int charMass;//veikejo plotas (mase)
        int counter;
        int Xpozition, Ypozition;//movement speed
        Point characterCentralPoint;
        
      
        bool isCharAlive = false;
        
        Timer tmrMovement;
        Timer tmrReduceMovement;
        Timer tmrCheck;
        Timer tmrFoeMovement;

        Random rnd = new Random();

        #endregion


       private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            

            picBackground = new PictureBox()               //setting background picture
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible= false
            };
            picBackground.Image = Properties.Resources.circles;
            picBackground.SendToBack();
            this.Controls.Add(picBackground);

           

            tmrMovement = new Timer()//acceleration timer
            {
                Enabled = false,
                Interval = boostTimerInterval
            };
            tmrMovement.Tick += tmrMovement_Tick;

            tmrReduceMovement = new Timer()//negative acceleration timer
            {
                Enabled = false,
                Interval = reduceSpeedTimerInterval

            };
            tmrReduceMovement.Tick += tmrReduceMovement_Tick;

            tmrCheck = new Timer() //coordinetes checking
            {
                Enabled = false,
                Interval = 10
            };
            tmrCheck.Tick += tmrCheck_Tick;

            tmrFoeMovement = new Timer() //enemy movement timer
            {
                Enabled = false,
                Interval = 25
            };
            tmrFoeMovement.Tick += tmrFoeMovement_Tick;

            //Label lblTime = new Label()
            //{
            //    Location = 
            //};

            backgroundSound = new SoundPlayer(Properties.Resources.music2); //create and launch backgrownd music
            //backgroundSound.PlayLooping();
            //var menustrip = new me
            //newGameIo();
        }

        private void NewGame()
        {

            enemyList = new List<Square>();
            //foreach (var item in enemyList)
            //{
            //    item.Dispose();
            //}
            if (isCharAlive)
            {
                picCharacter.Dispose();
                isCharAlive = false;
            }
            //lblTime.SendToBack();
            lblFoes.Visible = true;
            charSpeedX = 0;
            charSpeedY = 0;
            picBackground.Visible = true;
            tmrMovement.Enabled = true;
            tmrReduceMovement.Enabled = true;
            tmrCheck.Enabled = true;
            tmrFoeMovement.Enabled = true;
            backgroundSound.PlayLooping();
            foesCount = rnd.Next(minFoesLimit, maxFoesLimit); //quantity of enemies
            FoesLeft = foesCount;
            lblFoes.Text = "Foes count: " + FoesLeft.ToString();
            PlaceCharacter();                    //placing a character
            //public
            setFoes(foesCount);            //placing enemies on the form


        }

        void tmrFoeMovement_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < foesCount; i++)
            {
                if (enemyList[i].IsAlive)
                {
                    if (rnd.Next(1, 51) == 20)//switching direction
                    {
                        enemyList[i].Direction = rnd.Next(1, 9);
                    }

                    
                    if (enemyList[i].Location.Y <= picBackground.Location.Y)                //checking boundaries
                    {
                        switch (enemyList[i].Direction)
                        {
                            case 8:
                                enemyList[i].Direction = 6;
                                break;
                            case 1:
                                enemyList[i].Direction = 5;
                                break;
                            case 2:
                                enemyList[i].Direction = 6;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (enemyList[i].Location.Y + enemyList[i].Height >= picBackground.Height + picBackground.Location.Y)//checking boundaries
                    {
                        switch (enemyList[i].Direction)
                        {
                            case 6:
                                enemyList[i].Direction = 8;
                                break;
                            case 5:
                                enemyList[i].Direction = 1;
                                break;
                            case 4:
                                enemyList[i].Direction = 2;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (enemyList[i].Location.X <= picBackground.Location.X)                //checking boundaries
                    {
                        switch (enemyList[i].Direction)
                        {
                            case 6:
                                enemyList[i].Direction = 4;
                                break;
                            case 7:
                                enemyList[i].Direction = 3;
                                break;
                            case 8:
                                enemyList[i].Direction = 2;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (enemyList[i].Location.X + enemyList[i].Width >= picBackground.Width + picBackground.Location.X)//checking boundaries
                    {
                        switch (enemyList[i].Direction)
                        {
                            case 4:
                                enemyList[i].Direction = 6;
                                break;
                            case 3:
                                enemyList[i].Direction = 7;
                                break;
                            case 2:
                                enemyList[i].Direction = 8;
                                break;
                            default:
                                break;
                        }
                    }
                }
                Point tempPnt = Movement(enemyList[i].Direction);
                enemyList[i].Location = new Point(enemyList[i].Location.X + tempPnt.X, enemyList[i].Location.Y + tempPnt.Y);
            }
           
            
        }

        bool CheckWin()
        {
            foreach (var item in enemyList)
            {
                if (item.IsAlive == true)
                {
                    return false;
                }
            }
            return true;

        }

        void tmrCheck_Tick(object sender, EventArgs e)
        {
            if (isCharAlive)
            {
                characterCentralPoint = new Point(picCharacter.Location.X + picCharacter.Width / 2, picCharacter.Location.Y + picCharacter.Height / 2);
                for (int i = 0; i < enemyList.Count; i++)
                {
                    //you are eaten
                    if (enemyList[i].Location.Y < characterCentralPoint.Y && enemyList[i].Location.Y + enemyList[i].Height > characterCentralPoint.Y && enemyList[i].Location.X < characterCentralPoint.X && enemyList[i].Location.X + enemyList[i].Width > characterCentralPoint.X && enemyList[i].IsAlive && charMass <= enemyList[i].GetMass())
                    {
                        
                        enemyList[i].Height = (int)Math.Sqrt((double)(enemyList[i].GetMass()+charMass));
                        enemyList[i].Width = enemyList[i].Height;
                        picCharacter.Dispose();
                        isCharAlive = false;
                        //implement sth



                        EndGame();
                    }
                    //you ate him
                    if (picCharacter.Location.Y < enemyList[i].GetCentralPoint().Y && picCharacter.Location.Y + picCharacter.Height > enemyList[i].GetCentralPoint().Y && picCharacter.Location.X < enemyList[i].GetCentralPoint().X && picCharacter.Location.X + picCharacter.Width > enemyList[i].GetCentralPoint().X && enemyList[i].GetMass() < charMass && enemyList[i].IsAlive)
                    {
                        FoesLeft--;
                        lblFoes.Text = "Foes count: " + FoesLeft.ToString();
                        enemyList[i].IsAlive = false;
                        enemyList[i].Visible = false;
                        picCharacter.Height = (int)Math.Sqrt((double)(enemyList[i].GetMass() + charMass));
                        picCharacter.Width = picCharacter.Height;
                        charMass = picCharacter.Height * picCharacter.Height;
                        if (CheckWin())
                        {
                            EndGame();
                        }
                        
                    }
                }
            }

            txtMass.Text = charMass.ToString();
            counter = 0;
            foreach (var item in enemyList)
            {
                if (item.IsAlive)
                {
                    counter++;
                }
            }
            txtFoesCount.Text = counter.ToString();


        }

        private void ClearArea()
        {
            tmrCheck.Stop();
            tmrFoeMovement.Stop();
            //foreach (var item in enemyList)
            //{
            //    item.Visible = false;
            //}
            picBackground.Visible = false;
            backgroundSound.Stop();
            //enemyList = null;

        }
        private void EndGame()
        {
            foreach (var item in enemyList)
            {
                item.Dispose();
            }
            lblFoes.Visible = false;
            picCharacter.Dispose();
            charSpeedX = 0;
            charSpeedY = 0;
            tmrMovement.Enabled = false;
            tmrReduceMovement.Enabled = false;
            tmrCheck.Enabled = false;
            tmrFoeMovement.Enabled = false;

            picBackground.Visible = false;
            backgroundSound.Stop();

            if (isCharAlive)
            {
                MessageBox.Show("You won");
            }
            else
            {
                MessageBox.Show("You are a loseeeer");
            }
            var rez = MessageBox.Show("Replay?", "Play again?", MessageBoxButtons.OKCancel);
            if (rez == DialogResult.OK)
            {
                NewGame();
            }
            else if (rez == DialogResult.Cancel)
            {
                this.Close();
            }
        }

       

        private Point Movement(int number) //vozvrashiaet smeshenie
        {
            switch (number)
            {
                case 1:
                    Ypozition = -foeSpeed;
                    break;
                case 2:
                    Xpozition = foeSpeed;
                    Ypozition = -foeSpeed;
                    break;

                case 3:
                    Xpozition = foeSpeed;
                    break;

                case 4:
                    Ypozition = foeSpeed;
                    Xpozition = foeSpeed;
                    break;
                case 5:
                    Ypozition = foeSpeed;
                    break;

                case 6:
                    Ypozition = foeSpeed;
                    Xpozition = -foeSpeed;
                    break;
                case 7:
                    Xpozition = -foeSpeed;
                    break;
                case 8:
                    Xpozition = -foeSpeed;
                    Ypozition = -foeSpeed;
                    break;
            }
            return new Point( Xpozition,Ypozition);
            
        }

        void tmrReduceMovement_Tick(object sender, EventArgs e)
        {
            if (charSpeedY > 0)
            {
                charSpeedY -= reduceBoostSpeed;
            }
            if (charSpeedY < 0)
            {
                charSpeedY += reduceBoostSpeed;
            }
            if (charSpeedX > 0)
            {
                charSpeedX -= reduceBoostSpeed;
            }
            if (charSpeedX < 0)
            {
                charSpeedX += reduceBoostSpeed;
            }
        }

        void tmrMovement_Tick(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.W))//input from keyboard
            {
                charSpeedY -= boostSpeed;
            }
            if (Keyboard.IsKeyDown(Key.A))
            {
                charSpeedX -= boostSpeed;
            }
            if (Keyboard.IsKeyDown(Key.S))
            {
                charSpeedY += boostSpeed;
            }
            if (Keyboard.IsKeyDown(Key.D))
            {
                charSpeedX += boostSpeed;
            }

            if (picCharacter.Location.X <= 0) //bouncing out of walls with reversed speed
            {
                if (charSpeedX < 0)
                {
                    charSpeedX = charSpeedX * (bounceCoefficient);
                }
            }
            if (picCharacter.Location.X >= picBackground.Width - picCharacter.Width)
            {
                if (charSpeedX > 0)
                {
                    charSpeedX = charSpeedX * (bounceCoefficient);
                }
            }
            if (picCharacter.Location.Y <= 0)
            {
                if (charSpeedY < 0)
                {
                    charSpeedY = charSpeedY * (bounceCoefficient);
                }
            }
            if (picCharacter.Location.Y >= picBackground.Height - picCharacter.Height)
            {
                if (charSpeedY > 0)
                {
                    charSpeedY = charSpeedY * (bounceCoefficient);
                }
            }

            picCharacter.Location = new Point(picCharacter.Location.X + (int)charSpeedX, picCharacter.Location.Y + (int)charSpeedY); //creating a new point of char location



        }

        private void setFoes(int count)
        {
            for (int i = 0; i < count; i++)//creating a list of objects of my class Enemy
            {
                Square foe = new Square(rnd, this);
                enemyList.Add(foe);
                picBackground.Controls.Add(foe);
            }
        }

        private void lblTime_Click(object sender, EventArgs e)
        {

        }

        private void PlaceCharacter()
        {
            int x = rnd.Next(picBackground.Width - 40);//generating random char position
            int y = rnd.Next(picBackground.Height - 40);
            startPoint = new Point(x, y);
            picCharacter = new PictureBox()
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(50, 50),
                Location = startPoint,
                Image = Properties.Resources.pokeball
            };
            picCharacter.SendToBack();
            charMass = picCharacter.Height * picCharacter.Width;
            picBackground.Controls.Add(picCharacter);
            isCharAlive = true;
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }
       

       
 
      

    }
    class Square : PictureBox
    {

        public Point OurLocation;
        public int SpeedX, SpeedY;
        public bool IsAlive;
        public int Direction;

        public int GetMass()
        {
            return Height * Height;
        }
        public Point GetCentralPoint()
        {
            return new Point(this.Location.X + Height / 2, (this.Location.Y + Height / 2));
        }

        public Square(Random rand, Form1 fr)
        {
            Height = rand.Next(2, 10) * 10;
            Width = Height;
            OurLocation = new Point(rand.Next(0, fr.Width - this.Height), rand.Next(0, fr.Height - this.Height));
            Location = OurLocation;
            Direction = rand.Next(1, 9);
            Size = new Size(Height, Height);
            BackColor = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
            IsAlive = true;
        }



        //private void InitializeComponent()
        //{
        //    ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
        //    this.SuspendLayout();
        //    ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
        //    this.ResumeLayout(false);

        //}

    }
}

