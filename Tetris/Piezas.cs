using System;
using System.Drawing;
using afliwsLib;

namespace Tetris
{
    public class Piezas
	{

		int InitX=5,InitY=1,x,y,Rotacion=0,PiezaActual,Width,Height,NextPiece=new Random().Next(0,6),xNext=16,YNext=1;
		public bool Available = true;
		
		ushort[] Colors = {		  (ushort)ConsoleColor.Blue, 
								  (ushort)ConsoleColor.DarkMagenta, 
								  (ushort)ConsoleColor.DarkGreen, 
								  (ushort)ConsoleColor.DarkYellow, 
								  (ushort)ConsoleColor.Cyan, 
								  (ushort)ConsoleColor.Red, 
								  (ushort)ConsoleColor.Yellow 
								};
		
		Point[,,] Forma = {							//Palo
							{ {new Point(0,0),new Point(0,1),new Point(0,2),new Point(0,3)},
							  {new Point(0,0),new Point(1,0),new Point(2,0),new Point(3,0)},
							  {new Point(0,0),new Point(0,1),new Point(0,2),new Point(0,3)},
							  {new Point(0,0),new Point(1,0),new Point(2,0),new Point(3,0)} },
													//Cuadrado
						    { {new Point(0,0),new Point(1,0),new Point(0,1),new Point(1,1)},
							  {new Point(0,0),new Point(1,0),new Point(0,1),new Point(1,1)},
							  {new Point(0,0),new Point(1,0),new Point(0,1),new Point(1,1)},
							  {new Point(0,0),new Point(1,0),new Point(0,1),new Point(1,1)} },
													//L
						    { {new Point(0,0),new Point(0,1),new Point(0,2),new Point(1,2)},
						      {new Point(0,1),new Point(1,1),new Point(2,1),new Point(2,0)},
						      {new Point(0,0),new Point(1,0),new Point(1,1),new Point(1,2)},
						      {new Point(0,0),new Point(0,1),new Point(1,0),new Point(2,0)} },
													//L Invertida
							{ {new Point(1,0),new Point(1,1),new Point(1,2),new Point(0,2)},
							  {new Point(0,0),new Point(1,0),new Point(2,0),new Point(2,1)},
							  {new Point(0,0),new Point(1,0),new Point(0,1),new Point(0,2)},
							  {new Point(0,0),new Point(0,1),new Point(1,1),new Point(2,1)} },
													//S
						    { {new Point(0,0),new Point(0,1),new Point(1,1),new Point(1,2)},
						      {new Point(1,0),new Point(2,0),new Point(0,1),new Point(1,1)},
						      {new Point(0,0),new Point(0,1),new Point(1,1),new Point(1,2)},
						      {new Point(1,0),new Point(2,0),new Point(0,1),new Point(1,1)} },
													//S Invertida
						    { {new Point(1,0),new Point(1,1),new Point(0,1),new Point(0,2)},
						      {new Point(0,0),new Point(1,0),new Point(1,1),new Point(2,1)},
						      {new Point(1,0),new Point(1,1),new Point(0,1),new Point(0,2)},
						      {new Point(0,0),new Point(1,0),new Point(1,1),new Point(2,1)} },
													//T
						    { {new Point(1,0),new Point(0,1),new Point(1,1),new Point(2,1)},
						      {new Point(1,0),new Point(1,1),new Point(1,2),new Point(0,1)},
						      {new Point(0,0),new Point(1,0),new Point(2,0),new Point(1,1)},
						      {new Point(0,0),new Point(0,1),new Point(0,2),new Point(1,1)} } 
						   };
						    
		public Piezas(){}
		
		public void Initialize()
		{
			NewPiece();
		}
		
		private void Draw(int x,int y,int Pieza)
		{
			for (int i = 0; i < 4; i++)
			{
				ConsoleFunctions.WriteConsole(	x + Forma[Pieza, Rotacion, i].X,
												y + Forma[Pieza, Rotacion, i].Y,
												Colors[Pieza],
												 "\x8");
			}
			UpdatePieceDimentions();
			Available = true;
		}

		private void Erase(int x, int y, int Pieza)
		{
			for (int i = 0; i < 4; i++)
			{
				ConsoleFunctions.WriteConsole(x + Forma[Pieza, Rotacion, i].X,
												y + Forma[Pieza, Rotacion, i].Y,
												 " ");
			}
		}
		
		private void NewPiece()
		{			
			this.Rotacion = 0;
			Erase(xNext, YNext, NextPiece);
			this.PiezaActual = NextPiece;
			this.NextPiece = new Random().Next(0, 7);
			Draw(xNext, YNext, NextPiece);				
			x = InitX;
			y = InitY;
			UpdatePieceDimentions();
			Draw(x,y,PiezaActual);
			Playground.SensibilizeCell(x,y,Height, Width);
					
		}
		
		public void RotatePiece()
		{
			int NewRotation=Rotacion;
			
			if (NewRotation == 3)
				NewRotation = 0;
			else
				NewRotation++;
			
			Point[] SendPiece = new Point[4];
			for (int i = 0; i < 4; i++)
			{
				SendPiece[i] = Forma[PiezaActual, NewRotation, i];
			}

				if (!Playground.CheckCollisionMove(SendPiece, x, y))
				{
					Available = false;
					Erase(x, y, PiezaActual);
					Rotacion=NewRotation;
					Sonidos.PlaySound(Sonidos.Sounds.Rotar); 
					Draw(x, y, PiezaActual);
					Playground.SensibilizeCell(x,y,Height, Width);	
				}
				else
					Sonidos.PlaySound(Sonidos.Sounds.Topa); 					
		}
		
		private void UpdatePieceDimentions()
		{
			Width=0;
			Height=0;
			for (int i = 0; i < 4; i++)
			{
				if (Forma[PiezaActual, Rotacion, i].X > Width)
					Width = Forma[PiezaActual, Rotacion, i].X;
				if (Forma[PiezaActual, Rotacion, i].Y > Height)
					Height = Forma[PiezaActual, Rotacion, i].Y;
			}
			Width++;
			Height++;
		}
		
		public void Fall()
		{
			Available = false;
			Point[] SendPiece = new Point[4];
			for(int i=0;i<4;i++)
			{
				SendPiece[i] = Forma[PiezaActual,Rotacion,i];
			}
			if(Playground.CheckCollisionFall(SendPiece,x,y+1))
			{
				ConsoleFunctions.FlushConsoleInputBuffer();
				if(!Playground.Lose) NewPiece();
			}
			else
			{
				UpdatePieceDimentions();
				Erase(x, y, PiezaActual);
				y++;
				Draw(x, y, PiezaActual);
			}
		}
		
		public void MoveLeft()
		{
			Point[] SendPiece = new Point[4];
			for (int i = 0; i < 4; i++)
			{
				SendPiece[i] = Forma[PiezaActual, Rotacion, i];
			}
			UpdatePieceDimentions();
			if(x != 1 && !Playground.CheckCollisionMove(SendPiece,x-1,y))
			{
				Available = false;
				Erase(x, y, PiezaActual);
				x--;
				Draw(x, y, PiezaActual);
				Playground.SensibilizeCell(x,y,Height, Width);
			}else
				Sonidos.PlaySound(Sonidos.Sounds.Topa); 
		}

		public void MoveRight()
		{
			Point[] SendPiece = new Point[4];
			for (int i = 0; i < 4; i++)
			{
				SendPiece[i] = Forma[PiezaActual, Rotacion, i];
			}
			UpdatePieceDimentions();
			if (x + Width != 11 && !Playground.CheckCollisionMove(SendPiece, x + 1, y))
			{
				Available = false;
				Erase(x, y, PiezaActual);
				x++;
				Draw(x, y, PiezaActual);
				Playground.SensibilizeCell(x,y,Height, Width);
			}
			else
				Sonidos.PlaySound(Sonidos.Sounds.Topa); 
		}
	}
}
