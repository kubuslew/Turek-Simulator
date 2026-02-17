using System;
using System.Windows.Forms;

namespace TurekSimulator
{
	/// <summary>
	/// Punkt wejścia aplikacji (WinForms).
	/// Odpowiada za inicjalizację ustawień renderowania oraz uruchomienie głównego okna (MenuForm).
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// Główna metoda startowa aplikacji.
		/// Ustawia styl wizualny kontrolek, tryb renderowania tekstu i pętlę menu.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MenuForm());
		}
	}
}
