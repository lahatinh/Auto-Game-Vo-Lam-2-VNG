using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoClient.Entities;

namespace AutoClient
{
    public static class GameGeneral
    {
        public static float Distance(float posX1, float posY1, float posX2, float posY2)
        {
            return Convert.ToSingle(Math.Sqrt(Math.Pow(posX2 - posX1, 2) + Math.Pow(posY2 - posY1, 2)));
        }
        public static float Distance(PlayerEntity entity1, PlayerEntity entity2)
        {
            return Convert.ToSingle(Math.Sqrt(Math.Pow(entity1.PositionX - entity2.PositionX, 2) + Math.Pow(entity1.PositionY - entity2.PositionY, 2)));
        }
        public static PlayerEntity FindNPCNear(string name, int distance, float posX, float posY, AutoClient client)
        {
            return FindNPCNear(name, distance, Convert.ToInt32(posX), Convert.ToInt32(posY), client);
        }

        public static PlayerEntity FindNPCNear(string name, int distance, int posX, int posY, AutoClient client)
        {
            client.RefreshEntityList();

            foreach (var entity in client.EntityList)
            {
                if (Distance(posX, posY, entity.PositionX, entity.PositionY) > distance)
                    continue;
                if (entity.EntityNameNoMark.ToLower().Trim() == name)
                    return entity;
            }
            return null;
        }

        public static PlayerEntity FindNPCNearest(string name, int distance, float posX, float posY, AutoClient client)
        {
            return FindNPCNear(name, distance, Convert.ToInt32(posX), Convert.ToInt32(posY), client);
        }
        public static PlayerEntity FindNPCNearest(string name, int distance, int posX, int posY, AutoClient client)
        {
            client.RefreshEntityList();

            PlayerEntity result = null;
            int minDistance = int.MaxValue;

            foreach (var entity in client.EntityList)
            {
                int d = Convert.ToInt32(Distance(posX, posY, entity.PositionX, entity.PositionY));
                if (d > distance)
                    continue;
                if (entity.EntityNameNoMark.ToLower().Trim() == name && d < minDistance)
                {
                    minDistance = d;
                    result = entity;
                }
            }
            return result;
        }

    }
}
