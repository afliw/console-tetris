using System;
using afliwsLib;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace Tetris
{
    class WindowSize
    {
        public enum Regular
        {
            Width = 31,
            Height = 25
        }

        public enum About
        {
            Width = 75,
            Height = 25
        }

        public enum Options
        {
            Width = 60,
            Height = 27
        }

        public enum Scores
        {
            Width = 71,
            Height = 25
        }

        public enum SaveScore
        {
            Width = 61,
            Height = 25
        }
    }

    class Program
	{
		public static void Game(object data)
		{ 
			Piezas Pieza = (Piezas)data;
			int SleepTime;
			do
			{
				if((SleepTime = 750-(Playground.Level*30)) < 1) SleepTime = 10;
				if(Pieza.Available && !Playground.Lose) Pieza.Fall();	
				Thread.Sleep(SleepTime);
			}while(true);
		}
		
		public static void LoadScores(ref string[] HighScoresArray)
		{
			try
			{
				StreamReader HighScoresStream = new StreamReader(@"Data\HighScores.afl");		
				while(!HighScoresStream.EndOfStream)
				{
					Array.Resize(ref HighScoresArray,HighScoresArray.Length+1);
					HighScoresArray[HighScoresArray.Length-1] = HighScoresStream.ReadLine();
					
				}
				HighScoresStream.Close();
			}
			catch(FileNotFoundException)
			{
				Console.WriteLine("Score's file not found, a new one will be created.");
				Console.Write("Press any key to continue...");
				Console.ReadKey(true);
				StreamWriter NewScores = new StreamWriter(File.Create(@"Data\HighScores.afl"));
				for(int i=0;i<10;i++)
					NewScores.WriteLine("Tetris|0|0|0");
				NewScores.Close();
				LoadScores(ref HighScoresArray);
			}
			catch(DirectoryNotFoundException)
			{
				Directory.CreateDirectory(@"Data");
				LoadScores(ref HighScoresArray);			
			}
		}
		
		public static void SaveScores(ref string[] HighScoresArray)
		{
			
			StreamWriter HighScoresStream = new StreamWriter(@"Data\HighScores.afl");
			HighScoresStream.Flush();
			foreach(string Score in HighScoresArray)
			{
				HighScoresStream.WriteLine(Score);				
			}
			HighScoresStream.Close();
		}
		
		public static void SetWindowWidth(int Size)
		{
			if(Size>Console.BufferWidth)
			{
				Console.BufferWidth = Size;
				while (Console.WindowWidth < Console.BufferWidth)
				{	
					Console.WindowWidth++;
					Thread.Sleep(1);
				}
			}
			else
			{
				while (Console.WindowWidth > Size)
				{
					Console.WindowWidth--;
					Console.BufferWidth--;
					Thread.Sleep(1);
				}
			}
		}
		
		public static int ConvertScore(string TableScore)
		{
			string[] Splited = TableScore.Split('|');
			return int.Parse(Splited[1]);
			
		}
		
		public static void ShowScoresTable(string[] HighScoresArray,int pos)
		{
			afliw.Clear(0, Console.WindowHeight, 31, Console.WindowWidth);
			Table ScoreTable = new Table(32,1,true,true);
			ScoreTable.AddColumn("Player",16);
			ScoreTable.AddColumn("Score",8);
			ScoreTable.AddColumn("Lines",5);
			ScoreTable.AddColumn("Level",5);
			foreach(string Score in HighScoresArray)
			{
				string [] Splited = Score.Split('|');
				ScoreTable.AddRow(Splited[0],Splited[1],Splited[2],Splited[3]);
			}
			if(pos != -1)
				ScoreTable.ChangeRowColor(pos,"Yellow");
			SetWindowWidth((int)WindowSize.Scores.Width);
			ScoreTable.Print("White","Magenta");
			
		}
		
		public static void Writeafliw()
		{
			int ActualLevel = Playground.Level + 1;
			ConsoleColor SaveFG = Console.ForegroundColor;
			if(ActualLevel > 15)
			{	
				ActualLevel = ActualLevel%15;
			}
			Console.ForegroundColor = (ConsoleColor)ActualLevel;
			Console.SetCursorPosition(12, 20);
			Console.Write("▀▀▀█ █▀▀▀ █ ▀ █   █");
			Console.SetCursorPosition(12, 21);
			Console.Write("   █ █    █ █ █ ▄ █");
			Console.SetCursorPosition(12, 22);
			Console.Write("█▀▀█ █▀▀  █ █ █ █ █");
			Console.SetCursorPosition(12, 23);
			Console.Write("█▄▄█ █    █ █ █████");
			Console.ForegroundColor = SaveFG;
		}
		
		public static void WriteInstructions()
		{
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.SetCursorPosition(13,12);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("◄");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(" & ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("►");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(": Move");
			Console.SetCursorPosition(13, 13);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("▼");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(": Fall Quickly");
			Console.SetCursorPosition(13, 14);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("▲");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(": Rotate");
			Console.SetCursorPosition(13, 15);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("Enter");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(": Pause/Optns");
			Console.SetCursorPosition(13, 16);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("Spacebar");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(": Scores");
			Console.SetCursorPosition(13, 17);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("F1");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(": About");
        }
		
		public static void About()
		{
			afliw.Clear(0, Console.WindowHeight, 31, Console.WindowWidth);
			SetWindowWidth((int)WindowSize.About.Width);
			Box Content = new Box(31,5,41,10);
			Content.Draw(0,"Blue");
			Console.SetCursorPosition(45,6);
			Console.Write("afliw's Tetris");
			Console.SetCursorPosition(32,8);
			Console.Write("Written by afliw.");
			Console.SetCursorPosition(32, 10);
			Console.Write("Tetris' orginal concept by Alexy Pajitnov.");
			Console.SetCursorPosition(32, 12);
			Console.Write("Music and Sounds extracted from");
			Console.SetCursorPosition(32,13);
			Console.Write("Super Nintendo game \"Super Tetris 3\".");
			Console.SetCursorPosition(32,15);
			Console.Write("Sound System: The irrKlang SDK v1.3.0");
			Console.SetCursorPosition(32, 16);
			Console.Write("by Nikolaus Gebhardt.");
			
		}
		
		public static void PauseGame(	Thread GamePlay,
										ref bool Quit,
										ref Piezas ObPieza)
		{
			ConsoleFunctions.CHAR_INFO[] Buffer = new ConsoleFunctions.CHAR_INFO[11 * 24];
			ConsoleFunctions.CHAR_INFO[] BufferNext = new ConsoleFunctions.CHAR_INFO[4 * 4];
			GamePlay.Suspend();
			Playground.Pause();
			Buffer = ConsoleFunctions.ReadConsoleOutput(1, 1, 11, 24);
			BufferNext = ConsoleFunctions.ReadConsoleOutput(15, 1, 4, 4);
			Console.Title = Console.Title + " (Paused)";
			afliw.Clear(1, 24, 1, 11);
			afliw.Clear(1, 5, 15, 19);
			ConsoleFunctions.WriteConsole(3, 1, (ushort)ConsoleColor.DarkCyan, "Paused");
			for (int i = 0; i < 11; i++)
			{
				Console.MoveBufferArea(3, 1 + i, 6, 1, 3, 2 + i);
				Thread.Sleep(300 / (5 * (i + 1)));
			}
			for (int i = 0; i < 3; i++)
			{
				Console.MoveBufferArea(3, 12 - i, 6, 1, 3, 11 - i);
				Thread.Sleep(30 * (i + 1));
			}
			Thread.Sleep(20);
			for (int i = 0; i < 3; i++)
			{
				Console.MoveBufferArea(3, 9 + i, 6, 1, 3, 10 + i);
				Thread.Sleep(30 * (i + 1));
			}
	
			int Option = 2;
			Menu PauseMenu = new Menu(2,14,-1,"Gray");
			PauseMenu.AddItem(true,"Continue","Restart","Config","Quit");
			ConsoleFunctions.FlushConsoleInputBuffer();
			do
			{
				Option = PauseMenu.DrawMenu(0);
				switch(Option)
				{
					case 0: UnPauseGame(GamePlay,Buffer,BufferNext); break;
					case 1: UnPauseGame(GamePlay, Buffer, BufferNext); RestartGame(ref ObPieza); break;
					case 2: Options(); break;
					case 3: Quit = true; break;
					
				}
			
			}while(Option == 2);			
		}

		public static void UnPauseGame(Thread GamePlay, ConsoleFunctions.CHAR_INFO[] Buffer, ConsoleFunctions.CHAR_INFO[] BufferNext)
		{
			afliw.Clear(1, 24, 1, 11);
			Console.Title = Console.Title.Remove(14);
			ConsoleFunctions.WriteBufferToConsole(1, 1, 11, 24, Buffer);
			ConsoleFunctions.WriteBufferToConsole(15, 1, 4, 4, BufferNext);
			Playground.UnPause();
			GamePlay.Resume();
			ConsoleFunctions.FlushConsoleInputBuffer();
		}
		
		public static void RestartGame(ref Piezas ObPieza)
		{
			SetWindowWidth((int)WindowSize.Regular.Width);
			Playground.Initialize();
			ObPieza.Initialize();
		}
		
		public static void OptSwitch(int Selected,int Switch)
		{
			string[] BGMNames = {"Technotris","Kalinka","Troika","OFF"};
			if(Selected != 4)
			{	
				if(Switch!=0)
				{
					switch(Selected)
					{
						case 0: Playground.Shadow = Playground.Shadow ? false : true; break;
						case 1: Sonidos.Sounds_ON = Sonidos.Sounds_ON ? false : true; break;
						case 2: if(Switch==1) Sonidos.SoundsVolume(true); else Sonidos.SoundsVolume(false);break;
						case 3: Playground.Flash = Playground.Flash ? false : true; break;
						case 5: if(Switch == 1) Sonidos.BGMVolume(true); else Sonidos.BGMVolume(false); break;
					}
				}
				Console.SetCursorPosition(49,2+(Selected*3));
			}
			else
			{					
				if(Switch!=0)
				{
					Sonidos.CurrentBGM+= Switch;
					if (Sonidos.CurrentBGM < 0)
						Sonidos.CurrentBGM = 3;
					else if (Sonidos.CurrentBGM > 3)
						Sonidos.CurrentBGM = 0;
				}
				afliw.Clear(2+(Selected*3),2+(Selected*3)+1, 40, Console.BufferWidth-5);
				Console.SetCursorPosition(52 - BGMNames[Sonidos.CurrentBGM].Length, 2 + (Selected * 3));
			}
			Console.Write("◄ ");
			switch(Selected)
			{
				case 0: if(Playground.Shadow) Console.Write("ON "); else Console.Write("OFF"); break;
				case 1: if(Sonidos.Sounds_ON) Console.Write("ON "); else Console.Write("OFF"); break;
				case 2: Console.Write(Sonidos.ReturnVolume(1).ToString("000")); break;
				case 3: if(Playground.Flash) Console.Write("ON "); else Console.Write("OFF"); break;
				case 4: Console.Write(BGMNames[Sonidos.CurrentBGM]); break;
				case 5: Console.Write(Sonidos.ReturnVolume(0).ToString("000")); break;
			}
			Console.Write(" ►");
		}
		
		public static void Options()
		{
			SetWindowWidth((int)WindowSize.Options.Width);
			afliw.Clear(0,Console.BufferHeight - 1,31,Console.BufferWidth - 1);
			Box OptionsBox = new Box(31,0,Console.BufferWidth - 34,Console.BufferHeight - 3);
			OptionsBox.Draw(0,"Magenta");
            string[] Opciones = {"Shadow","Sounds","Sound Volume","Tetris' Flash","Music","Music Volume","Back"};
			
			for(int i = 0;i < 7;i++)
			{
				Console.SetCursorPosition(35,2+(i*3));
				Console.Write(Opciones[i]);
			}
			
			for(int i=0;i<6;i++)
			{
				OptSwitch(i,0);
			}
			
			ushort[,] Normal = new ushort[1, Opciones[3].Length];
			for(int i=0;i<Normal.GetLength(1);i++)
				Normal[0,i] = 7;
			int Selected = 0;
			bool Back = false;
			int SaveBGM = Sonidos.CurrentBGM;
			do
			{
				ushort[,] Highlight = new ushort[1,Opciones[Selected].Length];
				
				for(int i=0;i<Highlight.GetLength(1);i++)
					Highlight[0,i] = 128;
				for(int i=0;i<Opciones.Length;i++)
				{
					ConsoleFunctions.WriteConsoleAttribute(35, 2 + (i * 3), Normal);
					if(i==Selected)
						ConsoleFunctions.WriteConsoleAttribute(35, 2 + (i * 3), Highlight);
				}
				
				switch(Console.ReadKey(true).Key)
				{
					case ConsoleKey.UpArrow: if(Selected == 0) Selected = 6; else Selected--; break;
					case ConsoleKey.DownArrow: if(Selected == 6) Selected = 0; else Selected++;break;
					case ConsoleKey.RightArrow: OptSwitch(Selected,1); break;
					case ConsoleKey.LeftArrow: OptSwitch(Selected,-1); break;
					case ConsoleKey.Enter: if(Selected == 6) Back = true; break;
				}
			}while(!Back);
			if(SaveBGM != Sonidos.CurrentBGM)
			{
				Sonidos.StopBGM();
				Sonidos.ChangeBGM();
			}
			SetWindowWidth((int)WindowSize.Regular.Width);
			
		}

        const int STDOUT_HANDLE = -11;
        [DllImport("kernel32.dll")]
        static extern int SetConsoleFont(IntPtr hOut, uint dwFontSize);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int dwType);
		
		static void Main(string[] args)
		{
            SetConsoleFont(GetStdHandle(STDOUT_HANDLE), 8);
			bool Quit=false,ShowingScores=false,ShowingAbout=false;
			Console.Title = "afliw's Tetris";
			Console.CursorVisible = false;				
			string[] HighScoresArray = new string[0];
            Sonidos.StopBGM();
			LoadScores(ref HighScoresArray);
			Box NextPieceBox = new Box(14,0,3,3);
			bool Paused = false;
			Piezas ObPieza = new Piezas();
			Thread GamePlay = new Thread(Game);
			Console.Clear();
			Writeafliw();
			WriteInstructions();
			Console.WindowHeight = (int)WindowSize.Regular.Height;
			Console.WindowWidth = (int)WindowSize.Regular.Width;
			Console.BufferHeight = (int)WindowSize.Regular.Height;
			Console.BufferWidth = (int)WindowSize.Regular.Width;
			ConsoleFunctions.SetConsoleFont(8);
			Playground.Initialize();
			ObPieza.Initialize();
			NextPieceBox.Draw(0, "Green");
			
			GamePlay.Start(ObPieza);
			string UserAction = " ";
			do
			{
				if(!Playground.Lose)					
				{
					switch(UserAction = Console.ReadKey(true).Key.ToString())
					{
						case "UpArrow": if (ObPieza.Available && !Paused) ObPieza.RotatePiece(); break;
						case "DownArrow": if (ObPieza.Available && !Paused)ObPieza.Fall();Playground.PointsByFall(); break;
						case "LeftArrow": if (ObPieza.Available && !Paused) ObPieza.MoveLeft();break;
						case "RightArrow": if (ObPieza.Available && !Paused) ObPieza.MoveRight(); break;
						case "Enter": PauseGame(GamePlay,ref Quit,ref ObPieza); break;
						case "Spacebar":									
									ShowingScores = ShowingScores ? false : true;
										if (ShowingScores)										
										{	
											ShowScoresTable(HighScoresArray, -1);										
											if(ShowingAbout) ShowingAbout=false;
										}
										else
											SetWindowWidth((int)WindowSize.Regular.Width);
									ConsoleFunctions.FlushConsoleInputBuffer();
									break;
						case "F1":
									ShowingAbout = ShowingAbout ? false : true;
									if(ShowingAbout)
									{		
										About();
										if (ShowingScores) ShowingScores = false;
									}
									else
										SetWindowWidth((int)WindowSize.Regular.Width);
									ConsoleFunctions.FlushConsoleInputBuffer();
									break;
																		
					}
				}
				else
				{
						int i;					
						for (i = 0; i < HighScoresArray.Length && ConvertScore(HighScoresArray[i]) >= Playground.Score; i++) ;
						if (i < HighScoresArray.Length - 1 || (i == HighScoresArray.Length - 1 && ConvertScore(HighScoresArray[HighScoresArray.Length-1]) < Playground.Score))
						{
							afliw.Clear(0, Console.WindowHeight, 31, Console.WindowWidth);
							SetWindowWidth((int)WindowSize.SaveScore.Width);
							string PlayersName;
							Console.SetCursorPosition(37,5);
							Console.ForegroundColor = ConsoleColor.Cyan;
							Console.Write("CONGRATULATIONS!");
							Console.ResetColor();
							Console.SetCursorPosition(35,6);
							Console.Write("You've made it in to");
							Console.SetCursorPosition(34, 7);
							Console.Write("the top 10 best scores.");
							InputField getname = new InputField(15,true,true,true,false,false);
							PlayersName = getname.GetString(33,9,true,"Name ",true);
							
							
							for(int j=HighScoresArray.Length-1;j>i;j--)
							{
								HighScoresArray[j] = HighScoresArray[j-1];
							}
							HighScoresArray[i] = PlayersName+"|"+Playground.Achivements();
							SaveScores(ref HighScoresArray);
							afliw.Clear(0,Console.WindowHeight,32,Console.WindowWidth);
							ShowScoresTable(HighScoresArray,i);
						}					
					Console.SetCursorPosition(4, 8);
					Console.Write("Play");
					Console.SetCursorPosition(3, 9);
					Console.Write("again?");
					Menu Retry = new Menu(4, 11, -1, "Gray");
					Retry.AddItem("Yes", "No");
					if (Retry.DrawMenu(0) == 0)
					{
						RestartGame(ref ObPieza);
					}
					else
						Quit = true;
				}
			}while(!Quit);
			try
			{
				GamePlay.Abort();
			}
			catch(Exception)
			{
				GamePlay.Resume();
				GamePlay.Abort();
			}
		}


	}
}
