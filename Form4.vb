Imports System.Drawing
Imports System.Windows.Forms

Public Class Form4
    Private ToolTip1 As New ToolTip()
    Private pnright As Panel
    Private pnwrong As Panel

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Guna2TextBox1_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox1.TextChanged
        ValidateInput(Guna2TextBox1, pnright, pnwrong, ToolTip1)
    End Sub

    Private Function IsValidNumber(input As String) As Boolean
        Dim number As Decimal
        Return Decimal.TryParse(input, number)
    End Function

    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Panel1.Hide()
        Panel4.Hide()
    End Sub

    Private Sub Panel4_Paint(sender As Object, e As PaintEventArgs) Handles Panel4.Paint

    End Sub
    Private Sub ValidateInput(textBox As Guna.UI2.WinForms.Guna2TextBox, panelright As Panel, panelWrong As Panel, toolTip As ToolTip)
        If IsValidNumber(textBox.Text) Then
            panelright.Visible = True
            panelWrong.Visible = False
            toolTip.SetToolTip(panelright, "")
        Else
            panelright.Visible = False
            panelWrong.Visible = True
            toolTip.SetToolTip(panelWrong, "Invalid input. Please enter a valid number.")
        End If
    End Sub

End Class