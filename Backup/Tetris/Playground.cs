using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using afliwsLib;

namespace Tetris
{
	public static class Playground
	{
		private static bool[,] Matriz = new bool[12,25];
		public static bool Lose;
		public static bool Shadow = true;
		public static bool Flash = true;
		public static int Score,Lines,Level;
		private static Box Limits = new Box(0, 0, 9, 22);
		private static Point[] LastSensibilizedPosition = new Point[4];
		private static ushort[,] LastSensibilizedColors = new ushort[1,4];
		private static int LastSensibilizedNumber = 0;
		
		public static string Achivements()
		{
			return (Score.ToString("00000000")+"|"+Lines.ToString("000")+"|"+Level.ToString("00"));
		}
		
		public static void Initialize()
		{
			Limits.Clear();
			Sonidos.StopBGM();
			Sonidos.PlayBGM();
			
			Limits.Draw(4, "Green");
			Console.ForegroundColor = ConsoleColor.DarkRed;
			for(int i=0;i<12;i++)
			{
				Console.SetCursorPosition(i,0);
				Console.Write("█");
			}
			for(int i=1;i<5;i++)
			{
				Console.SetCursorPosition(0, i);
				Console.Write("█");
				Console.SetCursorPosition(11, i);
				Console.Write("█");
			}				
			Console.ResetColor();
			
			
			Lose = false;
			Score=0;
			Lines=0;
			Level=0;
			for (int i = 1; i < 24; i++)
				for (int j = 1; j < 11; j++)
					Matriz[j, i] = false;
			
			
			for(int i=0;i<12;i++)
			{
				Matriz[i,0] = true;
				Matriz[i,24] = true;
			}
			for(int i=0;i<25;i++)
			{
				Matriz[0,i] = true;
				Matriz[11,i] = true;
			}
			UpdateStatus();
		}
		
		public static void Pause()
		{
			DeSensibilizeCell();
			Sonidos.PlaySound(Sonidos.Sounds.Pausa);
			Sonidos.PauseBGM();
		}
		
		public static void UnPause()
		{
			Sonidos.PlaySound(Sonidos.Sounds.Pausa);
			Sonidos.ContinueBGM();
		}
		
		public static void SensibilizeCell(int x,int y,int Height, int Width)
		{
			if(!Shadow)
				return;
			DeSensibilizeCell();
			ushort[,] SensColor = new ushort[1,1];
			SensColor[0,0] = (ushort)ConsoleColor.Gray;
			ushort[,] TempBuffer = new ushort[1,1];
			LastSensibilizedNumber = Width;
			for(int i=0;i<Width;i++)
			{
				for(int j=Height+y;j<25;j++)
				{
					if(Matriz[x+i,j])
					{
						TempBuffer = ConsoleFunctions.ReadConsoleAttribute(x+i,j,1,1);
						LastSensibilizedColors[0,i] = TempBuffer[0,0];
						ConsoleFunctions.WriteConsoleAttribute(x+i,j,SensColor);
						LastSensibilizedPosition[i].X = x+i;
						LastSensibilizedPosition[i].Y = j;
						break;
					}	
				}
			}
			
		}

		private static void DeSensibilizeCell()
		{
			ushort[,] SensColor = new ushort[1, 1];
			for (int i = 0; i < LastSensibilizedNumber; i++)
			{
				SensColor[0, 0] = LastSensibilizedColors[0, i];
				ConsoleFunctions.WriteConsoleAttribute(LastSensibilizedPosition[i].X,
														LastSensibilizedPosition[i].Y,
														SensColor);
			}
		}
			
		public static bool CheckCollisionFall(Point[] Pieza,int x, int y)
		{
			for(int i=0;i<4;i++)
			{
				if(Matriz[Pieza[i].X + x,Pieza[i].Y + y])
				{	
					for(int j=0;j<4;j++)
						Matriz[Pieza[j].X + x,Pieza[j].Y + y - 1]=true;
					Sonidos.PlaySound(Sonidos.Sounds.Topa);
					Thread.Sleep(200);
					Sonidos.PlaySound(Sonidos.Sounds.Posicionada);
					Score+=6*(Level+1);
					UpdateStatus();					
					LineControl();
					CheckIfLose();
					return true;
				}
			}
			return false;
		}
		
		public static bool CheckCollisionMove(Point[] Pieza,int x, int y)
		{
			for (int i = 0; i < 4; i++)
			{
				if (Matriz[Pieza[i].X + x, Pieza[i].Y + y])
				{
					return true;
				}
			}
			return false;
		}
		
		private static void CheckIfLose()
		{
			for(int i=1;i<11;i++)
				if(Matriz[i,4])
				{
					Sonidos.StopBGM();
					Sonidos.PlaySound(Sonidos.Sounds.Pierde_1);
					for (int j = 23; j > 0; j--)
					{
						Console.SetCursorPosition(1, j);
						Console.Write("KBKBKBKBKB");
						Thread.Sleep(20);
					}
					for (int j = 1; j < 24; j++)
					{
						Console.SetCursorPosition(1, j);
						Console.Write("          ");
						Thread.Sleep(20);
					}
					Sonidos.PlaySound(Sonidos.Sounds.Pierde_2);
					Lose = true;
					ConsoleFunctions.WriteConsoleInput('\0');
					break;
				}

		}
		
		private static void LineControl()
		{
			int[] CompleteLines = new int[0];
			int j;
			for(int i=23;i>1;i--)
			{
				for(j=1;j<11 && Matriz[j,i];j++);
				if(j==11)
				{
					Array.Resize<int>(ref CompleteLines, CompleteLines.Length+1);
					CompleteLines[CompleteLines.Length-1] = i;
				}			
			}
			if(CompleteLines.Length != 0)
				EatLines(CompleteLines);
		}
		
		private static void EatLines(int[] CompleteLines)
		{
			DeSensibilizeCell();
			switch (CompleteLines.Length)
			{
				case 1: Sonidos.PlaySound(Sonidos.Sounds.Single); 
						Score+=40 * (Level + 1);
						Lines+=1;
						break;
				case 2: Sonidos.PlaySound(Sonidos.Sounds.Double); 
						Score += 100 * (Level + 1);
						Lines += 2;
						break;
				case 3: Sonidos.PlaySound(Sonidos.Sounds.Triple); 
						Score += 300 * (Level + 1);
						Lines += 3;
						break;
				case 4: Sonidos.PlaySound(Sonidos.Sounds.Tetris); 
						Score += 1200 * (Level + 1);
						Lines += 4;
						FlashScreen();
						break;
			}
			for(int h=0;h<CompleteLines.Length;h++)
			{
				Console.SetCursorPosition(1,CompleteLines[h]);
				for(int q=1;q<11;q++)
				{
					Console.SetCursorPosition(q,CompleteLines[h]);
					Console.Write(" ");
					Thread.Sleep(15);
				}
				for(int i = CompleteLines[h]; i > 1; i--)
					for(int j=1;j<11;j++)
						Matriz[j,i] = Matriz[j,i-1];

				Console.MoveBufferArea(1, 4, 10, CompleteLines[h]- 4, 1, 5);
				for(int k=h+1;k<CompleteLines.Length;k++)
				{
					CompleteLines[k] +=1;
				}
				Sonidos.PlaySound(Sonidos.Sounds.Linea_Cae); 
			}
			UpdateStatus();
			Thread.Sleep(200);
			
			//Draw(20,0);
		}
		
		private static void UpdateStatus()
		{
			
			Console.SetCursorPosition(13,6);
			Console.Write("Score: "+Score.ToString("00000000"));
			Console.SetCursorPosition(13,8);
			Console.Write("Lines: "+Lines.ToString("000"));
			if(Level != (int)(Level = Lines / 10))
				Program.Writeafliw();
			Console.SetCursorPosition(13, 10);
			Console.Write("Level: " + Level.ToString("00"));
		}
		
		public static void PointsByFall()
		{
			Score+= 1*(Level+1);
			UpdateStatus();
		}
		
		private static void FlashScreen()
		{
			if(!Flash)
				return;
			ushort[,] FlashingColors = new ushort[Console.BufferHeight,Console.BufferWidth];
			for(int i=0;i<FlashingColors.GetLength(0);i++)
				for(int j=0;j<FlashingColors.GetLength(1);j++)
					FlashingColors[i,j] = 123;
			ushort[,] SaveAttributes = ConsoleFunctions.ReadConsoleAttribute(0, 0, Console.BufferWidth, Console.BufferHeight);
			for(int Times=0;Times<4;Times++)
			{
				ConsoleFunctions.WriteConsoleAttribute(0,0,FlashingColors);
				Thread.Sleep(45);
				ConsoleFunctions.WriteConsoleAttribute(0,0,SaveAttributes);
				Thread.Sleep(45);
			
			}
		}
		
	}
}
