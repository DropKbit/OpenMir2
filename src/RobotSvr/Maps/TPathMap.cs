﻿using System.Drawing;
using System.Runtime.InteropServices;

namespace RobotSvr
{
    public class TPathMap
    {
        public static Point[] g_MapPath;
        public const int SCALE = 4;
        public static string MAP_BASEPATH = GetMapPath();
        public static int[] TerrainParams = new int[2] { 4, -1 };
        public TMapHeader m_MapHeader;
        public TCellParams[,] m_MapData;
        public TMapInfo[] m_MapBuf;
        public int m_nPathWidth = 0;
        public TPathMapCell[,] m_PathMapArray;

        public TPathMap() : base()
        {
            m_MapBuf = null;
            m_nPathWidth = 0;
        }

        public static string GetMapPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "/Volumes/Data/Mirserver/Mir200/Map/";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                return "/opt/MirServer/Mir200/Map/";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "D:/Mirserver/Mir200/Map/";
            }
            return string.Empty;
        }

        public Point[] FindPathOnMap(int X, int Y)
        {
            Point[] result;
            int Direction;
            result = null;
            if ((X >= m_MapHeader.wWidth) || (Y >= m_MapHeader.wHeight))
            {
                return result;
            }
            if (m_PathMapArray[Y, X].Distance < 0)
            {
                return result;
            }
            result = new Point[m_PathMapArray[Y, X].Distance + 1];
            while (m_PathMapArray[Y, X].Distance > 0)
            {
                result[m_PathMapArray[Y, X].Distance] = new Point(X, Y);
                Direction = m_PathMapArray[Y, X].Direction;
                X = X - DirToDX(Direction);
                Y = Y - DirToDY(Direction);
            }
            result[0] = new Point(X, Y);
            return result;
        }

        public int DirToDX(int Direction)
        {
            int result;
            switch (Direction)
            {
                case 0:
                case 4:
                    result = 0;
                    break;
                // Modify the A .. B: 1 .. 3
                case 1:
                    result = 1;
                    break;
                default:
                    result = -1;
                    break;
            }
            return result;
        }

        public int DirToDY(int Direction)
        {
            int result;
            switch (Direction)
            {
                case 2:
                case 6:
                    result = 0;
                    break;
                // Modify the A .. B: 3 .. 5
                case 3:
                    result = 1;
                    break;
                default:
                    result = -1;
                    break;
            }
            return result;
        }

        protected int GetCost(int X, int Y, int Direction)
        {
            int result = 0;
            Direction = Direction & 7;
            //if ((X < 0) || (X >= m_MapHeader.wWidth) || (Y < 0) || (Y >= m_MapHeader.wHeight))
            //{
            //    result = -1;
            //}
            //else
            //{
            //    result = TerrainParams[m_MapData[X, Y].TerrainType || m_MapData[X, Y].TCellActor];
            //    if ((X < m_MapHeader.wWidth - m_nPathWidth) && (X > m_nPathWidth) && (Y < m_MapHeader.wHeight - m_nPathWidth) && (Y > m_nPathWidth))
            //    {
            //        Cost = TerrainParams[m_MapData[X - m_nPathWidth, Y].TerrainType || m_MapData[X - m_nPathWidth, Y].TCellActor] + TerrainParams[m_MapData[X + m_nPathWidth, Y].TerrainType || m_MapData[X + m_nPathWidth, Y].TCellActor] + TerrainParams[m_MapData[X, Y - m_nPathWidth].TerrainType || m_MapData[X, Y - m_nPathWidth].TCellActor] + TerrainParams[m_MapData[X, Y + m_nPathWidth].TerrainType || m_MapData[X, Y + m_nPathWidth].TCellActor];
            //        if (Cost < 4 * TerrainParams[false])
            //        {
            //            result = -1;
            //        }
            //    }
            //    if (((Direction & 1) == 1) && (result > 0))
            //    {
            //        result = result + (result >> 1);
            //    }
            //}
            return result;
        }

        public void FillPathMap_TestNeighbours()
        {
            //for (d = 0; d <= 7; d++)
            //{
            //    X = OldWave.item.X + DirToDX(d);
            //    Y = OldWave.item.Y + DirToDY(d);
            //    c = GetCost(X, Y, d);
            //    if ((c >= 0) && (result[Y, X].Distance < 0))
            //    {
            //        NewWave.Add(X, Y, c, d);
            //    }
            //}
        }

        public void FillPathMap_ExchangeWaves()
        {
            //TWave w = OldWave;
            //OldWave = NewWave;
            //NewWave = w;
            //NewWave.Clear();
        }

        protected TPathMapCell[,] FillPathMap(int X1, int Y1, int X2, int Y2)
        {
            TPathMapCell[,] result;
            int X;
            int Y;
            TWave OldWave;
            TWave NewWave;
            bool Finished;
            TWaveCell i;
            Finished = (X1 == X2) && (Y1 == Y2);
            if (Finished)
            {
                return null;
            }
            result = new TPathMapCell[m_MapHeader.wHeight, m_MapHeader.wWidth];
            for (Y = 0; Y <= (m_MapHeader.wHeight - 1); Y++)
            {
                for (X = 0; X <= (m_MapHeader.wWidth - 1); X++)
                {
                    result[Y, X].Distance = -1;
                }
            }
            OldWave = new TWave();
            NewWave = new TWave();
            result[Y1, X1].Distance = 0;
            OldWave.Add(X1, Y1, 0, 0);
            FillPathMap_TestNeighbours();
            while (!Finished)
            {
                FillPathMap_ExchangeWaves();
                if (!OldWave.start())
                {
                    break;
                }
                do
                {
                    i = OldWave.item;
                    i.Cost = i.Cost - OldWave.MinCost;
                    if (i.Cost > 0)
                    {
                        NewWave.Add(i.X, i.Y, i.Cost, i.Direction);
                    }
                    else
                    {
                        if (result[i.Y, i.X].Distance >= 0)
                        {
                            continue;
                        }
                        result[i.Y, i.X].Distance = result[i.Y - DirToDY(i.Direction), i.X - DirToDX(i.Direction)].Distance + 1;
                        result[i.Y, i.X].Direction = i.Direction;
                        Finished = (i.X == X2) && (i.Y == Y2);
                        if (Finished)
                        {
                            break;
                        }
                        FillPathMap_TestNeighbours();
                    }
                } while (!!OldWave.Next());
            }
            NewWave = null;
            OldWave = null;
            return result;
        }
    }
}

