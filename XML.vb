Imports System.Xml
Imports System.IO
Imports System.Xml.Linq


Public Class XML
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Guna2DataGridView1.Rows.Add()
        Guna2DataGridView1.Rows(Guna2DataGridView1.RowCount - 2).Cells(0).Value = Guna2TextBox1.Text
        Guna2DataGridView1.Rows(Guna2DataGridView1.RowCount - 2).Cells(1).Value = Guna2TextBox2.Text
        Guna2DataGridView1.Rows(Guna2DataGridView1.RowCount - 2).Cells(2).Value = Guna2TextBox3.Text
        Guna2DataGridView1.Rows(Guna2DataGridView1.RowCount - 2).Cells(3).Value = Guna2TextBox9.Text
        ' Create a new XML document
        Dim xmlDoc As New XDocument()

        ' Create the root element
        Dim board As XElement =
     <Board>
         <txtbox1><%= Guna2TextBox1.Text %></txtbox1>
         <txtbox2><%= Guna2TextBox2.Text %></txtbox2>
         <txtbox3><%= Guna2TextBox3.Text %></txtbox3>
         <txtbox4><%= Guna2TextBox4.Text %></txtbox4>
         <txtbox5><%= Guna2TextBox5.Text %></txtbox5>
         <txtbox6><%= Guna2TextBox6.Text %></txtbox6>
         <txtbox7><%= Guna2TextBox7.Text %></txtbox7>
         <txtbox8><%= Guna2TextBox8.Text %></txtbox8>
     </Board>

        ' Add the root element to the XML document
        xmlDoc.Add(board)

        ' Save the XML document to a file with the name of Guna2TextBox9.Text
        xmlDoc.Save("A:\" & Guna2TextBox9.Text & ".xml")

    End Sub

    Private Sub Guna2DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellContentClick

    End Sub

    Private Sub Guna2TextBox1_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox1.TextChanged

    End Sub

    Private Sub Guna2TextBox2_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox2.TextChanged

    End Sub

    Private Sub Guna2TextBox3_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox3.TextChanged

    End Sub

    Private Sub Guna2TextBox4_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox4.TextChanged

    End Sub

    Private Sub Guna2TextBox5_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox5.TextChanged

    End Sub

    Private Sub Guna2TextBox6_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox6.TextChanged

    End Sub

    Private Sub Guna2TextBox7_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox7.TextChanged

    End Sub

    Private Sub Guna2TextBox8_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox8.TextChanged

    End Sub

    Private Sub Guna2TextBox9_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox9.TextChanged

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            Guna2DataGridView1.Rows.Remove(Guna2DataGridView1.SelectedRows(0))
            File.Delete("A:\" & Guna2TextBox9.Text & ".xml")
        End If
    End Sub

    Private Sub Guna2DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellDoubleClick
        Dim rowIndex As Integer = e.RowIndex

        ' Get the XML file name from the selected row
        Dim xmlFileName As String = Guna2DataGridView1.Rows(rowIndex).Cells(3).Value.ToString()

        ' Check if the file exists
        If File.Exists("A:\" & xmlFileName & ".xml") Then
            ' Load the XML file
            Dim xmlDoc As New XmlDocument()
            xmlDoc.Load("A:\" & xmlFileName & ".xml")

            ' Populate the text boxes
            Guna2TextBox1.Text = xmlDoc.SelectSingleNode("/Board/txtbox1").InnerText
            Guna2TextBox2.Text = xmlDoc.SelectSingleNode("/Board/txtbox2").InnerText
            Guna2TextBox3.Text = xmlDoc.SelectSingleNode("/Board/txtbox3").InnerText
            Guna2TextBox4.Text = xmlDoc.SelectSingleNode("/Board/txtbox4").InnerText
            Guna2TextBox5.Text = xmlDoc.SelectSingleNode("/Board/txtbox5").InnerText
            Guna2TextBox6.Text = xmlDoc.SelectSingleNode("/Board/txtbox6").InnerText
            Guna2TextBox7.Text = xmlDoc.SelectSingleNode("/Board/txtbox7").InnerText
            Guna2TextBox8.Text = xmlDoc.SelectSingleNode("/Board/txtbox8").InnerText
            Guna2TextBox9.Text = xmlFileName
        End If
    End Sub

    Private Sub XML_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class