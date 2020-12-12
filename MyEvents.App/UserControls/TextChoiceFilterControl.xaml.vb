Imports Telerik.Data.Core

Namespace Global.MyEvents.App.UserControls
    Public NotInheritable Class TextChoiceFilterControl
        Inherits BaseFilterControl

        Public Property ViewModel As TextChoiceFilter

        Private filterDescriptor As DelegateFilterDescriptor

        Public Sub New(filter As DelegateFilterDescriptor, referredProperty As String)
            InitializeComponent()
            PropertyName = referredProperty
            If filter Is Nothing Then
                Select Case PropertyName
                    Case "Composer"
                        filterDescriptor = New DelegateFilterDescriptor With {.Filter = New ComposerChoiceFilter()}
                    Case "Soloist"
                        filterDescriptor = New DelegateFilterDescriptor With {.Filter = New SoloistChoiceFilter()}
                    Case "Director"
                        filterDescriptor = New DelegateFilterDescriptor With {.Filter = New DirectorChoiceFilter()}
                    Case "Venue"
                        filterDescriptor = New DelegateFilterDescriptor With {.Filter = New VenueChoiceFilter()}
                    Case "Contributors"
                        filterDescriptor = New DelegateFilterDescriptor With {.Filter = New PerformerChoiceFilter()}
                    Case "EventTypeDescriptor"
                        filterDescriptor = New DelegateFilterDescriptor With {.Filter = New EventTypeChoiceFilter()}
                    Case "PerformanceDate"
                        filterDescriptor = New DelegateFilterDescriptor With {.Filter = New PerformanceYearChoiceFilter()}
                End Select
            Else
                filterDescriptor = filter
            End If
            ViewModel = DirectCast(filterDescriptor.Filter, TextChoiceFilter)

        End Sub

        Protected Overrides Sub Initialize()
            ' This method Is called just before the control Is vizualized.
            ' Here you can put additional logic that will be executed
            ' before the initialization of the control. 
        End Sub

        Public Overrides Function BuildDescriptor() As FilterDescriptorBase
            Return filterDescriptor
        End Function

        Private Sub TextPatternBox_TextChanged(sender As AutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)
            ViewModel.ApplyTextPattern(sender.Text)
        End Sub
    End Class

End Namespace
