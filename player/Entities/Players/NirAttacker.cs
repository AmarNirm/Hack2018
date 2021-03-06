﻿using RoboCup.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using RoboCup.Infrastructure;

namespace RoboCup
{
    public class NirAttacker : Player
    {
        private const int WAIT_FOR_MSG_TIME = 10;

        public NirAttacker(Team team, ICoach coach)
            : base(team, coach)
        {
            m_startPosition = new PointF(m_sideFactor * 10, 0);
        }

        public override void play()
        {

            // first move to start position
            m_robot.Turn(40);
            m_robot.Dash(100);
            
            SeenObject obj;

            while (!m_timeOver)
            {
                if (m_number == FindPlayerClosestToTheBall())
                    m_robot.Dash(100);
            }
        }

        private SenseBodyInfo GetBodyInfo()
        {
            m_robot.SenseBody();
            SenseBodyInfo bodyInfo = null;
            while (bodyInfo == null)
            {
                Thread.Sleep(WAIT_FOR_MSG_TIME);
                bodyInfo = m_memory.getBodyInfo();
            }

            return bodyInfo;
        }
    }
}
