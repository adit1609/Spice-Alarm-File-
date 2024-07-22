Public Class Form3
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Dim selectedValue As String = ComboBox1.SelectedItem

        If selectedValue = "1" Then
            TabPage1.Enabled = True
            TabPage2.Enabled = False
            TabPage3.Enabled = False
            TabPage4.Enabled = False
        ElseIf selectedValue = "2" Then
            TabPage1.Enabled = False
            TabPage2.Enabled = True
            TabPage3.Enabled = False
            TabPage4.Enabled = False
        ElseIf selectedValue = "3" Then
            TabPage1.Enabled = False
            TabPage2.Enabled = False
            TabPage3.Enabled = True
            TabPage4.Enabled = False
        ElseIf selectedValue = "4" Then
            TabPage1.Enabled = False
            TabPage2.Enabled = False
            TabPage3.Enabled = False
            TabPage4.Enabled = True
        End If
    End Sub

    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' You might want to populate the ComboBox here
        ComboBox1.Items.Add("1")
        ComboBox1.Items.Add("2")
        ComboBox1.Items.Add("3")
        ComboBox1.Items.Add("4")
    End Sub
End Class
