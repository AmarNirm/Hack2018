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

            m_strategy = new Thread(play);
            m_strategy.Start();
        }

        public PointF GetGoalPosition(bool mine)
        {
            char targetGoal;
            if (mine)
            {
                targetGoal = m_side;
            }
            else
            {
                targetGoal = m_side=='l'?'r':'l';
            }
            // get goal position
            var goal = m_coach.GetSeenCoachObject($"goal {targetGoal}");
            PointF goalPos = new PointF();
            if (goal == null)
            {
            }
            else
            {
                goalPos = goal.Pos.Value;
            }
            return goalPos;
        }

        /// <summary>
        /// Returns absolute target position and angle
        /// </summary>
        /// <returns></returns>
        public Position WhereToGo()
        {
            // get goal position
            var goalPos = GetGoalPosition(false);
            // get ball position
            var ball = m_coach.GetSeenCoachObject("ball");
            PointF ballPos = new PointF();
            if (ball == null)
            {
            }
            else
            {
                ballPos=ball.Pos.Value;
            }
            // calculate target position
            float alpha = (float) Math.Atan((ballPos.Y - goalPos.Y) / (ballPos.X - goalPos.X));
            var targetX = (float) (ballPos.X - Math.Cos(alpha));
            var targetY = (float) (ballPos.Y + Math.Sin(alpha));
            var targetPos = new PointF(targetX, targetY);
            float targetAngle = 180 - alpha;
            
            return new Position(targetPos, targetAngle);
        }

        public void GoToBall(Position targetPos)
        {
            // Turn towards the target position
            TurnTowardsPoint(targetPos.Point);
            // Run
            RunTowardsPoint(targetPos.Point);
            // Turn towards the goal
            TurnTowardsPoint(GetGoalPosition(false));
        }

        public void TurnTowardsPoint(PointF point)
        {
            SeenCoachObject myObj = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {m_number}");
            var myAngle = myObj.BodyAngle.Value;
            float alphaPlayerToTarget = (float)Math.Atan((point.Y - myObj.Pos.Value.Y) / (point.X - myObj.Pos.Value.X));
            var relAngle = alphaPlayerToTarget - myAngle;
            m_robot.Turn(relAngle);
        }

        public void RunTowardsPoint(PointF point)
        {
            SeenCoachObject myObj = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {m_number}");
            var ballDistance = Utils.Distance(point, myObj.Pos.Value);
            m_robot.Dash(10 * ballDistance);
        }

        public virtual  void play()
        {
 
        }
    }
}
