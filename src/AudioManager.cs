using System.Media;

namespace TurekSimulator
{
	/// <summary>
	/// Centralny manager dźwięku odpowiedzialny za odtwarzanie
	/// ambientowej muzyki w tle gry.
	/// 
	/// Klasa statyczna – jedna instancja na całą aplikację,
	/// zapobiega wielokrotnemu uruchomieniu tego samego dźwięku.
	/// </summary>
	public static class AudioManager
	{
		// Odtwarzacz dźwięku systemowego (WAV)
		private static SoundPlayer _player;

		// Flaga zabezpieczająca przed wielokrotnym uruchomieniem muzyki
		private static bool _started;

		/// <summary>
		/// Uruchamia zapętloną muzykę ambientową gry.
		/// Metoda jest odporna na wielokrotne wywołania – dźwięk
		/// zostanie uruchomiony tylko raz.
		/// </summary>
		public static void StartAmbientLoop()
		{
			if (_started) return;

			_player = new SoundPlayer(Properties.Resources.Ambient);
			_player.Load();
			_player.PlayLooping();

			_started = true;
		}

		/// <summary>
		/// Zatrzymuje odtwarzanie muzyki ambientowej.
		/// Używane przy zamknięciu gry lub przejściu do menu,
		/// jeśli chcemy całkowicie wyciszyć dźwięk.
		/// </summary>
		public static void Stop()
		{
			if (_player == null) return;

			_player.Stop();
			_started = false;
		}
	}
}
