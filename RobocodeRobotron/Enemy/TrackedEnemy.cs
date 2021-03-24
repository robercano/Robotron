﻿using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;
using RC.Math;

namespace RC
{
    // TrackedEnemy
    //
    // Contains information of an enemy seen by the radar
    //
    public class TrackedEnemy
    {
        public struct EnemyDamage
        {
            public long Time;
            public Double Damage;

            public EnemyDamage(long time, Double damage)
            {
                Time = time;
                Damage = damage;
            }
        }

        public struct EnemyPosition
        {
            public long Time;
            public Vector2 Position;

            public EnemyPosition(long time, Vector2 position)
            {
                Time = time;
                Position = position;
            }
        }

        public String Name { get; private set; }
        public double HeadingRadians { get; private set; } = 0.0;
        public double BearingRadians { get; private set; } = 0.0;
        public double Energy { get; private set; } = 0.0;
        public double Velocity { get; private set; } = 0.0;
        public Vector2 Position
        {
            get
            {
                return _Position;
            }

            private set
            {
                _Position = value;
            }
        }
        private Vector2 _Position;

        public long Time { get; private set; } = 0;

        public Vector2 AntigravityVector { get; private set; }
        public Double Distance { get; private set; }
        public Double DangerScore = 0.0;

        public Double TrackingScore = 0.0;

        private List<EnemyDamage> DamageToPlayer;

        private List<EnemyPosition> PositionHistory;

        public TrackedEnemy(Robotron player, Enemy enemy)
        {
            Name = enemy.Name;
            DamageToPlayer = new List<EnemyDamage>();
            PositionHistory = new List<EnemyPosition>();

            UpdateFromRadar(player, enemy);
        }

        public void UpdateForCurrentTurn(Robotron player)
        {
            UpdateFromPlayer(player);
            CalculateDangerScore(player.Time);
            UpdatePositionHistory(player.Time);
        }

        private void UpdatePositionHistory(long time)
        {
            PositionHistory.RemoveAll(item => (time - item.Time) > Strategy.Config.PositionHistoryMaxTurnsToKeep);
        }

        public void UpdateFromRadar(Robotron player, Enemy enemy)
        {
            HeadingRadians = enemy.HeadingRadians;
            BearingRadians = enemy.BearingRadians;
            Energy = enemy.Energy;
            Velocity = enemy.Velocity;

            Position = enemy.Position;
            PositionHistory.Add(new EnemyPosition(enemy.Time, enemy.Position));

            Log("Enemy " + Name + " is at position " + enemy.Position);
            Log("  My position is " + new Vector2(player.X, player.Y));
            Time = enemy.Time;

            UpdateFromPlayer(player);
        }

        public void UpdateFromPlayer(Robotron player)
        {
            // Antigravity
            double gForce = 500000.0;

            Vector2 playerPos = new Vector2(player.X, player.Y);
            Vector2 playerEnemyVector = Position - playerPos;

            double distance = playerEnemyVector.Module();
            double distance2 = distance * distance;
            double forceStrength = gForce / distance2;

            AntigravityVector = -playerEnemyVector.GetNormalized() * forceStrength;

            Log("Enemy " + Name + " antigravity is" + AntigravityVector);

            // Distance
            Distance = Util.CalculateDistance(player.X, player.Y, Position.X, Position.Y);
        }

        public void UpdateHitPlayer(HitByBulletEvent evnt)
        {
            Double damage = 4 * evnt.Bullet.Power;
            if (evnt.Bullet.Power > 1.0)
            {
                damage += 2.0 * (evnt.Bullet.Power - 1.0);
            }

            DamageToPlayer.Add(new EnemyDamage(evnt.Time, damage));
        }

        private void CalculateDangerScore(long time)
        {
            // Remove old hits
            DamageToPlayer.RemoveAll(item => (time - item.Time) > Strategy.Config.MaxBulletHitTimeDiff);

            // Calculate danger score
            DangerScore = 0.0;

            if (DamageToPlayer.Count > 0)
            {
                foreach (TrackedEnemy.EnemyDamage enemyDamage in DamageToPlayer)
                {
                    DangerScore += enemyDamage.Damage;
                }
                DangerScore /= DamageToPlayer.Count;
            }
        }
    }
}