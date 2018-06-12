using RoboCup.Entities;
using RoboCup.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading;

namespace RoboCup
{
    public class Team
    {
        //public members
        public List<Player> m_playerList;
        public  String m_teamName;
        public IFormation m_teamFormation;
        public Coach m_coach;
        public int ClosestPlayerToTheBall;

        public Team(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.ToLower().StartsWith("teamname="))
                {
                    var results = arg.Split(new string[] {"="}, StringSplitOptions.RemoveEmptyEntries);
                    m_teamName = results[1];
                }
            }

            if (m_teamName is null)
            {
                m_teamName = SoccerParams.m_teamName;
            }

            m_coach = new Coach();
            m_teamFormation = new Formation_4_4_2();
            m_playerList = m_teamFormation.InitTeam(this, m_coach);
        }


        public virtual int FindPlayerClosestToTheBall()
        {
            SeenCoachObject ballPosByCoach = null;
            SeenCoachObject objPlayer = null;
            double Distance = -1;
            double CurrentDistance = -1;
            string player = "player England ";

            Object thisLock = new Object();

            lock(thisLock)
            {
                //go over all players in the team playerList
                for (int i=1;i<5;i++)
                {
                    ballPosByCoach = m_coach.GetSeenCoachObject("ball");
                    Double BallX = ballPosByCoach.Pos.Value.X;
                    Double BallY = ballPosByCoach.Pos.Value.Y;
                    
                    objPlayer = m_coach.GetSeenCoachObject(player + i.ToString());
                    Double PlayerX = objPlayer.Pos.Value.X;
                    Double PlayerY = objPlayer.Pos.Value.Y;
                    
                    //calculate the distance
                    CurrentDistance = Math.Sqrt(Math.Pow(PlayerX - BallX, 2) + Math.Pow(PlayerY - BallY, 2));

                    //set the current distance with the shortest one
                    if (i == 1)
                    {
                        Distance = CurrentDistance;
                        ClosestPlayerToTheBall = i;
                    }
                    else if (CurrentDistance < Distance)
                    {
                        Distance = CurrentDistance;
                        ClosestPlayerToTheBall = i;
                    }
               
                }
            }

            return ClosestPlayerToTheBall;
        }



    }
}
