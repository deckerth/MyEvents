Namespace Global.MyEvents.Models

    Public Class Performer
        Inherits DBObject
        Implements IEquatable(Of Performer)

        Public Property Name As String

        Public Sub New()
            Name = ""
        End Sub

        Public Sub New(text As String)
            Name = text
        End Sub

        Public Function IEquatable_Equals(other As Performer) As Boolean Implements IEquatable(Of Performer).Equals
            Return Name.Equals(other.Name)
        End Function

        Public Function Clone() As Performer
            Return DirectCast(MemberwiseClone(), Performer)
        End Function

    End Class

End Namespace

