using System;
using IrrKlang;

//namespace Tetris
//{
//    public static class Sonidos
//    {
//        [DllImport("WinMM.dll")]
//        public static extern bool PlaySound(string fname, int Mod, int flag);

//        const int LOOP = 0x0008;
//        const int ASYNC = 0x0001;
//        const int SYNC = 0x0000;

//        static bool sounds_on = true;

//        public static bool Sounds_ON
//        {
//            get { return Sonidos.sounds_on; }
//            set { Sonidos.sounds_on = value; }
//        }

//        static int currentBGM = new Random().Next(0,3);

//        public static int CurrentBGM
//        {
//            get { return Sonidos.currentBGM; }
//            set { Sonidos.currentBGM = value; }
//        }

//        static string Directorio = Directory.GetCurrentDirectory()+"\\Sound\\";

//        static string[] BGM = {"Technotris.wav","Kalinka.wav","Troika.wav"};

//        static string[] Sonido = {	"Topa.wav",
//                                    "Rotate.wav",
//                                    "Pause.wav",
//                                    "Posicionada.wav",
//                                    "Lose 1.wav",
//                                    "Lose 2.wav",
//                                    "Single.wav",
//                                    "Double.wav",
//                                    "Triple.wav",
//                                    "Tetris.wav",
//                                    "Linea Cae.wav"	};

//        public enum Sounds {Topa,
//                            Rotar,
//                            Pausa,
//                            Posicionada,
//                            Pierde_1,
//                            Pierde_2,
//                            Single,
//                            Double,
//                            Triple,
//                            Tetris,
//                            Linea_Cae	};

//        public static void PlayBGM()
//        {
//            if (currentBGM != 3)
//                PlaySound(Directorio + BGM[currentBGM],0, ASYNC);
//        }

//        public static void BGMVolume(bool Up)
//        {
//            //if(Up && IBGM.SoundVolume < 1.0f)
//            //    IBGM.SoundVolume+=0.01f;
//            //else if(!Up && IBGM.SoundVolume > 0.01f)
//            //    IBGM.SoundVolume-= 0.01f;
//        }

//        public static void ChangeBGM()
//        {
//            //if (currentBGM != 3)
//            //    IBGM.Play2D(Directory + BGM[currentBGM], true,true);
//        }

//        public static void PauseBGM()
//        {
//            //if (currentBGM != 3)
//            //    IBGM.SetAllSoundsPaused(true);
//        }

//        public static void ContinueBGM()
//        {
//            //if (currentBGM != 3)
//            //    IBGM.SetAllSoundsPaused(false);
//        }

//        public static void StopBGM()
//        {
//            //IBGM.StopAllSounds();
//        }

//        public static void PlaySound(Sounds SonidoATocar)
//        {
//            //if(sounds_on)
//            //    ISounds.Play2D(Directorio+Sonido[(int)SonidoATocar],false);
//        }

//        public static void SoundsVolume(bool Up)
//        {
//            //if (Up && ISounds.SoundVolume < 1.0f)
//            //    ISounds.SoundVolume += 0.01f;
//            //else if (!Up && ISounds.SoundVolume > 0.01f)
//            //    ISounds.SoundVolume -= 0.01f;
//        }

//        public static float ReturnVolume(int WichOne)
//        {
//            //if(WichOne == 0)
//            //    return IBGM.SoundVolume*100f;
//            //else
//            //    return ISounds.SoundVolume*100f;
//            return 0;
//        }

//    }
//}


namespace Tetris
{
    public static class Sonidos
    {
        static ISoundEngine ISounds = new ISoundEngine();
        static ISoundEngine IBGM = new ISoundEngine();

        static bool sounds_on = true;

        public static bool Sounds_ON
        {
            get { return sounds_on; }
            set { sounds_on = value; }
        }

        static int currentBGM = new Random().Next(0, 3);

        public static int CurrentBGM
        {
            get { return currentBGM; }
            set { currentBGM = value; }
        }

        const string Directory = "Sound/";

        static string[] BGM = { "Technotris.wav", "Kalinka.wav", "Troika.wav" };

        static string[] Sonido = {	"Topa.wav",
									"Rotate.wav",
									"Pause.wav",
									"Posicionada.wav",
									"Lose 1.wav",
									"Lose 2.wav",
									"Single.wav",
									"Double.wav",
									"Triple.wav",
									"Tetris.wav",
									"Linea Cae.wav"	};

        public enum Sounds
        {
            Topa,
            Rotar,
            Pausa,
            Posicionada,
            Pierde_1,
            Pierde_2,
            Single,
            Double,
            Triple,
            Tetris,
            Linea_Cae
        };

        public static void PlayBGM()
        {
            if (currentBGM != 3)
                IBGM.Play2D(Directory + BGM[currentBGM], true);
        }

        public static void BGMVolume(bool Up)
        {
            if (Up && IBGM.SoundVolume < 1.0f)
                IBGM.SoundVolume += 0.01f;
            else if (!Up && IBGM.SoundVolume > 0.01f)
                IBGM.SoundVolume -= 0.01f;
        }

        public static void ChangeBGM()
        {
            if (currentBGM != 3)
                IBGM.Play2D(Directory + BGM[currentBGM], true, true);
        }

        public static void PauseBGM()
        {
            if (currentBGM != 3)
                IBGM.SetAllSoundsPaused(true);
        }

        public static void ContinueBGM()
        {
            if (currentBGM != 3)
                IBGM.SetAllSoundsPaused(false);
        }

        public static void StopBGM()
        {
            IBGM.StopAllSounds();
        }

        public static void PlaySound(Sounds SonidoATocar)
        {
            if (sounds_on)
                ISounds.Play2D(Directory + Sonido[(int)SonidoATocar], false);
        }

        public static void SoundsVolume(bool Up)
        {
            if (Up && ISounds.SoundVolume < 1.0f)
                ISounds.SoundVolume += 0.01f;
            else if (!Up && ISounds.SoundVolume > 0.01f)
                ISounds.SoundVolume -= 0.01f;
        }

        public static float ReturnVolume(int WichOne)
        {
            if (WichOne == 0)
                return IBGM.SoundVolume * 100f;
            else
                return ISounds.SoundVolume * 100f;
        }

    }
}