Imports Microsoft.Toolkit.Uwp.UI

Namespace Global.MyEvents.App.UserControls

    Public NotInheritable Class TokenizedInputField
        Inherits UserControl
        Implements INotifyPropertyChanged

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Private Sub OnPropertyChanged(ByVal PropertyName As String)
            ' Raise the event, and make this procedure
            ' overridable, should someone want to inherit from
            ' this class and override this behavior:
            Dim e = New PropertyChangedEventArgs(PropertyName)
            RaiseEvent PropertyChanged(Me, e)
        End Sub

        Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register("Text",
                           GetType(String), GetType(TokenizedInputField), New PropertyMetadata(""))

        Public Property Text As String
            Get
                Return DirectCast(GetValue(TextProperty), String)
            End Get
            Set(value As String)
                If Not value.Equals(Text) Then
                    SetValue(TextProperty, value)
                    InitializeItems()
                    OnPropertyChanged("Text")
                End If
            End Set
        End Property

        Public Shared ReadOnly PlaceholderTextProperty As DependencyProperty = DependencyProperty.Register("PlaceholderText",
                           GetType(String), GetType(TokenizedInputField), New PropertyMetadata(""))

        Public Property PlaceholderText As String
            Get
                Return DirectCast(GetValue(PlaceholderTextProperty), String)
            End Get
            Set(value As String)
                SetValue(PlaceholderTextProperty, value)
            End Set
        End Property

        Public Shared ReadOnly HeaderProperty As DependencyProperty = DependencyProperty.Register("Header",
                           GetType(String), GetType(TokenizedInputField), New PropertyMetadata(""))

        Public Property Header As String
            Get
                Return DirectCast(GetValue(HeaderProperty), String)
            End Get
            Set(value As String)
                SetValue(HeaderProperty, value)
            End Set
        End Property

        Public Shared ReadOnly DelimiterProperty As DependencyProperty = DependencyProperty.Register("Delimiter",
                           GetType(String), GetType(TokenizedInputField), New PropertyMetadata(", "))

        Public Property Delimiter As String
            Get
                Return DirectCast(GetValue(DelimiterProperty), String)
            End Get
            Set(value As String)
                SetValue(DelimiterProperty, value)
                TrimmedDelimiter = value.Trim()
            End Set
        End Property

        Private _trimmedDelimiter As String = ","
        Public Property TrimmedDelimiter As String
            Get
                Return _trimmedDelimiter
            End Get
            Set(value As String)
                If Not value.Equals(_trimmedDelimiter) Then
                    _trimmedDelimiter = value
                    OnPropertyChanged("TrimmedDelimiter")
                End If
            End Set
        End Property

        Public Shared ReadOnly QueryIconProperty As DependencyProperty = DependencyProperty.Register("QueryIcon",
                           GetType(IconSource), GetType(TokenizedInputField), New PropertyMetadata(""))

        Public Property QueryIcon As IconSource
            Get
                Return DirectCast(GetValue(QueryIconProperty), IconSource)
            End Get
            Set(value As IconSource)
                SetValue(QueryIconProperty, value)
            End Set
        End Property

        Public Shared ReadOnly SuggestionsProperty As DependencyProperty = DependencyProperty.Register("Suggestions",
                           GetType(List(Of Token)), GetType(TokenizedInputField), Nothing)

        Public Property Suggestions As List(Of Token)
            Get
                Return DirectCast(GetValue(SuggestionsProperty), List(Of Token))
            End Get
            Set(value As List(Of Token))
                SetValue(SuggestionsProperty, value)
                InitializeSuggestedTokens()
            End Set
        End Property

        Public Property Items As New ObservableCollection(Of Token)

        Public Property CurrentInput As String = ""

        Private Sub InitializeItems()
            Dim tokens = Text.Split(TrimmedDelimiter, StringSplitOptions.RemoveEmptyEntries)
            Items.Clear()
            For Each t In tokens
                Items.Add(New Token With {.Text = t.Trim()})
            Next
        End Sub

        Public Property SuggestedTokens As AdvancedCollectionView

        Private Sub InitializeSuggestedTokens()
            SuggestedTokens = New AdvancedCollectionView(Suggestions) With {
            .Filter = Function(item As Object) As Boolean
                          Dim toCheck = DirectCast(item, Token)
                          If CurrentInput Is Nothing OrElse CurrentInput.Length = 0 Then
                              Return False
                          End If
                          Return toCheck.Text.Contains(CurrentInput, StringComparison.InvariantCultureIgnoreCase)
                      End Function
        }
            SuggestedTokens.SortDescriptions.Add(New SortDescription("Text", SortDirection.Ascending))
        End Sub

        Private Sub TokenBox_TextChanged(sender As AutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs) Handles TokenBox.TextChanged
            If args.CheckCurrent() AndAlso args.Reason = AutoSuggestionBoxTextChangeReason.UserInput Then
                SuggestedTokens.RefreshFilter()
            End If
        End Sub

        Private Sub UpdateText()
            Dim buffer = ""
            For Each i In TokenBox.Items
                Dim t As Token = TryCast(i, Token)
                If t IsNot Nothing Then
                    If Not String.IsNullOrEmpty(buffer) Then
                        buffer += Delimiter
                    End If
                    buffer += t.Text
                End If
            Next
            Text = buffer
        End Sub

        Private Sub TokenBox_TokenItemAdded(sender As Controls.TokenizingTextBox, args As Object) Handles TokenBox.TokenItemAdded
            UpdateText()
        End Sub

        Private Sub TokenBox_TokenItemRemoved(sender As Controls.TokenizingTextBox, args As Object) Handles TokenBox.TokenItemRemoved
            UpdateText()
        End Sub

        Private Sub TokenBox_TokenItemAdding(sender As Controls.TokenizingTextBox, args As Controls.TokenItemAddingEventArgs)
            args.Item = New Token With {.Text = args.TokenText}
        End Sub
    End Class

End Namespace
