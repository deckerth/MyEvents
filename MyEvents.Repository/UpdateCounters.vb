Namespace Global.MyEvents.Repository

    Public Class UpdateCounters

        Public Property Added As Integer = 0
        Public Property Skipped As Integer = 0
        Public Property Updated As Integer = 0

        Public ReadOnly Property Sum As Integer
            Get
                Return Added + Skipped + Updated
            End Get
        End Property

        Public Sub Increment(result As IEventRepository.UpsertResult)
            Select Case result
                Case Repository.IEventRepository.UpsertResult.added
                    Added += 1
                Case Repository.IEventRepository.UpsertResult.skipped
                    Skipped += 1
                Case Repository.IEventRepository.UpsertResult.updated
                    Updated += 1
            End Select
        End Sub

        Public Sub Reset()
            Added = 0
            Skipped = 0
            Updated = 0
        End Sub

    End Class

End Namespace
