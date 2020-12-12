Namespace Global.MyEvents.Models

    Public Class Composer
        Inherits DBObject
        Implements IEquatable(Of Composer)

        Public Property Name As String

        Public Sub New()
            Name = ""
        End Sub

        Public Sub New(text As String)
            Name = text
        End Sub

        Public Function IEquatable_Equals(other As Composer) As Boolean Implements IEquatable(Of Composer).Equals
            Return Name.Equals(other.Name)
        End Function

        Public Function Clone() As Composer
            Return DirectCast(MemberwiseClone(), Composer)
        End Function

    End Class

End Namespace
