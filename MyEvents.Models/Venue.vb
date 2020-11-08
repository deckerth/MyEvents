Namespace Global.MyEvents.Models

    Public Class Venue
        Inherits DBObject
        Implements IEquatable(Of Venue)

        Public Property Name As String

        Public Sub New()
            Name = ""
        End Sub

        Public Sub New(text As String)
            Name = text
        End Sub

        Public Function IEquatable_Equals(other As Venue) As Boolean Implements IEquatable(Of Venue).Equals
            Return Name.Equals(other.Name)
        End Function

        Public Function Clone() As Venue
            Return DirectCast(MemberwiseClone(), Venue)
        End Function

    End Class

End Namespace

