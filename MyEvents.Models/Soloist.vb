Namespace Global.MyEvents.Models

    Public Class Soloist
        Inherits DBObject
        Implements IEquatable(Of Soloist)

        Public Property Name As String

        Public Sub New()
            Name = ""
        End Sub

        Public Sub New(text As String)
            Name = text
        End Sub

        Public Function IEquatable_Equals(other As Soloist) As Boolean Implements IEquatable(Of Soloist).Equals
            Return Name.Equals(other.Name)
        End Function

        Public Function Clone() As Soloist
            Return DirectCast(MemberwiseClone(), Soloist)
        End Function

    End Class

End Namespace

