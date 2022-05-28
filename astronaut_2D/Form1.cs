using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace astronaut_2D
{
    public partial class Form1 : Form
    {
        int playerSpeed; 

        PictureBox[] moonrocks; 
        int moonroksCount; // количество лунных камней
        Image moonrock = Properties.Resources.moonrock_light;

        PictureBox[] enemies;
        int enemiesSpeed; 
        int enemiesCount; 
        int kills; 
        int health;
        Image EnemiesImg = Properties.Resources.enemy_moving;
        Image DeadEnemy = Properties.Resources.enemy_death;
        Image HurtedEnemy = Properties.Resources.enemy_hurt;
        Image EnemyEating = Properties.Resources.enemy_attack;

        PictureBox[] shots;
        int bulletSpeed;

        Random random; // для случайных событий
     
        public Form1()
        {
            InitializeComponent();
        }
      
        private void Form1_Load(object sender, EventArgs e)
        {
            random = new Random();
            
            playerSpeed = 3;
            Player.Image = Properties.Resources.astronaut_staying2;
            kills = 0;
            health = 10000;

            shots = new PictureBox[1];
            bulletSpeed = 30;

            for(int i = 0; i < shots.Length; i++) // инициализация пуль
            {
                shots[i] = new PictureBox();
                shots[i].BorderStyle = BorderStyle.None;
                shots[i].Size = new Size(30, 5);
                shots[i].BackColor = Color.Yellow;

                this.Controls.Add(shots[i]);
            }

            enemiesCount = 5;
            enemies = new PictureBox[enemiesCount];
            enemiesSpeed = 1;

            for (int i = 0; i < enemies.Length; i++) // инициализация врагов
            {
                enemies[i] = new PictureBox();
                enemies[i].Size = new Size(random.Next(100, 130), random.Next(100, 130));
                enemies[i].SizeMode = PictureBoxSizeMode.Zoom;
                enemies[i].BackColor = Color.Transparent;
                enemies[i].Image = EnemiesImg;
                enemies[i].Location = new Point((i + 2) * random.Next(50, 150) + 1000, random.Next(400, 650));

                this.Controls.Add(enemies[i]);
            }

            moonroksCount = 3;
            moonrocks = new PictureBox[moonroksCount];
                    
            for (int i = 0; i < moonrocks.Length; i++) // инициализация лунных камней
            {
                moonrocks[i] = new PictureBox();
                moonrocks[i].BackColor = Color.Transparent;
                moonrocks[i].Image = moonrock;
                moonrocks[i].Location = new Point((i + 2) * random.Next(50, 150), random.Next(400, 650));

                this.Controls.Add((moonrocks[i]));
            }

            MovingEnemies.Stop(); // начальная табличка с правилам игры
            Rules($"Цель игры состоит в том, чтобы не дать врагам " +
                $"\nдобраться до лунных камней (подсвечиваются " +
                $"\nфиолетовым на карте) и не погибнуть. " +
                $"\nУправление: " +
                $"\nдвижение - стрелки клавиатуры, " +
                $"\nстрельба - пробел." +
                $"\nНажмите пробел чтобы начать.");

            Statistics($"Здоровье: {(health / 100).ToString()} \nУбийства: {kills.ToString()} " +
                $"\nКамни: {moonroksCount.ToString()}"); // статистика
        }

        private void MovingLeft_Tick(object sender, EventArgs e)
        {
            if (Player.Left > 10)
                Player.Left -= playerSpeed;
        }

        private void MovingRight_Tick(object sender, EventArgs e)
        {
            if (Player.Left < 1150 )
                Player.Left += playerSpeed;
        }

        private void MovingTop_Tick(object sender, EventArgs e)
        {
            if (Player.Top > 320)
                Player.Top -= playerSpeed;
        }

        private void MovingDown_Tick(object sender, EventArgs e)
        {
            if (Player.Top < 510)
                Player.Top += playerSpeed;
        }

        private void MovingEnemies_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].Left -= enemiesSpeed;

                IsIntersected(); // проверка на пересечение с игроком

                if (enemies[i].Left < 50) // перемещение врагов с левого края на начальные позиции
                {
                    enemies[i].Size = new Size(random.Next(100, 130), random.Next(100, 130));
                    enemies[i].Location = new Point((i + 1) * random.Next(50, 150) + 1000, random.Next(400, 650));
                    enemies[i].Image = EnemiesImg;
                }
            }
        }

        private void Shooting_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < shots.Length; i++)
            {
                shots[i].Left += bulletSpeed;
            }
        }
       
        private void rocks_lightning_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < moonrocks.Length; i++)
            {
                moonrocks[i].Left += 0;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                MovingLeft.Start();
                Player.Image = Properties.Resources.astronaut_back;
            }
            if (e.KeyCode == Keys.Right)
            {
                MovingRight.Start();
                Player.Image = Properties.Resources.astronaut;
            }
            if (e.KeyCode == Keys.Up)
            {
                MovingTop.Start();
                Player.Image = Properties.Resources.astronaut;
            }
            if (e.KeyCode == Keys.Down)
            {
                MovingDown.Start();
                Player.Image = Properties.Resources.astronaut_back;
            }
            if (e.KeyCode == Keys.Space)
            {
                label4.Visible = false; // закрытие начального меню после нажатия пробела
                Player.Image = Properties.Resources.astronaut_shoot;
                for (int i = 0; i < shots.Length; i++)
                {
                    IsIntersected(); // проверка на пересечение врагов с пулями

                    if (shots[i].Left > 1280)
                    {
                        shots[i].Location = new Point(Player.Location.X + 30 + i * 50, Player.Location.Y + 100);
                        break;
                    }
                }               
            }
            Statistics($"Здоровье: {(health / 100).ToString()} \nУбийства: {kills.ToString()} " +
                $"\nКамни: {moonroksCount.ToString()}"); // статистика 
        }

        private void IsIntersected() // метод проверка пересечений
        {
            for (int i = 0;i < enemies.Length; i++) // враги и пули
            {
                if (shots[0].Bounds.IntersectsWith(enemies[i].Bounds))
                {
                    enemies[i].Image = DeadEnemy;
                    MovingEnemies.Stop();
                    enemies[i].Location = new Point((i + 2) * random.Next(50, 125) + 1000, random.Next(380, 600));
                    enemies[i].Image = EnemiesImg;
                    shots[0].Location = new Point((2000), Player.Location.Y + 50);
                    kills += 1;

                    Statistics($"Здоровье: {(health / 100).ToString()} \nУбийства: {kills.ToString()} " +
                $"\nКамни: {moonroksCount.ToString()}");                 
                }

                if (kills >= 100)
                {
                    for (int j = 0; j < enemies.Length; j++)
                    {
                        MovingEnemies.Stop();
                        enemies[i].Visible = false;
                        enemies[i].Location = new Point(-200, -200);
                        TextMessage("Победа!");
                    }
                }

                for (int j = 0; j < moonrocks.Length; j++) // враги и камни
                {
                    if (moonroksCount == 0)
                    {
                        MovingEnemies.Stop();
                        TextMessage("Игра окончена!");                       
                    }
                    if (moonrocks[j].Bounds.IntersectsWith(enemies[i].Bounds))
                    {
                        enemies[i].Image = EnemyEating;
                        moonroksCount -= 1;
                        enemiesCount += 1;
                        moonrocks[j].Location = new Point(-100, -100);
                    }
                    Statistics($"Здоровье: {(health / 100).ToString()} \nУбийства: {kills.ToString()} " +
                $"\nКамни: {moonroksCount.ToString()}");
                }

                if (Player.Bounds.IntersectsWith(enemies[i].Bounds)) // враги и 
                {                 
                    if (health <= 0)
                    {
                        Player.Visible = false;
                        enemies[i].Image = EnemyEating;
                        TextMessage("Игра окончена!");
                    }
                    health = health - 25;
                    Statistics($"Здоровье: {(health / 100).ToString()} \nУбийства: {kills.ToString()} " +
                 $"\nКамни: {moonroksCount.ToString()}");
                    break;
                }
                           
                MovingEnemies.Start();
            }
        }

        private void TextMessage(string str)
        {
            label1.Text = str;
            label1.Location = new Point(327, 175);
            label1.Visible = true;

            MovingEnemies.Stop();
            rocks_lightning.Stop();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Player.Image = Properties.Resources.astronaut_staying2;
            MovingLeft.Stop();
            MovingRight.Stop();
            MovingTop.Stop();
            MovingDown.Stop();           
        }       

        private void Rules(string str)
        {
            label4.Text = str;
            label4.Location = new Point(266, 131);
            label4.Visible = true;
        }

        private void Statistics(string str)
        {
            label2.Text = str;
            label2.Location = new Point(12, 9);
            label2.Visible = true;
        }
    }
}
