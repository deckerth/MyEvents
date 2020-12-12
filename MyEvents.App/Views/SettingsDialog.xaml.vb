Imports MyEvents.App.ViewModels

Namespace Global.MyEvents.App.Views

    Public NotInheritable Class SettingsDialog
        Inherits ContentDialog

        Public Property ViewModel As EventListPageViewModel

        Public Sub New()
            InitializeComponent()

            ViewModel = EventListPageViewModel.Current
            DataContext = ViewModel

            Select Case App.SelectedApplicationTheme
                Case App.ApplicationThemeDark
                    ThemeComboBox.SelectedItem = ThemeDark
                Case App.ApplicationThemeLight
                    ThemeComboBox.SelectedItem = ThemeLight
            End Select
        End Sub

        Private Async Function SettingsDialog_PrimaryButtonClick(sender As ContentDialog, args As ContentDialogButtonClickEventArgs) As Task

            Dim settings = Windows.Storage.ApplicationData.Current.LocalSettings
            Dim themeChanged As Boolean = False

            Select Case ThemeComboBox.SelectedIndex
                Case 0 'App.ApplicationThemeLight
                    If App.SelectedApplicationTheme = App.ApplicationThemeDark Then
                        settings.Values("ApplicationTheme") = App.ApplicationThemeLight
                        themeChanged = True
                    End If
                Case 1 'App.ApplicationThemeDark
                    If App.SelectedApplicationTheme = App.ApplicationThemeLight Then
                        settings.Values("ApplicationTheme") = App.ApplicationThemeDark
                        themeChanged = True
                    End If
            End Select

            If themeChanged Then
                Dim msg = New Windows.UI.Popups.MessageDialog(App.Texts.GetString("RestartAppForThemeChangeRequired"))
                Await msg.ShowAsync()
            End If

            Hide()

        End Function

        Private Async Sub ComputeIndex_Click(sender As Object, e As RoutedEventArgs) Handles ComputeIndex.Click
            ' Enforce the execution on another thread than the UI thread
            Await Task.Run(Sub() ViewModel.UpdateIndexAsync())
        End Sub

        Private Async Sub ImportDB_Click(sender As Object, e As RoutedEventArgs) Handles ImportDB.Click
            Await SettingsDialog_PrimaryButtonClick(sender, Nothing)
            Await ViewModel.OnImportDB()
        End Sub

        Private Async Sub ExportDB_Click(sender As Object, e As RoutedEventArgs) Handles ExportDB.Click
            Await SettingsDialog_PrimaryButtonClick(sender, Nothing)
            Await ViewModel.OnExportDB()
        End Sub
    End Class

End Namespace


