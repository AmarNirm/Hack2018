using System;
using System.Drawing;
using System.Threading;
using RoboCup;
using RoboCup.Entities;
using RoboCup.Infrastructure;

namespace RoboCup
{

    public class Player : IPlayer
    {
        // Protected members
        protected Robot m_robot;			    // robot which is controled by this brain
        protected Memory m_memory;				// place where all information is stored
        protected PointF m_startPosition;
        volatile protected bool m_timeOver;
        protected Thread m_strategy;
        protected int m_sideFactor
        {
            get
            {
                return m_side == 'r' ? 1 : -1;
            }
        }

        // Public members
        public int m_number;
        public char m_side;
        public String m_playMode;
        public Team m_team;
        public ICoach m_coach;

        public Player(Team team, ICoach coach)
        {
            m_coach = coach;
            m_memory = new Memory();
            m_team = team;
            m_robot = new Robot(m_memory);
            m_robot.Init(team.m_teamName, out m_side, out m_number, out m_playMode);

            Console.WriteLine("New Player - Team: " + m_team.m_teamName + " Side:" + m_side +" Num:" + m_number);

            m_strategy = new Thread(new ThreadStart(play));
            m_strategy.Start();
        }

        public virtual  void play()
        {
 
        }

        protected SeenCoachObject GetBall()
        {
            SeenCoachObject ball = null;
            while (!m_timeOver)
            {
                ball = m_coach.GetSeenCoachObject("ball");
                if (ball == null)
                {
                    Console.WriteLine("ball == null");
                    continue;
                }

                Console.WriteLine($"ball: {ball.Pos.Value.X},{ball.Pos.Value.Y}");
            }
            return ball;
        }

        protected SeenCoachObject GetCurrPlayer()
        {
            SeenCoachObject currPlayer = null;
            while (!m_timeOver)
            {
                currPlayer = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {m_number}");
                if (currPlayer == null)
                {
                    Console.WriteLine("currPlayer == null");
                    continue;
                }

                Console.WriteLine((string)currPlayer.Name);
            }

            return currPlayer;
        }
    }
}
