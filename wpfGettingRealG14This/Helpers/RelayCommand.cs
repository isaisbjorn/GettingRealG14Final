using System.Windows.Input;

namespace wpfGettingRealG14This.Helpers
{
	// RelayCommand er en hjælpeklasse der gør det nemt at lave Commands i WPF.
	// Det er en generel klasse der kan bruges til alle knapper i hele programmet, så vi slipper for at skrive den samme kode igen og igen.
	// Det skal forståes som et mellemled der modtager en metode som WPF kan kalde, og som så kalder den metode når knappen klikkes.
    // Det er en måde at "videregive" kaldet fra WPF til vores ViewModel-metode.
	//
	// I MVVM-mønsteret bruger vi Commands i stedet for Click-events i code-behind.
	// Det holder vores ViewModels fri for UI-kode og gør dem lettere at teste og genbruge, fordi de ikke er afhængige af WPF.
	// En Command er et objekt der indeholder:
	//   1. Execute  — hvad der skal ske når man klikker
	//   2. CanExecute — om knappen overhovedet er aktiv (true/false)
	//
	// ICommand er et C# interface som WPF forstår.
	// Vi implementerer den her med to simple lambdas (Action og Func<bool>). lambdas er anonyme metoder der kan sendes rundt som variabler.
	public class RelayCommand : ICommand
    {
        // Den metode der køres når knappen klikkes
        private readonly Action _execute;

        // En metode der returnerer true/false — bruges til at aktivere/deaktivere knappen.
        // Nullable (= null) fordi ikke alle commands behøver en CanExecute-tjek.
        private readonly Func<bool> _canExecute;

        // Konstruktøren modtager de to metoder udefra (fra ViewModel)
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        // Kører executemetoden — kaldes af WPF når knappen klikkes
        public void Execute(object parameter) => _execute();

		// Returnerer om knappen er aktiv. Fx hvis CanExecute er () => SelectedClient != null,
		// så er knappen kun aktiv når SelectedClient ikke er null.
		// Hvis ingen canExecute er givet (null), er knappen altid aktiv. fx SaveNoteCommand i ClientDetailViewModel har ingen CanExecute,
		// så den er altid aktiv. Det er smart fordi SaveNoteCommand tjekker selv om der er tekst i inputfeltet.
		public bool CanExecute(object parameter) =>
            _canExecute == null || _canExecute();

		// WPF "abonnerer" på dette event for at vide hvornår den skal
		// genkalde CanExecute — CommandManager.RequerySuggested
		// affyrer det automatisk fx når en TextBox ændres.
		// CommandManager.RequerySuggested er et event WPF bruger til at spørge alle
		// commands om deres CanExecute-status, fx når der sker ændringer i UI'et.
		
		public event EventHandler CanExecuteChanged
        {
            add    { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
