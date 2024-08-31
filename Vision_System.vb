Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Xml.Schema
Imports Guna.UI2.WinForms
Imports System.IO
Imports System.Drawing.Imaging
Imports System.Text.RegularExpressions
Imports System.Diagnostics.Eventing.Reader




Public Class Vision_System
    Private isDrawing As Boolean = False
    Private startPoint As Point
    Private endPoint As Point
    Private shapes As New List(Of Shape)()
    Private currentID As Integer = 1
    Private currentColor As Color = Color.Blue
    Private drawSquares As Boolean = False
    Dim openFileDialog As New OpenFileDialog()
    Private xposs As String
    Private yposs As String
    Private pythonProcess As Process





    Private Sub Guna2CircleButton1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click
        isDrawing = False
        drawSquares = False
        Guna2PictureBox1.Invalidate()
        Panel2.Show()
        Panel1.Hide()
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        drawSquares = True ' Set flag to draw squares
        isDrawing = False ' Ensure we're not drawing circles
        Panel1.Show()
        Panel2.Hide()
    End Sub

    Private Sub Guna2PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles Guna2PictureBox1.MouseDown
        ' Check if the number of shapes is less than the number of rows in the DataGridView
        'If shapes.Count < Guna2DataGridView1.Rows.Count - 1 Then ' Subtract 1 to account for the new row placeholder
        If e.Button = MouseButtons.Left Then
            startPoint = e.Location
            endPoint = e.Location ' Set endPoint initially to start point
            isDrawing = True
        End If
        'Else
        'MessageBox.Show("You have reached the maximum number of shapes allowed.", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ' End If
    End Sub


    Private Sub Guna2PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles Guna2PictureBox1.MouseMove
        If isDrawing AndAlso e.Button = MouseButtons.Left Then
            endPoint = e.Location
            Guna2PictureBox1.Invalidate()
        End If
    End Sub

    Private Sub Guna2PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2PictureBox1.Paint
        Dim g As Graphics = e.Graphics
        Dim font As New System.Drawing.Font("Arial", 12)
        Dim brush As New SolidBrush(Color.Black)

        ' Draw all stored shapes
        For Each shape In shapes
            Dim pen As New Pen(shape.Color, 2)
            If TypeOf shape Is Square Then
                Dim square As Square = DirectCast(shape, Square)
                g.DrawRectangle(pen, square.TopLeft.X, square.TopLeft.Y, square.Size.Width, square.Size.Height)
                g.DrawString(square.ID.ToString(), font, brush, square.TopLeft.X, square.TopLeft.Y - 20)
            ElseIf TypeOf shape Is Circle Then
                Dim circle As Circle = DirectCast(shape, Circle)
                g.DrawEllipse(pen, circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius, circle.Radius * 2, circle.Radius * 2)
                g.DrawString(circle.ID.ToString(), font, brush, circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius - 20)
            End If
        Next

        ' Draw the current shape if drawing is in progress
        If isDrawing Then
            Dim pen As New Pen(currentColor, 2)
            If drawSquares Then
                Dim size As New Size(Math.Abs(endPoint.X - startPoint.X), Math.Abs(endPoint.Y - startPoint.Y))
                Dim rect As New Rectangle(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), size.Width, size.Height)
                g.DrawRectangle(pen, rect)
                g.DrawString(currentID.ToString(), font, brush, rect.X, rect.Y - 20)
            Else ' Draw circle
                Dim radius As Integer = CInt(Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2)) / 2)
                Dim centerX As Integer = Math.Min(startPoint.X, endPoint.X) + radius
                Dim centerY As Integer = Math.Min(startPoint.Y, endPoint.Y) + radius
                g.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2)
                g.DrawString(currentID.ToString(), font, brush, centerX - radius, centerY - radius - 20)
            End If
        End If

    End Sub
    Private Function GetMaxCurrentID() As Integer
        Dim maxID As Integer = 0
        For Each row As DataGridViewRow In Guna2DataGridView1.Rows
            If Not row.IsNewRow Then
                Dim rowID As Integer = Convert.ToInt32(row.Cells(0).Value)
                If rowID > maxID Then
                    maxID = rowID
                End If
            End If
        Next
        Return maxID
    End Function

    Private Sub Guna2PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles Guna2PictureBox1.MouseUp
        If isDrawing Then
            ' Check if the current number of shapes is less than the number of rows in the DataGridView
            ' If shapes.Count < Guna2DataGridView1.Rows.Count  Then ' Subtract 1 to account for the new row placeholder
            If drawSquares Then
                ' Draw square as before
                Dim size As New Size(Math.Abs(endPoint.X - startPoint.X), Math.Abs(endPoint.Y - startPoint.Y))
                Dim topLeft As New Point(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y))
                shapes.Add(New Square(topLeft, size, currentID, currentColor))
                AddShapeToDataGridView(currentID, "Square", topLeft.X, topLeft.Y, size.Width, size.Height)
            Else
                ' Draw circle with dynamic size
                Dim radius As Integer = CInt(Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2)) / 2)
                Dim centerX As Integer = Math.Min(startPoint.X, endPoint.X) + radius
                Dim centerY As Integer = Math.Min(startPoint.Y, endPoint.Y) + radius
                shapes.Add(New Circle(New Point(centerX, centerY), radius, currentID, currentColor))
                AddShapeToDataGridView(currentID, "Circle", centerX - radius, centerY - radius, radius * 2, 0, centerX, centerY)
            End If

            currentID += 1
            isDrawing = False
            Guna2PictureBox1.Invalidate()
        Else
            shapes.Clear()
            MessageBox.Show("You cannot draw more shapes than the number of rows in the DataGridView.", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        'End If
    End Sub



    Private Sub AddShapeToDataGridView(id As Integer, shape As String, x1 As Integer, y1 As Integer, width As Integer, Optional height As Single = 0, Optional centerX As Integer = 0, Optional centerY As Integer = 0)
        xposs = TextBox1.Text
        yposs = TextBox2.Text
        Dim centerPoint As String

        ' Variables for main and sub IDs
        Dim mainid As Integer = 1
        Dim subid As Integer = 1
        Dim TYP As String = ""
        Dim CELVALUE As String

        ' Determine the main or sub ID
        If FIDTYPE.Text = "MAIN" Then
            If Guna2DataGridView1.Rows.Count > 0 Then
                For Each row1 As DataGridViewRow In Guna2DataGridView1.Rows
                    CELVALUE = Convert.ToString(row1.Cells(0).Value)
                    If CELVALUE(0) = "M" Then
                        mainid += 1
                    End If
                Next
            End If
            TYP = "MAIN" & mainid
        ElseIf (FIDTYPE.Text = "SUB") Or (FIDTYPE.Text = "") Then
            If Guna2DataGridView1.Rows.Count > 0 Then
                For Each row1 As DataGridViewRow In Guna2DataGridView1.Rows
                    CELVALUE = Convert.ToString(row1.Cells(0).Value)
                    If CELVALUE(0) = "S" Then
                        subid += 1
                    End If
                Next
            End If
            TYP = "SUB" & subid
        End If

        ' Add row based on shape type
        If shape = "Square" Then
            Dim x2 As Integer = x1 + width
            Dim y2 As Integer = y1 + height
            Dim centerXInt As Integer = CInt(x1 + width \ 2) ' Calculate center X as integer
            Dim centerYInt As Integer = CInt(y1 + height \ 2) ' Calculate center Y as integer
            centerPoint = $"({centerXInt}, {centerYInt})"
            Dim row As String() = {TYP, shape, x1.ToString(), y1.ToString(), x2.ToString(), y2.ToString(), centerPoint, xposs, yposs}
            Guna2DataGridView1.Rows.Add(row)
        ElseIf shape = "Circle" Then
            centerPoint = $"({centerX}, {centerY})"
            Dim row As String() = {TYP, shape, x1.ToString(), y1.ToString(), width.ToString(), height.ToString(), centerPoint, xposs, yposs}
            Guna2DataGridView1.Rows.Add(row)
        Else
            Throw New ArgumentException("Invalid shape type")
        End If
    End Sub


    Private Class Shape
        Public Property ID As Integer
        Public Property Color As Color

        Public Sub New(id As Integer, color As Color)
            Me.ID = id
            Me.Color = color
        End Sub
    End Class

    Private Class Square
        Inherits Shape

        Public Property TopLeft As Point
        Public Property Size As Size

        Public Sub New(topLeft As Point, size As Size, id As Integer, color As Color)
            MyBase.New(id, color)
            Me.TopLeft = topLeft
            Me.Size = size
        End Sub
    End Class

    Private Class Circle
        Inherits Shape

        Public Property Center As Point
        Public Property Radius As Integer

        Public Sub New(center As Point, radius As Integer, id As Integer, color As Color)
            MyBase.New(id, color)
            Me.Center = center
            Me.Radius = radius
        End Sub
    End Class
    'https://www.blackbox.ai/share/09842a2d-32d5-4dcc-820a-9aaac33eb1ac
    Private Sub Guna2DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)
        ' Handle cell content click events here if needed
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Check if a row is selected
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Show a confirmation dialog
            Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            ' If the user confirms the deletion
            If result = DialogResult.Yes Then
                ' Check if TextBox3 is empty
                Dim folderName As String = TextBox3.Text.Trim()
                If String.IsNullOrEmpty(folderName) Then
                    MessageBox.Show("Please select a recipe first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Exit Sub
                End If

                ' Get the selected row index
                Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

                ' Get the S.No from the selected row
                Dim serialNumber As String = Guna2DataGridView1.Rows(rowIndex).Cells(0).Value.ToString()

                ' Define the image file path based on S.No
                Dim imagePath As String = Path.Combine("A:\Project", folderName, serialNumber & ".png")

                ' Get the selected shape ID from the DataGridView
                Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value.ToString())

                ' Find and remove the shape with the corresponding ID
                Dim shapeToRemove As Shape = shapes.FirstOrDefault(Function(s) s.ID = (rowIndex + 1))
                If shapeToRemove IsNot Nothing Then
                    shapes.Remove(shapeToRemove)
                End If

                ' Remove the row from the DataGridView
                Guna2DataGridView1.Rows.RemoveAt(rowIndex)

                ' Clear and dispose of the PictureBox images
                ClearAndDisposePictureBox(PictureBox1, serialNumber)
                ClearAndDisposePictureBox(PictureBox2, serialNumber)

                ' Try to delete the image file if it exists
                Try
                    If File.Exists(imagePath) Then
                        File.Delete(imagePath)
                    End If
                Catch ex As IOException
                    MessageBox.Show("Error deleting the file: " & ex.Message)
                End Try

                ' Update the currentID to be one more than the maximum ID in the shapes list
                If shapes.Count > 0 Then
                    currentID = shapes.Max(Function(s) s.ID) + 1
                Else
                    currentID = 1
                End If

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            End If
        Else
            MessageBox.Show("Please select a row to delete.")
        End If
    End Sub


    Private Sub ClearAndDisposePictureBox(pictureBox As PictureBox, serialNumber As String)
        ' Check if the PictureBox contains an image with the specified serial number
        If pictureBox.Image IsNot Nothing Then
            ' Check if the image file path corresponds to the serial number
            Dim imagePath As String = Path.Combine("A:\Project", TextBox3.Text.Trim(), serialNumber & ".png")
            If File.Exists(imagePath) Then
                pictureBox.Image = Nothing
                pictureBox.Invalidate()
            End If
        End If
    End Sub



    Private Sub UpdateMainAndSubIDs()
        Dim mainid As Integer = 1
        Dim subid As Integer = 1

        For Each row As DataGridViewRow In Guna2DataGridView1.Rows
            Dim CELVALUE As String = Convert.ToString(row.Cells(0).Value)
            If CELVALUE.StartsWith("MAIN") Then
                mainid = Math.Max(mainid, Integer.Parse(CELVALUE.Substring(4)) + 1)
            ElseIf CELVALUE.StartsWith("SUB") Then
                subid = Math.Max(subid, Integer.Parse(CELVALUE.Substring(3)) + 1)
            End If
        Next

        ' Update the mainid and subid variables
        If FIDTYPE.Text = "MAIN" Then
            mainid += 1
        ElseIf FIDTYPE.Text = "SUB" Then
            subid += 1
        End If
    End Sub











    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ColorDialog1.ShowDialog() = DialogResult.OK Then
            currentColor = ColorDialog1.Color
        End If
    End Sub

    Private Sub Vision_System_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        currentID = 1
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        shapes.Clear()
        Guna2PictureBox1.Invalidate()
        'Guna2DataGridView1.Rows.Clear()
        ' Guna2DataGridView1.Invalidate()
        currentID = 1
    End Sub

    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png"
        openFileDialog.Title = "Select an Image to Upload"

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            Dim selectedFilePath As String = openFileDialog.FileName
            If IO.File.Exists(selectedFilePath) Then
                Dim image As System.Drawing.Image = System.Drawing.Image.FromFile(selectedFilePath)
                Guna2PictureBox1.Image = image
            Else
                MessageBox.Show("The file does not exist.")
            End If
        End If
    End Sub
    Private Sub UpdateDataGridViewWithTextBoxValues(rowIndex As Integer)
        ' Update the DataGridView to include values from TextBox1 and TextBox2
        Guna2DataGridView1.Rows(rowIndex).Cells(7).Value = TextBox1.Text
        Guna2DataGridView1.Rows(rowIndex).Cells(8).Value = TextBox2.Text
    End Sub
    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If square IsNot Nothing Then
                ' Increase the boundary from the top
                Dim increaseAmount As Integer = 2 ' Amount by which the boundary should be increased
                square.TopLeft = New Point(square.TopLeft.X, square.TopLeft.Y - increaseAmount)
                square.Size = New Size(square.Size.Width, square.Size.Height + increaseAmount)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(3).Value = square.TopLeft.Y ' Update the Y1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(5).Value = square.TopLeft.Y + square.Size.Height ' Update the Y2 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point
                UpdateDataGridViewWithTextBoxValues(rowIndex)
                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button5_Click(sender As Object, e As EventArgs) Handles Guna2Button5.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If square IsNot Nothing Then
                ' Increase the boundary from the bottom
                Dim increaseAmount As Integer = 2 ' Amount by which the boundary should be increased
                square.Size = New Size(square.Size.Width, square.Size.Height + increaseAmount)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(5).Value = square.TopLeft.Y + square.Size.Height ' Update the Y2 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button6_Click(sender As Object, e As EventArgs) Handles Guna2Button6.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If square IsNot Nothing Then
                ' Increase the boundary from the right
                Dim increaseAmount As Integer = 2 ' Amount by which the boundary should be increased
                square.Size = New Size(square.Size.Width + increaseAmount, square.Size.Height)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(4).Value = square.TopLeft.X + square.Size.Width ' Update the X2 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button7_Click(sender As Object, e As EventArgs) Handles Guna2Button7.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If square IsNot Nothing Then
                ' Increase the boundary from the left
                Dim increaseAmount As Integer = 2 ' Amount by which the boundary should be increased
                square.TopLeft = New Point(square.TopLeft.X - increaseAmount, square.TopLeft.Y)
                square.Size = New Size(square.Size.Width + increaseAmount, square.Size.Height)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(2).Value = square.TopLeft.X ' Update the X1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(4).Value = square.TopLeft.X + square.Size.Width ' Update the X2 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button8_Click(sender As Object, e As EventArgs) Handles Guna2Button8.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If square IsNot Nothing Then
                ' Contract the boundary from the top
                Dim contractAmount As Integer = 2 ' Amount by which the boundary should be contracted
                If square.Size.Height > contractAmount Then ' Ensure the height doesn't go negative
                    square.TopLeft = New Point(square.TopLeft.X, square.TopLeft.Y + contractAmount)
                    square.Size = New Size(square.Size.Width, square.Size.Height - contractAmount)

                    ' Update the DataGridView to reflect the changes
                    Guna2DataGridView1.Rows(rowIndex).Cells(3).Value = square.TopLeft.Y ' Update the Y1 value
                    Guna2DataGridView1.Rows(rowIndex).Cells(5).Value = square.TopLeft.Y + square.Size.Height ' Update the Y2 value
                    Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                    ' Redraw PictureBox
                    Guna2PictureBox1.Invalidate()
                Else
                    MessageBox.Show("Cannot contract square further.")
                End If
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button9_Click(sender As Object, e As EventArgs) Handles Guna2Button9.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If square IsNot Nothing Then
                ' Contract the boundary from the bottom
                Dim contractAmount As Integer = 2 ' Amount by which the boundary should be contracted
                If square.Size.Height > contractAmount Then ' Ensure the height doesn't go negative
                    square.Size = New Size(square.Size.Width, square.Size.Height - contractAmount)

                    ' Update the DataGridView to reflect the changes
                    Guna2DataGridView1.Rows(rowIndex).Cells(5).Value = square.TopLeft.Y + square.Size.Height ' Update the Y2 value
                    Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                    ' Redraw PictureBox
                    Guna2PictureBox1.Invalidate()
                Else
                    MessageBox.Show("Cannot contract square further.")
                End If
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button10_Click(sender As Object, e As EventArgs) Handles Guna2Button10.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If square IsNot Nothing Then
                ' Contract the boundary from the right
                Dim contractAmount As Integer = 2 ' Amount by which the boundary should be contracted
                If square.Size.Width > contractAmount Then ' Ensure the width doesn't go negative
                    square.Size = New Size(square.Size.Width - contractAmount, square.Size.Height)

                    ' Update the DataGridView to reflect the changes
                    Guna2DataGridView1.Rows(rowIndex).Cells(4).Value = square.TopLeft.X + square.Size.Width ' Update the X2 value
                    Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                    ' Redraw PictureBox
                    Guna2PictureBox1.Invalidate()
                Else
                    MessageBox.Show("Cannot contract square further.")
                End If
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button11_Click(sender As Object, e As EventArgs) Handles Guna2Button11.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If square IsNot Nothing Then
                ' Contract the boundary from the left
                Dim contractAmount As Integer = 2 ' Amount by which the boundary should be contracted
                If square.Size.Width > contractAmount Then ' Ensure the width doesn't go negative
                    square.TopLeft = New Point(square.TopLeft.X + contractAmount, square.TopLeft.Y)
                    square.Size = New Size(square.Size.Width - contractAmount, square.Size.Height)

                    ' Update the DataGridView to reflect the changes
                    Guna2DataGridView1.Rows(rowIndex).Cells(2).Value = square.TopLeft.X ' Update the X1 value
                    Guna2DataGridView1.Rows(rowIndex).Cells(4).Value = square.TopLeft.X + square.Size.Width ' Update the X2 value
                    Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point

                    ' Redraw PictureBox
                    Guna2PictureBox1.Invalidate()
                Else
                    MessageBox.Show("Cannot contract square further.")
                End If
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button12_Click(sender As Object, e As EventArgs) Handles Guna2Button12.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))
            If circle IsNot Nothing Then
                Dim moveAmount As Integer = 2 ' Amount by which the circle should be moved
                circle.Center = New Point(circle.Center.X, circle.Center.Y - moveAmount)
                Guna2DataGridView1.Rows(rowIndex).Cells(3).Value = circle.Center.Y - circle.Radius ' Update the Y1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({circle.Center.X}, {circle.Center.Y})" ' Update the center point
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button15_Click(sender As Object, e As EventArgs) Handles Guna2Button15.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then

            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If circle IsNot Nothing Then
                ' Move the circle downwards
                Dim moveAmount As Integer = 2
                circle.Center = New Point(circle.Center.X, circle.Center.Y + moveAmount)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(3).Value = circle.Center.Y - circle.Radius ' Update the Y1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({circle.Center.X}, {circle.Center.Y})" ' Update the center point

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button13_Click(sender As Object, e As EventArgs) Handles Guna2Button13.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If circle IsNot Nothing Then
                ' Move the circle to the left
                Dim moveAmount As Integer = 2 ' Amount by which the circle should be moved
                circle.Center = New Point(circle.Center.X - moveAmount, circle.Center.Y)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(2).Value = circle.Center.X - circle.Radius ' Update the X1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({circle.Center.X}, {circle.Center.Y})" ' Update the center point

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button14_Click(sender As Object, e As EventArgs) Handles Guna2Button14.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If circle IsNot Nothing Then
                ' Move the circle to the left
                Dim moveAmount As Integer = 2 ' Amount by which the circle should be moved
                circle.Center = New Point(circle.Center.X + moveAmount, circle.Center.Y)

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(2).Value = circle.Center.X - circle.Radius ' Update the X1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(6).Value = $"({circle.Center.X}, {circle.Center.Y})" ' Update the center point

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button16_Click(sender As Object, e As EventArgs) Handles Guna2Button16.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If circle IsNot Nothing Then
                ' Increase the size of the circle
                Dim increaseAmount As Integer = 2 ' Amount by which the radius should be increased
                circle.Radius += increaseAmount

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(2).Value = circle.Center.X - circle.Radius ' Update the X1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(3).Value = circle.Center.Y - circle.Radius ' Update the Y1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(4).Value = circle.Radius * 2 ' Update the width (diameter)

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button17_Click(sender As Object, e As EventArgs) Handles Guna2Button17.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = (Guna2DataGridView1.Rows(rowIndex).Cells(0).Value)

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If circle IsNot Nothing Then
                ' Increase the size of the circle
                Dim increaseAmount As Integer = 2 ' Amount by which the radius should be increased
                circle.Radius -= increaseAmount

                ' Update the DataGridView to reflect the changes
                Guna2DataGridView1.Rows(rowIndex).Cells(2).Value = circle.Center.X - circle.Radius ' Update the X1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(3).Value = circle.Center.Y - circle.Radius ' Update the Y1 value
                Guna2DataGridView1.Rows(rowIndex).Cells(4).Value = circle.Radius * 2 ' Update the width (diameter)

                ' Redraw PictureBox
                Guna2PictureBox1.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If

    End Sub

    Private Sub Guna2PictureBox1_Click(sender As Object, e As EventArgs) Handles Guna2PictureBox1.Click

    End Sub
    Private Sub SaveImage(shapeToSave As Shape, folderPath As String, imageName As String)
        ' Determine bounding box
        Dim boundingBox As Rectangle
        If TypeOf shapeToSave Is Square Then
            Dim square As Square = CType(shapeToSave, Square)
            boundingBox = New Rectangle(square.TopLeft, square.Size)
        ElseIf TypeOf shapeToSave Is Circle Then
            Dim circle As Circle = CType(shapeToSave, Circle)
            boundingBox = New Rectangle(circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius, circle.Radius * 2, circle.Radius * 2)
        Else
            MsgBox("Unknown shape type.")
            Return
        End If

        ' Create a bitmap and draw the image
        Dim bitmap As New Bitmap(boundingBox.Width, boundingBox.Height)
        Using g As Graphics = Graphics.FromImage(bitmap)
            Dim sourceRect As New Rectangle(boundingBox.Location, boundingBox.Size)
            Dim destRect As New Rectangle(0, 0, boundingBox.Width, boundingBox.Height)
            If Guna2PictureBox1.Image IsNot Nothing Then
                g.DrawImage(Guna2PictureBox1.Image, destRect, sourceRect, GraphicsUnit.Pixel)
            Else
                MsgBox("Live camera feed is not available.")
                Return
            End If
        End Using

        ' Ensure the directory exists
        If Not Directory.Exists(folderPath) Then Directory.CreateDirectory(folderPath)

        ' Define the save path
        Dim savePath As String = Path.Combine(folderPath, imageName)

        ' Save the bitmap to the specified path
        Try
            bitmap.Save(savePath, System.Drawing.Imaging.ImageFormat.Png)
            MsgBox("Image saved successfully to " & savePath)
        Catch ex As Exception
            MsgBox("Error saving the image: " & ex.Message)
        End Try
    End Sub





    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ' Check if any row is selected in the DataGridView
        If Guna2DataGridView1.SelectedRows.Count = 0 Then
            MsgBox("Please select a shape from the DataGridView.")
            Return
        End If

        ' Get the selected row index
        Dim rowIndex As Integer = Guna2DataGridView1.SelectedRows(0).Index

        ' Get the corresponding S.No from the selected row
        Dim selectedSNo As String = Guna2DataGridView1.Rows(rowIndex).Cells(0).Value.ToString()

        ' Get the corresponding shape ID
        Dim selectedShapeID As String = Guna2DataGridView1.Rows(rowIndex).Cells(0).Value.ToString()

        ' Find the shape with the corresponding ID
        Dim shapeToSave As Shape = shapes.FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

        If shapeToSave Is Nothing Then
            MsgBox("Selected shape not found.")
            Return
        End If

        ' Determine the bounding box of the shape
        Dim boundingBox As Rectangle
        If TypeOf shapeToSave Is Square Then
            Dim square As Square = CType(shapeToSave, Square)
            boundingBox = New Rectangle(square.TopLeft, square.Size)
        ElseIf TypeOf shapeToSave Is Circle Then
            Dim circle As Circle = CType(shapeToSave, Circle)
            boundingBox = New Rectangle(circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius, circle.Radius * 2, circle.Radius * 2)
        Else
            MsgBox("Unknown shape type.")
            Return
        End If

        ' Create a new bitmap with the size of the bounding box
        Dim bitmap As New Bitmap(boundingBox.Width, boundingBox.Height)

        ' Draw the relevant portion of the live camera PictureBox image onto the new bitmap
        Using g As Graphics = Graphics.FromImage(bitmap)
            ' Adjust the source rectangle to match the shape's bounding box within the PictureBox
            Dim sourceRect As New Rectangle(boundingBox.Location, boundingBox.Size)
            Dim destRect As New Rectangle(0, 0, boundingBox.Width, boundingBox.Height)

            ' Check if the live camera feed image is available
            If Guna2PictureBox1.Image IsNot Nothing Then
                ' Draw the image from Guna2PictureBox1, which contains the live camera feed
                g.DrawImage(Guna2PictureBox1.Image, destRect, sourceRect, GraphicsUnit.Pixel)
            Else
                MsgBox("Live camera feed is not available.")
                Return
            End If
        End Using

        ' Construct the directory path based on TextBox3 value
        Dim folderName As String = TextBox3.Text.Trim()
        Dim directoryPath As String

        If String.IsNullOrEmpty(folderName) Then
            ' If TextBox3 is empty, use the default path
            directoryPath = "A:\Project\Visual Path"
        Else
            ' Use the folder name from TextBox3
            directoryPath = Path.Combine("A:\Project", folderName)

            ' Create the directory if it doesn't exist
            If Not Directory.Exists(directoryPath) Then
                Directory.CreateDirectory(directoryPath)
            End If
        End If

        ' Save the new bitmap to the specified path with the S.No as filename
        Dim savePath As String = Path.Combine(directoryPath, selectedSNo & ".png")
        Try
            bitmap.Save(savePath, System.Drawing.Imaging.ImageFormat.Png)
            ' MsgBox("Image saved successfully to " & savePath)
        Catch ex As Exception
            MsgBox("Error saving the image: " & ex.Message)
        End Try
        Try
            Dim folderPath As String = "A:\2"
            Dim imageName As String = "image.png"

            ' Call the SaveImage function
            SaveImage(shapeToSave, folderPath, imageName)
        Catch ex As Exception

        End Try
        Try
            Guna2Button19.PerformClick()
        Catch ex As Exception

        End Try
        Try
            Guna2Button18.PerformClick()
        Catch ex As Exception

        End Try
    End Sub





    Private Sub Guna2PictureBox2_Click(sender As Object, e As EventArgs)

    End Sub



    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged

    End Sub

    Private Sub Guna2Button18_Click(sender As Object, e As EventArgs)

    End Sub

    Private imageLoadCount As Integer = 0 ' To keep track of how many images have been loaded

    Private Sub Guna2DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellDoubleClick, Guna2DataGridView1.CellContentClick
        ' Check if a valid row was clicked
        If e.RowIndex < 0 Then
            Exit Sub
        End If

        ' Check if TextBox3 is empty
        Dim folderName As String = TextBox3.Text.Trim()
        If String.IsNullOrEmpty(folderName) Then
            MessageBox.Show("Please select a recipe first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' Get the S.No from the clicked row
        Dim selectedRow As DataGridViewRow = Guna2DataGridView1.Rows(e.RowIndex)
        Dim serialNumber As String = selectedRow.Cells(0).Value.ToString()

        ' Determine the directory path based on TextBox3 value
        Dim directoryPath As String = Path.Combine("A:\Project", folderName)

        ' Define the image file path based on S.No
        Dim imagePath As String = Path.Combine(directoryPath, serialNumber & ".png")

        ' Load the image into the PictureBox
        If imageLoadCount = 0 Then
            If File.Exists(imagePath) Then
                PictureBox1.Image = System.Drawing.Image.FromFile(imagePath)
                imageLoadCount += 1
            Else
                MessageBox.Show("Image not found for S.No " & serialNumber, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        ElseIf imageLoadCount = 1 Then
            Dim result As DialogResult = MessageBox.Show("Do you want to load the second image?", "Confirmation", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                If File.Exists(imagePath) Then
                    PictureBox2.Image = System.Drawing.Image.FromFile(imagePath)
                    imageLoadCount += 1
                Else
                    MessageBox.Show("Image not found for S.No " & serialNumber, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        ElseIf imageLoadCount >= 2 Then
            Dim resetResult As DialogResult = MessageBox.Show("You have loaded two images. Do you want to reset?", "Reset", MessageBoxButtons.YesNo)
            If resetResult = DialogResult.Yes Then
                PictureBox1.Image = Nothing
                PictureBox2.Image = Nothing
                imageLoadCount = 0 ' Reset the counter
            End If
        End If
    End Sub


    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged

    End Sub

    Private Sub Guna2CircleButton2_Click(sender As Object, e As EventArgs)

    End Sub




    Private Sub Guna2Button18_Click_1(sender As Object, e As EventArgs) Handles Guna2Button18.Click
        If Guna2PictureBox1.Image IsNot Nothing Then
            Try
                ' Define the save path
                Dim savePath As String = "A:\1\image.png"

                ' Save the image in the specified path with the .png extension
                Guna2PictureBox1.Image.Save(savePath, ImageFormat.Png)

                ' MessageBox.Show("Image saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Failed to save the image. Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("No image to save.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub Guna2Button19_Click(sender As Object, e As EventArgs) Handles Guna2Button19.Click
        ' Check if a row is selected
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row
            Dim selectedRow As DataGridViewRow = Guna2DataGridView1.SelectedRows(0)

            ' Extract the value from column 6
            Dim column6Value As String = selectedRow.Cells(6).Value.ToString()

            ' Extract the name for the text file from the selected row (e.g., from column 1)
            Dim fileName As String = "text" & ".txt"

            ' Define the file path
            Dim filePath As String = "A:\3\" & fileName

            ' Write the value to the text file
            Try
                System.IO.File.WriteAllText(filePath, column6Value)
                'MessageBox.Show("Value stored in " & filePath)
            Catch ex As Exception
                MessageBox.Show("Error writing to file: " & ex.Message)
            End Try
        Else
            MessageBox.Show("Please select a row first.")
        End If
    End Sub

    Public Sub RunPythonScript()
        Dim pythonPath As String = "C:/Users/adity/AppData/Local/Programs/Python/Python312/python.exe"
        Dim scriptPath As String = "A:\S Sir\5.py"
        Dim imagePath As String = String.Empty ' Update if needed

        Dim startInfo As New ProcessStartInfo(pythonPath)
        startInfo.Arguments = """" & scriptPath & """ " & imagePath
        startInfo.UseShellExecute = False
        startInfo.RedirectStandardOutput = True
        startInfo.RedirectStandardError = True

        Dim process As New Process()
        process.StartInfo = startInfo
        process.Start()

        Dim output As String = process.StandardOutput.ReadToEnd()
        Dim [error] As String = process.StandardError.ReadToEnd()

        process.WaitForExit()

        ' Refined regex pattern to handle optional negative signs
        Dim regex As New Regex("Offset: \((?<x>-?\d+), (?<y>-?\d+)\)")
        Dim match As Match = regex.Match(output)

        If match.Success Then
            ' Extract coordinates from regex groups
            Dim x As String = match.Groups("x").Value
            Dim y As String = match.Groups("y").Value

            ' Update UI with coordinates
            TextBox5.Text = $"Offset: ({x}, {y})"
        Else
            TextBox5.Text = "No offset found"
            MessageBox.Show("No Template Found")
        End If

        ' Display the output and error in the console for debugging
        Console.WriteLine("Output: " & output)
        Console.WriteLine("Error: " & [error])

        ' Optionally handle the disposed image if needed
        ' Dispose of the previous image if it exists
    End Sub



    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            Button3.PerformClick()
        Catch ex As Exception

        End Try
        Try
            RunPythonScript()
        Catch ex As Exception

        End Try
        Try
            If pythonProcess IsNot Nothing AndAlso Not pythonProcess.HasExited Then

                pythonProcess.Kill()
                pythonProcess.Dispose()
                pythonProcess = Nothing
            End If
        Catch ex As Exception

        End Try
    End Sub
    Dim normal As Color = Color.White
    Private Sub Button4_MouseEnter(sender As Object, e As EventArgs) Handles Button4.MouseEnter
        Button4.BackColor = Color.Green
    End Sub

    Private Sub Button4_MouseLeave(sender As Object, e As EventArgs) Handles Button4.MouseLeave
        Button4.BackColor = normal
        Button4.TextAlign = ContentAlignment.MiddleCenter

    End Sub

    Private Sub Button4_MouseHover(sender As Object, e As EventArgs) Handles Button4.MouseHover
        Button4.TextAlign = ContentAlignment.MiddleLeft
    End Sub

    Private Sub Guna2Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel1.Paint

    End Sub
    Private Sub RedrawShapesFromDataGridView()
        ' Clear the current shapes list
        shapes.Clear()

        ' Iterate through each row in the DataGridView
        For Each row As DataGridViewRow In Guna2DataGridView1.Rows
            If Not row.IsNewRow Then
                ' Extract the numeric part of the ID, considering "SUB" or "MAIN" prefixes
                Dim idString As String = row.Cells(0).Value.ToString()
                Dim id As Integer = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(idString, "\d+").Value)

                Dim shapeType As String = row.Cells(1).Value.ToString()
                Dim x1 As Integer = Convert.ToInt32(row.Cells(2).Value)
                Dim y1 As Integer = Convert.ToInt32(row.Cells(3).Value)
                Dim x2 As Integer = Convert.ToInt32(row.Cells(4).Value)
                Dim y2 As Integer = Convert.ToInt32(row.Cells(5).Value)
                Dim centerPoint As String = row.Cells(6).Value.ToString()
                Dim color As Color = currentColor ' Use current color or set a specific one

                If shapeType = "Square" Then
                    Dim width As Integer = x2 - x1
                    Dim height As Integer = y2 - y1
                    Dim rect As New Rectangle(x1, y1, width, height)
                    shapes.Add(New Square(rect.Location, rect.Size, id, color))
                ElseIf shapeType = "Circle" Then
                    ' Extract radius from the X2 column, Y2 is unused and assumed to be 0
                    Dim radius As Integer = Convert.ToInt32(x2)

                    ' Parse the center point from Center Point column string "(x, y)"
                    Dim center As New Point
                    Dim match As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(centerPoint, "\((\d+),\s*(\d+)\)")
                    If match.Success Then
                        center.X = Convert.ToInt32(match.Groups(1).Value)
                        center.Y = Convert.ToInt32(match.Groups(2).Value)
                    End If

                    shapes.Add(New Circle(center, radius, id, color))
                End If
            End If
        Next

        ' Invalidate the PictureBox to trigger a redraw
        Guna2PictureBox1.Invalidate()
    End Sub






    Private Sub Guna2Button20_Click(sender As Object, e As EventArgs) Handles Guna2Button20.Click
        RedrawShapesFromDataGridView()
    End Sub

    Private Sub Panel4_Paint(sender As Object, e As PaintEventArgs) Handles Panel4.Paint

    End Sub
End Class

