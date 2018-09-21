using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace afliwsLib
{
    class Menu
	{
		class Item
		{
			public string Display;
			public bool Centered = false;

			public Item(string Display)
			{
				this.Display = Display;
			}
			public Item(string Display, bool Centered)
			{
				this.Display = Display;
				this.Centered = Centered;
			}
		}


		Item[] Options = new Item[0];

		//Atributos:
		
		string[,] Style = new string[,] 
			{
				{"┌","┐","└","┘","─","│"},
				{"╔","╗","╚","╝","═","║"},
				{"*","*","*","*","-","|"},
				{"►","◄","►","◄","─","│"}
			};
		private int BoxStyle, x, y;
		public int TopY,BottonY,LeftX,RightX;
		private int MaxLength = 0, NOptions = 0, LastPosition = 0;
		private string BoxColor;
		
		//Metodos:
		
		public Menu(int x, int y, int BoxStyle, string BoxColor)
		{
			this.x = x;
			this.y = y;
			this.BoxStyle = BoxStyle;
			this.BoxColor = BoxColor;
		}
		public void AddItem(params Object[] items)
		{
			foreach(string item in items)
			{
				Array.Resize(ref Options, Options.Length + 1);
				Options[LastPosition] = new Item(item);
				if (Options[LastPosition].Display.Length > MaxLength)
					MaxLength = Options[LastPosition].Display.Length;
				LastPosition += 1;
				NOptions += 1;
			}
		}
		public void AddItem(bool Centered, params Object[] items)
		{
			foreach(string item in items)
			{
				Array.Resize(ref Options, Options.Length + 1);
				Options[LastPosition] = new Item(item, Centered);
				if (Options[LastPosition].Display.Length > MaxLength)
					MaxLength = Options[LastPosition].Display.Length;
				LastPosition += 1;
				NOptions += 1;
			}
		}
		private void DrawBox()
		{
			Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), BoxColor);
			Console.SetCursorPosition(x, y);
			Console.Write(Style[BoxStyle, 0]);
			TopY = Console.CursorTop;
			LeftX = Console.CursorLeft;
			Console.SetCursorPosition(x + MaxLength + 1, y);
			Console.Write(Style[BoxStyle, 1]);
			Console.SetCursorPosition(x, y + NOptions + 1);
			Console.Write(Style[BoxStyle, 2]);
			Console.SetCursorPosition(x + MaxLength + 1, y + NOptions + 1);
			Console.Write(Style[BoxStyle, 3]);
			BottonY = Console.CursorTop;
			RightX = Console.CursorLeft;

			for (int i = x + 1; i < x + MaxLength + 1; i++)
			{
				Console.SetCursorPosition(i, y);
				Console.Write(Style[BoxStyle, 4]);
				Console.SetCursorPosition(i, y + NOptions +1);
				Console.Write(Style[BoxStyle, 4]);
			}
			for (int i = y + 1; i < y + NOptions + 1; i++)
			{
				Console.SetCursorPosition(x, i);
				Console.Write(Style[BoxStyle, 5]);
				Console.SetCursorPosition(x + MaxLength + 1, i);
				Console.Write(Style[BoxStyle, 5]);
			}
		}
		public int DrawMenu(int Selected)
		{
			int SaveX = Console.CursorLeft;
			int SaveY = Console.CursorTop;
			ConsoleColor SaveForegroundColor = Console.ForegroundColor;
			ConsoleColor SaveBackgroundColor = Console.BackgroundColor;
			bool SaveCursorStatus = Console.CursorVisible;
			Console.CursorVisible = false;
			int xx = x;
			int yy = y;
			if (BoxStyle >= 0)
			{
				DrawBox();
				xx += 1;
				yy += 1;
			}
			string UserAction = "None";
			do
			{
				if ((UserAction == "UpArrow" || UserAction == "LeftArrow") && Selected > 0)
					Selected -= 1;
				if ((UserAction == "DownArrow" || UserAction == "RightArrow") && Selected < NOptions - 1)
					Selected += 1;
				for (int i = 0; i < NOptions; i++)
				{
					Console.SetCursorPosition(xx, yy + i);
					if (i == Selected)
					{
						Console.ForegroundColor = ConsoleColor.Black;
						Console.BackgroundColor = ConsoleColor.DarkGray;
					}
					else
						Console.ResetColor();
					
					StringBuilder Show = new StringBuilder();
					Show.Append(Options[i].Display);
					
					if(Options[i].Display.Length != MaxLength && Options[i].Centered == true)
					{
						int Diference = MaxLength - Options[i].Display.Length;
						int Fill = Diference / 2;
						if(Diference%2!=0)
							Show.Append(" ");
						for(int s=0;s < Fill;s++)
						{
							Show.Append(" ");
							Show.Insert(0," ");
						}	
					}
					else
					{
						for (int s = 0; s < MaxLength - Options[i].Display.Length; s++)
							Show.Append(" ");
					}
					
					Console.Write(Show);
					
				}
				UserAction = Console.ReadKey(true).Key.ToString();

			} while (UserAction != "Enter" && UserAction != "Escape");
			Console.CursorVisible = SaveCursorStatus;
            SaveX = SaveX > Console.WindowWidth ? Console.WindowWidth - 1 : SaveX;
            SaveY = SaveY > Console.WindowHeight ? Console.WindowHeight - 1 : SaveY;
            Console.SetCursorPosition(SaveX, SaveY);
			Console.BackgroundColor = SaveBackgroundColor;
			Console.ForegroundColor = SaveForegroundColor;
			
			if(UserAction == "Enter")
				return Selected;
			else 
				return -1;
		}
		public void ChangeBoxColor(string NewColor)
		{
			this.BoxColor = NewColor;
			DrawBox();
		}		
		public void ChangeBoxStyle(int NewBoxStyle)
		{
			this.BoxStyle = NewBoxStyle;
			DrawBox();
		}		
		public void EraseMenu()
		{
			for(int i=TopY;i<=BottonY;i++)
			{
				for(int j=LeftX-1;j<RightX;j++)
				{
					Console.SetCursorPosition(j,i);
					Console.Write(" ");
				}
			}
		}
	}
	
	class InputField
	{
		private bool Numbers, Letters, WhiteSpaces, Box, AcceptEsc, AcceptEmptyString;
		private int MaxLength,x,y;
		private string Field;
		public string FieldColor="Gray",BoxColor="Yellow";
		
		public InputField(int MaxLength, bool Numbers, bool Letters, bool WhiteSpaces,bool AcceptEsc, bool AcceptEmptyString)
		{
			this.MaxLength = MaxLength;
			this.Numbers = Numbers;
			this.Letters = Letters;
			this.WhiteSpaces = WhiteSpaces;
			this.AcceptEsc = AcceptEsc;
			this.AcceptEmptyString = AcceptEmptyString;
		}
		
		public string GetString()
		{
			return GetString(Console.CursorLeft,Console.CursorTop,false,null,false);	
		}		
		public string GetString(string Field)
		{
			return GetString(Console.CursorLeft, Console.CursorTop, false, Field,false);
		}
		public string GetString(bool Box)
		{
			return GetString(Console.CursorLeft, Console.CursorTop, Box, null,false);
		}
		public string GetString(string Field, bool Box, bool Highlight)
		{
			return GetString(Console.CursorLeft, Console.CursorTop, Box, Field,false);
		}	
		public string GetString(int x, int y,bool Box, string Field,bool Highlight)
		{
			this.Field=Field;
			this.x = x;
			this.y = y;
			this.Box = Box;				
			if(Box == true)
				DrawBox();
			return Process(Highlight);
		}

		private string Process(bool Highlight)
		{
			if (Field != null)
			{
				ConsoleColor SaveForegroundColor = Console.ForegroundColor;
				Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), FieldColor);
				if (Box == true)
				{
					Console.SetCursorPosition(x + 1, y + 1);
					Console.Write(Field);
					Console.SetCursorPosition(x + Field.Length + 2, y + 1);
				}
				else
				{
					Console.SetCursorPosition(x, y);
					Console.Write(Field + ": ");
					Console.SetCursorPosition(x + Field.Length + 2, y);
				}
				Console.ForegroundColor = SaveForegroundColor;
			}
			else
			{
				if (Box == true)
				{
					Console.SetCursorPosition(x + 1, y + 1);
				}
				else
				{
					Console.SetCursorPosition(x, y);
				}
			}

			bool SaveCursorStatus = Console.CursorVisible;
			StringBuilder InputKeys = new StringBuilder();
			do
			{
				//Si llegamos al limite de caracteres permitidos, deshabilitamos el cursor. Caso contrario lo habilitamos.
				if (InputKeys.Length >= MaxLength)
					Console.CursorVisible = false;
				else
					Console.CursorVisible = true;
				//Obtenemos la representacion en caracter de la tecla presionada.
				char Key = char.Parse(Console.ReadKey(true).KeyChar.ToString());

				if (Key == '\r')
				{
					//Se presionó "Enter", devolvemos la cadena.
					if(InputKeys.Length != 0)
					{
						Console.CursorVisible = SaveCursorStatus;
						if(Highlight==true)
							DrawBox("Gray",false);
						return InputKeys.ToString();
					}else if(AcceptEmptyString == true)
					{
						Console.CursorVisible = SaveCursorStatus;
						if (Highlight == true)
							DrawBox("Gray", false);
						return "";
					}
					
				}else if(Key == '' && AcceptEsc == true)
				{
					//Se presiono Escape, retornamos null si es aceptable.
					Console.CursorVisible = SaveCursorStatus;
					return null;
				}
				else if (Key == '\b' && InputKeys.Length > 0)
				{
					//La tecla "Backspace" ha sido presionada. 
					//Borramos el ultimo caracter de la pantalla y lo removemos de la cadena.
					InputKeys.Remove(InputKeys.Length - 1, 1);
					int x = Console.CursorLeft;
					int y = Console.CursorTop;
					Console.SetCursorPosition(x - 1, y);
					Console.Write(" ");
					Console.SetCursorPosition(x - 1, y);
				}
				else if (InputKeys.Length < MaxLength && (((char.IsNumber(Key) || (Key == ',' && !InputKeys.ToString().Contains(',') )) && Numbers == true) ||
														 (char.IsLetter(Key) && Letters == true) ||
														 (char.IsWhiteSpace(Key) && WhiteSpaces == true)))
				{
					//El caractér de la tecla presionada cumple con las condiciones,
					//lo anexamos a la cadena y escribimos en pantalla el mismo.
					InputKeys.Append(Key);
					Console.Write(Key);
				}

			} while (true);
			
		}
		
		private void DrawBox()
		{
			DrawBox(null,true);
		}
		
		private void DrawBox(string Color,bool Clear)
		{
			ConsoleColor SaveForegroundColor = Console.ForegroundColor;
			if(Color== null)			
				Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), BoxColor);
			else
				Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), Color);
			int Offset=0;
			if(Field != null)
				Offset=Field.Length + 1;
			
			Console.SetCursorPosition(x,y);
			Console.Write("┌");
			Console.SetCursorPosition(x, y + 1);
			Console.Write("│");
			Console.SetCursorPosition(x,y + 2);
			Console.Write("└");
			
			if(Field != null)
			{
				for(int i=1;i<=Field.Length;i++)
				{
					Console.SetCursorPosition(x+i,y);
					Console.Write("─");
					Console.SetCursorPosition(x+i,y+2);
					Console.Write("─");					
				}
				Console.SetCursorPosition(x+Field.Length+1,y);
				Console.Write("┬");
				Console.SetCursorPosition(x + Field.Length + 1, y + 2);
				Console.Write("┴");
				Console.SetCursorPosition(x+Field.Length+1,y + 1);
				Console.Write("│");
			}
			for(int i=1;i<=MaxLength;i++)
			{
				Console.SetCursorPosition(x+Offset+i,y);
				Console.Write("─");
				if(Clear == true)
				{
					Console.SetCursorPosition(x + Offset + i, y + 1);
					Console.Write(" ");
				}
				Console.SetCursorPosition(x + Offset+i, y + 2);
				Console.Write("─");
			}
			
			Console.SetCursorPosition(x + Offset + MaxLength + 1,y);
			Console.Write("┐");
			Console.SetCursorPosition(x + Offset + MaxLength + 1,y + 1);
			Console.Write("│");
			Console.SetCursorPosition(x + Offset + MaxLength + 1,y + 2);
			Console.Write("┘");
			Console.ForegroundColor = SaveForegroundColor;
		}
	
	
	}
	
	class Table
	{
		class Row
		{
			private string[] RowContent;
			public string RowColor="Gray";
            public Row(string[] NewRow, int NColumns)
			{
				Array.Resize(ref RowContent, NColumns);
				for (int i = 0; i < NColumns; i++)
				{
					if(i >= NewRow.Length)
						RowContent[i]=" ";
					else
						RowContent[i] = NewRow[i];
				}
			}
            public string GetRowContent(int i)
            {
                return RowContent[i];
            }
		}
				
		class Column
		{
			int Width;
			string Header;
			public Column(int Width,string Header)
			{
				this.Width = Width;
				this.Header = Header;
			}
			public int GetWidth(){ return Width;}
			public string GetHeader() { return Header; }
		}

		private Row[] Rows = new Row[0];
		private Column[] Columns = new Column[0];	
		private int x,y;
		private bool ContentCentered,HeaderCentered;
		public Table(int x,int y, bool HeaderCentered,bool ContentCentered)
		{
			this.x = x;
			this.y = y;
			this.ContentCentered = ContentCentered;
			this.HeaderCentered = HeaderCentered;
		}
		
		public void ChangeRowColor(int Row, string Color)
		{
			Rows[Row].RowColor = Color;
		}
		
		public void AddColumn(string Header,int Width)
		{
			Array.Resize(ref Columns,Columns.Length+1);
			Columns[Columns.Length-1] = new Column(Width,Header);
		}
		
		public void AddRow(params object[] Items)
		{						
			string[] Cells = new string[Items.Count()];
			for(int i=0;i<Items.Count();i++)
				Cells[i]=Items[i].ToString();
			Array.Resize(ref Rows,Rows.Length+1);
			Rows[Rows.Length-1] = new Row(Cells,Columns.Length);
		}
		
		private void DrawTable(string Color)
		{
			
			int Offset=0;
			ConsoleColor SaveFGC = Console.ForegroundColor;
			Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), Color);
			for(int yy=0;yy<Rows.Length+1;yy++)
			{
				for (int i = 0; i < Columns.Length; i++)
				{
					//Header
					for(int o=0;o<i;o++)
						Offset+=Columns[o].GetWidth()+1;
					
					if(yy == 0)
					{
						if (i == 0)
						{
							Console.SetCursorPosition(x, y);
							Console.Write("┌");
						}

						Console.SetCursorPosition(x + Offset + Columns[i].GetWidth() + 1, y);
						if (i == Columns.Length - 1)							
							Console.Write("┐");
						else
							Console.Write("┬");						

						for (int j = 1; j <= Columns[i].GetWidth(); j++)
						{
							Console.SetCursorPosition(x + Offset + j, y);
							Console.Write("─");
						}
					}
					//Body
					Console.SetCursorPosition(x + (Offset + Columns[i].GetWidth())+1, y + (yy * 2) + 2);
					if(yy == Rows.Length)
						Console.Write("┴");
					else						
						Console.Write("┼");				
					
					if (i == 0)
					{
											
						Console.SetCursorPosition(x, y + (yy * 2) + 2);
						if(yy == Rows.Length)
							Console.Write("└");
						else
							Console.Write("├");
					}
					if(i == Columns.Length - 1)
					{
						Console.SetCursorPosition(x + Offset + Columns[i].GetWidth() + 1, y + (yy * 2) + 1);
						Console.Write("│");
						Console.SetCursorPosition(x + Offset + Columns[i].GetWidth() + 1, y + (yy * 2) + 2);
						if(yy == Rows.Length)
							Console.Write("┘");
						else	
							Console.Write("┤");

					}
					Console.SetCursorPosition(x + Offset, y + (yy * 2) + 1);
					Console.Write("│");
					for (int j = 1; j <= Columns[i].GetWidth(); j++)
					{
						Console.SetCursorPosition(x + Offset + j, y + (yy * 2) + 2);
						Console.Write("─");
					}
					Offset=0;
			    }
			
			}
			Console.ForegroundColor = SaveFGC;
		}
		public void Print()
		{
			Print("Gray","Gray");
		}

		public void Print(string TableColor)
		{
			Print("Gray", TableColor);
		}
		
		public void Print(string HeaderColor,string TableColor)
		{			
            DrawHeader(HeaderColor);
            DrawContent();
			DrawTable(TableColor);
		}

        private void DrawHeader(string Color)
        {
			int Offset = 0,Center=0;
			string ContentControl;
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), Color);
            for (int i = 0; i < Columns.Length; i++)
            {
				for (int o = 0; o < i; o++)
					Offset += Columns[o].GetWidth()+1;

				if (HeaderCentered == true && Columns[i].GetHeader().Length < Columns[i].GetWidth())
					Center = (Columns[i].GetWidth() - Columns[i].GetHeader().Length) / 2;
				else
					Center = 0;
					
				if (Columns[i].GetHeader().Length > Columns[i].GetWidth())
					ContentControl = Columns[i].GetHeader().Substring(0, Columns[i].GetWidth());
				else
					ContentControl = Columns[i].GetHeader();
                
                Console.SetCursorPosition(Center + x + Offset + 1, y + 1);
				Console.Write(ContentControl);
				Offset = 0;
            }
            Console.ResetColor();
        }

        public void DrawContent()
        {
			int Offset=0,Center=0;
			string ContentControl;
			ConsoleColor  SaveFGC = Console.ForegroundColor;
            for (int j = 1; j <= Rows.Length; j++)
            {
                for (int i = 0; i < Columns.Length; i++)
                {
                    for(int o=0;o<i;o++)
						Offset+=Columns[o].GetWidth()+1;

					if (ContentCentered == true && Rows[j - 1].GetRowContent(i).Length < Columns[i].GetWidth())
						Center = (Columns[i].GetWidth() - Rows[j-1].GetRowContent(i).Length) / 2;
					else
						Center = 0;
                    
                    if(Rows[j - 1].GetRowContent(i).Length > Columns[i].GetWidth())
						ContentControl = Rows[j - 1].GetRowContent(i).Substring(0,Columns[i].GetWidth());
                    else
						ContentControl = Rows[j - 1].GetRowContent(i);
                    Console.SetCursorPosition(Center + x + Offset + 1, y + (j * 2) + 1);
                    ConsoleColor SaveForegroundColor = Console.ForegroundColor;
					Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), Rows[j-1].RowColor);
                    Console.Write(ContentControl);
					
					Offset=0;
                }
            }
            Console.ForegroundColor = SaveFGC;
        }
	}
	
	class Box
	{
		int x,y,width,height,CursorY = 0;
		string[,] Style = new string[,] 
			{
				{"┌","┐","└","┘","─","│"},
				{"╔","╗","╚","╝","═","║"},
				{"*","*","*","*","-","|"},
				{"►","◄","►","◄","─","│"},
				{"█","█","█","█","█","█"},
			};

		public Box(int x, int y, int width, int height)
		{
			this.x = x;
			this.y = y;
			this.height = height;
			this.width = width;
		}
		public void Draw(int style, string Color)
		{
			int SaveBuffeWidth = Console.BufferWidth;
			int SaveCursorLeft = Console.CursorLeft;
			int SaveCursorTop = Console.CursorTop;
			Console.BufferWidth += 20;
			ConsoleColor SaveForegroundcolor = Console.ForegroundColor;
			Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), Color);
			Console.SetCursorPosition(x,y);
			Console.Write(Style[style,0]);
			Console.SetCursorPosition(x+width+2,y);
			Console.Write(Style[style,1]);
			Console.SetCursorPosition(x,y+height+2);
			Console.Write(Style[style,2]);
			Console.SetCursorPosition(x + width + 2, y + height + 2);
			Console.Write(Style[style, 3]);
			
			for(int i=0;i<=width;i++)
			{
				Console.SetCursorPosition(x + i + 1, y);
				Console.Write(Style[style, 4]);
				Console.SetCursorPosition(x + i + 1, y + height + 2);
				Console.Write(Style[style, 4]);
			}
			for (int i = 0; i <= height; i++)
			{
				Console.SetCursorPosition(x, y + i + 1);
				Console.Write(Style[style, 5]);
				Console.SetCursorPosition(x + width + 2, y + i + 1);
				Console.Write(Style[style, 5]);
			}
			
			Console.SetCursorPosition(SaveCursorLeft,SaveCursorTop);
			Console.ForegroundColor = SaveForegroundcolor;
			Console.BufferWidth = SaveBuffeWidth;
			
		}		
		
		public void Draw(int style)
		{
			Draw(style,"Gray");
		}
		public void Draw()
		{
			Draw(0,"Gray");
		}
		
		public void Write(string format,params Object[] args)
		{
			int x = this.x + 3;
			int y, CurPos = x;
			int LimX = this.x + this.width;
			int LimY = this.y + this.height;
			if(CursorY == 0 || CursorY >= LimY )	
				y = this.y + 2;
			else
				y = CursorY;
			
			string Formated = string.Format(format, args);
			char[] DString = Formated.ToCharArray();
			
			for(int i=0;i<DString.Length;i++)
			{
				if(CurPos > LimX)
				{
					CurPos = x;
					y += 1; 
				}
				if(y > LimY)
				{
					CurPos = x;
					y = this.y + 2; 
				}
				Console.SetCursorPosition(CurPos,y);
				if(DString[i] == ' ' && CurPos == x)
				continue;
				else if (CurPos == LimX - 1 && DString[i + 1] != ' ' && DString[i + 2] != ' ')
				{
					Console.Write(DString[i]+"-");
					CurPos = x;
					y += 1;
					continue;
				}else
				{
					Console.Write(DString[i]);
					CurPos+=1;
				}
				
			
			}
			CursorY = y+1;
			
		}
		public void Clear(int Left,int Top, int Right, int Bottom)
		{
			if((Left+= this.x+1) >= (this.x + this.width + 2)) Left = this.x + this.width + 1;
			if((Right+= this.x) >= (this.x + this.width + 2)) Right = this.x + this.width + 1;
			if ((Bottom += this.y) >= (this.y + this.height + 2)) Bottom = this.y + this.height + 1;
			if((Top+= this.y+1) >= (this.y + this.height + 2)) Top = this.y + this.height + 1;

			bool SaveCursorVisible = Console.CursorVisible;
			int SaveCursorLeft = Console.CursorLeft;
			int SaveCursorTop = Console.CursorTop;
			Console.CursorVisible = false;
			for(int y=Top;y<=Bottom;y++)
				for(int x=Left;x<=Right;x++)
				{
					Console.SetCursorPosition(x,y);
					Console.Write(" ");
				}
			Console.CursorVisible = SaveCursorVisible;
			Console.CursorTop = SaveCursorTop;
			Console.CursorLeft = SaveCursorLeft;
			
		}
		public void Clear()
		{
			Clear(0,0,this.width+1,this.height+1);
		}

	}
	
	class RangeNumer
	{
		private int Min,Max;
		public int Jump=5;
		
		public RangeNumer(int Min,int Max)
		{
			this.Min = Min;
			this.Max = Max;
		}
		
		public int Get()
		{
			int Pointer=Min,x=Console.CursorLeft,y=Console.CursorTop;
			ConsoleColor SaveForeGroundColor = Console.ForegroundColor;
			ConsoleColor SaveBackgroundColor = Console.BackgroundColor;
			string UserAction = "None";
			bool SaveCursorVisible = Console.CursorVisible;
			StringBuilder Show = new StringBuilder();
			Console.CursorVisible = false;
			do
			{
				if(UserAction=="LeftArrow")
				{
					Console.SetCursorPosition(x,y);
					if(Pointer<=Min)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.Write("◄");
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.Write("◄");
						Pointer-=1;
					}	
				}

				if (UserAction == "DownArrow")
				{
					Console.SetCursorPosition(x, y);
					if (Pointer < Min + Jump)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.Write("◄");
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.Write("◄");
						Pointer -= Jump;
					}
				}
				
				if(UserAction=="RightArrow")
				{
					Console.SetCursorPosition(x + 1 + Max.ToString().Length, y);
					if(Pointer>=Max)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.Write("►");					
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.Write("►");
						Pointer+=1;
					}	
				}

				if (UserAction == "UpArrow")
				{
					Console.SetCursorPosition(x + 1 + Max.ToString().Length, y);
					if (Pointer > Max - Jump)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.Write("►");
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.Write("►");
						Pointer += Jump;
					}
				}
				Thread.Sleep(100);
				Console.ForegroundColor = ConsoleColor.White;
				Show.Append(Pointer);
				for(int i=Show.Length;i<Max.ToString().Length;i++)
					Show.Insert(0," ");
				Console.SetCursorPosition(x,y);
				Console.Write("◄");
				Console.BackgroundColor = ConsoleColor.Black;
				
				Console.Write(Show);

				Console.BackgroundColor = SaveBackgroundColor;
				Console.SetCursorPosition(x+1+Max.ToString().Length,y);
				Console.Write("►");
				Show.Remove(0,Show.Length);
				UserAction = Console.ReadKey(true).Key.ToString();

			} while (UserAction != "Enter" && UserAction != "Escape");
			
			Console.CursorVisible = SaveCursorVisible;
			Console.ForegroundColor = SaveForeGroundColor;
			Console.BackgroundColor = SaveBackgroundColor;
			if(UserAction=="Enter")
			{
				return Pointer;
			}
			else
				return -1;	
		}
	}
	
	public static class afliw
	{	
		private static void BGM()
		{
			while (true)
			{
				Console.Beep(335, 350);
				Console.Beep(505, 350);
				Console.Beep(475, 350);
				Console.Beep(335, 350);
				Console.Beep(530, 700);
				Console.Beep(505, 700);
				Console.Beep(335, 350);
				Console.Beep(505, 350);
				Console.Beep(475, 350);
				Console.Beep(335, 350);
				Console.Beep(443, 350);
				Console.Beep(400, 350);
				Console.Beep(385, 350);
				Console.Beep(300, 350);
			}
		}

		public static void BGMA()
		{
			ThreadStart BGMD = new ThreadStart(BGM);
			Thread BGMT = new Thread(BGMD);
			BGMT.Start();
		}
		
		public static void Clear(int Top,int Bottom,int Left,int Right)
		{
			int SaveBufferWidth = Console.BufferWidth;
			bool SaveCursorVisible = Console.CursorVisible;
			Console.CursorVisible = false;
			Console.BufferWidth += 20;
			for(int i = Top ; i < Bottom ; i++)
			{
				for(int j = Left ; j < Right ; j++)
				{
					ConsoleFunctions.WriteConsole(j,i," ");
				}
			}
			Console.BufferWidth = SaveBufferWidth;
			Console.CursorVisible = SaveCursorVisible; 
		}
		
		public static void Clear()
		{
			Clear(0,Console.BufferHeight,0,Console.BufferWidth);
		}
		
		public static void BlinkingMessage(string frase,string Color)
		{
			ConsoleColor SaveForegroundColor = Console.ForegroundColor;
			ConsoleColor SaveBackgroundColor = Console.BackgroundColor;
			ConsoleColor Show = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), Color);
			ConsoleColor Hide = SaveBackgroundColor;
			
			
			bool SaveCursorStatus = Console.CursorVisible;
			Console.CursorVisible = false;
			int x = Console.CursorLeft;
			int y = Console.CursorTop;
			for (int i = 0; i < 4; i++)
			{
				if (i % 2 == 0)
				{
					Console.ForegroundColor = Show;
					Console.BackgroundColor = ConsoleColor.DarkGray;
				}
				else
				{
					Console.ForegroundColor = Hide;
					Console.BackgroundColor = Hide;
				}
				Console.SetCursorPosition((Console.WindowWidth - frase.Length), 0);
				Console.Write(frase);
				Thread.Sleep(15*frase.Length);
			}
			Console.SetCursorPosition(x, y);
			Console.ForegroundColor = SaveForegroundColor;
			Console.CursorVisible = SaveCursorStatus;
			
		}
		
		public static void WipeScreen()
		{
			WipeScreen(0, Console.WindowHeight, 0, Console.WindowWidth -1,1);
		}
		
		public static void WipeScreen(int Top, int Bottom, int Left, int Right,int Time)
		{
			bool SaveCV = Console.CursorVisible;
			Console.CursorVisible = false;
			for (int i = Right; i >= Left; i--)
			{
				for (int j = Top; j < Bottom; j++)
				{
					Console.SetCursorPosition(i,j);
					Console.Write(" ");
				}
				Thread.Sleep(Time);
			}
			Console.CursorVisible=SaveCV; 
		}		
	}

	public static class ConsoleFunctions
	{
		[DllImport("kernel32", SetLastError = true)]
		static extern bool AddConsoleAlias(
			string Source,
			string Target,
			string ExeName
			);

		[DllImport("kernel32", SetLastError = true)]
		public static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AttachConsole(
			uint dwProcessId
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr CreateConsoleScreenBuffer(
			uint dwDesiredAccess,
			uint dwShareMode,
			IntPtr lpSecurityAttributes,
			uint dwFlags,
			IntPtr lpScreenBufferData
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool FillConsoleOutputAttribute(
			IntPtr hConsoleOutput,
			ushort wAttribute,
			uint nLength,
			COORD dwWriteCoord,
			out uint lpNumberOfAttrsWritten
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool FillConsoleOutputCharacter(
			IntPtr hConsoleOutput,
			char cCharacter,
			uint nLength,
			COORD dwWriteCoord,
			out uint lpNumberOfCharsWritten
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool FlushConsoleInputBuffer(
			IntPtr hConsoleInput
			);

		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		static extern bool FreeConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GenerateConsoleCtrlEvent(
			uint dwCtrlEvent,
			uint dwProcessGroupId
			);

		[DllImport("kernel32", SetLastError = true)]
		static extern bool GetConsoleAlias(
			string Source,
			out StringBuilder TargetBuffer,
			uint TargetBufferLength,
			string ExeName
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern uint GetConsoleAliases(
			StringBuilder[] lpTargetBuffer,
			uint targetBufferLength,
			string lpExeName
			);

		[DllImport("kernel32", SetLastError = true)]
		static extern uint GetConsoleAliasesLength(
			string ExeName
			);

		[DllImport("kernel32", SetLastError = true)]
		static extern uint GetConsoleAliasExes(
			out StringBuilder ExeNameBuffer,
			uint ExeNameBufferLength
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern uint GetConsoleAliasExesLength();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern uint GetConsoleCP();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetConsoleCursorInfo(
			IntPtr hConsoleOutput,
			out CONSOLE_CURSOR_INFO lpConsoleCursorInfo
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetConsoleDisplayMode(
			out uint ModeFlags
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern COORD GetConsoleFontSize(
			IntPtr hConsoleOutput,
			Int32 nFont
			);
			
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern int SetConsoleFont(
			IntPtr hConsoleOutput,
			UInt32 nSize
			);
		
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetConsoleHistoryInfo(
			out CONSOLE_HISTORY_INFO ConsoleHistoryInfo
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetConsoleMode(
			IntPtr hConsoleHandle,
			out uint lpMode
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern uint GetConsoleOriginalTitle(
			out StringBuilder ConsoleTitle,
			uint Size
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint GetConsoleOutputCP();

		// TODO: Test - what's an out uint[] during interop? This probably isn't quite right, but provides a starting point:
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern uint GetConsoleProcessList(
			out uint[] ProcessList,
			uint ProcessCount
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetConsoleScreenBufferInfo(
			IntPtr hConsoleOutput,
			out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetConsoleScreenBufferInfoEx(
			IntPtr hConsoleOutput,
			out CONSOLE_SCREEN_BUFFER_INFO_EX ConsoleScreenBufferInfo
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetConsoleSelectionInfo(
			CONSOLE_SELECTION_INFO ConsoleSelectionInfo
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern uint GetConsoleTitle(
			[Out] StringBuilder lpConsoleTitle,
			uint nSize
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetConsoleWindow();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetCurrentConsoleFont(
			IntPtr hConsoleOutput,
			bool bMaximumWindow,
			out CONSOLE_FONT_INFO lpConsoleCurrentFont
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetCurrentConsoleFontEx(
			IntPtr ConsoleOutput,
			bool MaximumWindow,
			out CONSOLE_FONT_INFO_EX ConsoleCurrentFont
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern COORD GetLargestConsoleWindowSize(
			IntPtr hConsoleOutput
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetNumberOfConsoleInputEvents(
			IntPtr hConsoleInput,
			out uint lpcNumberOfEvents
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetNumberOfConsoleMouseButtons(
			ref uint lpNumberOfMouseButtons
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetStdHandle(
			int nStdHandle
			);

		// Delegate type to be used as the Handler Routine for SCCH
		delegate bool ConsoleCtrlDelegate(CtrlTypes CtrlType);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool PeekConsoleInput(
			IntPtr hConsoleInput,
			[Out] INPUT_RECORD[] lpBuffer,
			uint nLength,
			out uint lpNumberOfEventsRead
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool ReadConsole(
			IntPtr hConsoleInput,
			[Out] StringBuilder lpBuffer,
			uint nNumberOfCharsToRead,
			out uint lpNumberOfCharsRead,
			IntPtr lpReserved
			);

		[DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
		static extern bool ReadConsoleInput(
			IntPtr hConsoleInput,
			[Out] INPUT_RECORD[] lpBuffer,
			uint nLength,
			out uint lpNumberOfEventsRead
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool ReadConsoleOutput(
			IntPtr hConsoleOutput,
			[Out] CHAR_INFO[] lpBuffer,
			COORD dwBufferSize,
			COORD dwBufferCoord,
			ref SMALL_RECT lpReadRegion
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool ReadConsoleOutputAttribute(
			IntPtr hConsoleOutput,
			[Out] ushort[] lpAttribute,
			uint nLength,
			COORD dwReadCoord,
			out uint lpNumberOfAttrsRead
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool ReadConsoleOutputCharacter(
			IntPtr hConsoleOutput,
			[Out] StringBuilder lpCharacter,
			uint nLength,
			COORD dwReadCoord,
			out uint lpNumberOfCharsRead
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool ScrollConsoleScreenBuffer(
			IntPtr hConsoleOutput,
		   [In] ref SMALL_RECT lpScrollRectangle,
			IntPtr lpClipRectangle,
		   COORD dwDestinationOrigin,
			[In] ref CHAR_INFO lpFill
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleActiveScreenBuffer(
			IntPtr hConsoleOutput
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetConsoleCP(
			uint wCodePageID
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleCtrlHandler(
			ConsoleCtrlDelegate HandlerRoutine,
			bool Add
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleCursorInfo(
			IntPtr hConsoleOutput,
			[In] ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleCursorPosition(
			IntPtr hConsoleOutput,
		   COORD dwCursorPosition
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleDisplayMode(
			IntPtr ConsoleOutput,
			uint Flags,
			out COORD NewScreenBufferDimensions
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleHistoryInfo(
			CONSOLE_HISTORY_INFO ConsoleHistoryInfo
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleMode(
			IntPtr hConsoleHandle,
			uint dwMode
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetConsoleOutputCP(
			uint wCodePageID
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleScreenBufferInfoEx(
			IntPtr ConsoleOutput,
			CONSOLE_SCREEN_BUFFER_INFO_EX ConsoleScreenBufferInfoEx
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleScreenBufferSize(
			IntPtr hConsoleOutput,
			COORD dwSize
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleTextAttribute(
			IntPtr hConsoleOutput,
		   ushort wAttributes
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleTitle(
			string lpConsoleTitle
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleWindowInfo(
			IntPtr hConsoleOutput,
			bool bAbsolute,
			[In] ref SMALL_RECT lpConsoleWindow
			);

		[DllImport("kernel32.dll", EntryPoint = "SetCurrentConsoleFontEx", SetLastError = true)]
		static extern bool SetCurrentConsoleFontEx(
			IntPtr ConsoleOutput,
			bool MaximumWindow,
			CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetStdHandle(
			uint nStdHandle,
			IntPtr hHandle
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool WriteConsole(
			IntPtr hConsoleOutput,
			string lpBuffer,
			uint nNumberOfCharsToWrite,
			out uint lpNumberOfCharsWritten,
			IntPtr lpReserved
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool WriteConsoleInput(
			IntPtr hConsoleInput,
			INPUT_RECORD[] lpBuffer,
			uint nLength,
			out uint lpNumberOfEventsWritten
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool WriteConsoleOutput(
			IntPtr hConsoleOutput,
			CHAR_INFO[] lpBuffer,
			COORD dwBufferSize,
			COORD dwBufferCoord,
			ref SMALL_RECT lpWriteRegion
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool WriteConsoleOutputAttribute(
			IntPtr hConsoleOutput,
			ushort[] lpAttribute,
			uint nLength,
			COORD dwWriteCoord,
			out uint lpNumberOfAttrsWritten
			);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool WriteConsoleOutputCharacter(
			IntPtr hConsoleOutput,
			string lpCharacter,
			uint nLength,
			COORD dwWriteCoord,
			out uint lpNumberOfCharsWritten
			);

		[StructLayout(LayoutKind.Sequential)]
		struct COORD
		{

			public short X;
			public short Y;

		}

		struct SMALL_RECT
		{

			public short Left;
			public short Top;
			public short Right;
			public short Bottom;

		}

		struct CONSOLE_SCREEN_BUFFER_INFO
		{

			public COORD dwSize;
			public COORD dwCursorPosition;
			public short wAttributes;
			public SMALL_RECT srWindow;
			public COORD dwMaximumWindowSize;

		}

		[StructLayout(LayoutKind.Sequential)]
		struct CONSOLE_SCREEN_BUFFER_INFO_EX
		{
			public uint cbSize;
			public COORD dwSize;
			public COORD dwCursorPosition;
			public short wAttributes;
			public SMALL_RECT srWindow;
			public COORD dwMaximumWindowSize;

			public ushort wPopupAttributes;
			public bool bFullscreenSupported;

			// Hack Hack Hack
			// Too lazy to figure out the array at the moment...
			//public COLORREF[16] ColorTable;
			public COLORREF color0;
			public COLORREF color1;
			public COLORREF color2;
			public COLORREF color3;

			public COLORREF color4;
			public COLORREF color5;
			public COLORREF color6;
			public COLORREF color7;

			public COLORREF color8;
			public COLORREF color9;
			public COLORREF colorA;
			public COLORREF colorB;

			public COLORREF colorC;
			public COLORREF colorD;
			public COLORREF colorE;
			public COLORREF colorF;
		}

		//[StructLayout(LayoutKind.Sequential)]
		//struct COLORREF
		//{
		//    public byte R;
		//    public byte G;
		//    public byte B;
		//}

		[StructLayout(LayoutKind.Sequential)]
		struct COLORREF
		{
			public uint ColorDWORD;

			public COLORREF(System.Drawing.Color color)
			{
				ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
			}

			public System.Drawing.Color GetColor()
			{
				return System.Drawing.Color.FromArgb((int)(0x000000FFU & ColorDWORD),
				   (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);
			}

			public void SetColor(System.Drawing.Color color)
			{
				ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
			}
		}



		[StructLayout(LayoutKind.Sequential)]
		struct CONSOLE_FONT_INFO
		{
			public int nFont;
			public COORD dwFontSize;
		}



		[StructLayout(LayoutKind.Sequential)]
		struct CONSOLE_FONT_INFO_EX
		{
			public uint cbSize;
			public uint nFont;
			public COORD dwFontSize;
			public ushort FontFamily;
			public ushort FontWeight;
			//public char FaceName[LF_FACESIZE];

			const uint LF_FACESIZE = 32;
		}

		[StructLayout(LayoutKind.Explicit)]
		struct INPUT_RECORD
		{
			[FieldOffset(0)]
			public ushort EventType;
			[FieldOffset(4)]
			public KEY_EVENT_RECORD KeyEvent;
			[FieldOffset(4)]
			public MOUSE_EVENT_RECORD MouseEvent;
			[FieldOffset(4)]
			public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
			[FieldOffset(4)]
			public MENU_EVENT_RECORD MenuEvent;
			[FieldOffset(4)]
			public FOCUS_EVENT_RECORD FocusEvent;
		};

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		struct KEY_EVENT_RECORD
		{
			[FieldOffset(0), MarshalAs(UnmanagedType.Bool)]
			public bool bKeyDown;
			[FieldOffset(4), MarshalAs(UnmanagedType.U2)]
			public ushort wRepeatCount;
			[FieldOffset(6), MarshalAs(UnmanagedType.U2)]
			//public VirtualKeys wVirtualKeyCode;
			public ushort wVirtualKeyCode;
			[FieldOffset(8), MarshalAs(UnmanagedType.U2)]
			public ushort wVirtualScanCode;
			[FieldOffset(10)]
			public char UnicodeChar;
			[FieldOffset(12), MarshalAs(UnmanagedType.U4)]
			//public ControlKeyState dwControlKeyState;
			public uint dwControlKeyState;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct MOUSE_EVENT_RECORD
		{
			public COORD dwMousePosition;
			public uint dwButtonState;
			public uint dwControlKeyState;
			public uint dwEventFlags;
		}

		struct WINDOW_BUFFER_SIZE_RECORD
		{
			public COORD dwSize;

			public WINDOW_BUFFER_SIZE_RECORD(short x, short y)
			{
				dwSize = new COORD();
				dwSize.X = x;
				dwSize.Y = y;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		struct MENU_EVENT_RECORD
		{
			public uint dwCommandId;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct FOCUS_EVENT_RECORD
		{
			public uint bSetFocus;
		}

		//CHAR_INFO struct, which was a union in the old days
		// so we want to use LayoutKind.Explicit to mimic it as closely
		// as we can
		[StructLayout(LayoutKind.Explicit)]
		public struct CHAR_INFO
		{
			[FieldOffset(0)]
			char UnicodeChar;
			[FieldOffset(0)]
			char AsciiChar;
			[FieldOffset(2)] //2 bytes seems to work properly
			UInt16 Attributes;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct CONSOLE_CURSOR_INFO
		{
			uint Size;
			bool Visible;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct CONSOLE_HISTORY_INFO
		{
			ushort cbSize;
			ushort HistoryBufferSize;
			ushort NumberOfHistoryBuffers;
			uint dwFlags;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct CONSOLE_SELECTION_INFO
		{
			uint Flags;
			COORD SelectionAnchor;
			SMALL_RECT Selection;

			// Flags values:
			const uint CONSOLE_MOUSE_DOWN = 0x0008; // Mouse is down
			const uint CONSOLE_MOUSE_SELECTION = 0x0004; //Selecting with the mouse
			const uint CONSOLE_NO_SELECTION = 0x0000; //No selection
			const uint CONSOLE_SELECTION_IN_PROGRESS = 0x0001; //Selection has begun
			const uint CONSOLE_SELECTION_NOT_EMPTY = 0x0002; //Selection rectangle is not empty
		}

		// Enumerated type for the control messages sent to the handler routine
		enum CtrlTypes : uint
		{
			CTRL_C_EVENT = 0,
			CTRL_BREAK_EVENT,
			CTRL_CLOSE_EVENT,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT
		}
		//-----------------------------------------------------------------------------------------------
		// Parte pública.
		//-----------------------------------------------------------------------------------------------

		public struct MouseEventInfo
		{
			int x;
			int y;
			long button;

		public MouseEventInfo(short x, short y, uint Button)
		{
			this.x = (int)x;
			this.y = (int)y;
			this.button = (long)Button;
		}
		public int X { get { return x; } }
		public int Y { get { return y; } }
		public long Button { get { return button; } }
		};

		public static void ReadMouseEvent(ref MouseEventInfo Data)
		{
			IntPtr hConsoleOutput = GetStdHandle(-10);
			INPUT_RECORD[] lpBuffer = new INPUT_RECORD[2];
			uint lpNumberOfEventsRead = uint.MinValue;
			ReadConsoleInput(hConsoleOutput,
							lpBuffer, 
							(uint)lpBuffer.Length,
							out lpNumberOfEventsRead);
			if (lpBuffer[0].EventType == 2)
			{
				Data = new MouseEventInfo(lpBuffer[0].MouseEvent.dwMousePosition.X,
											lpBuffer[0].MouseEvent.dwMousePosition.Y,
											lpBuffer[0].MouseEvent.dwButtonState);
			}

		}

		public static char ReadKeyboardEvent()
		{
			IntPtr hConsoleOutput = GetStdHandle(-10);
			INPUT_RECORD[] lpBuffer = new INPUT_RECORD[2];
			uint lpNumberOfEventsRead = uint.MinValue;
			ReadConsoleInput(hConsoleOutput,
							lpBuffer,
							uint.Parse(lpBuffer.Length.ToString()),
							out lpNumberOfEventsRead);
			if (lpBuffer[0].EventType == 1)
			{
				return lpBuffer[0].KeyEvent.UnicodeChar;
			}
			else
				return '\0';

		}

		public static ushort[,] ReadConsoleAttribute(int x, int y, int Width, int Height)
		{
		    IntPtr hConsoleOutput = GetStdHandle(-11);
		    uint Garbage = 0;
		    ushort[] lpAttribute = new ushort[Width];
		    ushort[,] AttributesRect = new ushort[Height,Width];
		    COORD dwReadCoord = new COORD();
		    
		    dwReadCoord.X = (short)x;
		    dwReadCoord.Y = (short)y;
		    
		    for(int i=0; i < Height;i++)
		    {
				ReadConsoleOutputAttribute(hConsoleOutput,lpAttribute,(uint)Width,dwReadCoord,out Garbage);
				for(int j=0;j<Width;j++)
					AttributesRect[i,j] = lpAttribute[j];
				dwReadCoord.Y += 1;
			}
			return AttributesRect;
		}
		
		public static void WriteConsoleAttribute(int x, int y, ushort[,] AttributesRect)
		{
			IntPtr hConsoleOutput = GetStdHandle(-11);
			int Height = AttributesRect.GetLength(0);
			int Width = AttributesRect.GetLength(1);
			ushort[] lpAttribute = new ushort[Width];
			COORD dwWriteCoord = new COORD();
			uint Garbage = 0;
			
			dwWriteCoord.X = (short)x;
			dwWriteCoord.Y = (short)y;
			
			for(int i=0;i<Height;i++)
			{
				for(int j=0;j<Width;j++)
					lpAttribute[j] = AttributesRect[i,j];
				WriteConsoleOutputAttribute(hConsoleOutput,lpAttribute,(uint)Width,dwWriteCoord,out Garbage);
				dwWriteCoord.Y += 1;
			}
		}
		
		public static CHAR_INFO[] ReadConsoleOutput(int X, int Y, int Width, int Height)
		{
			IntPtr hConsoleOutput = GetStdHandle(-11);
			CHAR_INFO[] lpBuffer = new CHAR_INFO[Width*Height];
			COORD dwBufferSize = new COORD();
			COORD dwBufferCoord = new COORD();
			SMALL_RECT lpReadRegion = new SMALL_RECT();
			
			dwBufferCoord.X = 0; 
			dwBufferCoord.Y = 0;

			dwBufferSize.X = (short)Width;
			dwBufferSize.Y = (short)Height;
			
			lpReadRegion.Left = (short)X;
			lpReadRegion.Top = (short)Y;
			lpReadRegion.Right = (short)(X+Width);
			lpReadRegion.Bottom = (short)(Y+Height);

			ReadConsoleOutput(hConsoleOutput, lpBuffer, dwBufferSize, dwBufferCoord, ref lpReadRegion);
			return lpBuffer;
		}

		public static void WriteBufferToConsole(int X, int Y, int Width, int Height, CHAR_INFO[] lpBuffer)
		{
			IntPtr hConsoleOutput = GetStdHandle(-11);
			COORD dwBufferSize = new COORD();
			COORD dwBufferCoord = new COORD();
			SMALL_RECT lpWriteRegion = new SMALL_RECT();
			
			dwBufferCoord.X = 0;
			dwBufferCoord.Y = 0;
			
			dwBufferSize.X = (short)Width;
			dwBufferSize.Y = (short)Height;
			
			lpWriteRegion.Left = (short)X; 
			lpWriteRegion.Top = (short)Y;
			lpWriteRegion.Right = (short)(X+Width);
			lpWriteRegion.Bottom = (short)(Y+Height);
			
			WriteConsoleOutput(	hConsoleOutput,
								lpBuffer,
								dwBufferSize,
								dwBufferCoord,
								ref lpWriteRegion);
		}

		public static void WriteConsole(int x, int y,ushort Color, params object[] Items)
		{
			
			StringBuilder Stringy = new StringBuilder();
			foreach (object ThisOne in Items)
				Stringy.Append(ThisOne.ToString());
			IntPtr hConsoleOutput = GetStdHandle(-11);
			COORD Pos = new COORD();
			Pos.X = (short)x; Pos.Y = (short)y;
			uint Garbage = 0;
			ushort[,] ConvColor = new ushort[1, Stringy.Length];
			for(int i=0;i<Stringy.Length;i++)
				ConvColor[0,i] = Color;
			WriteConsoleOutputCharacter(hConsoleOutput, Stringy.ToString(), (uint)Stringy.Length, Pos, out Garbage);
			WriteConsoleAttribute(x,y,ConvColor);
		}
		
		public static void WriteConsole(int x, int y, params object[] Items)
		{
			WriteConsole(x,y,7,Items);
		}
		
		public static bool FlushConsoleInputBuffer()
		{
			return FlushConsoleInputBuffer(GetStdHandle(-10));
		}
		
		public static void WriteConsoleInput(char Tecla)
		{
			INPUT_RECORD[] Input = new INPUT_RECORD[1];
			Input[0].EventType = 1;
			Input[0].KeyEvent.UnicodeChar = Tecla;
			Input[0].KeyEvent.bKeyDown = true;
			uint Garbage=0;
			WriteConsoleInput(GetStdHandle(-10),Input,1,out Garbage);
		}
		
		public static void WriteConsoleInput(string Cadena)
		{
			char[] Teclas = Cadena.ToCharArray();
			foreach(char Tecla in Teclas)
				WriteConsoleInput(Tecla);
		}
		public static void WriteConsoleInput(ushort VirtualCode)
		{
			INPUT_RECORD[] Input = new INPUT_RECORD[1];
			Input[0].EventType = 1;
			Input[0].KeyEvent.wVirtualKeyCode = VirtualCode;
			Input[0].KeyEvent.bKeyDown = true;
			uint Garbage = 0;
			WriteConsoleInput(GetStdHandle(-10), Input, 1, out Garbage);
		}
		
		public static void SetConsoleFont(int Size)
		{
			SetConsoleFont(GetStdHandle(-11),(uint)Size);
		}

	}
}
