Imports System.Drawing
Imports System.Windows.Forms

Public Class Form5
    Private ToolTip1 As New ToolTip()
    Private Sub Guna2TextBox1_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox1.TextChanged
        If IsValidNumber(Guna2TextBox1.Text) Then
            pnright.Visible = True
            pnwrong.Visible = False
            ToolTip1.SetToolTip(pnright, "")
        Else
            pnright.Visible = False
            pnwrong.Visible = True
            ToolTip1.SetToolTip(pnwrong, "Invalid input. Please enter a valid number.")
        End If
    End Sub
    Private Function IsValidNumber(input As String) As Boolean
        Dim number As Decimal
        If Decimal.TryParse(input, number) Then
            Return True
        End If
        Dim doubleNumber As Double
        If Double.TryParse(input, doubleNumber) Then
            Return True
        End If
        Dim intNumber As Integer
        If Integer.TryParse(input, intNumber) Then
            Return True
        End If
        Return False
    End Function

    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        pnwrong.Hide()
    End Sub
End Class